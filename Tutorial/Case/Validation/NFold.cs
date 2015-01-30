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
    public class NFold : Tutorial<NFold>
    {
        public override void Run(string[] args)
        {
            // get classifier and labeled data
            BinarySvm classifierInst = BinarySvm.RunInstance(args);
            var classifier = (SvmBinaryClassifier<string>)classifierInst.Result["classifier"];
            var labeledData = (LabeledDataset<string, SparseVector<double>>)classifierInst.Result["labeled_data"];

            // validation
            labeledData.Shuffle();
            var perfData = new PerfData<string>();

            const int foldCount = 10;
            Console.WriteLine("Performing {0}-fold cross validation...", foldCount);
            for (int i = 0; i < foldCount; i++)
            {
                int foldN = i + 1;
                LabeledDataset<string, SparseVector<double>> testSet;
                LabeledDataset<string, SparseVector<double>> trainSet;
                labeledData.SplitForCrossValidation(foldCount, foldN, out trainSet, out testSet);

                int correctCount = 0;
                PerfMatrix<string> foldMatrix = perfData.GetPerfMatrix("tutorial", "binary svm", foldN);
                foreach (LabeledExample<string, SparseVector<double>> labeledExample in testSet)
                {
                    var prediction = classifier.Predict(labeledExample.Example);
                    foldMatrix.AddCount(labeledExample.Label, prediction.BestClassLabel);
                    if (prediction.BestClassLabel == labeledExample.Label) { correctCount++; }
                }
                Console.WriteLine("Accuracy for {0}-fold: {1}", i, foldMatrix.GetMicroAverage());
                Console.WriteLine("accuracy {0}", (double)correctCount / testSet.Count);
            }

            Console.WriteLine(perfData.GetSumPerfMatrix("tutorial", "binary svm").ToString());
        }
    }
}