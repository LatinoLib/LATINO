/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    WebUtils.cs
 *  Desc:    Fundamental Web-related routines
 *  Created: Nov-2006
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Web;

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
        // *** fetching Web pages ***

        private static Regex mCharsetRegex
            = new Regex(@"((charset)|(encoding))\s*=\s*(([""'](?<enc>[^""']+)[""'])|((?<enc>[^\s>""']+)))", RegexOptions.Compiled | RegexOptions.IgnoreCase); 

        private static IWebProxy mWebProxy
            = WebRequest.DefaultWebProxy;
        private static int mDefaultTimeout
            = 100000;
        private static int mNumRetries
            = 1;
        private static int mWaitBetweenRetries
            = 5000;

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
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("Timeout") : null);
                mDefaultTimeout = value;
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

        // TODO: remove 
        /*public static string FixUriPath(string uri) 
        {
            Utils.ThrowException(uri == null ? new ArgumentNullException("uri") : null);
            if (Uri.IsWellFormedUriString(uri, UriKind.Absolute)) { return uri; }
            Uri tmp = new Uri(uri); // throws UriFormatException
            string path = tmp.PathAndQuery;
            string[] parts = path.Split('/', '?', '&', '=', '#', '$', ';', ',', '@', ':');
            char[] separators = new char[parts.Length - 1];
            int idx = 0;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                idx += parts[i].Length;
                separators[i] = path[idx];
                idx++;
            }
            string escPath = "";
            for (int i = 0; i < parts.Length; i++)
            {
                escPath += Uri.EscapeDataString(HttpUtility.UrlDecode(parts[i]));
                if (i < parts.Length - 1) { escPath += separators[i]; }
            }
            return new Uri(tmp, escPath).OriginalString;
        }*/

        public static string GetWebPage(string url)
        {
            CookieContainer cookies = null;
            return GetWebPage(url, /*refUrl=*/null, ref cookies, Encoding.UTF8, mDefaultTimeout); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl)
        {
            CookieContainer cookies = null;
            return GetWebPage(url, refUrl, ref cookies, Encoding.UTF8, mDefaultTimeout); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl, ref CookieContainer cookies)
        {
            return GetWebPage(url, refUrl, ref cookies, Encoding.UTF8, mDefaultTimeout); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl, ref CookieContainer cookies, Encoding htmlEncoding, int timeout) 
        {            
            Utils.ThrowException(url == null ? new ArgumentNullException("url") : null);
            Utils.ThrowException(htmlEncoding == null ? new ArgumentNullException("htmlEncoding") : null);
            Utils.ThrowException(timeout <= 0 ? new ArgumentOutOfRangeException("timeout") : null);
            string pageHtml;
            int numRetries = 0;
            while (true)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); // throws UriFormatException
                request.Timeout = timeout;
                request.Proxy = mWebProxy;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.8.0.6) Gecko/20060728 Firefox/1.5.0.6";
                request.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,*/*;q=0.5";
                request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                request.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                if (cookies == null) { cookies = new CookieContainer(); }
                request.CookieContainer = cookies;
                if (refUrl != null) { request.Referer = refUrl; }
                StreamReader responseReader;
                try
                {
                    pageHtml = (responseReader = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), htmlEncoding)).ReadToEnd(); // throws WebException
                    responseReader.Close();
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
            return GetWebPageDetectEncoding(url, Encoding.UTF8, /*refUrl=*/null, ref foo, out bar, mDefaultTimeout); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding)
        {
            CookieContainer foo = null;
            bool bar;
            return GetWebPageDetectEncoding(url, defaultEncoding, /*refUrl=*/null, ref foo, out bar, mDefaultTimeout); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding, string refUrl)
        {
            CookieContainer foo = null;
            bool bar;
            return GetWebPageDetectEncoding(url, defaultEncoding, refUrl, ref foo, out bar, mDefaultTimeout); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding, string refUrl, ref CookieContainer cookies, out bool success, int timeout) 
        {
            Utils.ThrowException(defaultEncoding == null ? new ArgumentNullException("defaultEncoding") : null);
            Encoding extAsciiEnc = Encoding.GetEncoding("ISO-8859-1");
            string html = GetWebPage(url, refUrl, ref cookies, extAsciiEnc, timeout); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException, ArgumentOutOfRangeException
            // try to get encoding info from HTML
            success = false;
            Match match = mCharsetRegex.Match(html);
            if (match.Success)
            {
                string encStr = match.Result("${enc}");          
                try
                {
                    byte[] bytes = extAsciiEnc.GetBytes(html);
                    Encoding enc = Encoding.GetEncoding(encStr);                    
                    html = enc.GetString(bytes);
                    success = true;
                }
                catch 
                {
                    if (defaultEncoding != extAsciiEnc)
                    {
                        byte[] bytes = extAsciiEnc.GetBytes(html);
                        html = defaultEncoding.GetString(bytes);
                    }
                }
            }
            else 
            {
                if (defaultEncoding != extAsciiEnc)
                {
                    byte[] bytes = extAsciiEnc.GetBytes(html);
                    html = defaultEncoding.GetString(bytes);
                }
            }
            return html;
        }

        // *** search query normalization ***

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