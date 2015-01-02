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

namespace Latino.TextMining
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
        private ArrayList<SparseVector<double>> mTfVectors
            = new ArrayList<SparseVector<double>>();
        private int mMaxNGramLen
            = 2;
        private ArrayList<int> mFreeIdx
            = new ArrayList<int>();
        private int mWordFormUpdateInit 
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

        public IncrementalBowSpace(BinarySerializer reader)
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

        public ArrayList<Word>.ReadOnly Words
        {
            get { return mIdxInfo; }
        }

        public Word GetWordFromStem(string stem)
        {
            Utils.ThrowException(stem == null ? new ArgumentNullException("stem") : null);
            Word word;
            mWordInfo.TryGetValue(stem.ToLower(), out word);
            return word;
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

        public ArrayList<SparseVector<double>> GetMostOutdatedBows(int num, WordWeightType wordWeightType, bool normalizeVectors, double cutLowWeightsPerc,
            int minWordFreq)
        {
            Utils.ThrowException(num < 0 ? new ArgumentOutOfRangeException("num") : null);
            Utils.ThrowException(cutLowWeightsPerc < 0 || cutLowWeightsPerc >= 1 ? new ArgumentOutOfRangeException("cutLowWeightsPerc") : null);
            Utils.ThrowException(minWordFreq < 1 ? new ArgumentOutOfRangeException("minWordFreq") : null);
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
            else if (wordWeightType == WordWeightType.TfIdf || wordWeightType == WordWeightType.TfIdfSafe)
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
                            tmp.InnerDat.Add(tfInfo.Dat * Idf(mIdxInfo[tfInfo.Idx], mTfVectors.Count + (wordWeightType == WordWeightType.TfIdf ? 0 : 1)));
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
            Utils.ThrowException(num < 0 ? new ArgumentOutOfRangeException("num") : null);
            Utils.ThrowException(cutLowWeightsPerc < 0 || cutLowWeightsPerc >= 1 ? new ArgumentOutOfRangeException("cutLowWeightsPerc") : null);
            Utils.ThrowException(minWordFreq < 1 ? new ArgumentOutOfRangeException("minWordFreq") : null);
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
            else if (wordWeightType == WordWeightType.TfIdf || wordWeightType == WordWeightType.TfIdfSafe)
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
                            tmp.InnerDat.Add(tfInfo.Dat * Idf(mIdxInfo[tfInfo.Idx], mTfVectors.Count + (wordWeightType == WordWeightType.TfIdf ? 0 : 1)));
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

        public void Dequeue(int n)
        {
            Utils.ThrowException(n < 0 ? new ArgumentOutOfRangeException("n") : null);
            for (int i = 0; i < n; i++)
            {
                SparseVector<double> tfVec = mTfVectors[i];
                // decrease docFreq and freq
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
                    }
                }
            }
            mTfVectors.RemoveRange(0, n);
        }

        public void UpdateMostFrequentWordForms()
        {
            mWordFormUpdate = mWordFormUpdateInit;
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

        public void Enqueue(IEnumerable<string> docs)
        {
            Utils.ThrowException(docs == null ? new ArgumentNullException("docs") : null);
            foreach (string doc in docs)
            {
                Utils.ThrowException(doc == null ? new ArgumentValueException("docs") : null);
                SparseVector<double> tfVec = ProcessDocument(doc);
                mTfVectors.Add(tfVec);
                mWordFormUpdate--;
                if (mWordFormUpdate == 0)
                {
                    UpdateMostFrequentWordForms();
                }
            }
        }

        private void ProcessDocumentNGrams(ArrayList<WordStem> nGrams, int startIdx, Dictionary<int, int> tfVec, Set<string> docWords)
        {            
            string nGramStem = "";
            string nGram = "";
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                nGram += nGrams[i].mWord;
                nGramStem += nGrams[i].mStem;
                if (mWordInfo.ContainsKey(nGramStem))
                {
                    Word word = mWordInfo[nGramStem];
                    // increase docFreq and freq
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
                    ProcessDocumentNGrams(nGrams, 0, tfVec, docWords);
                }
            }
            int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                ProcessDocumentNGrams(nGrams, i, tfVec, docWords);
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
            mIdxInfo.Save(writer);
        }

        public void LoadVocabulary(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            ArrayList<IdxDat<Word>> tmp = new ArrayList<IdxDat<Word>>();
            // the following statements throw serialization-related exceptions
            mWordInfo.Clear();         
            mIdxInfo.Load(reader);
            foreach (Word word in mIdxInfo)
            {
                if (word != null) 
                { 
                    mWordInfo.Add(word.Stem, word); 
                }
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
            mFreeIdx.Save(writer);
            writer.WriteInt(mWordFormUpdateInit);
            writer.WriteInt(mWordFormUpdate);            
            mTfVectors.Save(writer);
        }

        public void Load(BinarySerializer reader)
        {
            // the following statements throw serialization-related exceptions
            LoadVocabulary(reader); // throws ArgumentNullException
            mTokenizer = reader.ReadObject<ITokenizer>();
            mStopWords = reader.ReadObject<Set<string>.ReadOnly>();
            mStemmer = reader.ReadObject<IStemmer>();
            mMaxNGramLen = reader.ReadInt();
            mFreeIdx.Load(reader);
            mWordFormUpdateInit = reader.ReadInt();
            mWordFormUpdate = reader.ReadInt();            
            mTfVectors.Load(reader);
        }
    }
}