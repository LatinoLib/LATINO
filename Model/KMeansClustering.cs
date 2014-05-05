/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    KMeans.cs
 *  Desc:    K-means clustering algorithm
 *  Created: Aug-2009
 *
 *  Author:  Miha Grcar 
 * 
 *  License: MIT (http://opensource.org/licenses/MIT) 
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class KMeansClustering
       |
       '-----------------------------------------------------------------------
    */
    public class KMeansClustering : IClustering<SparseVector<double>> 
    {        
        private ISimilarity<SparseVector<double>> mSimilarity
            = CosineSimilarity.Instance;
        private Random mRnd
            = new Random();
        private CentroidType mCentroidType
            = CentroidType.NrmL2;
        private double mEps
            = 0.0005;
        private int mTrials
            = 1;
        private int mK;
        
        private Logger mLogger
            = Logger.GetLogger(typeof(KMeansClustering));

        public KMeansClustering(int k)
        {
            Utils.ThrowException(k < 2 ? new ArgumentOutOfRangeException("k") : null);
            mK = k;
        }

        public Logger Logger
        {
            get { return mLogger; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Logger") : null);
                mLogger = value;
            }
        }

        public Random Random
        {
            get { return mRnd; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Random") : null);
                mRnd = value;
            }
        }

        public ISimilarity<SparseVector<double>> Similarity
        {
            get { return mSimilarity; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Similarity") : null);
                mSimilarity = value;
            }
        }

        public CentroidType CentroidType
        {
            get { return mCentroidType; }
            set { mCentroidType = value; }
        }

        public double Eps
        {
            get { return mEps; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("Eps") : null);
                mEps = value;
            }
        }

        public int Trials
        {
            get { return mTrials; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("Trials") : null);
                mTrials = value;
            }
        }

        // *** IClustering<LblT, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }

        public ClusteringResult Cluster(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count < mK ? new ArgumentValueException("dataset") : null);
            ClusteringResult clustering = null;
            ClusteringResult bestClustering = null;
            double globalBestClustQual = 0;
            for (int trial = 1; trial <= mTrials; trial++)
            {
                mLogger.Trace("Cluster", "Clustering trial {0} of {1} ...", trial, mTrials);
                ArrayList<SparseVector<double>> centroids = null;
                clustering = new ClusteringResult();
                for (int i = 0; i < mK; i++) { clustering.AddRoot(new Cluster()); }
                // select seed items
                double minSim = double.MaxValue;
                ArrayList<int> tmp = new ArrayList<int>(dataset.Count);
                for (int i = 0; i < dataset.Count; i++) { tmp.Add(i); }
                for (int k = 0; k < 3; k++)
                {
                    ArrayList<SparseVector<double>> seeds = new ArrayList<SparseVector<double>>(mK);
                    tmp.Shuffle(mRnd);
                    for (int i = 0; i < mK; i++)
                    {
                        seeds.Add(ModelUtils.ComputeCentroid(new SparseVector<double>[] { dataset[tmp[i]] }, mCentroidType));
                    }
                    // assess quality of seed items
                    double simAvg = 0;
                    foreach (SparseVector<double> seed1 in seeds)
                    {
                        foreach (SparseVector<double> seed2 in seeds)
                        {
                            if (seed1 != seed2)
                            {
                                simAvg += mSimilarity.GetSimilarity(seed1, seed2);
                            }
                        }
                    }
                    simAvg /= (double)(mK * mK - mK);
                    if (simAvg < minSim)
                    {
                        minSim = simAvg;
                        centroids = seeds;
                    }
                }
                // main loop
                int iter = 0;
                double bestClustQual = 0;
                double clustQual;
                while (true)
                {
                    iter++;
                    mLogger.Trace("Cluster", "Iteration {0} ...", iter);
                    clustQual = 0;
                    // assign items to clusters
                    foreach (Cluster cluster in clustering.Roots) { cluster.Items.Clear(); }
                    for (int i = 0; i < dataset.Count; i++)
                    {
                        SparseVector<double> example = dataset[i];
                        double maxSim = double.MinValue;
                        ArrayList<int> candidates = new ArrayList<int>();
                        for (int j = 0; j < mK; j++)
                        {
                            SparseVector<double> centroid = centroids[j];
                            double sim = mSimilarity.GetSimilarity(example, centroid);
                            if (sim > maxSim)
                            {
                                maxSim = sim;
                                candidates.Clear();
                                candidates.Add(j);
                            }
                            else if (sim == maxSim)
                            {
                                candidates.Add(j);
                            }
                        }
                        if (candidates.Count > 1)
                        {
                            candidates.Shuffle(mRnd); 
                        }
                        clustering.Roots[candidates[0]].Items.Add(i);
                        clustQual += maxSim;
                    }
                    clustQual /= (double)dataset.Count;
                    mLogger.Trace("Cluster", "Quality: {0:0.0000}", clustQual);
                    // check if done
                    if (iter > 1 && clustQual - bestClustQual <= mEps)
                    {
                        break;
                    }
                    bestClustQual = clustQual;
                    // compute new centroids
                    for (int i = 0; i < mK; i++)
                    {
                        centroids[i] = clustering.Roots[i].ComputeCentroid(dataset, mCentroidType);
                    }
                }
                if (trial == 1 || clustQual > globalBestClustQual)
                {
                    globalBestClustQual = clustQual;
                    bestClustering = clustering;
                }
            }
            return bestClustering;
        }

        ClusteringResult IClustering.Cluster(IUnlabeledExampleCollection dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            return Cluster((IUnlabeledExampleCollection<SparseVector<double>>)dataset); // throws ArgumentValueException
        }
    }
}
