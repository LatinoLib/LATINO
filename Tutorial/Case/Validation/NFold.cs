/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Svm.cs
 *  Desc:    Tutorial 5.1: Supervised machine learning
 *  Created: Apr-2010
 *
 *  Authors: 
 *
 **********************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Latino;
using Latino.Model;
using Latino.Model.Eval;
using Latino.TextMining;
using Microsoft.VisualBasic.FileIO;

namespace Tutorial.Case.Validation
{
    public class NFold : Tutorial<NFold>
    {
        public override void Run(string[] args)
        {
            // prepare data
            IStemmer stemmer;
            Set<string>.ReadOnly stopWords;
            TextMiningUtils.GetLanguageTools(Language.English, out stopWords, out stemmer);

            // Create a tokenizer.
            var tokenizer = new UnicodeTokenizer
                {
                    MinTokenLen = 2,                        // Each token must be at least 2 characters long.
                    Filter = TokenizerFilter.AlphaStrict    // Tokens can consist of alphabetic characters only.
                };

            // take data for two classes from cvs file
            var data = new List<LabeledTweet>(GetLabeledTweets().Where(lt => lt.Polarity != 2)).ToList();
            data = data.OrderBy(a => Guid.NewGuid()).ToList();

            // Create a bag-of-words space.
            var bowSpc = new BowSpace
                {
                    Tokenizer = tokenizer,                  // Assign the tokenizer.
                    StopWords = stopWords,                  // Assign the stop words.
                    Stemmer = stemmer,                      // Assign the stemmer.
                    MinWordFreq = 1,                        // A term must appear at least n-times in the corpus for it to be part of the vocabulary.
                    MaxNGramLen = 2,                        // Terms consisting of at most n-consecutive words will be considered.
                    WordWeightType = WordWeightType.TermFreq,  // Set the weighting scheme for the bag-of-words vectors to TF.
//                    WordWeightType = WordWeightType.TfIdf,  // Set the weighting scheme for the bag-of-words vectors to TF-IDF.
                    NormalizeVectors = true,                // The TF-IDF vectors will be normalized.
                    CutLowWeightsPerc = 0                   // The terms with the lowest weights, summing up to 20% of the overall weight sum, will be removed from each TF-IDF vector.
                };
            ArrayList<SparseVector<double>> bowData = bowSpc.Initialize(data.Select(d => d.Text));

            // label training set
            var trainingSet = new LabeledDataset<string, SparseVector<double>>();
            for (int i = 0; i < data.Count * 3 / 4; i++)
            {
                trainingSet.Add(data[i].Polarity == 0 ? "Negative" : "Positive", bowData[i]);
            }

            //-------------------- SVM

            var svmBinClass = new SvmBinaryClassifier<string>();
            //svmBinClass.BiasedHyperplane = false;
            //svmBinClass.CustomParams = "-t 3";   //non-linear kernel
            //svmBinClass.CustomParams = String.Format("-j {0}",j);

            svmBinClass.Train(trainingSet);

            // prepare test set
            var testSet = new LabeledDataset<string, SparseVector<double>>();
            for (int i = data.Count * 1 / 4; i < data.Count; i++)
            {
                testSet.Add(data[i].Polarity == 0 ? "Negative" : "Positive", bowData[i]);
            }

            var matrix = new PerfMatrix<string>(StringComparer.InvariantCulture);

            int correctCount = 0;
            foreach (LabeledExample<string, SparseVector<double>> labeledExample in testSet)
            {
                var prediction = svmBinClass.Predict(labeledExample.Example);
                Console.WriteLine(prediction.BestClassLabel + "\t" + labeledExample.Label);
                if (prediction.BestClassLabel == labeledExample.Label) { correctCount++; }
                matrix.AddCount(labeledExample.Label, prediction.BestClassLabel);
            }
            Result = (double)correctCount / testSet.Count;
            Console.WriteLine("Accuracy on test set: {0}", Result);

            Console.WriteLine(matrix.ToString());

            foreach (string label in new[] { "Positive", "Negative"})
            {
                foreach (PerfMetric metric in Enum.GetValues(typeof(PerfMetric)))
                {
                    Console.WriteLine("Score for '{0}', metric {1}: {2}", label, metric, matrix.GetScore(metric, label));
                }
            }
        }

        private static List<LabeledTweet> GetLabeledTweets()
        {
            var parser = new TextFieldParser(@"Example\Data\testdata.manual.2009.06.14.csv")
                {
                    TextFieldType = FieldType.Delimited
                };
            try
            {
                parser.SetDelimiters(",");
                var labeledData = new List<LabeledTweet>();
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    labeledData.Add(new LabeledTweet
                        {
                            Polarity = int.Parse(fields[0]),
                            Id = int.Parse(fields[1]),
                            Date = DateTime.ParseExact(fields[2], "ddd MMM dd HH:mm:ss UTC yyyy",
                                CultureInfo.CreateSpecificCulture("en-US")),
                            Query = fields[3],
                            User = fields[4],
                            Text = fields[5]
                        });
                }
                return labeledData;
            }
            finally
            {
                parser.Close();
            }
        }

        private class LabeledTweet
        {
            public int Polarity { get; set; } //  0 = negative, 2 = neutral, 4 = positive
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Query { get; set; }
            public string User { get; set; }
            public string Text { get; set; }
        }
    }
}