/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    BingSearchEngine.cs
 *  Desc:    Bing search engine 
 *  Created: Sep-2010
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

//Windows Live ID
//Username: mgrcar@gmail.com
//Password: fegre-...
//BING ID: D3B02CEBC20CF7EE89ACB4BFC586B942E43850D5

using System;
using System.Web;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using Latino.TextMining;
using Latino.Web;

namespace Latino.Experimental.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Class BingSearchEngine
       |
       '-----------------------------------------------------------------------
    */
    public class BingSearchEngine : SearchEngine
    {
        private string mAppId
            = "DummyBingAppId";
        private bool mRetry
            = true;
        private static Regex mResultItemRegex
            = new Regex(@"\<Result\>((\<Title\>(?<title>[^<]*)\</Title\>)|(\<Title /\>))" +
                @"((\<Summary\>(?<summary>[^<]*)\</Summary\>)|(\<Summary /\>))" +
                @"((\<Url\>(?<url>[^<]*)\</Url\>)|(\<Url /\>))", RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex mResultSetRegex
            = new Regex(@"\<ResultSet [^>]*totalResultsAvailable=""(?<totalResults>\d+)"" " +
                @"totalResultsReturned=""(?<resultsReturned>\d+)"" firstResultPosition=""(?<firstResult>\d+)""",
                RegexOptions.Compiled);

        public BingSearchEngine(string query) : base(query) // throws ArgumentNullException
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

        private string SendRequest(string request, out int firstResult, out int resultsReturned)
        {
            string response;
            try
            {
                response = WebUtils.GetWebPage(request);
            }
            catch (WebException) // *** Yahoo sometimes returns gateway timeout error (do a retry)
            {
                if (!mRetry) { throw; } // throws WebException
                Thread.Sleep(2000); // delay for 2 seconds
                response = WebUtils.GetWebPage(request); // throws WebException
            }
            if (response.Contains("</Error>")) { throw new QuotaExceededException(); }
            Match regexMatch = mResultSetRegex.Match(response);
            mTotalHits = Convert.ToInt64(regexMatch.Result("${totalResults}"));
            firstResult = Convert.ToInt32(regexMatch.Result("${firstResult}"));
            resultsReturned = Convert.ToInt32(regexMatch.Result("${resultsReturned}"));
            return response;
        }

        public override void Search()
        {
            Utils.ThrowException(mResultSetMaxSz > 1000 ? new ArgumentValueException("ResultSetMaxSize") : null);
            string langStr = TextMiningUtils.GetLanguageCode(mLanguage);
            mResultSet.Inner.Clear();
            if (mCache == null || !mCache.GetFromCache("YahooSearchEngine", mLanguage, mQuery, mResultSetMaxSz, ref mTotalHits, ref mResultSet))
            {
                int resultsPerPage = mResultSetMaxSz > 100 ? 100 : mResultSetMaxSz;
                for (int i = 0; i < mResultSetMaxSz; )
                {
                    string request = string.Format("http://search.yahooapis.com/WebSearchService/V1/webSearch?appid={0}&query={1}&results={2}&start={3}{4}",
                        HttpUtility.UrlEncode(mAppId), HttpUtility.UrlEncode(mQuery), resultsPerPage, i + 1,
                        mLanguage == Language.Unspecified ? "" : string.Format("&language={0}", langStr));
                    int firstResult, resultsReturned;
                    string response = SendRequest(request, out firstResult, out resultsReturned); // throws WebException, QuotaExceededException                    
                    if (mRetry && mTotalHits == 0) // *** Yahoo sometimes returns 0 results even if this is not the case (do a retry)
                    {
                        Thread.Sleep(2000); // delay for 2 seconds
                        response = SendRequest(request, out firstResult, out resultsReturned); // throws WebException, QuotaExceededException
                    }
                    if (firstResult != i + 1)
                    {
                        mTotalHits = i;
                        break;
                    }
                    Match regexMatch = mResultItemRegex.Match(response);
                    while (regexMatch.Success)
                    {
                        string title = HttpUtility.HtmlDecode(regexMatch.Result("${title}"));
                        string snippet = HttpUtility.HtmlDecode(regexMatch.Result("${summary}"));
                        string url = HttpUtility.HtmlDecode(regexMatch.Result("${url}"));
                        mResultSet.Inner.Add(new SearchEngineResultItem(title, snippet, url, mResultSet.Count + 1));
                        regexMatch = regexMatch.NextMatch();
                        if (++i == mResultSetMaxSz) { break; }
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
                    mCache.PutIntoCache("YahooSearchEngine", mLanguage, mQuery, mTotalHits, mResultSet);
                }
            }
        }
    }
}
