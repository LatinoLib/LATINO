/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    MemoryCache.cs
 *  Desc:    Cache for Web search engine results
 *  Created: Mar-2007
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using Latino.TextMining;

// TOOD: record should contain binary serialized result sets
// TODO: default query normalization procedure @ WebUtils

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Class SearchEngineCache
       |
       '-----------------------------------------------------------------------
    */
   /* internal static class SearchEngineCache
    {
        private static bool IsAlphaNum(string str)
        {
            foreach (char ch in str)
            {
                if (!(ch >= 'A' && ch <= 'Z') && !(ch >= 'a' && ch <= 'z') && !(ch >= '0' && ch <= '9')) { return false; }
            }
            return true;
        }
        private static string StringOfChars(int num, char ch)
        {
            string retVal = "";
            for (int i = 0; i < num; i++) { retVal += ch; }
            return retVal;
        }
        public static string NormalizeQuery(string query)
        {
            ArrayList<string> terms = new ArrayList<string>();
            Match regexMatch = new Regex(@"(?<sign>(\+|-)?)""(?<term>[^""]+)""").Match(query); // extract terms in quotes
            while (regexMatch.Success)
            {
                string term = regexMatch.Result("${term}").Trim().ToLower();
                term = new Regex(@"\s\s+").Replace(term, " ");
                if (term != "")
                {
                    if (!IsAlphaNum(term)) { term = "\"" + term + "\""; }
                    if (regexMatch.Result("${sign}") == "-") { term = "-" + term; }
                    terms.Add(term);
                }
                query = query.Substring(0, regexMatch.Index) + StringOfChars(regexMatch.Value.Length, ' ') + query.Substring(regexMatch.Index + regexMatch.Value.Length);
                regexMatch = regexMatch.NextMatch();
            }
            regexMatch = new Regex(@"(\s|^)(?<sign>(\+|-)?)(?<term>[^\s]+)").Match(query); // extract remaining terms
            while (regexMatch.Success)
            {
                string term = regexMatch.Result("${term}").ToLower();
                if (term != "and") // *** AND operator is ignored
                {
                    if (regexMatch.Result("${sign}") == "-") { term = "-" + term; }
                    terms.Add(term);
                }
                regexMatch = regexMatch.NextMatch();
            }
            terms.Sort();
            string nrmQuery = "";
            foreach (string term in terms)
            {
                nrmQuery += term + " ";
            }
            return nrmQuery.TrimEnd(' ');
        }
        public static string NormalizeQuery(string source, string language, string query)
        {
            // *** more complex queries that contain (, ), and/or OR are not normalized (just compressed, trimmed, and made lower-case)
            if (!new Regex(@"(\sOR\s)|\(|\)", RegexOptions.IgnoreCase).Match(query).Success)
            {
                query = NormalizeQuery(query);
            }
            else
            {
                query = new Regex(@"\s\s+").Replace(query, " ");
                query = query.Trim().ToLower();
            }
            return string.Format("{0} {1}: {2}", source, language, query);
        }
    }
    * /

    /* .-----------------------------------------------------------------------
       |		
       |  Class MemoryCache
       |
       '-----------------------------------------------------------------------
    */
    public class MemoryCache : ISearchEngineCache, ISerializable
    {
        private int mTtl
            = 0;
        private Dictionary<string, CacheRecord> mCache
            = new Dictionary<string, CacheRecord>();

        public WebUtils.NormalizeQueryDelegate NormalizeQuery
            = WebUtils.NormalizeQueryDefault;

        public MemoryCache()
        {
        }

        public MemoryCache(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public int Ttl
        {
            get { return mTtl; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("Ttl") : null);
                mTtl = value;
            }
        }

        // *** ISearchEngineCache interface implementation ***

        public bool GetFromCache(string source, Language language, string query, int maxSize, ref long totalHits, ref SearchEngineResultSet resultSet)
        {            
            Utils.ThrowException(source == null ? new ArgumentNullException("source") : null);
            Utils.ThrowException(query == null ? new ArgumentNullException("query") : null);
            Utils.ThrowException(maxSize < 0 ? new ArgumentOutOfRangeException("maxSize") : null);
            string normalizedQuery = string.Format("{0} {1} {2}", source, language, NormalizeQuery == null ? query : NormalizeQuery(query));
            if (mCache.ContainsKey(normalizedQuery))
            {
                CacheRecord cacheRecord = mCache[normalizedQuery];
                if (mTtl == 0 || DateTime.Now.Subtract(cacheRecord.TimeStamp).TotalDays <= mTtl) // record is not outdated
                {
                    if (cacheRecord.TotalHits == cacheRecord.ActualSize || maxSize <= cacheRecord.ActualSize)
                    {
                        totalHits = cacheRecord.TotalHits;
                        XmlTextReader xmlReader = new XmlTextReader(new StringReader(cacheRecord.ResultSetXml));
                        resultSet = new SearchEngineResultSet(xmlReader, maxSize);
                        Utils.VerboseLine("Cache hit.");
                        return true;
                    }
                }
            }
            Utils.VerboseLine("Cache miss.");
            return false;
        }

        public void PutIntoCache(string source, Language language, string query, long totalHits, SearchEngineResultSet resultSet)
        {
            Utils.ThrowException(source == null ? new ArgumentNullException("source") : null);
            Utils.ThrowException(query == null ? new ArgumentNullException("query") : null);
            Utils.ThrowException(resultSet == null ? new ArgumentNullException("resultSet") : null);
            Utils.ThrowException(totalHits < resultSet.Count ? new ArgumentValueException("totalHits") : null);
            string normalizedQuery = string.Format("{0} {1} {2}", source, language, NormalizeQuery == null ? query : NormalizeQuery(query));
            int actualSize = resultSet.Count;
            CacheRecord cacheRecord = new CacheRecord();
            cacheRecord.TotalHits = totalHits;
            cacheRecord.ActualSize = resultSet.Count;
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.Formatting = Formatting.Indented;
            resultSet.SaveXml(xmlWriter);
            cacheRecord.ResultSetXml = stringWriter.ToString();
            cacheRecord.TimeStamp = DateTime.Now;
            if (mCache.ContainsKey(normalizedQuery))
            {
                mCache[normalizedQuery] = cacheRecord;
            }
            else
            {
                mCache.Add(normalizedQuery, cacheRecord);
            }
        }

        public void RemoveEmptyEntries()
        {
            Set<string> keys = new Set<string>();
            foreach (KeyValuePair<string, CacheRecord> item in mCache)
            {
                if (item.Value.ActualSize == 0) { keys.Add(item.Key); }
            }
            foreach (string key in keys)
            {
                mCache.Remove(key);
            }
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following functions throw serialization-related exceptions
            writer.WriteInt(mCache.Count);
            foreach (KeyValuePair<string, CacheRecord> cacheRecord in mCache)
            {
                writer.WriteString(cacheRecord.Key);
                cacheRecord.Value.Save(writer);
            }
            writer.WriteInt(mTtl);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mCache.Clear();
            // the following functions throw serialization-related exceptions            
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                CacheRecord cacheRecord = new CacheRecord(reader);
                mCache.Add(key, cacheRecord);
            }
            mTtl = reader.ReadInt();
        }
    }
}