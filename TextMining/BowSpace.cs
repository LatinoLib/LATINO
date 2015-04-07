/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    BowSpace.cs
 *  Desc:    Bag-of-words space
 *  Created: Dec-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Latino.Model;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Enum WordWeightType
       |
       '-----------------------------------------------------------------------
    */
    public enum WordWeightType
    { 
        TermFreq,
        TfIdf,
        TfIdfSafe,
        LogDfTfIdf
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class Word
       |
       '-----------------------------------------------------------------------
    */
    public class Word : IEnumerable<KeyValuePair<string, int>>, ISerializable
    {
        internal int mIdx
            = -1;
        internal Dictionary<string, int> mForms
            = new Dictionary<string, int>();
        internal string mMostFrequentForm;
        internal string mStem;
        internal int mDocFreq
            = 1;
        internal int mFreq
            = 1;
        internal double mIdf
            = -1;

        public Word(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        internal Word(string word, string stem)
        {
            mStem = stem;
            mMostFrequentForm = word;
            mForms.Add(word, 1);
        }

        public string MostFrequentForm
        {
            get { return mMostFrequentForm; }
        }

        public ArrayList<string> GetWordForms()
        {
            return new ArrayList<string>(mForms.Keys);
        }

        public int DocFreq
        {
            get { return mDocFreq; }
        }

        public int Freq
        {
            get { return mFreq; }
        }

        public double Idf
        {
            get { return mIdf; }
        }

        public string Stem
        {
            get { return mStem; }
        }

        public static ulong GetHashCode64(string word)
        {
            Utils.ThrowException(word == null ? new ArgumentNullException("word") : null);
            byte[] hashCode128 = new MD5CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(word));
            ulong part1 = (ulong)BitConverter.ToInt64(hashCode128, 0);
            ulong part2 = (ulong)BitConverter.ToInt64(hashCode128, 8);
            return part1 ^ part2;
        }

        public ulong GetHashCode64()
        {
            return GetHashCode64(mStem);
        }

        // *** IEnumerable<KeyValuePair<string, int>> interface implementation ***

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return mForms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mForms.GetEnumerator();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(mIdx);
            writer.WriteString(mMostFrequentForm);
            writer.WriteString(mStem);
            writer.WriteInt(mDocFreq);
            writer.WriteInt(mFreq);
            writer.WriteDouble(mIdf);
            Utils.SaveDictionary(mForms, writer);
        }

        internal void Load(BinarySerializer reader)
        {
            mIdx = reader.ReadInt();
            mMostFrequentForm = reader.ReadString();
            mStem = reader.ReadString();
            mDocFreq = reader.ReadInt();
            mFreq = reader.ReadInt();
            mIdf = reader.ReadDouble();
            mForms = Utils.LoadDictionary<string, int>(reader);
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class BowSpace
       |
       '-----------------------------------------------------------------------
    */
    public class BowSpace : ISerializable
    {
        private class WordStem
        {
            public string mWord
                = null;
            public string mStem
                = null;
        }

        private ITokenizer mTokenizer
            = new UnicodeTokenizer();
        private Set<string>.ReadOnly mStopWords
            = null;
        private IStemmer mStemmer
            = null;
        private Dictionary<string, Word> mWordInfo
            = new Dictionary<string, Word>();
        private ArrayList<Word> mIdxInfo
            = new ArrayList<Word>();
        private int mMaxNGramLen
            = 2;
        private int mMinWordFreq
            = 5;
        private WordWeightType mWordWeightType
            = WordWeightType.TermFreq;
        private double mCutLowWeightsPerc
            = 0.2;
        private bool mNormalizeVectors
            = true;
        private bool mKeepWordForms
            = false;

        private Logger mLogger
            = Logger.GetInstanceLogger(typeof(BowSpace));

        public BowSpace()
        {
            // configure tokenizer
            UnicodeTokenizer tokenizer = (UnicodeTokenizer)mTokenizer;
            tokenizer.Filter = TokenizerFilter.AlphanumLoose;
            tokenizer.MinTokenLen = 2;
        }

        public BowSpace(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public Logger Logger
        {
            get { return mLogger; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Logger") : null);
                mLogger = value;
            }
        }

        public ITokenizer Tokenizer
        {
            get { return mTokenizer; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Tokenizer") : null);
                mTokenizer = value; 
            }
        }

        public Set<string>.ReadOnly StopWords
        {
            get { return mStopWords; }
            set { mStopWords = value; }
        }

        public IStemmer Stemmer
        {
            get { return mStemmer; }
            set { mStemmer = value; }
        }

        public int MaxNGramLen
        {
            get { return mMaxNGramLen; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("MaxNGramLen") : null);
                mMaxNGramLen = value;
            }
        }

        public int MinWordFreq
        {
            get { return mMinWordFreq; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("MinWordFreq") : null);
                mMinWordFreq = value;
            }
        }

        public WordWeightType WordWeightType
        {
            get { return mWordWeightType; }
            set { mWordWeightType = value; }
        }

        public double CutLowWeightsPerc
        {
            get { return mCutLowWeightsPerc; }
            set
            {
                Utils.ThrowException(value < 0 || value >= 1 ? new ArgumentOutOfRangeException("CutLowWeightsPerc") : null);
                mCutLowWeightsPerc = value;
            }
        }

        public bool NormalizeVectors
        {
            get { return mNormalizeVectors; }
            set { mNormalizeVectors = value; }
        }

        public ArrayList<Word>.ReadOnly Words
        {
            get { return mIdxInfo; }
        }

        public bool KeepWordForms
        {
            get { return mKeepWordForms; }
            set { mKeepWordForms = value; }
        }

        public void OutputStats(StreamWriter writer)
        {
            writer.WriteLine("Word\tStem\tF\tDF");
            foreach (KeyValuePair<string, Word> wordInfo in mWordInfo)
            {
                writer.WriteLine("{0}\t{1}\t{2}\t{3}", wordInfo.Value.mMostFrequentForm, wordInfo.Key, wordInfo.Value.mFreq, wordInfo.Value.mDocFreq);
            }
        }

        private void CutLowWeights(ref SparseVector<double> vec)
        {
            ModelUtils.CutLowWeights(ref vec, mCutLowWeightsPerc);
        }

        private void ProcessNGramsPass1(ArrayList<WordStem> nGrams, int startIdx, Set<string> docWords)
        {
            string nGramStem = "";
            string nGram = "";
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                nGram += nGrams[i].mWord;
                nGramStem += nGrams[i].mStem;
                if (!mWordInfo.ContainsKey(nGramStem))
                {
                    Word nGramInfo = new Word(nGram, nGramStem);
                    mWordInfo.Add(nGramStem, nGramInfo);
                    docWords.Add(nGramStem);
                }
                else
                {
                    Word nGramInfo = mWordInfo[nGramStem];
                    if (!docWords.Contains(nGramStem))
                    {
                        docWords.Add(nGramStem);
                        nGramInfo.mDocFreq++;
                    }
                    nGramInfo.mFreq++;
                    if (!nGramInfo.mForms.ContainsKey(nGram))
                    {
                        nGramInfo.mForms.Add(nGram, 1);
                    }
                    else
                    {
                        nGramInfo.mForms[nGram]++;
                    }
                }
                nGram += " ";
                nGramStem += " ";
            }
        }

        private void ProcessNGramsPass2(ArrayList<WordStem> nGrams, int startIdx, Dictionary<int, int> tfVec)
        {
            string nGramStem = "";
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                nGramStem += nGrams[i].mStem;
                if (mWordInfo.ContainsKey(nGramStem))
                {
                    Word wordInfo = mWordInfo[nGramStem];
                    if (wordInfo.mIdx == -1)
                    {
                        wordInfo.mIdx = mIdxInfo.Count;
                        tfVec.Add(wordInfo.mIdx, 1);
                        mIdxInfo.Add(wordInfo);
                    }
                    else if (!tfVec.ContainsKey(wordInfo.mIdx))
                    {
                        tfVec.Add(wordInfo.mIdx, 1);
                    }
                    else
                    {
                        tfVec[wordInfo.mIdx]++;
                    }
                }
                else
                {
                    break;
                }
                nGramStem += " ";
            }
        }

        public ArrayList<SparseVector<double>> Initialize(IEnumerable<string> documents)
        {
            return Initialize(documents, /*largeScale=*/false);
        }

        public ArrayList<SparseVector<double>> Initialize(IEnumerable<string> documents, bool largeScale)
        {
            Utils.ThrowException(documents == null ? new ArgumentNullException("documents") : null);            
            mWordInfo.Clear();
            mIdxInfo.Clear();
            ArrayList<SparseVector<double>> bows = new ArrayList<SparseVector<double>>();
            // build vocabulary
            mLogger.Info("Initialize", "Building vocabulary ...");
            int docCount = 0;
            if (!largeScale)
            {                
                foreach (string document in documents)
                {
                    docCount++;
                    mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, /*numSteps=*/-1);
                    Set<string> docWords = new Set<string>();
                    ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
                    foreach (string token in mTokenizer.GetTokens(document))
                    {
                        string word = token.Trim().ToLower();
                        if (mStopWords == null || !mStopWords.Contains(word))
                        {
                            string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
                            if (nGrams.Count < mMaxNGramLen)
                            {
                                WordStem wordStem = new WordStem();
                                wordStem.mWord = word;
                                wordStem.mStem = stem;
                                nGrams.Add(wordStem);
                                if (nGrams.Count < mMaxNGramLen) { continue; }
                            }
                            else
                            {
                                WordStem wordStem = nGrams[0];
                                wordStem.mWord = word;
                                wordStem.mStem = stem;
                                for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                                nGrams[mMaxNGramLen - 1] = wordStem;
                            }
                            ProcessNGramsPass1(nGrams, 0, docWords);
                        }
                    }
                    int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
                    for (int i = startIdx; i < nGrams.Count; i++)
                    {
                        ProcessNGramsPass1(nGrams, i, docWords);
                    }
                }
                mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, docCount);
            }
            else // large-scale mode (needs less memory, slower)
            {
                for (int n = 1; n <= mMaxNGramLen; n++)
                {
                    docCount = 0;
                    mLogger.Info("Initialize", "Pass {0} of {1} ...", n, mMaxNGramLen);
                    foreach (string document in documents)
                    {
                        docCount++;
                        mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, /*numSteps=*/-1);
                        ArrayList<WordStem> nGrams = new ArrayList<WordStem>(n);
                        Set<string> docWords = new Set<string>();
                        foreach (string token in mTokenizer.GetTokens(document))
                        {
                            string word = token.Trim().ToLower();
                            if (mStopWords == null || !mStopWords.Contains(word))
                            {
                                string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
                                if (nGrams.Count < n)
                                {
                                    WordStem wordStem = new WordStem();
                                    wordStem.mWord = word;
                                    wordStem.mStem = stem;
                                    nGrams.Add(wordStem);
                                    if (nGrams.Count < n) { continue; }
                                }
                                else
                                {
                                    WordStem wordStem = nGrams[0];
                                    wordStem.mWord = word;
                                    wordStem.mStem = stem;
                                    for (int i = 0; i < n - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                                    nGrams[n - 1] = wordStem;
                                }
                                string nGram = nGrams[0].mWord;
                                string nGramStem = nGrams[0].mStem;
                                if (n > 1)
                                {
                                    for (int i = 1; i < n - 1; i++)
                                    {
                                        nGram += " " + nGrams[i].mWord;
                                        nGramStem += " " + nGrams[i].mStem;
                                    }
                                    if (!mWordInfo.ContainsKey(nGramStem)) { continue; }
                                    if (mWordInfo[nGramStem].mFreq < mMinWordFreq) { continue; }
                                    string nGramStem2 = "";
                                    for (int i = 1; i < n - 1; i++)
                                    {
                                        nGramStem2 += nGrams[i].mStem + " ";
                                    }
                                    nGramStem2 += nGrams[n - 1].mStem;
                                    if (!mWordInfo.ContainsKey(nGramStem2)) { continue; }
                                    if (mWordInfo[nGramStem2].mFreq < mMinWordFreq) { continue; }
                                    nGram += " " + nGrams[n - 1].mWord;
                                    nGramStem += " " + nGrams[n - 1].mStem;
                                }
                                if (!mWordInfo.ContainsKey(nGramStem))
                                {
                                    Word nGramInfo = new Word(nGram, nGramStem);
                                    mWordInfo.Add(nGramStem, nGramInfo);
                                    docWords.Add(nGramStem);
                                }
                                else
                                {
                                    Word nGramInfo = mWordInfo[nGramStem];
                                    if (!docWords.Contains(nGramStem))
                                    {
                                        nGramInfo.mDocFreq++;
                                        docWords.Add(nGramStem);                                        
                                    }
                                    nGramInfo.mFreq++;
                                    if (!nGramInfo.mForms.ContainsKey(nGram))
                                    {
                                        nGramInfo.mForms.Add(nGram, 1);
                                    }
                                    else
                                    {
                                        nGramInfo.mForms[nGram]++;
                                    }
                                }
                            }
                        }
                    }
                    mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, docCount);
                }
            }            
            // remove unfrequent words and n-grams, precompute IDF      
            ArrayList<string> removeList = new ArrayList<string>();
            foreach (KeyValuePair<string, Word> wordInfo in mWordInfo)
            {
                if (wordInfo.Value.mFreq < mMinWordFreq)
                {
                    removeList.Add(wordInfo.Key);
                }
                else
                {
                    wordInfo.Value.mIdf = Math.Log((double)docCount / (double)wordInfo.Value.mDocFreq);
                }
            }
            foreach (string key in removeList) { mWordInfo.Remove(key); }
            // determine most frequent word and n-gram forms
            foreach (Word wordInfo in mWordInfo.Values)
            { 
                int max = 0;
                foreach (KeyValuePair<string, int> wordForm in wordInfo.mForms)
                { 
                    if (wordForm.Value > max) 
                    { 
                        max = wordForm.Value;
                        wordInfo.mMostFrequentForm = wordForm.Key;
                    }
                }
                if (!mKeepWordForms) { wordInfo.mForms.Clear(); } 
            }
            // compute bag-of-words vectors
            mLogger.Info("Initialize", "Computing bag-of-words vectors ...");           
            int docNum = 1;
            foreach (string document in documents)
            {
                mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} / {1} ...", docNum++, docCount);
                Dictionary<int, int> tfVec = new Dictionary<int, int>();
                ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
                foreach (string token in mTokenizer.GetTokens(document))
                {
                    string word = token.Trim().ToLower();                    
                    if (mStopWords == null || !mStopWords.Contains(word))
                    {
                        string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
                        if (nGrams.Count < mMaxNGramLen)
                        {
                            WordStem wordStem = new WordStem();
                            wordStem.mWord = word;
                            wordStem.mStem = stem;
                            nGrams.Add(wordStem);
                            if (nGrams.Count < mMaxNGramLen) { continue; }
                        }
                        else
                        {
                            WordStem wordStem = nGrams[0];
                            wordStem.mWord = word;
                            wordStem.mStem = stem;
                            for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                            nGrams[mMaxNGramLen - 1] = wordStem;
                        }
                        ProcessNGramsPass2(nGrams, 0, tfVec);
                    }
                }
                int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
                for (int i = startIdx; i < nGrams.Count; i++)
                {
                    ProcessNGramsPass2(nGrams, i, tfVec);
                }
                SparseVector<double> docVec = new SparseVector<double>();
                if (mWordWeightType == WordWeightType.TermFreq)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(tfItem.Value);
                    }
                }
                else if (mWordWeightType == WordWeightType.TfIdf)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                        if (tfIdf > 0)
                        {
                            docVec.InnerIdx.Add(tfItem.Key);
                            docVec.InnerDat.Add(tfIdf);
                        }
                    }
                }
                else if (mWordWeightType == WordWeightType.LogDfTfIdf)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                        if (tfIdf > 0)
                        {
                            docVec.InnerIdx.Add(tfItem.Key);
                            docVec.InnerDat.Add(Math.Log(1 + mIdxInfo[tfItem.Key].mDocFreq) * tfIdf);
                        }
                    }
                }
                docVec.Sort();
                CutLowWeights(ref docVec);
                if (mNormalizeVectors) { ModelUtils.TryNrmVecL2(docVec); }
                bows.Add(docVec); 
            }
            return bows; 
        }

        // TODO: merge this with Initialize
        public ArrayList<SparseVector<double>> InitializeTokenized(IEnumerable<ITokenizerEnumerable> documents, bool largeScale)
        {
            Utils.ThrowException(documents == null ? new ArgumentNullException("documents") : null);
            mWordInfo.Clear();
            mIdxInfo.Clear();
            ArrayList<SparseVector<double>> bows = new ArrayList<SparseVector<double>>();
            // build vocabulary
            mLogger.Info("Initialize", "Building vocabulary ...");
            int docCount = 0;
            if (!largeScale)
            {
                foreach (ITokenizerEnumerable document in documents)
                {
                    docCount++;
                    mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, /*numSteps=*/-1);
                    Set<string> docWords = new Set<string>();
                    ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
                    foreach (string word in document)
                    {
                        //string word = token.Trim().ToLower();
                        if (mStopWords == null || !mStopWords.Contains(word))
                        {
                            string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
                            if (nGrams.Count < mMaxNGramLen)
                            {
                                WordStem wordStem = new WordStem();
                                wordStem.mWord = word;
                                wordStem.mStem = stem;
                                nGrams.Add(wordStem);
                                if (nGrams.Count < mMaxNGramLen) { continue; }
                            }
                            else
                            {
                                WordStem wordStem = nGrams[0];
                                wordStem.mWord = word;
                                wordStem.mStem = stem;
                                for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                                nGrams[mMaxNGramLen - 1] = wordStem;
                            }
                            ProcessNGramsPass1(nGrams, 0, docWords);
                        }
                    }
                    int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
                    for (int i = startIdx; i < nGrams.Count; i++)
                    {
                        ProcessNGramsPass1(nGrams, i, docWords);
                    }
                }
                mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, docCount);
            }
            else // large-scale mode (needs less memory, slower)
            {
                for (int n = 1; n <= mMaxNGramLen; n++)
                {
                    docCount = 0;
                    mLogger.Info("Initialize", "Pass {0} of {1} ...", n, mMaxNGramLen);
                    foreach (ITokenizerEnumerable document in documents)
                    {
                        docCount++;
                        mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, /*numSteps=*/-1);
                        ArrayList<WordStem> nGrams = new ArrayList<WordStem>(n);
                        Set<string> docWords = new Set<string>();
                        foreach (string word in (IEnumerable<string>)document)
                        {
                            //string word = token.Trim().ToLower();
                            if (mStopWords == null || !mStopWords.Contains(word))
                            {
                                string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
                                if (nGrams.Count < n)
                                {
                                    WordStem wordStem = new WordStem();
                                    wordStem.mWord = word;
                                    wordStem.mStem = stem;
                                    nGrams.Add(wordStem);
                                    if (nGrams.Count < n) { continue; }
                                }
                                else
                                {
                                    WordStem wordStem = nGrams[0];
                                    wordStem.mWord = word;
                                    wordStem.mStem = stem;
                                    for (int i = 0; i < n - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                                    nGrams[n - 1] = wordStem;
                                }
                                string nGram = nGrams[0].mWord;
                                string nGramStem = nGrams[0].mStem;
                                if (n > 1)
                                {
                                    for (int i = 1; i < n - 1; i++)
                                    {
                                        nGram += " " + nGrams[i].mWord;
                                        nGramStem += " " + nGrams[i].mStem;
                                    }
                                    if (!mWordInfo.ContainsKey(nGramStem)) { continue; }
                                    if (mWordInfo[nGramStem].mFreq < mMinWordFreq) { continue; }
                                    string nGramStem2 = "";
                                    for (int i = 1; i < n - 1; i++)
                                    {
                                        nGramStem2 += nGrams[i].mStem + " ";
                                    }
                                    nGramStem2 += nGrams[n - 1].mStem;
                                    if (!mWordInfo.ContainsKey(nGramStem2)) { continue; }
                                    if (mWordInfo[nGramStem2].mFreq < mMinWordFreq) { continue; }
                                    nGram += " " + nGrams[n - 1].mWord;
                                    nGramStem += " " + nGrams[n - 1].mStem;
                                }
                                if (!mWordInfo.ContainsKey(nGramStem))
                                {
                                    Word nGramInfo = new Word(nGram, nGramStem);
                                    mWordInfo.Add(nGramStem, nGramInfo);
                                    docWords.Add(nGramStem);
                                }
                                else
                                {
                                    Word nGramInfo = mWordInfo[nGramStem];
                                    if (!docWords.Contains(nGramStem))
                                    {
                                        nGramInfo.mDocFreq++;
                                        docWords.Add(nGramStem);
                                    }
                                    nGramInfo.mFreq++;
                                    if (!nGramInfo.mForms.ContainsKey(nGram))
                                    {
                                        nGramInfo.mForms.Add(nGram, 1);
                                    }
                                    else
                                    {
                                        nGramInfo.mForms[nGram]++;
                                    }
                                }
                            }
                        }
                    }
                    mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, docCount);
                }
            }
            // remove unfrequent words and n-grams, precompute IDF      
            ArrayList<string> removeList = new ArrayList<string>();
            foreach (KeyValuePair<string, Word> wordInfo in mWordInfo)
            {
                if (wordInfo.Value.mFreq < mMinWordFreq)
                {
                    removeList.Add(wordInfo.Key);
                }
                else
                {
                    wordInfo.Value.mIdf = Math.Log((double)docCount / (double)wordInfo.Value.mDocFreq);
                }
            }
            foreach (string key in removeList) { mWordInfo.Remove(key); }
            // determine most frequent word and n-gram forms
            foreach (Word wordInfo in mWordInfo.Values)
            {
                int max = 0;
                foreach (KeyValuePair<string, int> wordForm in wordInfo.mForms)
                {
                    if (wordForm.Value > max)
                    {
                        max = wordForm.Value;
                        wordInfo.mMostFrequentForm = wordForm.Key;
                    }
                }
                if (!mKeepWordForms) { wordInfo.mForms.Clear(); }
            }
            // compute bag-of-words vectors
            mLogger.Info("Initialize", "Computing bag-of-words vectors ...");
            int docNum = 1;
            foreach (ITokenizerEnumerable document in documents)
            {
                mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} / {1} ...", docNum++, docCount);
                Dictionary<int, int> tfVec = new Dictionary<int, int>();
                ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
                foreach (string word in (IEnumerable<string>)document)
                {
                    //string word = token.Trim().ToLower();
                    if (mStopWords == null || !mStopWords.Contains(word))
                    {
                        string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
                        if (nGrams.Count < mMaxNGramLen)
                        {
                            WordStem wordStem = new WordStem();
                            wordStem.mWord = word;
                            wordStem.mStem = stem;
                            nGrams.Add(wordStem);
                            if (nGrams.Count < mMaxNGramLen) { continue; }
                        }
                        else
                        {
                            WordStem wordStem = nGrams[0];
                            wordStem.mWord = word;
                            wordStem.mStem = stem;
                            for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                            nGrams[mMaxNGramLen - 1] = wordStem;
                        }
                        ProcessNGramsPass2(nGrams, 0, tfVec);
                    }
                }
                int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
                for (int i = startIdx; i < nGrams.Count; i++)
                {
                    ProcessNGramsPass2(nGrams, i, tfVec);
                }
                SparseVector<double> docVec = new SparseVector<double>();
                if (mWordWeightType == WordWeightType.TermFreq)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(tfItem.Value);
                    }
                }
                else if (mWordWeightType == WordWeightType.TfIdf)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                        if (tfIdf > 0)
                        {
                            docVec.InnerIdx.Add(tfItem.Key);
                            docVec.InnerDat.Add(tfIdf);
                        }
                    }
                }
                else if (mWordWeightType == WordWeightType.LogDfTfIdf)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                        if (tfIdf > 0)
                        {
                            docVec.InnerIdx.Add(tfItem.Key);
                            docVec.InnerDat.Add(Math.Log(1 + mIdxInfo[tfItem.Key].mDocFreq) * tfIdf);
                        }
                    }
                }
                docVec.Sort();
                CutLowWeights(ref docVec);
                if (mNormalizeVectors) { ModelUtils.TryNrmVecL2(docVec); }
                bows.Add(docVec);
            }
            return bows;
        }

        private void ProcessDocumentNGrams(ArrayList<WordStem> nGrams, int startIdx, Dictionary<int, int> tfVec)
        {
            string nGramStem = "";
            string nGram = "";
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                nGram += nGrams[i].mWord;
                nGramStem += nGrams[i].mStem;
                if (mWordInfo.ContainsKey(nGramStem))
                {
                    int stemIdx = mWordInfo[nGramStem].mIdx;
                    if (tfVec.ContainsKey(stemIdx))
                    {
                        tfVec[stemIdx]++;
                    }
                    else
                    {
                        tfVec.Add(stemIdx, 1);
                    }
                }
                nGram += " ";
                nGramStem += " ";
            }
        }

        public SparseVector<double> ProcessDocument(string document)
        {
            return ProcessDocument(document, mStemmer); // throws ArgumentNullException
        }

        public SparseVector<double> ProcessDocument(string document, IStemmer stemmer)
        {
            Utils.ThrowException(document == null ? new ArgumentNullException("document") : null);
            Dictionary<int, int> tfVec = new Dictionary<int, int>();
            ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
            foreach (string token in mTokenizer.GetTokens(document))
            {
                string word = token.Trim().ToLower();
                if (mStopWords == null || !mStopWords.Contains(word))
                {
                    string stem = stemmer == null ? word : stemmer.GetStem(word).Trim().ToLower();
                    if (nGrams.Count < mMaxNGramLen)
                    {
                        WordStem wordStem = new WordStem();
                        wordStem.mWord = word;
                        wordStem.mStem = stem;
                        nGrams.Add(wordStem);
                        if (nGrams.Count < mMaxNGramLen) { continue; }
                    }
                    else
                    {
                        WordStem wordStem = nGrams[0];
                        wordStem.mWord = word;
                        wordStem.mStem = stem;
                        for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                        nGrams[mMaxNGramLen - 1] = wordStem;
                    }
                    ProcessDocumentNGrams(nGrams, 0, tfVec);
                }
            }
            int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                ProcessDocumentNGrams(nGrams, i, tfVec);
            }
            SparseVector<double> docVec = new SparseVector<double>();
            if (mWordWeightType == WordWeightType.TermFreq)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    docVec.InnerIdx.Add(tfItem.Key);
                    docVec.InnerDat.Add(tfItem.Value);
                }
            }
            else if (mWordWeightType == WordWeightType.TfIdf)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                    if (tfIdf > 0)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(tfIdf);
                    }
                }
            }
            else if (mWordWeightType == WordWeightType.LogDfTfIdf)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                    if (tfIdf > 0)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(Math.Log(1 + mIdxInfo[tfItem.Key].mDocFreq) * tfIdf);
                    }
                }
            }
            docVec.Sort();
            CutLowWeights(ref docVec);
            if (mNormalizeVectors) { ModelUtils.TryNrmVecL2(docVec); }
            return docVec;
        }

        // TODO: merge with ProcessDocument
        public SparseVector<double> ProcessDocumentTokenized(ITokenizerEnumerable document, IStemmer stemmer)
        {
            Utils.ThrowException(document == null ? new ArgumentNullException("document") : null);
            Dictionary<int, int> tfVec = new Dictionary<int, int>();
            ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
            foreach (string word in (IEnumerable<string>)document)
            {
                //string word = token.Trim().ToLower();
                if (mStopWords == null || !mStopWords.Contains(word))
                {
                    string stem = stemmer == null ? word : stemmer.GetStem(word).Trim().ToLower();
                    if (nGrams.Count < mMaxNGramLen)
                    {
                        WordStem wordStem = new WordStem();
                        wordStem.mWord = word;
                        wordStem.mStem = stem;
                        nGrams.Add(wordStem);
                        if (nGrams.Count < mMaxNGramLen) { continue; }
                    }
                    else
                    {
                        WordStem wordStem = nGrams[0];
                        wordStem.mWord = word;
                        wordStem.mStem = stem;
                        for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                        nGrams[mMaxNGramLen - 1] = wordStem;
                    }
                    ProcessDocumentNGrams(nGrams, 0, tfVec);
                }
            }
            int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                ProcessDocumentNGrams(nGrams, i, tfVec);
            }
            SparseVector<double> docVec = new SparseVector<double>();
            if (mWordWeightType == WordWeightType.TermFreq)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    docVec.InnerIdx.Add(tfItem.Key);
                    docVec.InnerDat.Add(tfItem.Value);
                }
            }
            else if (mWordWeightType == WordWeightType.TfIdf)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                    if (tfIdf > 0)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(tfIdf);
                    }
                }
            }
            else if (mWordWeightType == WordWeightType.LogDfTfIdf)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                    if (tfIdf > 0)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(Math.Log(1 + mIdxInfo[tfItem.Key].mDocFreq) * tfIdf);
                    }
                }
            }
            docVec.Sort();
            CutLowWeights(ref docVec);
            if (mNormalizeVectors) { ModelUtils.TryNrmVecL2(docVec); }
            return docVec;
        }

        public ArrayList<KeyDat<double, Word>> GetKeywords(SparseVector<double>.ReadOnly bowVec)
        {
            Utils.ThrowException(bowVec == null ? new ArgumentNullException("bowVec") : null);
            ArrayList<KeyDat<double, Word>> keywords = new ArrayList<KeyDat<double, Word>>(bowVec.Count);
            foreach (IdxDat<double> item in bowVec)
            {
                keywords.Add(new KeyDat<double, Word>(item.Dat, mIdxInfo[item.Idx])); // throws ArgumentOutOfRangeException
            }
            keywords.Sort(DescSort<KeyDat<double, Word>>.Instance);
            return keywords;
        }

        public ArrayList<Word> GetKeywords(SparseVector<double>.ReadOnly bowVec, int n)
        {
            Utils.ThrowException(n <= 0 ? new ArgumentOutOfRangeException("n") : null);
            ArrayList<KeyDat<double, Word>> keywords = GetKeywords(bowVec); // throws ArgumentNullException, ArgumentOutOfRangeException            
            int keywordCount = Math.Min(n, keywords.Count);
            ArrayList<Word> keywordList = new ArrayList<Word>(keywordCount);
            for (int i = 0; i < keywordCount; i++)
            {
                keywordList.Add(keywords[i].Dat);
            }
            return keywordList;
        }

        public string GetKeywordsStr(SparseVector<double>.ReadOnly bowVec, int n)
        {
            ArrayList<Word> keywords = GetKeywords(bowVec, n); // throws ArgumentOutOfRangeException, ArgumentNullException
            if (keywords.Count == 0) { return ""; }
            string keywordsStr = keywords[0].MostFrequentForm;
            for (int i = 1; i < keywords.Count; i++)
            {
                keywordsStr += ", " + keywords[i].MostFrequentForm;
            }
            return keywordsStr;
        }

        // *** ISerializable interface implementation ***

        public void SaveVocabulary(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(mWordInfo.Count);
            foreach (KeyValuePair<string, Word> item in mWordInfo)
            {
                writer.WriteString(item.Key);
                item.Value.Save(writer);
            }
        }

        public void LoadVocabulary(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            ArrayList<IdxDat<Word>> tmp = new ArrayList<IdxDat<Word>>();
            // the following statements throw serialization-related exceptions
            mWordInfo.Clear();
            mIdxInfo.Clear();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                Word dat = new Word(reader);
                mWordInfo.Add(key, dat);
                tmp.Add(new IdxDat<Word>(dat.mIdx, dat));
            }
            tmp.Sort();
            foreach (IdxDat<Word> item in tmp)
            {
                mIdxInfo.Add(item.Dat);
            }
        }

        public void Save(BinarySerializer writer)
        {
            // the following statements throw serialization-related exceptions
            SaveVocabulary(writer); // throws ArgumentNullException
            writer.WriteObject(mTokenizer);
            writer.WriteObject(mStopWords);
            writer.WriteObject(mStemmer);
            writer.WriteInt(mMaxNGramLen);
            writer.WriteInt(mMinWordFreq);
            writer.WriteInt((int)mWordWeightType);
            writer.WriteDouble(mCutLowWeightsPerc);
            writer.WriteBool(mNormalizeVectors);
            writer.WriteBool(mKeepWordForms);
        }

        public void Load(BinarySerializer reader)
        {
            // the following statements throw serialization-related exceptions
            LoadVocabulary(reader); // throws ArgumentNullException
            mTokenizer = reader.ReadObject<ITokenizer>();
            mStopWords = reader.ReadObject<Set<string>.ReadOnly>();
            mStemmer = reader.ReadObject<IStemmer>();
            mMaxNGramLen = reader.ReadInt();
            mMinWordFreq = reader.ReadInt();
            mWordWeightType = (WordWeightType)reader.ReadInt();
            mCutLowWeightsPerc = reader.ReadDouble();
            mNormalizeVectors = reader.ReadBool();
            mKeepWordForms = reader.ReadBool();
        }
    }
}