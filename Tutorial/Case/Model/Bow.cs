/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Bow.cs
 *  Desc:    Tutorial 4.1: Text preprocessing
 *  Created: Apr-2010
 *
 *  Authors: Miha Grcar
 *
 **********************************************************************/

using System;
using System.IO;
using Latino;
using Latino.TextMining;

namespace Tutorial.Case.Model
{
    public class Bow : Tutorial<Bow>
    {
        public override void Run(object[] args)
        {
            // Get the stop words and stemmer for English.

            IStemmer stemmer;
            IStopWords stopWords;
            TextMiningUtils.GetLanguageTools(Language.English,
                out stopWords, out stemmer);

            // Test the stemmer.

            Output.WriteLine(stemmer.GetStem("running"));
            // Output: run

            // Create a tokenizer.

            UnicodeTokenizer tokenizer = new UnicodeTokenizer();
            tokenizer.MinTokenLen = 2; // Each token must be at least 2 
            // characters long.
            tokenizer.TokenFilter = TokenFilter.AlphaStrict; // Tokens
            // can consist of alphabetic characters only.

            // Test the tokenizer.

            foreach (string token in tokenizer.GetTokens("one 1 two 2 three 3 one_1 two_2 three_3"))
            {
                Console.Write("\"{0}\" ", token);
            }
            Output.WriteLine();
            // Output: "one" "two" "three"

            // Load a document corpus from a file. Each line in the file
            // represents one document.

            string[] docs = File.ReadAllLines(@"Data\YahooFinance.txt");

            // Create a bag-of-words space.

            BowSpace bowSpc = new BowSpace();
            bowSpc.Tokenizer = tokenizer; // Assign the tokenizer.
            bowSpc.StopWords = stopWords; // Assign the stop words.
            bowSpc.Stemmer = stemmer; // Assign the stemmer.
            bowSpc.MinWordFreq = 3; // A term must appear at least 3
            // times in the corpus for it to be part of the 
            // vocabulary.
            bowSpc.MaxNGramLen = 3; // Terms consisting of at most 3 
            // consecutive words will be considered.
            bowSpc.WordWeightType = WordWeightType.TfIdf; // Set the 
            // weighting scheme for the bag-of-words vectors to
            // TF-IDF.
            bowSpc.NormalizeVectors = true; // The TF-IDF vectors will 
            // be normalized.
            bowSpc.CutLowWeightsPerc = 0.2; // The terms with the lowest
            // weights, summing up to 20% of the overall weight sum,
            // will be removed from each TF-IDF vector.

            bowSpc.Initialize(docs); // Initialize the Bow space.

            // Output the vocabulary (the terms, their stems, 
            // frequencies, and document frequencies) to the console.

            StreamWriter stdOut = new StreamWriter(Console.OpenStandardOutput());
            bowSpc.OutputStats(stdOut);
            stdOut.Close();

            // Output the TF-IDF vector representing the description of
            // Google to the console.


            /*
            SparseVector<double>.ReadOnly googVec  = bowSpc.BowVectors[4192 - 1]; // The description of 
                // Google can be found at the row 4192 in the corpus.
            foreach (IdxDat<double> termInfo in googVec)
            {
                Output.WriteLine("{0} : {1}", 
                    bowSpc.Words[termInfo.Idx].MostFrequentForm, 
                    termInfo.Dat);
            }

            // Extract the top 5 terms with the highest TF-IDF weights 
            // from the vector representing Google.

            Output.WriteLine(bowSpc.GetKeywordsStr(googVec, 5));
            // Output: google, relevant, targeted advertising, search, 
            // index
             */
        }
    }
}