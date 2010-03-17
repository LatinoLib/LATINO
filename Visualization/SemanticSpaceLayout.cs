/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          SemanticSpaceLayout.cs
 *  Version:       1.0
 *  Desc:		   Semantic space layout algorithm
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Latino.Model;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Class SemanticSpaceLayout 
       |
       '-----------------------------------------------------------------------
    */
    public class SemanticSpaceLayout : ILayoutAlgorithm
    {
        private IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> mDataset;
        private Random mRandom
            = new Random(1);
        private double mKMeansEps
            = 0.01;
        private int mKClust
            = 100;
        private double mSimThresh
            = 0.005;
        private int mKNn
            = 10;

        public SemanticSpaceLayout(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            mDataset = dataset;
        }

        public Random Random
        {
            get { return mRandom; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Random") : null);
                mRandom = value; 
            }
        }

        public double KMeansEps
        {
            get { return mKMeansEps; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("KMeansEps") : null);
                mKMeansEps = value;
            }
        }

        public int KMeansK
        {
            get { return mKClust; }
            set
            {
                Utils.ThrowException(value < 2 ? new ArgumentOutOfRangeException("KMeansK") : null);
                mKClust = value;
            }
        }

        public double SimThresh
        {
            get { return mSimThresh; }
            set 
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("SimThresh") : null);
                mSimThresh = value;
            }
        }

        public int NeighborhoodSize
        {
            get { return mKNn; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("NeighborhoodSize") : null);
                mKNn = value;
            }
        }

        // *** ILayoutAlgorithm interface implementation ***

        public Vector2D[] ComputeLayout()
        {
            return ComputeLayout(/*settings=*/null);
        }

        public Vector2D[] ComputeLayout(LayoutSettings settings)
        {
            UnlabeledDataset<SparseVector<double>.ReadOnly> dataset = new UnlabeledDataset<SparseVector<double>.ReadOnly>(mDataset);
            // clustering 
            Utils.VerboseLine("Clustering ...");
            KMeansFast kMeans = new KMeansFast(mKClust);
            kMeans.Eps = mKMeansEps;
            kMeans.Random = mRandom;
            kMeans.Trials = 1;
            ClusteringResult clustering = kMeans.Cluster(mDataset); // throws ArgumentValueException
            // determine reference instances
            UnlabeledDataset<SparseVector<double>.ReadOnly> dsRefInst = new UnlabeledDataset<SparseVector<double>.ReadOnly>();
            foreach (Cluster cluster in clustering.Roots)
            {
                SparseVector<double> centroid = cluster.ComputeCentroid(mDataset, CentroidType.NrmL2);
                dsRefInst.Add(centroid); // dataset of reference instances
                dataset.Add(centroid); // add centroids to the main dataset
            }
            // position reference instances
            Utils.VerboseLine("Positioning reference instances ...");
            SparseMatrix<double> simMtx = ModelUtils.GetDotProductSimilarity(dsRefInst, mSimThresh, /*fullMatrix=*/false);
            StressMajorizationLayout sm = new StressMajorizationLayout(dsRefInst.Count, new DistFunc(simMtx));
            sm.Random = mRandom;
            Vector2D[] centrPos = sm.ComputeLayout();
            // k-NN
            Utils.VerboseLine("Computing similarities ...");
            simMtx = ModelUtils.GetDotProductSimilarity(dataset, mSimThresh, /*fullMatrix=*/true);
            Utils.VerboseLine("Constructing system of linear equations ...");
            LabeledDataset<double, SparseVector<double>.ReadOnly> lsqrDs = new LabeledDataset<double, SparseVector<double>.ReadOnly>();
            foreach (IdxDat<SparseVector<double>> simMtxRow in simMtx)
            {
                if (simMtxRow.Dat.Count <= 1)
                {
                    Utils.VerboseLine("*** Warning: instance #{0} has no neighborhood.", simMtxRow.Idx);
                }
                ArrayList<KeyDat<double, int>> knn = new ArrayList<KeyDat<double, int>>(simMtxRow.Dat.Count);
                foreach (IdxDat<double> item in simMtxRow.Dat)
                {
                    if (item.Idx != simMtxRow.Idx)
                    {
                        knn.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                    }
                }
                knn.Sort(new DescSort<KeyDat<double, int>>());
                int count = Math.Min(knn.Count, mKNn);
                SparseVector<double> eq = new SparseVector<double>();
                double wgt = 1.0 / (double)count;
                for (int i = 0; i < count; i++)
                {
                    eq.InnerIdx.Add(knn[i].Dat);
                    eq.InnerDat.Add(-wgt);
                }
                eq.InnerIdx.Sort(); // *** sort only indices
                eq[simMtxRow.Idx] = 1;
                lsqrDs.Add(0, eq);
            }
            Vector2D[] layout = new Vector2D[dataset.Count - mKClust];
            for (int i = dataset.Count - mKClust, j = 0; i < dataset.Count; i++, j++)
            {
                SparseVector<double> eq = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(i, 1) });
                lsqrDs.Add(centrPos[j].X, eq);
            }
            LSqrModel lsqr = new LSqrModel();
            lsqr.Train(lsqrDs);
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i].X = lsqr.Solution[i];
            }
            for (int i = lsqrDs.Count - mKClust, j = 0; i < lsqrDs.Count; i++, j++)
            {
                lsqrDs[i].Label = centrPos[j].Y;
            }
            lsqr.Train(lsqrDs);
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i].Y = lsqr.Solution[i];
            }
            return settings == null ? layout : settings.AdjustLayout(layout);
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class DistFunc
           |
           '-----------------------------------------------------------------------
        */
        private class DistFunc : IDistance<int>
        {
            private SparseMatrix<double>.ReadOnly mSimMtx;

            public DistFunc(SparseMatrix<double>.ReadOnly simMtx)
            {
                mSimMtx = simMtx;
            }

            public double GetDistance(int a, int b) 
            {
                if (a > b) { return 1.0 - mSimMtx.TryGet(b, a, 0); }
                else { return 1.0 - mSimMtx.TryGet(a, b, 0); }
            }

            public void Save(BinarySerializer dummy)
            {
                throw new NotImplementedException();
            }
        }
    }
}