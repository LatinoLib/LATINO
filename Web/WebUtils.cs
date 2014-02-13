/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    WebUtils.cs
 *  Desc:    Fundamental Web-related routines
 *  Created: Nov-2006
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |
       |  Class WebUtils
       |
       '-----------------------------------------------------------------------
    */
    public static class WebUtils
    {
        private static Regex mHeaderCharsetRegex
            = new Regex(@"charset\s*=\s*(([""'](?<enc>[^""']+)[""'])|((?<enc>[^\s>""';]+)))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex mContentCharsetRegex
            = new Regex(@"(\<\?xml[^>]+encoding\s*=\s*(([""'](?<enc>[^""']+)[""'])|((?<enc>[^\s>""';]+))))|(\<meta[^>]+charset\s*=\s*(([""'](?<enc>[^""']+)[""'])|((?<enc>[^\s>""';]+))))|(\<meta[^>]+charset[^>]+content\s*=\s*(([""'](?<enc>[^""']+)[""'])|((?<enc>[^\s>""';]+))))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex mHtmlOrXmlMimeTypeRegex
            = new Regex(@"(text/xml)|(text/html)|(application/xhtml\+xml)|(application/xml)|(application/[^+]+\+xml)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static IWebProxy mWebProxy
            = WebRequest.DefaultWebProxy;
        private static int mDefaultTimeout
            = 100000;
        private static int mNumRetries
            = 1;
        private static int mWaitBetweenRetries
            = 5000;
        private static int mReadWriteTimeout
            = 300000;

        // *** Getting Web content ***

        public static void UseDefaultWebProxy()
        {
            mWebProxy = WebRequest.DefaultWebProxy;
        }

        public static void SetWebProxy(string url)
        {
            if (url == null) { mWebProxy = null; }
            else { mWebProxy = new WebProxy(url); } // throws UriFormatException
        }

        public static string GetWebProxyUrl(string resourceUrl)
        {
            if (mWebProxy == null) { return null; }
            else { return mWebProxy.GetProxy(new Uri(resourceUrl)).ToString(); } // throws UriFormatException
        }

        public static string GetWebProxyUrl()
        {
            string rndUrl = string.Format("http://{0}/", Guid.NewGuid().ToString("N"));
            string proxyUrl = GetWebProxyUrl(rndUrl);
            if (proxyUrl == rndUrl) { return null; }
            else { return proxyUrl; }
        }

        public static int DefaultTimeout
        {
            get { return mDefaultTimeout; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("DefaultTimeout") : null);
                mDefaultTimeout = value;
            }
        }

        public static int ReadWriteTimeout
        {
            get { return mReadWriteTimeout; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("ReadWriteTimeout") : null);
                mReadWriteTimeout = value;
            }
        }

        public static int NumRetries
        {
            get { return mNumRetries; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("mNumRetries") : null);
                mNumRetries = value;
            }
        }

        public static int WaitBetweenRetries
        {
            get { return mWaitBetweenRetries; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("WaitBetweenRetries") : null);
                mWaitBetweenRetries = value;
            }
        }

        public static string GetWebPage(string url)
        {
            return GetWebPage(url, /*refUrl=*/null); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl)
        {
            CookieContainer cookies = null;
            return GetWebPage(url, refUrl, ref cookies); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl, ref CookieContainer cookies)
        {
            string dummy;
            return GetWebPage(url, refUrl, ref cookies, Encoding.UTF8, mDefaultTimeout, out dummy, /*authHeader=*/null); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl, ref CookieContainer cookies, Encoding htmlEncoding, int timeout, out string responseUri,
            string authHeader)
        {            
            Utils.ThrowException(url == null ? new ArgumentNullException("url") : null);
            Utils.ThrowException(htmlEncoding == null ? new ArgumentNullException("htmlEncoding") : null);
            Utils.ThrowException(timeout <= 0 ? new ArgumentOutOfRangeException("timeout") : null);
            responseUri = null;
            string pageHtml;
            int numRetries = 0;
            while (true)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); // throws UriFormatException
                request.Timeout = timeout;
                request.ReadWriteTimeout = mReadWriteTimeout;
                request.Proxy = mWebProxy;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.8.0.6) Gecko/20060728 Firefox/1.5.0.6";
                request.Accept = "application/json,text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,*/*;q=0.5";
                request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                request.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                if (authHeader != null)
                {
                    request.Headers.Add("Authorization", authHeader);
                }
                if (cookies == null) { cookies = new CookieContainer(); }
                request.CookieContainer = cookies;
                if (refUrl != null) { request.Referer = refUrl; }
                StreamReader responseReader;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // throws WebException
                    pageHtml = (responseReader = new StreamReader(response.GetResponseStream(), htmlEncoding)).ReadToEnd(); // throws WebException
                    responseReader.Close();
                    responseUri = response.ResponseUri.ToString();
                    break; 
                }
                catch (WebException e)
                {
                    numRetries++;
                    if (numRetries > mNumRetries) { throw e; }
                    Thread.Sleep(mWaitBetweenRetries);
                }
            } 
            return pageHtml;
        }

        public static string GetWebPageDetectEncoding(string url)
        {
            CookieContainer foo = null;
            bool bar;
            string dummy;
            return GetWebPageDetectEncoding(url, Encoding.GetEncoding("ISO-8859-1"), /*refUrl=*/null, ref foo, out bar, mDefaultTimeout, out dummy); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding)
        {
            CookieContainer foo = null;
            bool bar;
            string dummy;
            return GetWebPageDetectEncoding(url, defaultEncoding, /*refUrl=*/null, ref foo, out bar, mDefaultTimeout, out dummy); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding, string refUrl)
        {
            CookieContainer foo = null;
            bool bar;
            string dummy;
            return GetWebPageDetectEncoding(url, defaultEncoding, refUrl, ref foo, out bar, mDefaultTimeout, out dummy); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding, string refUrl, ref CookieContainer cookies, out bool success, int timeout, out string responseUri)
        {
            Utils.ThrowException(defaultEncoding == null ? new ArgumentNullException("defaultEncoding") : null);
            string mimeType, charSet;
            byte[] bytes = GetWebResource(url, refUrl, ref cookies, timeout, out mimeType, out charSet, /*sizeLimit=*/0, out responseUri); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException, ArgumentOutOfRangeException
            if (charSet == null) 
            { 
                success = false; 
                return defaultEncoding.GetString(bytes); 
            }
            else 
            { 
                success = true;
                return Encoding.GetEncoding(charSet).GetString(bytes); // throws ArgumentException
            } 
        }

        public static byte[] GetWebResource(string url, out string mimeType, out string charSet, int sizeLimit)
        {
            CookieContainer cookies = null;
            string dummy;
            return GetWebResource(url, /*refUrl=*/null, ref cookies, mDefaultTimeout, out mimeType, out charSet, sizeLimit, out dummy); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException
        }

        public static byte[] GetWebResource(string url, out string mimeType, out string charSet)
        {
            CookieContainer cookies = null;
            string dummy;
            return GetWebResource(url, /*refUrl=*/null, ref cookies, mDefaultTimeout, out mimeType, out charSet, /*sizeLimit=*/0, out dummy); // throws ArgumentNullException, ArgumentOutOfRangeException, UriFormatException, WebException
        }

        // From http://www.w3.org/TR/html4/charset.html#h-5.2.2:
        // Conforming user agents must observe the following priorities when determining a document's character encoding (from highest priority to lowest):
        // 1. An HTTP "charset" parameter in a "Content-Type" field.
        // 2. A META declaration with "http-equiv" set to "Content-Type" and a value set for "charset".
        // 3. The charset attribute set on an element that designates an external resource.
        // The same for XML. See http://www.apps.ietf.org/rfc/rfc3023.html.
        public static byte[] GetWebResource(string url, string refUrl, ref CookieContainer cookies, int timeout, out string mimeType, out string charSet, int sizeLimit, out string responseUri)
        {
            Utils.ThrowException(url == null ? new ArgumentNullException("url") : null);
            Utils.ThrowException(timeout <= 0 ? new ArgumentOutOfRangeException("timeout") : null);
            byte[] bytes;
            int numRetries = 0;
            responseUri = null;
            while (true)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); // throws UriFormatException
                request.Timeout = timeout;
                request.ReadWriteTimeout = mReadWriteTimeout;
                request.Proxy = mWebProxy;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.8.0.6) Gecko/20060728 Firefox/1.5.0.6";
                request.Accept = "application/json,text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,*/*;q=0.5";
                request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                request.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                if (cookies == null) { cookies = new CookieContainer(); }
                request.CookieContainer = cookies;
                if (refUrl != null) { request.Referer = refUrl; }
                Stream responseStream;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // throws WebException
                    responseUri = response.ResponseUri.ToString();
                    mimeType = response.ContentType;
                    responseStream = response.GetResponseStream();
                    bytes = Utils.ReadAllBytes(responseStream, sizeLimit);
                    responseStream.Close();
                    charSet = null;
                    if (bytes == null) { return null; } // size limit exceeded                                     
                    Match m;
                    if ((m = mHeaderCharsetRegex.Match(mimeType)).Success)
                    {
                        charSet = m.Result("${enc}");
                    }
                    else if (mHtmlOrXmlMimeTypeRegex.Match(mimeType).Success) // HTML or XML-based data format
                    {
                        string str = Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
                        m = mContentCharsetRegex.Match(str);
                        if (m.Success) { charSet = m.Result("${enc}"); }
                    }
                    break;
                }
                catch (WebException e)
                {
                    numRetries++;
                    if (numRetries > mNumRetries) { throw e; }
                    Thread.Sleep(mWaitBetweenRetries);
                }
            }
            return bytes;
        }

        // *** Search query normalization ***

        public static string NormalizeQuery(string query)
        {
            Utils.ThrowException(query == null ? new ArgumentNullException("query") : null);
            int state = 0;
            bool exclude = false;
            ArrayList<KeyDat<string, bool>> searchTerms = new ArrayList<KeyDat<string, bool>>();
            string term = "";
            foreach (char ch in query)
            {
                switch (state)
                { 
                    case 0:
                        if (ch == '+' || ch == '-')
                        {
                            state = 1;
                            exclude = ch == '-';
                        }
                        else if (ch == '"') { state = 2; }
                        else if (!Char.IsSeparator(ch)) { term += ch; state = 3; }
                        break;
                    case 1:
                        if (ch == '"') { state = 2; }
                        else if (Char.IsSeparator(ch)) { state = 0; }
                        else { term += ch; state = 3; }
                        break;
                    case 2:
                        if (ch == '"')
                        {
                            state = 0;
                            searchTerms.Add(new KeyDat<string, bool>(Regex.Replace(term, @"\s+", " ").Trim(' ').ToLower(), exclude));
                            exclude = false;
                            term = "";
                        }
                        else { term += ch; }
                        break;
                    case 3:
                        if (Char.IsSeparator(ch))
                        {
                            state = 0;
                            searchTerms.Add(new KeyDat<string, bool>(Regex.Replace(term, @"\s+", " ").Trim(' ').ToLower(), exclude));
                            exclude = false;
                            term = "";
                        }
                        else { term += ch; }
                        break;
                }
            }
            if (term != "")
            {
                searchTerms.Add(new KeyDat<string, bool>(Regex.Replace(term, @"\s+", " ").Trim(' ').ToLower(), exclude));
            }
            searchTerms.Sort();
            string queryNrm = "";
            foreach (KeyDat<string, bool> item in searchTerms)
            {
                if (item.Key != "")
                {
                    bool quotes = item.Key.Contains(" ");
                    queryNrm += string.Format("{2}{0}{1}{0} ", quotes ? "\"" : "", item.Key, item.Dat ? "-" : "");
                }
            }
            return queryNrm.TrimEnd(' ');
        }

        public delegate string NormalizeQueryDelegate(string query);
    }
}