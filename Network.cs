/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Network.cs 
 *  Desc:    Network data structure 
 *  Created: Jan-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Network<VtxT, EdgeT>
       |
       '-----------------------------------------------------------------------
    */
    public class Network<VtxT, EdgeT> : ICloneable<Network<VtxT, EdgeT>>, IDeeplyCloneable<Network<VtxT, EdgeT>>, IContentEquatable<Network<VtxT, EdgeT>>, 
        ISerializable
    {
        protected SparseMatrix<EdgeT> mMtx
            = new SparseMatrix<EdgeT>();
        protected ArrayList<VtxT> mVtx 
            = new ArrayList<VtxT>();        
        protected Dictionary<VtxT, int> mVtxToIdx;

        public Network()
        {
            mVtxToIdx = new Dictionary<VtxT, int>();
        }

        public Network(IEqualityComparer<VtxT> vtxCmp)
        {
            mVtxToIdx = new Dictionary<VtxT, int>(vtxCmp);
        }

        public Network(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public Network(BinarySerializer reader, IEqualityComparer<VtxT> vtxCmp)
        {
            Load(reader, vtxCmp); // throws ArgumentNullException, serialization-related exceptions
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < mVtx.Count; i++)
            {
                str.Append(mVtx[i]);
                str.Append(": { ");
                if (mMtx[i] != null)
                {
                    foreach (IdxDat<EdgeT> vtxInfo in mMtx[i])
                    {
                        str.Append(string.Format("( {0} {1} ) ", mVtx[vtxInfo.Idx], vtxInfo.Dat));
                    }                    
                }
                str.AppendLine("}");
            }
            return str.ToString().TrimEnd('\n', '\r');
        }

        public string ToString(string format)
        {
            if (format == "DEF") // default
            {
                return ToString();
            }
            else if (format == "AMC") // adjacency matrix - compact
            {
                return mMtx.ToString("C");
            }
            else if (format == "AME") // adjacency matrix - extended
            {
                return mMtx.ToString("E");
            }
            else
            {
                throw new ArgumentNotSupportedException("format");
            }
        }   
     
        // *** Vertices ***

        public int AddVertex(VtxT vtx)
        {
            mVtx.Add(vtx);
            if (vtx != null) { mVtxToIdx.Add(vtx, mVtx.Count - 1); } // throws ArgumentException
            return mVtx.Count - 1;
        }

        public void SetVertexAt(int idx, VtxT vtx)
        {
            if (mVtx[idx] != null) { mVtxToIdx.Remove(mVtx[idx]); } // throws ArgumentOutOfRangeException
            mVtx[idx] = vtx;
            if (vtx != null) { mVtxToIdx.Add(vtx, idx); } // throws ArgumentException
        }

        public void RemoveVertexAt(int idx)
        {
            if (mVtx[idx] != null) { mVtxToIdx.Remove(mVtx[idx]); } // throws ArgumentOutOfRangeException
            for (int i = idx + 1; i < mVtx.Count; i++)
            {
                if (mVtx[i] != null) { mVtxToIdx[mVtx[i]]--; }
            }
            mVtx.RemoveAt(idx);
            mMtx.PurgeColAt(idx);
            mMtx.PurgeRowAt(idx);
        }

        public void RemoveVertex(VtxT vtx)
        {
            RemoveVertexAt(mVtxToIdx[vtx]); // throws ArgumentNullException, KeyNotFoundException
        }

        public bool IsVertex(VtxT vtx)
        {
            return mVtxToIdx.ContainsKey(vtx); // throws ArgumentNullException
        }

        public int GetVertexIdx(VtxT vtx)
        {
            return mVtxToIdx[vtx]; // throws ArgumentNullException, KeyNotFoundException
        }

        public ArrayList<VtxT>.ReadOnly Vertices
        {
            get { return mVtx; }
        }

        // *** Edges ***

        public void SetEdgeAt(int vtx1Idx, int vtx2Idx, EdgeT val)
        {
            Utils.ThrowException((vtx1Idx < 0 || vtx1Idx >= mVtx.Count) ? new ArgumentOutOfRangeException("vtx1Idx") : null);
            Utils.ThrowException((vtx2Idx < 0 || vtx2Idx >= mVtx.Count) ? new ArgumentOutOfRangeException("vtx2Idx") : null);
            mMtx[vtx1Idx, vtx2Idx] = val; // throws ArgumentNullException
        }

        public void SetEdge(VtxT vtx1, VtxT vtx2, EdgeT val)
        {
            int vtx1Idx = mVtxToIdx[vtx1], vtx2Idx = mVtxToIdx[vtx2]; // throws ArgumentNullException, KeyNotFoundException            
            mMtx[vtx1Idx, vtx2Idx] = val; // throws ArgumentNullException
        }

        public bool IsEdgeAt(int vtx1Idx, int vtx2Idx)
        {
            Utils.ThrowException((vtx1Idx < 0 || vtx1Idx >= mVtx.Count) ? new ArgumentOutOfRangeException("vtx1Idx") : null);
            Utils.ThrowException((vtx2Idx < 0 || vtx2Idx >= mVtx.Count) ? new ArgumentOutOfRangeException("vtx2Idx") : null);
            return mMtx.ContainsAt(vtx1Idx, vtx2Idx);
        }

        public bool IsEdge(VtxT vtx1, VtxT vtx2)
        {
            int vtx1Idx = mVtxToIdx[vtx1], vtx2Idx = mVtxToIdx[vtx2]; // throws ArgumentNullException, KeyNotFoundException
            return mMtx.ContainsAt(vtx1Idx, vtx2Idx);
        }

        public EdgeT GetEdgeAt(int vtx1Idx, int vtx2Idx)
        {
            Utils.ThrowException((vtx1Idx < 0 || vtx1Idx >= mVtx.Count) ? new ArgumentOutOfRangeException("vtx1Idx") : null);
            Utils.ThrowException((vtx2Idx < 0 || vtx2Idx >= mVtx.Count) ? new ArgumentOutOfRangeException("vtx2Idx") : null);
            return mMtx[vtx1Idx, vtx2Idx]; // throws ArgumentValueException
        }

        public EdgeT GetEdge(VtxT vtx1, VtxT vtx2)
        {
            int vtx1Idx = mVtxToIdx[vtx1], vtx2Idx = mVtxToIdx[vtx2]; // throws ArgumentNullException, KeyNotFoundException            
            return mMtx[vtx1Idx, vtx2Idx]; // throws ArgumentValueException
        }

        public void RemoveEdgeAt(int vtx1Idx, int vtx2Idx)
        {
            Utils.ThrowException((vtx1Idx < 0 || vtx1Idx >= mVtx.Count) ? new ArgumentOutOfRangeException("vtx1Idx") : null);
            Utils.ThrowException((vtx2Idx < 0 || vtx2Idx >= mVtx.Count) ? new ArgumentOutOfRangeException("vtx2Idx") : null);
            mMtx.RemoveAt(vtx1Idx, vtx2Idx);
        }

        public void RemoveEdges()
        {
            mMtx.Clear();
        }

        public void RemoveEdge(VtxT vtx1, VtxT vtx2)
        {
            int vtx1Idx = mVtxToIdx[vtx1], vtx2Idx = mVtxToIdx[vtx2]; // throws ArgumentNullException, KeyNotFoundException            
            mMtx.RemoveAt(vtx1Idx, vtx2Idx); 
        }

        public SparseMatrix<EdgeT>.ReadOnly Edges
        {
            get { return mMtx; }
        }

        public void ClearEdges()
        {
            mMtx.Clear();
        }

        // *** Operations ***

        public void Clear()
        {
            mMtx.Clear();
            mVtx.Clear();
            mVtxToIdx.Clear();
        }

        public void PerformEdgeOperation(Utils.UnaryOperatorDelegate<EdgeT> unaryOp)
        {
            mMtx.PerformUnaryOperation(unaryOp); // throws ArgumentNullException
        }

        public void ToUndirected(Utils.BinaryOperatorDelegate<EdgeT> binOp)
        {
            mMtx.Symmetrize(binOp); // throws ArgumentNullException
        }

        public bool IsUndirected()
        {
            return mMtx.IsSymmetric();
        }

        public void SetLoops(EdgeT val)
        {
            mMtx.SetDiagonal(mVtx.Count, val); // throws ArgumentNullException
        }

        public void RemoveLoops()
        {
            mMtx.RemoveDiagonal();
        }

        public bool ContainsLoop()
        {
            return mMtx.ContainsDiagonalElement();
        }

        public void InvertEdges()
        {
            mMtx = mMtx.GetTransposedCopy();
        }

        public double GetSparseness()
        {
            return mMtx.GetSparseness(mVtx.Count, mVtx.Count); // throws ArgumentException
        }

        public SparseMatrix<EdgeT>[] GetComponentsUndirected(ref int[] seeds, bool seedsOnly)
        {
            Utils.ThrowException(!IsUndirected() ? new InvalidOperationException() : null);
            ArrayList<int> seedList = new ArrayList<int>();
            ArrayList<SparseMatrix<EdgeT>> components = new ArrayList<SparseMatrix<EdgeT>>();
            Set<int> unvisited = new Set<int>();
            for (int j = 0; j < mVtx.Count; j++) { unvisited.Add(j); }
            while (unvisited.Count > 0)
            {
                int seedIdx = unvisited.Any;
                SparseMatrix<EdgeT> component = new SparseMatrix<EdgeT>();
                seedList.Add(seedIdx);
                Queue<int> queue = new Queue<int>(new int[] { seedIdx });
                unvisited.Remove(seedIdx);
                while (queue.Count > 0)
                {
                    int vtxIdx = queue.Dequeue();
                    SparseVector<EdgeT> vtxInfo = mMtx[vtxIdx];
                    if (vtxInfo != null)
                    {
                        if (!seedsOnly)
                        {
                            component[vtxIdx] = vtxInfo.Clone();
                        }
                        foreach (IdxDat<EdgeT> otherVtx in vtxInfo)
                        {
                            if (unvisited.Contains(otherVtx.Idx))
                            {
                                unvisited.Remove(otherVtx.Idx);                                
                                queue.Enqueue(otherVtx.Idx);
                            }
                        }
                    }
                }
                if (!seedsOnly /*&& component.GetLastNonEmptyRowIdx() >= 0*/)
                {
                    components.Add(component);
                }
            }
            seeds = seedList.ToArray();           
            return components.ToArray();
        }

        // *** ICloneable<Network<VtxT, EdgeT>> interface implementation ***

        public Network<VtxT, EdgeT> Clone()
        {
            Network<VtxT, EdgeT> clone = new Network<VtxT, EdgeT>();
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

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable<Network<VtxT, EdgeT>> interface implementation ***

        public Network<VtxT, EdgeT> DeepClone()
        {
            Network<VtxT, EdgeT> clone = new Network<VtxT, EdgeT>();
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

        object IDeeplyCloneable.DeepClone()
        {
            return DeepClone();
        }

        // *** IContentEquatable<Network<VtxT, EdgeT>> interface implementation ***

        public bool ContentEquals(Network<VtxT, EdgeT> other)
        {
            return other != null && mMtx.ContentEquals(other.mMtx) && mVtx.ContentEquals(other.mVtx);
        }

        public bool ContentEquals(object other)
        {
            Utils.ThrowException((other != null && !(other is Network<VtxT, EdgeT>)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((Network<VtxT, EdgeT>)other);
        }

        // *** ISerializable interface implementation ***

        public void Load(BinarySerializer reader)
        {
            Load(reader, /*vtxCmp=*/null); // throws ArgumentNullException, serialization-related exceptions
        }

        public void Load(BinarySerializer reader, IEqualityComparer<VtxT> vtxCmp)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mMtx.Load(reader);
            mVtx.Load(reader);
            mVtxToIdx = new Dictionary<VtxT, int>(vtxCmp);
            int i = 0;
            foreach (VtxT vtx in mVtx)
            {
                if (vtx != null)
                {
                    mVtxToIdx.Add(vtx, i++);
                }
            }
        }

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            mMtx.Save(writer);
            mVtx.Save(writer);
        }
    }
}
