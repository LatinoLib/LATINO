/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          SparseVectorDataset.cs
 *  Version:       1.0
 *  Desc:		   Dataset of sparse vectors
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class SparseVectorDataset<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class SparseVectorDataset<LblT> : LabeledDataset<LblT, SparseVector<double>.ReadOnly>
    {
        public SparseVectorDataset()
        {
        }

        public SparseVectorDataset(IEnumerable<LabeledExample<LblT, SparseVector<double>.ReadOnly>> examples)
        {
            m_items.AddRange(examples); // throws ArgumentNullException
        }

        public SparseVectorDataset(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        private SparseMatrix<double> GetTransposedMatrix()
        {
            SparseMatrix<double> tr_mtx = new SparseMatrix<double>();
            int row_idx = 0;
            foreach (LabeledExample<LblT, SparseVector<double>.ReadOnly> item in m_items)
            {
                foreach (IdxDat<double> vec_item in item.Example)
                {
                    if (!tr_mtx.ContainsRowAt(vec_item.Idx))
                    {
                        tr_mtx[vec_item.Idx] = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(row_idx, vec_item.Dat) });
                    }
                    else
                    {
                        tr_mtx[vec_item.Idx].InnerIdx.Add(row_idx);
                        tr_mtx[vec_item.Idx].InnerDat.Add(vec_item.Dat);
                    }
                }
                row_idx++;
            }
            return tr_mtx;
        }

        private void GetDotProductSimilarity(SparseVector<double>.ReadOnly vec, ref double[] sim_vec, ref SparseMatrix<double> tr_mtx, int start_idx)
        {
            foreach (IdxDat<double> item in vec)
            {
                SparseVector<double> col = tr_mtx[item.Idx];
                if (col != null)
                {
                    int start_idx_direct = col.GetDirectIdx(start_idx);
                    if (start_idx_direct < 0) { start_idx_direct = ~start_idx_direct; }
                    for (int i = start_idx_direct; i < col.Count; i++)
                    {
                        IdxDat<double> tr_mtx_item = col.GetDirect(i);
                        sim_vec[tr_mtx_item.Idx] += item.Dat * tr_mtx_item.Dat;
                    }
                }
            }
        }

        public double[] GetDotProductSimilarity(SparseVector<double>.ReadOnly vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            SparseMatrix<double> tr_mtx = GetTransposedMatrix();
            double[] sim_vec = new double[m_items.Count];
            GetDotProductSimilarity(vec, ref sim_vec, ref tr_mtx, /*start_idx=*/0);
            return sim_vec;
        }

        public SparseMatrix<double> GetDotProductSimilarity(double thresh, bool full_matrix) // if full_matrix is false, upper triangular sparse matrix of dot products is computed
        {
            Utils.ThrowException(thresh < 0 ? new ArgumentOutOfRangeException("thresh") : null);
            SparseMatrix<double> tr_mtx = GetTransposedMatrix();
            double[] sim_vec = new double[m_items.Count];
            SparseMatrix<double> sim_mtx = new SparseMatrix<double>();
            int row_idx = 0;
            foreach (LabeledExample<LblT, SparseVector<double>.ReadOnly> item in m_items)
            {
                GetDotProductSimilarity(item.Example, ref sim_vec, ref tr_mtx, /*start_idx=*/full_matrix ? 0 : row_idx);
                for (int idx = 0; idx < sim_vec.Length; idx++)
                {
                    double sim = sim_vec[idx];
                    if (sim > thresh)
                    {
                        if (!sim_mtx.ContainsRowAt(row_idx))
                        {
                            sim_mtx[row_idx] = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(idx, sim) });
                        }
                        else
                        {
                            sim_mtx[row_idx].InnerIdx.Add(idx);
                            sim_mtx[row_idx].InnerDat.Add(sim);
                        }
                    }
                    sim_vec[idx] = 0;
                }
                row_idx++;
            }
            return sim_mtx;
        }

        public SparseMatrix<double> GetDotProductSimilarity(double thresh) 
        {
            return GetDotProductSimilarity(thresh, /*full_matrix=*/false); // throws ArgumentOutOfRangeException
        }

        public SparseMatrix<double> GetDotProductSimilarity()
        {
            return GetDotProductSimilarity(/*thresh=*/0, /*full_matrix=*/false); 
        }
    }
}