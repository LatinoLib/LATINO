/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Tokenizer.cs
 *  Desc:    Text tokenizer based on regular expressions
 *  Created: Apr-2009
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class RegexTokenizer
       |
       '-----------------------------------------------------------------------  
    */
    public class RegexTokenizer : ITokenizer
    {
        private Regex mTokenRegex
            = new Regex(@"\p{L}+(-\p{L}+)*", RegexOptions.Compiled);
        private Regex mDelimRegex
            = new Regex(@"\s+|$", RegexOptions.Compiled); // *** this is not (yet?) publicly accessible
        private bool mIgnoreUnknownTokens
            = false;
        private RegexOptions mRegexOptions
            = RegexOptions.Compiled;

        public RegexTokenizer()
        {
        }

        public RegexTokenizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public string TokenRegex
        { 
            get { return mTokenRegex.ToString(); }
            set { mTokenRegex = new Regex(value, mRegexOptions); } // throws ArgumentNullException, ArgumentException
        }

        public bool IgnoreUnknownTokens
        {
            get { return mIgnoreUnknownTokens; }
            set { mIgnoreUnknownTokens = value; }
        }

        public RegexOptions TokenRegexOptions
        {
            get { return mRegexOptions; }
            set 
            { 
                mRegexOptions = value;
                mTokenRegex = new Regex(mTokenRegex.ToString(), mRegexOptions);
            }
        }

        // *** ITokenizer interface implementation ***

        public ITokenizerEnumerable GetTokens(string text)
        {
            return new TokenizerEnumerable(new Enumerator(text, mTokenRegex, mDelimRegex, mIgnoreUnknownTokens));
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer) // *** current state and text are not saved
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt((int)mRegexOptions);
            writer.WriteString(mTokenRegex.ToString());
            writer.WriteString(mDelimRegex.ToString());
            writer.WriteBool(mIgnoreUnknownTokens);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mRegexOptions = (RegexOptions)reader.ReadInt();
            mTokenRegex = new Regex(reader.ReadString(), mRegexOptions);
            mDelimRegex = new Regex(reader.ReadString(), mRegexOptions);
            mIgnoreUnknownTokens = reader.ReadBool();
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Enumerator
           |
           '-----------------------------------------------------------------------
        */
        public class Enumerator : ITokenizerEnumerator
        {
            private string mText;
            private Regex mTokenRegex;
            private Regex mDelimRegex;
            private bool mIgnoreUnknownTokens;
            private Queue<Pair<int, string>> mTokens
                = new Queue<Pair<int, string>>();
            private Match mTokenMatch
                = null;

            internal Enumerator(string text, Regex tokenRegex, Regex delimRegex, bool ignoreUnknownTokens)
            {
                mText = text;
                mTokenRegex = tokenRegex;
                mDelimRegex = delimRegex;
                mIgnoreUnknownTokens = ignoreUnknownTokens;
            }

            private void GetMoreTokens()
            {
                int startIdx = 0;
                if (mTokenMatch == null)
                {
                    mTokenMatch = mTokenRegex.Match(mText);
                }
                else
                {
                    startIdx = mTokenMatch.Index + mTokenMatch.Value.Length;
                    mTokenMatch = mTokenMatch.NextMatch();
                }
                if (mTokenMatch.Success)
                {
                    if (!mIgnoreUnknownTokens)
                    {
                        int len = mTokenMatch.Index - startIdx;
                        if (len > 0)
                        {
                            string glue = mText.Substring(startIdx, len);
                            Match delimMatch = mDelimRegex.Match(glue);
                            int innerStartIdx = 0;
                            while (delimMatch.Success)
                            {
                                int innerLen = delimMatch.Index - innerStartIdx;
                                if (innerLen > 0)
                                {
                                    mTokens.Enqueue(new Pair<int, string>(startIdx + innerStartIdx, glue.Substring(innerStartIdx, innerLen)));
                                }
                                innerStartIdx = delimMatch.Index + delimMatch.Value.Length;
                                delimMatch = delimMatch.NextMatch();
                            }
                        }
                    }
                    mTokens.Enqueue(new Pair<int, string>(mTokenMatch.Index, mTokenMatch.Value));
                    if (!mIgnoreUnknownTokens && !mTokenMatch.NextMatch().Success) // tokenize tail
                    {
                        startIdx = mTokenMatch.Index + mTokenMatch.Value.Length;
                        int len = mText.Length - startIdx;
                        if (len > 0)
                        {
                            string glue = mText.Substring(startIdx, len);
                            Match delimMatch = mDelimRegex.Match(glue);
                            int innerStartIdx = 0;
                            while (delimMatch.Success)
                            {
                                int innerLen = delimMatch.Index - innerStartIdx;
                                if (innerLen > 0)
                                {
                                    mTokens.Enqueue(new Pair<int, string>(startIdx + innerStartIdx, glue.Substring(innerStartIdx, innerLen)));
                                }
                                innerStartIdx = delimMatch.Index + delimMatch.Value.Length;
                                delimMatch = delimMatch.NextMatch();
                            }
                        }
                    }
                }
            }

            public int CurrentTokenIdx
            {
                get
                {
                    Utils.ThrowException(mTokens.Count == 0 ? new InvalidOperationException() : null);
                    return mTokens.Peek().First;
                }
            }

            // *** IEnumerator<string> interface implementation ***

            public string Current
            {
                get
                {
                    Utils.ThrowException(mTokens.Count == 0 ? new InvalidOperationException() : null);
                    return mTokens.Peek().Second;
                }
            }

            public Pair<int, int> CurrentPos 
			{
                get 
				{
                    return new Pair<int, int>(mTokens.Peek().First, mTokens.Peek().First + mTokens.Peek().Second.Length);
                }
            }

            object IEnumerator.Current
            {
                get { return Current; } // throws InvalidOperationException
            }

            public bool MoveNext()
            {
                if (mTokens.Count > 0) { mTokens.Dequeue(); }
                if (mTokens.Count == 0) { GetMoreTokens(); }
                if (mTokens.Count == 0) { Reset(); }
                return mTokens.Count > 0;
            }

            public void Reset()
            {
                mTokens.Clear();
                mTokenMatch = null;
            }

            public void Dispose()
            {
            }
        }
    }
}