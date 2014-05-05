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
 *  License: MIT (http://opensource.org/licenses/MIT)
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
        protected Random mRnd
            = new Random();
        protected double mEps
            = 0.0005;
        protected int mTrials
            = 1;
        protected int mK;

        internal ArrayList<CentroidData> mCentroids
            = null;

        protected Logger mLogger
            = Logger.GetInstanceLogger(typeof(KMeansClusteringFast));

        public KMeansClusteringFast(int k)
        {
            Utils.ThrowException(k < 1 ? new ArgumentOutOfRangeException("k") : null);
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

        public int K
        {
            get { return mK; }
        }

        public ArrayList<SparseVector<double>> GetCentroids()
        {
            Utils.ThrowException(mCentroids == null ? new InvalidOperationException() : null);
            ArrayList<SparseVector<double>> centroids = new ArrayList<SparseVector<double>>();
            foreach (CentroidData centroid in mCentroids)
            {
                centroids.Add(centroid.GetSparseVector());
            }
            return centroids;
        }

        // *** IClustering<LblT, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }

        private ClusteringResult CreateSingleCluster(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            ClusteringResult clustering = new ClusteringResult();
            Cluster root = new Cluster();
            for (int i = 0; i < dataset.Count; i++) { root.Items.Add(i); }
            clustering.AddRoot(root);
            CentroidData centroid = new CentroidData();
            centroid.Items.AddRange(root.Items);
            centroid.Update(dataset);
            centroid.UpdateCentroidLen();
            mCentroids = new ArrayList<CentroidData>();
            mCentroids.Add(centroid);
            return clustering;
        }

        protected ClusteringResult kMeans(IUnlabeledExampleCollection<SparseVector<double>> dataset, int k)
        {
            if (k == 1) { return CreateSingleCluster(dataset); } // border case
            double globalBestClustQual = 0;
            for (int trial = 1; trial <= mTrials; trial++)
            {
                mLogger.Trace("Cluster", "Clustering trial {0} of {1} ...", trial, mTrials);
                ArrayList<CentroidData> centroids = new ArrayList<CentroidData>(k);
                ArrayList<int> bestSeeds = null;
                for (int i = 0; i < k; i++)
                {
                    centroids.Add(new CentroidData());
                }
                // select seed items
                double minSim = double.MaxValue;
                ArrayList<int> tmp = new ArrayList<int>(dataset.Count);
                for (int i = 0; i < dataset.Count; i++) { tmp.Add(i); }
                for (int i = 0; i < 3; i++)
                {
                    ArrayList<SparseVector<double>> seeds = new ArrayList<SparseVector<double>>(k);
                    tmp.Shuffle(mRnd);
                    for (int j = 0; j < k; j++)
                    {
                        seeds.Add(dataset[tmp[j]]);
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
                    simAvg /= (double)(k * k - k);
                    if (simAvg < minSim)
                    {
                        minSim = simAvg;
                        bestSeeds = new ArrayList<int>(k);
                        for (int j = 0; j < k; j++) { bestSeeds.Add(tmp[j]); }
                    }
                }
                for (int i = 0; i < k; i++)
                {
                    centroids[i].Items.Add(bestSeeds[i]);
                    centroids[i].Update(dataset);
                    centroids[i].UpdateCentroidLen();
                }
                // execute main loop
                double clustQual;                
                kMeansMainLoop(dataset, centroids, out clustQual);
                if (trial == 1 || clustQual > globalBestClustQual)
                {
                    globalBestClustQual = clustQual;
                    mCentroids = centroids;
                }
            }
            return GetClusteringResult();
        }

        protected ClusteringResult GetClusteringResult()
        {
            ClusteringResult clustering = new ClusteringResult();
            foreach (CentroidData centroid in mCentroids)
            {
                clustering.AddRoot(new Cluster());
                clustering.Roots.Last.Items.AddRange(centroid.CurrentItems);
                clustering.Roots.Last.ClusterInfo = centroid.Tag;
            }
            return clustering;    
        }

        internal void Assign(ArrayList<CentroidData> centroids, SparseMatrix<double> dataMtx, int instCount, int offs, out double clustQual)
        {
            int k = centroids.Count;
            double[][] dotProd = new double[k][];
            clustQual = 0;
            int i = 0;
            foreach (CentroidData cen in centroids)
            {
                SparseVector<double> cenVec = cen.GetSparseVector();
                dotProd[i++] = ModelUtils.GetDotProductSimilarity(dataMtx, instCount, cenVec);
            }
            for (int instIdx = 0; instIdx < instCount; instIdx++)
            {
                double maxSim = double.MinValue;
                ArrayList<int> candidates = new ArrayList<int>();
                for (int cenIdx = 0; cenIdx < k; cenIdx++)
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
                centroids[candidates[0]].Items.Add(instIdx + offs);
                clustQual += maxSim;
            }
            clustQual /= (double)instCount;
        }

        internal void Update(IUnlabeledExampleCollection<SparseVector<double>> dataset, ArrayList<CentroidData> centroids)
        {
            foreach (CentroidData centroid in centroids)
            {
                centroid.Update(dataset);
                centroid.UpdateCentroidLen();
            }
        }

        internal void kMeansMainLoop(IUnlabeledExampleCollection<SparseVector<double>> dataset, ArrayList<CentroidData> centroids, out double clustQual)
        {
            double[][] dotProd = new double[centroids.Count][];
            SparseMatrix<double> dataMtx = ModelUtils.GetTransposedMatrix(dataset);
            int iter = 0;
            double bestClustQual = 0;
            while (true)
            {
                iter++;
                mLogger.Trace("Cluster", "Iteration {0} ...", iter);
                // assign items to clusters
                Assign(centroids, dataMtx, dataset.Count, /*offs=*/0, out clustQual);                
                mLogger.Trace("Cluster", "Quality: {0:0.0000}", clustQual);
                // update centroids
                Update(dataset, centroids);
                // check if done
                if (iter > 1 && clustQual - bestClustQual <= mEps) { break; }
                bestClustQual = clustQual;
            }
        }

        public virtual ClusteringResult Cluster(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count < mK ? new ArgumentValueException("dataset") : null);            
            return kMeans(dataset, mK);
        }

        ClusteringResult IClustering.Cluster(IUnlabeledExampleCollection dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            return Cluster((IUnlabeledExampleCollection<SparseVector<double>>)dataset); // throws ArgumentValueException
        }
    }
}
