using System;
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
        // class-specific metrics
        Precision,
        Recall,
        F1,
        Accuracy,
        // micro-averaged
        MicroPrecision,
        MicroRecall,
        MicroF1,
        MicroAccuracy,
        // macro-averaged
        MacroPrecision,
        MacroRecall,
        MacroF1,
        MacroAccuracy
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
            public void Resize(int n)
            {
                while (Count < n) { Add(null); }
            }

            public void Put(int foldNum, PerfMatrix<LblT> mtx)
            {
                Resize(foldNum);
                this[foldNum - 1] = mtx;
            }
        }

        private Dictionary<string, Dictionary<string, FoldData>> mData
            = new Dictionary<string, Dictionary<string, FoldData>>();
        private IEqualityComparer<LblT> mLblEqCmp
            = null;

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

        public PerfMatrix<LblT> GetPerfMatrix(string expName, string algoName, int foldNum)
        {
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            Dictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    foldData.Resize(foldNum);
                    if (foldData[foldNum - 1] == null) { foldData[foldNum - 1] = new PerfMatrix<LblT>(mLblEqCmp); }
                    return foldData[foldNum - 1];
                }
                else
                {
                    algoData.Add(algoName, foldData = new FoldData());
                    PerfMatrix<LblT> mtx = new PerfMatrix<LblT>(mLblEqCmp);
                    foldData.Put(foldNum, mtx);
                    return mtx;
                }
            }
            else
            {
                FoldData foldData = new FoldData();
                mData.Add(expName, algoData = new Dictionary<string, FoldData>());
                algoData.Add(algoName, foldData);
                PerfMatrix<LblT> mtx = new PerfMatrix<LblT>(mLblEqCmp);
                foldData.Put(foldNum, mtx);
                return mtx;
            }
        }

        public double GetVal(int foldNum, string expName, string algoName, PerfMetric metric, LblT lbl)
        {
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            Dictionary<string, FoldData> algoData;
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
            return double.NaN;
        }

        public double GetAvg(string expName, string algoName, PerfMetric metric, LblT lbl, out double stdev) // TODO: test if stdev is correct
        {
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            stdev = double.NaN;
            Dictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    double sum = 0;
                    foreach (PerfMatrix<LblT> mtx in foldData)
                    {
                        if (mtx == null) { throw new InvalidOperationException(); }
                        sum += mtx.GetScore(metric, lbl);
                    }
                    double avg = sum / (double)foldData.Count;
                    sum = 0;
                    foreach (PerfMatrix<LblT> mtx in foldData)
                    {
                        double val = mtx.GetScore(metric, lbl) - avg;
                        sum += val * val;
                    }
                    stdev = Math.Sqrt(sum / (double)foldData.Count);
                    return avg;
                }
            }
            return double.NaN;
        }

        public double GetAvg(string expName, string algoName, PerfMetric metric, LblT lbl)
        {
            double stdev;
            return GetAvg(expName, algoName, metric, lbl, out stdev); // throws ArgumentNullException, InvalidOperationException
        }

        public string ToString(int foldNum, string expName, PerfMetric metric, LblT lbl, string fmt) // set expName to null to get stats for all experiments
        {
            Utils.ThrowException(foldNum < 1 ? new ArgumentOutOfRangeException("foldNum") : null);
            Utils.ThrowException(fmt == null ? new ArgumentNullException("fmt") : null);
            ArrayList<string> expList = new ArrayList<string>(mData.Keys);
            expList.Sort();
            Set<string> tmp = new Set<string>();
            foreach (Dictionary<string, FoldData> item in mData.Values) { tmp.AddRange(item.Keys); }
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
                if (expName != null)
                {
                    double val = GetVal(foldNum, expName, algoName, metric, lbl);
                    sb.Append("\t" + val.ToString(fmt));
                }
                else
                {
                    foreach (string exp in expList)
                    {
                        sb.Append("\t" + GetVal(foldNum, exp, algoName, metric, lbl).ToString(fmt));
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
            foreach (Dictionary<string, FoldData> item in mData.Values) { tmp.AddRange(item.Keys); }
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
                        double val = GetAvg(exp, algoName, metric, lbl, out stdev); // throws InvalidOperationException
                        sb.Append("\t" + val.ToString(fmt));
                    }
                }
                else
                {
                    double stdev;
                    sb.Append("\t" + GetAvg(expName, algoName, metric, lbl, out stdev).ToString(fmt)); // throws InvalidOperationException
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
        private Dictionary<LblT, Dictionary<LblT, int>> mMtx
            = new Dictionary<LblT, Dictionary<LblT, int>>(); // TODO: set lbl equality comparer
        private Set<LblT> mLabels
            = new Set<LblT>(); // TODO: set lbl equality comparer
        private IEqualityComparer<LblT> mLblEqCmp;

        public PerfMatrix(IEqualityComparer<LblT> lblEqCmp)
        {
            mLblEqCmp = lblEqCmp;
        }

        public void AddLabels(IEnumerable<LblT> labels)
        {
            mLabels.AddRange(labels);
        }

        public void AddCount(LblT actual, LblT predicted, int count)
        {
            mLabels.AddRange(new LblT[] { actual, predicted });
            Dictionary<LblT, int> row;
            if (mMtx.TryGetValue(actual, out row))
            {
                int oldCount;
                if (row.TryGetValue(predicted, out oldCount))
                {
                    row[predicted] = oldCount + count;
                }
                else
                {
                    row.Add(predicted, count);
                }
            }
            else
            {
                mMtx.Add(actual, row = new Dictionary<LblT, int>());
                row.Add(predicted, count);
            }
        }

        public void AddCount(LblT actual, LblT predicted)
        {
            AddCount(actual, predicted, /*count=*/1);
        }

        public int Get(LblT actual, LblT predicted)
        {
            Dictionary<LblT, int> row;
            if (mMtx.TryGetValue(actual, out row))
            {
                int c;
                if (row.TryGetValue(predicted, out c))
                {
                    return c;
                }
            }
            return 0;
        }

        public void Reset()
        {
            mMtx.Clear();
            mLabels.Clear();
        }

        private int SumRow(LblT lbl)
        {
            Dictionary<LblT, int> row;
            if (mMtx.TryGetValue(lbl, out row))
            {
                int sum = 0;
                foreach (int c in row.Values) { sum += c; }
                return sum;
            }
            return 0;
        }

        private int SumCol(LblT lbl)
        {
            int sum = 0;
            foreach (Dictionary<LblT, int> row in mMtx.Values)
            {
                int c;
                if (row.TryGetValue(lbl, out c)) { sum += c; }
            }
            return sum;
        }

        private int SumAll()
        {
            int sum = 0;
            foreach (Dictionary<LblT, int> row in mMtx.Values)
            {
                foreach (int val in row.Values) { sum += val; }
            }
            return sum;
        }

        private int SumDiag()
        {
            int sum = 0;
            foreach (KeyValuePair<LblT, Dictionary<LblT, int>> row in mMtx)
            {
                int c;
                if (row.Value.TryGetValue(row.Key, out c)) { sum += c; }
            }
            return sum;
        }

        public double GetPrecision(LblT lbl)
        {
            int all;
            int precCount = GetPrecisionCount(lbl, out all);
            if (all == 0) { return 1; } // *** boundary case
            return (double)precCount / (double)all;
        }

        public int GetPrecisionCount(LblT lbl, out int all)
        {
            all = SumCol(lbl);
            return Get(lbl, lbl);
        }

        public double GetRecall(LblT lbl)
        {
            int all;
            int recallCount = GetRecallCount(lbl, out all);
            if (all == 0) { return 1; } // *** boundary case
            return (double)recallCount / (double)all;
        }

        public int GetRecallCount(LblT lbl, out int all)
        {
            all = SumRow(lbl);
            return Get(lbl, lbl);
        }

        public double GetF(double w, LblT lbl)
        {
            double p = GetPrecision(lbl);
            double r = GetRecall(lbl);
            if (p == 0 && r == 0) { return 0; } // *** boundary case
            return (w * w + 1) * p * r / (w * w * p + r);
        }

        public double GetF1(LblT lbl)
        {
            return GetF(1, lbl);
        }

        // *** Micro-averaging (over documents) ***

        public double GetMicroAverage() // *** equal to micro-precision, micro-recall, micro-F, micro-accuracy 
        {
            return (double)SumDiag() / (double)SumAll(); 
        }

        // *** Macro-averaging (over classes) ***

        public double GetMacroPrecision()
        {
            double sum = 0;
            foreach (LblT lbl in mLabels) { sum += GetPrecision(lbl); }
            return sum / (double)mLabels.Count;
        }

        public double GetMacroRecall() // *** equals to macro-accuracy
        {
            double sum = 0;
            foreach (LblT lbl in mLabels) { sum += GetRecall(lbl); }
            return sum / (double)mLabels.Count;
        }

        public double GetMacroF(double w)
        {
            double sum = 0;
            foreach (LblT lbl in mLabels) { sum += GetF(w, lbl); }
            return sum / (double)mLabels.Count;
        }

        public double GetMacroF1()
        {
            double sum = 0;
            foreach (LblT lbl in mLabels) { sum += GetF1(lbl); }
            return sum / (double)mLabels.Count;
        }

        public double GetScore(PerfMetric metric, LblT lbl)
        {
            switch (metric)
            {
                case PerfMetric.Precision:
                    return GetPrecision(lbl);
                case PerfMetric.Accuracy:
                case PerfMetric.Recall:
                    return GetRecall(lbl);
                case PerfMetric.F1:
                    return GetF1(lbl);
                case PerfMetric.MicroPrecision:
                case PerfMetric.MicroRecall:
                case PerfMetric.MicroF1:
                case PerfMetric.MicroAccuracy:
                    return GetMicroAverage();
                case PerfMetric.MacroPrecision:
                    return GetMacroPrecision();
                case PerfMetric.MacroAccuracy:
                case PerfMetric.MacroRecall:
                    return GetMacroRecall();
                case PerfMetric.MacroF1:
                    return GetMacroF1();
                default:
                    throw new ArgumentValueException("metric");
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            ArrayList<LblT> labels = new ArrayList<LblT>(mLabels.OrderBy(x => x.ToString()));
            string line = "\t";
            foreach (LblT predicted in labels)
            {
                line += predicted + "\t";
            }
            str.AppendLine(line.TrimEnd('\t'));
            foreach (LblT actual in labels)
            {
                str.Append(actual + "\t");
                line = "";
                foreach (LblT predicted in labels)
                {
                    line += Get(actual, predicted) + "\t";
                }
                str.AppendLine(line.TrimEnd('\t'));
            }
            return str.ToString().TrimEnd();
        }
    }
}