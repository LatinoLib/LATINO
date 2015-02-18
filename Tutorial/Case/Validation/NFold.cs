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
using System.Linq;
using Latino;
using Latino.Model;
using Latino.Model.Eval;
using Tutorial.Case.Model;

namespace Tutorial.Case.Validation
{
    public class NFold : Tutorial<NFold>
    {
        public override void Run(object[] args)
        {
            int foldCount = args.Any() ? (int)args[0] : 10;
            args = args.Skip(1).ToArray();

            // get classifier and labeled data
            BinarySvm classifierInst = BinarySvm.RunInstanceNull(args);
            var classifier = (SvmBinaryClassifier<string>)classifierInst.Result["classifier"];
            var labeledData = (LabeledDataset<string, SparseVector<double>>)classifierInst.Result["labeled_data"];

            bool stratified = true;

            // cross validation
            if (stratified)
            {
                labeledData.GroupLabels();
            } else
            {
                labeledData.Shuffle(new Random(1));
            }

            var perfData = new PerfData<string>();
            foreach (var g in labeledData.GroupBy(le => le.Label))
            {
                Output.WriteLine("total {0} {1}\t {2:0.00}", g.Key, g.Count(), (double)g.Count() / labeledData.Count);
            }

            Output.WriteLine("Performing {0}{1}-fold cross validation...", stratified ? "stratified " : "", foldCount);
            for (int i = 0; i < foldCount; i++)
            {
                int foldN = i + 1;
                LabeledDataset<string, SparseVector<double>> testSet;
                LabeledDataset<string, SparseVector<double>> trainSet;
                
                if (stratified)
                {
                    labeledData.SplitForStratifiedCrossValidation(foldCount, foldN, out trainSet, out testSet);
                }
                else
                {
                    labeledData.SplitForCrossValidation(foldCount, foldN, out trainSet, out testSet);
                }

                classifier.Train(trainSet);

                PerfMatrix<string> foldMatrix = perfData.GetPerfMatrix("tutorial", "binary svm", foldN);
                foreach (LabeledExample<string, SparseVector<double>> labeledExample in testSet)
                {
                    Prediction<string> prediction = classifier.Predict(labeledExample.Example);
                    foldMatrix.AddCount(labeledExample.Label, prediction.BestClassLabel);
                }
                Output.WriteLine("Accuracy for {0}-fold: {1:0.00}", foldN, foldMatrix.GetAccuracy());
            }

            Output.WriteLine("Sum confusion matrix:");
            PerfMatrix<string> sumPerfMatrix = perfData.GetSumPerfMatrix("tutorial", "binary svm");
            Output.WriteLine(sumPerfMatrix.ToString());
            Output.WriteLine("Average accuracy: {0:0.00}", sumPerfMatrix.GetAccuracy());
            foreach (string label in perfData.GetLabels("tutorial", "binary svm"))
            {
                double stdDev;
                Output.WriteLine("Precision for '{0}': {1:0.00} std. dev: {2:0.00}", label, 
                    perfData.GetAvg("tutorial", "binary svm", PerfMetric.Precision, label, out stdDev), stdDev);
            }
        }
    }
}