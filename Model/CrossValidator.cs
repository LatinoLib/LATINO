using System;
using System.Linq;
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
            Models = new List<IModel<LblT, ExT2>>();

            NumFolds = 10;
            IsStratified = true;
            ExpName = "";
        }

        public LabeledDataset<LblT, ExT1> Dataset { get; set; }
        public List<IModel<LblT, ExT2>> Models { get; private set; }
        public PerfData<LblT> PerfData { get; set; }

        public int NumFolds { get; set; }
        public bool IsStratified { get; set; }
        public string ExpName { get; set; }
        public int Parallelism { get; set; }

        public Func<MappingCrossValidator<LblT, ExT1, ExT2>, int, ILabeledDataset<LblT, ExT1>, ILabeledDataset<LblT, ExT2>> TrainSetFunc { get; set; }
        public Func<MappingCrossValidator<LblT, ExT1, ExT2>, int, ILabeledDataset<LblT, ExT1>, ILabeledDataset<LblT, ExT2>> TestSetFunc { get; set; }
        public Action<MappingCrossValidator<LblT, ExT1, ExT2>, int, IModel<LblT, ExT2>, ILabeledDataset<LblT, ExT2>> AfterTrainAction { get; set; }
        public Action<MappingCrossValidator<LblT, ExT1, ExT2>, int, IModel<LblT, ExT2>, KeyValuePair<LabeledExample<LblT, ExT2>, Prediction<LblT>>> AfterPredictAction { get; set; }
        public Action<MappingCrossValidator<LblT, ExT1, ExT2>, int, ILabeledDataset<LblT, ExT1>, ILabeledDataset<LblT, ExT1>> BeforeFoldAction { get; set; }
        public Action<MappingCrossValidator<LblT, ExT1, ExT2>, int, ILabeledDataset<LblT, ExT1>, ILabeledDataset<LblT, ExT1>> AfterFoldAction { get; set; }

        public void Run()
        {
            Preconditions.CheckArgumentRange(NumFolds >= 2 && NumFolds < Dataset.Count);
            Preconditions.CheckArgument(Models.Any());
            Preconditions.CheckNotNullArgument(Dataset);

            if (IsStratified) { Dataset.GroupLabels(); } else { Dataset.Shuffle(new Random(1)); }

            PerfData = new PerfData<LblT>();
            for (int i = 0; i < NumFolds; i++)
            {
                int foldN = i + 1;

                // fold data
                LabeledDataset<LblT, ExT1> testSet, trainSet;
                if (IsStratified) { Dataset.SplitForStratifiedCrossValidation(NumFolds, foldN, out trainSet, out testSet); }
                else { Dataset.SplitForCrossValidation(NumFolds, foldN, out trainSet, out testSet); }

                BeforeFold(foldN, trainSet, testSet);

                // pefrorm mapping
                ILabeledDataset<LblT, ExT2> mappedTrainSet = MapTrainSet(foldN, trainSet);
                ILabeledDataset<LblT, ExT2> mappedTestSet = MapTestSet(foldN, testSet);

                // validate
                foreach (IModel<LblT, ExT2> model in Models)
                {
                    // train
                    model.Train(mappedTrainSet);
                    AfterTrain(foldN, model, mappedTrainSet);

                    // test
                    PerfMatrix<LblT> foldMatrix = PerfData.GetPerfMatrix(ExpName, GetModelName(model), foldN);
                    var foldPredictions = new List<Pair<LabeledExample<LblT, ExT2>, Prediction<LblT>>>();
                    foreach (LabeledExample<LblT, ExT2> le in mappedTestSet)
                    {
                        Prediction<LblT> prediction = model.Predict(le.Example);
                        foldMatrix.AddCount(le.Label, prediction.BestClassLabel);
                        foldPredictions.Add(new Pair<LabeledExample<LblT, ExT2>, Prediction<LblT>>(le, prediction));
                        AfterPredict(foldN, model, le, prediction);
                    }
                }

                AfterFold(foldN, trainSet, testSet);
            }
        }

        public string GetModelName(IModel<LblT, ExT2> model)
        {
            Preconditions.CheckNotNullArgument(model);
            return model.GetType().Name;
        }

        protected virtual ILabeledDataset<LblT, ExT2> MapTrainSet(int foldN, ILabeledDataset<LblT, ExT1> trainSet)
        {
            return TrainSetFunc != null ? TrainSetFunc(this, foldN, trainSet) : (ILabeledDataset<LblT, ExT2>)trainSet;
        }

        protected virtual ILabeledDataset<LblT, ExT2> MapTestSet(int foldN, ILabeledDataset<LblT, ExT1> testSet)
        {
            return TestSetFunc != null ? TestSetFunc(this, foldN, testSet) : (ILabeledDataset<LblT, ExT2>)testSet;
        }

        protected virtual void AfterTrain(int foldN, IModel<LblT, ExT2> model, ILabeledDataset<LblT, ExT2> trainSet)
        {
            if (AfterTrainAction != null)
            {
                AfterTrainAction(this, foldN, model, trainSet);
            }
        }

        protected virtual void AfterPredict(int foldN, IModel<LblT, ExT2> model, LabeledExample<LblT, ExT2> labeledExample, Prediction<LblT> prediction)
        {
            if (AfterPredictAction != null)
            {
                AfterPredictAction(this, foldN, model, new KeyValuePair<LabeledExample<LblT, ExT2>, Prediction<LblT>>(labeledExample, prediction));
            }
        }

        protected virtual void BeforeFold(int foldN, ILabeledDataset<LblT, ExT1> trainSet, ILabeledDataset<LblT, ExT1> testSet)
        {
            if (AfterFoldAction != null)
            {
                BeforeFoldAction(this, foldN, trainSet, testSet);
            }
        }

        protected virtual void AfterFold(int foldN, ILabeledDataset<LblT, ExT1> trainSet, ILabeledDataset<LblT, ExT1> testSet)
        {
            if (AfterFoldAction != null)
            {
                AfterFoldAction(this, foldN, trainSet, testSet);
            }
        }
    }
}