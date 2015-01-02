/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
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
using System.Reflection;

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
        private int mN;
        private ArrayList<LanguageProfile> mLanguageProfiles;

        private static Logger mLogger
            = Logger.GetLogger(typeof(LanguageDetector));

        public LanguageDetector(int n)
        {
            Utils.ThrowException(n <= 0 ? new ArgumentOutOfRangeException("n") : null);
            mN = n;
            mLanguageProfiles = new ArrayList<LanguageProfile>();
        }

        public LanguageDetector() : this(/*n=*/2)
        {
        }

        public LanguageDetector(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public static LanguageDetector GetLanguageDetectorPrebuilt()
        {
            LanguageDetector ld = new LanguageDetector();
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (string resName in assembly.GetManifestResourceNames())
            {
                if (resName.EndsWith(".ldp"))
                {
                    // load language detector profile
                    BinarySerializer ser = new BinarySerializer(assembly.GetManifestResourceStream(resName));
                    LanguageProfile langProfile = new LanguageProfile(ser);
                    ser.Close();
                    ld.AddLanguageProfile(langProfile);
                    mLogger.Debug("GetLanguageDetectorPrebuilt", "Loaded resource {0}.", resName);
                }
            }
            return ld;
        }

        public void AddLanguageProfile(LanguageProfile l)
        {
            Utils.ThrowException(l == null ? new ArgumentNullException("l") : null);
            Utils.ThrowException((!l.IsRanked || l.N != mN) ? new ArgumentValueException("l") : null);
            mLanguageProfiles.Add(l);
        }

        public void BuildProfilesFromCorpus(string dir, int cutOff, Encoding codePage, Encoding loadAs)
        {
            Utils.ThrowException(dir == null ? new ArgumentNullException("dir") : null);
            Utils.ThrowException(!Utils.VerifyFolderName(dir, /*mustExist=*/true) ? new ArgumentValueException("dir") : null);
            Utils.ThrowException(cutOff < 1 ? new ArgumentOutOfRangeException("cutOff") : null);
            mLanguageProfiles.Clear(); 
            string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories);
            Array.Sort(files);
            LanguageProfile langProfile = null;
            Language lastLang = Language.Unspecified;
            foreach (string f in files)
            {
                Language lang;
                string fileLangCode = Path.GetFileName(f).Substring(0, 2).ToLower();
                lang = TextMiningUtils.GetLanguage(fileLangCode);
                // skip file names starting with unknown language code
                if (lang == Language.Unspecified) 
                {
                    mLogger.Warn("BuildProfilesFromCorpus", "Unknown: " + Path.GetFileName(f));
                    continue; 
                }
                if (lang != lastLang)
                {
                    // add new language
                    mLogger.Debug("BuildProfilesFromCorpus", lang + ": " + Path.GetFileName(f));
                    langProfile = new LanguageProfile(mN, lang, codePage);
                    langProfile.AddTokensFromFile(f, loadAs);
                    mLanguageProfiles.Add(langProfile);
                    lastLang = lang;
                }
                else
                {
                    // add corpus file to the last language added
                    mLogger.Debug("BuildProfilesFromCorpus", lang + ": " + Path.GetFileName(f));
                    langProfile.AddTokensFromFile(f, loadAs);
                }
            }
            // ranks n-grams
            // *** n-grams should not be added to languages after this
            foreach (LanguageProfile l in mLanguageProfiles)
            {
                l.Trim(cutOff);
                l.DoRanking(); // throws InvalidOperationException
            }
        }

        public void BuildProfilesFromCorpus(string dir)
        {
            BuildProfilesFromCorpus(dir, /*cutOff=*/500, /*codePage=*/Encoding.UTF8, /*loadAs=*/Encoding.UTF8); // throws ArgumentNullException, ArgumentValueException, InvalidOperationException
        }  

        public LanguageProfile DetectLanguage(NGramProfile p, int cutOff)
        {
            Utils.ThrowException(mLanguageProfiles.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException((!p.IsRanked) ? new ArgumentValueException("p") : null);
            Utils.ThrowException(cutOff < 1 ? new ArgumentOutOfRangeException("cutOff") : null);            
            LanguageProfile matchingLang = null;
            double minDist = Double.MaxValue;
            foreach (LanguageProfile l in mLanguageProfiles)
            {
                double dist = p.CalcOutOfPlace(l, cutOff);
                if (dist < minDist)
                {
                    matchingLang = l;
                    minDist = dist;
                }
            }
            return matchingLang;
        }

        public ArrayList<KeyDat<double, LanguageProfile>> DetectLanguageAll(NGramProfile p, int cutOff)
        {
            Utils.ThrowException(mLanguageProfiles.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException((!p.IsRanked) ? new ArgumentValueException("p") : null);
            Utils.ThrowException(cutOff < 1 ? new ArgumentOutOfRangeException("cutOff") : null);
            ArrayList<KeyDat<double, LanguageProfile>> result = new ArrayList<KeyDat<double, LanguageProfile>>();
            foreach (LanguageProfile l in mLanguageProfiles)
            {
                double dist = p.CalcOutOfPlace(l, cutOff);
                result.Add(new KeyDat<double, LanguageProfile>(dist, l));
            }
            return result;
        }
        
        public LanguageProfile DetectLanguage(NGramProfile p)
        {
            return DetectLanguage(p, /*cutOff=*/500); // throws ArgumentNullException, ArgumentValueException, InvalidOperationException 
        }

        public ArrayList<KeyDat<double, LanguageProfile>> DetectLanguageAll(NGramProfile p)
        {
            return DetectLanguageAll(p, /*cutOff=*/500); // throws ArgumentNullException, ArgumentValueException, InvalidOperationException 
        }

        public LanguageProfile DetectLanguage(string str)
        {
            Utils.ThrowException(mLanguageProfiles.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(str == null ? new ArgumentNullException("str") : null);
            NGramProfile p = new NGramProfile(mN);
            p.AddTokensFromString(str);
            if (p.IsEmpty) { return null; }
            p.DoRanking();
            return DetectLanguage(p);
        }

        public ArrayList<KeyDat<double, LanguageProfile>> DetectLanguageAll(string str)
        {
            Utils.ThrowException(mLanguageProfiles.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(str == null ? new ArgumentNullException("str") : null);
            NGramProfile p = new NGramProfile(mN);
            p.AddTokensFromString(str);
            if (p.IsEmpty) { return null; }
            p.DoRanking();
            return DetectLanguageAll(p);
        }

        public void Clear()
        {
            mLanguageProfiles.Clear();
        }

        public ArrayList<LanguageProfile>.ReadOnly LanguageProfiles
        {
            get { return mLanguageProfiles; }
        }

        public int N
        {
            get { return mN; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (LanguageProfile l in mLanguageProfiles) 
            { 
                sb.AppendLine(l.Language.ToString()); 
            }
            return sb.ToString().TrimEnd();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(mN);
            writer.WriteInt(mLanguageProfiles.Count);
            foreach (LanguageProfile l in mLanguageProfiles) 
            { 
                l.Save(writer); 
            }
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            Clear();
            // the following statements throw serialization-related exceptions
            mN = reader.ReadInt();
            int langCount = reader.ReadInt();
            for (int i = 0; i < langCount; i++) 
            { 
                mLanguageProfiles.Add(new LanguageProfile(reader)); 
            }
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
        private Language mLang;
        private Encoding mCodePage;

        public LanguageProfile(int n, Language lang, Encoding codePage) : base(n) // throws ArgumentOutOfRangeException
        {
            Utils.ThrowException(lang == Language.Unspecified ? new ArgumentValueException("lang") : null);
            mLang = lang;
            mCodePage = codePage;
        }

        public LanguageProfile(int n, Language lang) : this(n, lang, /*codePage=*/null) // throws ArgumentOutOfRangeException, ArgumentValueException
        {
        }

        public LanguageProfile(Language lang) : this(/*n=*/2, lang) // throws ArgumentValueException
        {
        }

        public LanguageProfile(BinarySerializer reader) : base()
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public Language Language
        {
            get { return mLang; }
        }

        public Encoding CodePage
        {
            get { return mCodePage; }
        }

        // *** ISerializable interface implementation ***

        public override void Save(BinarySerializer writer)
        {
            // the following statements throw serialization-related exceptions
            base.Save(writer); // throws ArgumentNullException
            writer.WriteInt((int)mLang);
            writer.WriteString(mCodePage == null ? null : mCodePage.WebName);
        }

        public override void Load(BinarySerializer reader)
        {
            // the following statements throw serialization-related exceptions
            base.Load(reader); // throws ArgumentNullException
            mLang = (Language)reader.ReadInt();
            string codePageName = reader.ReadString();
            mCodePage = codePageName == null ? null : Encoding.GetEncoding(codePageName);
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
        private int mN; 
        private Dictionary<string, uint> mHist;
        private bool mIsRanked
            = false;

        public NGramProfile(int n)
        {
            Utils.ThrowException(n <= 0 ? new ArgumentOutOfRangeException("n") : null);
            mN = n;
            mHist = new Dictionary<string, uint>();
        }

        public NGramProfile() : this(/*n=*/2) 
        {
        }

        public NGramProfile(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public bool IsEmpty
        {
            get { return mHist.Count == 0; }
        }

        public void AddTokensFromString(string str)
        {
            AddTokensFromString(str, TokenizerType.AlphaOnly);
        }

        public void AddTokensFromString(string str, TokenizerType tokType)
        {
            Utils.ThrowException(str == null ? new ArgumentNullException("str") : null);
            Utils.ThrowException(mIsRanked ? new InvalidOperationException() : null);
            SimpleTokenizer tokenizer = new SimpleTokenizer();
            tokenizer.Type = tokType;
            foreach (string token in tokenizer.GetTokens(str))
            {
                AddToken(token.ToUpper());
            }
        }

        public void AddTokensFromFile(string file, Encoding loadAs)
        {
            AddTokensFromFile(file, TokenizerType.AlphaOnly, loadAs);
        }

        public void AddTokensFromFile(string file, TokenizerType tokType, Encoding loadAs)
        {
            Utils.ThrowException(!Utils.VerifyFileNameOpen(file) ? new ArgumentValueException("file") : null);
            Utils.ThrowException(mIsRanked ? new InvalidOperationException() : null);
            StreamReader reader = new StreamReader(file, loadAs);
            SimpleTokenizer tokenizer = new SimpleTokenizer();
            tokenizer.Type = tokType;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (string token in tokenizer.GetTokens(line)) 
                { 
                    AddToken(token.ToUpper()); 
                }
            }
            reader.Close();
        }

        public void AddToken(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Utils.ThrowException(mIsRanked ? new InvalidOperationException() : null);
            token = token.Trim();
            string paddedToken = token;
            for (int i = 1; i <= mN; i++)
            {
                paddedToken = paddedToken.PadRight(paddedToken.Length + (i - 1), '_');
                int pos = 0;
                int end = paddedToken.Length - i + 1;
                uint v = 0;
                while (pos < end)
                {
                    string ngram = paddedToken.Substring(pos++, i);
                    if (mHist.TryGetValue(ngram, out v)) { mHist[ngram] = v + 1; }
                    else { mHist[ngram] = 1; }
                }
                paddedToken = token.PadLeft(token.Length + 1, '_');
            }
        }

        public void DoRanking()
        {
            Utils.ThrowException(mHist.Count == 0 ? new InvalidOperationException() : null);
            Utils.ThrowException(mIsRanked ? new InvalidOperationException() : null);
            // copy (key, value) pairs into list and sort them descendingly by value
            List<KeyValuePair<string, uint>> l = new List<KeyValuePair<string, uint>>(mHist);
            l.Sort(delegate(KeyValuePair<string, uint> pair1, KeyValuePair<string, uint> pair2) {
                if (pair1.Value > pair2.Value) { return -1; }
                if (pair1.Value < pair2.Value) { return 1; }
                return 0;
            });
            // assign rank to each n-gram
            uint count = 1;
            uint lastRank = 0;
            uint lastValue = 0;
            foreach (KeyValuePair<string, uint> kvp in l)
            {
                if (kvp.Value != lastValue)
                {
                    mHist[kvp.Key] = count;
                    lastRank = count;
                }
                else { mHist[kvp.Key] = lastRank; }
                lastValue = kvp.Value;
                count++;
            }
            mIsRanked = true; // indicates that ranks have been assigned
        }

        public void Trim(int cutOff)
        {
            Utils.ThrowException(cutOff < 1 ? new ArgumentOutOfRangeException("cutOff") : null);
            Utils.ThrowException(mIsRanked ? new InvalidOperationException() : null);
            if (mHist.Count <= cutOff) { return; }
            ArrayList<KeyDat<uint, string>> aux = new ArrayList<KeyDat<uint, string>>(mHist.Count);
            foreach (KeyValuePair<string, uint> item in mHist)
            {
                aux.Add(new KeyDat<uint, string>(item.Value, item.Key));
            }
            aux.Sort(DescSort<KeyDat<uint, string>>.Instance);
            aux.RemoveRange(cutOff, mHist.Count - cutOff);
            mHist.Clear();
            foreach (KeyDat<uint, string> item in aux)
            {
                mHist.Add(item.Dat, item.Key);
            }
        }

        public uint CalcOutOfPlace(NGramProfile p, int cutOff)
        {            
            Utils.ThrowException(p == null ? new ArgumentNullException("p") : null);
            Utils.ThrowException(cutOff < 1 ? new ArgumentOutOfRangeException("cutOff") : null);
            Utils.ThrowException(!mIsRanked ? new InvalidOperationException() : null);
            int dist = 0;
            uint v = 0;
            uint sum = 0;
            int penalty = Math.Min(cutOff, p.mHist.Count);
            // sum up distances from each n-gram in this profile to the same n-gram in profile p
            foreach (KeyValuePair<string, uint> kvp in mHist)
            {
                if (kvp.Value > cutOff) { continue; }
                if (p.mHist.TryGetValue(kvp.Key, out v))
                {
                    if (v > cutOff) { dist = penalty; }
                    else { dist = (int)Math.Abs(v - kvp.Value); }
                }
                else { dist = penalty; }
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
            get { return mN; } 
        }

        public bool IsRanked
        {
            get { return mIsRanked; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(mHist.Count * 5);
            foreach (KeyValuePair<string, uint> kvp in mHist) 
            { 
                sb.AppendLine(kvp.Key + "\t" + kvp.Value); 
            }
            return sb.ToString().TrimEnd();
        }

        // *** ISerializable interface implementation ***

        public virtual void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(mN);
            writer.WriteBool(mIsRanked);
            writer.WriteInt(mHist.Count);
            foreach (KeyValuePair<string, uint> kvp in mHist)
            {
                writer.WriteString(kvp.Key);
                writer.WriteUInt(kvp.Value);
            }
        }

        public virtual void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mN = reader.ReadInt();
            mIsRanked = reader.ReadBool();
            int count = reader.ReadInt();
            mHist = new Dictionary<string, uint>(count);
            for (int i = 0; i < count; i++) 
            { 
                mHist.Add(reader.ReadString(), reader.ReadUInt()); 
            }
        }
    }
}