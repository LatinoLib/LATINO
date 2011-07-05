/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IncrementalSemanticSpaceLayout.cs
 *  Desc:    Semantic space layout algorithm 
 *  Created: Nov-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Latino.Model;
using System.IO;
using Latino.Visualization;
using Latino.Experimental.Model;

namespace Latino.Experimental.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Class IncrementalSemanticSpaceLayout 
       |
       '-----------------------------------------------------------------------
    */
    public class IncrementalSemanticSpaceLayout : ILayoutAlgorithm
    {
        private UnlabeledDataset<SparseVector<double>> mDataset;
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
        private int mKNnExt
            = 60;
        private IncrementalKMeans mKMeans
            = null;
        private Vector2D[] mRefPos
            = null;
        private ArrayList<Patch> mPatches
            = new ArrayList<Patch>();
        private ArrayList<double> mSolX 
            = null;
        private ArrayList<double> mSolY 
            = null;

        private static Logger mLogger
            = Logger.GetLogger(typeof(IncrementalSemanticSpaceLayout));

        public IncrementalSemanticSpaceLayout(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            mDataset = new UnlabeledDataset<SparseVector<double>>(dataset);
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
            // clustering 
            mLogger.Info("ComputeLayout", "Clustering ...");
            mKMeans = new IncrementalKMeans(mKClust);
            mKMeans.Eps = mKMeansEps;
            mKMeans.Random = mRandom;
            mKMeans.Trials = 3;
            ClusteringResult clustering = mKMeans.Cluster(mDataset); // throws ArgumentValueException
            // determine reference instances
            UnlabeledDataset<SparseVector<double>> dsRefInst = new UnlabeledDataset<SparseVector<double>>();
            foreach (SparseVector<double> centroid in mKMeans.GetCentroids())
            {
                dsRefInst.Add(centroid); // dataset of reference instances
                mDataset.Add(centroid); // add centroids to the main dataset
            }
            // position reference instances
            mLogger.Info("ComputeLayout", "Positioning reference instances ...");
            SparseMatrix<double> simMtx = ModelUtils.GetDotProductSimilarity(dsRefInst, mSimThresh, /*fullMatrix=*/false);
            StressMajorizationLayout sm = new StressMajorizationLayout(dsRefInst.Count, new DistFunc(simMtx));
            sm.Random = mRandom;
            sm.MaxSteps = int.MaxValue;
            sm.MinDiff = 0.00001;
            mRefPos = sm.ComputeLayout();
            // k-NN
            mLogger.Info("ComputeLayout", "Computing similarities ...");           
            simMtx = ModelUtils.GetDotProductSimilarity(mDataset, mSimThresh, /*fullMatrix=*/true);
            mLogger.Info("ComputeLayout", "Constructing system of linear equations ...");
            LabeledDataset<double, SparseVector<double>> lsqrDs = new LabeledDataset<double, SparseVector<double>>();
            mPatches = new ArrayList<Patch>(mDataset.Count);
            for (int i = 0; i < mDataset.Count; i++)
            {
                mPatches.Add(new Patch(i));
            }
            foreach (IdxDat<SparseVector<double>> simMtxRow in simMtx)
            {
                if (simMtxRow.Dat.Count <= 1)
                {
                    mLogger.Warn("ComputeLayout", "Instance #{0} has no neighborhood.", simMtxRow.Idx);
                }
                ArrayList<KeyDat<double, int>> knn = new ArrayList<KeyDat<double, int>>(simMtxRow.Dat.Count);
                foreach (IdxDat<double> item in simMtxRow.Dat)
                {
                    if (item.Idx != simMtxRow.Idx)
                    {
                        knn.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                    }
                }
                knn.Sort(DescSort<KeyDat<double, int>>.Instance);
                int count = Math.Min(knn.Count, mKNnExt);
                for (int i = 0; i < count; i++)
                {
                    mPatches[simMtxRow.Idx].List.Add(new KeyDat<double, Patch>(knn[i].Key, mPatches[knn[i].Dat]));                    
                }
                mPatches[simMtxRow.Idx].ProcessList();
                count = Math.Min(knn.Count, mKNn);
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
            Vector2D[] layout = new Vector2D[mDataset.Count - mKClust];
            for (int i = mDataset.Count - mKClust, j = 0; i < mDataset.Count; i++, j++)
            {
                SparseVector<double> eq = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(i, 1) });
                lsqrDs.Add(mRefPos[j].X, eq);
            }
            LSqrModel lsqr = new LSqrModel();
            lsqr.Train(lsqrDs);
            mSolX = lsqr.Solution.GetWritableCopy();
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i].X = lsqr.Solution[i];
            }
            for (int i = lsqrDs.Count - mKClust, j = 0; i < lsqrDs.Count; i++, j++)
            {
                lsqrDs[i].Label = mRefPos[j].Y;
            }
            lsqr.Train(lsqrDs);
            mSolY = lsqr.Solution.GetWritableCopy();
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i].Y = lsqr.Solution[i];
            }
            return settings == null ? layout : settings.AdjustLayout(layout);
        }

        private void Output(IEnumerable<KeyDat<double, Patch>> list)
        { 
            foreach (KeyDat<double, Patch> item in list)
            {
                Console.Write("({0} {1}) ", item.Key, item.Dat.Idx);
            }
            Console.WriteLine();
        }

        public class PtInfo
        {
            public double X;
            public double Y;
            public SparseVector<double> Vec;
        }

        // TODO: exceptions
        public Vector2D[] Update(int numDequeue, IEnumerable<SparseVector<double>> newInst, bool test, LayoutSettings settings, ref PtInfo[] ptInfo, int _count)
        {
            // clustering             
            mLogger.Info("Update", "Clustering ...");
            /*prof*/StopWatch sw = new StopWatch();
            mKMeans.Eps = mKMeansEps;
            int iter = 0;
            mKMeans.Update(numDequeue, newInst, ref iter);
            /*prof*/sw.Save("cl.txt", _count, iter.ToString());
            // determine reference instances
            /*prof*/sw.Reset();
            UnlabeledDataset<SparseVector<double>> dsRefInst = new UnlabeledDataset<SparseVector<double>>();
            UnlabeledDataset<SparseVector<double>> dsNewInst = new UnlabeledDataset<SparseVector<double>>(newInst);
            foreach (SparseVector<double> centroid in mKMeans.GetCentroids())
            {
                dsRefInst.Add(centroid); // dataset of reference instances
                dsNewInst.Add(centroid); // dataset of new instances
            }
            // position reference instances
            mLogger.Info("Update", "Positioning reference instances ...");
            SparseMatrix<double> simMtx = ModelUtils.GetDotProductSimilarity(dsRefInst, mSimThresh, /*fullMatrix=*/false);
            StressMajorizationLayout sm = new StressMajorizationLayout(dsRefInst.Count, new DistFunc(simMtx));
            sm.Random = mRandom;
            sm.MaxSteps = int.MaxValue;
            sm.MinDiff = 1E-3;
            mRefPos = sm.ComputeLayout(/*settings=*/null, mRefPos/*make this a property!!!*/);
            /*prof*/sw.Save("sm.txt", _count);
            // k-NN
            /*prof*/sw.Reset();
            DateTime t = DateTime.Now;
            mLogger.Info("Update", "Computing similarities ...");
            // update list of neighborhoods
            mPatches.RemoveRange(mDataset.Count - mKClust, mKClust);
            mPatches.RemoveRange(0, numDequeue);
            // remove instances from [dataset and] neighborhoods
            foreach (Patch patch in mPatches)
            {                                
                if (patch.Min != null && (patch.Min.Idx < numDequeue || patch.Max.Idx >= mDataset.Count - mKClust))
                {
                    int oldCount = patch.List.Count;
                    ArrayList<KeyDat<double, Patch>> tmp = new ArrayList<KeyDat<double, Patch>>();
                    foreach (KeyDat<double, Patch> item in patch.List)
                    {
                        if (item.Dat.Idx >= numDequeue && item.Dat.Idx < mDataset.Count - mKClust)
                        {
                            tmp.Add(item);
                        }
                        //else
                        //{
                        //    Console.WriteLine("Remove {0}", item.Dat.Idx - numDequeue);
                        //}
                    }                   
                    patch.List = tmp;
                    patch.ProcessList();
                    patch.NeedUpdate = patch.List.Count < mKNn && oldCount >= mKNn;
                }                
            }
            // update dataset
            mDataset.RemoveRange(mDataset.Count - mKClust, mKClust);
            mDataset.RemoveRange(0, numDequeue);
            // add new instances to dataset 
            int preAddCount = mDataset.Count;
            mDataset.AddRange(dsNewInst);
            // precompute transposed matrices
            SparseMatrix<double> trNewInst = ModelUtils.GetTransposedMatrix(dsNewInst);
            SparseMatrix<double> trDataset = ModelUtils.GetTransposedMatrix(mDataset);
            // add new instances to neighborhoods
            for (int i = 0; i < dsNewInst.Count; i++)
            {
                mPatches.Add(new Patch(-1));
                mPatches.Last.NeedUpdate = true;
            }
            for (int i = 0; i < mPatches.Count; i++)
            { 
                mPatches[i].Idx = i;
            }
            for (int i = 0; i < mPatches.Count; i++)
            {
                Patch patch = mPatches[i];                
                SparseVector<double> vec = mDataset[i];
                if (vec != null)
                {
                    if (patch.NeedUpdate) // full update required
                    {
                        //if (i == 1347) { Console.WriteLine("full update"); }
                        SparseVector<double> simVec = ModelUtils.GetDotProductSimilarity(trDataset, mDataset.Count, vec, mSimThresh);
                        ArrayList<KeyDat<double, int>> tmp = new ArrayList<KeyDat<double, int>>();
                        foreach (IdxDat<double> item in simVec)
                        {
                            if (item.Idx != i)
                            {
                                tmp.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                            }
                        }
                        tmp.Sort(new Comparer2());
                        int count = Math.Min(tmp.Count, mKNnExt);
                        patch.List.Clear();
                        for (int j = 0; j < count; j++)
                        {
                            patch.List.Add(new KeyDat<double, Patch>(tmp[j].Key, mPatches[tmp[j].Dat]));
                        }
                        patch.ProcessList();
                        patch.NeedUpdate = false;
                    }
                    else // only new instances need to be considered
                    {
                        //if (i == 1347) { Console.WriteLine("partial update"); }
                        SparseVector<double> simVec = ModelUtils.GetDotProductSimilarity(trNewInst, dsNewInst.Count, vec, mSimThresh);
                        // check if further processing is needed
                        bool needMerge = false;
                        if (test)
                        {
                            foreach (IdxDat<double> item in simVec)
                            {
                                if (item.Dat >= patch.MinSim) 
                                {
                                    needMerge = true;
                                    //Console.WriteLine("{0} {1}", item.Dat, patch.MinSim);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (IdxDat<double> item in simVec)
                            {
                                if (item.Dat > patch.MinSim) 
                                {
                                    needMerge = true;
                                    //Console.WriteLine("{0} {1}", item.Dat, patch.MinSim);
                                    break;
                                }
                            }
                        }
                        if (needMerge || patch.List.Count < mKNn)
                        {
                            //if (i == 1347) { Console.WriteLine("merge"); }
                            int oldCount = patch.List.Count;
                            ArrayList<KeyDat<double, Patch>> tmp = new ArrayList<KeyDat<double, Patch>>();
                            foreach (IdxDat<double> item in simVec)
                            {
                                tmp.Add(new KeyDat<double, Patch>(item.Dat, mPatches[item.Idx + preAddCount]));
                            }
                            // merge the two lists
                            // TODO: speed this up
                            patch.List.AddRange(tmp);
                            patch.List.Sort(new Comparer());
                            // trim list to size
                            if (oldCount >= mKNn)
                            {
                                patch.List.RemoveRange(oldCount, patch.List.Count - oldCount);
                            }
                            patch.ProcessList();
                        }
                    }
                }
            }
            /*prof*/sw.Save("knn.txt", _count);
            // *** Test ***
            sw.Reset();
            ModelUtils.GetDotProductSimilarity(mDataset, mSimThresh, /*fullMatrix=*/true);
            sw.Save("selfSim.txt", _count, mDataset.Count.ToString());
            if (test)
            {
                simMtx = ModelUtils.GetDotProductSimilarity(mDataset, mSimThresh, /*fullMatrix=*/true);
                ArrayList<Patch> patches = new ArrayList<Patch>();
                for (int i = 0; i < mDataset.Count; i++)
                {
                    patches.Add(new Patch(i));
                }
                foreach (IdxDat<SparseVector<double>> simMtxRow in simMtx)
                {
                    if (simMtxRow.Dat.Count <= 1)
                    {
                        mLogger.Warn("Update", "Instance #{0} has no neighborhood.", simMtxRow.Idx);
                    }
                    ArrayList<KeyDat<double, int>> knn = new ArrayList<KeyDat<double, int>>(simMtxRow.Dat.Count);
                    foreach (IdxDat<double> item in simMtxRow.Dat)
                    {
                        if (item.Idx != simMtxRow.Idx)
                        {
                            knn.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                        }
                    }
                    knn.Sort(new Comparer2());
                    int count = Math.Min(knn.Count, mKNnExt);
                    for (int i = 0; i < count; i++)
                    {
                        patches[simMtxRow.Idx].List.Add(new KeyDat<double, Patch>(knn[i].Key, patches[knn[i].Dat]));
                    }
                    patches[simMtxRow.Idx].ProcessList();
                }
                // compare 
                if (patches.Count != mPatches.Count) { throw new Exception("Count mismatch."); }
                for (int i = 0; i < mPatches.Count; i++)
                {
                    if (patches[i].List.Count < mKNn && patches[i].List.Count != mPatches[i].List.Count)
                    {
                        Console.WriteLine(mPatches[i].List.Count);
                        Console.WriteLine(patches[i].List.Count);
                        Output(mPatches[i].List);
                        Output(patches[i].List);
                        Console.WriteLine(i);
                        throw new Exception("List count mismatch.");
                    }
                    int count = Math.Min(mPatches[i].List.Count, mKNn);
                    for (int j = 0; j < count; j++)
                    {
                        //Console.WriteLine("{4} {0}-{1} {2}-{3}", mPatches[i].List[j].Key, mPatches[i].List[j].Dat.Idx, patches[i].List[j].Key, patches[i].List[j].Dat.Idx, i);
                        if (mPatches[i].List[j].Key != patches[i].List[j].Key || mPatches[i].List[j].Dat.Idx != patches[i].List[j].Dat.Idx)
                        {
                            Console.WriteLine("i:{4} fast:{0}-{1} slow:{2}-{3}", mPatches[i].List[j].Key, mPatches[i].List[j].Dat.Idx, patches[i].List[j].Key, patches[i].List[j].Dat.Idx, i);
                            int idxFast = mPatches[i].List[j].Dat.Idx;
                            int idxSlow = patches[i].List[j].Dat.Idx;
                            Console.WriteLine("slow @ fast idx: {0}", GetKey(patches[i].List, idxFast));
                            Console.WriteLine("fast @ slow idx: {0}", GetKey(mPatches[i].List, idxSlow));
                            throw new Exception("Patch item mismatch.");
                        }
                    }
                }
            }
            // *** End of test ***
            //Console.WriteLine("Number of patches: {0}", mPatches.Count);
            //int waka = 0;
            //foreach (Patch patch in mPatches)
            //{
            //    waka += patch.List.Count;
            //}
            //Console.WriteLine("Avg list size: {0}", (double)waka / (double)mPatches.Count);
            Console.WriteLine((DateTime.Now - t).TotalMilliseconds);
            /*prof*/sw.Reset();
            mLogger.Info("Update", "Constructing system of linear equations ...");
            LabeledDataset<double, SparseVector<double>> lsqrDs = new LabeledDataset<double, SparseVector<double>>();
            Vector2D[] layout = new Vector2D[mDataset.Count - mKClust];
            foreach (Patch patch in mPatches)
            {
                int count = Math.Min(patch.List.Count, mKNn);
                SparseVector<double> eq = new SparseVector<double>();
                double wgt = 1.0 / (double)count;
                for (int i = 0; i < count; i++)
                {
                    eq.InnerIdx.Add(patch.List[i].Dat.Idx);
                    eq.InnerDat.Add(-wgt);
                }
                eq.InnerIdx.Sort(); // *** sort only indices
                eq[patch.Idx] = 1;
                lsqrDs.Add(0, eq);
            }
            for (int i = mDataset.Count - mKClust, j = 0; i < mDataset.Count; i++, j++)
            {
                SparseVector<double> eq = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(i, 1) });
                lsqrDs.Add(mRefPos[j].X, eq);
            }
            LSqrModel lsqr = new LSqrModel();
            mSolX.RemoveRange(0, numDequeue);
            double[] aux = new double[mKClust];
            mSolX.CopyTo(mSolX.Count - mKClust, aux, 0, mKClust);
            mSolX.RemoveRange(mSolX.Count - mKClust, mKClust);
            foreach (SparseVector<double> newVec in newInst)
            {
                mSolX.Add(0);
            }
            mSolX.AddRange(aux);
            lsqr.InitialSolution = mSolX.ToArray();
            lsqr.Train(lsqrDs);            
            mSolX = lsqr.Solution.GetWritableCopy();
            //for (int i = 0; i < lsqr.InitialSolution.Length; i++) 
            //{
            //    Console.WriteLine("{0}\t{1}", lsqr.InitialSolution[i], lsqr.Solution[i]);
            //}
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i].X = lsqr.Solution[i];
            }
            for (int i = lsqrDs.Count - mKClust, j = 0; i < lsqrDs.Count; i++, j++)
            {
                lsqrDs[i].Label = mRefPos[j].Y;
            }
            mSolY.RemoveRange(0, numDequeue);
            aux = new double[mKClust];
            mSolY.CopyTo(mSolY.Count - mKClust, aux, 0, mKClust);
            mSolY.RemoveRange(mSolY.Count - mKClust, mKClust);
            foreach (SparseVector<double> newVec in newInst)
            {
                mSolY.Add(0);
            }
            mSolY.AddRange(aux);
            lsqr.InitialSolution = mSolY.ToArray();
            lsqr.Train(lsqrDs);
            mSolY = lsqr.Solution.GetWritableCopy();
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i].Y = lsqr.Solution[i];
            }
            /*prof*/sw.Save("lsqr.txt", _count);
            // -----------------------------------------------------------------
            // make ptInfo
            // -----------------------------------------------------------------
            ptInfo = new PtInfo[layout.Length];
            int ii = 0;
            foreach (Vector2D pt in layout)
            {
                ptInfo[ii] = new PtInfo();
                ptInfo[ii].X = pt.X;
                ptInfo[ii].Y = pt.Y;
                ptInfo[ii].Vec = mDataset[ii];
                ii++;
            }
            // -----------------------------------------------------------------
            return settings == null ? layout : settings.AdjustLayout(layout);
        }

        private double GetKey(ArrayList<KeyDat<double, Patch>> list, int idx)
        {
            foreach (KeyDat<double, Patch> item in list)
            {
                if (item.Dat.Idx == idx) { return item.Key; }
            }
            return -1;
        }

        private class Comparer : IComparer<KeyDat<double, Patch>> 
        {
            public int Compare(KeyDat<double, Patch> x, KeyDat<double, Patch> y)
            {
                if (Math.Abs(y.Key - x.Key) > 0.000001) { return y.Key.CompareTo(x.Key); }
                return y.Dat.Idx.CompareTo(x.Dat.Idx); 
            }
        }

        private class Comparer2 : IComparer<KeyDat<double, int>>
        {
            public int Compare(KeyDat<double, int> x, KeyDat<double, int> y)
            {
                if (Math.Abs(y.Key - x.Key) > 0.000001) { return y.Key.CompareTo(x.Key); }
                return y.Dat.CompareTo(x.Dat);
            }
        }
        /* .-----------------------------------------------------------------------
           |
           |  Class DistFunc
           |
           '-----------------------------------------------------------------------
        */
        private class DistFunc : IDistance<int>
        {
            private SparseMatrix<double> mSimMtx;

            public DistFunc(SparseMatrix<double> simMtx)
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

        /* .-----------------------------------------------------------------------
           |
           |  Class Patch
           |
           '-----------------------------------------------------------------------
        */
        private class Patch
        {
            public int Idx
                = -1;
            public ArrayList<KeyDat<double, Patch>> List
                = new ArrayList<KeyDat<double, Patch>>();
            public Patch Min
                = null;
            public Patch Max
                = null;
            public double MinSim
                = -1;
            public bool NeedUpdate
                = false;

            public Patch(int idx)
            {
                Idx = idx;
            }

            public void ProcessList()
            {
                MinSim = -1;
                if (List.Count > 0) { MinSim = List.Last.Key; }
                Min = null;
                Max = null;
                foreach (KeyDat<double, Patch> item in List)
                {
                    if (Max == null || item.Dat.Idx > Max.Idx) { Max = item.Dat; }
                    if (Min == null || item.Dat.Idx < Min.Idx) { Min = item.Dat; }
                }
            }
        }
    }
}