using System;
using System.Collections.Generic;

namespace Latino.Model.Eval
{
    public class CrossValidation<LblT, ExT>
    {
        public CrossValidation()
        {
            NumFolds = 10;
            IsStratified = true;
            ExpName = "";
            AlgName = "";
        }

        public LabeledDataset<LblT, ExT> Dataset { get; set; }
        public IModel<LblT> Model { get; set; }
        public PerfData<LblT> PerfData { get; set; }

        public int NumFolds { get; set; }
        public bool IsStratified { get; set; }
        public string ExpName { get; set; }
        public string AlgName { get; set; }

        public Action<CrossValidation<LblT, ExT>, int> BeforeFoldAction { get; set; }
        public Action<CrossValidation<LblT, ExT>, int, LabeledDataset<LblT, ExT>> AfterTrainAction { get; set; }
        public Action<CrossValidation<LblT, ExT>, int, LabeledExample<LblT, ExT>, Prediction<LblT>> AfterPredictAction { get; set; }
        public Action<CrossValidation<LblT, ExT>, int, LabeledDataset<LblT, ExT>, List<Pair<LabeledExample<LblT, ExT>, Prediction<LblT>>>> AfterFoldAction { get; set; }

        public void Perform()
        {
            Preconditions.CheckArgumentRange(NumFolds >= 2 && NumFolds < Dataset.Count);
            Preconditions.CheckNotNullArgument(Model);
            Preconditions.CheckNotNullArgument(Dataset);

            if (IsStratified) { Dataset.GroupLabels(); } else { Dataset.Shuffle(new Random(1)); }

            PerfData = new PerfData<LblT>();
            for (int i = 0; i < NumFolds; i++)
            {
                int foldN = i + 1;
                LabeledDataset<LblT, ExT> testSet;
                LabeledDataset<LblT, ExT> trainSet;

                BeforeFold(foldN);

                if (IsStratified) { Dataset.SplitForStratifiedCrossValidation(NumFolds, foldN, out trainSet, out testSet); }
                else { Dataset.SplitForCrossValidation(NumFolds, foldN, out trainSet, out testSet); }

                Model.Train(trainSet);
                AfterTrain(foldN, trainSet);

                PerfMatrix<LblT> foldMatrix = PerfData.GetPerfMatrix(ExpName, AlgName, foldN);
                var foldPredictions = new List<Pair<LabeledExample<LblT, ExT>, Prediction<LblT>>>();
                foreach (LabeledExample<LblT, ExT> labeled in testSet)
                {
                    Prediction<LblT> prediction = Model.Predict(labeled.Example);
                    foldMatrix.AddCount(labeled.Label, prediction.BestClassLabel);
                    foldPredictions.Add(new Pair<LabeledExample<LblT, ExT>, Prediction<LblT>>(labeled, prediction));
                    AfterPredict(foldN, labeled, prediction);
                }
                AfterFold(foldN, trainSet, foldPredictions);
            }
        }

        protected virtual void AfterTrain(int foldN, LabeledDataset<LblT, ExT> trainSet)
        {
            if (AfterTrainAction != null)
            {
                AfterTrainAction(this, foldN, trainSet);
            }
        }

        protected virtual void AfterPredict(int foldN, LabeledExample<LblT, ExT> labeledExample, Prediction<LblT> prediction)
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

        protected virtual void AfterFold(int foldN, LabeledDataset<LblT, ExT> trainSet, List<Pair<LabeledExample<LblT, ExT>, Prediction<LblT>>> foldPredictions)
        {
            if (AfterFoldAction != null)
            {
                AfterFoldAction(this, foldN, trainSet, foldPredictions);
            }
        }
    }
}