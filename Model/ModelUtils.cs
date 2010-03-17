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
       |  Class ModelUtils
       |
       '-----------------------------------------------------------------------
    */
    public static class ModelUtils
    {
        // *** Conversion of examples ***

        public static object ConvertExample(object inVec, Type outVecType)
        {
            Utils.ThrowException(inVec == null ? new ArgumentNullException("inVec") : null);
            Utils.ThrowException(outVecType == null ? new ArgumentNullException("outVecType") : null);
            // special cases (fast conversions)
            if (inVec.GetType() == outVecType)
            {
                return inVec;
            }
            // general conversion routine (through SparseVector)
            SparseVector<double> tmp;
            if (inVec.GetType() == typeof(SparseVector<double>))
            {
                tmp = (SparseVector<double>)inVec;
            }
            else if (inVec.GetType() == typeof(SparseVector<double>.ReadOnly))
            {
                tmp = ((SparseVector<double>.ReadOnly)inVec).GetWritableCopy();
            }
            else if (inVec.GetType() == typeof(BinaryVector<int>) || inVec.GetType() == typeof(BinaryVector<int>.ReadOnly))
            {
                tmp = new SparseVector<double>(((BinaryVector<int>.ReadOnly)inVec).Count);
                foreach (int item in (BinaryVector<int>.ReadOnly)inVec)
                {
                    tmp.InnerIdx.Add(item);
                    tmp.InnerDat.Add(1);
                }
            }
            //else if (inVec.GetType() == typeof(SvmFeatureVector))
            //{
            //    tmp = new SparseVector<double>(((SvmFeatureVector)inVec).Count);
            //    foreach (IdxDat<float> item in (SvmFeatureVector)inVec)
            //    {
            //        tmp.Inner.Add(new IdxDat<double>(item.Idx, item.Dat));
            //    }
            //}        
            else
            {
                throw new ArgumentTypeException("inVec");
            }
            object outVec = null;
            if (outVecType == typeof(SparseVector<double>))
            {
                outVec = tmp;
            }
            else if (outVecType == typeof(SparseVector<double>.ReadOnly))
            {
                outVec = new SparseVector<double>.ReadOnly(tmp);
            }
            else if (outVecType == typeof(BinaryVector<int>))
            {
                outVec = new BinaryVector<int>();
                ((BinaryVector<int>)outVec).Inner.AddRange(tmp.InnerIdx);
            }
            else if (outVecType == typeof(BinaryVector<int>.ReadOnly))
            {
                BinaryVector<int> vec = new BinaryVector<int>();
                vec.Inner.AddRange(tmp.InnerIdx);
                outVec = new BinaryVector<int>.ReadOnly(vec);
            }
            //else if (outVecType == typeof(SvmFeatureVector))
            //{
            //    SparseVector<float> tmp2 = new SparseVector<float>(tmp.Count);
            //    foreach (IdxDat<double> item in tmp)
            //    {
            //        tmp2.Inner.Add(new IdxDat<float>(item.Idx, (float)item.Dat)); // *** casting double to float
            //    }
            //    outVec = new SvmFeatureVector(tmp2);
            //}
            else
            {
                throw new ArgumentValueException("outVecType");
            }
            return outVec;
        }

        public static OutVecT ConvertExample<OutVecT>(object inVec)
        {
            return (OutVecT)ConvertExample(inVec, typeof(OutVecT)); // throws ArgumentNullException, ArgumentTypeException, ArgumentValueException
        }

        // *** Classification of groups of examples ***

        public static Prediction<LblT> ClassifyGroup<LblT, ExT>(IEnumerable<ExT> examples, IModel<LblT, ExT> model)
        {
            return ClassifyGroup<LblT, ExT>(examples, model, GroupClassifyMethod.Sum, /*lblCmp=*/null); // throws InvalidOperationException, ArgumentNullException
        }

        public static Prediction<LblT> ClassifyGroup<LblT, ExT>(IEnumerable<ExT> examples, IModel<LblT, ExT> model, GroupClassifyMethod method)
        {
            return ClassifyGroup<LblT, ExT>(examples, model, method, /*lblCmp=*/null); // throws InvalidOperationException, ArgumentNullException
        }

        public static Prediction<LblT> ClassifyGroup<LblT, ExT>(IEnumerable<ExT> examples, IModel<LblT, ExT> model, GroupClassifyMethod method, IEqualityComparer<LblT> lblCmp)
        {
            Dictionary<LblT, double> tmp = new Dictionary<LblT, double>(lblCmp);
            foreach (ExT example in examples)
            {
                Prediction<LblT> result = model.Predict(example); // throws InvalidOperationException, ArgumentNullException
                foreach (KeyDat<double, LblT> lblInfo in result)
                {
                    if (method == GroupClassifyMethod.Vote)
                    {
                        if (!tmp.ContainsKey(lblInfo.Dat)) 
                        { 
                            tmp.Add(lblInfo.Dat, 1); 
                        } 
                        else 
                        { 
                            tmp[lblInfo.Dat]++; 
                        }
                        break;
                    }
                    else
                    {
                        if (!tmp.ContainsKey(lblInfo.Dat))
                        {
                            tmp.Add(lblInfo.Dat, lblInfo.Key);
                        }
                        else
                        {
                            switch (method)
                            {
                                case GroupClassifyMethod.Max:
                                    tmp[lblInfo.Dat] = Math.Max(lblInfo.Key, tmp[lblInfo.Dat]);
                                    break;
                                case GroupClassifyMethod.Sum:
                                    tmp[lblInfo.Dat] += lblInfo.Key;
                                    break;
                            }
                        }
                    }
                }
            }
            Prediction<LblT> aggrResult = new Prediction<LblT>();
            foreach (KeyValuePair<LblT, double> item in tmp)
            {
                aggrResult.Items.Add(new KeyDat<double, LblT>(item.Value, item.Key));
            }
            aggrResult.Items.Sort(new DescSort<KeyDat<double, LblT>>());
            return aggrResult;
        }

        // *** Computation of centroids ***

        public static SparseVector<double> ComputeCentroid(IEnumerable<SparseVector<double>.ReadOnly> vecList, CentroidType type)
        {
            Utils.ThrowException(vecList == null ? new ArgumentNullException("vecList") : null);
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            int vecCount = 0;
            foreach (SparseVector<double>.ReadOnly vec in vecList)
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
                vecCount++;
            }
            Utils.ThrowException(vecCount == 0 ? new ArgumentValueException("vecList") : null);
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
                        centroid.InnerDat.Add(item.Value / (double)vecCount);
                    }
                    break;
                case CentroidType.NrmL2:
                    double vecLen = 0;
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        vecLen += item.Value * item.Value;
                    }
                    Utils.ThrowException(vecLen == 0 ? new InvalidOperationException() : null);
                    vecLen = Math.Sqrt(vecLen);
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value / vecLen);
                    }
                    break;
            }
            centroid.Sort();
            return centroid;
        }

        public static SparseVector<double> ComputeCentroid(IEnumerable<int> vecIdxList, IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, CentroidType type)
        {
            Utils.ThrowException(vecIdxList == null ? new ArgumentNullException("vecIdxList") : null);
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            int vecCount = 0;
            foreach (int vecIdx in vecIdxList)
            {
                Utils.ThrowException((vecIdx < 0 || vecIdx >= dataset.Count) ? new ArgumentValueException("vecIdxList") : null);
                SparseVector<double>.ReadOnly vec = dataset[vecIdx];
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
                vecCount++;
            }
            Utils.ThrowException(vecCount == 0 ? new ArgumentValueException("vecIdxList") : null);
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
                        centroid.InnerDat.Add(item.Value / (double)vecCount);
                    }
                    break;
                case CentroidType.NrmL2:
                    double vecLen = 0;
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        vecLen += item.Value * item.Value;
                    }
                    Utils.ThrowException(vecLen == 0 ? new InvalidOperationException() : null);
                    vecLen = Math.Sqrt(vecLen);
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value / vecLen);
                    }
                    break;
            }
            centroid.Sort();
            return centroid;
        }

        public static SparseVector<double> ComputeCentroidWgt(IEnumerable<Pair<double, SparseVector<double>.ReadOnly>> wgtVecList, CentroidType type)
        {
            Utils.ThrowException(wgtVecList == null ? new ArgumentNullException("wgtVecList") : null);
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            double wgtSum = 0;
            foreach (Pair<double, SparseVector<double>.ReadOnly> wgtVec in wgtVecList)
            {
                foreach (IdxDat<double> item in wgtVec.Second)
                {
                    if (tmp.ContainsKey(item.Idx))
                    {
                        tmp[item.Idx] += wgtVec.First * item.Dat;
                    }
                    else
                    {
                        tmp.Add(item.Idx, wgtVec.First * item.Dat);
                    }
                }
                wgtSum += wgtVec.First;
            }
            Utils.ThrowException(wgtSum == 0 ? new ArgumentValueException("wgtVecList") : null);
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
                        centroid.InnerDat.Add(item.Value / wgtSum);
                    }
                    break;
                case CentroidType.NrmL2:
                    double vecLen = 0;
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        vecLen += item.Value * item.Value;
                    }
                    Utils.ThrowException(vecLen == 0 ? new InvalidOperationException() : null);
                    vecLen = Math.Sqrt(vecLen);
                    foreach (KeyValuePair<int, double> item in tmp)
                    {
                        centroid.InnerIdx.Add(item.Key);
                        centroid.InnerDat.Add(item.Value / vecLen);
                    }
                    break;
            }
            centroid.Sort();
            return centroid;
        }

        // *** IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> template specialization ***

        private static void GetDotProductSimilarity(SparseVector<double>.ReadOnly vec, double[] simVec, SparseMatrix<double>.ReadOnly trMtx, int startIdx)
        {
            foreach (IdxDat<double> item in vec)
            {
                SparseVector<double>.ReadOnly col = trMtx[item.Idx];
                if (col != null)
                {
                    int startIdxDirect = col.GetDirectIdx(startIdx);
                    if (startIdxDirect < 0) { startIdxDirect = ~startIdxDirect; }
                    for (int i = startIdxDirect; i < col.Count; i++)
                    {
                        IdxDat<double> trMtxItem = col.GetDirect(i);
                        simVec[trMtxItem.Idx] += item.Dat * trMtxItem.Dat;
                    }
                }
            }
        }

        public static SparseMatrix<double> GetTransposedMatrix(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            SparseMatrix<double> trMtx = new SparseMatrix<double>();
            int rowIdx = 0;
            foreach (SparseVector<double>.ReadOnly item in dataset)
            {
                foreach (IdxDat<double> vecItem in item)
                {
                    if (!trMtx.ContainsRowAt(vecItem.Idx))
                    {
                        trMtx[vecItem.Idx] = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(rowIdx, vecItem.Dat) });
                    }
                    else
                    {
                        trMtx[vecItem.Idx].InnerIdx.Add(rowIdx);
                        trMtx[vecItem.Idx].InnerDat.Add(vecItem.Dat);
                    }
                }
                rowIdx++;
            }
            return trMtx;
        }

        public static double[] GetDotProductSimilarity(SparseMatrix<double>.ReadOnly trMtx, int datasetCount, SparseVector<double>.ReadOnly vec)
        {
            // TODO: exceptions on dataset count
            Utils.ThrowException(trMtx == null ? new ArgumentNullException("trMtx") : null);
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double[] simVec = new double[datasetCount];
            GetDotProductSimilarity(vec, simVec, trMtx, /*startIdx=*/0);
            return simVec;
        }

        public static SparseVector<double> GetDotProductSimilarity(SparseMatrix<double>.ReadOnly trMtx, int datasetCount, SparseVector<double>.ReadOnly vec, double thresh)
        {
            // TODO: exceptions on dataset count
            Utils.ThrowException(thresh < 0 ? new ArgumentOutOfRangeException("thresh") : null);
            double[] simVec = GetDotProductSimilarity(trMtx, datasetCount, vec); // throws ArgumentNullException
            SparseVector<double> sparseVec = new SparseVector<double>();
            for (int i = 0; i < simVec.Length; i++)
            {
                if (simVec[i] > thresh)
                {
                    sparseVec.InnerIdx.Add(i);
                    sparseVec.InnerDat.Add(simVec[i]);
                }
            }
            return sparseVec;
        }

        public static double[] GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, SparseVector<double>.ReadOnly vec)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            SparseMatrix<double> trMtx = GetTransposedMatrix(dataset);
            double[] simVec = new double[dataset.Count];
            GetDotProductSimilarity(vec, simVec, trMtx, /*startIdx=*/0);
            return simVec;
        }

        public static SparseVector<double> GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, SparseVector<double>.ReadOnly vec, double thresh)
        {
            Utils.ThrowException(thresh < 0 ? new ArgumentOutOfRangeException("thresh") : null);
            double[] simVec = GetDotProductSimilarity(dataset, vec); // throws ArgumentNullException
            SparseVector<double> sparseVec = new SparseVector<double>();
            for (int i = 0; i < simVec.Length; i++)
            {
                if (simVec[i] > thresh)
                {
                    sparseVec.InnerIdx.Add(i);
                    sparseVec.InnerDat.Add(simVec[i]);
                }
            }
            return sparseVec;
        }

        public static SparseMatrix<double> GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, double thresh, bool fullMatrix) // if fullMatrix is false, upper (right) triangular sparse matrix of dot products is computed
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(thresh < 0 ? new ArgumentOutOfRangeException("thresh") : null);
            SparseMatrix<double> trMtx = GetTransposedMatrix(dataset);
            double[] simVec = new double[dataset.Count];
            SparseMatrix<double> simMtx = new SparseMatrix<double>();
            int rowIdx = 0;
            foreach (SparseVector<double>.ReadOnly item in dataset)
            {
                GetDotProductSimilarity(item, simVec, trMtx, /*startIdx=*/fullMatrix ? 0 : rowIdx);
                for (int idx = 0; idx < simVec.Length; idx++)
                {
                    double sim = simVec[idx];
                    if (sim > thresh)
                    {
                        if (!simMtx.ContainsRowAt(rowIdx))
                        {
                            simMtx[rowIdx] = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(idx, sim) });
                        }
                        else
                        {
                            simMtx[rowIdx].InnerIdx.Add(idx);
                            simMtx[rowIdx].InnerDat.Add(sim);
                        }
                    }
                    simVec[idx] = 0;
                }
                rowIdx++;
            }
            return simMtx;
        }

        public static SparseMatrix<double> GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, double thresh)
        {
            return GetDotProductSimilarity(dataset, thresh, /*fullMatrix=*/false); // throws ArgumentOutOfRangeException, ArgumentNullException
        }

        public static SparseMatrix<double> GetDotProductSimilarity(/*this*/ IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            return GetDotProductSimilarity(dataset, /*thresh=*/0, /*fullMatrix=*/false); // throws ArgumentNullException
        }
    }
}
