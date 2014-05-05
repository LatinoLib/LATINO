/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    HierarchicalBisectingClusteringFast.cs
 *  Desc:    Hierarchical bisecting clustering algorithm (fast)
 *  Created: May-2012
 *
 *  Author:  Miha Grcar 
 * 
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class HierarchicalBisectingClusteringFast
       |
       '-----------------------------------------------------------------------
    */
    public class HierarchicalBisectingClusteringFast : IClustering<SparseVector<double>>
    {
        private KMeansClusteringFast mKMeansClustering
            = new KMeansClusteringFast(/*k=*/2);
        private double mMinQuality
            = 0.2;

        public HierarchicalBisectingClusteringFast()
        {
            mKMeansClustering.Logger = Logger.GetLogger(typeof(HierarchicalBisectingClusteringFast));
        }

        public Logger Logger
        {
            get { return mKMeansClustering.Logger; }
            set { mKMeansClustering.Logger = value; } // throws ArgumentNullException
        }

        public Random Random
        {
            get { return mKMeansClustering.Random; }
            set { mKMeansClustering.Random = value; } // throws ArgumentNullException
        }

        public double Eps
        {
            get { return mKMeansClustering.Eps; }
            set { mKMeansClustering.Eps = value; } // throws ArgumentOutOfRangeException
        }

        public int Trials
        {
            get { return mKMeansClustering.Trials; }
            set { mKMeansClustering.Trials = value; } // throws ArgumentOutOfRangeException
        }

        public double MinQuality
        {
            get { return mMinQuality; }
            set
            {
                Utils.ThrowException((value > 1.0 || value < 0.0) ? new ArgumentOutOfRangeException("MinQuality") : null);
                mMinQuality = value;
            }
        }

        // *** IClustering<LblT, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }

        private UnlabeledDataset<SparseVector<double>> GetDatasetSubset(IEnumerable<int> items, IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            UnlabeledDataset<SparseVector<double>> datasetSubset = new UnlabeledDataset<SparseVector<double>>();
            foreach (int item in items) { datasetSubset.Add(dataset[item]); }
            return datasetSubset;
        }

        private double GetClusterQuality(IUnlabeledExampleCollection<SparseVector<double>> dataset, out SparseVector<double> centroid)
        {
            // compute centroid
            centroid = ModelUtils.ComputeCentroid(dataset, CentroidType.NrmL2);
            // compute intra-cluster similarities
            double[] simData = ModelUtils.GetDotProductSimilarity(dataset, centroid);
            // compute cluster quality
            double quality = 0;
            for (int i = 0; i < simData.Length; i++) { quality += simData[i]; }
            quality /= (double)simData.Length;
            return quality;
        }

        public ClusteringResult Cluster(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count < 2 ? new ArgumentValueException("dataset") : null);
            ClusteringResult clusteringResult = new ClusteringResult();
            Queue<Cluster> queue = new Queue<Cluster>();
            // create root
            Cluster root = new Cluster();
            for (int i = 0; i < dataset.Count; i++)
            {
                Utils.ThrowException(dataset[i].Count == 0 ? new ArgumentValueException("dataset") : null);
                root.Items.Add(i);
            }
            clusteringResult.AddRoot(root);
            // add root to queue
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                // get next cluster
                Cluster cluster = queue.Dequeue();
                // compute cluster quality
                UnlabeledDataset<SparseVector<double>> localDataset = GetDatasetSubset(cluster.Items, dataset);
                SparseVector<double> centroid;
                double quality = GetClusterQuality(localDataset, out centroid);
                cluster.ClusterInfo = new Pair<SparseVector<double>, double>(centroid, quality);
                if (quality < mMinQuality)
                {
                    // split cluster, add children to queue
                    ClusteringResult localResult = mKMeansClustering.Cluster(localDataset);
                    for (int i = 0; i < 2; i++)
                    {
                        cluster.AddChild(localResult.Roots[i]);
                        localResult.Roots[i].Parent = cluster;
                        queue.Enqueue(localResult.Roots[i]);
                    }
                }
            }
            return clusteringResult;
        }

        ClusteringResult IClustering.Cluster(IUnlabeledExampleCollection dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            return Cluster((IUnlabeledExampleCollection<SparseVector<double>>)dataset); // throws ArgumentValueException
        }
    }
}
