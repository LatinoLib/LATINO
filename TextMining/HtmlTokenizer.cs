/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    HtmlTokenizer.cs
 *  Desc:    Tokenizer for HTML documents 
 *  Created: Jul-2010
 *
 *  Authors: Miha Grcar, Marko Brakus
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class HtmlTokenizer
       |
       '-----------------------------------------------------------------------
    */
    public class HtmlTokenizer : ITokenizer
    {
        private static string mTagRegexStr 
            = @"<(?<startMark>/?)(?<tagName>\w([-_:]?\w)*)((\s+\w([-_:]?\w)*(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)(?<endMark>/?)>";
        private static string mNumberRegexStr 
            = @"[0-9][\.,0-9]*";
        private static string mWordRegexStr 
            = @"\p{L}+";
        private static string mCommentRegexStr 
            = @"(<!--)|(-->)";
        private static string mCharRefRegexStr
            = @"&((#\d+)|(#[xX][0-9A-Fa-f]+)|([a-zA-Z0-9]+));";
        private static Regex mTagRegex
            = new Regex("^" + mTagRegexStr + "$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex mNumberRegex
            = new Regex("^" + mNumberRegexStr + "$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex mWordRegex
            = new Regex("^" + mWordRegexStr + "$", RegexOptions.Compiled | RegexOptions.Singleline);
        public/*private*/ static Regex mCharRefRegex
            = new Regex("^" + mCharRefRegexStr + "$", RegexOptions.Compiled | RegexOptions.Singleline);

        private RegexTokenizer mRegexTokenizer
            = new RegexTokenizer();
        private IStemmer mStemmer;
        private bool mNormalize 
            = true;
        private bool mPreprocessHtml
            = true;
        private bool mSkipCharRefs
            = true;

        public HtmlTokenizer() : this(/*stemmer=*/null, "")
        {
        }
        
        public HtmlTokenizer(IStemmer stemmer) : this(stemmer, "")
        { 
        }

        public HtmlTokenizer(IStemmer stemmer, string text)
        {
            Utils.ThrowException(text == null ? new ArgumentNullException("text") : null);
            mRegexTokenizer.Text = TranscodeHtml(text);
            mStemmer = stemmer;
            mRegexTokenizer.TokenRegex = string.Format("({0})|({1})|({2})|({3})|({4})", mTagRegexStr, mNumberRegexStr, mWordRegexStr, mCommentRegexStr, mCharRefRegexStr);
            mRegexTokenizer.IgnoreUnknownTokens = true;
            mRegexTokenizer.TokenRegexOptions |= RegexOptions.Singleline;
        }

        private string CleanText(string text)
        {
            text = Regex.Replace(text, mTagRegexStr, " ", RegexOptions.Singleline); // remove tags
            text = Regex.Replace(text.Trim(), @"\s\s+", " "); // compress text
            return text;
        }

        private string TranscodeHtml(string html)
        {
            html = Regex.Replace(html, @"<!doctype\s+[^>]*>", "", RegexOptions.IgnoreCase);
            if (mPreprocessHtml)
            {
                html = html.Replace("&lt;", " ");
                html = html.Replace("&gt;", " ");
                html = HttpUtility.HtmlDecode(html);
            }
            return html;
        }

        public bool Normalize
        {
            get { return mNormalize; }
            set { mNormalize = value; }
        }

        public bool PreprocessHtml
        {
            get { return mPreprocessHtml; }
            set { mPreprocessHtml = value; }
        }

        public bool SkipCharRefs
        {
            get { return mSkipCharRefs; }
            set { mSkipCharRefs = value; }
        }

        public static string GetTagName(string tag)
        {
            Utils.ThrowException(tag == null ? new ArgumentNullException("tag") : null);
            Match m = mTagRegex.Match(tag);
            if (m.Success)
            {
                return m.Result("${tagName}").ToLower();
            }
            return null;
        }

        public static bool IsTag(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            return mTagRegex.Match(token).Success;
        }

        public static bool IsWord(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            return mWordRegex.Match(token).Success;
        }

        public static bool IsOpeningTag(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Match m = mTagRegex.Match(token);
            if (!m.Success) { return false; }
            return m.Result("${startMark}") != "/";
        }

        public static bool IsClosingTag(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Match m = mTagRegex.Match(token);
            if (!m.Success) { return false; }
            return (m.Result("${startMark}") == "/");
        }

        public static bool IsNumber(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            return mNumberRegex.Match(token).Success;
        }

        public static bool IsCharRef(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            return mCharRefRegex.Match(token).Success;
        }

        public string GetTextBlock(int startIdx, int len)
        {
            Utils.ThrowException((startIdx < 0 || startIdx >= mRegexTokenizer.Text.Length) ? new ArgumentOutOfRangeException("startIdx") : null);
            Utils.ThrowException((len < 0 || startIdx + len > mRegexTokenizer.Text.Length) ? new ArgumentOutOfRangeException("len") : null);
            return mRegexTokenizer.Text.Substring(startIdx, len);
        }
       
        public string GetTextBlockCleaned(int startIdx, int len)
        {
            Utils.ThrowException((startIdx < 0 || startIdx >= mRegexTokenizer.Text.Length) ? new ArgumentOutOfRangeException("startIdx") : null);
            Utils.ThrowException((len < 0 || startIdx + len > mRegexTokenizer.Text.Length) ? new ArgumentOutOfRangeException("len") : null);
            return CleanText(mRegexTokenizer.Text.Substring(startIdx, len));
        }

        // *** ITokenizer interface implementation ***

        public string Text
        {
            get { return mRegexTokenizer.Text; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Text") : null);
                mRegexTokenizer.Text = TranscodeHtml(value);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new Enumerator(mRegexTokenizer, mStemmer, mNormalize, mSkipCharRefs);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer) // *** current state and text are not saved
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteBool(mNormalize);
            writer.WriteBool(mPreprocessHtml);
            writer.WriteObject(mStemmer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mNormalize = reader.ReadBool();
            mPreprocessHtml = reader.ReadBool();
            mStemmer = reader.ReadObject<IStemmer>();
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Enumerator
           |
           '-----------------------------------------------------------------------
        */
        public class Enumerator : IEnumerator<string>
        {
            private RegexTokenizer mTokenizer;
            private RegexTokenizer.Enumerator mEnum;
            private IStemmer mStemmer;
            private bool mNormalize;
            private string mCurrentToken 
                = null;
            private int mCurrentTokenIdx
                = -1;
            private int mCurrentTokenLen
                = -1;
            private string mNextToken
                = null;
            private bool mSkipCharRefs;

            internal Enumerator(RegexTokenizer tokenizer, IStemmer stemmer, bool normalize, bool skipCharRefs)
            {
                mTokenizer = tokenizer;
                mEnum = (RegexTokenizer.Enumerator)tokenizer.GetEnumerator();
                mStemmer = stemmer;
                mNormalize = normalize;
                mSkipCharRefs = skipCharRefs;
            }

            private string NormalizeToken(string token, out string nextToken)
            {
                nextToken = null;
                if (token == "<!--" || token == "-->" || mCharRefRegex.Match(token).Success)
                {
                    return token;
                }
                Match m = null;
                if ((m = mTagRegex.Match(token)).Success)
                {
                    if (m.Result("${startMark}") == "/")
                    {
                        return "</" + m.Result("${tagName}").ToLower() + ">";
                    }
                    if (m.Result("${endMark}") == "/")
                    {
                        nextToken = "</" + m.Result("${tagName}").ToLower() + ">";
                        return "<" + m.Result("${tagName}").ToLower() + ">";                        
                    }
                    return "<" + m.Result("${tagName}").ToLower() + ">";
                }
                if ((m = mNumberRegex.Match(token)).Success)
                {
                    return "1";
                }
                if ((m = mWordRegex.Match(token)).Success)
                {
                    return mStemmer == null ? token : mStemmer.GetStem(token.ToLower());
                }
                return null; // *** we should not get here
            }

            public int CurrentTokenIdx
            {
                get
                {
                    Utils.ThrowException(mCurrentToken == null ? new InvalidOperationException() : null);
                    return mCurrentTokenIdx;
                }
            }

            public int CurrentTokenLen
            {
                get
                {
                    Utils.ThrowException(mCurrentToken == null ? new InvalidOperationException() : null);
                    return mCurrentTokenLen;
                }
            }

            // *** IEnumerator<string> interface implementation ***

            public string Current
            {
                get 
                {
                    Utils.ThrowException(mCurrentToken == null ? new InvalidOperationException() : null);
                    return mCurrentToken;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; } // throws InvalidOperationException
            }

            public bool MoveNext()
            {
                if (mNextToken != null)
                {
                    mCurrentToken = mNextToken;
                    mNextToken = null;
                    return true;
                }
                bool gotNext = mEnum.MoveNext();
                if (mSkipCharRefs) { while (gotNext && mCharRefRegex.Match(mEnum.Current).Success) { gotNext = mEnum.MoveNext(); } }
                if (gotNext)
                {
                    mCurrentToken = mEnum.Current;
                    mCurrentTokenIdx = mEnum.CurrentTokenIdx;
                    mCurrentTokenLen = mCurrentToken.Length;
                    if (mNormalize) { mCurrentToken = NormalizeToken(mCurrentToken, out mNextToken); }
                    return true;
                }
                else
                {
                    Reset();
                    return false;
                }
            }

            public void Reset()
            {
                mEnum.Reset();
                mCurrentToken = null;
                mCurrentTokenIdx = -1;
                mCurrentTokenLen = -1;
                mNextToken = null;
            }

            public void Dispose()
            {
            }
        }
    }
}