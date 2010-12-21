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

        public static string GetWebPage(string url)
        {
            CookieContainer dummy = null;
            return GetWebPage(url, /*refUrl=*/null, ref dummy, Encoding.UTF8); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl)
        {
            CookieContainer dummy = null;
            return GetWebPage(url, refUrl, ref dummy, Encoding.UTF8); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl, ref CookieContainer cookies)
        {
            return GetWebPage(url, refUrl, ref cookies, Encoding.UTF8); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl, ref CookieContainer cookies, Encoding htmlEncoding) 
        {            
            Utils.ThrowException(url == null ? new ArgumentNullException("url") : null);
            Utils.ThrowException(!Uri.IsWellFormedUriString(url, UriKind.Absolute) ? new ArgumentValueException("url") : null);
            Utils.ThrowException(Array.IndexOf(new string[] { "http", "https" }, new Uri(url).Scheme) < 0 ? new ArgumentValueException("url") : null);
            Utils.ThrowException(htmlEncoding == null ? new ArgumentNullException("htmlEncoding") : null);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); // throws UriFormatException
            request.Proxy = mWebProxy;
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.8.0.6) Gecko/20060728 Firefox/1.5.0.6";
            request.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,*/*;q=0.5";
            request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            request.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            if (cookies == null) { cookies = new CookieContainer(); }
            request.CookieContainer = cookies; 
            if (refUrl != null) { request.Referer = refUrl; }
            StreamReader responseReader;
            string pageHtml = (responseReader = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), htmlEncoding)).ReadToEnd(); // throws WebException
            responseReader.Close();
            return pageHtml;
        }

        public static string GetWebPageDetectEncoding(string url)
        {
            CookieContainer foo = null;
            bool bar;
            return GetWebPageDetectEncoding(url, Encoding.UTF8, /*refUrl=*/null, ref foo, out bar); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding)
        {
            CookieContainer foo = null;
            bool bar;
            return GetWebPageDetectEncoding(url, defaultEncoding, /*refUrl=*/null, ref foo, out bar); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding, string refUrl)
        {
            CookieContainer foo = null;
            bool bar;
            return GetWebPageDetectEncoding(url, defaultEncoding, refUrl, ref foo, out bar); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException  
        }

        public static string GetWebPageDetectEncoding(string url, Encoding defaultEncoding, string refUrl, ref CookieContainer cookies, out bool success) 
        {
            Utils.ThrowException(defaultEncoding == null ? new ArgumentNullException("defaultEncoding") : null);
            Encoding extAscii = Encoding.GetEncoding("ISO-8859-1");
            string html = GetWebPage(url, refUrl, ref cookies, extAscii); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
            // try to get encoding info from HTML
            success = false;
            Match match = mCharsetRegex.Match(html);
            if (match.Success)
            {
                string encStr = match.Result("${enc}");                
                try
                {
                    byte[] bytes = extAscii.GetBytes(html);
                    Encoding enc = Encoding.GetEncoding(encStr);                    
                    html = enc.GetString(bytes);
                    //Console.WriteLine(encStr);
                    success = true;
                }
                catch 
                {
                    if (defaultEncoding != extAscii)
                    {
                        byte[] bytes = extAscii.GetBytes(html);
                        html = defaultEncoding.GetString(bytes);
                    }
                }
            }
            else 
            {
                if (defaultEncoding != extAscii)
                {
                    byte[] bytes = extAscii.GetBytes(html);
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