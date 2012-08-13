/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    TextMiningUtils.cs
 *  Desc:    Text mining utilities
 *  Created: Mar-2010
 *
 *  Authors: Miha Grcar, Marko Brakus
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class TextMiningUtils
       |    
       '-----------------------------------------------------------------------
    */
    public static class TextMiningUtils
    {
        /* .-----------------------------------------------------------------------
           |
           |  Class RangeInfo
           |    
           '-----------------------------------------------------------------------
        */
        private class RangeInfo
        {
            char mLastChar;
            string mRangeId;

            public RangeInfo(char lastChar, string rangeId)
            {
                mLastChar = lastChar;
                mRangeId = rangeId;
            }
        }

        private static ArrayList<Ref<string>> mCharRanges
            = new ArrayList<Ref<string>>();

        static TextMiningUtils()
        {
            Stream stream = Utils.GetManifestResourceStream(typeof(TextMiningUtils), "CharRanges.txt");
            StreamReader reader = new StreamReader(stream);
            string _line;
            Ref<string> charRangeOther = "Other";
            while ((_line = reader.ReadLine()) != null)
            {
                string line = _line.Trim();
                if (!line.StartsWith("#") && line != "")
                {
                    string[] parts = line.Split('\t');
                    int startIdx = int.Parse(parts[0], NumberStyles.HexNumber);
                    int endIdx = int.Parse(parts[1], NumberStyles.HexNumber);
                    if (startIdx > mCharRanges.Count)
                    {
                        //Console.Write(mCharRanges.Count + "\t");
                        for (int i = mCharRanges.Count; i < startIdx; i++) { mCharRanges.Add(charRangeOther); }
                        //Console.WriteLine("{0}\t{1}", mCharRanges.Count - 1, "Other");
                    }
                    Ref<string> charRangeId = parts[2];
                    for (int i = startIdx; i <= endIdx; i++) { mCharRanges.Add(charRangeId); }
                    //Console.WriteLine("{0}\t{1}\t{2}", startIdx, endIdx, parts[2]);                    
                }
            }
            stream.Close();
        }

        public static void GetLanguageTools(Language language, out Set<string>.ReadOnly stopWords, out IStemmer stemmer)
        {
            switch (language)
            { 
                case Language.Bulgarian:
                    stopWords = StopWords.BulgarianStopWords;
                    stemmer = new Lemmatizer(language);
                    break;
                case Language.Czech:
                    stopWords = StopWords.CzechStopWords;
                    stemmer = new Lemmatizer(language);
                    break;
                case Language.Danish:
                    stopWords = StopWords.DanishStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.Dutch:
                    stopWords = StopWords.DutchStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.English:
                    stopWords = StopWords.EnglishStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.Estonian:
                    stopWords = null; // *** stop words are missing
                    stemmer = new Lemmatizer(language);
                    break;
                case Language.Finnish:
                    stopWords = StopWords.FinnishStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.French:
                    stopWords = StopWords.FrenchStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.German:
                    stopWords = StopWords.GermanStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.Hungarian:
                    stopWords = StopWords.HungarianStopWords;
                    stemmer = new Lemmatizer(language);
                    break;
                case Language.Italian:
                    stopWords = StopWords.ItalianStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.Norwegian:
                    stopWords = StopWords.NorwegianStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.Portuguese:
                    stopWords = StopWords.PortugueseStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.Romanian:
                    stopWords = StopWords.RomanianStopWords;
                    stemmer = new Lemmatizer(language);
                    break;
                case Language.Russian:
                    stopWords = StopWords.RussianStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.Serbian:
                    stopWords = StopWords.SerbianStopWordsLatin;
                    stemmer = new Lemmatizer(language);
                    break;
                case Language.Slovene:
                    stopWords = StopWords.SloveneStopWords;
                    stemmer = new Lemmatizer(language);
                    break;
                case Language.Spanish:
                    stopWords = StopWords.SpanishStopWords;
                    stemmer = new Stemmer(language);
                    break;
                case Language.Swedish:
                    stopWords = StopWords.SwedishStopWords;
                    stemmer = new Stemmer(language);
                    break;
                default:
                    throw new ArgumentNotSupportedException("language");
            }            
        }

        public static Language GetLanguage(string code)
        {
            if (code == "en") { return Language.English; }
            else if (code == "fr") { return Language.French; }
            else if (code == "de") { return Language.German; }
            else if (code == "es") { return Language.Spanish; }
            else if (code == "da") { return Language.Danish; }
            else if (code == "nl") { return Language.Dutch; }
            else if (code == "fi") { return Language.Finnish; }
            else if (code == "it") { return Language.Italian; }
            else if (code == "no") { return Language.Norwegian; }
            else if (code == "pt") { return Language.Portuguese; }
            else if (code == "sv") { return Language.Swedish; }
            else if (code == "sr") { return Language.Serbian; }
            else if (code == "sl") { return Language.Slovene; }
            else if (code == "ro") { return Language.Romanian; }
            else if (code == "hu") { return Language.Hungarian; }
            else if (code == "et") { return Language.Estonian; }
            else if (code == "bg") { return Language.Bulgarian; }
            else if (code == "cs") { return Language.Czech; }
            else if (code == "ru") { return Language.Russian; }
            else if (code == "el") { return Language.Greek; }
            else if (code == "lt") { return Language.Lithuanian; }
            else if (code == "lv") { return Language.Latvian; }
            else if (code == "mt") { return Language.Maltese; }
            else if (code == "pl") { return Language.Polish; }
            else if (code == "sk") { return Language.Slovak; }
            else if (code == "tr") { return Language.Turkish; }
            else if (code == "vi") { return Language.Vietnamese; }
            else if (code == "is") { return Language.Icelandic; }
            else { return Language.Unspecified; }
        }

        public static string GetLanguageCode(Language language)
        {
            switch (language)
            {
                case Language.Unspecified:
                    return null;
                case Language.English:
                    return "en";
                case Language.French:
                    return "fr";
                case Language.German:
                    return "de";
                case Language.Spanish:
                    return "es";
                case Language.Danish:
                    return "da";
                case Language.Dutch:
                    return "nl";
                case Language.Finnish:
                    return "fi";
                case Language.Italian:
                    return "it";
                case Language.Norwegian:
                    return "no";
                case Language.Portuguese:
                    return "pt";
                case Language.Swedish:
                    return "sv";
                case Language.Serbian:
                    return "sr";
                case Language.Slovene:
                    return "sl";
                case Language.Romanian:
                    return "ro";
                case Language.Hungarian:
                    return "hu";
                case Language.Estonian:
                    return "et";
                case Language.Bulgarian:
                    return "bg";
                case Language.Czech:
                    return "cs";
                case Language.Russian:
                    return "ru";
                case Language.Greek:
                    return "el";
                case Language.Lithuanian:
                    return "lt";
                case Language.Latvian:
                    return "lv";
                case Language.Maltese:
                    return "mt";
                case Language.Polish:
                    return "pl";
                case Language.Slovak:
                    return "sk";
                case Language.Turkish:
                    return "tr";
                case Language.Vietnamese:
                    return "vi";
                case Language.Icelandic:
                    return "is";
                default:
                    throw new ArgumentNotSupportedException("language"); // should not happen
            }
        }

        public static ulong GetSimHash64(SparseVector<double>.ReadOnly vec, ArrayList<string>.ReadOnly vocabulary, double eps)
        {
            // TODO: check parameters
            double[] v = new double[64];
            Array.Clear(v, 0, 64);
            foreach (IdxDat<double> item in vec)
            {
                string word = vocabulary[item.Idx];
                double weight = item.Dat;
                ulong hashCode = Word.GetHashCode64(word);
                for (int i = 0; i < 64; i++)
                {
                    if ((hashCode & (1UL << i)) > 0) { v[i] += weight; }
                    else { v[i] -= weight; }
                }
            }
            ulong fp = 0;
            for (int i = 0; i < 64; i++)
            {
                if (v[i] >= eps) { fp |= (1UL << i); }
            }
            return fp;
        }

        public static ulong GetSimHash64(SparseVector<double>.ReadOnly vec, ArrayList<Word>.ReadOnly vocabulary, double eps)
        {
            // TODO: check parameters
            double[] v = new double[64];
            Array.Clear(v, 0, 64);
            foreach (IdxDat<double> item in vec)
            {
                Word word = vocabulary[item.Idx];
                double weight = item.Dat;
                ulong hashCode = word.GetHashCode64();
                for (int i = 0; i < 64; i++)
                {
                    if ((hashCode & (1UL << i)) > 0) { v[i] += weight; }
                    else { v[i] -= weight; }
                }
            }
            ulong fp = 0;
            for (int i = 0; i < 64; i++)
            {
                if (v[i] >= eps) { fp |= (1UL << i); }
            }
            return fp;
        }

        public static string GetCharRange(string text)
        {
            MultiSet<string> counter = new MultiSet<string>();
            foreach (int ch in text)
            {
                if (ch >= mCharRanges.Count) { counter.Add("Other"); continue; }
                counter.Add(mCharRanges[ch]);
            }
            int max = 0;
            string charRange = "Other";
            foreach (KeyValuePair<string, int> item in counter)
            {
                if (item.Value > max) { max = item.Value; charRange = item.Key; }
            }
            return charRange;
        }
    }
}