/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    BinarySvm.cs
 *  Desc:    Tutorial 5.1: Supervised machine learning
 *  Created: Apr-2010
 *
 *  Authors: 
 *
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Latino;
using Latino.Model;
using Latino.Model.Eval;
using Tutorial.Case.Model;
using Action = System.Action;

namespace Tutorial.Case.Validation
{
    public class NFoldParallel : Tutorial<NFoldParallel>
    {
        public override void Run(object[] args)
        {
            // get labeled data
            BinarySvm classifierInst = BinarySvm.RunInstanceNull(args);
            var labeledData = (LabeledDataset<string, SparseVector<double>>)classifierInst.Result["labeled_data"];

            // convert dataset to binary vector
            var ds = (LabeledDataset<string, BinaryVector>)labeledData.ConvertDataset(typeof(BinaryVector), false);

            // cross validation with task validator
            var validator = new TaskCrossValidator<string, BinaryVector>(new System.Func<IModel<string, BinaryVector>>[]
                {
                    // model instances are constructed on the fly
                    () => new NaiveBayesClassifier<string>()
                })
            {
                NumFolds = 10, // default
                IsStratified = true, // default
                ExpName = "", // default

                Dataset = ds,
                OnAfterTrain = (sender, foldN, model, trainSet) =>
                {
                    var m = (NaiveBayesClassifier<string>)model;
                    // do stuff after model is trained for a fold...
                },
                OnAfterPrediction = (sender, foldN, model, le, prediction) =>
                {
                    lock (Output) Output.WriteLine("actual: {0} \tpredicted: {1}\t score: {2:0.0000}", le.Label, prediction.BestClassLabel, prediction.BestScore);
                }
            };


            var cores = (int)(Math.Round(Environment.ProcessorCount * 0.9) - 1); // use 90% of cpu cores
            Output.WriteLine("Multi-threaded using {0} cores\n", cores);
            Output.Flush();


            // using .net framework
            
            // model level parallelization
            Parallel.ForEach(
                validator.GetFoldAndModelTasks(),
                new ParallelOptions { MaxDegreeOfParallelism = cores },
                foldTask => Parallel.ForEach(
                    foldTask(),
                    new ParallelOptions { MaxDegreeOfParallelism = cores },
                    modelTask => modelTask()
                )
            );

            // fold level 
/*
            Parallel.ForEach(validator.GetFoldTasks(), new ParallelOptions { MaxDegreeOfParallelism = cores }, t => t());
*/



            // for some serious workload better use SmartThreadPool
            // requires reference to package https://www.nuget.org/packages/SmartThreadPool.dll/

            var exceptions = new List<Exception>();

            // model level parallelization
/*
            var threadPool = new SmartThreadPool { MaxThreads = cores };
            foreach (System.Func<Action[]> foldTask in validator.GetFoldAndModelTasks())
            {
                System.Func<Action[]> ft = foldTask;
                threadPool.QueueWorkItem(o =>
                {
                    foreach (Action modelTask in ft())
                    {
                        Action mt = modelTask;
                        threadPool.QueueWorkItem(p =>
                        {
                            mt();
                            return null;
                        }, null, wi => { if (wi.Exception != null) { exceptions.Add((Exception)wi.Exception); } });
                    }
                    return null;
                }, null, wi => { if (wi.Exception != null) { exceptions.Add((Exception)wi.Exception); } });
            }
            threadPool.WaitForIdle();
            threadPool.Shutdown();
*/

            // fold level
/*
            var threadPool = new SmartThreadPool { MaxThreads = cores };
            foreach (Action foldTask in validator.GetFoldTasks())
            {
                Action ft = foldTask;
                threadPool.QueueWorkItem(o =>
                {
                    ft();
                    return null;
                }, null, wi => { if (wi.Exception != null) { exceptions.Add((Exception)wi.Exception); } });
            }
            threadPool.WaitForIdle();
            threadPool.Shutdown();
*/

            foreach (Exception exception in exceptions)
            {
                throw new Exception("Error during validation", exception);
            }



            Output.WriteLine("Sum confusion matrix:");
            PerfMatrix<string> sumPerfMatrix = validator.PerfData.GetSumPerfMatrix("", validator.GetModelName(0));
            Output.WriteLine(sumPerfMatrix.ToString());
            Output.WriteLine("Average accuracy: {0:0.00}", sumPerfMatrix.GetAccuracy());
            foreach (string label in validator.PerfData.GetLabels("", validator.GetModelName(0)))
            {
                double stdDev;
                Output.WriteLine("Precision for '{0}': {1:0.00} std. dev: {2:0.00}", label,
                    validator.PerfData.GetAvg("", validator.GetModelName(0), ClassPerfMetric.Precision, label, out stdDev), stdDev);
            }
        }
    }
}