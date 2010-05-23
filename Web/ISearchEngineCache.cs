/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ISearchEngineCache.cs
 *  Desc:    Search engine cache interface definition
 *  Created: Mar-2007
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using Latino.TextMining;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Interface ISearchEngineCache
       |
       '-----------------------------------------------------------------------
    */
    public interface ISearchEngineCache
    {
        bool GetFromCache(string source, Language language, string query, int maxSize, ref long totalHits,
            ref SearchEngineResultSet resultSet);
        void PutIntoCache(string source, Language language, string query, long totalHits, SearchEngineResultSet resultSet);
    }
}
