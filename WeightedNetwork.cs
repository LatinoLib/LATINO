/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    WeightedNetwork.cs 
 *  Desc:    Wighted network data structure 
 *  Created: Jan-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino
{     
    /* .-----------------------------------------------------------------------
       |
       |  Class WeightedNetwork<VtxT>
       |
       '-----------------------------------------------------------------------
    */
    public class WeightedNetwork<VtxT> : Network<VtxT, double>, ICloneable<WeightedNetwork<VtxT>>, IDeeplyCloneable<WeightedNetwork<VtxT>>
    {
        public WeightedNetwork() : base()
        {
        }

        public WeightedNetwork(IEqualityComparer<VtxT> vtxCmp) : base(vtxCmp)
        {
        }

        public WeightedNetwork(BinarySerializer reader) : base(reader) // throws ArgumentNullException, serialization-related exceptions
        {
        }

        public WeightedNetwork(BinarySerializer reader, IEqualityComparer<VtxT> vtxCmp) : base(reader, vtxCmp) // throws ArgumentNullException, serialization-related exceptions
        {
        }

        // *** Operations ***

        public bool HasPositiveEdges()
        {
            foreach (IdxDat<SparseVector<double>> row in mMtx)
            {
                foreach (IdxDat<double> item in row.Dat)
                {
                    if (item.Dat <= 0) { return false; }
                }
            }
            return true;
        }

        public bool HasNonNegativeEdges()
        {
            foreach (IdxDat<SparseVector<double>> row in mMtx)
            {
                foreach (IdxDat<double> item in row.Dat)
                {
                    if (item.Dat < 0) { return false; }
                }
            }
            return true;
        }

        public bool ContainsZeroEdge(double eps)
        {
            Utils.ThrowException(eps < 0 ? new ArgumentOutOfRangeException("eps") : null);
            foreach (IdxDat<SparseVector<double>> row in mMtx)
            {
                foreach (IdxDat<double> item in row.Dat)
                {
                    if (Math.Abs(item.Dat) <= eps) { return true; }
                }
            }
            return false;
        }

        public bool IsUndirected(double eps)
        {
            Utils.ThrowException(eps < 0 ? new ArgumentOutOfRangeException("eps") : null);
            foreach (IdxDat<SparseVector<double>> row in mMtx)
            {
                foreach (IdxDat<double> item in row.Dat)
                {
                    if (mMtx[item.Idx] == null) { return false; }
                    int directIdx = mMtx[item.Idx].GetDirectIdx(row.Idx);
                    if (directIdx < 0) { return false; }
                    double val = mMtx[item.Idx].GetDirect(directIdx).Dat;
                    if (Math.Abs(item.Dat - val) > eps) { return false; }
                }
            }
            return true;
        }

        private KeyDat<int, int>[] GetShortestPaths(IEnumerable<int> srcVtx)
        {
            Set<KeyDat<int, int>> visited = new Set<KeyDat<int, int>>();
            foreach (int item in srcVtx) { visited.Add(new KeyDat<int, int>(item, 0)); }
            Queue<KeyDat<int, int>> queue = new Queue<KeyDat<int, int>>(visited);
            while (queue.Count > 0)
            {
                KeyDat<int, int> vtxKd = queue.Dequeue();
                SparseVector<double> vtxInfo = mMtx[vtxKd.Key];
                if (vtxInfo != null)
                {
                    foreach (IdxDat<double> item in vtxInfo)
                    {
                        if (!visited.Contains(new KeyDat<int, int>(item.Idx)))
                        {
                            KeyDat<int, int> newVtxKd = new KeyDat<int, int>(item.Idx, vtxKd.Dat + 1);
                            visited.Add(newVtxKd);
                            queue.Enqueue(newVtxKd);
                        }
                    }
                }
            }
            return visited.ToArray();
        }

        public double[] PageRank()
        {
            return PageRank(/*srcVtxList=*/null, /*damping=*/0.85); // throws InvalidOperationException, ArgumentOutOfRangeException
        }

        public double[] PageRank(IEnumerable<int> srcVtxList)
        {
            return PageRank(srcVtxList, /*damping=*/0.85); // throws InvalidOperationException, ArgumentOutOfRangeException
        }

        public double[] PageRank(IEnumerable<int> srcVtxList, double damping)
        {
            return PageRank(srcVtxList, damping, /*maxSteps=*/10000, /*eps=*/0.00001); // throws InvalidOperationException, ArgumentOutOfRangeException
        }

        public double[] PageRank(IEnumerable<int> srcVtxList, double damping, int maxSteps, double eps)
        {
            return PageRank(srcVtxList, damping, maxSteps, eps, /*initPr=*/null, /*noBounding=*/true, /*inlinks=*/null); // throws InvalidOperationException, ArgumentOutOfRangeException
        }

        public double[] PageRank(IEnumerable<int> srcVtxList, double damping, int maxSteps, double eps, double[] initPr, bool noBounding, SparseMatrix<double>.ReadOnly inlinks)
        {            
            Utils.ThrowException((mVtx.Count == 0 || !HasPositiveEdges()) ? new InvalidOperationException() : null);
            Utils.ThrowException((damping < 0 || damping >= 1) ? new ArgumentOutOfRangeException("damping") : null);
            Utils.ThrowException(maxSteps <= 0 ? new ArgumentOutOfRangeException("maxSteps") : null);
            Utils.ThrowException(eps < 0 ? new ArgumentOutOfRangeException("eps") : null);
            Utils.ThrowException((initPr != null && initPr.Length != mVtx.Count) ? new ArgumentValueException("initPr") : null);
            // *** inlinks needs to be the transposed form of this.Edges; to check if this is true, uncomment the following line            
            //Utils.ThrowException((inlinks != null && !((SparseMatrix<double>.ReadOnly)mMtx.GetTransposedCopy()).ContentEquals(inlinks)) ? new ArgumentValueException("inlinks") : null);            
            int srcVtxCount = 0; 
            if (srcVtxList != null)
            {
                foreach (int srcVtxIdx in srcVtxList)
                {
                    srcVtxCount++;
                    Utils.ThrowException((srcVtxIdx < 0 || srcVtxIdx >= mVtx.Count) ? new ArgumentOutOfRangeException("srcVtxList item") : null);
                }
            }
            // precompute weight sums
            double[] wgtSum = new double[mVtx.Count];
            for (int i = 0; i < mVtx.Count; i++)
            {
                wgtSum[i] = 0;
                if (mMtx[i] != null)
                {
                    foreach (IdxDat<double> otherVtx in mMtx[i])
                    {
                        wgtSum[i] += otherVtx.Dat;
                    }
                }
            }
            // initialize rankVec
            double[] rankVec = new double[mVtx.Count];            
            if (initPr != null)
            {
                double rankSum = 0;
                foreach (double val in initPr) 
                { 
                    rankSum += val;
                    Utils.ThrowException(val < 0 ? new ArgumentOutOfRangeException("initPr item") : null);
                }
                Utils.ThrowException(rankSum == 0 ? new ArgumentValueException("initPr") : null);
                for (int vtxIdx = 0; vtxIdx < mVtx.Count; vtxIdx++)
                {
                    rankVec[vtxIdx] = initPr[vtxIdx] / rankSum; 
                }
            }
            else
            {
                if (srcVtxCount == 0)
                {
                    double initRank = 1.0 / (double)mVtx.Count;
                    for (int vtxIdx = 0; vtxIdx < mVtx.Count; vtxIdx++) { rankVec[vtxIdx] = initRank; }
                }
                else
                {
                    double initRank = 1.0 / (double)srcVtxCount;
                    for (int vtxIdx = 0; vtxIdx < mVtx.Count; vtxIdx++) { rankVec[vtxIdx] = 0; }
                    foreach (int srcVtxIdx in srcVtxList) { rankVec[srcVtxIdx] = initRank; }
                }
            }
            //Console.WriteLine(new ArrayList<double>(rankVec));
            // transpose adjacency matrix
            if (inlinks == null)
            {
                inlinks = mMtx.GetTransposedCopy();
            }
            // compute shortest paths
            KeyDat<int, int>[] vtxInfo = null;
            if (srcVtxCount > 0 && !noBounding)
            {
                KeyDat<int, int>[] tmp = GetShortestPaths(srcVtxList);
                vtxInfo = new KeyDat<int, int>[tmp.Length];
                int i = 0;
                foreach (KeyDat<int, int> item in tmp) { vtxInfo[i++] = new KeyDat<int, int>(item.Dat, item.Key); }
                Array.Sort(vtxInfo);
            }
            // main loop
            int step = 0;
            double diff;       
            do
            {
                //DateTime then = DateTime.Now;
                // compute new Page Rank for each vertex
                double[] newRankVec = new double[mVtx.Count];
                if (srcVtxCount == 0 || noBounding)
                {
                    for (int vtxIdx = 0; vtxIdx < mVtx.Count; vtxIdx++)
                    {
                        double newRank = 0;
                        if (inlinks.ContainsRowAt(vtxIdx))
                        {
                            foreach (IdxDat<double> otherVtx in inlinks[vtxIdx])
                            {
                                newRank += otherVtx.Dat / wgtSum[otherVtx.Idx] * (double)rankVec[otherVtx.Idx];
                            }
                        }
                        newRankVec[vtxIdx] = newRank;
                    }
                    //Console.WriteLine("< {0} >", new ArrayList<double>(newRankVec));
                }
                else
                {
                    for (int i = 0; i < vtxInfo.Length && (initPr != null || vtxInfo[i].Key <= step + 1); i++)
                    {
                        int vtxIdx = vtxInfo[i].Dat;
                        double newRank = 0;
                        if (inlinks.ContainsRowAt(vtxIdx))
                        {
                            foreach (IdxDat<double> otherVtx in inlinks[vtxIdx])
                            {
                                newRank += otherVtx.Dat / wgtSum[otherVtx.Idx] * (double)rankVec[otherVtx.Idx];
                            }
                        }
                        newRankVec[vtxIdx] = newRank;
                    }
                }
                // normalize newRankVec by distributing (1.0 - rankSum) to source vertices
                double rankSum = 0;
                for (int i = 0; i < newRankVec.Length; i++) { rankSum += newRankVec[i]; }
                if (rankSum <= 0.999999)
                {
                    if (srcVtxCount == 0)
                    {
                        double distrRank = (1.0 - rankSum) / (double)mVtx.Count;
                        for (int i = 0; i < newRankVec.Length; i++) { newRankVec[i] += distrRank; }
                    }
                    else
                    {
                        double distrRank = (1.0 - rankSum) / (double)srcVtxCount;
                        foreach (int srcVtxIdx in srcVtxList) { newRankVec[srcVtxIdx] += distrRank; }
                    }
                }
                // incorporate damping factor
                if (srcVtxCount == 0)
                {
                    double distrRank = (1.0 - damping) / (double)mVtx.Count;
                    for (int i = 0; i < newRankVec.Length; i++) { newRankVec[i] = damping * newRankVec[i] + distrRank; }
                }
                else
                {
                    double distrRank = (1.0 - damping) / (double)srcVtxCount;
                    for (int i = 0; i < newRankVec.Length; i++) { newRankVec[i] = damping * newRankVec[i]; }
                    foreach (int srcVtxIdx in srcVtxList) { newRankVec[srcVtxIdx] += distrRank; }
                }
                // compute difference
                diff = 0;
                for (int i = 0; i < mVtx.Count; i++)
                {
                    diff += Math.Abs(rankVec[i] - newRankVec[i]);
                }
                rankVec = newRankVec;
                //Console.WriteLine(new ArrayList<double>(rankVec));
                step++;
                //Console.WriteLine("Step {0}\tTime {1}", step, (DateTime.Now - then).TotalMilliseconds);
            } while (step < maxSteps && diff > eps);
            return rankVec;
        }

        public SparseMatrix<double> GetLaplacianMatrix()
        {
            Utils.ThrowException(!IsUndirected(1E-6) ? new InvalidOperationException() : null);
            SparseMatrix<double> lapl = mMtx.Clone();
            double[] diag = new double[mMtx.GetLastNonEmptyColIdx() + 1];
            foreach (IdxDat<SparseVector<double>> row in mMtx)
            {
                foreach (IdxDat<double> item in row.Dat)
                {
                    if (row.Idx != item.Idx)
                    {
                        diag[item.Idx] += item.Dat;
                    }
                }
            }
            lapl.PerformUnaryOperation(delegate(double val) { return -val; });
            for (int i = 0; i < diag.Length; i++)
            {
                if (diag[i] != 0)
                {
                    lapl[i, i] = diag[i];
                }
            }
            return lapl;
        }

        public SparseMatrix<double> SimRank(double damping, int maxSteps/*, double eps*/)
        {
            Utils.ThrowException(mVtx.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException((damping < 0 || damping >= 1) ? new ArgumentOutOfRangeException("damping") : null);
            Utils.ThrowException(maxSteps <= 0 ? new ArgumentOutOfRangeException("maxSteps") : null);
            //Utils.ThrowException(eps < 0 ? new ArgumentOutOfRangeException("eps") : null);
            SparseMatrix<double> simRank = new SparseMatrix<double>();
            // initialize SimRank
            for (int i = 0; i < mVtx.Count; i++)
            {
                simRank[i] = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(i, 1) });
            }
            // main loop
            int step = 0;
            do
            {
                for (int i = 0; i < mVtx.Count; i++)
                {
                    for (int j = 0; j < mVtx.Count; j++)
                    {
                        if (i != j && mMtx[i] != null && mMtx[j] != null)
                        {
                            SparseVector<double> mtx_i = mMtx[i];
                            SparseVector<double> mtx_j = mMtx[j];
                            double sumSum = 0;
                            foreach (IdxDat<double> item_i in mtx_i)
                            {
                                if (simRank.ContainsRowAt(item_i.Idx))
                                {
                                    foreach (IdxDat<double> item_j in mtx_j)
                                    {
                                        int idx = simRank[item_i.Idx].GetDirectIdx(item_j.Idx);
                                        if (idx >= 0)
                                        {
                                            sumSum += simRank[item_i.Idx].GetDatDirect(idx);
                                        }
                                    }
                                }
                            }
                            double sim = damping / (double)(mtx_i.Count * mtx_j.Count) * sumSum;
                            if (sim > 0) { simRank[i, j] = sim; } // threshold!!!
                        }
                    }
                }
                step++;
            } while (step < maxSteps /*&& diff > eps*/);
            return simRank;
        }

        // *** ICloneable<Network<VtxT, EdgeT>> interface adaptation ***

        new public WeightedNetwork<VtxT> Clone()
        {
            WeightedNetwork<VtxT> clone = new WeightedNetwork<VtxT>();
            clone.mVtx = mVtx.Clone();
            clone.mMtx = mMtx.Clone();
            int i = 0;
            foreach (VtxT vtx in clone.mVtx)
            {
                if (vtx != null)
                {
                    clone.mVtxToIdx.Add(vtx, i++);
                }
            }
            return clone;
        }

        // *** IDeeplyCloneable<Network<VtxT, EdgeT>> interface adaptation ***

        new public WeightedNetwork<VtxT> DeepClone()
        {
            WeightedNetwork<VtxT> clone = new WeightedNetwork<VtxT>();
            clone.mVtx = mVtx.DeepClone();
            clone.mMtx = mMtx.DeepClone();
            int i = 0;
            foreach (VtxT vtx in clone.mVtx)
            {
                if (vtx != null)
                {
                    clone.mVtxToIdx.Add(vtx, i++);
                }
            }
            return clone;
        }
    }
}