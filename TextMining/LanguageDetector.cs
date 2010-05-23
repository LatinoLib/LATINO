/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    LanguageDetector.cs
 *  Desc:    Language detector based on character n-grams
 *  Created: Mar-2010
 *
 *  Authors: Marko Brakus
 *
 ***************************************************************************/

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class LanguageDetector
       |
       '-----------------------------------------------------------------------
    */
    public class LanguageDetector
    {
        private ArrayList<LanguageProfile> languageProfiles
            = new ArrayList<LanguageProfile>();

        public void ReadCorpus(string dir)
        {
            Utils.ThrowException(!Utils.VerifyPathName(dir, /*mustExist=*/true) ? new ArgumentValueException("dir") : null);

            if (languageProfiles != null)
                languageProfiles.Clear();

            string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories);
            Array.Sort(files);

            LanguageProfile lang = new LanguageProfile(" ");
            string lastLangCode = "";
            foreach (string f in files)
            {
                string fileLangCode = Path.GetFileName(f).Substring(0, 2).ToLower();
                if (fileLangCode.Equals(lastLangCode) == false)
                {
                    // adds new language
                    Console.WriteLine(fileLangCode + ":\t" + Path.GetFileName(f));
                    lang = new LanguageProfile(fileLangCode, f);
                    languageProfiles.Add(lang);
                    lastLangCode = fileLangCode;
                }
                else
                {
                    // adds corpus file to the last language added
                    Console.WriteLine(fileLangCode + ":\t" + Path.GetFileName(f));
                    lang.AddTokensFromFile(f);
                }
            }

            foreach (LanguageProfile p in languageProfiles)
            {
                p.DoRanking();
            }
        }

        private void AddLanguageProfile(LanguageProfile l)
        {
            Utils.ThrowException(l == null ? new ArgumentNullException("l") : null);
            Utils.ThrowException(!l.IsRanked ? new ArgumentValueException("l") : null);
            languageProfiles.Add(l);
        }

        /*  FindMatchingLanguage
         * 
         *  Returns language which best matches given profile 'p'.
         *  
         */
        public LanguageProfile FindMatchingLanguage(NGramProfile p)
        {
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException(!p.IsRanked ? new ArgumentValueException("p") : null);
            Utils.ThrowException(languageProfiles.Count == 0 ? new InvalidOperationException() : null);
            // first finds language most similar to the profile 'p'
            LanguageProfile matchingLang = null;
            double maxSimilarity = -Double.MaxValue;
            double sim;
            foreach (LanguageProfile l in languageProfiles)
            {
                sim = p.CalcSimilarity(l, NGramProfile.Similarity.Spearman);
                if (sim > maxSimilarity)
                {
                    matchingLang = l;
                    maxSimilarity = sim;
                }
            }

            return matchingLang;
        }

        /*  FindMatchingLanguage
         * 
         *  Returns language which best matches profile of the given string.
         *  
         */
        public LanguageProfile FindMatchingLanguage(string str)
        {
            Utils.ThrowException(languageProfiles.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(str == null ? new ArgumentNullException("str") : null);
            NGramProfile p = new NGramProfile(/*n=*/3);
            p.AddTokensFromString(str);
            p.DoRanking();
            return FindMatchingLanguage(p);
        }

        /*  GetLanguage
         * 
         *  Returns language with code 'langCode'.
         * 
         */
        public LanguageProfile GetLanguageProfile(string langCode)
        {
            return languageProfiles.Find(delegate(LanguageProfile lang)
                {
                    return lang.Code.ToLower().Equals(langCode.ToLower());
                });
        }

        /*  Exists
         * 
         *  Checks if it contains language with code 'langCode'
         * 
         */
        public bool Exists(string langCode)
        {
            return GetLanguageProfile(langCode) != null;
        }

        public ArrayList<LanguageProfile>.ReadOnly LanguageProfiles
        {
            get { return languageProfiles; }
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class LanguageProfile
       |
       '-----------------------------------------------------------------------
    */
    public class LanguageProfile : NGramProfile
    {
        private string code;

        public LanguageProfile(string code, string corpusFileName) : base(/*n=*/3)
        {
            Utils.ThrowException(code == null ? new ArgumentNullException("code") : null);
            this.code = code;
            AddTokensFromFile(corpusFileName); // throws ArgumentValueException
        }

        public LanguageProfile(string code) : base(/*n=*/3)
        {
            Utils.ThrowException(code == null ? new ArgumentNullException("code") : null);
            this.code = code;
        }

        public string Code
        {
            get { return code; }
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class NGramProfile
       |
       '-----------------------------------------------------------------------
    */
    public class NGramProfile
    {
        public enum Similarity 
        { 
            Rank = 1, 
            Spearman 
        };

        private int n;
        private Dictionary<string, uint> hist
            = new Dictionary<string, uint>();        
        private static Regex digitRegex
            = new Regex("[0-9]", RegexOptions.Compiled);

        public NGramProfile(int n)
        {
            Utils.ThrowException(n <= 0 ? new ArgumentOutOfRangeException("n") : null);
            this.n = n;
        }

        /*  AddTokens
         * 
         *  Adds all tokens from the 'tokenizer' to the profile
         * 
         */
        public void AddTokens(ITokenizer tokenizer)
        {
            Utils.ThrowException(tokenizer == null ? new ArgumentNullException("tokenizer") : null);
            Utils.ThrowException(n < 0 ? new InvalidOperationException() : null);
            foreach (string token in tokenizer)
                if (!digitRegex.Match(token).Success)
                    AddToken(token.ToUpper());
        }

        /*  AddTokensFromString
         *
         *  Tokenizes the string and adds tokens to the profile
         *
         */
        public void AddTokensFromString(string str)
        {
            Utils.ThrowException(str == null ? new ArgumentNullException("str") : null);
            Utils.ThrowException(n < 0 ? new InvalidOperationException() : null);
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            tokenizer.TokenRegex = @"\w[\w']+";
            tokenizer.Text = str;
            AddTokens(tokenizer);
        }

        /*  AddTokensFromFile
         *
         *  Reads 'file', tokenizes it and adds tokens to the profile
         *
         */
        public void AddTokensFromFile(string file)
        {
            Utils.ThrowException(!Utils.VerifyFileNameOpen(file) ? new ArgumentValueException("file") : null);
            Utils.ThrowException(n < 0 ? new InvalidOperationException() : null);
            StreamReader reader = new StreamReader(file);
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            tokenizer.TokenRegex = @"\w[\w']+";
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                tokenizer.Text = line;
                foreach (string token in tokenizer)
                    if (!digitRegex.Match(token).Success)
                        AddToken(token.ToUpper());
            }
            reader.Close();
        }

        /*  AddToken
         *
         *  Adds one token (word) to the profile. The 'token' is broken into character N-grams, which are then
         *  added to the profile.
         * 
         */
        public void AddToken(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Utils.ThrowException(n < 0 ? new InvalidOperationException() : null);
            token = token.Trim();
            string paddedToken = token;
            for (int i = 1; i <= n; i++)
            {
                paddedToken = paddedToken.PadRight(paddedToken.Length + (i - 1), '_');
                int pos = 0;
                int end = paddedToken.Length - i + 1;
                uint v = 0;
                while (pos < end)
                {
                    string ngram = paddedToken.Substring(pos++, i);
                    if (hist.TryGetValue(ngram, out v))
                        hist[ngram] = v + 1;
                    else
                        hist[ngram] = 1;
                }
                paddedToken = token.PadLeft(token.Length + 1, '_');
            }
        }

        /*  DoRanking
         *
         *  Assigns rank to each n-gram. N-gram with the highest frequency
         *  gets rank 1, second one 2 and so on.
         *  This method should be called only once after the profile has 
         *  been completely built.
         * 
         */
        public void DoRanking()
        {
            Utils.ThrowException(hist.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(n < 0 ? new InvalidOperationException() : null);
            // copies (key,value) pairs into a list and sorts them descendingly by the value
            List<KeyValuePair<string, uint>> l = new List<KeyValuePair<string, uint>>(hist);
            l.Sort(delegate(KeyValuePair<string, uint> pair1, KeyValuePair<string, uint> pair2)
                {
                    if (pair1.Value > pair2.Value)
                        return -1;
                    if (pair1.Value < pair2.Value)
                        return 1;
                    return 0;
                });

            // assigns rank to each n-gram
            uint count = 1;
            uint lastRank = 0;
            uint lastValue = 0;
            foreach (KeyValuePair<string, uint> kvp in l)
            {
                if (kvp.Value != lastValue)
                {
                    hist[kvp.Key] = count;
                    lastRank = count;
                }
                else
                    hist[kvp.Key] = lastRank;
                lastValue = kvp.Value;
                count++;
            }
            n = -1; // indicates that ranks have been assigned
        }

        public bool IsRanked
        {
            get { return n < 0; }
        }

        /*  CalcSimilarity
         * 
         *  Calculates similarity (of given type 'sim') between this profile 
         *  and given profile 'p'.
         * 
         */
        public double CalcSimilarity(NGramProfile p, Similarity sim)
        {
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException(n > 0 ? new InvalidOperationException() : null);
            switch (sim)
            {
                case Similarity.Rank:
                    return CalcRankDistance(p);

                case Similarity.Spearman:
                    return CalcSpearman(p);
            }
            return CalcSpearman(p);
        }

        /*  CalcRankDistance
         *
         *  Calculates distance between this profile and given profile 'p'.
         *  Returned value is in [0,inf).
         * 
         */
        public uint CalcRankDistance(NGramProfile p)
        {
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException(n > 0 ? new InvalidOperationException() : null);
            uint dist = 0;
            uint v = 0;
            uint sum = 0;
            // sums up distances of each N-gram in this profile to 
            // the same N-gram in profile 'p'
            foreach (KeyValuePair<string, uint> kvp in hist)
            {
                if (p.hist.TryGetValue(kvp.Key, out v))
                    dist = (uint)Math.Abs(v - kvp.Value);
                else
                    dist = (uint)hist.Count;
                sum += dist;
            }
            return sum;
        }

        /* CalcSpearman
        * 
        * Calculates Spearman correlation between this ranked profile 
        * and given ranked profile 'p'. Returned value is in [-1,1]
        *
        */
        public double CalcSpearman(NGramProfile p)
        {
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException(n > 0 ? new InvalidOperationException() : null);
            double mean1 = 0.0;
            double mean2 = 0.0;
            uint v = 0;
            foreach (KeyValuePair<string, uint> kvp in hist)
            {
                mean1 += kvp.Value;
                if (p.hist.TryGetValue(kvp.Key, out v))
                    mean2 += v;
                else
                    mean2 += (uint)hist.Count;
            }
            mean1 /= hist.Count;
            mean2 /= hist.Count;

            double diff1 = 0.0;
            double diff2 = 0.0;
            double upperSum = 0.0;
            double sqDiff1 = 0.0;
            double sqDiff2 = 0.0;
            foreach (KeyValuePair<string, uint> kvp in hist)
            {
                diff1 = kvp.Value - mean1;
                sqDiff1 += diff1 * diff1;
                if (p.hist.TryGetValue(kvp.Key, out v))
                    diff2 = v - mean2;
                else
                    diff2 = hist.Count - mean2;
                sqDiff2 += diff2 * diff2;
                upperSum += diff1 * diff2;
            }

            // calculates and returns Pearson's correlation coefficient
            return upperSum / Math.Sqrt(sqDiff1 * sqDiff2);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(hist.Count * 5);
            sb.AppendLine("N-gram\tValue");
            foreach (KeyValuePair<string, uint> kvp in hist)
                sb.AppendLine(kvp.Key + "\t" + kvp.Value);
            return sb.ToString();
        }
    }
}