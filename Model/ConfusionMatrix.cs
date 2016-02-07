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
        KAlphaLinear,
        KAlphaInterval,
        F1AvgExtremeClasses,
        AccuracyTolerance1
    }

    public static class StdErrTables
    {
        public static double GetProb_0ToZ(double zScore)
        {
            Preconditions.CheckArgument(zScore >= 0);
            int idx = Math.Abs(mZScoreZeroProbs.BinarySearch(new Tuple<double, double>(zScore, double.NaN), TupleItem1Comparer.Instance));
            return idx < mZScoreZeroProbs.Count ? mZScoreZeroProbs[idx].Item2 : 0.49999;
        }

        public static double GetZScore(double prob_0ToZ)
        {
            Preconditions.CheckArgument(prob_0ToZ >= 0);
            if (mZeroProbsZScores == null)
            {
                mZeroProbsZScores = mZScoreZeroProbs.Select(t => new Tuple<double, double>(t.Item2, t.Item1)).OrderBy(t => t.Item1).ToList();
            }

            int idx = Math.Abs(mZeroProbsZScores.BinarySearch(new Tuple<double, double>(1.96, double.NaN), TupleItem1Comparer.Instance));
            return mZeroProbsZScores[idx].Item2;
        }

        // probability that a statistic is between 0 (the mean) and Z
        private static readonly List<Tuple<double, double>> mZScoreZeroProbs = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(0, 0.00000), new Tuple<double, double>(0.01, 0.00399),
                new Tuple<double, double>(0.02, 0.00798), new Tuple<double, double>(0.03, 0.01197),
                new Tuple<double, double>(0.04, 0.01595), new Tuple<double, double>(0.05, 0.01994),
                new Tuple<double, double>(0.06, 0.02392), new Tuple<double, double>(0.07, 0.02790),
                new Tuple<double, double>(0.08, 0.03188), new Tuple<double, double>(0.09, 0.03586),
                new Tuple<double, double>(0.1, 0.03980), new Tuple<double, double>(0.11, 0.04380),
                new Tuple<double, double>(0.12, 0.04776), new Tuple<double, double>(0.13, 0.05172),
                new Tuple<double, double>(0.14, 0.05567), new Tuple<double, double>(0.15, 0.05966),
                new Tuple<double, double>(0.16, 0.06360), new Tuple<double, double>(0.17, 0.06749),
                new Tuple<double, double>(0.18, 0.07142), new Tuple<double, double>(0.19, 0.07535),
                new Tuple<double, double>(0.2, 0.07930), new Tuple<double, double>(0.21, 0.08317),
                new Tuple<double, double>(0.22, 0.08706), new Tuple<double, double>(0.23, 0.09095),
                new Tuple<double, double>(0.24, 0.09483), new Tuple<double, double>(0.25, 0.09871),
                new Tuple<double, double>(0.26, 0.10257), new Tuple<double, double>(0.27, 0.10642),
                new Tuple<double, double>(0.28, 0.11026), new Tuple<double, double>(0.29, 0.11409),
                new Tuple<double, double>(0.3, 0.11791), new Tuple<double, double>(0.31, 0.12172),
                new Tuple<double, double>(0.32, 0.12552), new Tuple<double, double>(0.33, 0.12930),
                new Tuple<double, double>(0.34, 0.13307), new Tuple<double, double>(0.35, 0.13683),
                new Tuple<double, double>(0.36, 0.14058), new Tuple<double, double>(0.37, 0.14431),
                new Tuple<double, double>(0.38, 0.14803), new Tuple<double, double>(0.39, 0.15173),
                new Tuple<double, double>(0.4, 0.15542), new Tuple<double, double>(0.41, 0.15910),
                new Tuple<double, double>(0.42, 0.16276), new Tuple<double, double>(0.43, 0.16640),
                new Tuple<double, double>(0.44, 0.17003), new Tuple<double, double>(0.45, 0.17364),
                new Tuple<double, double>(0.46, 0.17724), new Tuple<double, double>(0.47, 0.18082),
                new Tuple<double, double>(0.48, 0.18439), new Tuple<double, double>(0.49, 0.18793),
                new Tuple<double, double>(0.5, 0.19146), new Tuple<double, double>(0.51, 0.19497),
                new Tuple<double, double>(0.52, 0.19847), new Tuple<double, double>(0.53, 0.20194),
                new Tuple<double, double>(0.54, 0.20540), new Tuple<double, double>(0.55, 0.20884),
                new Tuple<double, double>(0.56, 0.21226), new Tuple<double, double>(0.57, 0.21566),
                new Tuple<double, double>(0.58, 0.21904), new Tuple<double, double>(0.59, 0.22240),
                new Tuple<double, double>(0.6, 0.22575), new Tuple<double, double>(0.61, 0.22907),
                new Tuple<double, double>(0.62, 0.23237), new Tuple<double, double>(0.63, 0.23565),
                new Tuple<double, double>(0.64, 0.23891), new Tuple<double, double>(0.65, 0.24215),
                new Tuple<double, double>(0.66, 0.24537), new Tuple<double, double>(0.67, 0.24857),
                new Tuple<double, double>(0.68, 0.25175), new Tuple<double, double>(0.69, 0.25490),
                new Tuple<double, double>(0.7, 0.25804), new Tuple<double, double>(0.71, 0.26115),
                new Tuple<double, double>(0.72, 0.26424), new Tuple<double, double>(0.73, 0.26730),
                new Tuple<double, double>(0.74, 0.27035), new Tuple<double, double>(0.75, 0.27337),
                new Tuple<double, double>(0.76, 0.27637), new Tuple<double, double>(0.77, 0.27935),
                new Tuple<double, double>(0.78, 0.28230), new Tuple<double, double>(0.79, 0.28524),
                new Tuple<double, double>(0.8, 0.28814), new Tuple<double, double>(0.81, 0.29103),
                new Tuple<double, double>(0.82, 0.29389), new Tuple<double, double>(0.83, 0.29673),
                new Tuple<double, double>(0.84, 0.29955), new Tuple<double, double>(0.85, 0.30234),
                new Tuple<double, double>(0.86, 0.30511), new Tuple<double, double>(0.87, 0.30785),
                new Tuple<double, double>(0.88, 0.31057), new Tuple<double, double>(0.89, 0.31327),
                new Tuple<double, double>(0.9, 0.31594), new Tuple<double, double>(0.91, 0.31859),
                new Tuple<double, double>(0.92, 0.32121), new Tuple<double, double>(0.93, 0.32381),
                new Tuple<double, double>(0.94, 0.32639), new Tuple<double, double>(0.95, 0.32894),
                new Tuple<double, double>(0.96, 0.33147), new Tuple<double, double>(0.97, 0.33398),
                new Tuple<double, double>(0.98, 0.33646), new Tuple<double, double>(0.99, 0.33891),
                new Tuple<double, double>(1, 0.34134), new Tuple<double, double>(1.01, 0.34375),
                new Tuple<double, double>(1.02, 0.34614), new Tuple<double, double>(1.03, 0.34849),
                new Tuple<double, double>(1.04, 0.35083), new Tuple<double, double>(1.05, 0.35314),
                new Tuple<double, double>(1.06, 0.35543), new Tuple<double, double>(1.07, 0.35769),
                new Tuple<double, double>(1.08, 0.35993), new Tuple<double, double>(1.09, 0.36214),
                new Tuple<double, double>(1.1, 0.36433), new Tuple<double, double>(1.11, 0.36650),
                new Tuple<double, double>(1.12, 0.36864), new Tuple<double, double>(1.13, 0.37076),
                new Tuple<double, double>(1.14, 0.37286), new Tuple<double, double>(1.15, 0.37493),
                new Tuple<double, double>(1.16, 0.37698), new Tuple<double, double>(1.17, 0.37900),
                new Tuple<double, double>(1.18, 0.38100), new Tuple<double, double>(1.19, 0.38298),
                new Tuple<double, double>(1.2, 0.38493), new Tuple<double, double>(1.21, 0.38686),
                new Tuple<double, double>(1.22, 0.38877), new Tuple<double, double>(1.23, 0.39065),
                new Tuple<double, double>(1.24, 0.39251), new Tuple<double, double>(1.25, 0.39435),
                new Tuple<double, double>(1.26, 0.39617), new Tuple<double, double>(1.27, 0.39796),
                new Tuple<double, double>(1.28, 0.39973), new Tuple<double, double>(1.29, 0.40147),
                new Tuple<double, double>(1.3, 0.40320), new Tuple<double, double>(1.31, 0.40490),
                new Tuple<double, double>(1.32, 0.40658), new Tuple<double, double>(1.33, 0.40824),
                new Tuple<double, double>(1.34, 0.40988), new Tuple<double, double>(1.35, 0.41149),
                new Tuple<double, double>(1.36, 0.41308), new Tuple<double, double>(1.37, 0.41466),
                new Tuple<double, double>(1.38, 0.41621), new Tuple<double, double>(1.39, 0.41774),
                new Tuple<double, double>(1.4, 0.41924), new Tuple<double, double>(1.41, 0.42073),
                new Tuple<double, double>(1.42, 0.42220), new Tuple<double, double>(1.43, 0.42364),
                new Tuple<double, double>(1.44, 0.42507), new Tuple<double, double>(1.45, 0.42647),
                new Tuple<double, double>(1.46, 0.42785), new Tuple<double, double>(1.47, 0.42922),
                new Tuple<double, double>(1.48, 0.43056), new Tuple<double, double>(1.49, 0.43189),
                new Tuple<double, double>(1.5, 0.43319), new Tuple<double, double>(1.51, 0.43448),
                new Tuple<double, double>(1.52, 0.43574), new Tuple<double, double>(1.53, 0.43699),
                new Tuple<double, double>(1.54, 0.43822), new Tuple<double, double>(1.55, 0.43943),
                new Tuple<double, double>(1.56, 0.44062), new Tuple<double, double>(1.57, 0.44179),
                new Tuple<double, double>(1.58, 0.44295), new Tuple<double, double>(1.59, 0.44408),
                new Tuple<double, double>(1.6, 0.44520), new Tuple<double, double>(1.61, 0.44630),
                new Tuple<double, double>(1.62, 0.44738), new Tuple<double, double>(1.63, 0.44845),
                new Tuple<double, double>(1.64, 0.44950), new Tuple<double, double>(1.65, 0.45053),
                new Tuple<double, double>(1.66, 0.45154), new Tuple<double, double>(1.67, 0.45254),
                new Tuple<double, double>(1.68, 0.45352), new Tuple<double, double>(1.69, 0.45449),
                new Tuple<double, double>(1.7, 0.45543), new Tuple<double, double>(1.71, 0.45637),
                new Tuple<double, double>(1.72, 0.45728), new Tuple<double, double>(1.73, 0.45818),
                new Tuple<double, double>(1.74, 0.45907), new Tuple<double, double>(1.75, 0.45994),
                new Tuple<double, double>(1.76, 0.46080), new Tuple<double, double>(1.77, 0.46164),
                new Tuple<double, double>(1.78, 0.46246), new Tuple<double, double>(1.79, 0.46327),
                new Tuple<double, double>(1.8, 0.46407), new Tuple<double, double>(1.81, 0.46485),
                new Tuple<double, double>(1.82, 0.46562), new Tuple<double, double>(1.83, 0.46638),
                new Tuple<double, double>(1.84, 0.46712), new Tuple<double, double>(1.85, 0.46784),
                new Tuple<double, double>(1.86, 0.46856), new Tuple<double, double>(1.87, 0.46926),
                new Tuple<double, double>(1.88, 0.46995), new Tuple<double, double>(1.89, 0.47062),
                new Tuple<double, double>(1.9, 0.47128), new Tuple<double, double>(1.91, 0.47193),
                new Tuple<double, double>(1.92, 0.47257), new Tuple<double, double>(1.93, 0.47320),
                new Tuple<double, double>(1.94, 0.47381), new Tuple<double, double>(1.95, 0.47441),
                new Tuple<double, double>(1.96, 0.47500), new Tuple<double, double>(1.97, 0.47558),
                new Tuple<double, double>(1.98, 0.47615), new Tuple<double, double>(1.99, 0.47670),
                new Tuple<double, double>(2, 0.47725), new Tuple<double, double>(2.01, 0.47778),
                new Tuple<double, double>(2.02, 0.47831), new Tuple<double, double>(2.03, 0.47882),
                new Tuple<double, double>(2.04, 0.47932), new Tuple<double, double>(2.05, 0.47982),
                new Tuple<double, double>(2.06, 0.48030), new Tuple<double, double>(2.07, 0.48077),
                new Tuple<double, double>(2.08, 0.48124), new Tuple<double, double>(2.09, 0.48169),
                new Tuple<double, double>(2.1, 0.48214), new Tuple<double, double>(2.11, 0.48257),
                new Tuple<double, double>(2.12, 0.48300), new Tuple<double, double>(2.13, 0.48341),
                new Tuple<double, double>(2.14, 0.48382), new Tuple<double, double>(2.15, 0.48422),
                new Tuple<double, double>(2.16, 0.48461), new Tuple<double, double>(2.17, 0.48500),
                new Tuple<double, double>(2.18, 0.48537), new Tuple<double, double>(2.19, 0.48574),
                new Tuple<double, double>(2.2, 0.48610), new Tuple<double, double>(2.21, 0.48645),
                new Tuple<double, double>(2.22, 0.48679), new Tuple<double, double>(2.23, 0.48713),
                new Tuple<double, double>(2.24, 0.48745), new Tuple<double, double>(2.25, 0.48778),
                new Tuple<double, double>(2.26, 0.48809), new Tuple<double, double>(2.27, 0.48840),
                new Tuple<double, double>(2.28, 0.48870), new Tuple<double, double>(2.29, 0.48899),
                new Tuple<double, double>(2.3, 0.48928), new Tuple<double, double>(2.31, 0.48956),
                new Tuple<double, double>(2.32, 0.48983), new Tuple<double, double>(2.33, 0.49010),
                new Tuple<double, double>(2.34, 0.49036), new Tuple<double, double>(2.35, 0.49061),
                new Tuple<double, double>(2.36, 0.49086), new Tuple<double, double>(2.37, 0.49111),
                new Tuple<double, double>(2.38, 0.49134), new Tuple<double, double>(2.39, 0.49158),
                new Tuple<double, double>(2.4, 0.49180), new Tuple<double, double>(2.41, 0.49202),
                new Tuple<double, double>(2.42, 0.49224), new Tuple<double, double>(2.43, 0.49245),
                new Tuple<double, double>(2.44, 0.49266), new Tuple<double, double>(2.45, 0.49286),
                new Tuple<double, double>(2.46, 0.49305), new Tuple<double, double>(2.47, 0.49324),
                new Tuple<double, double>(2.48, 0.49343), new Tuple<double, double>(2.49, 0.49361),
                new Tuple<double, double>(2.5, 0.49379), new Tuple<double, double>(2.51, 0.49396),
                new Tuple<double, double>(2.52, 0.49413), new Tuple<double, double>(2.53, 0.49430),
                new Tuple<double, double>(2.54, 0.49446), new Tuple<double, double>(2.55, 0.49461),
                new Tuple<double, double>(2.56, 0.49477), new Tuple<double, double>(2.57, 0.49492),
                new Tuple<double, double>(2.58, 0.49506), new Tuple<double, double>(2.59, 0.49520),
                new Tuple<double, double>(2.6, 0.49534), new Tuple<double, double>(2.61, 0.49547),
                new Tuple<double, double>(2.62, 0.49560), new Tuple<double, double>(2.63, 0.49573),
                new Tuple<double, double>(2.64, 0.49585), new Tuple<double, double>(2.65, 0.49598),
                new Tuple<double, double>(2.66, 0.49609), new Tuple<double, double>(2.67, 0.49621),
                new Tuple<double, double>(2.68, 0.49632), new Tuple<double, double>(2.69, 0.49643),
                new Tuple<double, double>(2.7, 0.49653), new Tuple<double, double>(2.71, 0.49664),
                new Tuple<double, double>(2.72, 0.49674), new Tuple<double, double>(2.73, 0.49683),
                new Tuple<double, double>(2.74, 0.49693), new Tuple<double, double>(2.75, 0.49702),
                new Tuple<double, double>(2.76, 0.49711), new Tuple<double, double>(2.77, 0.49720),
                new Tuple<double, double>(2.78, 0.49728), new Tuple<double, double>(2.79, 0.49736),
                new Tuple<double, double>(2.8, 0.49744), new Tuple<double, double>(2.81, 0.49752),
                new Tuple<double, double>(2.82, 0.49760), new Tuple<double, double>(2.83, 0.49767),
                new Tuple<double, double>(2.84, 0.49774), new Tuple<double, double>(2.85, 0.49781),
                new Tuple<double, double>(2.86, 0.49788), new Tuple<double, double>(2.87, 0.49795),
                new Tuple<double, double>(2.88, 0.49801), new Tuple<double, double>(2.89, 0.49807),
                new Tuple<double, double>(2.9, 0.49813), new Tuple<double, double>(2.91, 0.49819),
                new Tuple<double, double>(2.92, 0.49825), new Tuple<double, double>(2.93, 0.49831),
                new Tuple<double, double>(2.94, 0.49836), new Tuple<double, double>(2.95, 0.49841),
                new Tuple<double, double>(2.96, 0.49846), new Tuple<double, double>(2.97, 0.49851),
                new Tuple<double, double>(2.98, 0.49856), new Tuple<double, double>(2.99, 0.49861),
                new Tuple<double, double>(3, 0.49865), new Tuple<double, double>(3.01, 0.49869),
                new Tuple<double, double>(3.02, 0.49874), new Tuple<double, double>(3.03, 0.49878),
                new Tuple<double, double>(3.04, 0.49882), new Tuple<double, double>(3.05, 0.49886),
                new Tuple<double, double>(3.06, 0.49889), new Tuple<double, double>(3.07, 0.49893),
                new Tuple<double, double>(3.08, 0.49896), new Tuple<double, double>(3.09, 0.49900)
            };

        private static List<Tuple<double, double>> mZeroProbsZScores;

        private class TupleItem1Comparer : IComparer<Tuple<double, double>>
        {
            public static readonly TupleItem1Comparer Instance = new TupleItem1Comparer();

            private TupleItem1Comparer()
            {
            }

            public int Compare(Tuple<double, double> x, Tuple<double, double> y)
            {
                return x.Item1 < y.Item1 ? -1 : (x.Item1 > y.Item1 ? 1 : 0);
            }
        }
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
        private readonly IEqualityComparer<LblT> mLblEqCmp
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
                else
                {
                    if (!algoData.TryAdd(algoName, foldData = new FoldData())) { foldData = algoData[algoName]; }
                    PerfMatrix<LblT> mtx = new PerfMatrix<LblT>(mLblEqCmp);
                    foldData.Put(foldNum, mtx);
                    return mtx;
                }
            }
            else
            {
                if (!mData.TryAdd(expName, algoData = new ConcurrentDictionary<string, FoldData>())) { algoData = mData[expName]; }
                FoldData foldData = new FoldData();
                if (!algoData.TryAdd(algoName, foldData)) { foldData = algoData[algoName]; }
                PerfMatrix<LblT> mtx = new PerfMatrix<LblT>(mLblEqCmp);
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
            return double.NaN;
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
            return double.NaN;
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
            return double.NaN;
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
            double zval;
            Preconditions.CheckArgument(PerfMatrix<LblT>.StdErrProbZValues.TryGetValue(confidenceLevel, out zval));
            double stddev;
            double avg = GetAvg(expName, algoName, metric, out stddev);
            stderr = zval * stddev / Math.Sqrt(GetFoldCount(expName, algoName));
            return avg;
        }

        public double GetAvgStdErr(string expName, string algoName, ClassPerfMetric metric, LblT lbl, out double stderr, double confidenceLevel = 0.95)
        {
            double zval;
            Preconditions.CheckArgument(PerfMatrix<LblT>.StdErrProbZValues.TryGetValue(confidenceLevel, out zval));
            double stddev;
            double avg = GetAvg(expName, algoName, metric, lbl, out stddev);
            stderr = zval * stddev / Math.Sqrt(GetFoldCount(expName, algoName));
            return avg;
        }

        public double GetAvgStdErr(string expName, string algoName, OrdinalPerfMetric metric, IEnumerable<LblT> orderedLabels, out double stderr, double confidenceLevel = 0.95)
        {
            double zval;
            Preconditions.CheckArgument(PerfMatrix<LblT>.StdErrProbZValues.TryGetValue(confidenceLevel, out zval));
            double stddev;
            double avg = GetAvg(expName, algoName, metric, orderedLabels, out stddev);
            stderr = zval * stddev / Math.Sqrt(GetFoldCount(expName, algoName));
            return avg;
        }

        private double GetAvg(string expName, string algoName, Func<PerfMatrix<LblT>, double> scoreFunc, out double stdev) // TODO: test if stdev is correct
        {
            Preconditions.CheckNotNull(scoreFunc);
            Utils.ThrowException(expName == null ? new ArgumentNullException("expName") : null);
            Utils.ThrowException(algoName == null ? new ArgumentNullException("algoName") : null);
            stdev = double.NaN;
            ConcurrentDictionary<string, FoldData> algoData;
            if (mData.TryGetValue(expName, out algoData))
            {
                FoldData foldData;
                if (algoData.TryGetValue(algoName, out foldData))
                {
                    double[] results = foldData
                        .Where(m => m != null)
                        .Select(scoreFunc)
                        .Where(r => !double.IsNaN(r) && !double.IsInfinity(r)).ToArray();
                    if (!results.Any()) { return double.NaN; }

                    double avg = results.Average();
                    stdev = Math.Sqrt(results.Select(r => (r - avg) * (r - avg)).Sum() / results.Length);
                    return avg;
                }
            }
            return double.NaN;
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
        public static Dictionary<double, double> StdErrProbZValues = new Dictionary<double, double>
            {
                { 0.9, 1.64 }, { 0.95, 1.96 }, { 0.99, 2.58 }, 
            };

        private ConcurrentDictionary<LblT, ConcurrentDictionary<LblT, int>> mMtx
            = new ConcurrentDictionary<LblT, ConcurrentDictionary<LblT, int>>(); // TODO: set lbl equality comparer

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

        public bool RemoveLabel(LblT label)
        {
            return mLabels.Remove(label);
        }

        public void AddCount(LblT actual, LblT predicted, int count)
        {
            mLabels.AddRange(new[] { actual, predicted });
            ConcurrentDictionary<LblT, int> row;
            if (mMtx.TryGetValue(actual, out row))
            {
                int oldCount;
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
                if (!mMtx.TryAdd(actual, row = new ConcurrentDictionary<LblT, int>())) 
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

        public int Get(LblT actual, LblT predicted)
        {
            ConcurrentDictionary<LblT, int> row;
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

        public int GetActual(LblT lbl)
        {
            return SumRow(lbl);
        }

        public int GetPredicted(LblT lbl)
        {
            return SumCol(lbl);
        }

        public void Reset()
        {
            mMtx.Clear();
            mLabels.Clear();
        }

        private int SumRow(LblT lbl)
        {
            ConcurrentDictionary<LblT, int> row;
            if (mMtx.TryGetValue(lbl, out row))
            {
                int sum = 0;
                foreach (int c in row.Values)
                {
                    sum += c;
                }
                return sum;
            }
            return 0;
        }

        private int SumCol(LblT lbl)
        {
            int sum = 0;
            foreach (ConcurrentDictionary<LblT, int> row in mMtx.Values)
            {
                int c;
                if (row.TryGetValue(lbl, out c))
                {
                    sum += c;
                }
            }
            return sum;
        }

        public int GetSumAll()
        {
            int sum = 0;
            foreach (ConcurrentDictionary<LblT, int> row in mMtx.Values)
            {
                foreach (int val in row.Values)
                {
                    sum += val;
                }
            }
            return sum;
        }

        public int GetSumDiag()
        {
            int sum = 0;
            foreach (KeyValuePair<LblT, ConcurrentDictionary<LblT, int>> row in mMtx)
            {
                int c;
                if (row.Value.TryGetValue(row.Key, out c))
                {
                    sum += c;
                }
            }
            return sum;
        }

        public double GetPrecision(LblT lbl)
        {
            int all;
            int precCount = GetPrecisionCount(lbl, out all);
            return (double) precCount / all;
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
            return (double) recallCount / all;
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
            return (w * w + 1) * p * r / (w * w * p + r);
        }

        public double GetF1(LblT lbl)
        {
            return GetF(1, lbl);
        }

        public int GetActualSum(LblT lbl)
        {
            return SumRow(lbl);
        }

        public double GetActualRatio(LblT lbl)
        {
            return (double)GetActualSum(lbl) / GetSumAll();
        }

        public int GetPredictedSum(LblT lbl)
        {
            return SumCol(lbl);
        }

        public double GetPredictedRatio(LblT lbl)
        {
            return (double)GetPredictedSum(lbl) / GetSumAll();
        }


        // *** Micro-averaging (over examples) ***

        public double GetAccuracy()
        {
            return (double) GetSumDiag() / GetSumAll();
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
                case PerfMetric.KAlphaNominal:
                    return GetKAlpha();
                case PerfMetric.AccStdErrorConf90:
                    return GetAccStdError(0.9);
                case PerfMetric.AccStdErrorConf95:
                    return GetAccStdError(0.95);
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
            StringBuilder str = new StringBuilder();
            ArrayList<LblT> labels = new ArrayList<LblT>(mLabels.OrderBy(x => x.ToString()));
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
                    str.Append(Get(actual, predicted).ToString(CultureInfo.InvariantCulture).PadLeft(len));
                }
                str.Append("|".PadLeft(4));
                str.Append(SumRow(actual).ToString(CultureInfo.InvariantCulture).PadLeft(len));
                str.Append(((float) SumRow(actual)/all).ToString("P1").PadLeft(len));
                str.AppendLine();
            }

            str.AppendLine(new string('-', (labels.Count + 3)*len + 8));
            str.Append("sum predicted".PadLeft(len));
            str.Append("|".PadLeft(4));
            foreach (LblT predicted in labels)
            {
                str.Append(SumCol(predicted).ToString(CultureInfo.InvariantCulture).PadLeft(len));
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


        public double GetAccStdError(double confidenceLevel = 0.95)
        {
            double z;
            Preconditions.CheckArgument(StdErrProbZValues.TryGetValue(confidenceLevel, out z));
            double acc = GetAccuracy();
            return z * Math.Sqrt(acc * (1 - acc) / GetSumAll());
        }

        // implementation of http://vassarstats.net/kappaexp.html and http://vassarstats.net/kappa.html
        // correction from wikipedia Krippendorff's Alpha
        public double GetKrippendorffsAlpha(IEnumerable<LblT> orderedLabels, Dictionary<LblT, Dictionary<LblT, double>> weights)
        {
            Preconditions.CheckNotNull(weights);
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
                var row = labels.ToDictionary(predicted => predicted, predicted => GetActual(actual) / s * GetPredicted(predicted) / (s - 1)); // a[i] * p[j] / s / (s - 1)
                expected.Add(actual, row);
            }

            // the weights matrix
            weights = weights.ToDictionary(kv => kv.Key, kv => kv.Value); // modify the copy
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
            return 1 - (1 - s1) / (1 - s2);            
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


        // (weights) matrix to string
        /*
        public string ToString(IEnumerable<LblT> orderedLabels, Dictionary<LblT, Dictionary<LblT, double>> matrix)
        {          
            List<LblT> labels = Preconditions.CheckNotNull(orderedLabels).ToList();
            StringBuilder str = new StringBuilder();

            int len = Math.Max(labels.Max(l => l.ToString().Length), 11);

            str.Append("".PadRight(len));
            str.Append("|".PadLeft(4));
            foreach (LblT predicted in labels)
            {
                str.Append(predicted.ToString().PadLeft(len));
            }
            str.AppendLine();
            str.AppendLine(new string('-', (labels.Count + 1) * len + 4));            
            foreach (LblT actual in labels)
            {
                str.Append(actual.ToString().PadLeft(len));
                str.Append("|".PadLeft(4));
                foreach (LblT predicted in labels)
                {
                    str.Append(matrix[actual][predicted].ToString().PadLeft(len));
                }
                str.AppendLine();
            }
            return str.ToString();
        }
        */

    }

}