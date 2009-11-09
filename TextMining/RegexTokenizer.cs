/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Tokenizer.cs
 *  Version:       1.0
 *  Desc:		   Text tokenizer based on regular expressions
 *  Author:        Miha Grcar
 *  Created on:    Apr-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
        private string m_text
            = "";
        private Regex m_token_regex
            = new Regex(@"[A-Za-z]+(-[A-Za-z]+)*", RegexOptions.Compiled);
        private Regex m_delim_regex
            = new Regex(@"\s+|$", RegexOptions.Compiled); // *** this is not (yet?) publicly accessible
        private bool m_ignore_unknown_tokens
            = false;

        public RegexTokenizer()
        {
        }

        public RegexTokenizer(string text)
        {
            Utils.ThrowException(text == null ? new ArgumentNullException("text") : null);
            m_text = text;
        }

        public RegexTokenizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public string TokenRegex
        { 
            get { return m_token_regex.ToString(); }
            set { m_token_regex = new Regex(value, RegexOptions.Compiled); } // throws ArgumentNullException, ArgumentException
        }

        public bool IgnoreUnknownTokens
        {
            get { return m_ignore_unknown_tokens; }
            set { m_ignore_unknown_tokens = value; }
        }

        // *** ITokenizer interface implementation ***

        public string Text
        {
            get { return m_text; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Text") : null);
                m_text = value;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new Enumerator(m_text, m_token_regex, m_delim_regex, m_ignore_unknown_tokens);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(m_text, m_token_regex, m_delim_regex, m_ignore_unknown_tokens);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer) // *** current state and text are not saved
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteString(m_token_regex.ToString());
            writer.WriteString(m_delim_regex.ToString());
            writer.WriteBool(m_ignore_unknown_tokens);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            m_token_regex = new Regex(reader.ReadString(), RegexOptions.Compiled);
            m_delim_regex = new Regex(reader.ReadString(), RegexOptions.Compiled);
            m_ignore_unknown_tokens = reader.ReadBool();
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Enumerator
           |
           '-----------------------------------------------------------------------
        */
        public class Enumerator : IEnumerator<string>
        {
            private string m_text;
            private Regex m_token_regex;
            private Regex m_delim_regex;
            private bool m_ignore_unknown_tokens;
            private Queue<string> m_tokens
                = new Queue<string>();
            private Match m_token_match
                = null;

            internal Enumerator(string text, Regex token_regex, Regex delim_regex, bool ignore_unknown_tokens)
            {
                m_text = text;
                m_token_regex = token_regex;
                m_delim_regex = delim_regex;
                m_ignore_unknown_tokens = ignore_unknown_tokens;
            }

            private void GetMoreTokens()
            {
                int start_idx = 0;
                if (m_token_match == null)
                {
                    m_token_match = m_token_regex.Match(m_text);
                }
                else
                {
                    start_idx = m_token_match.Index + m_token_match.Value.Length;
                    m_token_match = m_token_match.NextMatch();
                }
                if (m_token_match.Success)
                {
                    if (!m_ignore_unknown_tokens)
                    {
                        int len = m_token_match.Index - start_idx;
                        if (len > 0)
                        {
                            string glue = m_text.Substring(start_idx, len);
                            Match delim_match = m_delim_regex.Match(glue);
                            int inner_start_idx = 0;
                            while (delim_match.Success)
                            {
                                int inner_len = delim_match.Index - inner_start_idx;
                                if (inner_len > 0)
                                {
                                    m_tokens.Enqueue(glue.Substring(inner_start_idx, inner_len));
                                }
                                inner_start_idx = delim_match.Index + delim_match.Value.Length;
                                delim_match = delim_match.NextMatch();
                            }
                        }
                    }
                    m_tokens.Enqueue(m_token_match.Value);
                    if (!m_ignore_unknown_tokens && !m_token_match.NextMatch().Success) // tokenize tail
                    {
                        start_idx = m_token_match.Index + m_token_match.Value.Length;
                        int len = m_text.Length - start_idx;
                        if (len > 0)
                        {
                            string glue = m_text.Substring(start_idx, len);
                            Match delim_match = m_delim_regex.Match(glue);
                            int inner_start_idx = 0;
                            while (delim_match.Success)
                            {
                                int inner_len = delim_match.Index - inner_start_idx;
                                if (inner_len > 0)
                                {
                                    m_tokens.Enqueue(glue.Substring(inner_start_idx, inner_len));
                                }
                                inner_start_idx = delim_match.Index + delim_match.Value.Length;
                                delim_match = delim_match.NextMatch();
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
                    Utils.ThrowException(m_tokens.Count == 0 ? new InvalidOperationException() : null);
                    return m_tokens.Peek();
                }
            }

            object IEnumerator.Current
            {
                get { return Current; } // throws InvalidOperationException
            }

            public bool MoveNext()
            {
                if (m_tokens.Count > 0) { m_tokens.Dequeue(); }
                if (m_tokens.Count == 0) { GetMoreTokens(); }
                if (m_tokens.Count == 0) { Reset(); }
                return m_tokens.Count > 0;
            }

            public void Reset()
            {
                m_tokens.Clear();
                m_token_match = null;
            }

            public void Dispose()
            {
            }
        }
    }
}