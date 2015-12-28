using System;
using System.Collections.Generic;
using Latino.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LatinoTest.Model
{
    [TestClass]
    public class StratifiedCrossValidation
    {
        const int DatasetSize = 111;

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "items not sorted")]
        public void TestSortedCheckFail()
        {
            LabeledDataset<int, int> testSet, trainSet;
            LabeledDataset<int, int> ld = NewData(new[,] { { 1, 10 }, { 2, 1 }, { 1, 1 } });
            ld.SplitForStratifiedCrossValidation(2, 1, out trainSet, out testSet);
        }

        [TestMethod]
        public void TestGroupedCheckOk()
        {
            LabeledDataset<int, int> testSet, trainSet;
            LabeledDataset<int, int> ld = NewData(new[,] { { 1, 10 }, { 2, 1 } });
            ld.SplitForStratifiedCrossValidation(2, 1, out trainSet, out testSet);

            ld = NewData(new[,] { { 1, 10 }, { 2, 1 }, { 1, 1 } });
            ld.GroupLabels();
            ld.SplitForStratifiedCrossValidation(2, 1, out trainSet, out testSet);

            ld = NewData(new[,] { { 1, 10 }, { 2, 1 }, { 1, 1 } });
            ld.GroupLabels();
            ld.SplitForStratifiedCrossValidation(2, 1, out trainSet, out testSet);
        }

        [TestMethod]
        public void TestFolding()
        {
            for (int size = 2; size <= DatasetSize; size++)
            {
                LabeledDataset<int, int> ld = NewData(new[,] { { 1, size } }, true);
                for (int numFolds = 2; numFolds <= size; numFolds++)
                {
                    var aggTestSet = new LabeledDataset<int, int>();
                    for (int i = 0; i < numFolds; i++)
                    {
                        LabeledDataset<int, int> trainSet, testSet;
                        ld.SplitForStratifiedCrossValidation(numFolds, i + 1, out trainSet, out testSet);
                        AssertSetEquality(trainSet.Concat(testSet), ld);
                        aggTestSet.AddRange(testSet);
                    }
                    AssertSetEquality(aggTestSet, ld);
                }
            }
        }

        [TestMethod]
        public void TestEvenlyDistributed()
        {
            int size = DatasetSize;
            for (int numLabels = 2; numLabels <= size / 2; numLabels++)
            {
                var labelCounts = new int[numLabels, 2];
                for (int label = 1; label <= numLabels; label++)
                {
                    int segSize = size / numLabels;
                    if (label <= size % numLabels) { segSize++; }
                    labelCounts[label - 1, 0] = label;
                    labelCounts[label - 1, 1] = segSize;
                }
                double labelDistr = 1.0 / numLabels;
                LabeledDataset<int, int> ld = NewData(labelCounts, true);
                for (int numFolds = 2; numFolds <= size / numLabels; numFolds++)
                {
                    var aggTestSet = new LabeledDataset<int, int>();
                    for (int i = 0; i < numFolds; i++)
                    {
                        LabeledDataset<int, int> trainSet, testSet;
                        ld.SplitForStratifiedCrossValidation(numFolds, i + 1, out trainSet, out testSet);
                        AssertSetEquality(trainSet.Concat(testSet), ld);
                        aggTestSet.AddRange(testSet);

                        foreach (double distr in testSet.GroupBy(le => le.Label).Select(g => (double)g.Count() / testSet.Count))
                        {
                            Assert.IsTrue(Math.Abs(labelDistr - distr) <= 1.0 / testSet.Count);
                        }
                        foreach (double distr in trainSet.GroupBy(le => le.Label).Select(g => (double)g.Count() / trainSet.Count))
                        {
                            Assert.IsTrue(Math.Abs(labelDistr - distr) <= 1.0 / trainSet.Count);
                        }
                    }
                    AssertSetEquality(aggTestSet, ld);
                }
            }
        }

        [TestMethod]
        public void TestUnevenlyDistributed()
        {
            int size = DatasetSize;
            double[] labelDistrs = { 0.2, 0.4, 0.1, 0.3 };

            var labelCounts = new int[labelDistrs.Length, 2];
            int addedCount = 0;
            for (int label = 1; label <= labelDistrs.Length; label++)
            {
                labelCounts[label - 1, 0] = label;
                var labelCount = (int)Math.Truncate(labelDistrs[label - 1] * size);
                labelCounts[label - 1, 1] = labelCount;
                addedCount += labelCount;
            }
            for (int i = 0; i < size - addedCount; i++)
            {
                int idx = i % labelCounts.Length;
                labelCounts[idx, 1]++;
                labelDistrs[idx] = (double)labelCounts[idx, 1] / size;
            }

            LabeledDataset<int, int> ld = NewData(labelCounts, true);
            for (int numFolds = 2; numFolds <= size / labelDistrs.Length; numFolds++)
            {
                var aggTestSet = new LabeledDataset<int, int>();
                for (int i = 0; i < numFolds; i++)
                {
                    LabeledDataset<int, int> trainSet, testSet;
                    ld.SplitForStratifiedCrossValidation(numFolds, i + 1, out trainSet, out testSet);
                    AssertSetEquality(trainSet.Concat(testSet), ld);
                    aggTestSet.AddRange(testSet);

                    var test = new List<double>();
                    foreach (IGrouping<int, LabeledExample<int, int>> group in testSet.GroupBy(le => le.Label))
                    {
                        double distr = (double)group.Count() / testSet.Count;
                        int label = group.Key;
                        int j = 0;
                        for (; labelCounts[j, 0] != label; j++) { }
                        Assert.IsTrue(Math.Abs(labelDistrs[j] - distr) <= 1.0 / testSet.Count + 0.00001);
                        test.Add((double)group.Count() / testSet.Count);
                    }

                    var train = new List<double>();
                    foreach (IGrouping<int, LabeledExample<int, int>> group in trainSet.GroupBy(le => le.Label))
                    {
                        double distr = (double)group.Count() / trainSet.Count;
                        int label = group.Key;
                        int j = 0;
                        for (; labelCounts[j, 0] != label; j++) { }
                        Assert.IsTrue(Math.Abs(labelDistrs[j] - distr) <= 1.0 / trainSet.Count + 0.00001);
                        train.Add((double)group.Count() / trainSet.Count);
                    }
                }
                AssertSetEquality(aggTestSet, ld);
            }
        }

        private static void AssertSetEquality(IEnumerable<LabeledExample<int, int>> le1, IEnumerable<LabeledExample<int, int>> le2)
        {
            List<int> examples1 = le1.Select(le => le.Example).ToList();
            List<int> examples2 = le2.Select(le => le.Example).ToList();
            Assert.AreEqual(examples1.Count(), examples2.Count());
            Assert.AreEqual(examples1.Distinct().Count(), examples2.Distinct().Count());
            Assert.AreEqual(examples1.Min(), examples2.Min());
            Assert.AreEqual(examples1.Max(), examples2.Max());
        }

        private static LabeledDataset<int, int> NewData(int[,] labelCounts, bool sortShuffled = false)
        {
            var result = new LabeledDataset<int, int>();
            for (int i = 0, k = 1; i <= labelCounts.GetUpperBound(0); i++)
            {
                int label = labelCounts[i, 0], count = labelCounts[i, 1];
                for (int j = 0; j < count; j++)
                {
                    result.Add(label, k++);
                }
            }
            if (sortShuffled)
            {
                result.GroupLabels();
            }
            return result;
        }
    }
}
