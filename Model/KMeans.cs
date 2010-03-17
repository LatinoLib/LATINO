/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          KMeans.cs
 *  Version:       1.0
 *  Desc:		   K-means clustering algorithm
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 * 
 ***************************************************************************/

using System;

namespace Latino.Model
{
    public class KMeans : IClustering<SparseVector<double>.ReadOnly> 
    {        
        private ISimilarity<SparseVector<double>.ReadOnly> mSimilarity
            = new CosineSimilarity();
        private Random mRnd
            = new Random();
        private CentroidType mCentroidType
            = CentroidType.NrmL2;
        private double mEps
            = 0.0005;
        private int mTrials
            = 1;
        private int mK;

        public KMeans(int k)
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

        public ISimilarity<SparseVector<double>.ReadOnly> Similarity
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
            ClusteringResult bestClustering = null;
            double globalBestClustQual = 0;
            for (int trial = 1; trial <= mTrials; trial++)
            {
                Utils.VerboseLine("*** CLUSTERING TRIAL {0} OF {1} ***", trial, mTrials);
                ArrayList<SparseVector<double>.ReadOnly> centroids = null;
                clustering = new ClusteringResult();
                for (int i = 0; i < mK; i++) { clustering.AddRoot(new Cluster()); }
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
                        seeds.Add(ModelUtils.ComputeCentroid(new SparseVector<double>.ReadOnly[] { dataset[tmp[i]] }, mCentroidType));
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
                    clustQual = 0;
                    // assign items to clusters
                    foreach (Cluster cluster in clustering.Roots) { cluster.Items.Clear(); }
                    for (int i = 0; i < dataset.Count; i++)
                    {
                        SparseVector<double>.ReadOnly example = dataset[i];
                        double maxSim = double.MinValue;
                        ArrayList<int> candidates = new ArrayList<int>();
                        for (int j = 0; j < mK; j++)
                        {
                            SparseVector<double>.ReadOnly centroid = centroids[j];
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
                        if (candidates.Count > 0) // *** is this always true? 
                        {
                            clustering.Roots[candidates[0]].Items.Add(i);
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
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>.ReadOnly>) ? new ArgumentTypeException("dataset") : null);
            return Cluster((IUnlabeledExampleCollection<SparseVector<double>.ReadOnly>)dataset); // throws ArgumentValueException
        }
    }
}
