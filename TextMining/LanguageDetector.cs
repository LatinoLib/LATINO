/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    LanguageDetector.cs
 *  Desc:    Language detector based on character n-grams
 *  Created: Mar-2010
 *
 *  Authors: Marko Brakus, Miha Grcar
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
    public class LanguageDetector : ISerializable
    {
        private int n;
        private ArrayList<LanguageProfile> languageProfiles;

        public LanguageDetector(int n)
        {
            Utils.ThrowException(n <= 0 ? new ArgumentOutOfRangeException("n") : null);
            this.n = n;
            languageProfiles = new ArrayList<LanguageProfile>();
        }

        public LanguageDetector(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public void AddLanguageProfile(LanguageProfile l)
        {
            Utils.ThrowException(l == null ? new ArgumentNullException("l") : null);
            Utils.ThrowException((!l.IsRanked || l.N != n) ? new ArgumentValueException("l") : null);
            languageProfiles.Add(l);
        }

        public LanguageProfile FindMatchingLanguage(NGramProfile p, int cutOff)
        {
            Utils.ThrowException(languageProfiles.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException((!p.IsRanked || p.N != n) ? new ArgumentValueException("p") : null);
            Utils.ThrowException(cutOff < 1 ? new ArgumentOutOfRangeException("cutOff") : null);            
            // finds language most similar to the profile 'p'
            LanguageProfile matchingLang = null;
            double minDist = Double.MaxValue;
            double dist;
            foreach (LanguageProfile l in languageProfiles)
            {
                dist = p.CalcOutOfPlace(l, cutOff);
                if (dist < minDist)
                {
                    matchingLang = l;
                    minDist = dist;
                }
            }

            return matchingLang;
        }
        
        public LanguageProfile FindMatchingLanguage(NGramProfile p)
        {
            return FindMatchingLanguage(p, /*cutOff=*/500); // throws ArgumentNullException, ArgumentValueException, InvalidOperationException 
        }

        public LanguageProfile FindMatchingLanguage(string str)
        {
            Utils.ThrowException(languageProfiles.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(str == null ? new ArgumentNullException("str") : null);
            NGramProfile p = new NGramProfile(n);
            p.AddTokens(str);
            p.DoRanking();
            return FindMatchingLanguage(p);
        }

        public void Clear()
        {
            languageProfiles.Clear();
        }

        public ArrayList<LanguageProfile>.ReadOnly LanguageProfiles
        {
            get { return languageProfiles; }
        }

        public int N
        {
            get { return n; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Languages:");
            foreach (LanguageProfile l in languageProfiles)
                sb.AppendLine(l.Code);
            return sb.ToString();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(n);
            writer.WriteInt(languageProfiles.Count);
            foreach (LanguageProfile l in languageProfiles)
                l.Save(writer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            Clear();
            // the following statements throw serialization-related exceptions
            n = reader.ReadInt();
            int langCount = reader.ReadInt();
            for (int i = 0; i < langCount; i++)
                languageProfiles.Add(new LanguageProfile(reader));
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

        public LanguageProfile(int n, string code) : base(n) // throws ArgumentOutOfRangeException
        {
            Utils.ThrowException(code == null ? new ArgumentNullException("code") : null);
            this.code = code;
        }

        public LanguageProfile(string code) : this(/*n=*/2, code) // throws ArgumentNullException
        {
        }

        public LanguageProfile(BinarySerializer reader) : base(/*n=*/1)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public string Code
        {
            get { return code; }
        }

        // *** ISerializable interface implementation ***

        public override void Save(BinarySerializer writer)
        {
            // the following statements throw serialization-related exceptions
            base.Save(writer); // throws ArgumentNullException
            writer.WriteString(code);
        }

        public override void Load(BinarySerializer reader)
        {
            // the following statements throw serialization-related exceptions
            base.Load(reader); // throws ArgumentNullException
            code = reader.ReadString();
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class NGramProfile
       |
       '-----------------------------------------------------------------------
    */
    public class NGramProfile : ISerializable
    {
        private int n; 
        private Dictionary<string, uint> hist;
        private bool isRanked
            = false;
        private static Regex digitRegex
            = new Regex("[0-9]", RegexOptions.Compiled);

        public NGramProfile(int n)
        {
            Utils.ThrowException(n <= 0 ? new ArgumentOutOfRangeException("n") : null);
            this.n = n;
            hist = new Dictionary<string, uint>();
        }

        public NGramProfile() : this(/*n=*/2) 
        {
        }

        public NGramProfile(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public void AddTokens(ITokenizer tokenizer)
        {
            Utils.ThrowException(tokenizer == null ? new ArgumentNullException("tokenizer") : null);
            Utils.ThrowException(isRanked ? new InvalidOperationException() : null);
            foreach (string token in tokenizer)
                if (!digitRegex.Match(token).Success)
                    AddToken(token.ToUpper());
        }

        public void AddTokens(string str)
        {
            Utils.ThrowException(str == null ? new ArgumentNullException("str") : null);
            Utils.ThrowException(isRanked ? new InvalidOperationException() : null);
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            tokenizer.TokenRegex = @"[^\s][^\s]+";
            tokenizer.Text = str;
            AddTokens(tokenizer);
        }

        public void AddTokensFromFile(string file)
        {
            Utils.ThrowException(!Utils.VerifyFileNameOpen(file) ? new ArgumentValueException("file") : null);
            Utils.ThrowException(isRanked ? new InvalidOperationException() : null);
            StreamReader reader = Utils.CheckUnicodeSignature(file) ? new StreamReader(file) : new StreamReader(file, Encoding.GetEncoding("ISO-8859-1"));
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            tokenizer.TokenRegex = @"[^\s][^\s]+";
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

        public void AddToken(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Utils.ThrowException(isRanked ? new InvalidOperationException() : null);
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

        public void DoRanking()
        {
            Utils.ThrowException(hist.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(isRanked ? new InvalidOperationException() : null);
            // copies (key,value) pairs into a list and sorts them descendingly by the value
            List<KeyValuePair<string, uint>> l = new List<KeyValuePair<string, uint>>(hist);
            l.Sort(delegate(KeyValuePair<string, uint> pair1,
                    KeyValuePair<string, uint> pair2)
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
            isRanked = true; // indicates that ranks have been assigned
        }

        public uint CalcOutOfPlace(NGramProfile p, int cutOff)
        {            
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException(cutOff < 1 ? new ArgumentOutOfRangeException("cutOff") : null);
            Utils.ThrowException(!isRanked ? new InvalidOperationException() : null);
            int dist = 0;
            uint v = 0;
            uint sum = 0;
            int penalty = Math.Min(cutOff, p.hist.Count);
            // sums up distances of each N-gram in this profile to 
            // the same N-gram in profile 'p'
            foreach (KeyValuePair<string, uint> kvp in hist)
            {
                if (kvp.Value > cutOff)
                    continue;
                if (p.hist.TryGetValue(kvp.Key, out v))
                    if (v > cutOff)
                        dist = penalty;
                    else
                        dist = (int)Math.Abs(v - kvp.Value);
                else
                    dist = penalty;
                sum += (uint)dist;
            }
            return sum;
        }

        public uint CalcOutOfPlace(NGramProfile p)
        {
            return CalcOutOfPlace(p, /*cutOff=*/500); // throws ArgumentNullException, ArgumentOutOfRangeException, InvalidOperationException
        }

        public int N
        { 
            get { return n; } 
        }

        public bool IsRanked
        {
            get { return isRanked; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(hist.Count * 5);
            sb.AppendLine("N-gram, value:");
            foreach (KeyValuePair<string, uint> kvp in hist)
                sb.AppendLine(kvp.Key + "\t" + kvp.Value);
            return sb.ToString();
        }

        // *** ISerializable interface implementation ***

        public virtual void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(n);
            writer.WriteBool(isRanked);
            writer.WriteInt(hist.Count);
            foreach (KeyValuePair<string, uint> kvp in hist)
            {
                writer.WriteString(kvp.Key);
                writer.WriteUInt(kvp.Value);
            }
        }

        public virtual void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            n = reader.ReadInt();
            isRanked = reader.ReadBool();
            int count = reader.ReadInt();
            hist = new Dictionary<string, uint>(count);
            for (int i = 0; i < count; i++)
                hist.Add(reader.ReadString(), reader.ReadUInt());
        }
    }
}