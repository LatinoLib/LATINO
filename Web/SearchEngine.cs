/*==========================================================================;
 *
 *  (c) 2006-08 JSI.  All rights reserved.
 *
 *  File:          SearchEngine.cs
 *  Version:       1.0
 *  Desc:		   Interfaces to various Web search engines
 *  Author:        Miha Grcar
 *  Created on:    Nov-2006
 *  Last modified: Aug-2008
 *  Revision:      Aug-2008
 *
 ***************************************************************************/

using System;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Net;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Enum SearchEngineLanguage
       |
       '-----------------------------------------------------------------------
    */
    public enum SearchEngineLanguage // Google-compatible language enumeration
    {
        Any,
        En,
        Fr,
        De
        // TODO: add other languages
        //code  language
        //----  ---------
        // sq   Albanian
        // ar   Arabic
        // bg   Bulgarian
        // ca   Catalan
        // szh  Chinese (simplified)
        // tzh  Chinese (traditional)
        // hr   Croatian
        // cs   Czech
        // da   Danish
        // nl   Dutch
        // en   English
        // et   Estonian
        // fi   Finnish
        // fr   French
        // de   German
        // el   Greek
        // he   Hebrew
        // hu   Hungarian
        // is   Icelandic
        // it   Italian
        // ja   Japanese
        // ko   Korean
        // lv   Latvian
        // lt   Lithuanian
        // no   Norwegian
        // fa   Persian
        // pl   Polish
        // pt   Portuguese
        // ro   Romanian
        // ru   Russian
        // sk   Slovak
        // sl   Slovenian
        // es   Spanish
        // sv   Swedish
        // th   Thai
        // tr   Turkish
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Struct SearchEngineResult
       |
       '-----------------------------------------------------------------------
    */
    public struct SearchEngineResult : ISerializable, IXmlSerializable
    {
        private string mTitle;
        private string mSnippet;
        private string mUrl;
        private double mRelevance;
        private string mRawText;

        public SearchEngineResult(string title, string snippet, string url, double relevance, string rawText)
        {
            mTitle = title;
            mSnippet = snippet;
            mUrl = url;
            mRelevance = relevance;
            mRawText = rawText;
        }

        public SearchEngineResult(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mTitle = reader.ReadString();
            mSnippet = reader.ReadString();
            mUrl = reader.ReadString();
            mRelevance = reader.ReadDouble();
            mRawText = reader.ReadString();
        }

        public SearchEngineResult(XmlReader xmlReader)
        {
            Utils.ThrowException(xmlReader == null ? new ArgumentNullException("xmlReader") : null);
            mTitle = null;
            mSnippet = null;
            mUrl = null;
            mRelevance = -1;
            mRawText = null;
            while (xmlReader.Read()) // throws XmlException
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "Result")
                    {
                        string relevanceStr = xmlReader.GetAttribute("relevance");
                        if (relevanceStr != null) { mRelevance = XmlConvert.ToDouble(relevanceStr); } // throws OverflowException, FormatException
                        mUrl = xmlReader.GetAttribute("url");
                    }
                    else if (xmlReader.Name == "Title")
                    {
                        if (!xmlReader.IsEmptyElement)
                        {
                            xmlReader.Read(); // throws XmlException
                            Utils.ThrowException((xmlReader.NodeType != XmlNodeType.Text && xmlReader.NodeType != XmlNodeType.CDATA) ? new XmlFormatException() : null);
                            mTitle = xmlReader.Value;
                        }
                    }
                    else if (xmlReader.Name == "Snippet")
                    {
                        if (!xmlReader.IsEmptyElement)
                        {
                            xmlReader.Read(); // throws XmlException
                            Utils.ThrowException((xmlReader.NodeType != XmlNodeType.Text && xmlReader.NodeType != XmlNodeType.CDATA) ? new XmlFormatException() : null);
                            mSnippet = xmlReader.Value;
                        }
                    }
                    else if (xmlReader.Name == "RawText")
                    {
                        if (!xmlReader.IsEmptyElement)
                        {
                            xmlReader.Read(); // throws XmlException
                            Utils.ThrowException((xmlReader.NodeType != XmlNodeType.Text && xmlReader.NodeType != XmlNodeType.CDATA) ? new XmlFormatException() : null);
                            mRawText = xmlReader.Value;
                        }
                    }
                }
                else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Result")
                {
                    break;
                }
            }
        }

        public string Title
        {
            get { return mTitle; }
        }

        public string Snippet
        {
            get { return mSnippet; }
        }

        public string Url
        {
            get { return mUrl; }
        }

        internal void SetUrl(string url)
        {
            mUrl = url;
        }

        public double Relevance
        {
            get { return mRelevance; }
        }

        public string RawText
        {
            get { return mRawText; }
        }

        // *** IXmlSerializable interface implementation ***

        public void SaveXml(XmlWriter xmlWriter)
        {
            Utils.ThrowException(xmlWriter == null ? new ArgumentNullException("xmlWriter") : null);
            // the following statements throw InvalidOperationException if the writer is closed
            xmlWriter.WriteStartElement("Result");
            xmlWriter.WriteAttributeString("relevance", XmlConvert.ToString(mRelevance));
            xmlWriter.WriteAttributeString("url", mUrl);
            xmlWriter.WriteStartElement("Title");
            xmlWriter.WriteString(mTitle);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Snippet");
            xmlWriter.WriteString(mSnippet);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("RawText");
            xmlWriter.WriteString(mRawText);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteString(mTitle);
            writer.WriteString(mSnippet);
            writer.WriteString(mUrl);
            writer.WriteDouble(mRelevance);
            writer.WriteString(mRawText);
        }
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Class SearchEngineResultSet
       |
       '-----------------------------------------------------------------------
    */
    public class SearchEngineResultSet : IEnumerableList<SearchEngineResult>, ISerializable, IXmlSerializable, ICloneable
    {
        private ArrayList<SearchEngineResult> mItems
            = new ArrayList<SearchEngineResult>();

        public SearchEngineResultSet()
        {
        }

        public SearchEngineResultSet(BinarySerializer reader)
        {
            mItems.Load(reader); // throws serialization-related exceptions
        }

        public SearchEngineResultSet(XmlReader xmlReader) : this(xmlReader, int.MaxValue) // throws ArgumentNullException, ArgumentOutOfRangeException, XmlFormatException, XmlException, OverflowException, FormatException
        {            
        }

        public SearchEngineResultSet(XmlReader xmlReader, int sizeLimit)
        {
            Utils.ThrowException(xmlReader == null ? new ArgumentNullException("xmlReader") : null);
            Utils.ThrowException(sizeLimit < 0 ? new ArgumentOutOfRangeException("sizeLimit") : null);
            if (sizeLimit != 0)
            {
                while (xmlReader.Read()) // throws XmlException
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ResultSet")
                    {
                        int size = XmlConvert.ToInt32(xmlReader.GetAttribute("size")); // throws ArgumentNullException, OverflowException, FormatException
                        for (int i = 0; i < size; i++)
                        {
                            mItems.Add(new SearchEngineResult(xmlReader)); // throws XmlFormatException, XmlException, OverflowException, FormatException
                            if (mItems.Count == sizeLimit) { return; }
                        }
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "ResultSet")
                    {
                        break;
                    }
                }
            }
        }

        internal ArrayList<SearchEngineResult> Items
        {
            get { return mItems; }
        }

        // *** IEnumerableList<SearchEngineResult> interface implementation ***

        public int Count
        {
            get { return mItems.Count; }
        }

        public SearchEngineResult this[int index]
        {
            get
            {
                Utils.ThrowException((index < 0 || index >= mItems.Count) ? new ArgumentOutOfRangeException("index") : null);
                return mItems[index];
            }
        }

        object IEnumerableList.this[int index]
        {
            get { return this[index]; } // throws ArgumentOutOfRangeException
        }

        public IEnumerator<SearchEngineResult> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** IXmlSerializable interface implementation ***

        public void SaveXml(XmlWriter xmlWriter)
        {
            Utils.ThrowException(xmlWriter == null ? new ArgumentNullException("xmlWriter") : null);
            // the following statements throw InvalidOperationException if the writer is closed
            xmlWriter.WriteStartElement("ResultSet");
            xmlWriter.WriteAttributeString("size", XmlConvert.ToString(Count));
            foreach (SearchEngineResult result in mItems)
            {
                result.SaveXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        // *** ICloneable interface implementation ***

        public object Clone()
        {
            SearchEngineResultSet clone = new SearchEngineResultSet();
            clone.mItems = mItems.Clone();
            return clone;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            mItems.Save(writer); // throws serialization-related exceptions
        }
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Abstract class SearchEngine
       |
       '-----------------------------------------------------------------------
    */
    public abstract class SearchEngine
    {
        protected const int RAW_TEXT_MAX_LEN
            = 600;
        protected string mQuery;
        protected int mResultSetMaxSz
            = 10;
        protected long mTotalHits
            = -1;
        protected SearchEngineLanguage mLanguage
            = SearchEngineLanguage.Any;
        protected ISearchEngineCache mCache
            = null;
        protected SearchEngineResultSet mResultSet
            = new SearchEngineResultSet();

        public SearchEngine(string query)
        {
            Utils.ThrowException(query == null ? new ArgumentNullException("query") : null);
            mQuery = query;
        }

        public string Query
        {
            get { return mQuery; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Query") : null);
                mQuery = value;
            }
        }

        public int ResultSetMaxSize
        {
            get { return mResultSetMaxSz; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("ResultSetMaxSize") : null);
                mResultSetMaxSz = value;
            }
        }

        public long TotalHits
        {
            get { return mTotalHits; }
        }

        public SearchEngineLanguage Language
        {
            get { return mLanguage; }
            set { mLanguage = value; }
        }

        public ISearchEngineCache Cache
        {
            get { return mCache; }
            set { mCache = value; }
        }

        public SearchEngineResultSet ResultSet
        {
            get { return mResultSet; }
        }

        // the following function is implemented in derived classes
        public abstract void Search();
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Class GoogleDefine
       |
       '-----------------------------------------------------------------------
    */
    public class GoogleDefine : SearchEngine
    {
        public GoogleDefine(string query) : base(query) // throws ArgumentNullException
        {
        }

        public override void Search()
        {
            mResultSet.Items.Clear();
            if (mCache == null || !mCache.GetFromCache("GoogleDefine", mLanguage, mQuery, mResultSetMaxSz, ref mTotalHits, ref mResultSet))
            {
                int i = 0;
                string defHtml = WebUtils.GetWebPage(string.Format("http://www.google.com/search?hl={0}&q=define%3A{1}", mLanguage == SearchEngineLanguage.Any ? "" : mLanguage.ToString().ToLower(), HttpUtility.UrlEncode(mQuery))); // throws WebException
                Match defMatch = new Regex("<li>(?<def>[^<]*)(<br><a href=\"(?<href>[^\"]*))?", RegexOptions.Singleline).Match(defHtml);
                while (defMatch.Success)
                {
                    string def = HttpUtility.HtmlDecode(defMatch.Result("${def}").Trim());
                    string href = defMatch.Result("${href}");
                    string url = null;
                    Match matchUrl = new Regex("&q=(?<url>[^&]*)").Match(href);
                    if (matchUrl.Success) { url = HttpUtility.UrlDecode(matchUrl.Result("${url}")); }
                    int rawTxtMaxLen = Math.Min(defMatch.Value.Length, RAW_TEXT_MAX_LEN);
                    mResultSet.Items.Add(new SearchEngineResult(mQuery, def, url, ++i, defMatch.Value.Substring(0, rawTxtMaxLen)));
                    defMatch = defMatch.NextMatch();
                }
                string lastUrl = null;
                for (int j = mResultSet.Count - 1; j >= 0; j--)
                {
                    if (mResultSet[j].Url == null) { mResultSet[j].SetUrl(lastUrl); }
                    else { lastUrl = mResultSet[j].Url; }
                }
                mTotalHits = mResultSet.Count;
                if (mCache != null)
                {
                    mCache.PutIntoCache("GoogleDefine", mLanguage, mQuery, mTotalHits, mResultSet);
                }
                if (mResultSetMaxSz < mResultSet.Count)
                {
                    mResultSet.Items.RemoveRange(mResultSetMaxSz, mResultSet.Count - mResultSetMaxSz);
                }
            }
        }
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Class QuotaExceededException
       |
       '-----------------------------------------------------------------------
    */
    public class QuotaExceededException : Exception
    {
        public QuotaExceededException() : base("Quota exceeded.")
        {
        }
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Class YahooSearch
       |
       '-----------------------------------------------------------------------
    */
    public class YahooSearch : SearchEngine
    {
        private string mAppId
            = "YahooDemo";
        private bool mRetry
            = true;
        private static Regex mRegex
            = new Regex(@"\<Result\>((\<Title\>(?<title>[^<]*)\</Title\>)|(\<Title /\>))" +
                @"((\<Summary\>(?<summary>[^<]*)\</Summary\>)|(\<Summary /\>))" +
                @"((\<Url\>(?<url>[^<]*)\</Url\>)|(\<Url /\>))", RegexOptions.Singleline | RegexOptions.Compiled);

        public YahooSearch(string query) : base(query) // throws ArgumentNullException
        {
        }

        public string AppId
        {
            get { return mAppId; }
            set
            {
                Utils.ThrowException(mAppId == null ? new ArgumentNullException("AppId") : null);
                mAppId = value;
            }
        }

        public bool Retry
        {
            get { return mRetry; }
            set { mRetry = value; }
        }

        public override void Search()
        {
            Utils.ThrowException(mResultSetMaxSz > 1000 ? new InvalidOperationException() : null);
            mResultSet.Items.Clear();
            if (mCache == null || !mCache.GetFromCache("YahooSearch", mLanguage, mQuery, mResultSetMaxSz, ref mTotalHits, ref mResultSet))
            {
                int resultsPerPage = mResultSetMaxSz > 100 ? 100 : mResultSetMaxSz;
                for (int i = 0; i < mResultSetMaxSz; )
                {
                    string request = string.Format("http://search.yahooapis.com/WebSearchService/V1/webSearch?appid={0}&query={1}&results={2}&start={3}{4}",
                        HttpUtility.UrlEncode(mAppId), HttpUtility.UrlEncode(mQuery), resultsPerPage, i + 1,
                        mLanguage == SearchEngineLanguage.Any ? "" : string.Format("&language={0}", mLanguage.ToString().ToLower()));
                    Regex regex = new Regex(@"\<ResultSet [^>]*totalResultsAvailable=""(?<totalResults>\d+)"" " +
                        @"totalResultsReturned=""(?<resultsReturned>\d+)"" firstResultPosition=""(?<firstResult>\d+)""");
                    string response;
                    try
                    {
                        response = WebUtils.GetWebPage(request);
                    }
                    catch (WebException) // *** Yahoo sometimes returns gateway timeout error (do a retry on a WebException)
                    {
                        if (!mRetry) { throw; } // throws WebException
                        Thread.Sleep(2000); // delay for 2 seconds
                        response = WebUtils.GetWebPage(request); // throws WebException
                    }
                    if (response.Contains("</Error>")) { throw new QuotaExceededException(); }
                    Match regexMatch = regex.Match(response);
                    mTotalHits = Convert.ToInt64(regexMatch.Result("${totalResults}"));
                    int firstResult = Convert.ToInt32(regexMatch.Result("${firstResult}"));
                    int resultsReturned = Convert.ToInt32(regexMatch.Result("${resultsReturned}"));
                    if (mRetry && mTotalHits == 0) // *** Yahoo sometimes returns 0 results even if this is not the case (do a retry)
                    {
                        Thread.Sleep(2000); // delay for 2 seconds
                        try
                        {
                            response = WebUtils.GetWebPage(request);
                        }
                        catch (WebException) // *** Yahoo sometimes returns gateway timeout error (do a retry on a WebException)
                        {
                            if (!mRetry) { throw; } // throws WebException
                            Thread.Sleep(2000); // delay for 2 seconds
                            response = WebUtils.GetWebPage(request); // throws WebException
                        }
                        if (response.Contains("</Error>")) { throw new QuotaExceededException(); }
                        regexMatch = regex.Match(response);
                        mTotalHits = Convert.ToInt64(regexMatch.Result("${totalResults}"));
                        firstResult = Convert.ToInt32(regexMatch.Result("${firstResult}"));
                        resultsReturned = Convert.ToInt32(regexMatch.Result("${resultsReturned}"));
                    }
                    #region Debug code
                    //StreamWriter writer = new StreamWriter(string.Format("C:\\Test\\{0}.{1}.txt", mQuery, i + 1));
                    //writer.WriteLine("Request: {0}", request);
                    //writer.WriteLine(response);
                    //writer.Close();
                    #endregion
                    if (firstResult != i + 1)
                    {
                        mTotalHits = i;
                        break;
                    }
                    regexMatch = mRegex.Match(response);
                    while (regexMatch.Success)
                    {
                        string title = HttpUtility.HtmlDecode(regexMatch.Result("${title}"));
                        string snippet = HttpUtility.HtmlDecode(regexMatch.Result("${summary}"));
                        string url = HttpUtility.HtmlDecode(regexMatch.Result("${url}"));
                        mResultSet.Items.Add(new SearchEngineResult(title, snippet, url, mResultSet.Count + 1, /*rawText=*/""));
                        regexMatch = regexMatch.NextMatch();
                        i++;
                        if (i == mResultSetMaxSz) { break; }
                    }
                    if (resultsReturned < resultsPerPage)
                    {
                        mTotalHits = firstResult + resultsReturned - 1;
                        break;
                    }
                }
                mTotalHits = Math.Max(mTotalHits, (long)mResultSet.Count); // just to make sure ...
                if (mCache != null)
                {
                    mCache.PutIntoCache("YahooSearch", mLanguage, mQuery, mTotalHits, mResultSet);
                }
            }
        }
    }
}