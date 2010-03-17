/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          KMeansFast.cs
 *  Version:       1.0
 *  Desc:		   K-means clustering algorithm (optimized for speed)
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 * 
 ***************************************************************************/

using System;

namespace Latino.Model
{
    public class KMeansFast : IClustering<SparseVector<double>.ReadOnly> 
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

        public KMeansFast(int k)
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
            ClusteringResult clustering = null;
            double globalBestClustQual = 0;
            for (int trial = 1; trial <= mTrials; trial++)
            {
                Utils.VerboseLine("*** CLUSTERING TRIAL {0} OF {1} ***", trial, mTrials);
                ArrayList<Centroid> centroids = new ArrayList<Centroid>(mK);
                ArrayList<int> bestSeeds = null;
                centroids.Add(new Centroid(dataset));
                int vecLen = centroids[0].VecLen;
                for (int i = 1; i < mK; i++)
                {
                    centroids.Add(new Centroid(dataset, vecLen));
                }
                // select seed items
                double minSim = double.MaxValue;
                ArrayList<int> tmp = new ArrayList<int>(dataset.Count);
                for (int i = 0; i < dataset.Count; i++) { tmp.Add(i); }
                for (int k = 0; k < 3; k++)
                {
                    ArrayList<SparseVector<double>.ReadOnly> seeds = new ArrayList<SparseVector<double>.ReadOnly>(mK);
                    tmp.Shuffle(mRnd);
                    for (int i = 0; i < mK; i++)
                    {
                        seeds.Add(dataset[tmp[i]]);
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
                for (int i = 0; i < mK; i++)
                {
                    centroids[i].Items.Add(bestSeeds[i]);
                    centroids[i].Update();
                }
                // main loop
                int iter = 0;
                double bestClustQual = 0;
                double clustQual;
                while (true)
                {
                    iter++;
                    clustQual = 0;
                    // assign items to clusters
                    for (int i = 0; i < dataset.Count; i++)
                    {
                        SparseVector<double>.ReadOnly example = dataset[i];
                        double maxSim = double.MinValue;
                        ArrayList<int> candidates = new ArrayList<int>();
                        for (int j = 0; j < mK; j++)
                        {
                            double sim = centroids[j].GetDotProduct(example);
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
                            centroids[candidates[0]].Items.Add(i);
                            clustQual += maxSim;
                        }
                    }
                    clustQual /= (double)dataset.Count;
                    Utils.VerboseLine("*** Iteration {0} ***", iter);
                    Utils.VerboseLine("Quality: {0:0.0000}", clustQual);
                    // check if done
                    if (iter > 1 && clustQual - bestClustQual <= mEps)
                    {
                        break;
                    }
                    bestClustQual = clustQual;
                    // compute new centroids
                    for (int i = 0; i < mK; i++)
                    {
                        centroids[i].ResetNrmL2();
                        centroids[i].Update();
                        centroids[i].NormalizeL2();
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
                        clustering.Roots.Last.Items.AddRange(centroids[i].Items);
                    }
                }
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
