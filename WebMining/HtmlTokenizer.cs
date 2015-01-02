/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    HtmlTokenizer.cs
 *  Desc:    Tokenizer for HTML documents (based on HtmlAgilityPack)
 *  Created: Apr-2011
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Latino.TextMining;

namespace Latino.WebMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class HtmlTokenizer
       |
       '-----------------------------------------------------------------------
    */
    public class HtmlTokenizer // WARNME: this is not an ITokenizer
    {
        /* .-----------------------------------------------------------------------
           |
           |  Enum TokenType
           |
           '-----------------------------------------------------------------------
        */
        public enum TokenType
        {
            OpenTag,
            EmptyTag,
            StartTag,
            EndTag,
            Text,
            Number,
            Word,
            Unknown
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Token
           |
           '-----------------------------------------------------------------------
        */
        public class Token
        {
            internal TokenType mTokenType
                = TokenType.Unknown;
            internal string mTokenStr
                = null;
            internal int mStartIndex
                = -1;
            internal int mLength
                = -1;
            internal string mTagName
                = null;

            public TokenType TokenType
            {
                get { return mTokenType; }
            }

            public string TokenStr
            {
                get { return mTokenStr; }
            }

            public int StartIndex
            {
                get { return mStartIndex; }
            }

            public int Length
            {
                get { return mLength; }
            }

            public bool IsTag
            {
                get
                {
                    return TokenType == TokenType.OpenTag || TokenType == TokenType.EmptyTag ||
                        TokenType == TokenType.StartTag || TokenType == TokenType.EndTag;
                } 
            }

            public string TagName
            {
                get { return mTagName; }
            }

            public string GetNormalized(IStemmer stemmer)
            {
                switch (mTokenType)
                { 
                    case TokenType.OpenTag:
                    case TokenType.StartTag:
                        return string.Format("<{0}>", mTagName);
                    case TokenType.EndTag:
                        return string.Format("</{0}>", mTagName);
                    case TokenType.EmptyTag:
                        return string.Format("<{0}/>", mTagName);
                    case TokenType.Number:
                        return "1";
                    case TokenType.Word:
                        return stemmer == null ? mTokenStr : stemmer.GetStem(mTokenStr);
                    default:
                        return mTokenStr;                     
                }
            }
        }

        private static string mNumberRegexStr
            = @"[0-9][\.,0-9]*";
        private static string mWordRegexStr
            = @"\p{L}+";
        private static Regex mNumberRegex 
            = new Regex("^" + mNumberRegexStr + "$", RegexOptions.Compiled);
        private static Regex mWordRegex
            = new Regex("^" + mWordRegexStr + "$", RegexOptions.Compiled);
        
        private ArrayList<Token> mTokenList
            = new ArrayList<Token>();
        private bool mNormalize
            = true;
        private string mText;
        private IStemmer mStemmer;
        private bool mDecodeTextBlocks;
        private bool mTokenizeTextBlocks;
        private bool mApplySkipRules;

        private Set<string> mSkipTags
            = new Set<string>(new string[] { "script", "noscript", "style" });

        public HtmlTokenizer(string text, IStemmer stemmer, bool decodeTextBlocks, bool tokenizeTextBlocks, bool applySkipRules)
        {
            Utils.ThrowException(text == null ? new ArgumentNullException("text") : null);
            mText = text;
            mStemmer = stemmer;
            mDecodeTextBlocks = decodeTextBlocks;
            mTokenizeTextBlocks = tokenizeTextBlocks;
            mApplySkipRules = applySkipRules;
            HtmlDocument htmlDoc = new HtmlDocument();
            Configure(htmlDoc);
            htmlDoc.LoadHtml(text);            
            HtmlNodeCollection nodes = new HtmlNodeCollection(/*parentNode=*/null);
            nodes.Add(htmlDoc.DocumentNode);
            RegexTokenizer textBlockTokenizer = null;
            if (mTokenizeTextBlocks)
            {
                textBlockTokenizer = new RegexTokenizer();
                textBlockTokenizer.TokenRegex = string.Format("({0})|({1})", mWordRegexStr, mNumberRegexStr);
                textBlockTokenizer.IgnoreUnknownTokens = true;
            }
            CreateTokens(nodes, textBlockTokenizer);
        }

        public HtmlTokenizer(string text) : this(text, /*stemmer=*/null, /*decodeTextBlocks=*/true, /*tokenizeTextBlocks=*/true, /*applySkipRules=*/true) // throws ArgumentNullException
        { 
        }

        public HtmlTokenizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        private void Configure(HtmlDocument htmlDoc)
        {
            htmlDoc.OptionCheckSyntax = false;
            htmlDoc.OptionReadEncoding = false;
            // *** set htmlDoc.OptionUseIdAttribute?
        }

        private IEnumerable<Token> CreateToken(HtmlNode node, out Token endTag, RegexTokenizer textBlockTokenizer)
        {
            IEnumerable<Token> tokens = null;
            endTag = null;
            if (node.NodeType == HtmlNodeType.Element)
            {                
                // case 1: open tag like <i> without </i> (inside another tag like <b><i></b>)
                if (node._innerlength <= 0 && node._outerlength <= 0)
                {
                    Token token = new Token();
                    token.mTokenType = TokenType.OpenTag;
                    token.mStartIndex = node._outerstartindex;
                    token.mLength = node._innerstartindex - node._outerstartindex;
                    token.mTokenStr = mText.Substring(token.mStartIndex, token.mLength);
                    token.mTagName = node.Name.ToLower();
                    tokens = new Token[] { token };
                }
                // case 2: open tag like <i> without </i> (other cases)
                else if (node._innerlength <= 0 && node.EndNode == null)
                {
                    Token token = new Token();
                    token.mTokenType = TokenType.OpenTag;
                    token.mStartIndex = node._outerstartindex;
                    token.mLength = node._outerlength;
                    token.mTokenStr = mText.Substring(token.mStartIndex, token.mLength);
                    token.mTagName = node.Name.ToLower();
                    tokens = new Token[] { token };
                }
                // case 3: empty tag like <br> or <br/>
                else if (node._innerlength <= 0)
                {
                    if (node.EndNode._outerstartindex != node._outerstartindex) // handle <tag></tag> pair 
                    {
                        string startTagStr = mText.Substring(node._outerstartindex, node.EndNode._outerstartindex - node._outerstartindex);
                        Token firstTag = new Token();
                        firstTag.mTokenType = TokenType.StartTag;
                        firstTag.mStartIndex = node._outerstartindex;
                        firstTag.mLength = startTagStr.Length;
                        firstTag.mTokenStr = startTagStr;
                        firstTag.mTagName = node.Name.ToLower();
                        string endTagStr = mText.Substring(node.EndNode._outerstartindex, node.EndNode._outerlength);
                        Token secondTag = new Token();
                        secondTag.mTokenType = TokenType.EndTag;
                        secondTag.mStartIndex = firstTag.mStartIndex + firstTag.mLength;
                        secondTag.mLength = endTagStr.Length;
                        secondTag.mTokenStr = endTagStr;
                        secondTag.mTagName = firstTag.mTagName;
                        tokens = new Token[] { firstTag, secondTag };
                    }
                    else // handle <tag/>
                    {
                        Token token = new Token();
                        token.mTokenType = TokenType.EmptyTag;
                        token.mStartIndex = node._outerstartindex;
                        token.mLength = node._outerlength;
                        token.mTokenStr = mText.Substring(node._outerstartindex, node._outerlength);
                        token.mTagName = node.Name.ToLower();
                        tokens = new Token[] { token };
                    }
                }
                // case 4: closed tag like <b>some text</b>
                else
                {
                    Token token = new Token();
                    token.mTokenType = TokenType.StartTag;
                    token.mStartIndex = node._outerstartindex;
                    token.mLength = node._innerstartindex - node._outerstartindex;
                    token.mTokenStr = mText.Substring(token.mStartIndex, token.mLength);
                    token.mTagName = node.Name.ToLower();
                    tokens = new Token[] { token };
                    endTag = new Token();
                    endTag.mTokenType = TokenType.EndTag;
                    endTag.mStartIndex = node._innerstartindex + node._innerlength;
                    endTag.mLength = node._outerstartindex + node._outerlength - endTag.mStartIndex;
                    endTag.mTokenStr = mText.Substring(endTag.mStartIndex, endTag.mLength);
                    endTag.mTagName = token.mTagName;
                }
            }
            else if (node.NodeType == HtmlNodeType.Text)
            {
                if (textBlockTokenizer == null)
                {
                    Token token = new Token();
                    token.mTokenType = TokenType.Text;
                    token.mStartIndex = node._innerstartindex;
                    token.mLength = node._innerlength;
                    token.mTokenStr = mText.Substring(node._innerstartindex, node._innerlength);
                    if (mDecodeTextBlocks) { token.mTokenStr = HttpUtility.HtmlDecode(token.mTokenStr); }
                    tokens = new Token[] { token };
                }
                else // tokenize text block
                {
                    tokens = new ArrayList<Token>();
                    string text = mText.Substring(node._innerstartindex, node._innerlength);
                    RegexTokenizer.Enumerator tokEnum = (RegexTokenizer.Enumerator)textBlockTokenizer.GetTokens(mDecodeTextBlocks ? HttpUtility.HtmlDecode(text) : text).GetEnumerator();
                    int baseIdx = node._innerstartindex;
                    while (tokEnum.MoveNext())
                    {
                        string tokenStr = tokEnum.Current;
                        Token token = new Token();
                        token.mTokenType = GetTokenType(tokenStr);
                        if (!mDecodeTextBlocks)
                        {
                            token.mStartIndex = baseIdx + tokEnum.CurrentTokenIdx;
                            token.mLength = tokenStr.Length;
                        }
                        token.mTokenStr = tokenStr;
                        ((ArrayList<Token>)tokens).Add(token);
                    }
                    if (((ArrayList<Token>)tokens).Count == 0) { tokens = null; }
                }
            }
            return tokens;
        }

        private void CreateTokens(HtmlNodeCollection nodes, RegexTokenizer textBlockTokenizer)
        {
            foreach (HtmlNode node in nodes)
            {
                Token endTag;
                IEnumerable<Token> tokens = CreateToken(node, out endTag, textBlockTokenizer);
                if (tokens != null) { mTokenList.AddRange(tokens); }
                if (!mApplySkipRules || !mSkipTags.Contains(node.Name.ToLower()))
                {
                    CreateTokens(node.ChildNodes, textBlockTokenizer);
                }
                if (endTag != null) { mTokenList.Add(endTag); }
            }
        }

        public ArrayList<Token>.ReadOnly Tokens
        {
            get { return mTokenList; }
        }

        public bool Normalize
        {
            get { return mNormalize; }
            set { mNormalize = value; }
        }

        private static TokenType GetTokenType(string token)
        {
            if (mWordRegex.Match(token).Success)
            {
                return TokenType.Word;
            }
            else if (mNumberRegex.Match(token).Success)
            {
                return TokenType.Number;
            }
            return TokenType.Unknown;
        }

        // *** ITokenizer interface implementation ***

        public string Text
        {
            get { return mText; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Text") : null);
                mText = value;
                HtmlDocument htmlDoc = new HtmlDocument();
                Configure(htmlDoc);
                htmlDoc.LoadHtml(value);
                HtmlNodeCollection nodes = new HtmlNodeCollection(/*parentNode=*/null);
                nodes.Add(htmlDoc.DocumentNode);
                mTokenList.Clear();
                RegexTokenizer textBlockTokenizer = null;
                if (mTokenizeTextBlocks)
                {
                    textBlockTokenizer = new RegexTokenizer();
                    textBlockTokenizer.TokenRegex = string.Format("({0})|({1})", mWordRegexStr, mNumberRegexStr);
                    textBlockTokenizer.IgnoreUnknownTokens = true;
                }
                CreateTokens(nodes, textBlockTokenizer);                
            }
        }

        public ITokenizerEnumerator GetEnumerator() 
        {
            return new Enumerator(mTokenList, mStemmer, mNormalize);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer) // *** current state and text are not saved
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteBool(mNormalize);
            writer.WriteObject(mStemmer);
            writer.WriteBool(mDecodeTextBlocks);
            writer.WriteBool(mTokenizeTextBlocks);
            writer.WriteBool(mApplySkipRules);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mNormalize = reader.ReadBool();
            mStemmer = reader.ReadObject<IStemmer>();
            mDecodeTextBlocks = reader.ReadBool();
            mTokenizeTextBlocks = reader.ReadBool();
            mApplySkipRules = reader.ReadBool();
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Enumerator
           |
           '-----------------------------------------------------------------------
        */
        public class Enumerator : ITokenizerEnumerator
        {
            private ArrayList<Token> mTokenList;
            private IStemmer mStemmer;
            private IEnumerator<Token> mEnum;
            private bool mNormalize;
            private int mTokenListIdx
                = -1;
            private string mTokenStr 
                = null;

            internal Enumerator(ArrayList<Token> tokenList, IStemmer stemmer, bool normalize)
            {
                mTokenList = tokenList;
                mStemmer = stemmer;
                mNormalize = normalize;                
                mEnum = tokenList.GetEnumerator();
            }

            public int CurrentTokenIdx
            {
                get
                {
                    Utils.ThrowException(mTokenStr == null || mEnum.Current.mStartIndex == -1 ? new InvalidOperationException() : null);
                    return mEnum.Current.mStartIndex;
                }
            }

            public int CurrentTokenLen
            {
                get
                {
                    Utils.ThrowException(mTokenStr == null || mEnum.Current.Length == -1 ? new InvalidOperationException() : null);
                    return mEnum.Current.Length;
                }
            }

            public int CurrentTokenListIdx
            {
                get
                {
                    Utils.ThrowException(mTokenListIdx == -1 ? new InvalidOperationException() : null);
                    return mTokenListIdx;
                }
            }

            public Token CurrentToken
            {
                get
                {
                    Utils.ThrowException(mTokenStr == null ? new InvalidOperationException() : null);
                    return mEnum.Current;
                }
            }

            // *** IEnumerator<string> interface implementation ***

            public string Current
            {
                get 
                {
                    Utils.ThrowException(mTokenStr == null ? new InvalidOperationException() : null);
                    return mTokenStr;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; } // throws InvalidOperationException
            }

            public Pair<int, int> CurrentPos {
                get { throw new NotImplementedException(); }
            }

            public bool MoveNext()
            {
                if (mEnum.MoveNext())
                {
                    mTokenStr = mNormalize ? mEnum.Current.GetNormalized(mStemmer) : mEnum.Current.mTokenStr;
                    mTokenListIdx++;
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
                mTokenStr = null;
                mTokenListIdx = -1;
            }

            public void Dispose()
            {
            }

        }
    }
}