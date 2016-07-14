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
using System.Collections.Generic;
using System.Globalization;
using Latino;
using Latino.Model;
using Latino.TextMining;
using Microsoft.VisualBasic.FileIO;

namespace Tutorial.Case.Model
{
    public class BinarySvm : Tutorial<BinarySvm>
    {
        public override void Run(object[] args)
        {
            // prepare data
            IStemmer stemmer;
            IStopWords stopWords;
            TextMiningUtils.GetLanguageTools(Language.English, out stopWords, out stemmer);

            // Create a tokenizer.
            var tokenizer = new UnicodeTokenizer
                {
                    MinTokenLen = 2,                        // Each token must be at least 2 characters long.
                    Filter = TokenizerFilter.AlphaStrict    // Tokens can consist of alphabetic characters only.
                };

            // take data for two classes from cvs file
            var data = new List<LabeledTweet>(GetLabeledTweets().Where(lt => lt.Polarity != 2)).ToList();

            // Create a bag-of-words space.
            var bowSpc = new BowSpace
                {
                    Tokenizer = tokenizer,                  // Assign the tokenizer.
                    StopWords = stopWords,                  // Assign the stop words.
                    Stemmer = stemmer,                      // Assign the stemmer.
                    MinWordFreq = 1,                        // A term must appear at least n-times in the corpus for it to be part of the vocabulary.
                    MaxNGramLen = 2,                        // Terms consisting of at most n-consecutive words will be considered.
                    WordWeightType = WordWeightType.TermFreq,  // Set the weighting scheme for the bag-of-words vectors to TF.
                    //WordWeightType = WordWeightType.TfIdf,  // Set the weighting scheme for the bag-of-words vectors to TF-IDF.
                    NormalizeVectors = true,                // The TF-IDF vectors will be normalized.
                    CutLowWeightsPerc = 0                   // The terms with the lowest weights, summing up to 20% of the overall weight sum, will be removed from each TF-IDF vector.
                };
            ArrayList<SparseVector<double>> bowData = bowSpc.Initialize(data.Select(d => d.Text));

            // label data
            var labeledSet = new LabeledDataset<string, SparseVector<double>>();
            for (int i = 0; i < data.Count; i++)
            {
                labeledSet.Add(data[i].Label, bowData[i]);
            }
            labeledSet.Shuffle();

            int testSize = labeledSet.Count / 10;
            var trainingSet = new LabeledDataset<string, SparseVector<double>>(labeledSet.Skip(testSize));
            var testSet = new LabeledDataset<string, SparseVector<double>>(labeledSet.Take(testSize));

            //-------------------- SVM

            var svmBinClass = new SvmBinaryClassifier<string> { VerbosityLevel = SvmLightVerbosityLevel.Off };
            if (args.Any()) { svmBinClass.C = (int)args[0]; }
            //svmBinClass.BiasedHyperplane = true;
            //svmBinClass.CustomParams = "-t 3";   // non-linear kernel
            //svmBinClass.CustomParams = String.Format("-j {0}",j);

            svmBinClass.Train(trainingSet);

            int correct = 0;
            double avgDist = 0;
            foreach (LabeledExample<string, SparseVector<double>> labeledExample in testSet)
            {
                var prediction = svmBinClass.Predict(labeledExample.Example);
                //Output.WriteLine("actual: {0}\tpredicted: {1}\t score: {2:0.0000}", labeledExample.Label, prediction.BestClassLabel, prediction.BestScore);
                avgDist += prediction.BestScore;
                if (prediction.BestClassLabel == labeledExample.Label) { correct++; }
            }

            Output.WriteLine("Accuracy: {0:0.00}",  100.0 * correct / testSet.Count);
            Output.WriteLine("Avg. distance: {0:0.00}",  avgDist / testSet.Count);

            Result.Add("accuracy", (double)correct / testSet.Count);

            Result.Add("classifier", svmBinClass);
            Result.Add("labeled_data", labeledSet);
        }

        private static List<LabeledTweet> GetLabeledTweets()
        {
            var parser = new TextFieldParser(@"Data\testdata.manual.2009.06.14.csv")
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
            public int Polarity { get; set; }
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Query { get; set; }
            public string User { get; set; }
            public string Text { get; set; }
 
            public string Label
            {
                get
                {
                    switch (Polarity)
                    {
                        case 0: return "Negative";
                        case 2: return "Neutral";
                        case 4: return "Positive";
                        default: return "";
                    }
                }
            }
        }
    }
}