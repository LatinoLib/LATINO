using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Latino.Model.Eval
{
    /* .-----------------------------------------------------------------------
       |
       |  Enum PerfMetric
       |
       '-----------------------------------------------------------------------
    */
    public enum PerfMetric
    {
        Accuracy,
        Error,
         // special
        Kappa,
        KAlphaNominal,

        // micro-averaged
        MicroPrecision,
        MicroRecall,
        MicroF1,
        
        // macro-averaged
        MacroPrecision,
        MacroRecall,
        MacroF1,

        AccStdErrorConf90,
        AccStdErrorConf95,
        AccStdErrorConf99
    }

    public enum ClassPerfMetric
    {
        Precision,
        Recall,
        F1,

        ActualCount, 
        ActualRatio,
        PredictedCount, 
        PredictedRatio
    }


    public enum OrdinalPerfMetric
    {
        MeanAbsoluteError,
        MeanSquaredError,
        ErrorTolerance1,
        MeanSquaredErrorNormalized1,
        MeanAbsoluteErrorNormalized1,
        WeightedKappaLinear,
        WeightedKappaSquared,
        KAlphaLinear,
        KAlphaInterval,
        F1AvgExtremeClasses,
        AccuracyTolerance1
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class PerfData<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class PerfData<LblT>
    {
        /* .-----------------------------------------------------------------------
           |
           |  Class FoldData
           |
           '-----------------------------------------------------------------------
        */
        private class FoldData : ArrayList<PerfMatrix<LblT>>
        {
            private readonly object mLock = new object();

            public void Resize(int n)
            {
                lock (mLock)
                {
                    while (Count < n) { Add(null); }                    
                }
            }

            public void Put(int foldNum, PerfMatrix<LblT> mtx)
            {
                Resize(foldNum);
                this[foldNum - 1] = mtx;
            }
        }

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, FoldData>> mData
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, FoldData>>();
        private readonly IEqualityComparer<LblT> mLblEqCmp;

        public PerfData()
        {
        }

        public PerfData(IEqualityComparer<LblT> lblEqCmp)
        {
            mLblEqCmp = lblEqCmp;
        }

        public void Reset()
        {
            mData.Clear();
        }

        public Tuple<string, string>[] GetDataKeys()
        {
            return mData.SelectMany(kv => kv.Value.Keys.Select(k => new Tuple<string, string>(kv.Key, k))).ToArray();
        } 

        public PerfMatrix<LblT> GetPerfMatrix(string expName, string algoName, int foldNum)
        {
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    foldData.Resize(foldNum);
                    return foldData[foldNum - 1] ?? (foldData[foldNum - 1] = new PerfMatrix<LblT>(mLblEqCmp));
                }
                if (!algoData.TryAdd(algoName, foldData = new FoldData())) { foldData = algoData[algoName]; }
                var mtx = new PerfMatrix<LblT>(mLblEqCmp);
                foldData.Put(foldNum, mtx);
                return mtx;
            }
            else
            {
                if (!mData.TryAdd(expName, algoData = new ConcurrentDictionary<string, FoldData>())) { algoData = mData[expName]; }
                var foldData = new FoldData();
                if (!algoData.TryAdd(algoName, foldData)) { foldData = algoData[algoName]; }
                var mtx = new PerfMatrix<LblT>(mLblEqCmp);
                foldData.Put(foldNum, mtx);
                return mtx;
            }
        }

        public void SetPerfMatrix(string expName, string algoName, int foldNum, PerfMatrix<LblT> matrix)
        {
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    foldData.Resize(foldNum);
                    foldData[foldNum - 1] = matrix;
                }
                else
                {
                    if (!algoData.TryAdd(algoName, foldData = new FoldData())) { foldData = algoData[algoName]; }
                    foldData.Put(foldNum, matrix);
                }
            }
            else
            {
                FoldData foldData = new FoldData();
                if (!mData.TryAdd(expName, algoData = new ConcurrentDictionary<string, FoldData>())) { algoData = mData[expName]; }
                algoData.TryAdd(algoName, foldData);
                foldData.Put(foldNum, matrix);
            }
        }

        public PerfMatrix<LblT> GetSumPerfMatrix(string expName, string algoName)
        {
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);

            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    PerfMatrix<LblT> sumMtx = new PerfMatrix<LblT>(mLblEqCmp);
                    foreach (PerfMatrix<LblT> foldMtx in foldData.Where(m => m != null))
                    {
                        Set<LblT> labels = foldMtx.GetLabels();
                        foreach (LblT actual in labels)
                        {
                            foreach (LblT predicted in labels)
                            {
                                sumMtx.AddCount(actual, predicted, foldMtx.Get(actual, predicted));
                            }
                        }
                    }
                    return sumMtx;
                }
            }
            return null;
        }

        public int GetFoldCount(string expName, string algoName)
        {
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);

            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    return foldData.Count(m => m != null);
                }
            }
            return 0;
        }

        public Set<LblT> GetLabels(string expName, string algoName)
        {
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);

            var labels = new Set<LblT>();
            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    foreach (PerfMatrix<LblT> foldMtx in foldData.Where(m => m != null))
                    {
                        labels.AddRange(foldMtx.GetLabels());
                    }
                }
            }
            return labels;
        }

        public double GetVal(int foldNum, string expName, string algoName, PerfMetric metric)
        {
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    if (foldNum <= foldData.Count && foldData[foldNum - 1] != null)
                    {
                        return foldData[foldNum - 1].GetScore(metric);
                    }
                }
            }
            return Double.NaN;
        }

        public double GetVal(int foldNum, string expName, string algoName, ClassPerfMetric metric, LblT lbl)
        {
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    if (foldNum <= foldData.Count && foldData[foldNum - 1] != null)
                    {
                        return foldData[foldNum - 1].GetScore(metric, lbl);
                    }
                }
            }
            return Double.NaN;
        }

        public double GetVal(int foldNum, string expName, string algoName, OrdinalPerfMetric metric, IEnumerable<LblT> orderedLabels)
        {
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    if (foldNum <= foldData.Count && foldData[foldNum - 1] != null)
                    {
                        return foldData[foldNum - 1].GetScore(metric, orderedLabels);
                    }
                }
            }
            return Double.NaN;
        }

        public double GetAvg(string expName, string algoName, PerfMetric metric, out double stdev)
        {
            return GetAvg(expName, algoName, mtx => mtx.GetScore(metric), out stdev);
        }

        public double GetAvg(string expName, string algoName, ClassPerfMetric metric, LblT lbl, out double stdev)
        {
            return GetAvg(expName, algoName, mtx => mtx.GetScore(metric, lbl), out stdev);
        }

        public double GetAvg(string expName, string algoName, OrdinalPerfMetric metric, IEnumerable<LblT> orderedLabels, out double stdev)
        {
            return GetAvg(expName, algoName, mtx => mtx.GetScore(metric, orderedLabels), out stdev);
        }

        public double GetAvgStdErr(string expName, string algoName, PerfMetric metric, out double stderr, double confidenceLevel = 0.95)
        {
            double zval = StatsUtils.GetZScore(confidenceLevel);
            double stddev;
            double avg = GetAvg(expName, algoName, metric, out stddev);
            stderr = zval * stddev / Math.Sqrt(GetFoldCount(expName, algoName));
            return avg;
        }

        public double GetAvgStdErr(string expName, string algoName, ClassPerfMetric metric, LblT lbl, out double stderr, double confidenceLevel = 0.95)
        {
            double zval = StatsUtils.GetZScore(confidenceLevel);
            double stddev;
            double avg = GetAvg(expName, algoName, metric, lbl, out stddev);
            stderr = zval * stddev / Math.Sqrt(GetFoldCount(expName, algoName));
            return avg;
        }

        public double GetAvgStdErr(string expName, string algoName, OrdinalPerfMetric metric, IEnumerable<LblT> orderedLabels, out double stderr, double confidenceLevel = 0.95)
        {
            double zval = StatsUtils.GetZScore(confidenceLevel);
            double stddev;
            double avg = GetAvg(expName, algoName, metric, orderedLabels, out stddev);
            stderr = zval * stddev / Math.Sqrt(GetFoldCount(expName, algoName));
            return avg;
        }

        private double GetAvg(string expName, string algoName, Func<PerfMatrix<LblT>, double> scoreFunc, out double stdev)
        {
            Preconditions.CheckNotNull(scoreFunc);
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            stdev = Double.NaN;
            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    double[] results = foldData
                        .Where(m => m != null)
                        .Select(scoreFunc)
                        .Where(r => !Double.IsNaN(r) && !Double.IsInfinity(r)).ToArray();
                    if (!results.Any()) { return Double.NaN; }

                    double avg = results.Average();
                    stdev = Math.Sqrt(results.Select(r => (r - avg) * (r - avg)).Sum() / results.Length);
                    return avg;
                }
            }
            return Double.NaN;
        }

        public double GetAvg(string expName, string algoName, PerfMetric metric)
        {
            double stdev;
            return GetAvg(expName, algoName, metric, out stdev); 
        }

        public double GetAvg(string expName, string algoName, ClassPerfMetric metric, LblT label)
        {
            double stdev;
            return GetAvg(expName, algoName, metric, label, out stdev); 
        }

        public double GetAvg(string expName, string algoName, OrdinalPerfMetric metric, IEnumerable<LblT> orderedLabels)
        {
            double stdev;
            return GetAvg(expName, algoName, metric, orderedLabels, out stdev); 
        }

        public string ToString(int foldNum, string expName, PerfMetric metric, LblT lbl, string fmt) // set expName to null to get stats for all experiments
        {
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            Utils.ThrowException(fmt == null ? new ArgumentNullException("fmt") : null);
            var expList = new ArrayList<string>(mData.Keys);
            expList.Sort();
            var tmp = new Set<string>();
            foreach (ConcurrentDictionary<string, FoldData> item in mData.Values) { tmp.AddRange(item.Keys); }
            var algoList = new ArrayList<string>(tmp);
            algoList.Sort();
            var sb = new StringBuilder();
            // header
            if (expName == null)
            {
                foreach (string exp in expList) { sb.Append("\t" + exp); }
                sb.AppendLine();
            }
            // rows
            foreach (string algoName in algoList)
            {
                sb.Append(algoName);
                if (expName != null)
                {
                    double val = GetVal(foldNum, expName, algoName, metric);
                    sb.Append("\t" + val.ToString(fmt));
                }
                else
                {
                    foreach (string exp in expList)
                    {
                        sb.Append("\t" + GetVal(foldNum, exp, algoName, metric).ToString(fmt));
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        public string ToString(int foldNum, string expName, PerfMetric metric, LblT lbl)
        {
            return ToString(foldNum, expName, metric, lbl, "0.0000"); // throws ArgumentOutOfRangeException
        }

        public string ToString(int foldNum, string expName, PerfMetric metric)
        {
            return ToString(foldNum, expName, metric, default(LblT)); // throws ArgumentOutOfRangeException
        }

        public string ToString(string expName, PerfMetric metric, LblT lbl, string fmt) // set expName to null to get stats for all experiments
        {
            Utils.ThrowException(fmt == null ? new ArgumentNullException("fmt") : null);
            ArrayList<string> expList = new ArrayList<string>(mData.Keys);
            expList.Sort();
            Set<string> tmp = new Set<string>();
            foreach (ConcurrentDictionary<string, FoldData> item in mData.Values) { tmp.AddRange(item.Keys); }
            ArrayList<string> algoList = new ArrayList<string>(tmp);
            algoList.Sort();
            StringBuilder sb = new StringBuilder();
            // header
            if (expName == null)
            {
                foreach (string exp in expList) { sb.Append("\t" + exp); }
                sb.AppendLine();
            }
            // rows
            foreach (string algoName in algoList)
            {
                sb.Append(algoName);
                if (expName == null)
                {
                    foreach (string exp in expList)
                    {
                        double stdev;
                        double val = GetAvg(exp, algoName, metric, out stdev); // throws InvalidOperationException
                        sb.Append("\t" + val.ToString(fmt));
                    }
                }
                else
                {
                    double stdev;
                    sb.Append("\t" + GetAvg(expName, algoName, metric, out stdev).ToString(fmt)); // throws InvalidOperationException
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        public string ToString(string expName, PerfMetric metric, LblT lbl)
        {
            return ToString(expName, metric, lbl, "0.0000"); // throws InvalidOperationException
        }

        public string ToString(string expName, PerfMetric metric)
        {
            return ToString(expName, metric, default(LblT)); // throws InvalidOperationException
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class PerfMatrix<LblT>
       |
       '-----------------------------------------------------------------------
    */

    public class PerfMatrix<LblT>
    {
        private readonly ConcurrentDictionary<LblT, ConcurrentDictionary<LblT, double>> mMtx
            = new ConcurrentDictionary<LblT, ConcurrentDictionary<LblT, double>>();

        private readonly Set<LblT> mLabels
            = new Set<LblT>(); // TODO: set lbl equality comparer

        private IEqualityComparer<LblT> mLblEqCmp;

        public PerfMatrix(IEqualityComparer<LblT> lblEqCmp = null)
        {
            mLblEqCmp = lblEqCmp;
        }

        public void AddLabels(IEnumerable<LblT> labels)
        {
            mLabels.AddRange(labels);
        }

        public bool RemoveLabel(LblT label)
        {
            return mLabels.Remove(label);
        }

        public void AddCount(LblT actual, LblT predicted, double count)
        {
            mLabels.AddRange(new[] { actual, predicted });
            ConcurrentDictionary<LblT, double> row;
            if (mMtx.TryGetValue(actual, out row))
            {
                double oldCount;
                if (row.TryGetValue(predicted, out oldCount))
                {
                    row[predicted] = oldCount + count;
                }
                else
                {
                    if (!row.TryAdd(predicted, count))
                    {
                        row[predicted] += count;
                    }
                }
            }
            else
            {
                if (!mMtx.TryAdd(actual, row = new ConcurrentDictionary<LblT, double>())) 
                {
                    row = mMtx[actual];
                }
                if (!row.TryAdd(predicted, count))
                {
                    row[predicted] += count;
                }
            }
        }

        public void AddCount(LblT actual, LblT predicted)
        {
            AddCount(actual, predicted, /*count=*/1);
        }

        public Set<LblT> GetLabels()
        {
            return new Set<LblT>(mLabels);
        }

        public double Get(LblT actual, LblT predicted)
        {
            ConcurrentDictionary<LblT, double> row;
            if (mMtx.TryGetValue(actual, out row))
            {
                double c;
                if (row.TryGetValue(predicted, out c))
                {
                    return c;
                }
            }
            return 0;
        }

        public double GetActual(LblT lbl)
        {
            return SumRow(lbl);
        }

        public double GetPredicted(LblT lbl)
        {
            return SumCol(lbl);
        }

        public void Reset()
        {
            mMtx.Clear();
            mLabels.Clear();
        }

        private double SumRow(LblT lbl)
        {
            ConcurrentDictionary<LblT, double> row;
            if (mMtx.TryGetValue(lbl, out row))
            {
                double sum = 0;
                foreach (double c in row.Values)
                {
                    sum += c;
                }
                return sum;
            }
            return 0;
        }

        private double SumCol(LblT lbl)
        {
            double sum = 0;
            foreach (ConcurrentDictionary<LblT, double> row in mMtx.Values)
            {
                double c;
                if (row.TryGetValue(lbl, out c))
                {
                    sum += c;
                }
            }
            return sum;
        }

        public double GetSumAll()
        {
            double sum = 0;
            foreach (ConcurrentDictionary<LblT, double> row in mMtx.Values)
            {
                foreach (double val in row.Values)
                {
                    sum += val;
                }
            }
            return sum;
        }

        public double GetSumDiag()
        {
            double sum = 0;
            foreach (KeyValuePair<LblT, ConcurrentDictionary<LblT, double>> row in mMtx)
            {
                double c;
                if (row.Value.TryGetValue(row.Key, out c))
                {
                    sum += c;
                }
            }
            return sum;
        }

        public double GetPrecision(LblT lbl)
        {
            double all;
            double precCount = GetPrecisionCount(lbl, out all);
            return precCount / all;
        }

        public double GetPrecisionCount(LblT lbl, out double all)
        {
            all = SumCol(lbl);
            return Get(lbl, lbl);
        }

        public double GetRecall(LblT lbl)
        {
            double all;
            double recallCount = GetRecallCount(lbl, out all);
            return recallCount / all;
        }

        public double GetRecallCount(LblT lbl, out double all)
        {
            all = SumRow(lbl);
            return Get(lbl, lbl);
        }

        public double GetF(double w, LblT lbl)
        {
            double p = GetPrecision(lbl);
            double r = GetRecall(lbl);
            return (w * w + 1) * p * r / (w * w * p + r);
        }

        public double GetF1(LblT lbl)
        {
            return GetF(1, lbl);
        }

        public double GetActualSum(LblT lbl)
        {
            return SumRow(lbl);
        }

        public double GetActualRatio(LblT lbl)
        {
            return GetActualSum(lbl) / GetSumAll();
        }

        public double GetPredictedSum(LblT lbl)
        {
            return SumCol(lbl);
        }

        public double GetPredictedRatio(LblT lbl)
        {
            return GetPredictedSum(lbl) / GetSumAll();
        }


        // *** Micro-averaging (over examples) ***

        public double GetAccuracy()
        {
            return GetSumDiag() / GetSumAll();
        }

        public double GetError()
        {
            return 1 - GetAccuracy();
        }

        public double GetMicroPrecision()
        {
            double result = mLabels.Sum(lbl => GetActual(lbl) * GetPrecision(lbl));
            return result / GetSumAll();
        }

        public double GetMicroRecall()
        {
            double result = mLabels.Sum(lbl => GetActual(lbl) * GetRecall(lbl));
            return result / GetSumAll();
        }

        public double GetMicroF1()
        {
            double result = mLabels.Sum(lbl => GetActual(lbl) * GetF1(lbl));
            return result / GetSumAll();
        }


        // *** Macro-averaging (over classes) ***

        public double GetMacroPrecision()
        {
            double sum = mLabels.Sum(lbl => GetPrecision(lbl));
            return sum / mLabels.Count;
        }

        public double GetMacroRecall()
        {
            double sum = mLabels.Sum(lbl => GetRecall(lbl));
            return sum / mLabels.Count;
        }

        public double GetMacroF(double w)
        {
            double sum = mLabels.Sum(lbl => GetF(w, lbl));
            return sum / mLabels.Count;
        }

        public double GetMacroF1()
        {
            double sum = mLabels.Sum(lbl => GetF1(lbl));
            return sum / mLabels.Count;
        }


        public double GetScore(PerfMetric metric)
        {
            switch (metric)
            {             
                case PerfMetric.Accuracy:
                    return GetAccuracy();
                case PerfMetric.MicroPrecision:
                    return GetMicroPrecision();
                case PerfMetric.MicroRecall:
                    return GetMicroRecall();
                case PerfMetric.MicroF1:
                    return GetMicroF1();
                case PerfMetric.MacroPrecision:
                    return GetMacroPrecision();
                case PerfMetric.MacroRecall:
                    return GetMacroRecall();
                case PerfMetric.MacroF1:
                    return GetMacroF1();
                case PerfMetric.Error:
                    return GetError();
                case PerfMetric.Kappa:
                    return GetKappa();
                case PerfMetric.KAlphaNominal:
                    return GetKAlpha();
                case PerfMetric.AccStdErrorConf90:
                    return GetAccStdError(0.9);
                case PerfMetric.AccStdErrorConf95:
                    return GetAccStdError();
                case PerfMetric.AccStdErrorConf99:
                    return GetAccStdError(0.99);
                default:
                    throw new ArgumentValueException("metric");
            }
        }


        public double GetScore(ClassPerfMetric metric, LblT lbl)
        {
            switch (metric)
            {
                case ClassPerfMetric.Precision:
                    return GetPrecision(lbl);
                case ClassPerfMetric.Recall:
                    return GetRecall(lbl);
                case ClassPerfMetric.F1:
                    return GetF1(lbl);    
                case ClassPerfMetric.PredictedCount:
                    return GetPredictedSum(lbl);
                case ClassPerfMetric.PredictedRatio:
                    return GetPredictedRatio(lbl);
                case ClassPerfMetric.ActualCount:
                    return GetActualSum(lbl);
                case ClassPerfMetric.ActualRatio:
                    return GetActualRatio(lbl);
                default:
                    throw new ArgumentValueException("metric");
            }
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            var labels = new ArrayList<LblT>(mLabels.OrderBy(x => x.ToString()));
            var all = GetSumAll();
            int len = Math.Max(labels.DefaultIfEmpty(default(LblT)).Max(l => l.ToString().Length), 
                Math.Max(all.ToString(CultureInfo.InvariantCulture).Length, 11)) + 2;

            str.Append("".PadRight(len));
            str.Append("|".PadLeft(4));
            foreach (LblT predicted in labels)
            {
                str.Append(predicted.ToString().PadLeft(len));
            }
            str.Append("|".PadLeft(4));
            str.Append("sum actual".PadLeft(len));
            str.Append("%".PadLeft(len));
            str.AppendLine();
            str.AppendLine(new string('-', (labels.Count + 3)*len + 8));

            foreach (LblT actual in labels)
            {
                str.Append(actual.ToString().PadLeft(len));
                str.Append("|".PadLeft(4));
                foreach (LblT predicted in labels)
                {
                    str.Append(Get(actual, predicted).ToString("0.000", CultureInfo.InvariantCulture).PadLeft(len));
                }
                str.Append("|".PadLeft(4));
                str.Append(SumRow(actual).ToString("0.000", CultureInfo.InvariantCulture).PadLeft(len));
                str.Append(((float) SumRow(actual)/all).ToString("P1").PadLeft(len));
                str.AppendLine();
            }

            str.AppendLine(new string('-', (labels.Count + 3)*len + 8));
            str.Append("sum predicted".PadLeft(len));
            str.Append("|".PadLeft(4));
            foreach (LblT predicted in labels)
            {
                str.Append(SumCol(predicted).ToString("0.000", CultureInfo.InvariantCulture).PadLeft(len));
            }
            str.Append("|".PadLeft(4));
            str.AppendLine(all.ToString(CultureInfo.InvariantCulture).PadLeft(len));

            str.Append("%".PadLeft(len));
            str.Append("|".PadLeft(4));
            foreach (LblT predicted in labels)
            {
                str.Append(((float) SumCol(predicted)/all).ToString("P1").PadLeft(len));
            }
            str.AppendLine();

            return str.ToString().TrimEnd();
        }

        // General metrices
        public string ToString(IEnumerable<PerfMetric> perfMetrics) // empty for all metrics
        {
            PerfMetric[] metrics = perfMetrics as PerfMetric[] ?? Preconditions.CheckNotNull(perfMetrics.ToArray());
            if (!metrics.Any())
            {
                metrics = Enum.GetValues(typeof (PerfMetric)).Cast<PerfMetric>().ToArray();
            }

            StringBuilder str = new StringBuilder();

            str.AppendLine("\nGeneral metrices:");
            foreach (var perfMetric in metrics)
            {                
                str.Append(perfMetric.ToString().PadLeft(30));
                str.AppendLine(GetScore(perfMetric).ToString("n3").PadLeft(6));                
            }
            return str.ToString();
        }

        // Class-specific metrics
        public string ToString(IEnumerable<ClassPerfMetric> classMetrics) // metrices = empty for all metrics
        {
            ClassPerfMetric[] metrics = classMetrics as ClassPerfMetric[] ?? Preconditions.CheckNotNull(classMetrics).ToArray();
            if (!metrics.Any())
            {
                metrics = Enum.GetValues(typeof(ClassPerfMetric)).Cast<ClassPerfMetric>().ToArray();
            }

            var labels = new ArrayList<LblT>(mLabels.OrderBy(x => x.ToString()));

            var str = new StringBuilder();
            str.AppendLine("\nClass-specific metrices:");
            int len = Math.Max(labels.DefaultIfEmpty(default(LblT)).Max(l => l.ToString().Length), 13) + 2;
            str.Append("".PadRight(len));
            foreach (ClassPerfMetric metric in metrics)
            {
                str.Append(metric.ToString().PadLeft(len));
            }
            str.AppendLine();
            foreach (LblT lbl in labels)
            {
                str.Append(lbl.ToString().PadLeft(len));
                foreach (ClassPerfMetric metric in metrics)
                {
                    str.Append(GetScore(metric, lbl).ToString("n3").PadLeft(len));                    
                }
                str.AppendLine();
            }
            return str.ToString();
        }

        // Ordinal regression metrics
        public string ToString(IEnumerable<LblT> orderedLabels, IEnumerable<OrdinalPerfMetric> ordinalMetrics)
        {
            LblT[] labels = orderedLabels as LblT[] ?? Preconditions.CheckNotNull(orderedLabels).ToArray();
            OrdinalPerfMetric[] metrics = ordinalMetrics as OrdinalPerfMetric[] ?? (ordinalMetrics == null ? null : ordinalMetrics.ToArray());
            if (metrics == null || !metrics.Any())
            {
                metrics = Enum.GetValues(typeof(OrdinalPerfMetric)).Cast<OrdinalPerfMetric>().ToArray();
            }

            var str = new StringBuilder();
            str.AppendLine("\nOrdinal metrices:");
            foreach (OrdinalPerfMetric metric in metrics)
            {
                str.Append(metric.ToString().PadLeft(30));
                str.AppendLine(GetScore(metric, labels).ToString("n3").PadLeft(6));
            }
            return str.ToString();
        }


        // *** Ordinal regression measures ***

        public double GetError(IEnumerable<LblT> orderedLabels, Dictionary<LblT, Dictionary<LblT, double>> weights)
        {
            LblT[] labels = orderedLabels as LblT[] ?? Preconditions.CheckNotNull(orderedLabels).ToArray();
            Preconditions.CheckArgument(!mLabels.Except(labels).Any());
            Preconditions.CheckArgument(!labels.Except(mLabels).Any());

            double sum = 0;
            foreach (LblT actual in mLabels)
            {
                foreach (LblT predicted in mLabels)
                {
                    sum += Get(actual, predicted) * weights[actual][predicted];
                }
            }
            return sum / GetSumAll();
        }

        public double GetKAlpha()
        {
            return GetKrippendorffsAlpha(mLabels, GetErrorXWeights(mLabels));
        }

        public double GetKappa()
        {
            return GetWeightedKappa(mLabels, GetErrorXWeights(mLabels));
        }

        public double GetAccStdError(double confidenceLevel = 0.95)
        {
            double z = StatsUtils.GetZScore(confidenceLevel);
            double acc = GetAccuracy();
            return z * Math.Sqrt(acc * (1 - acc) / GetSumAll());
        }

        // implementation of http://vassarstats.net/kappaexp.html and http://vassarstats.net/kappa.html
        public double GetWeightedKappa(IEnumerable<LblT> orderedLabels, Dictionary<LblT, Dictionary<LblT, double>> weights)
        {
            LblT[] labels = orderedLabels as LblT[] ?? Preconditions.CheckNotNull(orderedLabels).ToArray();
            Preconditions.CheckArgument(!mLabels.Except(labels).Any());
            Preconditions.CheckArgument(!labels.Except(mLabels).Any());

            // the observed matrix            
            double s = GetSumAll();
            var observed = new Dictionary<LblT, Dictionary<LblT, double>>();
            foreach (LblT actual in labels)
            {
                Dictionary<LblT, double> row = labels.ToDictionary(predicted => predicted, predicted => Get(actual, predicted) / s);
                observed.Add(actual, row);
            }

            // the matrix of expected values
            var expected = new Dictionary<LblT, Dictionary<LblT, double>>();
            foreach (LblT actual in labels)
            {
                var row = labels.ToDictionary(predicted => predicted, predicted => GetActual(actual) / s * GetPredicted(predicted) / s); // a[i] * p[j] / s / s
                expected.Add(actual, row);
            }

            // the weights matrix            
            foreach (LblT actual in labels)
            {
                foreach (LblT predicted in labels)
                {
                    weights[actual][predicted] = 1 - weights[actual][predicted];
                }
            }

            double s1 = 0, s2 = 0;
            foreach (LblT actual in labels)
            {
                foreach (LblT predicted in labels)
                {
                    s1 += weights[actual][predicted] * observed[actual][predicted];
                    s2 += weights[actual][predicted] * expected[actual][predicted];
                }
            }
            double kappa = 1 - (1 - s1) / (1 - s2);
            return kappa;
        }

        // correction from wikipedia Krippendorff's Alpha
        public double GetKrippendorffsAlpha(IEnumerable<LblT> orderedLabels, Dictionary<LblT, Dictionary<LblT, double>> weights)
        {
            Preconditions.CheckNotNull(weights);
            LblT[] labels = orderedLabels as LblT[] ?? Preconditions.CheckNotNull(orderedLabels).ToArray();
            Preconditions.CheckArgument(!mLabels.Except(labels).Any());
            Preconditions.CheckArgument(!labels.Except(mLabels).Any());
            Preconditions.CheckArgument(IsSymetrical());

            double denom = 0, numer = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                for (int j = i + 1; j < labels.Length; j++)
                {
                    numer += Get(labels[i], labels[j]) * weights[labels[i]][labels[j]];
                    denom += SumRow(labels[i]) * SumCol(labels[j]) * weights[labels[i]][labels[j]];
                }
            }

            return 1 - (GetSumAll() - 1) * numer / (denom + double.Epsilon);
        }

        public double GetF1AvgExtremeClasses(IEnumerable<LblT> orderedLabels) 
        {
            LblT[] labels = orderedLabels as LblT[] ?? orderedLabels.ToArray();
            LblT label1 = labels.First();
            LblT label2 = labels.Last();

            return (GetF1(label1) + GetF1(label2)) / 2;
        }

        public double GetScore(OrdinalPerfMetric metric, IEnumerable<LblT> orderedLabels = null)
        {
            LblT[] labels = null;
            if (orderedLabels == null)
            {
                if (typeof(LblT).IsEnum) // take label order from the enum type definition
                {
                    labels = Enum.GetValues(typeof(LblT)).Cast<LblT>().ToArray();
                }
            }
            else
            {
                labels = orderedLabels as LblT[] ?? orderedLabels.ToArray();
            }

            switch (metric)
            {
                case OrdinalPerfMetric.MeanAbsoluteError:
                    return GetError(labels, GetLinearWeights(labels));
                case OrdinalPerfMetric.MeanSquaredError:
                    return GetError(labels, GetSquareWeights(labels));
                case OrdinalPerfMetric.ErrorTolerance1:
                    return GetError(labels, GetErrorXWeights(labels, 1));
                case OrdinalPerfMetric.AccuracyTolerance1:
                    return 1 - GetError(labels, GetErrorXWeights(labels, 1));
                case OrdinalPerfMetric.MeanAbsoluteErrorNormalized1:
                    return GetError(labels, GetLinearWeights(labels, true));
                case OrdinalPerfMetric.MeanSquaredErrorNormalized1:
                    return GetError(labels, GetSquareWeights(labels, true));
                case OrdinalPerfMetric.WeightedKappaLinear:
                    return GetWeightedKappa(labels, GetLinearWeights(labels, true));
                case OrdinalPerfMetric.WeightedKappaSquared:
                    return GetWeightedKappa(labels, GetSquareWeights(labels, true));
                case OrdinalPerfMetric.KAlphaLinear:
                    return GetKrippendorffsAlpha(labels, GetLinearWeights(labels, true));
                case OrdinalPerfMetric.KAlphaInterval:
                    return GetKrippendorffsAlpha(labels, GetSquareWeights(labels, true));
                case OrdinalPerfMetric.F1AvgExtremeClasses:
                    return GetF1AvgExtremeClasses(labels);
                default:
                    throw new ArgumentValueException("invalid ordered metric");
            }
        }

        // weight matrices

        public static Dictionary<LblT, Dictionary<LblT, double>> GetZeroMatrix(Set<LblT> labels)
        {
            return labels.ToDictionary(ol => ol, ol => labels.ToDictionary(ol1 => ol1, ol1 => 0.0));
        }

        public static Dictionary<LblT, Dictionary<LblT, double>> GetErrorXWeights(IEnumerable<LblT> orderedLabels, int tolerance = 0)
        {
            List<LblT> labels = Preconditions.CheckNotNull(orderedLabels).ToList();
            Dictionary<LblT, Dictionary<LblT, double>> weights = GetZeroMatrix(new Set<LblT>(labels));
            for (int i = 0; i < labels.Count; i++)
            {
                for (int j = 0; j < labels.Count; j++)
                {
                    weights[labels[i]][labels[j]] = Math.Abs(i - j) > tolerance ? 1 : 0;
                }
            }
            return weights;
        }
        
        public static Dictionary<LblT, Dictionary<LblT, double>> GetLinearWeights(IEnumerable<LblT> orderedLabels, bool normalize = false)
        {
            List<LblT> labels = Preconditions.CheckNotNull(orderedLabels).ToList();
            Dictionary<LblT, Dictionary<LblT, double>> weights = GetZeroMatrix(new Set<LblT>(labels));
            double step = 1;
            if (normalize)
            {
                step = 1.0 / (labels.Count - 1);
            }
            for (int i = 0; i < labels.Count; i++)
            {
                for (int j = 0; j < labels.Count; j++)
                {
                    weights[labels[i]][labels[j]] = Math.Abs(i * step - j * step);
                }
            }
            return weights;
        }

        public static Dictionary<LblT, Dictionary<LblT, double>> GetSquareWeights(IEnumerable<LblT> orderedLabels, bool normalize = false)
        {
            LblT[] labels = orderedLabels as LblT[] ?? Preconditions.CheckNotNull(orderedLabels.ToArray());

            Dictionary<LblT, Dictionary<LblT, double>> weights = GetLinearWeights(labels, normalize);
            foreach (LblT lblAct in labels)
            {
                foreach (var lblPred in labels)
                {
                    weights[lblAct][lblPred] = weights[lblAct][lblPred] * weights[lblAct][lblPred];
                }
            }
            return weights;
        }

        public bool IsSymetrical(IEnumerable<LblT> labels = null)
        {
            LblT[] values = labels == null ? Enum.GetValues(typeof(LblT)).Cast<LblT>().ToArray() : labels.ToArray();
            foreach (LblT first in values)
            {
                foreach (LblT second in values)
                {
                    if (EqualityComparer<LblT>.Default.Equals(first, second)) { continue; }
                    if (Get(first, second) != Get(second, first)) { return false; }
                }
            }
            return true;
        }
    }
}