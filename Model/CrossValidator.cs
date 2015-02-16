using System;
using System.Collections.Generic;

namespace Latino.Model.Eval
{
    public class CrossValidator<LblT, ExT> : MappingCrossValidator<LblT, ExT, ExT>
    {
        protected override ILabeledDataset<LblT, ExT> MapTestSet(int foldN, ILabeledDataset<LblT, ExT> testSet)
        {
            return TestSetFunc != null ? TestSetFunc(this, foldN, testSet) : testSet;
        }

        protected override ILabeledDataset<LblT, ExT> MapTrainSet(int foldN, ILabeledDataset<LblT, ExT> trainSet)
        {
            return TrainSetFunc != null ? TrainSetFunc(this, foldN, trainSet) : trainSet;
        }
    }

    public class MappingCrossValidator<LblT, ExT1, ExT2>
    {
        public MappingCrossValidator()
        {
            NumFolds = 10;
            IsStratified = true;
            ExpName = "";
            AlgName = "";
        }

        public LabeledDataset<LblT, ExT1> Dataset { get; set; }
        public IModel<LblT, ExT2> Model { get; set; }
        public PerfData<LblT> PerfData { get; set; }

        public int NumFolds { get; set; }
        public bool IsStratified { get; set; }
        public string ExpName { get; set; }
        public string AlgName { get; set; }

        public Action<MappingCrossValidator<LblT, ExT1, ExT2>, int> BeforeFoldAction { get; set; }
        public Func<MappingCrossValidator<LblT, ExT1, ExT2>, int, ILabeledDataset<LblT, ExT1>, ILabeledDataset<LblT, ExT2>> TrainSetFunc { get; set; }
        public Func<MappingCrossValidator<LblT, ExT1, ExT2>, int, ILabeledDataset<LblT, ExT1>, ILabeledDataset<LblT, ExT2>> TestSetFunc { get; set; }
        public Action<MappingCrossValidator<LblT, ExT1, ExT2>, int, ILabeledDataset<LblT, ExT1>> AfterTrainAction { get; set; }
        public Action<MappingCrossValidator<LblT, ExT1, ExT2>, int, LabeledExample<LblT, ExT2>, Prediction<LblT>> AfterPredictAction { get; set; }
        public Action<MappingCrossValidator<LblT, ExT1, ExT2>, int, ILabeledDataset<LblT, ExT1>, List<Pair<LabeledExample<LblT, ExT2>, Prediction<LblT>>>> AfterFoldAction { get; set; }

        public void Run()
        {
            Preconditions.CheckArgumentRange(NumFolds >= 2 && NumFolds < Dataset.Count);
            Preconditions.CheckNotNullArgument(Model);
            Preconditions.CheckNotNullArgument(Dataset);

            if (IsStratified) { Dataset.GroupLabels(); } else { Dataset.Shuffle(new Random(1)); }

            PerfData = new PerfData<LblT>();
            for (int i = 0; i < NumFolds; i++)
            {
                int foldN = i + 1;
                LabeledDataset<LblT, ExT1> testSet;
                LabeledDataset<LblT, ExT1> trainSet;

                BeforeFold(foldN);

                if (IsStratified) { Dataset.SplitForStratifiedCrossValidation(NumFolds, foldN, out trainSet, out testSet); }
                else { Dataset.SplitForCrossValidation(NumFolds, foldN, out trainSet, out testSet); }

                ILabeledDataset<LblT, ExT2> mappedTrainSet = MapTrainSet(foldN, trainSet);
                Model.Train(mappedTrainSet);
                AfterTrain(foldN, trainSet);

                ILabeledDataset<LblT, ExT2> mappedTestSet = MapTestSet(foldN, testSet);
                PerfMatrix<LblT> foldMatrix = PerfData.GetPerfMatrix(ExpName, AlgName, foldN);
                var foldPredictions = new List<Pair<LabeledExample<LblT, ExT2>, Prediction<LblT>>>();
                foreach (LabeledExample<LblT, ExT2> le in mappedTestSet)
                {
                    Prediction<LblT> prediction = Model.Predict(le.Example);
                    foldMatrix.AddCount(le.Label, prediction.BestClassLabel);
                    foldPredictions.Add(new Pair<LabeledExample<LblT, ExT2>, Prediction<LblT>>(le, prediction));
                    AfterPredict(foldN, le, prediction);
                }
                AfterFold(foldN, trainSet, foldPredictions);
            }
        }

        protected virtual ILabeledDataset<LblT, ExT2> MapTrainSet(int foldN, ILabeledDataset<LblT, ExT1> trainSet)
        {
            return TrainSetFunc != null ? TrainSetFunc(this, foldN, trainSet) : (ILabeledDataset<LblT, ExT2>)trainSet;
        }

        protected virtual ILabeledDataset<LblT, ExT2> MapTestSet(int foldN, ILabeledDataset<LblT, ExT1> testSet)
        {
            return TestSetFunc != null ? TestSetFunc(this, foldN, testSet) : (ILabeledDataset<LblT, ExT2>)testSet;
        }

        protected virtual void AfterTrain(int foldN, ILabeledDataset<LblT, ExT1> trainSet)
        {
            if (AfterTrainAction != null)
            {
                AfterTrainAction(this, foldN, trainSet);
            }
        }

        protected virtual void AfterPredict(int foldN, LabeledExample<LblT, ExT2> labeledExample, Prediction<LblT> prediction)
        {
            if (AfterPredictAction != null)
            {
                AfterPredictAction(this, foldN, labeledExample, prediction);
            }
        }

        protected virtual void BeforeFold(int foldN)
        {
            if (BeforeFoldAction != null)
            {
                BeforeFoldAction(this, foldN);
            }
        }

        protected virtual void AfterFold(int foldN, ILabeledDataset<LblT, ExT1> trainSet, List<Pair<LabeledExample<LblT, ExT2>, Prediction<LblT>>> foldPredictions)
        {
            if (AfterFoldAction != null)
            {
                AfterFoldAction(this, foldN, trainSet, foldPredictions);
            }
        }
    }
}