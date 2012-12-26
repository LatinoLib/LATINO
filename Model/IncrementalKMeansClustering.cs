/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IncrementalKMeans.cs 
 *  Desc:    Incremental k-means clustering algorithm
 *  Created: Aug-2009
 *
 *  Author:  Miha Grcar 
 * 
 *  License: GNU LGPL (http://www.gnu.org/licenses/lgpl.txt)
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class IncrementalKMeansClustering
       |
       '-----------------------------------------------------------------------
    */
    public class IncrementalKMeansClustering : KMeansClusteringFast
    {        
        private UnlabeledDataset<SparseVector<double>> mDataset
            = null;
        
        public IncrementalKMeansClustering(int k) : base(k) // throws ArgumentOutOfRangeException
        {
            mLogger = Logger.GetInstanceLogger(typeof(IncrementalKMeansClustering));
        }

        public override ClusteringResult Cluster(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            mDataset = new UnlabeledDataset<SparseVector<double>>(dataset);
            return base.Cluster(dataset);
        }

        private double GetClustQual()
        {
            double clustQual = 0;
            foreach (CentroidData centroid in mCentroids)
            {
                foreach (int itemIdx in centroid.CurrentItems)
                {
                    clustQual += centroid.GetDotProduct(mDataset[itemIdx]);
                }
            }
            clustQual /= (double)mDataset.Count;
            return clustQual;
        }

        // TODO: exceptions
        public ClusteringResult Update(int dequeueN, IEnumerable<SparseVector<double>> addList, ref int iter)
        {
            StopWatch stopWatch = new StopWatch();
            // update centroid data (1)
            foreach (CentroidData centroid in mCentroids)
            {
                foreach (int item in centroid.CurrentItems)
                {
                    if (item >= dequeueN) { centroid.Items.Add(item); }
                }
                centroid.Update(mDataset);
                centroid.UpdateCentroidLen();
            }
            // update dataset
            mDataset.RemoveRange(0, dequeueN);
            int offs = mDataset.Count;
            mDataset.AddRange(addList);
            // update centroid data (2)
            foreach (CentroidData centroid in mCentroids)
            {
                Set<int> itemsOffs = new Set<int>();
                foreach (int item in centroid.CurrentItems)
                {
                    itemsOffs.Add(item - dequeueN);
                }
                centroid.CurrentItems.Inner.SetItems(itemsOffs);
                centroid.Items.SetItems(itemsOffs);
            }
            // assign new instances
            mLogger.Info("Update", "Initializing ...");
            int k = 0;
            foreach (SparseVector<double> example in addList)
            {
                double maxSim = double.MinValue;
                ArrayList<int> candidates = new ArrayList<int>();
                for (int cenIdx = 0; cenIdx < mK; cenIdx++)
                {
                    double sim = mCentroids[cenIdx].GetDotProduct(example);
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
                mCentroids[candidates[0]].Items.Add(offs + k);
                k++;
            }
            // update centroids
            foreach (CentroidData centroid in mCentroids)
            {
                centroid.Update(mDataset);
                centroid.UpdateCentroidLen();
            }
            double bestClustQual = GetClustQual();
            mLogger.Info("Update", "Quality: {0:0.0000}", bestClustQual);
            // main k-means loop
            iter = 0;
            while (true)
            {
                iter++;
                mLogger.Info("Update", "Iteration {0} ...", iter);
                // assign items to clusters
                for (int i = 0; i < mDataset.Count; i++)
                {
                    SparseVector<double> example = mDataset[i];
                    double maxSim = double.MinValue;
                    ArrayList<int> candidates = new ArrayList<int>();
                    for (int j = 0; j < mK; j++)
                    {
                        double sim = mCentroids[j].GetDotProduct(example);
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
                    if (candidates.Count > 0) // *** is this always true? 
                    {
                        mCentroids[candidates[0]].Items.Add(i);
                    }
                }
                //
                // *** OPTIMIZE THIS with GetDotProductSimilarity (see this.Cluster) !!! ***
                //
                //Console.WriteLine(">>> {0} >>> loop: assign items to clusters", stopWatch.TotalMilliseconds);
                stopWatch.Reset();
                // update centroids
                foreach (CentroidData centroid in mCentroids)
                {
                    centroid.Update(mDataset);
                    centroid.UpdateCentroidLen();
                }
                double clustQual = GetClustQual();
                //Console.WriteLine(">>> {0} >>> loop: update centroids", stopWatch.TotalMilliseconds);
                stopWatch.Reset();
                mLogger.Info("Update", "Quality: {0:0.0000} Diff: {1:0.0000}", clustQual, clustQual - bestClustQual);
                // check if done
                if (clustQual - bestClustQual <= mEps)
                {
                    break;
                }
                bestClustQual = clustQual;
            }
            // save the result
            ClusteringResult clustering = new ClusteringResult();
            for (int i = 0; i < mK; i++)
            {
                clustering.AddRoot(new Cluster());
                clustering.Roots.Last.Items.AddRange(mCentroids[i].Items);
            }
            return clustering;
        }
    }
}