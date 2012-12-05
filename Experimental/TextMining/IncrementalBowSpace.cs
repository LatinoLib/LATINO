/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IncrementalBowSpace.cs 
 *  Desc:    Incremental bag-of-words space 
 *  Created: Dec-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Latino.Model;
using Latino.TextMining;

namespace Latino.Experimental.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class IncrementalBowSpace
       |
       '-----------------------------------------------------------------------
    */
    public class IncrementalBowSpace : ISerializable
    {
        private class WordStem
        {
            public string Word
                = null;
            public string Stem
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
        private ArrayList<SparseVector<double>> mTfVectors
            = new ArrayList<SparseVector<double>>();
        private int mMaxNGramLen
            = 2;
        //private int mMinWordFreq
        //    = 5;
        //private WordWeightType mWordWeightType
        //    = WordWeightType.TermFreq;
        //private double mCutLowWeightsPerc
        //    = 0.2;
        //private bool mNormalizeVectors
        //    = true;
        private ArrayList<int> mFreeIdx
            = new ArrayList<int>();
        private int mWordFormUpdateInit
            = 500;
        private int mWordFormUpdate
            = 500;

        private ArrayList<Word>.ReadOnly mIdxInfoReadOnly;

        private static Logger mLogger
            = Logger.GetLogger(typeof(IncrementalBowSpace));

        public IncrementalBowSpace()
        {
            // configure tokenizer
            UnicodeTokenizer tokenizer = (UnicodeTokenizer)mTokenizer;
            tokenizer.Filter = TokenizerFilter.AlphanumLoose;
            tokenizer.MinTokenLen = 2;
            mIdxInfoReadOnly = mIdxInfo;
        }

        public IncrementalBowSpace(BinarySerializer reader) : this()
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
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

        //public int MinWordFreq
        //{
        //    get { return mMinWordFreq; }
        //    set
        //    {
        //        Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("MinWordFreq") : null);
        //        mMinWordFreq = value;
        //    }
        //}

        //public WordWeightType WordWeightType
        //{
        //    get { return mWordWeightType; }
        //    set { mWordWeightType = value; }
        //}

        //public double CutLowWeightsPerc
        //{
        //    get { return mCutLowWeightsPerc; }
        //    set
        //    {
        //        Utils.ThrowException(value < 0 || value >= 1 ? new ArgumentOutOfRangeException("CutLowWeightsPerc") : null);
        //        mCutLowWeightsPerc = value;
        //    }
        //}

        //public bool NormalizeVectors
        //{
        //    get { return mNormalizeVectors; }
        //    set { mNormalizeVectors = value; }
        //}

        public ArrayList<Word>.ReadOnly Words
        {
            get { return mIdxInfoReadOnly; }
        }

        private double Idf(Word word, int docCount)
        {
            return Math.Log((double)docCount / (double)word.mDocFreq);
        }

        public void OutputStats(StreamWriter writer)
        {
            writer.WriteLine("Word\tStem\tF\tDF");
            foreach (KeyValuePair<string, Word> wordInfo in mWordInfo)
            {
                writer.WriteLine("{0}\t{1}\t{2}\t{3}", wordInfo.Value.mMostFrequentForm, wordInfo.Key, wordInfo.Value.mFreq, wordInfo.Value.mDocFreq);
            }
        }

        private void ProcessNGramsPass1(ArrayList<WordStem> nGrams, int startIdx, Set<string> docWords)
        {
            string nGramStem = "";
            string nGram = "";
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                nGram += nGrams[i].Word;
                nGramStem += nGrams[i].Stem;
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

        //private void ProcessNGramsPass2(ArrayList<WordStem> nGrams, int startIdx, Dictionary<int, int> tfVec)
        //{
        //    string nGramStem = "";
        //    for (int i = startIdx; i < nGrams.Count; i++)
        //    {
        //        nGramStem += nGrams[i].Stem;
        //        if (mWordInfo.ContainsKey(nGramStem))
        //        {
        //            Word wordInfo = mWordInfo[nGramStem];
        //            if (wordInfo.mIdx == -1)
        //            {
        //                if (mFreeIdx.Count > 0)
        //                {
        //                    wordInfo.mIdx = mFreeIdx.Last;
        //                    mIdxInfo[wordInfo.mIdx] = wordInfo;
        //                    mFreeIdx.RemoveAt(mFreeIdx.Count - 1);
        //                }
        //                else
        //                {
        //                    wordInfo.mIdx = mIdxInfo.Count;
        //                    mIdxInfo.Add(wordInfo);                        
        //                }
        //                tfVec.Add(wordInfo.mIdx, 1);                        
        //            }
        //            else if (!tfVec.ContainsKey(wordInfo.mIdx))
        //            {
        //                tfVec.Add(wordInfo.mIdx, 1);
        //            }
        //            else
        //            {
        //                tfVec[wordInfo.mIdx]++;
        //            }
        //        }
        //        else
        //        {
        //            break;
        //        }
        //        nGramStem += " ";
        //    }
        //}

        //public void Initialize(IEnumerable<string> documents)
        //{
        //    Utils.ThrowException(documents == null ? new ArgumentNullException("documents") : null);            
        //    mWordInfo.Clear();
        //    mIdxInfo.Clear();
        //    mTfVectors.Clear();
        //    // build vocabulary
        //    mLogger.Info("Initialize", "Building vocabulary ...");
        //    int docCount = 0;
        //    foreach (string document in documents)
        //    {
        //        docCount++;
        //        mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, /*numSteps=*/-1);
        //        Set<string> docWords = new Set<string>();
        //        ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
        //        mTokenizer.Text = document;
        //        foreach (string token in mTokenizer)
        //        {
        //            string word = token.Trim().ToLower();
        //            if (mStopWords == null || !mStopWords.Contains(word))
        //            {
        //                string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
        //                if (nGrams.Count < mMaxNGramLen)
        //                {
        //                    WordStem wordStem = new WordStem();
        //                    wordStem.Word = word;
        //                    wordStem.Stem = stem;
        //                    nGrams.Add(wordStem);
        //                    if (nGrams.Count < mMaxNGramLen) { continue; }
        //                }
        //                else
        //                {
        //                    WordStem wordStem = nGrams[0];
        //                    wordStem.Word = word;
        //                    wordStem.Stem = stem;
        //                    for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
        //                    nGrams[mMaxNGramLen - 1] = wordStem;
        //                }
        //                ProcessNGramsPass1(nGrams, 0, docWords);
        //            }
        //        }
        //        int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
        //        for (int i = startIdx; i < nGrams.Count; i++)
        //        {
        //            ProcessNGramsPass1(nGrams, i, docWords);
        //        }
        //    }
        //    mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} ...", docCount, docCount);
        //    // determine most frequent word and n-gram forms
        //    foreach (Word wordInfo in mWordInfo.Values)
        //    { 
        //        int max = 0;
        //        foreach (KeyValuePair<string, int> wordForm in wordInfo.mForms)
        //        { 
        //            if (wordForm.Value > max) 
        //            { 
        //                max = wordForm.Value;
        //                wordInfo.mMostFrequentForm = wordForm.Key;
        //            }
        //        }
        //    }
        //    // compute bag-of-words vectors
        //    mLogger.Info("Initialize", "Computing bag-of-words vectors ...");           
        //    int docNum = 1;
        //    foreach (string document in documents)
        //    {
        //        mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Initialize", "Document {0} / {1} ...", docNum++, docCount);
        //        Dictionary<int, int> tfVec = new Dictionary<int, int>();
        //        ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
        //        mTokenizer.Text = document;
        //        foreach (string token in mTokenizer)
        //        {
        //            string word = token.Trim().ToLower();                    
        //            if (mStopWords == null || !mStopWords.Contains(word))
        //            {
        //                string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
        //                if (nGrams.Count < mMaxNGramLen)
        //                {
        //                    WordStem wordStem = new WordStem();
        //                    wordStem.Word = word;
        //                    wordStem.Stem = stem;
        //                    nGrams.Add(wordStem);
        //                    if (nGrams.Count < mMaxNGramLen) { continue; }
        //                }
        //                else
        //                {
        //                    WordStem wordStem = nGrams[0];
        //                    wordStem.Word = word;
        //                    wordStem.Stem = stem;
        //                    for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
        //                    nGrams[mMaxNGramLen - 1] = wordStem;
        //                }
        //                ProcessNGramsPass2(nGrams, 0, tfVec);
        //            }
        //        }
        //        int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
        //        for (int i = startIdx; i < nGrams.Count; i++)
        //        {
        //            ProcessNGramsPass2(nGrams, i, tfVec);
        //        }
        //        SparseVector<double> docVec = new SparseVector<double>(tfVec.Count);
        //        foreach (KeyValuePair<int, int> tfItem in tfVec)
        //        {
        //            docVec.InnerIdx.Add(tfItem.Key);
        //            docVec.InnerDat.Add(tfItem.Value);
        //        }
        //        docVec.Sort();
        //        mTfVectors.Add(docVec);
        //    }
        //}

        public ArrayList<SparseVector<double>> GetMostOutdatedBows(int num, WordWeightType wordWeightType, bool normalizeVectors, double cutLowWeightsPerc,
            int minWordFreq)
        {
            num = Math.Min(num, mTfVectors.Count);
            ArrayList<SparseVector<double>> bowVectors = new ArrayList<SparseVector<double>>(num);
            if (wordWeightType == WordWeightType.TermFreq)
            {
                for (int i = 0; i < num; i++)
                {
                    SparseVector<double> tfVec = mTfVectors[i];
                    SparseVector<double> tmp = new SparseVector<double>(tfVec.Count);
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= minWordFreq)
                        {
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            tmp.InnerDat.Add(tfInfo.Dat);
                        }
                    }
                    ModelUtils.CutLowWeights(ref tmp, cutLowWeightsPerc);
                    if (normalizeVectors) { ModelUtils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            else if (wordWeightType == WordWeightType.TfIdf)
            {
                for (int i = 0; i < num; i++)
                {
                    SparseVector<double> tfVec = mTfVectors[i];
                    SparseVector<double> tmp = new SparseVector<double>(tfVec.Count);
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= minWordFreq)
                        {
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            tmp.InnerDat.Add(tfInfo.Dat * Idf(mIdxInfo[tfInfo.Idx], mTfVectors.Count));
                        }
                    }
                    ModelUtils.CutLowWeights(ref tmp, cutLowWeightsPerc);
                    if (normalizeVectors) { ModelUtils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            else if (wordWeightType == WordWeightType.LogDfTfIdf)
            {
                for (int i = 0; i < num; i++)
                {
                    SparseVector<double> tfVec = mTfVectors[i];
                    SparseVector<double> tmp = new SparseVector<double>(tfVec.Count);
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= minWordFreq)
                        {
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            double tfIdf = tfInfo.Dat * Idf(mIdxInfo[tfInfo.Idx], mTfVectors.Count);
                            tmp.InnerDat.Add(Math.Log(1 + mIdxInfo[tfInfo.Idx].mDocFreq) * tfIdf);
                        }
                    }
                    ModelUtils.CutLowWeights(ref tmp, cutLowWeightsPerc);
                    if (normalizeVectors) { ModelUtils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            return bowVectors;
        }

        public ArrayList<SparseVector<double>> GetMostRecentBows(int num, WordWeightType wordWeightType, bool normalizeVectors, double cutLowWeightsPerc,
            int minWordFreq)
        {
            num = Math.Min(num, mTfVectors.Count);
            ArrayList<SparseVector<double>> bowVectors = new ArrayList<SparseVector<double>>(num);
            if (wordWeightType == WordWeightType.TermFreq)
            {
                for (int i = mTfVectors.Count - num; i < mTfVectors.Count; i++)
                {
                    SparseVector<double> tfVec = mTfVectors[i];
                    SparseVector<double> tmp = new SparseVector<double>(tfVec.Count);
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= minWordFreq)
                        {
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            tmp.InnerDat.Add(tfInfo.Dat);
                        }
                    }
                    ModelUtils.CutLowWeights(ref tmp, cutLowWeightsPerc);
                    if (normalizeVectors) { ModelUtils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            else if (wordWeightType == WordWeightType.TfIdf)
            {
                for (int i = mTfVectors.Count - num; i < mTfVectors.Count; i++)
                {
                    SparseVector<double> tfVec = mTfVectors[i];
                    SparseVector<double> tmp = new SparseVector<double>(tfVec.Count);
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= minWordFreq)
                        {
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            tmp.InnerDat.Add(tfInfo.Dat * Idf(mIdxInfo[tfInfo.Idx], mTfVectors.Count));
                        }
                    }
                    ModelUtils.CutLowWeights(ref tmp, cutLowWeightsPerc);
                    if (normalizeVectors) { ModelUtils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            else if (wordWeightType == WordWeightType.LogDfTfIdf)
            {
                for (int i = mTfVectors.Count - num; i < mTfVectors.Count; i++)
                {
                    SparseVector<double> tfVec = mTfVectors[i];
                    SparseVector<double> tmp = new SparseVector<double>(tfVec.Count);
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= minWordFreq)
                        {
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            double tfIdf = tfInfo.Dat * Idf(mIdxInfo[tfInfo.Idx], mTfVectors.Count);
                            tmp.InnerDat.Add(Math.Log(1 + mIdxInfo[tfInfo.Idx].mDocFreq) * tfIdf);
                        }
                    }
                    ModelUtils.CutLowWeights(ref tmp, cutLowWeightsPerc);
                    if (normalizeVectors) { ModelUtils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            return bowVectors;        
        }

        public int Count
        {
            get { return mTfVectors.Count; }
        }

        public void Dequeue(int n, Set<int> removedWords)
        {
            for (int i = 0; i < n; i++)
            {
                SparseVector<double> tfVec = mTfVectors[i];
                // decrease docFreq and freq, update IDF
                foreach (IdxDat<double> tfInfo in tfVec)
                {
                    Word word = mIdxInfo[tfInfo.Idx];
                    word.mFreq -= (int)tfInfo.Dat;
                    word.mDocFreq--;
                    if (word.mDocFreq == 0)
                    {
                        mIdxInfo[word.mIdx] = null;
                        mWordInfo.Remove(word.mStem);
                        mFreeIdx.Add(word.mIdx);
                        removedWords.Add(word.mIdx);
                    }
                }
            }
            mTfVectors.RemoveRange(0, n);
        }

        public void Enqueue(IEnumerable<string> docs, Set<int> addedWords)
        {
            foreach (string doc in docs)
            {
                SparseVector<double> tfVec = ProcessDocument(doc, addedWords);
                mTfVectors.Add(tfVec);
                mWordFormUpdate--;
                if (mWordFormUpdate == 0)
                {
                    mWordFormUpdate = mWordFormUpdateInit;
                    // set most frequent word forms
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
                    }
                }
            }
        }

        private void ProcessDocumentNGrams(ArrayList<WordStem> nGrams, int startIdx, Dictionary<int, int> tfVec, Set<string> docWords, Set<int> addedWords)
        {            
            string nGramStem = "";
            string nGram = "";
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                nGram += nGrams[i].Word;
                nGramStem += nGrams[i].Stem;
                if (mWordInfo.ContainsKey(nGramStem))
                {
                    Word word = mWordInfo[nGramStem];
                    // increase docFreq and freq, update IDF
                    word.mFreq++;
                    if (!word.mForms.ContainsKey(nGram))
                    {
                        word.mForms.Add(nGram, 1);
                    }
                    else
                    {
                        word.mForms[nGram]++;
                    }
                    if (!docWords.Contains(nGramStem))
                    {
                        word.mDocFreq++;
                        docWords.Add(nGramStem);
                    }
                    if (tfVec.ContainsKey(word.mIdx))
                    {
                        tfVec[word.mIdx]++;
                    }
                    else
                    {
                        if (word.mIdx == -1)
                        {
                            if (mFreeIdx.Count > 0)
                            {
                                word.mIdx = mFreeIdx.Last;
                                mIdxInfo[word.mIdx] = word;
                                mFreeIdx.RemoveAt(mFreeIdx.Count - 1);
                            }
                            else
                            {
                                word.mIdx = mIdxInfo.Count;
                                mIdxInfo.Add(word);
                            }
                            addedWords.Add(word.mIdx);
                        }
                        tfVec.Add(word.mIdx, 1);
                    }
                }
                else // new word
                {
                    Word word = new Word(nGram, nGramStem);                    
                    mWordInfo.Add(nGramStem, word);
                    docWords.Add(nGramStem);
                    if (mFreeIdx.Count > 0)
                    {
                        word.mIdx = mFreeIdx.Last;
                        mIdxInfo[word.mIdx] = word;
                        mFreeIdx.RemoveAt(mFreeIdx.Count - 1);
                    }
                    else
                    {
                        word.mIdx = mIdxInfo.Count;
                        mIdxInfo.Add(word);
                    }
                    addedWords.Add(word.mIdx);
                    tfVec.Add(word.mIdx, 1);
                }
                nGram += " ";
                nGramStem += " ";
            }
        }

        public SparseVector<double> ProcessDocument(string document, Set<int> addedWords)
        {
            return ProcessDocument(document, mStemmer, addedWords); // throws ArgumentNullException
        }

        public SparseVector<double> ProcessDocument(string document, IStemmer stemmer, Set<int> addedWords)
        {
            Utils.ThrowException(document == null ? new ArgumentNullException("document") : null);
            Set<string> docWords = new Set<string>();
            Dictionary<int, int> tfVec = new Dictionary<int, int>();
            ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
            mTokenizer.Text = document;
            foreach (string token in mTokenizer)
            {
                string word = token.Trim().ToLower();
                if (mStopWords == null || !mStopWords.Contains(word))
                {
                    string stem = stemmer == null ? word : stemmer.GetStem(word).Trim().ToLower();
                    if (nGrams.Count < mMaxNGramLen)
                    {
                        WordStem wordStem = new WordStem();
                        wordStem.Word = word;
                        wordStem.Stem = stem;
                        nGrams.Add(wordStem);
                        if (nGrams.Count < mMaxNGramLen) { continue; }
                    }
                    else
                    {
                        WordStem wordStem = nGrams[0];
                        wordStem.Word = word;
                        wordStem.Stem = stem;
                        for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                        nGrams[mMaxNGramLen - 1] = wordStem;
                    }
                    ProcessDocumentNGrams(nGrams, 0, tfVec, docWords, addedWords);
                }
            }
            int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                ProcessDocumentNGrams(nGrams, i, tfVec, docWords, addedWords);
            }
            SparseVector<double> docVec = new SparseVector<double>(tfVec.Count);
            foreach (KeyValuePair<int, int> tfItem in tfVec)
            {
                docVec.InnerIdx.Add(tfItem.Key);
                docVec.InnerDat.Add(tfItem.Value);
            }
            docVec.Sort();
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
            mTfVectors.Clear(); // *** bags-of-words are removed
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
            //writer.WriteInt(mMinWordFreq);
            //writer.WriteInt((int)mWordWeightType);
            //writer.WriteDouble(mCutLowWeightsPerc);
            //writer.WriteBool(mNormalizeVectors);
        }

        public void Load(BinarySerializer reader)
        {
            // the following statements throw serialization-related exceptions
            LoadVocabulary(reader); // throws ArgumentNullException
            mTokenizer = reader.ReadObject<ITokenizer>();
            mStopWords = reader.ReadObject<Set<string>.ReadOnly>();
            mStemmer = reader.ReadObject<IStemmer>();
            mMaxNGramLen = reader.ReadInt();
            //mMinWordFreq = reader.ReadInt();
            //mWordWeightType = (WordWeightType)reader.ReadInt();
            //mCutLowWeightsPerc = reader.ReadDouble();
            //mNormalizeVectors = reader.ReadBool();
        }
    }
}