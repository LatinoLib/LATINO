/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          __IncrementalKMeans__.cs 
 *  Version:       1.0
 *  Desc:		   Incremental k-means clustering algorithm (experimental)
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    public class IncrementalKMeans : IClustering<SparseVector<double>.ReadOnly> 
    {
        private int mK;
        private static ISimilarity<SparseVector<double>.ReadOnly> mSimilarity
            = new DotProductSimilarity();
        private Random mRnd
            = new Random();
        private double mEps
            = 0.0005;
        private int mTrials
            = 1;
        private ArrayList<CentroidData> mCentroids
            = null;
        private ArrayList<KeyDat<double, int>> mMedoids
            = null;
        private UnlabeledDataset<SparseVector<double>.ReadOnly> mDataset
            = null;

        public IncrementalKMeans(int k)
        {
            Utils.ThrowException(k < 2 ? new ArgumentOutOfRangeException("k") : null);
            mK = k;
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

        // *** IClustering<LblT, SparseVector<double>.ReadOnly> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>.ReadOnly); }
        }

        public ClusteringResult Cluster(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count < mK ? new ArgumentValueException("dataset") : null);
            mDataset = new UnlabeledDataset<SparseVector<double>.ReadOnly>(dataset);            
            ClusteringResult clustering = null;
            double globalBestClustQual = 0;
            for (int trial = 1; trial <= mTrials; trial++)
            {
                Utils.VerboseLine("*** CLUSTERING TRIAL {0} OF {1} ***", trial, mTrials);
                ArrayList<CentroidData> centroids = new ArrayList<CentroidData>(mK);
                ArrayList<int> bestSeeds = null;
                for (int i = 0; i < mK; i++)
                {
                    centroids.Add(new CentroidData());
                }
                // select seed items
                double minSim = double.MaxValue;
                ArrayList<int> tmp = new ArrayList<int>(mDataset.Count);
                for (int i = 0; i < mDataset.Count; i++) { tmp.Add(i); }
                for (int k = 0; k < 3; k++)
                {
                    ArrayList<SparseVector<double>.ReadOnly> seeds = new ArrayList<SparseVector<double>.ReadOnly>(mK);
                    tmp.Shuffle(mRnd);
                    for (int i = 0; i < mK; i++)
                    {
                        seeds.Add(mDataset[tmp[i]]);
                    }
                    // assess quality of seed items
                    double simAvg = 0;
                    foreach (SparseVector<double>.ReadOnly seed1 in seeds)
                    {
                        foreach (SparseVector<double>.ReadOnly seed2 in seeds)
                        {
                            if (seed1 != seed2)
                            {
                                simAvg += mSimilarity.GetSimilarity(seed1, seed2);
                            }
                        }
                    }
                    simAvg /= (double)(mK * mK - mK);
                    //Console.WriteLine(simAvg);
                    if (simAvg < minSim)
                    {
                        minSim = simAvg;
                        bestSeeds = new ArrayList<int>(mK);
                        for (int i = 0; i < mK; i++) { bestSeeds.Add(tmp[i]); }
                    }
                }
                ArrayList<KeyDat<double, int>> medoids = new ArrayList<KeyDat<double, int>>(mK);
                for (int i = 0; i < mK; i++)
                {
                    centroids[i].Items.Add(bestSeeds[i]);
                    centroids[i].Update(mDataset);
                    centroids[i].UpdateCentroidLen();
                    medoids.Add(new KeyDat<double, int>(-1, bestSeeds[i]));
                }
                double[,] dotProd = new double[mDataset.Count, mK];
                SparseMatrix<double> dsMat = ModelUtils.GetTransposedMatrix(mDataset);
                // main loop
                int iter = 0;
                double bestClustQual = 0;
                double clustQual;
                while (true)
                {
                    iter++;
                    clustQual = 0;
                    // assign items to clusters
                    //StopWatch stopWatch = new StopWatch();               
                    int j = 0;
                    foreach (CentroidData cen in centroids)
                    {
                        SparseVector<double> cenVec = cen.GetSparseVector();
                        double[] dotProdSimVec = ModelUtils.GetDotProductSimilarity(dsMat, mDataset.Count, cenVec);
                        for (int i = 0; i < dotProdSimVec.Length; i++)
                        {
                            if (dotProdSimVec[i] > 0)
                            {
                                dotProd[i, j] = dotProdSimVec[i];
                            }
                        }
                        j++;
                    }
                    for (int dsInstIdx = 0; dsInstIdx < mDataset.Count; dsInstIdx++)
                    {
                        double maxSim = double.MinValue;
                        ArrayList<int> candidates = new ArrayList<int>();
                        for (int cenIdx = 0; cenIdx < mK; cenIdx++)
                        {
                            double sim = dotProd[dsInstIdx, cenIdx];
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
                        if (candidates.Count > 0) // *** is this always true? 
                        {
                            centroids[candidates[0]].Items.Add(dsInstIdx);
                            clustQual += maxSim;
                            if (medoids[candidates[0]].Key < maxSim)
                            {
                                medoids[candidates[0]] = new KeyDat<double, int>(maxSim, dsInstIdx);
                            }
                        }
                    }
                    //Console.WriteLine(stopWatch.TotalMilliseconds);
                    clustQual /= (double)mDataset.Count;
                    Utils.VerboseLine("*** Iteration {0} ***", iter);
                    Utils.VerboseLine("Quality: {0:0.0000}", clustQual);
                    // compute new centroids
                    for (int i = 0; i < mK; i++)
                    {
                        centroids[i].Update(mDataset);
                        centroids[i].UpdateCentroidLen();
                    }
                    // check if done
                    if (iter > 1 && clustQual - bestClustQual <= mEps)
                    {
                        break;
                    }
                    bestClustQual = clustQual;
                    for (int i = 0; i < medoids.Count; i++)
                    {
                        medoids[i] = new KeyDat<double, int>(-1, medoids[i].Dat);
                    }
                }
                if (trial == 1 || clustQual > globalBestClustQual)
                {
                    globalBestClustQual = clustQual;
                    mCentroids = centroids;
                    mMedoids = medoids;
                    // save the result
                    clustering = new ClusteringResult();
                    for (int i = 0; i < mK; i++)
                    {
                        clustering.AddRoot(new Cluster());
                        clustering.Roots.Last.Items.AddRange(centroids[i].Items);
                    }
                }
            }            
            return clustering;
        }

        // TODO: exceptions
        public ArrayList<int> GetMedoids()
        {
            ArrayList<int> medoids = new ArrayList<int>(mMedoids.Count);
            foreach (KeyDat<double, int> item in mMedoids)
            {
                medoids.Add(item.Dat);
            }
            return medoids;
        }

        // TODO: exceptions
        public ArrayList<SparseVector<double>> GetCentroids()
        {
            ArrayList<SparseVector<double>> centroids = new ArrayList<SparseVector<double>>();
            foreach (CentroidData centroid in mCentroids)
            {
                centroids.Add(centroid.GetSparseVector());
            }
            return centroids;
        }

        //private double GetQual()
        //{
        //    double clustQual = 0;
        //    foreach (Centroid centroid in mCentroids)
        //    {
        //        foreach (int itemIdx in centroid.CurrentItems)
        //        {
        //            clustQual += centroid.GetDotProduct(mDataset[itemIdx]);
        //        }
        //    }
        //    clustQual /= (double)mDataset.Count;
        //    return clustQual;
        //}

        // TODO: exceptions
        public ClusteringResult Update(int dequeueN, IEnumerable<SparseVector<double>.ReadOnly> addList, ref int iter)
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
            Console.WriteLine(">>> {0} >>> update centroid data (1)", stopWatch.TotalMilliseconds);
            stopWatch.Reset();
            // update dataset
            mDataset.RemoveRange(0, dequeueN);
            int ofs = mDataset.Count;
            mDataset.AddRange(addList);
            Console.WriteLine(">>> {0} >>> update dataset", stopWatch.TotalMilliseconds);
            stopWatch.Reset();
            // update centroid data (2)
            foreach (CentroidData centroid in mCentroids)
            {
                Set<int> itemsOfs = new Set<int>();
                foreach (int item in centroid.CurrentItems)
                {
                    itemsOfs.Add(item - dequeueN);
                }
                centroid.CurrentItems.Inner.SetItems(itemsOfs);
                centroid.Items.SetItems(itemsOfs);
            }
            Console.WriteLine(">>> {0} >>> update centroid data (2)", stopWatch.TotalMilliseconds);
            stopWatch.Reset();
            // assign new instances
            double bestClustQual = 0;
            {
                int i = 0;
                foreach (SparseVector<double>.ReadOnly example in addList)
                {
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
                        mCentroids[candidates[0]].Items.Add(ofs + i);
                    }
                    i++;
                }
                // update centroids
                foreach (CentroidData centroid in mCentroids)
                {
                    centroid.Update(mDataset);
                    centroid.UpdateCentroidLen();
                }
                //Console.WriteLine(GetQual());
                foreach (CentroidData centroid in mCentroids)
                {
                    foreach (int itemIdx in centroid.CurrentItems)
                    {
                        bestClustQual += centroid.GetDotProduct(mDataset[itemIdx]);
                    }
                }
                bestClustQual /= (double)mDataset.Count;
                Utils.VerboseLine("*** Initialization ***");
                Utils.VerboseLine("Quality: {0:0.0000}", bestClustQual);
            }
            Console.WriteLine(">>> {0} >>> assign new instances", stopWatch.TotalMilliseconds);
            stopWatch.Reset();
            // main k-means loop
            iter = 0;
            while (true)
            {
                iter++;
                // assign items to clusters
                for (int i = 0; i < mDataset.Count; i++)
                {
                    SparseVector<double>.ReadOnly example = mDataset[i];
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
                Console.WriteLine(">>> {0} >>> loop: assign items to clusters", stopWatch.TotalMilliseconds);
                stopWatch.Reset();
                double clustQual = 0;
                // update centroids
                foreach (CentroidData centroid in mCentroids)
                {
                    centroid.Update(mDataset);
                    centroid.UpdateCentroidLen();
                }
                //Console.WriteLine(GetQual());
                foreach (CentroidData centroid in mCentroids)
                {
                    foreach (int itemIdx in centroid.CurrentItems)
                    {
                        clustQual += centroid.GetDotProduct(mDataset[itemIdx]);
                    }
                }
                clustQual /= (double)mDataset.Count;
                Console.WriteLine(">>> {0} >>> loop: update centroids", stopWatch.TotalMilliseconds);
                stopWatch.Reset();
                Utils.VerboseLine("*** Iteration {0} ***", iter);
                Utils.VerboseLine("Quality: {0:0.0000} Diff: {1:0.0000}", clustQual, clustQual - bestClustQual);
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

        ClusteringResult IClustering.Cluster(IUnlabeledExampleCollection dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>.ReadOnly>) ? new ArgumentTypeException("dataset") : null);
            return Cluster((IUnlabeledExampleCollection<SparseVector<double>.ReadOnly>)dataset); // throws ArgumentValueException
        }
    }
}