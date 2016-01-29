/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    UnicodeTokenizer.cs
 *  Desc:    Text tokenizer based on Unicode line-splitting rules
 *  Created: Dec-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Enum TokenizerFilter
       |
       '-----------------------------------------------------------------------
    */
    public enum TokenizerFilter
    {
        AlphanumLoose,  // accept tokens that contain at least one alphanumeric character
        AlphanumStrict, // accept tokens that contain alphanumeric characters only
        AlphaLoose,     // accept tokens that contain at least one alphabetic character
        AlphaStrict,    // accept tokens that contain alphabetic characters only
        None            // accept all tokens
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class UnicodeTokenizer
       |
       '-----------------------------------------------------------------------
    */
    // This tokenizer (partially) follows the rules defined at http://www.unicode.org/reports/tr29/#Word_Boundaries
    public class UnicodeTokenizer : ITokenizer 
    {
        private TokenizerFilter mFilter
            = TokenizerFilter.None;    
        private int mMinTokenLen
            = 1;

        public UnicodeTokenizer()
        {
        }

        public UnicodeTokenizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public TokenizerFilter Filter
        {
            get { return mFilter; }
            set { mFilter = value; }
        }

        public int MinTokenLen
        {
            get { return mMinTokenLen; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("MinTokenLen") : null);
                mMinTokenLen = value;
            }
        }

        // *** ITokenizer interface implementation ***

        public ITokenizerEnumerable GetTokens(string text)
        {
            return new TokenizerEnumerable(new Enumerator(text, mFilter, mMinTokenLen));
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer) // *** current state and text are not saved
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt((int)mFilter);
            writer.WriteInt(mMinTokenLen);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mFilter = (TokenizerFilter)reader.ReadInt();
            mMinTokenLen = reader.ReadInt();
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Enumerator
           |
           '-----------------------------------------------------------------------
        */
        public class Enumerator : ITokenizerEnumerator
        {
            [Flags]
            private enum FilterFlags
            {
                ContainsAlpha = 1,
                ContainsNumeric = 2,
                ContainsOther = 4
            }

            private FilterFlags mFF;
            private string mText;
            private TokenizerFilter mFilter;            
            private int mMinTokenLen;
            private int mStartIdx
                = 0;
            private int mEndIdx
                = 0;

            internal Enumerator(string text, TokenizerFilter filter, int minTokenLen)
            {
                mFilter = filter;
                mText = text;
                mMinTokenLen = minTokenLen;
            }

            private static bool IsNewline(char ch)
            {
                return ch == '\u000B' || ch == '\u000C' || ch == '\u0085' || ch == '\u2028' || ch == '\u2029' ||
                    ch == '\r' || ch == '\n'; // *** \r and \n added for convenience (handled separately in the original rules)
            }

            private static bool IsMidLetter(char ch)
            {
                return ch == '\u00B7' || ch == '\u05F4' || ch == '\u2027' || ch == '\u003A' || ch == '\u0387' ||
                    ch == '\uFE13' || ch == '\uFE55' || ch == '\uFF1A';
            }

            private static bool IsMidNum(char ch)
            {
                return ch == '\u002C' || ch == '\u003B' || ch == '\u037E' || ch == '\u0589' || ch == '\u060C' ||
                    ch == '\u060D' || ch == '\u07F8' || ch == '\u2044' || ch == '\uFE10' || ch == '\uFE14' ||
                    ch == '\u066C' || ch == '\uFE50' || ch == '\uFE54' || ch == '\uFF0C' || ch == '\uFF1B';
            }

            private static bool IsMidNumLet(char ch)
            {
                return ch == '\u0027' || ch == '\u002E' || ch == '\u2018' || ch == '\u2019' || ch == '\u2024' ||
                    ch == '\uFE52' || ch == '\uFF07' || ch == '\uFF0E';
            }

            private static bool IsExtendNumLet(char ch)
            {
                return ch == '\u005F' || ch == '\u203F' || ch == '\u2040' || ch == '\u2054' || ch == '\uFE33' ||
                    ch == '\uFE34' || ch == '\uFE4D' || ch == '\uFE4E' || ch == '\uFE4F' || ch == '\uFF3F';
            }

            private static bool IsALetter(char ch)
            {
                return char.IsLetter(ch);
            }

            private static bool IsNumeric(char ch)
            {
                return char.IsNumber(ch);
            }

            private bool AcceptToken()
            {
                return ((mFilter == TokenizerFilter.AlphanumLoose && (mFF & (FilterFlags.ContainsAlpha | FilterFlags.ContainsNumeric)) != 0) ||
                    (mFilter == TokenizerFilter.AlphanumStrict && (mFF & FilterFlags.ContainsOther) == 0) ||
                    (mFilter == TokenizerFilter.AlphaLoose && (mFF & FilterFlags.ContainsAlpha) != 0) ||
                    (mFilter == TokenizerFilter.AlphaStrict && mFF == FilterFlags.ContainsAlpha) ||
                    mFilter == TokenizerFilter.None) && mEndIdx - mStartIdx >= mMinTokenLen;
            }

            private void GetNextToken()
            {
                mFF = 0;
                for (int i = mStartIdx; i < mText.Length - 1; i++)
                {
                    char ch1 = mText[i];
                    char ch2 = mText[i + 1];
                    if (IsALetter(ch1)) { mFF |= FilterFlags.ContainsAlpha; }
                    else if (IsNumeric(ch1)) { mFF |= FilterFlags.ContainsNumeric; }
                    else { mFF |= FilterFlags.ContainsOther; }
                    if (ch1 == '\r' && ch2 == '\n') // WB3
                    {
                    }
                    else if (IsNewline(ch1) || IsNewline(ch2)) // WB3a, WB3b
                    {
                        mEndIdx = i + 1;
                        return;
                    }
                    else if (IsALetter(ch1) && IsALetter(ch2)) // WB5
                    {
                    }
                    else if (i <= mText.Length - 3 && IsALetter(ch1) && (IsMidLetter(ch2) || IsMidNumLet(ch2)) && IsALetter(mText[i + 2])) // WB6
                    {
                    }
                    else if (i >= 1 && IsALetter(mText[i - 1]) && (IsMidLetter(ch1) || IsMidNumLet(ch1)) && IsALetter(ch2)) // WB7
                    {
                    }
                    else if ((IsNumeric(ch1) && IsNumeric(ch2)) || (IsALetter(ch1) && IsNumeric(ch2)) || (IsNumeric(ch1) && IsALetter(ch2))) // WB8, WB9, WB10
                    {
                    }
                    else if (i >= 1 && IsNumeric(mText[i - 1]) && (IsMidNum(ch1) || IsMidNumLet(ch1)) && IsNumeric(ch2)) // WB11
                    {
                    }
                    else if (i <= mText.Length - 3 && IsNumeric(ch1) && (IsMidNum(ch2) || IsMidNumLet(ch2)) && IsNumeric(mText[i + 2])) // WB12
                    {
                    }
                    else if ((IsALetter(ch1) || IsNumeric(ch1) || IsExtendNumLet(ch1)) && IsExtendNumLet(ch2)) // WB13a
                    {
                    }
                    else if (IsExtendNumLet(ch1) && (IsALetter(ch2) || IsNumeric(ch2))) // WB13b
                    {
                    }
                    else // WB14
                    {
                        mEndIdx = i + 1;
                        return;
                    }
                }
                if (mEndIdx == mText.Length)
                {
                    mEndIdx = -1;
                }
                else
                {
                    char lastCh = mText[mText.Length - 1];
                    if (IsALetter(lastCh)) { mFF |= FilterFlags.ContainsAlpha; }
                    else if (IsNumeric(lastCh)) { mFF |= FilterFlags.ContainsNumeric; }
                    else { mFF |= FilterFlags.ContainsOther; }
                    mEndIdx = mText.Length;
                }
            }

            // *** IEnumerator<string> interface implementation ***

            public string Current
            {
                get
                {
                    Utils.ThrowException(mStartIdx == mEndIdx ? new InvalidOperationException() : null);
                    return mText.Substring(mStartIdx, mEndIdx - mStartIdx);
                }
            }

            public Pair<int,int> CurrentPos
            {
                get
                {
                    return new Pair<int,int>(mStartIdx,mEndIdx);
                }
            }

            object IEnumerator.Current
            {
                get { return Current; } // throws InvalidOperationException
            }

            public bool MoveNext()
            {
                do
                {
                    mStartIdx = mEndIdx;
                    GetNextToken();
                }
                while (!AcceptToken() && mEndIdx != -1);
                if (mEndIdx == -1)
                {
                    Reset();
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                mStartIdx = 0;
                mEndIdx = 0;
            }

            public void Dispose()
            {
            }
        }
    }
}