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
        private static string tag 
            = @"<(?<startMark>/?)(?<tagName>\w([-_:]?\w)*)((\s+\w([-_:]?\w)*(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)(?<endMark>/?)>";
        private static string number 
            = @"[0-9][\.,0-9]*";
        private static string word 
            = @"\p{L}+";
        private static string comment 
            = "(<!--)|(-->)";
        private static Regex tagRegex
            = new Regex("^" + tag + "$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex numberRegex
            = new Regex("^" + number + "$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex wordRegex
            = new Regex("^" + word + "$", RegexOptions.Compiled | RegexOptions.Singleline);

        private RegexTokenizer mRegexTokenizer
            = new RegexTokenizer();
        private IStemmer mStemmer;
        private bool mNormalize 
            = true;
        private bool mDecodeHtml
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
            mRegexTokenizer.Text = PreprocHtml(text);
            mStemmer = stemmer;
            mRegexTokenizer.TokenRegex = string.Format("({0})|({1})|({2})|({3})", tag, number, word, comment);
            mRegexTokenizer.IgnoreUnknownTokens = true;
            mRegexTokenizer.TokenRegexOptions |= RegexOptions.Singleline;
        }

        private string CleanText(string text)
        {
            text = Regex.Replace(text, tag, " ", RegexOptions.Singleline); // remove tags
            text = Regex.Replace(text.Trim(), @"\s\s+", " "); // compress text
            return text;
        }

        private string PreprocHtml(string html)
        {
            html = Regex.Replace(html, @"<!doctype\s+[^>]*>", "", RegexOptions.IgnoreCase);
            if (mDecodeHtml)
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

        public bool DecodeHtml
        {
            get { return mDecodeHtml; }
            set { mDecodeHtml = value; }
        }

        public static string GetTagName(string tag)
        {
            Utils.ThrowException(tag == null ? new ArgumentNullException("tag") : null);
            Match m = tagRegex.Match(tag);
            if (m.Success)
            {
                return m.Result("${tagName}").ToLower();
            }
            return null;
        }

        public static bool IsTag(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            return tagRegex.Match(token).Success;
        }

        public static bool IsWord(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            return wordRegex.Match(token).Success;
        }

        public static bool IsOpeningTag(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Match m = tagRegex.Match(token);
            if (!m.Success) { return false; }
            return m.Result("${startMark}") != "/";
        }

        public static bool IsClosingTag(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Match m = tagRegex.Match(token);
            if (!m.Success) { return false; }
            return (m.Result("${startMark}") == "/");
        }

        public static bool IsNumber(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            return numberRegex.Match(token).Success;
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
                mRegexTokenizer.Text = PreprocHtml(value);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new Enumerator(mRegexTokenizer, mStemmer, mNormalize);
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
            writer.WriteBool(mDecodeHtml);
            writer.WriteObject(mStemmer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mNormalize = reader.ReadBool();
            mDecodeHtml = reader.ReadBool();
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

            internal Enumerator(RegexTokenizer tokenizer, IStemmer stemmer, bool normalize)
            {
                mTokenizer = tokenizer;
                mEnum = (RegexTokenizer.Enumerator)tokenizer.GetEnumerator();
                mStemmer = stemmer;
                mNormalize = normalize;
            }

            private string NormalizeToken(string token, out string nextToken)
            {
                nextToken = null;
                if (token == "<!--" || token == "-->")
                {
                    return token;
                }
                Match m = null;
                if ((m = tagRegex.Match(token)).Success)
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
                if ((m = numberRegex.Match(token)).Success)
                {
                    return "1";
                }
                if ((m = wordRegex.Match(token)).Success)
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
                if (mEnum.MoveNext())
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