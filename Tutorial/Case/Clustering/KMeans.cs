/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    KMeans.cs
 *  Desc:    Tutorial 4.2: Unsupervised machine learning
 *  Created: Apr-2010
 *
 *  Authors: Miha Grcar
 *
 **********************************************************************/

using System;
using System.IO;
using Latino;
using Latino.TextMining;
using Latino.Model;

namespace Tutorial.Case.Clustering
{
    public class KMeans : Tutorial<KMeans>
    {
        public override void Run(object[] args)
        {
            // Get the stop words and stemmer for English.

            IStemmer stemmer;
            Set<string>.ReadOnly stopWords;
            TextMiningUtils.GetLanguageTools(Language.English,
                out stopWords, out stemmer);

            // Create a tokenizer.

            UnicodeTokenizer tokenizer = new UnicodeTokenizer();
            tokenizer.MinTokenLen = 2; // Each token must be at least 2 
            // characters long.
            tokenizer.Filter = TokenizerFilter.AlphaStrict; // Tokens
            // can consist of alphabetic characters only.

            // Load a document corpus from a file. Each line in the file
            // represents one document.

            string[] docs = File.ReadAllLines(@"Example\Data\YahooFinance.txt");

            // Create a bag-of-words space.

            BowSpace bowSpc = new BowSpace();
            bowSpc.Tokenizer = tokenizer; // Assign the tokenizer.
            bowSpc.StopWords = stopWords; // Assign the stop words.
            bowSpc.Stemmer = stemmer; // Assign the stemmer.
            bowSpc.MinWordFreq = 3; // A term must appear at least 3 times in the corpus for it to be part of the vocabulary.
            bowSpc.MaxNGramLen = 3; // Terms consisting of at most 3 consecutive words will be considered.
            bowSpc.WordWeightType = WordWeightType.TfIdf; // Set the weighting scheme for the bag-of-words vectors to TF-IDF.
            bowSpc.NormalizeVectors = true; // The TF-IDF vectors will be normalized.
            bowSpc.CutLowWeightsPerc = 0.2; // The terms with the lowest weights, summing up to 20% of the overall weight sum, will be removed from each TF-IDF vector.

            ArrayList<SparseVector<double>> sparseVectors = bowSpc.Initialize(docs); // Initialize the Bow space.
            UnlabeledDataset<SparseVector<double>> ud = new UnlabeledDataset<SparseVector<double>>(sparseVectors);

            // Compute 100 clusters of documents.

            KMeansClusteringFast kMeans = new KMeansClusteringFast(100); // Set k to 100.
            kMeans.Trials = 3; // Perform 3 repetitions. Take the best result.
            kMeans.Eps = 0.001; // Stop iterating when the partition quality increases for less than 0.001.

            ClusteringResult cr = kMeans.Cluster(ud); // Execute.

            // Extract the top 5 terms with the highest TF-IDF weights 
            // from each of the clusters' centroids and output the 
            // number of documents (companies) in each cluster.

            foreach (Cluster cl in cr.Roots)
            {
                SparseVector<double>.ReadOnly centroid = cl.ComputeCentroid(ud, CentroidType.NrmL2);
                Console.Write(bowSpc.GetKeywordsStr(centroid, 5));
                Output.WriteLine(" ({0} companies)", cl.Items.Count);
            }

            // Output the documents that are contained in the first 
            // cluster.

            foreach (int docIdx in cr.Roots[0].Items)
            {
                Output.WriteLine(docs[docIdx]);
            }
        }
    }
}