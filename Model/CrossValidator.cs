﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace Latino.Model.Eval
{
    public class CrossValidator<LblT, ExT> : MappingCrossValidator<LblT, ExT, ExT>
    {
        public CrossValidator()
        {
        }

        public CrossValidator(IEnumerable<IModel<LblT, ExT>> models) : base(models)
        {
        }
    }

    public class TaskCrossValidator<LblT, ExT> : TaskMappingCrossValidator<LblT, ExT, ExT>
    {
        public TaskCrossValidator()
        {
        }

        public TaskCrossValidator(IEnumerable<Func<IModel<LblT, ExT>>> modelFuncs) : base(modelFuncs)
        {
        }
    }

    public class TaskMappingCrossValidator<LblT, InputExT, ModelExT> : AbstractMappingCrossValidator<LblT, InputExT, ModelExT>
    {
        private readonly object mLock = new object();

        public TaskMappingCrossValidator()
        {
        }

        public TaskMappingCrossValidator(IEnumerable<Func<IModel<LblT, ModelExT>>> modelFuncs)
        {
            ModelFuncs = modelFuncs.ToList();
        }

        public List<Func<IModel<LblT, ModelExT>>> ModelFuncs { get; protected set; }

        public override List<IModel<LblT, ModelExT>> Models
        {
            get
            {
                return ModelFuncs.Select(f => f()).ToList();
            }
        }
        
        public IEnumerable<Action> RunGetFoldTasks()
        {
            PrepareRun();

            var tasks = new List<Action>();
            for (int i = 0; i < NumFolds; i++)
            {
                int li = i;
                tasks.Add(() => RunFold(li + 1));
            }
            return tasks;
        }

        protected override PerfMatrix<LblT> GetPerfMatrix(string algName, int foldN)
        {
            lock (mLock)
            {
                return PerfData.GetPerfMatrix(ExpName, algName, foldN);
            }
        }
    }

    public class MappingCrossValidator<LblT, ExT1, ExT2> : AbstractMappingCrossValidator<LblT, ExT1, ExT2>
    {
        private readonly List<IModel<LblT, ExT2>> mModels;

        public MappingCrossValidator()
        {
            mModels = new List<IModel<LblT, ExT2>>();
        }

        public MappingCrossValidator(IEnumerable<IModel<LblT, ExT2>> models)
        {
            mModels = Preconditions.CheckNotNullArgument(models).ToList();
        }

        public override List<IModel<LblT, ExT2>> Models { get { return mModels; } }

        public void Run()
        {
            PrepareRun();
            for (int i = 0; i < NumFolds; i++)
            {
                RunFold(i + 1);
            }
        }
    }


    public abstract class AbstractMappingCrossValidator<LblT, InputExT, ModelExT>
    {
        protected AbstractMappingCrossValidator()
        {

            NumFolds = 10;
            IsStratified = true;
            IsShuffleStratified = false;
            ExpName = "";
        }

        public LabeledDataset<LblT, InputExT> Dataset { get; set; }

        public int NumFolds { get; set; }
        public bool IsStratified { get; set; }
        public bool IsShuffleStratified { get; set; }
        public string ExpName { get; set; }
        public int Parallelism { get; set; }

        public PerfData<LblT> PerfData { get; private set; }
        public abstract List<IModel<LblT, ModelExT>> Models { get; }

        public delegate void AfterPredictAction(AbstractMappingCrossValidator<LblT, InputExT, ModelExT> sender, int fondN,
            IModel<LblT, ModelExT> model, LabeledExample<LblT, ModelExT> labeledExample, Prediction<LblT> prediction);

        public Func<AbstractMappingCrossValidator<LblT, InputExT, ModelExT>, int, ILabeledDataset<LblT, InputExT>, ILabeledDataset<LblT, ModelExT>> TrainSetMap { get; set; }
        public Func<AbstractMappingCrossValidator<LblT, InputExT, ModelExT>, int, ILabeledDataset<LblT, InputExT>, ILabeledDataset<LblT, ModelExT>> TestSetMap { get; set; }
        public Action<AbstractMappingCrossValidator<LblT, InputExT, ModelExT>, int, IModel<LblT, ModelExT>, ILabeledDataset<LblT, ModelExT>> BeforeTrainEventHandler { get; set; }
        public Action<AbstractMappingCrossValidator<LblT, InputExT, ModelExT>, int, IModel<LblT, ModelExT>, ILabeledDataset<LblT, ModelExT>> AfterTrainEventHandler { get; set; }
        public AfterPredictAction AfterPredictEventHandler { get; set; }
        public Action<AbstractMappingCrossValidator<LblT, InputExT, ModelExT>, int, ILabeledDataset<LblT, InputExT>, ILabeledDataset<LblT, InputExT>> BeforeFoldEventHandler { get; set; }
        public Action<AbstractMappingCrossValidator<LblT, InputExT, ModelExT>, int, ILabeledDataset<LblT, InputExT>, ILabeledDataset<LblT, InputExT>> AfterFoldEventHandler { get; set; }

        protected virtual void PrepareRun()
        {
            Preconditions.CheckArgumentRange(NumFolds >= 2 && NumFolds < Dataset.Count);
            Preconditions.CheckArgument(Models.Any());
            Preconditions.CheckNotNullArgument(Dataset);

            if (IsStratified) { Dataset.GroupLabels(IsShuffleStratified); } else { Dataset.Shuffle(new Random(1)); }

            PerfData = new PerfData<LblT>();
        }

        protected void RunFold(int foldN)
        {
            // fold data
            LabeledDataset<LblT, InputExT> testSet, trainSet;
            if (IsStratified)
            {
                Dataset.SplitForStratifiedCrossValidation(NumFolds, foldN, out trainSet, out testSet);
            }
            else
            {
                Dataset.SplitForCrossValidation(NumFolds, foldN, out trainSet, out testSet);
            }

            BeforeFold(foldN, trainSet, testSet);

            // pefrorm mapping
            ILabeledDataset<LblT, ModelExT> mappedTrainSet = MapTrainSet(foldN, trainSet);
            ILabeledDataset<LblT, ModelExT> mappedTestSet = MapTestSet(foldN, testSet);

            // validate
            foreach (IModel<LblT, ModelExT> model in Models)
            {
                // train
                BeforeTrain(foldN, model, mappedTrainSet);
                model.Train(mappedTrainSet);
                AfterTrain(foldN, model, mappedTrainSet);

                // test
                PerfMatrix<LblT> foldMatrix = GetPerfMatrix(GetModelName(model), foldN);
                var foldPredictions = new List<Pair<LabeledExample<LblT, ModelExT>, Prediction<LblT>>>();
                foreach (LabeledExample<LblT, ModelExT> le in mappedTestSet)
                {
                    Prediction<LblT> prediction = model.Predict(le.Example);
                    if (prediction.Any())
                    {
                        foldMatrix.AddCount(le.Label, prediction.BestClassLabel);
                    }
                    foldPredictions.Add(new Pair<LabeledExample<LblT, ModelExT>, Prediction<LblT>>(le, prediction));
                    AfterPredict(foldN, model, le, prediction);
                }
            }

            AfterFold(foldN, trainSet, testSet);
        }

        protected virtual PerfMatrix<LblT> GetPerfMatrix(string algName, int foldN)
        {
            return PerfData.GetPerfMatrix(ExpName, algName, foldN);
        }

        public string GetModelName(int modelN)
        {
            Preconditions.CheckArgumentRange(modelN >= 0 && modelN < Models.Count);
            return GetModelName(Models[modelN]);
        }
        
        public string GetModelName(IModel<LblT, ModelExT> model)
        {
            Preconditions.CheckNotNullArgument(model);
            return model.GetType().Name;
        }

        protected virtual ILabeledDataset<LblT, ModelExT> MapTrainSet(int foldN, ILabeledDataset<LblT, InputExT> trainSet)
        {
            return TrainSetMap != null ? TrainSetMap(this, foldN, trainSet) : (ILabeledDataset<LblT, ModelExT>)trainSet;
        }

        protected virtual ILabeledDataset<LblT, ModelExT> MapTestSet(int foldN, ILabeledDataset<LblT, InputExT> testSet)
        {
            return TestSetMap != null ? TestSetMap(this, foldN, testSet) : (ILabeledDataset<LblT, ModelExT>)testSet;
        }

        protected virtual void BeforeTrain(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> trainSet)
        {
            if (BeforeTrainEventHandler != null)
            {
                BeforeTrainEventHandler(this, foldN, model, trainSet);
            }
        }

        protected virtual void AfterTrain(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> trainSet)
        {
            if (AfterTrainEventHandler != null)
            {
                AfterTrainEventHandler(this, foldN, model, trainSet);
            }
        }

        protected virtual void AfterPredict(int foldN, IModel<LblT, ModelExT> model, LabeledExample<LblT, ModelExT> labeledExample, Prediction<LblT> prediction)
        {
            if (AfterPredictEventHandler != null)
            {
                AfterPredictEventHandler(this, foldN, model, labeledExample, prediction);
            }
        }

        protected virtual void BeforeFold(int foldN, ILabeledDataset<LblT, InputExT> trainSet, ILabeledDataset<LblT, InputExT> testSet)
        {
            if (BeforeFoldEventHandler != null)
            {
                BeforeFoldEventHandler(this, foldN, trainSet, testSet);
            }
        }

        protected virtual void AfterFold(int foldN, ILabeledDataset<LblT, InputExT> trainSet, ILabeledDataset<LblT, InputExT> testSet)
        {
            if (AfterFoldEventHandler != null)
            {
                AfterFoldEventHandler(this, foldN, trainSet, testSet);
            }
        }
    }
}