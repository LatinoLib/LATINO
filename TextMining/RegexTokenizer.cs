/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Tokenizer.cs
 *  Desc:    Text tokenizer based on regular expressions
 *  Created: Apr-2009
 *
 *  Authors: Miha Grcar
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
        private string mText
            = "";
        private Regex mTokenRegex
            = new Regex(@"\p{L}+(-\p{L}+)*", RegexOptions.Compiled);
        private Regex mDelimRegex
            = new Regex(@"\s+|$", RegexOptions.Compiled); // *** this is not (yet?) publicly accessible
        private bool mIgnoreUnknownTokens
            = false;

        public RegexTokenizer()
        {
        }

        public RegexTokenizer(string text)
        {
            Utils.ThrowException(text == null ? new ArgumentNullException("text") : null);
            mText = text;
        }

        public RegexTokenizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public string TokenRegex
        { 
            get { return mTokenRegex.ToString(); }
            set { mTokenRegex = new Regex(value, RegexOptions.Compiled); } // throws ArgumentNullException, ArgumentException
        }

        public bool IgnoreUnknownTokens
        {
            get { return mIgnoreUnknownTokens; }
            set { mIgnoreUnknownTokens = value; }
        }

        // *** ITokenizer interface implementation ***

        public string Text
        {
            get { return mText; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Text") : null);
                mText = value;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new Enumerator(mText, mTokenRegex, mDelimRegex, mIgnoreUnknownTokens);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(mText, mTokenRegex, mDelimRegex, mIgnoreUnknownTokens);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer) // *** current state and text are not saved
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteString(mTokenRegex.ToString());
            writer.WriteString(mDelimRegex.ToString());
            writer.WriteBool(mIgnoreUnknownTokens);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mTokenRegex = new Regex(reader.ReadString(), RegexOptions.Compiled);
            mDelimRegex = new Regex(reader.ReadString(), RegexOptions.Compiled);
            mIgnoreUnknownTokens = reader.ReadBool();
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Enumerator
           |
           '-----------------------------------------------------------------------
        */
        public class Enumerator : IEnumerator<string>
        {
            private string mText;
            private Regex mTokenRegex;
            private Regex mDelimRegex;
            private bool mIgnoreUnknownTokens;
            private Queue<string> mTokens
                = new Queue<string>();
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
                                    mTokens.Enqueue(glue.Substring(innerStartIdx, innerLen));
                                }
                                innerStartIdx = delimMatch.Index + delimMatch.Value.Length;
                                delimMatch = delimMatch.NextMatch();
                            }
                        }
                    }
                    mTokens.Enqueue(mTokenMatch.Value);
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
                                    mTokens.Enqueue(glue.Substring(innerStartIdx, innerLen));
                                }
                                innerStartIdx = delimMatch.Index + delimMatch.Value.Length;
                                delimMatch = delimMatch.NextMatch();
                            }
                        }
                    }
                }
            }

            // *** IEnumerator<string> interface implementation ***

            public string Current
            {
                get
                {
                    Utils.ThrowException(mTokens.Count == 0 ? new InvalidOperationException() : null);
                    return mTokens.Peek();
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