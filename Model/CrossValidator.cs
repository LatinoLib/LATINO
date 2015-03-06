using System;
using System.Collections.Concurrent;
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
        private List<Action> mFoldTasks;
        private List<Func<Action[]>> mFoldRetModelTasks;
        private ConcurrentDictionary<int, List<Action>> mFoldModelTasks;

        public TaskMappingCrossValidator()
        {
        }

        public TaskMappingCrossValidator(IEnumerable<Func<IModel<LblT, ModelExT>>> modelFuncs)
        {
            ModelFuncs = modelFuncs as List<Func<IModel<LblT, ModelExT>>> ?? modelFuncs.ToList();
        }

        public List<Func<IModel<LblT, ModelExT>>> ModelFuncs { get; protected set; }

        public override List<IModel<LblT, ModelExT>> Models
        {
            get
            {
                return ModelFuncs.Select(f => f()).ToList();
            }
        }
        
        public Action[] GetFoldTasks()
        {
            Preconditions.CheckState(mFoldTasks == null && mFoldRetModelTasks == null, "this instance can not be reused");

            mFoldTasks = new List<Action>();
            DoRun();
            return mFoldTasks.ToArray();
        }

        public Func<Action[]>[] GetFoldAndModelTasks()
        {
            Preconditions.CheckState(mFoldTasks == null && mFoldRetModelTasks == null, "this instance can not be reused");
            
            mFoldRetModelTasks = new List<Func<Action[]>>();
            mFoldModelTasks = new ConcurrentDictionary<int, List<Action>>();
            DoRun();
            return mFoldRetModelTasks.ToArray();
        }

        protected override void DoRunFold(int foldN)
        {
            if (mFoldTasks != null)
            {
                mFoldTasks.Add(() => RunFold(foldN));
            }
            else
            {
                if (!mFoldModelTasks.TryAdd(foldN, new List<Action>())) { throw new Exception(); }
                mFoldRetModelTasks.Add(() =>
                {
                    RunFold(foldN);
                    return mFoldModelTasks[foldN].ToArray();
                });
            }
        }

        protected override void DoRunModel(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> mappedTrainSet,
            LabeledDataset<LblT, InputExT> testSet, ILabeledDataset<LblT, ModelExT> mappedTestSet, CrossValidationTimeProfile modelProfile)
        {
            if (mFoldTasks != null)
            {
                RunModel(foldN, model, mappedTrainSet, testSet, mappedTestSet, modelProfile);
            }
            else
            {
                lock (mFoldModelTasks[foldN])
                {
                    mFoldModelTasks[foldN].Add(() => RunModel(foldN, model, mappedTrainSet, testSet, mappedTestSet, modelProfile));
                }
            }
        }

        protected override PerfMatrix<LblT> GetPerfMatrix(string algName, int foldN)
        {
            lock (mLock)
            {
                return PerfData.GetPerfMatrix(ExpName, algName, foldN);
            }
        }
    }

    public class MappingCrossValidator<LblT, InputExT, ModelExT> : AbstractMappingCrossValidator<LblT, InputExT, ModelExT>
    {
        private readonly List<IModel<LblT, ModelExT>> mModels;

        public MappingCrossValidator()
        {
            mModels = new List<IModel<LblT, ModelExT>>();
        }

        public MappingCrossValidator(IEnumerable<IModel<LblT, ModelExT>> models)
        {
            mModels = Preconditions.CheckNotNullArgument(models).ToList();
        }

        public override List<IModel<LblT, ModelExT>> Models { get { return mModels; } }

        public void Run()
        {
            DoRun();
        }

        protected override void DoRunFold(int foldN)
        {
            RunFold(foldN);
        }

        protected override void DoRunModel(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> mappedTrainSet, 
            LabeledDataset<LblT, InputExT> testSet, ILabeledDataset<LblT, ModelExT> mappedTestSet, CrossValidationTimeProfile modelProfile)
        {
            RunModel(foldN, model, mappedTrainSet, testSet, mappedTestSet, modelProfile);
        }
    }


    public abstract class AbstractMappingCrossValidator<LblT, InputExT, ModelExT>
    {
        protected readonly ConcurrentDictionary<int, ConcurrentDictionary<string, CrossValidationTimeProfile>> mFoldModelTimes =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, CrossValidationTimeProfile>>();

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

        public PerfData<LblT> PerfData { get; private set; }
        public abstract List<IModel<LblT, ModelExT>> Models { get; }


        public delegate ILabeledDataset<LblT, ModelExT> TrainSetFunc(
            AbstractMappingCrossValidator<LblT, InputExT, ModelExT> sender, int foldN, ILabeledDataset<LblT, InputExT> trainSet);
        public delegate ILabeledDataset<LblT, ModelExT> TestSetFunc(
            AbstractMappingCrossValidator<LblT, InputExT, ModelExT> sender, int foldN, ILabeledDataset<LblT, InputExT> testSet);
        public delegate void FoldPhaseHandler(
            AbstractMappingCrossValidator<LblT, InputExT, ModelExT> sender, int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> dataset);
        public delegate void AfterPredictHandler(
            AbstractMappingCrossValidator<LblT, InputExT, ModelExT> sender, int fondN, IModel<LblT, ModelExT> model, 
            InputExT example, LabeledExample<LblT, ModelExT> labeledExample, Prediction<LblT> prediction);
        public delegate void FoldHandler(
            AbstractMappingCrossValidator<LblT, InputExT, ModelExT> sender, int foldN, ILabeledDataset<LblT, InputExT> trainSet, ILabeledDataset<LblT, InputExT> testSet);

        public TrainSetFunc OnTrainSetMap { get; set; }
        public TestSetFunc OnTestSetMap { get; set; }
        public FoldPhaseHandler OnBeforeTrain { get; set; }
        public FoldPhaseHandler OnAfterTrain { get; set; }
        public FoldPhaseHandler OnBeforeTest { get; set; }
        public FoldPhaseHandler OnAfterTest { get; set; }
        public AfterPredictHandler OnAfterPrediction { get; set; }
        public FoldHandler OnBeforeFold { get; set; }
        public FoldHandler OnAfterFold { get; set; }
        public Func<AbstractMappingCrossValidator<LblT, InputExT, ModelExT>, IModel<LblT, ModelExT>, string> ModelNameFunc { get; set; }


        protected void DoRun()
        {
            Preconditions.CheckArgumentRange(NumFolds >= 2 && NumFolds < Dataset.Count);
            Preconditions.CheckArgument(Models.Any());
            Preconditions.CheckNotNullArgument(Dataset);

            if (IsStratified) { Dataset.GroupLabels(IsShuffleStratified); } else { Dataset.Shuffle(new Random(1)); }

            PerfData = new PerfData<LblT>();
            mFoldModelTimes.Clear();
            for (int i = 0; i < NumFolds; i++)
            {
                DoRunFold(i + 1);
            }
        }

        protected abstract void DoRunFold(int foldN);

        protected abstract void DoRunModel(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> mappedTrainSet,
            LabeledDataset<LblT, InputExT> testSet, ILabeledDataset<LblT, ModelExT> mappedTestSet, CrossValidationTimeProfile modelProfile);


        protected void RunFold(int foldN)
        {
            var foldProfile = new CrossValidationTimeProfile { FoldN = foldN, FoldStartTime = DateTime.Now };
            var foldProfiles = new ConcurrentDictionary<string, CrossValidationTimeProfile>();
            foldProfiles.TryAdd("", foldProfile);
            mFoldModelTimes.TryAdd(foldN, foldProfiles);

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
                string modelName = GetModelName(model);
                var modelProfile = new CrossValidationTimeProfile
                    {
                        FoldN = foldN, 
                        ModelName = modelName, 
                        FoldStartTime = foldProfile.FoldStartTime,
                        TrainStartTime = DateTime.Now
                    };
                foldProfiles.TryAdd(modelName, modelProfile);

                DoRunModel(foldN, model, mappedTrainSet, testSet, mappedTestSet, modelProfile);
            }

            foldProfile.FoldEndTime = DateTime.Now;
            AfterFold(foldN, trainSet, testSet);
        }


        protected void RunModel(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> mappedTrainSet, 
            LabeledDataset<LblT, InputExT> testSet, ILabeledDataset<LblT, ModelExT> mappedTestSet, CrossValidationTimeProfile modelProfile)
        {
            // train
            BeforeTrain(foldN, model, mappedTrainSet);
            model.Train(mappedTrainSet);
            AfterTrain(foldN, model, mappedTrainSet);
            modelProfile.TrainEndTime = DateTime.Now;

            // test
            modelProfile.TestStartTime = DateTime.Now;
            BeforeTest(foldN, model, mappedTestSet);
            PerfMatrix<LblT> foldMatrix = GetPerfMatrix(GetModelName(model), foldN);
            var foldPredictions = new List<Pair<LabeledExample<LblT, ModelExT>, Prediction<LblT>>>();
            for (int i = 0; i < mappedTestSet.Count; i++)
            {
                LabeledExample<LblT, ModelExT> le = mappedTestSet[i];

                Prediction<LblT> prediction = model.Predict(le.Example);
                if (prediction.Any())
                {
                    foldMatrix.AddCount(le.Label, prediction.BestClassLabel);
                }
                foldPredictions.Add(new Pair<LabeledExample<LblT, ModelExT>, Prediction<LblT>>(le, prediction));
                AfterPrediction(foldN, model, testSet[i].Example, le, prediction);
            }

            modelProfile.TestEndTime = DateTime.Now;
            AfterTest(foldN, model, mappedTestSet);
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
        
        public virtual string GetModelName(IModel<LblT, ModelExT> model)
        {
            Preconditions.CheckNotNullArgument(model);
            return ModelNameFunc == null ? model.GetType().Name : ModelNameFunc(this, model);
        }

        public CrossValidationTimeProfile[] GetFoldTimeProfile(int foldN)
        {
            ConcurrentDictionary<string, CrossValidationTimeProfile> modelTimes;
            return mFoldModelTimes.TryGetValue(foldN, out modelTimes) ? modelTimes.Values.ToArray() : null;
        }

        public CrossValidationTimeProfile GetFoldTimeProfile(int foldN, int modelN)
        {
            Preconditions.CheckArgumentRange(modelN >= 0 && modelN < Models.Count);
            return GetFoldModelTimeProfile(foldN, Models[modelN]);
            
        }
        public CrossValidationTimeProfile GetFoldModelTimeProfile(int foldN, IModel<LblT, ModelExT> model)
        {
            ConcurrentDictionary<string, CrossValidationTimeProfile> modelTimes;
            if (mFoldModelTimes.TryGetValue(foldN, out modelTimes))
            {
                CrossValidationTimeProfile profile;
                return modelTimes.TryGetValue(GetModelName(model), out profile) ? profile : null;
            }
            return null;
        }

        public Dictionary<int, Dictionary<string, CrossValidationTimeProfile>> GetTimeProfile()
        {
            return mFoldModelTimes.ToDictionary(kv => kv.Key, kv => kv.Value.ToDictionary(kvv => kvv.Key, kvv => kvv.Value));
        }

        protected virtual ILabeledDataset<LblT, ModelExT> MapTrainSet(int foldN, ILabeledDataset<LblT, InputExT> trainSet)
        {
            return OnTrainSetMap != null ? OnTrainSetMap(this, foldN, trainSet) : (ILabeledDataset<LblT, ModelExT>)trainSet;
        }

        protected virtual ILabeledDataset<LblT, ModelExT> MapTestSet(int foldN, ILabeledDataset<LblT, InputExT> testSet)
        {
            return OnTestSetMap != null ? OnTestSetMap(this, foldN, testSet) : (ILabeledDataset<LblT, ModelExT>)testSet;
        }

        protected virtual void BeforeTrain(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> trainSet)
        {
            if (OnBeforeTrain != null)
            {
                OnBeforeTrain(this, foldN, model, trainSet);
            }
        }

        protected virtual void AfterTrain(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> trainSet)
        {
            if (OnAfterTrain != null)
            {
                OnAfterTrain(this, foldN, model, trainSet);
            }
        }

        protected virtual void BeforeTest(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> testSet)
        {
            if (OnBeforeTest != null)
            {
                OnBeforeTest(this, foldN, model, testSet);
            }
        }

        protected virtual void AfterTest(int foldN, IModel<LblT, ModelExT> model, ILabeledDataset<LblT, ModelExT> testSet)
        {
            if (OnAfterTest != null)
            {
                OnAfterTest(this, foldN, model, testSet);
            }
        }

        protected virtual void AfterPrediction(int foldN, IModel<LblT, ModelExT> model, InputExT example, LabeledExample<LblT, ModelExT> labeledExample, Prediction<LblT> prediction)
        {
            if (OnAfterPrediction != null)
            {
                OnAfterPrediction(this, foldN, model, example, labeledExample, prediction);
            }
        }

        protected virtual void BeforeFold(int foldN, ILabeledDataset<LblT, InputExT> trainSet, ILabeledDataset<LblT, InputExT> testSet)
        {
            if (OnBeforeFold != null)
            {
                OnBeforeFold(this, foldN, trainSet, testSet);
            }
        }

        protected virtual void AfterFold(int foldN, ILabeledDataset<LblT, InputExT> trainSet, ILabeledDataset<LblT, InputExT> testSet)
        {
            if (OnAfterFold != null)
            {
                OnAfterFold(this, foldN, trainSet, testSet);
            }
        }
    }

    public class CrossValidationTimeProfile
    {
        public int FoldN { get; set; }
        public string ModelName { get; set; }

        public DateTime? FoldStartTime { get; set; }
        public DateTime? FoldEndTime { get; set; }
        public TimeSpan? FoldDuration
        {
            get { return FoldEndTime == null || FoldStartTime == null ? default(TimeSpan?) : FoldEndTime - FoldStartTime; }
        }

        public DateTime? TrainStartTime { get; set; }
        public DateTime? TrainEndTime { get; set; }
        public TimeSpan? TrainDuration
        {
            get { return TrainEndTime == null || TestStartTime == null ? default(TimeSpan?) : TrainEndTime - TrainStartTime; }
        }

        public DateTime? TestStartTime { get; set; }
        public DateTime? TestEndTime { get; set; }
        public TimeSpan? TestDuration
        {
            get { return TestEndTime == null || TestStartTime == null ? default(TimeSpan?) : TestEndTime - TestStartTime; }
        }
    }
}