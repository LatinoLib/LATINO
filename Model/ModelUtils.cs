/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          ModelUtils.cs
 *  Version:       1.0
 *  Desc:		   Fundamental ML-related utilities
 *  Author:        Miha Grcar
 *  Created on:    Aug-2008
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
       |  Enum GroupClassifyMethod
       |
       '-----------------------------------------------------------------------
    */
    public enum GroupClassifyMethod
    {
        Sum,
        Max,
        Vote
    }

    /* .-----------------------------------------------------------------------
       |
       |  Enum CentroidType
       |
       '-----------------------------------------------------------------------
    */
    public enum CentroidType
    {
        Sum,
        Avg,
        NrmL2
    }

    /* .-----------------------------------------------------------------------
       |
       |  Static class ModelUtils
       |
       '-----------------------------------------------------------------------
    */
    public static class ModelUtils
    {
        // *** Conversion of examples ***

        public static object ConvertExample(object in_vec, Type out_vec_type)
        {
            Utils.ThrowException(in_vec == null ? new ArgumentNullException("in_vec") : null);
            Utils.ThrowException(out_vec_type == null ? new ArgumentNullException("out_vec_type") : null);
            // special cases (fast conversions)
            if (in_vec.GetType() == out_vec_type)
            {
                return in_vec;
            }
            // general conversion routine (through SparseVector)
            SparseVector<double> tmp;
            if (in_vec.GetType() == typeof(SparseVector<double>))
            {
                tmp = (SparseVector<double>)in_vec;
            }
            else if (in_vec.GetType() == typeof(SparseVector<double>.ReadOnly))
            {
                tmp = ((SparseVector<double>.ReadOnly)in_vec).GetWritableCopy();
            }
            else if (in_vec.GetType() == typeof(BinaryVector<int>) || in_vec.GetType() == typeof(BinaryVector<int>.ReadOnly))
            {
                tmp = new SparseVector<double>(((BinaryVector<int>.ReadOnly)in_vec).Count);
                foreach (int item in (BinaryVector<int>.ReadOnly)in_vec)
                {
                    tmp.InnerIdx.Add(item);
                    tmp.InnerDat.Add(1);
                }
            }
            //else if (in_vec.GetType() == typeof(SvmFeatureVector))
            //{
            //    tmp = new SparseVector<double>(((SvmFeatureVector)in_vec).Count);
            //    foreach (IdxDat<float> item in (SvmFeatureVector)in_vec)
            //    {
            //        tmp.Inner.Add(new IdxDat<double>(item.Idx, item.Dat));
            //    }
            //}        
            else
            {
                throw new ArgumentTypeException("in_vec");
            }
            object out_vec = null;
            if (out_vec_type == typeof(SparseVector<double>))
            {
                out_vec = tmp;
            }
            else if (out_vec_type == typeof(SparseVector<double>.ReadOnly))
            {
                out_vec = new SparseVector<double>.ReadOnly(tmp);
            }
            else if (out_vec_type == typeof(BinaryVector<int>))
            {
                out_vec = new BinaryVector<int>();
                ((BinaryVector<int>)out_vec).Inner.AddRange(tmp.InnerIdx);
            }
            else if (out_vec_type == typeof(BinaryVector<int>.ReadOnly))
            {
                BinaryVector<int> vec = new BinaryVector<int>();
                vec.Inner.AddRange(tmp.InnerIdx);
                out_vec = new BinaryVector<int>.ReadOnly(vec);
            }
            //else if (out_vec_type == typeof(SvmFeatureVector))
            //{
            //    SparseVector<float> tmp_2 = new SparseVector<float>(tmp.Count);
            //    foreach (IdxDat<double> item in tmp)
            //    {
            //        tmp_2.Inner.Add(new IdxDat<float>(item.Idx, (float)item.Dat)); // *** casting double to float
            //    }
            //    out_vec = new SvmFeatureVector(tmp_2);
            //}
            else
            {
                throw new ArgumentValueException("out_vec_type");
            }
            return out_vec;
        }

        public static OutVecT ConvertExample<OutVecT>(object in_vec)
        {
            return (OutVecT)ConvertExample(in_vec, typeof(OutVecT)); // throws ArgumentNullException, ArgumentTypeException, ArgumentValueException
        }

        // *** Classification of groups of examples ***

        public static Prediction<LblT> ClassifyGroup<LblT, ExT>(IEnumerable<ExT> examples, IModel<LblT, ExT> model)
        {
            return ClassifyGroup<LblT, ExT>(examples, model, GroupClassifyMethod.Sum, /*lbl_cmp=*/null); // throws InvalidOperationException, ArgumentNullException
        }

        public static Prediction<LblT> ClassifyGroup<LblT, ExT>(IEnumerable<ExT> examples, IModel<LblT, ExT> model, GroupClassifyMethod method)
        {
            return ClassifyGroup<LblT, ExT>(examples, model, method, /*lbl_cmp=*/null); // throws InvalidOperationException, ArgumentNullException
        }

        public static Prediction<LblT> ClassifyGroup<LblT, ExT>(IEnumerable<ExT> examples, IModel<LblT, ExT> model, GroupClassifyMethod method, IEqualityComparer<LblT> lbl_cmp)
        {
            Dictionary<LblT, double> tmp = new Dictionary<LblT, double>(lbl_cmp);
            foreach (ExT example in examples)
            {
                Prediction<LblT> result = model.Predict(example); // throws InvalidOperationException, ArgumentNullException
                foreach (KeyDat<double, LblT> lbl_info in result)
                {
                    if (method == GroupClassifyMethod.Vote)
                    {
                        if (!tmp.ContainsKey(lbl_info.Dat)) 
                        { 
                            tmp.Add(lbl_info.Dat, 1); 
                        } 
                        else 
                        { 
                            tmp[lbl_info.Dat]++; 
                        }
                        break;
                    }
                    else
                    {
                        if (!tmp.ContainsKey(lbl_info.Dat))
                        {
                            tmp.Add(lbl_info.Dat, lbl_info.Key);
                        }
                        else
                        {
                            switch (method)
                            {
                                case GroupClassifyMethod.Max:
                                    tmp[lbl_info.Dat] = Math.Max(lbl_info.Key, tmp[lbl_info.Dat]);
                                    break;
                                case GroupClassifyMethod.Sum:
                                    tmp[lbl_info.Dat] += lbl_info.Key;
                                    break;
                            }
                        }
                    }
                }
            }
            Prediction<LblT> aggr_result = new Prediction<LblT>();
            foreach (KeyValuePair<LblT, double> item in tmp)
            {
                aggr_result.Items.Add(new KeyDat<double, LblT>(item.Value, item.Key));
            }
            aggr_result.Items.Sort(new DescSort<KeyDat<double, LblT>>());
            return aggr_result;
        }

        // *** Computation of centroids ***

        public static SparseVector<double> ComputeCentroid(IEnumerable<SparseVector<double>.ReadOnly> vec_list, CentroidType type)
        {
            Utils.ThrowException(vec_list == null ? new ArgumentNullException("vec_list") : null);
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            int vec_count = 0;
            foreach (SparseVector<double>.ReadOnly vec in vec_list)
            {
                foreach (IdxDat<double> item in vec)
                {
                    if (tmp.ContainsKey(item.Idx))
                    {
                        tmp[item.Idx] += item.Dat;
                    }
                    else
                    {
                        tmp.Add(item.Idx, item.Dat);
                    }
                }
                vec_count++;
            }
            Utils.ThrowException(vec_count == 0 ? new ArgumentValueException("vec_list") : null);
            SparseVector<double> centroid = new SparseVector<double>();
            switch (type)
            {
                case CentroidType.Sum:
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value);
                    }                    
                    break;
                case CentroidType.Avg:                    
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value / (double)vec_count);
                    }
                    break;
                case CentroidType.NrmL2:
                    double vec_len = 0;
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        vec_len += item.Value * item.Value;
                    }
                    Utils.ThrowException(vec_len == 0 ? new InvalidOperationException() : null);
                    vec_len = Math.Sqrt(vec_len);
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value / vec_len);
                    }
                    break;
            }
            centroid.Sort();
            return centroid;
        }

        public static SparseVector<double> ComputeCentroidWgt(IEnumerable<Pair<double, SparseVector<double>.ReadOnly>> wgt_vec_list, CentroidType type)
        {
            Utils.ThrowException(wgt_vec_list == null ? new ArgumentNullException("wgt_vec_list") : null);
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            double wgt_sum = 0;
            foreach (Pair<double, SparseVector<double>.ReadOnly> wgt_vec in wgt_vec_list)
            {
                foreach (IdxDat<double> item in wgt_vec.Second)
                {
                    if (tmp.ContainsKey(item.Idx))
                    {
                        tmp[item.Idx] += wgt_vec.First * item.Dat;
                    }
                    else
                    {
                        tmp.Add(item.Idx, wgt_vec.First * item.Dat);
                    }
                }
                wgt_sum += wgt_vec.First;
            }
            Utils.ThrowException(wgt_sum == 0 ? new ArgumentValueException("wgt_vec_list") : null);
            SparseVector<double> centroid = new SparseVector<double>();
            switch (type)
            {
                case CentroidType.Sum:
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value);
                    }
                    break;
                case CentroidType.Avg:
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value / wgt_sum);
                    }
                    break;
                case CentroidType.NrmL2:
                    double vec_len = 0;
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        vec_len += item.Value * item.Value;
                    }
                    Utils.ThrowException(vec_len == 0 ? new InvalidOperationException() : null);
                    vec_len = Math.Sqrt(vec_len);
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value / vec_len);
                    }
                    break;
            }
            centroid.Sort();
            return centroid;
        }

        // *** IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> template specialization ***

        private static void GetDotProductSimilarity(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, SparseVector<double>.ReadOnly vec, ref double[] sim_vec, ref SparseMatrix<double> tr_mtx, int start_idx)
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

        private static SparseMatrix<double> GetTransposedMatrix(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            SparseMatrix<double> tr_mtx = new SparseMatrix<double>();
            int row_idx = 0;
            foreach (SparseVector<double>.ReadOnly item in dataset)
            {
                foreach (IdxDat<double> vec_item in item)
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

        public static double[] GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, SparseVector<double>.ReadOnly vec)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            SparseMatrix<double> tr_mtx = GetTransposedMatrix(dataset);
            double[] sim_vec = new double[dataset.Count];
            GetDotProductSimilarity(dataset, vec, ref sim_vec, ref tr_mtx, /*start_idx=*/0);
            return sim_vec;
        }

        public static SparseMatrix<double> GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, double thresh, bool full_matrix) // if full_matrix is false, upper (right) triangular sparse matrix of dot products is computed
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(thresh < 0 ? new ArgumentOutOfRangeException("thresh") : null);
            SparseMatrix<double> tr_mtx = GetTransposedMatrix(dataset);
            double[] sim_vec = new double[dataset.Count];
            SparseMatrix<double> sim_mtx = new SparseMatrix<double>();
            int row_idx = 0;
            foreach (SparseVector<double>.ReadOnly item in dataset)
            {
                GetDotProductSimilarity(dataset, item, ref sim_vec, ref tr_mtx, /*start_idx=*/full_matrix ? 0 : row_idx);
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

        public static SparseMatrix<double> GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, double thresh)
        {
            return GetDotProductSimilarity(dataset, thresh, /*full_matrix=*/false); // throws ArgumentOutOfRangeException, ArgumentNullException
        }

        public static SparseMatrix<double> GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            return GetDotProductSimilarity(dataset, /*thresh=*/0, /*full_matrix=*/false); // throws ArgumentNullException
        }
    }
}
