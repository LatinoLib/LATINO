/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SearchEngine.cs
 *  Desc:    Web search engine base class
 *  Created: Nov-2006
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using Latino.TextMining;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Class SearchEngine
       |
       '-----------------------------------------------------------------------
    */
    public abstract class SearchEngine
    {
        protected string mQuery;
        protected int mResultSetMaxSz
            = 10;
        protected long mTotalHits
            = -1;
        protected Language mLanguage
            = Language.Unspecified;
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

        public Language Language
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
}