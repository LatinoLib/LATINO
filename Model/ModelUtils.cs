/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ModelUtils.cs
 *  Desc:    Fundamental LATINO Model utilities
 *  Created: Aug-2008
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

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
            // general conversion routine (through SparseVector<double>)
            SparseVector<double> tmp;
            if (inVec.GetType() == typeof(SparseVector<double>))
            {
                tmp = (SparseVector<double>)inVec;
            }
            else if (inVec.GetType() == typeof(SparseVector<double>.ReadOnly))
            {
                tmp = ((SparseVector<double>.ReadOnly)inVec).GetWritableCopy();
            }
            else if (inVec.GetType() == typeof(BinaryVector)) 
            {
                tmp = new SparseVector<double>(((BinaryVector)inVec).Count);
                foreach (int item in (BinaryVector)inVec)
                {
                    tmp.InnerIdx.Add(item);
                    tmp.InnerDat.Add(1);
                }
            }
            else if (inVec.GetType() == typeof(BinaryVector.ReadOnly))
            {
                tmp = new SparseVector<double>(((BinaryVector.ReadOnly)inVec).Count);
                foreach (int item in (BinaryVector.ReadOnly)inVec)
                {
                    tmp.InnerIdx.Add(item);
                    tmp.InnerDat.Add(1);
                }
            }       
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
            else if (outVecType == typeof(BinaryVector))
            {
                outVec = new BinaryVector();
                ((BinaryVector)outVec).Inner.AddRange(tmp.InnerIdx);
            }
            else if (outVecType == typeof(BinaryVector.ReadOnly))
            {
                BinaryVector vec = new BinaryVector();
                vec.Inner.AddRange(tmp.InnerIdx);
                outVec = new BinaryVector.ReadOnly(vec);
            }
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
            Utils.ThrowException(examples == null ? new ArgumentNullException("examples") : null);
            Utils.ThrowException(model == null ? new ArgumentNullException("model") : null);
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
                aggrResult.Inner.Add(new KeyDat<double, LblT>(item.Value, item.Key));
            }
            aggrResult.Inner.Sort(DescSort<KeyDat<double, LblT>>.Instance);
            return aggrResult;
        }

        // *** Computation of centroids ***

        public static SparseVector<double> ComputeCentroid(IEnumerable<SparseVector<double>> vecList, CentroidType type)
        {
            Utils.ThrowException(vecList == null ? new ArgumentNullException("vecList") : null);
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            int vecCount = 0;
            foreach (SparseVector<double> vec in vecList)
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
            //Utils.ThrowException(vecCount == 0 ? new ArgumentValueException("vecList") : null);
            if (vecCount == 0) { return new SparseVector<double>(); }
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
                    //Utils.ThrowException(vecLen == 0 ? new InvalidOperationException() : null);
                    vecLen = Math.Sqrt(vecLen);
                    if (vecLen > 0)
                    {
                        foreach (KeyValuePair<int, double> item in tmp)
                        {
                            centroid.InnerIdx.Add(item.Key);
                            centroid.InnerDat.Add(item.Value / vecLen);
                        }
                    }
                    break;
            }
            centroid.Sort();
            return centroid;
        }

        public static SparseVector<double> ComputeCentroid(IEnumerable<int> vecIdxList, IUnlabeledExampleCollection<SparseVector<double>> dataset, CentroidType type)
        {
            Utils.ThrowException(vecIdxList == null ? new ArgumentNullException("vecIdxList") : null);
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            int vecCount = 0;
            foreach (int vecIdx in vecIdxList)
            {
                Utils.ThrowException((vecIdx < 0 || vecIdx >= dataset.Count) ? new ArgumentValueException("vecIdxList") : null);
                SparseVector<double> vec = dataset[vecIdx];
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
            //Utils.ThrowException(vecCount == 0 ? new ArgumentValueException("vecIdxList") : null);
            if (vecCount == 0) { return new SparseVector<double>(); }
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
                    //Utils.ThrowException(vecLen == 0 ? new InvalidOperationException() : null);
                    vecLen = Math.Sqrt(vecLen);
                    if (vecLen > 0)
                    {
                        foreach (KeyValuePair<int, double> item in tmp)
                        {
                            centroid.InnerIdx.Add(item.Key);
                            centroid.InnerDat.Add(item.Value / vecLen);
                        }
                    }
                    break;
            }
            centroid.Sort();
            return centroid;
        }

        public static SparseVector<double> ComputeCentroidWgt(IEnumerable<Pair<double, SparseVector<double>>> wgtVecList, CentroidType type)
        {
            Utils.ThrowException(wgtVecList == null ? new ArgumentNullException("wgtVecList") : null);
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            double wgtSum = 0;
            foreach (Pair<double, SparseVector<double>> wgtVec in wgtVecList)
            {
                Utils.ThrowException(wgtVec.First < 0 || wgtVec.Second == null ? new ArgumentValueException("wgtVecList") : null);
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
            if (wgtSum == 0) { return new SparseVector<double>(); }
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
                    //Utils.ThrowException(vecLen == 0 ? new InvalidOperationException() : null);
                    vecLen = Math.Sqrt(vecLen);
                    if (vecLen > 0)
                    {
                        foreach (KeyValuePair<int, double> item in tmp)
                        {
                            centroid.InnerIdx.Add(item.Key);
                            centroid.InnerDat.Add(item.Value / vecLen);
                        }
                    }
                    break;
            }
            centroid.Sort();
            return centroid;
        }

        // *** Dataset utilities ***

        public static SparseMatrix<double> GetTransposedMatrix(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            //if (dataset.Count == 0) { return new SparseMatrix<double>(); }
            SparseMatrix<double> trMtx = new SparseMatrix<double>();
            int rowIdx = 0;
            foreach (SparseVector<double> item in dataset)
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

        public static UnlabeledDataset<ExT> ConvertToUnlabeledDataset<LblT, ExT>(ILabeledExampleCollection<LblT, ExT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            UnlabeledDataset<ExT> unlabeledDataset = new UnlabeledDataset<ExT>();
            foreach (LabeledExample<LblT, ExT> labeledExample in dataset)
            {
                unlabeledDataset.Add(labeledExample.Example);
            }
            return unlabeledDataset;
        }

        public static LabeledDataset<int, SparseVector<double>> LoadDataset(string fileName)
        {
            Utils.ThrowException(fileName == null ? new ArgumentNullException("fileName") : null);
            Utils.ThrowException(!Utils.VerifyFileNameOpen(fileName) ? new ArgumentValueException("fileName") : null);
            StreamReader reader = new StreamReader(fileName);
            LabeledDataset<int, SparseVector<double>> dataset = LoadDataset(reader);
            reader.Close();
            return dataset;
        }

        public static LabeledDataset<int, SparseVector<double>> LoadDataset(StreamReader reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            string line;
            LabeledDataset<int, SparseVector<double>> dataset = new LabeledDataset<int, SparseVector<double>>();
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.StartsWith("#"))
                {
                    Match labelMatch = new Regex(@"^(?<label>[+-]?\d+(\.\d+)?)(\s|$)").Match(line);
                    Utils.ThrowException(!labelMatch.Success ? new IOException() : null);
                    int label = Convert.ToInt32(labelMatch.Result("${label}"));
                    Match match = new Regex(@"(?<feature>\d+):(?<weight>[-]?[\d\.]+)").Match(line);
                    SparseVector<double> vec = new SparseVector<double>();
                    while (match.Success)
                    {
                        int feature = Convert.ToInt32(match.Result("${feature}"));
                        double weight = Convert.ToDouble(match.Result("${weight}"), CultureInfo.InvariantCulture);
                        match = match.NextMatch();
                        vec[feature] = weight;
                    }
                    dataset.Add(new LabeledExample<int, SparseVector<double>>(label, vec));
                }
            }
            return dataset;
        }

        // *** Sparse vector utilities ***

        public static double GetVecLenL2(SparseVector<double> vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = 0;
            ArrayList<double> datInner = vec.InnerDat;
            foreach (double val in datInner)
            {
                len += val * val;
            }
            return Math.Sqrt(len);
        }

        public static double GetVecLenL2(SparseVector<double>.ReadOnly vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            return GetVecLenL2(vec.Inner);
        }

        public static void NrmVecL2(SparseVector<double> vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = GetVecLenL2(vec);
            Utils.ThrowException(len == 0 ? new InvalidOperationException() : null);
            ArrayList<double> datInner = vec.InnerDat;
            for (int i = 0; i < vec.Count; i++)
            {
                vec.SetDirect(i, datInner[i] / len);
            }
        }

        public static bool TryNrmVecL2(SparseVector<double> vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = GetVecLenL2(vec);
            if (len == 0) { return false; }
            ArrayList<double> datInner = vec.InnerDat;
            for (int i = 0; i < vec.Count; i++)
            {
                vec.SetDirect(i, datInner[i] / len);
            }
            return true;
        }

        public static void CutLowWeights(ref SparseVector<double> vec, double cutLowWgtPerc)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            Utils.ThrowException(cutLowWgtPerc < 0 || cutLowWgtPerc >= 1 ? new ArgumentValueException("cutLowWgtPerc") : null);
            if (cutLowWgtPerc > 0)
            {
                double wgtSum = 0;
                ArrayList<KeyDat<double, int>> tmp = new ArrayList<KeyDat<double, int>>(vec.Count);
                foreach (IdxDat<double> item in vec)
                {
                    wgtSum += item.Dat;
                    tmp.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                }
                tmp.Sort();
                double cutSum = cutLowWgtPerc * wgtSum;
                double cutWgt = -1;
                foreach (KeyDat<double, int> item in tmp)
                {
                    cutSum -= item.Key;
                    if (cutSum <= 0)
                    {
                        cutWgt = item.Key;
                        break;
                    }
                }
                SparseVector<double> newVec = new SparseVector<double>();
                if (cutWgt != -1)
                {
                    foreach (IdxDat<double> item in vec)
                    {
                        if (item.Dat >= cutWgt)
                        {
                            newVec.InnerIdx.Add(item.Idx);
                            newVec.InnerDat.Add(item.Dat);
                        }
                    }
                }
                vec = newVec;
            }
        }

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

        public static double[] GetDotProductSimilarity(SparseMatrix<double>.ReadOnly trMtx, int datasetCount, SparseVector<double>.ReadOnly vec)
        {            
            Utils.ThrowException(trMtx == null ? new ArgumentNullException("trMtx") : null);
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            Utils.ThrowException(datasetCount < 0 ? new ArgumentOutOfRangeException("datasetCount") : null);
            double[] simVec = new double[datasetCount];
            GetDotProductSimilarity(vec, simVec, trMtx, /*startIdx=*/0);
            return simVec;
        }

        public static SparseVector<double> GetDotProductSimilarity(SparseMatrix<double>.ReadOnly trMtx, int datasetCount, SparseVector<double>.ReadOnly vec, double thresh)
        {
            Utils.ThrowException(thresh < 0 ? new ArgumentOutOfRangeException("thresh") : null);
            double[] simVec = GetDotProductSimilarity(trMtx, datasetCount, vec); // throws ArgumentNullException, ArgumentOutOfRangeException
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

        public static double[] GetDotProductSimilarity(IUnlabeledExampleCollection<SparseVector<double>> dataset, SparseVector<double>.ReadOnly vec)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            SparseMatrix<double> trMtx = GetTransposedMatrix(dataset);
            double[] simVec = new double[dataset.Count];
            GetDotProductSimilarity(vec, simVec, trMtx, /*startIdx=*/0);
            return simVec;
        }

        public static SparseVector<double> GetDotProductSimilarity(IUnlabeledExampleCollection<SparseVector<double>> dataset, SparseVector<double>.ReadOnly vec, double thresh)
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

        public static SparseMatrix<double> GetDotProductSimilarity(IUnlabeledExampleCollection<SparseVector<double>> dataset, double thresh, bool fullMatrix) 
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(thresh < 0 ? new ArgumentOutOfRangeException("thresh") : null);
            SparseMatrix<double> trMtx = GetTransposedMatrix(dataset);
            double[] simVec = new double[dataset.Count];
            SparseMatrix<double> simMtx = new SparseMatrix<double>();
            int rowIdx = 0;
            foreach (SparseVector<double> item in dataset)
            {
                GetDotProductSimilarity(item, simVec, trMtx, /*startIdx=*/fullMatrix ? 0 : rowIdx); // if fullMatrix is false, upper (right) triangular sparse matrix of dot products is computed
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

        public static SparseMatrix<double> GetDotProductSimilarity(IUnlabeledExampleCollection<SparseVector<double>> dataset, double thresh)
        {
            return GetDotProductSimilarity(dataset, thresh, /*fullMatrix=*/false); // throws ArgumentOutOfRangeException, ArgumentNullException
        }

        public static SparseMatrix<double> GetDotProductSimilarity(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            return GetDotProductSimilarity(dataset, /*thresh=*/0, /*fullMatrix=*/false); // throws ArgumentNullException
        }
    }
}
