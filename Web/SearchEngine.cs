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
        private string m_title;
        private string m_snippet;
        private string m_url;
        private double m_relevance;
        private string m_raw_text;

        public SearchEngineResult(string title, string snippet, string url, double relevance, string raw_text)
        {
            m_title = title;
            m_snippet = snippet;
            m_url = url;
            m_relevance = relevance;
            m_raw_text = raw_text;
        }

        public SearchEngineResult(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            m_title = reader.ReadString();
            m_snippet = reader.ReadString();
            m_url = reader.ReadString();
            m_relevance = reader.ReadDouble();
            m_raw_text = reader.ReadString();
        }

        public SearchEngineResult(XmlReader xml_reader)
        {
            Utils.ThrowException(xml_reader == null ? new ArgumentNullException("xml_reader") : null);
            m_title = null;
            m_snippet = null;
            m_url = null;
            m_relevance = -1;
            m_raw_text = null;
            while (xml_reader.Read()) // throws XmlException
            {
                if (xml_reader.NodeType == XmlNodeType.Element)
                {
                    if (xml_reader.Name == "Result")
                    {
                        string relevance_str = xml_reader.GetAttribute("relevance");
                        if (relevance_str != null) { m_relevance = XmlConvert.ToDouble(relevance_str); } // throws OverflowException, FormatException
                        m_url = xml_reader.GetAttribute("url");
                    }
                    else if (xml_reader.Name == "Title")
                    {
                        if (!xml_reader.IsEmptyElement)
                        {
                            xml_reader.Read(); // throws XmlException
                            Utils.ThrowException((xml_reader.NodeType != XmlNodeType.Text && xml_reader.NodeType != XmlNodeType.CDATA) ? new XmlFormatException() : null);
                            m_title = xml_reader.Value;
                        }
                    }
                    else if (xml_reader.Name == "Snippet")
                    {
                        if (!xml_reader.IsEmptyElement)
                        {
                            xml_reader.Read(); // throws XmlException
                            Utils.ThrowException((xml_reader.NodeType != XmlNodeType.Text && xml_reader.NodeType != XmlNodeType.CDATA) ? new XmlFormatException() : null);
                            m_snippet = xml_reader.Value;
                        }
                    }
                    else if (xml_reader.Name == "RawText")
                    {
                        if (!xml_reader.IsEmptyElement)
                        {
                            xml_reader.Read(); // throws XmlException
                            Utils.ThrowException((xml_reader.NodeType != XmlNodeType.Text && xml_reader.NodeType != XmlNodeType.CDATA) ? new XmlFormatException() : null);
                            m_raw_text = xml_reader.Value;
                        }
                    }
                }
                else if (xml_reader.NodeType == XmlNodeType.EndElement && xml_reader.Name == "Result")
                {
                    break;
                }
            }
        }

        public string Title
        {
            get { return m_title; }
        }

        public string Snippet
        {
            get { return m_snippet; }
        }

        public string Url
        {
            get { return m_url; }
        }

        internal void SetUrl(string url)
        {
            m_url = url;
        }

        public double Relevance
        {
            get { return m_relevance; }
        }

        public string RawText
        {
            get { return m_raw_text; }
        }

        // *** IXmlSerializable interface implementation ***

        public void SaveXml(XmlWriter xml_writer)
        {
            Utils.ThrowException(xml_writer == null ? new ArgumentNullException("xml_writer") : null);
            // the following statements throw InvalidOperationException if the writer is closed
            xml_writer.WriteStartElement("Result");
            xml_writer.WriteAttributeString("relevance", XmlConvert.ToString(m_relevance));
            xml_writer.WriteAttributeString("url", m_url);
            xml_writer.WriteStartElement("Title");
            xml_writer.WriteString(m_title);
            xml_writer.WriteEndElement();
            xml_writer.WriteStartElement("Snippet");
            xml_writer.WriteString(m_snippet);
            xml_writer.WriteEndElement();
            xml_writer.WriteStartElement("RawText");
            xml_writer.WriteString(m_raw_text);
            xml_writer.WriteEndElement();
            xml_writer.WriteEndElement();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteString(m_title);
            writer.WriteString(m_snippet);
            writer.WriteString(m_url);
            writer.WriteDouble(m_relevance);
            writer.WriteString(m_raw_text);
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
        private ArrayList<SearchEngineResult> m_items
            = new ArrayList<SearchEngineResult>();

        public SearchEngineResultSet()
        {
        }

        public SearchEngineResultSet(BinarySerializer reader)
        {
            m_items.Load(reader); // throws serialization-related exceptions
        }

        public SearchEngineResultSet(XmlReader xml_reader) : this(xml_reader, int.MaxValue) // throws ArgumentNullException, ArgumentOutOfRangeException, XmlFormatException, XmlException, OverflowException, FormatException
        {            
        }

        public SearchEngineResultSet(XmlReader xml_reader, int size_limit)
        {
            Utils.ThrowException(xml_reader == null ? new ArgumentNullException("xml_reader") : null);
            Utils.ThrowException(size_limit < 0 ? new ArgumentOutOfRangeException("size_limit") : null);
            if (size_limit != 0)
            {
                while (xml_reader.Read()) // throws XmlException
                {
                    if (xml_reader.NodeType == XmlNodeType.Element && xml_reader.Name == "ResultSet")
                    {
                        int size = XmlConvert.ToInt32(xml_reader.GetAttribute("size")); // throws ArgumentNullException, OverflowException, FormatException
                        for (int i = 0; i < size; i++)
                        {
                            m_items.Add(new SearchEngineResult(xml_reader)); // throws XmlFormatException, XmlException, OverflowException, FormatException
                            if (m_items.Count == size_limit) { return; }
                        }
                    }
                    else if (xml_reader.NodeType == XmlNodeType.EndElement && xml_reader.Name == "ResultSet")
                    {
                        break;
                    }
                }
            }
        }

        internal ArrayList<SearchEngineResult> Items
        {
            get { return m_items; }
        }

        // *** IEnumerableList<SearchEngineResult> interface implementation ***

        public int Count
        {
            get { return m_items.Count; }
        }

        public SearchEngineResult this[int index]
        {
            get
            {
                Utils.ThrowException((index < 0 || index >= m_items.Count) ? new ArgumentOutOfRangeException("index") : null);
                return m_items[index];
            }
        }

        object IEnumerableList.this[int index]
        {
            get { return this[index]; } // throws ArgumentOutOfRangeException
        }

        public IEnumerator<SearchEngineResult> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** IXmlSerializable interface implementation ***

        public void SaveXml(XmlWriter xml_writer)
        {
            Utils.ThrowException(xml_writer == null ? new ArgumentNullException("xml_writer") : null);
            // the following statements throw InvalidOperationException if the writer is closed
            xml_writer.WriteStartElement("ResultSet");
            xml_writer.WriteAttributeString("size", XmlConvert.ToString(Count));
            foreach (SearchEngineResult result in m_items)
            {
                result.SaveXml(xml_writer);
            }
            xml_writer.WriteEndElement();
        }

        // *** ICloneable interface implementation ***

        public object Clone()
        {
            SearchEngineResultSet clone = new SearchEngineResultSet();
            clone.m_items = m_items.Clone();
            return clone;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            m_items.Save(writer); // throws serialization-related exceptions
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
        protected string m_query;
        protected int m_result_set_max_sz
            = 10;
        protected long m_total_hits
            = -1;
        protected SearchEngineLanguage m_language
            = SearchEngineLanguage.Any;
        protected ISearchEngineCache m_cache
            = null;
        protected SearchEngineResultSet m_result_set
            = new SearchEngineResultSet();

        public SearchEngine(string query)
        {
            Utils.ThrowException(query == null ? new ArgumentNullException("query") : null);
            m_query = query;
        }

        public string Query
        {
            get { return m_query; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Query") : null);
                m_query = value;
            }
        }

        public int ResultSetMaxSize
        {
            get { return m_result_set_max_sz; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("ResultSetMaxSize") : null);
                m_result_set_max_sz = value;
            }
        }

        public long TotalHits
        {
            get { return m_total_hits; }
        }

        public SearchEngineLanguage Language
        {
            get { return m_language; }
            set { m_language = value; }
        }

        public ISearchEngineCache Cache
        {
            get { return m_cache; }
            set { m_cache = value; }
        }

        public SearchEngineResultSet ResultSet
        {
            get { return m_result_set; }
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
            m_result_set.Items.Clear();
            if (m_cache == null || !m_cache.GetFromCache("GoogleDefine", m_language, m_query, m_result_set_max_sz, ref m_total_hits, ref m_result_set))
            {
                int i = 0;
                string def_html = WebUtils.GetWebPage(string.Format("http://www.google.com/search?hl={0}&q=define%3A{1}", m_language == SearchEngineLanguage.Any ? "" : m_language.ToString().ToLower(), HttpUtility.UrlEncode(m_query))); // throws WebException
                Match def_match = new Regex("<li>(?<def>[^<]*)(<br><a href=\"(?<href>[^\"]*))?", RegexOptions.Singleline).Match(def_html);
                while (def_match.Success)
                {
                    string def = HttpUtility.HtmlDecode(def_match.Result("${def}").Trim());
                    string href = def_match.Result("${href}");
                    string url = null;
                    Match match_url = new Regex("&q=(?<url>[^&]*)").Match(href);
                    if (match_url.Success) { url = HttpUtility.UrlDecode(match_url.Result("${url}")); }
                    int raw_txt_max_len = Math.Min(def_match.Value.Length, RAW_TEXT_MAX_LEN);
                    m_result_set.Items.Add(new SearchEngineResult(m_query, def, url, ++i, def_match.Value.Substring(0, raw_txt_max_len)));
                    def_match = def_match.NextMatch();
                }
                string last_url = null;
                for (int j = m_result_set.Count - 1; j >= 0; j--)
                {
                    if (m_result_set[j].Url == null) { m_result_set[j].SetUrl(last_url); }
                    else { last_url = m_result_set[j].Url; }
                }
                m_total_hits = m_result_set.Count;
                if (m_cache != null)
                {
                    m_cache.PutIntoCache("GoogleDefine", m_language, m_query, m_total_hits, m_result_set);
                }
                if (m_result_set_max_sz < m_result_set.Count)
                {
                    m_result_set.Items.RemoveRange(m_result_set_max_sz, m_result_set.Count - m_result_set_max_sz);
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
        private string m_app_id
            = "YahooDemo";
        private bool m_retry
            = true;
        private static Regex m_regex
            = new Regex(@"\<Result\>((\<Title\>(?<title>[^<]*)\</Title\>)|(\<Title /\>))" +
                @"((\<Summary\>(?<summary>[^<]*)\</Summary\>)|(\<Summary /\>))" +
                @"((\<Url\>(?<url>[^<]*)\</Url\>)|(\<Url /\>))", RegexOptions.Singleline | RegexOptions.Compiled);

        public YahooSearch(string query) : base(query) // throws ArgumentNullException
        {
        }

        public string AppId
        {
            get { return m_app_id; }
            set
            {
                Utils.ThrowException(m_app_id == null ? new ArgumentNullException("AppId") : null);
                m_app_id = value;
            }
        }

        public bool Retry
        {
            get { return m_retry; }
            set { m_retry = value; }
        }

        public override void Search()
        {
            Utils.ThrowException(m_result_set_max_sz > 1000 ? new InvalidOperationException() : null);
            m_result_set.Items.Clear();
            if (m_cache == null || !m_cache.GetFromCache("YahooSearch", m_language, m_query, m_result_set_max_sz, ref m_total_hits, ref m_result_set))
            {
                int results_per_page = m_result_set_max_sz > 100 ? 100 : m_result_set_max_sz;
                for (int i = 0; i < m_result_set_max_sz; )
                {
                    string request = string.Format("http://search.yahooapis.com/WebSearchService/V1/webSearch?appid={0}&query={1}&results={2}&start={3}{4}",
                        HttpUtility.UrlEncode(m_app_id), HttpUtility.UrlEncode(m_query), results_per_page, i + 1,
                        m_language == SearchEngineLanguage.Any ? "" : string.Format("&language={0}", m_language.ToString().ToLower()));
                    Regex regex = new Regex(@"\<ResultSet [^>]*totalResultsAvailable=""(?<total_results>\d+)"" " +
                        @"totalResultsReturned=""(?<results_returned>\d+)"" firstResultPosition=""(?<first_result>\d+)""");
                    string response;
                    try
                    {
                        response = WebUtils.GetWebPage(request);
                    }
                    catch (WebException) // *** Yahoo sometimes returns gateway timeout error (do a retry on a WebException)
                    {
                        if (!m_retry) { throw; } // throws WebException
                        Thread.Sleep(2000); // delay for 2 seconds
                        response = WebUtils.GetWebPage(request); // throws WebException
                    }
                    if (response.Contains("</Error>")) { throw new QuotaExceededException(); }
                    Match regex_match = regex.Match(response);
                    m_total_hits = Convert.ToInt64(regex_match.Result("${total_results}"));
                    int first_result = Convert.ToInt32(regex_match.Result("${first_result}"));
                    int results_returned = Convert.ToInt32(regex_match.Result("${results_returned}"));
                    if (m_retry && m_total_hits == 0) // *** Yahoo sometimes returns 0 results even if this is not the case (do a retry)
                    {
                        Thread.Sleep(2000); // delay for 2 seconds
                        try
                        {
                            response = WebUtils.GetWebPage(request);
                        }
                        catch (WebException) // *** Yahoo sometimes returns gateway timeout error (do a retry on a WebException)
                        {
                            if (!m_retry) { throw; } // throws WebException
                            Thread.Sleep(2000); // delay for 2 seconds
                            response = WebUtils.GetWebPage(request); // throws WebException
                        }
                        if (response.Contains("</Error>")) { throw new QuotaExceededException(); }
                        regex_match = regex.Match(response);
                        m_total_hits = Convert.ToInt64(regex_match.Result("${total_results}"));
                        first_result = Convert.ToInt32(regex_match.Result("${first_result}"));
                        results_returned = Convert.ToInt32(regex_match.Result("${results_returned}"));
                    }
                    #region Debug code
                    //StreamWriter writer = new StreamWriter(string.Format("C:\\Test\\{0}.{1}.txt", m_query, i + 1));
                    //writer.WriteLine("Request: {0}", request);
                    //writer.WriteLine(response);
                    //writer.Close();
                    #endregion
                    if (first_result != i + 1)
                    {
                        m_total_hits = i;
                        break;
                    }
                    regex_match = m_regex.Match(response);
                    while (regex_match.Success)
                    {
                        string title = HttpUtility.HtmlDecode(regex_match.Result("${title}"));
                        string snippet = HttpUtility.HtmlDecode(regex_match.Result("${summary}"));
                        string url = HttpUtility.HtmlDecode(regex_match.Result("${url}"));
                        m_result_set.Items.Add(new SearchEngineResult(title, snippet, url, m_result_set.Count + 1, /*raw_text=*/""));
                        regex_match = regex_match.NextMatch();
                        i++;
                        if (i == m_result_set_max_sz) { break; }
                    }
                    if (results_returned < results_per_page)
                    {
                        m_total_hits = first_result + results_returned - 1;
                        break;
                    }
                }
                m_total_hits = Math.Max(m_total_hits, (long)m_result_set.Count); // just to make sure ...
                if (m_cache != null)
                {
                    m_cache.PutIntoCache("YahooSearch", m_language, m_query, m_total_hits, m_result_set);
                }
            }
        }
    }
}