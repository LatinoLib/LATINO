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
using Latino;
using Latino.Model;
using Latino.Model.Eval;
using Tutorial.Case.Model;

namespace Tutorial.Case.Validation
{
    public class NFoldClass : Tutorial<NFoldClass>
    {
        public override void Run(object[] args)
        {
            // get labeled data
            BinarySvm classifierInst = BinarySvm.RunInstanceNull(args);
            var labeledData = (LabeledDataset<string, SparseVector<double>>)classifierInst.Result["labeled_data"];

            // convert dataset to binary vector
            var ds = (LabeledDataset<string, BinaryVector>)labeledData.ConvertDataset(typeof(BinaryVector), false);

            // cross validation ...with the convenience class
            var validation = new CrossValidator<string, BinaryVector>
            {
                NumFolds = 10, // default
                IsStratified = true, // default

                Model = new NaiveBayesClassifier<string>(),
                Dataset = ds,
                AfterTrainAction = (sender, foldN, trainSet) =>
                {
                    var model = (NaiveBayesClassifier<string>)sender.Model;
                    // do stuff after model is trained for a fold...
                },
                AfterPredictAction = (sender, foldN, labeledExample, prediction) =>
                    Output.WriteLine("actual: {0} \tpredicted: {1}\t score: {2:0.0000}", labeledExample.Label, prediction.BestClassLabel, prediction.BestScore),
                AfterFoldAction = (sender, foldN, trainSet, foldPredictions) =>
                {
                    PerfMatrix<string> foldMatrix = sender.PerfData.GetPerfMatrix(sender.ExpName, sender.AlgName, foldN);
                    Output.WriteLine("Accuracy for {0}-fold: {1:0.00}", foldN, foldMatrix.GetMicroAverage());
                }
            };
            validation.Perform();

            Output.WriteLine("Sum confusion matrix:");
            PerfMatrix<string> sumPerfMatrix = validation.PerfData.GetSumPerfMatrix("", "");
            Output.WriteLine(sumPerfMatrix.ToString());
            Output.WriteLine("Average accuracy: {0:0.00}", sumPerfMatrix.GetMicroAverage());
            foreach (string label in validation.PerfData.GetLabels("", ""))
            {
                double stdDev;
                Output.WriteLine("Accuracy for '{0}': {1:0.00} std. dev: {2:0.00}", label,
                    validation.PerfData.GetAvg("", "", PerfMetric.MicroAccuracy, label, out stdDev), stdDev);
            }
        }
    }
}