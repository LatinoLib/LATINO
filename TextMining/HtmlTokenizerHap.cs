/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    HtmlTokenizerHap.cs
 *  Desc:    Tokenizer for HTML documents (based on HtmlAgilityPack)
 *  Created: Apr-2011
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class HtmlTokenizerHap
       |
       '-----------------------------------------------------------------------
    */
    public class HtmlTokenizerHap : ITokenizer
    {
        /* .-----------------------------------------------------------------------
           |
           |  Enum TokenType
           |
           '-----------------------------------------------------------------------
        */
        public enum TokenType
        {
            Tag,
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
        internal class Token
        {
            public TokenType mTokenType
                = TokenType.Unknown;
            public string mTokenStr
                = null;
            public int mStartIndex
                = -1;
            public int mLength
                = 0;
        }

        private static string mNumberRegexStr
            = @"[0-9][\.,0-9]*";
        private static string mWordRegexStr
            = @"\p{L}+";
        private static string mTagRegexStr
            = @"<(?<startSlash>/?)(?<tagName>[^\s]+).*?(?<endSlash>/?)>";
        private static Regex mNumberRegex 
            = new Regex("^" + mNumberRegexStr + "$", RegexOptions.Compiled);
        private static Regex mWordRegex
            = new Regex("^" + mWordRegexStr + "$", RegexOptions.Compiled);
        private static Regex mTagRegex
            = new Regex("^" + mTagRegexStr + "$", RegexOptions.Compiled | RegexOptions.Singleline);
        
        private ArrayList<Token> mTokenList
            = new ArrayList<Token>();
        private bool mNormalize
            = true;
        private string mText;
        private IStemmer mStemmer;
        private bool mDecodeTextBlocks;
        private bool mTokenizeTextBlocks;                

        public HtmlTokenizerHap(string text, IStemmer stemmer, bool decodeTextBlocks, bool tokenizeTextBlocks)
        {
            Utils.ThrowException(text == null ? new ArgumentNullException("text") : null);
            mText = text;
            mStemmer = stemmer;
            mDecodeTextBlocks = decodeTextBlocks;
            mTokenizeTextBlocks = tokenizeTextBlocks;            
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

        public HtmlTokenizerHap(string text) : this(text, /*stemmer=*/null, /*decodeTextBlocks=*/true, /*tokenizeTextBlocks=*/true) // throws ArgumentNullException
        { 
        }

        public HtmlTokenizerHap(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        private void Configure(HtmlDocument htmlDoc)
        {
            htmlDoc.OptionCheckSyntax = false;
            htmlDoc.OptionReadEncoding = false;
            // *** what is the following option for?
            //htmlDoc.OptionUseIdAttribute = false;
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
                        string endTagStr = mText.Substring(node.EndNode._outerstartindex, node.EndNode._outerlength);
                        Token secondTag = new Token();
                        secondTag.mTokenType = TokenType.EndTag;
                        secondTag.mStartIndex = firstTag.mStartIndex + firstTag.mLength;
                        secondTag.mLength = endTagStr.Length;
                        secondTag.mTokenStr = endTagStr;
                        tokens = new Token[] { firstTag, secondTag };
                    }
                    else // handle <tag/>
                    {
                        Token token = new Token();
                        token.mTokenType = TokenType.EmptyTag;
                        token.mStartIndex = node._outerstartindex;
                        token.mLength = node._outerlength;
                        token.mTokenStr = mText.Substring(node._outerstartindex, node._outerlength);
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
                    tokens = new Token[] { token };
                    endTag = new Token();
                    endTag.mTokenType = TokenType.EndTag;
                    endTag.mStartIndex = node._innerstartindex + node._innerlength;
                    endTag.mLength = node._outerstartindex + node._outerlength - endTag.mStartIndex;
                    endTag.mTokenStr = mText.Substring(endTag.mStartIndex, endTag.mLength);
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
                    textBlockTokenizer.Text = mDecodeTextBlocks ? HttpUtility.HtmlDecode(text) : text;
                    RegexTokenizer.Enumerator tokEnum = (RegexTokenizer.Enumerator)textBlockTokenizer.GetEnumerator();
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
                CreateTokens(node.ChildNodes, textBlockTokenizer);
                if (endTag != null) { mTokenList.Add(endTag); }
            }
        }

        public bool Normalize
        {
            get { return mNormalize; }
            set { mNormalize = value; }
        }

        public string GetTextBlock(int startIdx, int len)
        {
            Utils.ThrowException((startIdx < 0 || startIdx >= mText.Length) ? new ArgumentOutOfRangeException("startIdx") : null);
            Utils.ThrowException((len < 0 || startIdx + len > mText.Length) ? new ArgumentOutOfRangeException("len") : null);
            return mDecodeTextBlocks ? HttpUtility.HtmlDecode(mText.Substring(startIdx, len)) : mText.Substring(startIdx, len);
        }

        public static TokenType GetTokenType(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Match m;
            if ((m = mTagRegex.Match(token)).Success)
            {
                if (m.Result("${startSlash}") == "/")
                {
                    return TokenType.EndTag;
                }
                else if (m.Result("${endSlash}") == "/")
                {
                    return TokenType.EmptyTag;
                }
                return TokenType.Tag;
            }
            else if (mWordRegex.Match(token).Success)
            {
                return TokenType.Word;
            }
            else if (mNumberRegex.Match(token).Success)
            {
                return TokenType.Number;
            }
            return TokenType.Unknown; 
        }

        public static string NormalizeToken(string token, IStemmer stemmer)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Match m;
            if ((m = mTagRegex.Match(token)).Success)
            {
                if (m.Result("${startSlash}") == "/")
                {
                    return "</" + m.Result("${tagName}") + ">";
                }
                else if (m.Result("${endSlash}") == "/")
                {
                    return "<" + m.Result("${tagName}") + "/>";
                }
                return "<" + m.Result("${tagName}") + ">";
            }
            else if (mWordRegex.Match(token).Success)
            {
                return stemmer != null ? stemmer.GetStem(token) : token;
            }
            else if (mNumberRegex.Match(token).Success)
            {
                return "1";
            }
            return token;
        }

        public static bool IsTag(string token)
        {
            TokenType type = GetTokenType(token); // throws ArgumentNullException
            return type == TokenType.EmptyTag || type == TokenType.EndTag || type == TokenType.Tag;
        }

        public static string GetTagName(string token)
        {
            Utils.ThrowException(token == null ? new ArgumentNullException("token") : null);
            Match m;
            if ((m = mTagRegex.Match(token)).Success)
            {
                return m.Result("${tagName}");
            }
            return null;
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

        public IEnumerator<string> GetEnumerator()
        {
            return new Enumerator(mTokenList, mStemmer, mNormalize);
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
            writer.WriteObject(mStemmer);
            writer.WriteBool(mDecodeTextBlocks);
            writer.WriteBool(mTokenizeTextBlocks);            
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mNormalize = reader.ReadBool();
            mStemmer = reader.ReadObject<IStemmer>();
            mDecodeTextBlocks = reader.ReadBool();
            mTokenizeTextBlocks = reader.ReadBool();
        }

        // TODO: simplify the code below (?)
        /* .-----------------------------------------------------------------------
           |
           |  Class Enumerator
           |
           '-----------------------------------------------------------------------
        */
        public class Enumerator : IEnumerator<string>
        {
            private ArrayList<Token> mTokenList;
            private IStemmer mStemmer;
            private IEnumerator<Token> mEnum;
            private bool mNormalize;
            private string mCurrentToken 
                = null;
            private int mCurrentTokenIdx
                = -1;
            private int mCurrentTokenLen
                = 0;
            private TokenType mCurrentTokenType
                = TokenType.Unknown;

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
                    Utils.ThrowException(mCurrentToken == null ? new InvalidOperationException() : null);
                    //Utils.ThrowException(mCurrentTokenIdx == -1 ? new InvalidOperationException() : null); // TODO: remove this!!!
                    return mCurrentTokenIdx;
                }
            }

            public int CurrentTokenLen
            {
                get
                {
                    Utils.ThrowException(mCurrentToken == null ? new InvalidOperationException() : null);
                    //Utils.ThrowException(mCurrentTokenLen == 0 ? new InvalidOperationException() : null); // TODO: remove this!!!
                    return mCurrentTokenLen;
                }
            }

            public TokenType CurrentTokenType
            {
                get
                {
                    Utils.ThrowException(mCurrentToken == null ? new InvalidOperationException() : null);
                    return mCurrentTokenType;
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
                if (mEnum.MoveNext())
                {
                    mCurrentToken = mNormalize ? NormalizeToken(mEnum.Current.mTokenStr, mStemmer) : mEnum.Current.mTokenStr;
                    mCurrentTokenIdx = mEnum.Current.mStartIndex;
                    mCurrentTokenLen = mEnum.Current.mLength;
                    mCurrentTokenType = mEnum.Current.mTokenType;
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
                mCurrentTokenLen = 0;
                mCurrentTokenType = TokenType.Unknown;
            }

            public void Dispose()
            {
            }
        }
    }
}