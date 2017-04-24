using System;
using System.Collections.Generic;
using System.Linq;

namespace Latino.Model.Eval
{
    public interface IAgreementExample<out T, out U>
    {
        T UnitId();
        U ObserverId();
    }

    public enum AgreementKind
    {
        Inter, Self, InterIncludingUnits, InterExcludingUnits, InterExcludingLabels
    }



    public static class StatsUtils
    {
        public static ExT[][] GetMcBootstrapSamples<ExT>(IEnumerable<ExT> examples, int numOfSamples, int seed = -1)
        {
            Preconditions.CheckArgumentRange(numOfSamples > 0);

            ExT[] sample = examples as ExT[] ?? examples.ToArray();
            var result = new ExT[numOfSamples][];
            Random random = seed >= 0 ? new Random(seed) : new Random();
            for (int i = 0; i < numOfSamples; i++)
            {
                var resampled = new ExT[sample.Length];
                for (int j = 0; j < sample.Length; j++)
                {
                    resampled[j] = sample[random.Next(sample.Length)];
                }
                result[i] = resampled;
            }
            return result;
        }


        /* Used for calculating Krripendorff alpha
         */
        public static PerfMatrix<LblT> GetCoincidenceMatrix<LblT>(PerfMatrix<LblT> mtx, IEnumerable<LblT> excludeLabels = null)
        {
            Preconditions.CheckNotNull(mtx);
            LblT[] values = Enum.GetValues(typeof(LblT)).Cast<LblT>()
                .Where(v => excludeLabels == null || !excludeLabels.Contains(v)).ToArray();
            var result = new PerfMatrix<LblT>();
            foreach (LblT first in values)
            {
                foreach (LblT second in values)
                {
                    result.AddCount(first, second, mtx.Get(first, second) + mtx.Get(second, first));
                }
            }
            return result;
        }

        public static PerfMatrix<LblT> GetCoincidenceMatrix<LblT, T, U>(IEnumerable<ILabeledExample<LblT, IAgreementExample<T, U>>> examples,
            AgreementKind kind = AgreementKind.Inter, U observerId = default(U))
        {
            int count;
            return GetCoincidenceMatrix(new PerfMatrix<LblT>(), examples, out count, kind, observerId);
        }

        public static PerfMatrix<LblT> GetCoincidenceMatrix<LblT, T, U>(IEnumerable<ILabeledExample<LblT, IAgreementExample<T, U>>> examples,
            out int unitCount, AgreementKind kind = AgreementKind.Inter, U observerId = default(U))
        {
            return GetCoincidenceMatrix(new PerfMatrix<LblT>(), examples, out unitCount, kind, observerId);
        }

        public static PerfMatrix<LblT> GetCoincidenceMatrix<LblT, T, U>(PerfMatrix<LblT> mtx, IEnumerable<ILabeledExample<LblT, IAgreementExample<T, U>>> examples,
            AgreementKind kind = AgreementKind.Inter, U observerId = default(U))
        {
            int count;
            return GetCoincidenceMatrix(mtx, examples, out count, kind, observerId);
        }

        public static PerfMatrix<LblT> GetCoincidenceMatrix<LblT, T, U>(PerfMatrix<LblT> mtx, IEnumerable<ILabeledExample<LblT, IAgreementExample<T, U>>> examples,
            out int unitCount, AgreementKind kind = AgreementKind.Inter, U observerId = default(U))
        {
            Preconditions.CheckNotNull(mtx);
            mtx.AddLabels(Enum.GetValues(typeof(LblT)).Cast<LblT>());
            ILabeledExample<LblT, IAgreementExample<T, U>>[][] unitGroups = GroupForAgreementKind(examples, kind, observerId);
            unitCount = 0;
            foreach (ILabeledExample<LblT, IAgreementExample<T, U>>[] ug in unitGroups)
            {
                List<LblT> pairLab1 = new List<LblT>(), pairLab2 = new List<LblT>();
                for (int i = 0; i < ug.Length; i++)
                {
                    for (int j = i + 1; j < ug.Length; j++)
                    {
                        bool equalObserver = EqualityComparer<U>.Default.Equals(ug[i].Example.ObserverId(), ug[j].Example.ObserverId());
                        if (kind == AgreementKind.Self && equalObserver || kind != AgreementKind.Self && !equalObserver)
                        {
                            pairLab1.Add(ug[i].Label); 
                            pairLab2.Add(ug[j].Label);
                        }
                    }
                }
                if (pairLab1.Any())
                {
                    double w = kind == AgreementKind.Self ? ug.Length : ug.GroupBy(u => u.Example.ObserverId()).Count();
                    double inc = w / pairLab1.Count / 2;
                    for (int i = 0; i < pairLab1.Count; i++)
                    {
                        mtx.AddCount(pairLab1[i], pairLab2[i], inc);
                        mtx.AddCount(pairLab2[i], pairLab1[i], inc);
                    }
                    unitCount++;
                }
            }
            return mtx;
        }

        private static ILabeledExample<LblT, IAgreementExample<T, U>>[][] GroupForAgreementKind<LblT, T, U>(
            IEnumerable<ILabeledExample<LblT, IAgreementExample<T, U>>> examples, AgreementKind kind, U observerId)
        {
            ILabeledExample<LblT, IAgreementExample<T, U>>[][] unitGroups = Preconditions.CheckNotNull(examples)
                .GroupBy(e => e.Example.UnitId())
                .Where(g => g.Count() > 1 && !EqualityComparer<T>.Default.Equals(g.Key, default(T))) // skip nulls or zeros
                .Select(g => g.ToArray()).ToArray();

            switch (kind)
            {
                case AgreementKind.Self:
                    unitGroups = EqualityComparer<U>.Default.Equals(observerId, default(U))
                        ? unitGroups // observer not specified 
                            .Select(u => u.GroupBy(e => e.Example.ObserverId())
                                .Where(g => g.Count() > 1 && !EqualityComparer<U>.Default.Equals(g.Key, default(U)))
                                .SelectMany(g => g).ToArray())
                            .Where(u => u.Count() > 1).ToArray()
                        : unitGroups
                            .Select(u => u.Where(e => EqualityComparer<U>.Default.Equals(e.Example.ObserverId(), observerId)).ToArray())
                            .Where(u => u.Count() > 1).ToArray();
                    break;
                case AgreementKind.InterExcludingLabels:
                    unitGroups = unitGroups
                        .Select(u => u.Where(e => !EqualityComparer<U>.Default.Equals(e.Example.ObserverId(), observerId)).ToArray())
                        .Where(u => u.Count() > 1).ToArray();
                    break;
                case AgreementKind.InterIncludingUnits:
                    unitGroups = unitGroups
                        .Where(u => u.Any(e => EqualityComparer<U>.Default.Equals(e.Example.ObserverId(), observerId)))
                        .ToArray();
                    break;
                case AgreementKind.InterExcludingUnits:
                    unitGroups = unitGroups
                        .Where(u => u.All(e => !EqualityComparer<U>.Default.Equals(e.Example.ObserverId(), observerId)))
                        .ToArray();
                    break;
            }
            return unitGroups;
        }

        public static double GetZProb(double zScore)
        {
            return GetZProbFrom0ToZ(zScore) * 2;
        }

        public static double GetZProbFrom0ToZ(double zScore)
        {
            Preconditions.CheckArgument(zScore >= 0);
            int idx = Math.Abs(mZScoreZeroProbs.BinarySearch(new Tuple<double, double>(zScore, Double.NaN), TupleItem1Comparer.Instance));
            return mZScoreZeroProbs[Math.Min(idx, mZScoreZeroProbs.Count - 1)].Item2;
        }

        public static double GetZScore(double probPlusMinusZ)
        {
            return GetZScoreByProbFrom0ToZ(probPlusMinusZ / 2);
        }

        public static double GetZScoreByProbFrom0ToZ(double probFrom0ToZ)
        {
            Preconditions.CheckArgument(probFrom0ToZ >= 0);
            if (mZeroProbsZScores == null)
            {
                mZeroProbsZScores = mZScoreZeroProbs.Select(t => new Tuple<double, double>(t.Item2, t.Item1)).OrderBy(t => t.Item1).ToList();
            }

            int idx = Math.Abs(mZeroProbsZScores.BinarySearch(new Tuple<double, double>(probFrom0ToZ, Double.NaN), TupleItem1Comparer.Instance));
            return mZeroProbsZScores[Math.Min(idx, mZeroProbsZScores.Count - 1)].Item2;
        }

        // probability that a statistic is between 0 (the mean) and Z
        private static List<Tuple<double, double>> mZeroProbsZScores;
        private static readonly List<Tuple<double, double>> mZScoreZeroProbs = new List<Tuple<double, double>>
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
}