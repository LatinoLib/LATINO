/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          __IncrementalBowSpace__.cs (experimental)
 *  Version:       1.0
 *  Desc:		   Incremental bag-of-words space 
 *  Author:        Miha Grcar
 *  Created on:    Dec-2008
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Latino.Model;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class WordExt
       |
       '-----------------------------------------------------------------------
    */
    public class WordExt : Word
    {
        internal string mStem;

        public WordExt(string nGram, string stem) : base(nGram)
        {
            mStem = stem;
        }

        public WordExt(BinarySerializer reader) : base(reader)
        {
            // TODO: implementation
            throw new NotImplementedException();
        }

        public new double Idf(int docCount)
        {
            return Math.Log((double)docCount / (double)mDocFreq);
        }
    }

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
        private Dictionary<string, WordExt> mWordInfo
            = new Dictionary<string, WordExt>();
        private ArrayList<WordExt> mIdxInfo
            = new ArrayList<WordExt>();
        private ArrayList<SparseVector<double>> mTfVectors
            = new ArrayList<SparseVector<double>>();
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
        private ArrayList<int> mFreeIdx
            = new ArrayList<int>();
        private int mWordFormUpdateInit // TODO: expose to the user
            = 500;
        private int mWordFormUpdate
            = 500;

        public IncrementalBowSpace()
        {
            // configure tokenizer
            UnicodeTokenizer tokenizer = (UnicodeTokenizer)mTokenizer;
            tokenizer.Filter = TokenizerFilter.AlphanumLoose;
            tokenizer.MinTokenLen = 2;
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

        public ArrayList<WordExt>.ReadOnly Words
        {
            get { return mIdxInfo; }
        }

        public void OutputStats(StreamWriter writer)
        {
            writer.WriteLine("Word\tStem\tF\tDF");
            foreach (KeyValuePair<string, WordExt> wordInfo in mWordInfo)
            {
                writer.WriteLine("{0}\t{1}\t{2}\t{3}", wordInfo.Value.mMostFrequentForm, wordInfo.Key, wordInfo.Value.mFreq, wordInfo.Value.mDocFreq);
            }
        }

        private void CutLowWeights(ref SparseVector<double> vec)
        {
            if (mCutLowWeightsPerc > 0)
            {
                double wgtSum = 0;
                ArrayList<KeyDat<double, int>> tmp = new ArrayList<KeyDat<double, int>>(vec.Count);
                foreach (IdxDat<double> item in vec)
                {
                    wgtSum += item.Dat;
                    tmp.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                }
                tmp.Sort();
                double cutSum = mCutLowWeightsPerc * wgtSum;
                double cutWgt = -1;
                foreach (KeyDat<double, int> item in tmp)
                {
                    cutSum -= item.Key;
                    if (cutSum <= 0)
                    {
                        cutWgt = item.Key;
                        break;
                    }
                }                
                SparseVector<double> newVec = new SparseVector<double>();
                if (cutWgt != -1)
                {
                    foreach (IdxDat<double> item in vec)
                    {
                        if (item.Dat >= cutWgt)
                        {
                            newVec.InnerIdx.Add(item.Idx);
                            newVec.InnerDat.Add(item.Dat);
                        }
                    }
                }
                vec = newVec;
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
                    WordExt nGramInfo = new WordExt(nGram, nGramStem);
                    mWordInfo.Add(nGramStem, nGramInfo);
                    docWords.Add(nGramStem);
                }
                else
                {
                    WordExt nGramInfo = mWordInfo[nGramStem];
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
                nGramStem += nGrams[i].Stem;
                if (mWordInfo.ContainsKey(nGramStem))
                {
                    WordExt wordInfo = mWordInfo[nGramStem];
                    if (wordInfo.mIdx == -1)
                    {
                        if (mFreeIdx.Count > 0)
                        {
                            wordInfo.mIdx = mFreeIdx.Last;
                            mIdxInfo[wordInfo.mIdx] = wordInfo;
                            mFreeIdx.RemoveAt(mFreeIdx.Count - 1);
                        }
                        else
                        {
                            wordInfo.mIdx = mIdxInfo.Count;
                            mIdxInfo.Add(wordInfo);                        
                        }
                        tfVec.Add(wordInfo.mIdx, 1);
                        
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

        public void Initialize(IEnumerable<string> documents)
        {
            Utils.ThrowException(documents == null ? new ArgumentNullException("documents") : null);            
            mWordInfo.Clear();
            mIdxInfo.Clear();
            mTfVectors.Clear();
            // build vocabulary
            Utils.VerboseLine("Building vocabulary ...");
            int docCount = 0;
            foreach (string document in documents)
            {
                docCount++;
                Utils.VerboseProgress("Document {0} ...", docCount);
                Set<string> docWords = new Set<string>();
                ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
                mTokenizer.Text = document;
                foreach (string token in mTokenizer)
                {
                    string word = token.Trim().ToLower();
                    if (mStopWords == null || !mStopWords.Contains(word))
                    {
                        string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
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
                        ProcessNGramsPass1(nGrams, 0, docWords);
                    }
                }
                int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
                for (int i = startIdx; i < nGrams.Count; i++)
                {
                    ProcessNGramsPass1(nGrams, i, docWords);
                }
            }
            Utils.VerboseProgress("Document {0} ...", docCount, docCount);
            // determine most frequent word and n-gram forms
            foreach (WordExt wordInfo in mWordInfo.Values)
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
            // compute bag-of-words vectors
            Utils.VerboseLine("Computing bag-of-words vectors ...");           
            int docNum = 1;
            foreach (string document in documents)
            {                
                Utils.VerboseProgress("Document {0} / {1} ...", docNum++, docCount);
                Dictionary<int, int> tfVec = new Dictionary<int, int>();
                ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
                mTokenizer.Text = document;
                foreach (string token in mTokenizer)
                {
                    string word = token.Trim().ToLower();                    
                    if (mStopWords == null || !mStopWords.Contains(word))
                    {
                        string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
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
                        ProcessNGramsPass2(nGrams, 0, tfVec);
                    }
                }
                int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
                for (int i = startIdx; i < nGrams.Count; i++)
                {
                    ProcessNGramsPass2(nGrams, i, tfVec);
                }
                SparseVector<double> docVec = new SparseVector<double>();
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    docVec.InnerIdx.Add(tfItem.Key);
                    docVec.InnerDat.Add(tfItem.Value);
                }
                docVec.Sort();
                mTfVectors.Add(docVec);
            }
        }

        // TODO: exceptions
        public ArrayList<SparseVector<double>.ReadOnly> GetBowVectors()
        {
            ArrayList<SparseVector<double>.ReadOnly> bowVectors = new ArrayList<SparseVector<double>.ReadOnly>(mTfVectors.Count);            
            if (mWordWeightType == WordWeightType.TermFreq)
            {
                foreach (SparseVector<double> tfVec in mTfVectors)
                {
                    SparseVector<double> tmp = new SparseVector<double>();
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= mMinWordFreq)
                        {
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            tmp.InnerDat.Add(tfInfo.Dat);
                        }
                    }
                    CutLowWeights(ref tmp);
                    if (mNormalizeVectors) { Utils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            else if (mWordWeightType == WordWeightType.TfIdf)
            {
                foreach (SparseVector<double> tfVec in mTfVectors)
                {
                    SparseVector<double> tmp = new SparseVector<double>();
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= mMinWordFreq)
                        {                           
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            tmp.InnerDat.Add(tfInfo.Dat * mIdxInfo[tfInfo.Idx].Idf(mTfVectors.Count));
                        }
                    }
                    CutLowWeights(ref tmp);
                    if (mNormalizeVectors) { Utils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            else if (mWordWeightType == WordWeightType.LogDfTfIdf)
            {
                foreach (SparseVector<double> tfVec in mTfVectors)
                {
                    SparseVector<double> tmp = new SparseVector<double>();
                    foreach (IdxDat<double> tfInfo in tfVec)
                    {
                        if (mIdxInfo[tfInfo.Idx].Freq >= mMinWordFreq)
                        {
                            tmp.InnerIdx.Add(tfInfo.Idx);
                            double tfIdf = tfInfo.Dat * mIdxInfo[tfInfo.Idx].Idf(mTfVectors.Count);
                            tmp.InnerDat.Add(Math.Log(1 + mIdxInfo[tfInfo.Idx].mDocFreq) * tfIdf);
                        }
                    }
                    CutLowWeights(ref tmp);
                    if (mNormalizeVectors) { Utils.TryNrmVecL2(tmp); }
                    bowVectors.Add(tmp);
                }
            }
            return bowVectors;
        }

        // TODO: exceptions
        public void Dequeue(int n)
        {
            for (int i = 0; i < n; i++)
            {
                SparseVector<double> tfVec = mTfVectors[i];
                // decrease docFreq and freq, update IDF
                foreach (IdxDat<double> tfInfo in tfVec)
                {
                    WordExt word = mIdxInfo[tfInfo.Idx];
                    word.mFreq -= (int)tfInfo.Dat;
                    word.mDocFreq--;
                    if (word.mDocFreq == 0)
                    {
                        mIdxInfo[word.mIdx] = null;
                        mWordInfo.Remove(word.mStem);
                        mFreeIdx.Add(word.mIdx);
                    }
                }
            }
            mTfVectors.RemoveRange(0, n);
        }

        // TODO: exceptions
        public ArrayList<SparseVector<double>> Enqueue(IEnumerable<string> docs)
        {
            ArrayList<SparseVector<double>> retVal = new ArrayList<SparseVector<double>>();
            foreach (string doc in docs)
            {
                SparseVector<double> tfVec = ProcessDocument(doc);
                SparseVector<double> vec = null;
                mTfVectors.Add(tfVec);
                mWordFormUpdate--;
                if (mWordFormUpdate == 0)
                {
                    mWordFormUpdate = mWordFormUpdateInit;
                    // set most frequent word forms
                    foreach (WordExt wordInfo in mWordInfo.Values)
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
                if (mWordWeightType == WordWeightType.TermFreq)
                {
                    vec = new SparseVector<double>();
                    foreach (IdxDat<double> item in tfVec)
                    {
                        if (mIdxInfo[item.Idx].Freq >= mMinWordFreq)
                        {
                            vec.InnerIdx.Add(item.Idx);
                            vec.InnerDat.Add(item.Dat);
                        }
                    }
                }
                else if (mWordWeightType == WordWeightType.TfIdf)
                {
                    vec = new SparseVector<double>();
                    foreach (IdxDat<double> item in tfVec)
                    {
                        if (mIdxInfo[item.Idx].Freq >= mMinWordFreq)
                        {
                            vec.InnerIdx.Add(item.Idx);
                            vec.InnerDat.Add(item.Dat * mIdxInfo[item.Idx].Idf(mTfVectors.Count));
                        }
                    }
                }
                else if (mWordWeightType == WordWeightType.LogDfTfIdf)
                {
                    vec = new SparseVector<double>();
                    foreach (IdxDat<double> item in tfVec)
                    {
                        if (mIdxInfo[item.Idx].Freq >= mMinWordFreq)
                        {
                            vec.InnerIdx.Add(item.Idx);
                            double tfIdf = item.Dat * mIdxInfo[item.Idx].Idf(mTfVectors.Count);
                            vec.InnerDat.Add(Math.Log(1 + mIdxInfo[item.Idx].mDocFreq) * tfIdf);
                        }
                    }
                }
                CutLowWeights(ref vec);
                if (mNormalizeVectors) { Utils.TryNrmVecL2(vec); }
                retVal.Add(vec);
            }
            return retVal;
        }

        private void ProcessDocumentNGrams(ArrayList<WordStem> nGrams, int startIdx, Dictionary<int, int> tfVec, Set<string> docWords)
        {            
            string nGramStem = "";
            string nGram = "";
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                nGram += nGrams[i].Word;
                nGramStem += nGrams[i].Stem;
                if (mWordInfo.ContainsKey(nGramStem))
                {
                    WordExt word = mWordInfo[nGramStem];
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
                        }
                        tfVec.Add(word.mIdx, 1);
                    }
                }
                else // new word
                {
                    WordExt word = new WordExt(nGram, nGramStem);                    
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
                    tfVec.Add(word.mIdx, 1);
                }
                nGram += " ";
                nGramStem += " ";
            }
        }

        private SparseVector<double> ProcessDocument(string document)
        {
            Set<string> docWords = new Set<string>();
            Dictionary<int, int> tfVec = new Dictionary<int, int>();
            ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
            mTokenizer.Text = document;
            foreach (string token in mTokenizer)
            {
                string word = token.Trim().ToLower();
                if (mStopWords == null || !mStopWords.Contains(word))
                {
                    string stem = mStemmer == null ? word : mStemmer.GetStem(word).Trim().ToLower();
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
                    ProcessDocumentNGrams(nGrams, 0, tfVec, docWords);
                }
            }
            int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                ProcessDocumentNGrams(nGrams, i, tfVec, docWords);
            }
            SparseVector<double> docVec = new SparseVector<double>();
            foreach (KeyValuePair<int, int> tfItem in tfVec)
            {
                docVec.InnerIdx.Add(tfItem.Key);
                docVec.InnerDat.Add(tfItem.Value);
            }
            docVec.Sort();
            return docVec;
        }

        public ArrayList<KeyDat<double, string>> GetKeywords(SparseVector<double>.ReadOnly bowVec)
        {
            Utils.ThrowException(bowVec == null ? new ArgumentNullException("bowVec") : null);            
            ArrayList<KeyDat<double, string>> keywords = new ArrayList<KeyDat<double, string>>(bowVec.Count);
            foreach (IdxDat<double> item in bowVec)
            {
                keywords.Add(new KeyDat<double, string>(item.Dat, mIdxInfo[item.Idx].mMostFrequentForm)); // throws ArgumentOutOfRangeException
            }
            keywords.Sort(new DescSort<KeyDat<double, string>>());
            return keywords;
        }

        public ArrayList<string> GetKeywords(SparseVector<double>.ReadOnly bowVec, int n)
        {
            Utils.ThrowException(n <= 0 ? new ArgumentOutOfRangeException("n") : null);
            ArrayList<KeyDat<double, string>> keywords = GetKeywords(bowVec); // throws ArgumentNullException, ArgumentOutOfRangeException            
            int keywordCount = Math.Min(n, keywords.Count);
            ArrayList<string> keywordList = new ArrayList<string>(keywordCount);
            for (int i = 0; i < keywordCount; i++)
            {
                keywordList.Add(keywords[i].Dat);
            }
            return keywordList;
        }

        // *** ISerializable interface implementation ***

        public void SaveVocabulary(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(mWordInfo.Count);
            foreach (KeyValuePair<string, WordExt> item in mWordInfo)
            {
                writer.WriteString(item.Key);
                item.Value.Save(writer);
            }
        }

        public void LoadVocabulary(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            ArrayList<IdxDat<WordExt>> tmp = new ArrayList<IdxDat<WordExt>>();
            // the following statements throw serialization-related exceptions
            mWordInfo.Clear();
            mIdxInfo.Clear();
            mTfVectors.Clear(); // *** bags-of-words are removed 
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                WordExt dat = new WordExt(reader);
                mWordInfo.Add(key, dat);
                tmp.Add(new IdxDat<WordExt>(dat.mIdx, dat));
            }
            tmp.Sort();
            foreach (IdxDat<WordExt> item in tmp)
            {
                mIdxInfo.Add(item.Dat);
            }
        }

        public void Save(BinarySerializer writer)
        {
            // the following statements throw serialization-related exceptions
            SaveVocabulary(writer); // throws ArgumentNullException
            writer.WriteObject<ITokenizer>(mTokenizer);
            writer.WriteObject<Set<string>.ReadOnly>(mStopWords);
            writer.WriteObject<IStemmer>(mStemmer);
            mTfVectors.Save(writer);
            writer.WriteInt(mMaxNGramLen);
            writer.WriteInt((int)mWordWeightType);
            writer.WriteDouble(mCutLowWeightsPerc);
            writer.WriteBool(mNormalizeVectors);
        }

        public void Load(BinarySerializer reader)
        {
            // the following statements throw serialization-related exceptions
            LoadVocabulary(reader); // throws ArgumentNullException
            mTokenizer = reader.ReadObject<ITokenizer>();
            mStopWords = reader.ReadObject<Set<string>.ReadOnly>();
            mStemmer = reader.ReadObject<IStemmer>();
            mTfVectors.Load(reader);
            mMaxNGramLen = reader.ReadInt();
            mWordWeightType = (WordWeightType)reader.ReadInt();
            mCutLowWeightsPerc = reader.ReadDouble();
            mNormalizeVectors = reader.ReadBool();
        }
    }
}