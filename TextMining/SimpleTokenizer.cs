using System;
using System.Collections;
using System.Collections.Generic;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Enum TokenizerType
       |
       '-----------------------------------------------------------------------
    */
    public enum TokenizerType
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
        private TokenizerType mType
            = TokenizerType.AllChars; 
        private int mMinTokenLen
            = 1;

        public SimpleTokenizer()
        {
        }

        public SimpleTokenizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public TokenizerType Type
        {
            get { return mType; }
            set { mType = value; }
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
            return new TokenizerEnumerable(new Enumerator(text, mType, mMinTokenLen));
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer) // *** current state and text are not saved
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt((int)mType);
            writer.WriteInt(mMinTokenLen);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mType = (TokenizerType)reader.ReadInt();
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
            private TokenizerType mType;
            private int mMinTokenLen;
            private int mStartIdx
                = -1;
            private int mEndIdx
                = -1;

            internal Enumerator(string text, TokenizerType type, int minTokenLen)
            {
                mType = type;
                mText = text;
                mMinTokenLen = minTokenLen;
            }

            private void GetNextToken()
            {
                mStartIdx = mEndIdx + 1;
                if (mType == TokenizerType.AlphaOnly)
                {
                    while (mStartIdx < mText.Length && !char.IsLetter(mText[mStartIdx])) { mStartIdx++; }
                    if (mStartIdx == mText.Length) { mStartIdx = -1; return; }
                    mEndIdx = mStartIdx + 1;
                    while (mEndIdx < mText.Length && char.IsLetter(mText[mEndIdx])) { mEndIdx++; }
                }
                else if (mType == TokenizerType.AlphanumOnly)
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
