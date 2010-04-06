/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          TextMiningUtils.cs
 *  Version:       1.0
 *  Desc:		   Text mining utilities
 *  Author:        Miha Grcar
 *  Created on:    Mar-2010
 *  Last modified: Apr-2010
 *  Revision:      Mar-2010
 *
 ***************************************************************************/

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
                //case Language.Estonian:
                //    stopWords = null; // *** missing stop words
                //    stemmer = new Lemmatizer(language);
                //    break;
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
    }
}