/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    GoogleDefine.cs
 *  Desc:    Google definitions search engine
 *  Created: Nov-2006
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System.Web;
using System.Text.RegularExpressions;
using Latino.TextMining;

namespace Latino.Web
{
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
            string langStr = TextMiningUtils.GetLanguageCode(mLanguage);
            mResultSet.Inner.Clear();
            if (mCache == null || !mCache.GetFromCache("GoogleDefine", mLanguage, mQuery, mResultSetMaxSz, ref mTotalHits, ref mResultSet))
            {
                int i = 0;
                string defHtml = WebUtils.GetWebPage(string.Format("http://www.google.com/search?defl={0}&q=define%3A{1}", langStr, HttpUtility.UrlEncode(mQuery))); // throws WebException
                Match defMatch = new Regex("<li>(?<def>[^<]*)(<br><a href=\"(?<href>[^\"]*))?", RegexOptions.Singleline).Match(defHtml);
                while (defMatch.Success)
                {
                    string def = HttpUtility.HtmlDecode(defMatch.Result("${def}").Trim());
                    string href = defMatch.Result("${href}");
                    string url = null;
                    Match matchUrl = new Regex("&q=(?<url>[^&]*)").Match(href);
                    if (matchUrl.Success) { url = HttpUtility.UrlDecode(matchUrl.Result("${url}")); }
                    mResultSet.Inner.Add(new SearchEngineResultItem(mQuery, def, url, ++i));
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
                    mResultSet.Inner.RemoveRange(mResultSetMaxSz, mResultSet.Count - mResultSetMaxSz);
                }
            }
        }
    }
}
