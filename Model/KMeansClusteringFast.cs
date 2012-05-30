/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    KMeansFast.cs
 *  Desc:    K-means clustering algorithm (optimized for speed)
 *  Created: Aug-2009
 *
 *  Author:  Miha Grcar 
 * 
 *  License: GNU LGPL (http://www.gnu.org/licenses/lgpl.txt)
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class KMeansClusteringFast
       |
       '-----------------------------------------------------------------------
    */
    public class KMeansClusteringFast : IClustering<SparseVector<double>> 
    {        
        private Random mRnd
            = new Random();
        private double mEps
            = 0.0005;
        private int mTrials
            = 1;
        private int mK;

        private Logger mLogger
            = Logger.GetLogger(typeof(KMeansClusteringFast));

        public KMeansClusteringFast(int k)
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
            double globalBestClustQual = 0;
            for (int trial = 1; trial <= mTrials; trial++)
            {
                mLogger.Info("Cluster", "Clustering trial {0} of {1} ...", trial, mTrials);
                ArrayList<CentroidData> centroids = new ArrayList<CentroidData>(mK);
                ArrayList<int> bestSeeds = null;
                for (int i = 0; i < mK; i++)
                {
                    centroids.Add(new CentroidData());
                }
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
                        seeds.Add(dataset[tmp[i]]);
                    }
                    // assess quality of seed items
                    double simAvg = 0;
                    foreach (SparseVector<double> seed1 in seeds)
                    {
                        foreach (SparseVector<double> seed2 in seeds)
                        {
                            if (seed1 != seed2)
                            {
                                simAvg += DotProductSimilarity.Instance.GetSimilarity(seed1, seed2);
                            }
                        }
                    }
                    simAvg /= (double)(mK * mK - mK);
                    if (simAvg < minSim)
                    {
                        minSim = simAvg;
                        bestSeeds = new ArrayList<int>(mK);
                        for (int i = 0; i < mK; i++) { bestSeeds.Add(tmp[i]); }
                    }
                }
                for (int i = 0; i < mK; i++)
                {
                    centroids[i].Items.Add(bestSeeds[i]);
                    centroids[i].Update(dataset);
                    centroids[i].UpdateCentroidLen();
                }
                double[][] dotProd = new double[mK][];
                SparseMatrix<double> dsMtx = ModelUtils.GetTransposedMatrix(dataset);
                // main loop
                int iter = 0;
                double bestClustQual = 0;
                double clustQual;
                while (true)
                {
                    iter++;
                    mLogger.Info("Cluster", "Iteration {0} ...", iter);
                    clustQual = 0;
                    // assign items to clusters
                    int j = 0;
                    foreach (CentroidData cen in centroids)
                    {
                        SparseVector<double> cenVec = cen.GetSparseVector();
                        dotProd[j] = ModelUtils.GetDotProductSimilarity(dsMtx, dataset.Count, cenVec);
                        j++;
                    }
                    for (int instIdx = 0; instIdx < dataset.Count; instIdx++)
                    {
                        double maxSim = double.MinValue;
                        ArrayList<int> candidates = new ArrayList<int>();
                        for (int cenIdx = 0; cenIdx < mK; cenIdx++)
                        {
                            double sim = dotProd[cenIdx][instIdx];
                            if (sim > maxSim)
                            {
                                maxSim = sim;
                                candidates.Clear();
                                candidates.Add(cenIdx);
                            }
                            else if (sim == maxSim)
                            {
                                candidates.Add(cenIdx);
                            }
                        }
                        if (candidates.Count > 1)
                        {
                            candidates.Shuffle(mRnd);
                        }
                        centroids[candidates[0]].Items.Add(instIdx);
                        clustQual += maxSim;
                    }
                    clustQual /= (double)dataset.Count;                    
                    mLogger.Info("Cluster", "Quality: {0:0.0000}", clustQual);
                    // check if done
                    if (iter > 1 && clustQual - bestClustQual <= mEps)
                    {
                        break;
                    }
                    bestClustQual = clustQual;
                    // compute new centroids
                    for (int i = 0; i < mK; i++)
                    {
                        centroids[i].Update(dataset);
                        centroids[i].UpdateCentroidLen();
                    }
                }
                if (trial == 1 || clustQual > globalBestClustQual)
                {
                    globalBestClustQual = clustQual;
                    // save the result
                    clustering = new ClusteringResult();
                    for (int i = 0; i < mK; i++)
                    {
                        clustering.AddRoot(new Cluster());
                        foreach (int idx in centroids[i].Items) { clustering.Roots.Last.Items.Add(new IdxDat<double>(idx)); }
                    }
                }
            }
            return clustering;
        }

        ClusteringResult IClustering.Cluster(IUnlabeledExampleCollection dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            return Cluster((IUnlabeledExampleCollection<SparseVector<double>>)dataset); // throws ArgumentValueException
        }
    }
}
