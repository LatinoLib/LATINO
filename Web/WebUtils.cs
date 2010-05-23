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

        public static string GetHttpProxyUrl()
        {
            string rndUrl = string.Format("http://{0}/", Guid.NewGuid().ToString("N"));
            string proxyUrl = GetWebProxyUrl(rndUrl);
            if (proxyUrl == rndUrl) { return null; }
            else { return proxyUrl; }
        }

        public static string GetHttpsProxyUrl()
        {
            string rndUrl = string.Format("https://{0}/", Guid.NewGuid().ToString("N"));
            string proxyUrl = GetWebProxyUrl(rndUrl);
            if (proxyUrl == rndUrl) { return null; }
            else { return proxyUrl; }
        }

        public static string GetWebPage(string url)
        {
            CookieContainer dummy = null;
            return GetWebPage(url, /*refUrl=*/null, ref dummy); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl)
        {
            CookieContainer dummy = null;
            return GetWebPage(url, refUrl, ref dummy); // throws ArgumentNullException, ArgumentValueException, UriFormatException, WebException
        }

        public static string GetWebPage(string url, string refUrl, ref CookieContainer cookies) 
        {
            Utils.ThrowException(url == null ? new ArgumentNullException("url") : null);
            Utils.ThrowException((!url.Trim().StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.Trim().StartsWith("https://", StringComparison.OrdinalIgnoreCase)) ? new ArgumentValueException("url") : null);
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
            string pageHtml = (responseReader = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream())).ReadToEnd(); // throws WebException
            responseReader.Close();
            return pageHtml;
        }

        // *** search query normalization ***

        public static string NormalizeQueryDefault(string query)
        {
            // TBD
            return query;
        }

        public delegate string NormalizeQueryDelegate(string query);
    }
}