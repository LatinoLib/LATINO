using System;
using System.Collections.Generic;
using System.Linq;
using Latino.Model;

namespace Latino.TextMining
{
    public class DeltaBowSpace<LabelT> : BowSpace
    {
        private Dictionary<int, double> mWordDeltas = new Dictionary<int, double>();

        public DeltaBowSpace()
        {
        }

        public DeltaBowSpace(BinarySerializer reader) 
        {
            Load(reader);
        }

        public List<SparseVector<double>> Initialize(ILabeledDataset<LabelT, string> labeledDataset)
        {
            return Initialize(labeledDataset, false);
        }

        public override ArrayList<SparseVector<double>> Initialize(IEnumerable<string> documents, bool largeScale, bool vocabularyOnly)
        {
            throw new NotSupportedException("call of invalid method for this class");
        }

        public List<SparseVector<double>> Initialize(ILabeledDataset<LabelT, string> labeledDataset, bool largeScale)
        {
            bool normalizeVectors = NormalizeVectors;
            NormalizeVectors = false;
            List<SparseVector<double>> bowData = base.Initialize(labeledDataset.Select(d => d.Example), largeScale);
            NormalizeVectors = normalizeVectors;

            // count word label frequencies
            var labelWordCounts = new Dictionary<LabelT, Dictionary<int, int>>();
            for (int i = 0; i < bowData.Count; i++)
            {
                foreach (IdxDat<double> idxDat in bowData[i])
                {
                    LabelT label = labeledDataset[i].Label;
                    Dictionary<int, int> wordCounts;
                    if (!labelWordCounts.TryGetValue(label, out wordCounts))
                    {
                        labelWordCounts.Add(label, wordCounts = new Dictionary<int, int>());
                    }
                    int count;
                    if (!wordCounts.TryGetValue(idxDat.Idx, out count))
                    {
                        wordCounts.Add(idxDat.Idx, 1);
                    }
                    else
                    {
                        wordCounts[idxDat.Idx] = count + 1;
                    }
                }
            }

            // calc deltas
            int labelCount = labelWordCounts.Count;
            var counts = new List<double>();
            foreach (Word word in Words)
            {
                counts.Clear();
                foreach (KeyValuePair<LabelT, Dictionary<int, int>> kv in labelWordCounts)
                {
                    int count;
                    if (kv.Value.TryGetValue(word.mIdx, out count))
                    {
                        counts.Add(count);
                    }
                }
                if (counts.Any())
                {
                    double max = counts.Max();
                    mWordDeltas.Add(word.mIdx, Math.Abs(Math.Log(
                        max / Math.Max(counts.Sum() - max, 1) * (labelCount - 1), 2)));
                }
                else
                {
                    mWordDeltas.Add(word.mIdx, 1);
                }
            }

            // transform vectors using deltas
            var bowDataset = new List<SparseVector<double>>();
            foreach (SparseVector<double> bow in bowData)
            {
                CalcDeltaBow(bow, normalizeVectors);
                bowDataset.Add(bow);
            }

            return bowDataset;
        }

        public override SparseVector<double> ProcessDocument(string document, IStemmer stemmer)
        {
            bool normalizeVectors = NormalizeVectors;
            NormalizeVectors = false;
            SparseVector<double> bow = base.ProcessDocument(document, stemmer);
            NormalizeVectors = normalizeVectors;

            CalcDeltaBow(bow, normalizeVectors);
            return bow;
        }

        public override void Save(BinarySerializer writer)
        {
            base.Save(writer);
            mWordDeltas.SaveDictionary(writer);
        }

        public new void Load(BinarySerializer reader)
        {
            base.Load(reader);
            mWordDeltas = reader.LoadDictionary<int, double>();
        }

        public Tuple<Word, double>[] GetFreqWords(int count = 50)
        {
            return mWordDeltas
                .OrderByDescending(kv => kv.Value).Take(50)
                .Select(kv => new Tuple<Word, double>(Words.First(w => w.mIdx == kv.Key), kv.Value))
                .ToArray();
        }

        private void CalcDeltaBow(SparseVector<double> bow, bool normalizeVectors)
        {
            foreach (IdxDat<double> idxDat in bow)
            {
                bow[idxDat.Idx] = idxDat.Dat * mWordDeltas[idxDat.Idx];
            }
            if (normalizeVectors)
            {
                ModelUtils.TryNrmVecL2(bow);
            }
        }
    }
}