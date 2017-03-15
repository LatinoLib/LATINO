using System;
using System.Collections;
using System.Collections.Generic;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Enum TokenType
       |
       '-----------------------------------------------------------------------
    */
    public enum TokenType
    {
        AllChars,    // equivalent to [^\s]+
        AlphaOnly,   // equivalent to \p{L}+
        AlphanumOnly // equivalent to [\p{L}\d]+
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class SimpleTokenizer
       |
       '-----------------------------------------------------------------------
    */
    public class SimpleTokenizer : ITokenizer
    {
        private TokenType mTokenType
            = TokenType.AlphaOnly;
        private int mMinTokenLen
            = 2;

        public SimpleTokenizer()
        {
        }

        public SimpleTokenizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public TokenType TokenType
        {
            get { return mTokenType; }
            set { mTokenType = value; }
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
            return new TokenizerEnumerable(() => new Enumerator(text, mTokenType, mMinTokenLen));
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer) // *** current state and text are not saved
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt((int)mTokenType);
            writer.WriteInt(mMinTokenLen);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mTokenType = (TokenType)reader.ReadInt();
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
            private string mText;
            private TokenType mType;
            private int mMinTokenLen;
            private int mStartIdx
                = -1;
            private int mEndIdx
                = -1;

            internal Enumerator(string text, TokenType type, int minTokenLen)
            {
                mType = type;
                mText = text;
                mMinTokenLen = minTokenLen;
            }

            private void GetNextToken()
            {
                mStartIdx = mEndIdx + 1;
                if (mType == TokenType.AlphaOnly)
                {
                    while (mStartIdx < mText.Length && !char.IsLetter(mText[mStartIdx])) { mStartIdx++; }
                    if (mStartIdx == mText.Length) { mStartIdx = -1; return; }
                    mEndIdx = mStartIdx + 1;
                    while (mEndIdx < mText.Length && char.IsLetter(mText[mEndIdx])) { mEndIdx++; }
                }
                else if (mType == TokenType.AlphanumOnly)
                {
                    while (mStartIdx < mText.Length && !char.IsLetterOrDigit(mText[mStartIdx])) { mStartIdx++; }
                    if (mStartIdx == mText.Length) { mStartIdx = -1; return; }
                    mEndIdx = mStartIdx + 1;
                    while (mEndIdx < mText.Length && char.IsLetterOrDigit(mText[mEndIdx])) { mEndIdx++; }
                }
                else // TokenizerType.AllChars
                {
                    while (mStartIdx < mText.Length && char.IsWhiteSpace(mText[mStartIdx])) { mStartIdx++; }
                    if (mStartIdx == mText.Length) { mStartIdx = -1; return; }
                    mEndIdx = mStartIdx + 1;
                    while (mEndIdx < mText.Length && !char.IsWhiteSpace(mText[mEndIdx])) { mEndIdx++; }                
                }
                mEndIdx--;
            }


            public int CurrentTokenIdx
            {
                get
                {
                    Utils.ThrowException(mStartIdx == -1 ? new InvalidOperationException() : null);
                    return mStartIdx;
                }
            }

            // *** IEnumerator<string> interface implementation ***

            public string Current
            {
                get
                {
                    Utils.ThrowException(mStartIdx == -1 ? new InvalidOperationException() : null);
                    return mText.Substring(mStartIdx, mEndIdx - mStartIdx + 1);
                }
            }

            public Pair<int, int> CurrentPos 
			{
                get 
				{
                    return new Pair<int, int>(mStartIdx, mEndIdx+1);
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
                    GetNextToken();
                }
                while (mStartIdx != -1 && !(mEndIdx - mStartIdx + 1 >= mMinTokenLen));
                if (mStartIdx == -1)
                {
                    Reset();
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                mStartIdx = -1;
                mEndIdx = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}
