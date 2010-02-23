/*==========================================================================;
 *
 *  (c) 2007-08 JSI.  All rights reserved.
 *
 *  File:          SearchEngineCache.cs
 *  Version:       1.0
 *  Desc:		   Cache for Web search engine results
 *  Author:        Miha Grcar
 *  Created on:    Mar-2007
 *  Last modified: Nov-2008
 *  Revision:      Nov-2008
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

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
        bool GetFromCache(string source, SearchEngineLanguage language, string query, int max_size, ref long total_hits,
            ref SearchEngineResultSet result_set);
        void PutIntoCache(string source, SearchEngineLanguage language, string query, long total_hits, SearchEngineResultSet result_set);
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Class SearchEngineCache
       |
       '-----------------------------------------------------------------------
    */
    internal static class SearchEngineCache
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
            string ret_val = "";
            for (int i = 0; i < num; i++) { ret_val += ch; }
            return ret_val;
        }
        public static string NormalizeQuery(string query)
        {
            ArrayList<string> terms = new ArrayList<string>();
            Match regex_match = new Regex(@"(?<sign>(\+|-)?)""(?<term>[^""]+)""").Match(query); // extract terms in quotes
            while (regex_match.Success)
            {
                string term = regex_match.Result("${term}").Trim().ToLower();
                term = new Regex(@"\s\s+").Replace(term, " ");
                if (term != "")
                {
                    if (!IsAlphaNum(term)) { term = "\"" + term + "\""; }
                    if (regex_match.Result("${sign}") == "-") { term = "-" + term; }
                    terms.Add(term);
                }
                query = query.Substring(0, regex_match.Index) + StringOfChars(regex_match.Value.Length, ' ') + query.Substring(regex_match.Index + regex_match.Value.Length);
                regex_match = regex_match.NextMatch();
            }
            regex_match = new Regex(@"(\s|^)(?<sign>(\+|-)?)(?<term>[^\s]+)").Match(query); // extract remaining terms
            while (regex_match.Success)
            {
                string term = regex_match.Result("${term}").ToLower();
                if (term != "and") // *** AND operator is ignored
                {
                    if (regex_match.Result("${sign}") == "-") { term = "-" + term; }
                    terms.Add(term);
                }
                regex_match = regex_match.NextMatch();
            }
            terms.Sort();
            string nrm_query = "";
            foreach (string term in terms)
            {
                nrm_query += term + " ";
            }
            return nrm_query.TrimEnd(' ');
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

    /* .-----------------------------------------------------------------------
       |		
       |  Class CacheRecord
       |
       '-----------------------------------------------------------------------
    */
    internal class CacheRecord : ISerializable
    {
        public long TotalHits;
        public int ActualSize;
        public string ResultSetXml;
        public DateTime TimeStamp;
        public CacheRecord()
        {
        }
        public CacheRecord(BinarySerializer reader)
        {
            Load(reader);
        }
        // *** ISerializable interface implementation ***
        public void Save(BinarySerializer writer)
        {
            writer.WriteLong(TotalHits);
            writer.WriteInt(ActualSize);
            writer.WriteString(ResultSetXml);
            writer.WriteDouble(TimeStamp.ToOADate());
        }
        public void Load(BinarySerializer reader)
        {
            TotalHits = reader.ReadLong();
            ActualSize = reader.ReadInt();
            ResultSetXml = reader.ReadString();
            TimeStamp = DateTime.FromOADate(reader.ReadDouble());
        }
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Class MemoryCache
       |
       '-----------------------------------------------------------------------
    */
    public class MemoryCache : ISearchEngineCache, ISerializable
    {
        private int m_ttl
            = 0;
        private Dictionary<string, CacheRecord> m_cache
            = new Dictionary<string, CacheRecord>();
        public MemoryCache()
        {
        }
        public MemoryCache(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }
        public int Ttl
        {
            get { return m_ttl; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("Ttl value") : null);
                m_ttl = value;
            }
        }
        // *** ISearchEngineCache interface implementation ***
        public bool GetFromCache(string source, SearchEngineLanguage language, string query, int max_size, ref long total_hits, ref SearchEngineResultSet result_set)
        {
            // TODO: throw exceptions
            string normalized_query = SearchEngineCache.NormalizeQuery(source, language.ToString(), query);
            if (m_cache.ContainsKey(normalized_query))
            {
                CacheRecord cache_record = m_cache[normalized_query];
                if (m_ttl == 0 || DateTime.Now.Subtract(cache_record.TimeStamp).TotalDays <= m_ttl) // record is not outdated
                {
                    if (cache_record.TotalHits == cache_record.ActualSize || max_size <= cache_record.ActualSize)
                    {
                        total_hits = cache_record.TotalHits;
                        XmlTextReader xml_reader = new XmlTextReader(new StringReader(cache_record.ResultSetXml));
                        result_set = new SearchEngineResultSet(xml_reader, max_size);
                        Utils.VerboseLine("Cache hit.");
                        return true;
                    }
                }
            }
            Utils.VerboseLine("Cache miss.");
            return false;
        }
        public void PutIntoCache(string source, SearchEngineLanguage language, string query, long total_hits, SearchEngineResultSet result_set)
        {
            // TODO: throw exceptions
            string normalized_query = SearchEngineCache.NormalizeQuery(source, language.ToString(), query);
            int actual_size = result_set.Count;
            CacheRecord cache_record = new CacheRecord();
            cache_record.TotalHits = total_hits;
            cache_record.ActualSize = result_set.Count;
            StringWriter string_writer = new StringWriter();
            XmlTextWriter xml_writer = new XmlTextWriter(string_writer);
            xml_writer.Formatting = Formatting.Indented;
            result_set.SaveXml(xml_writer);
            cache_record.ResultSetXml = string_writer.ToString();
            cache_record.TimeStamp = DateTime.Now;
            if (m_cache.ContainsKey(normalized_query))
            {
                m_cache[normalized_query] = cache_record;
            }
            else
            {
                m_cache.Add(normalized_query, cache_record);
            }
        }
        public void RemoveEmptyEntries()
        {
            Set<string> keys = new Set<string>();
            foreach (KeyValuePair<string, CacheRecord> item in m_cache)
            {
                if (item.Value.ActualSize == 0) { keys.Add(item.Key); }
            }
            foreach (string key in keys)
            {
                m_cache.Remove(key);
            }
        }
        // *** ISerializable interface implementation ***
        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following functions throw serialization-related exceptions
            writer.WriteInt(m_cache.Count);
            foreach (KeyValuePair<string, CacheRecord> cache_record in m_cache)
            {
                writer.WriteString(cache_record.Key);
                cache_record.Value.Save(writer);
            }
            writer.WriteInt(m_ttl);
        }
        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            m_cache.Clear();
            // the following functions throw serialization-related exceptions            
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                CacheRecord cache_record = new CacheRecord(reader);
                m_cache.Add(key, cache_record);
            }
            m_ttl = reader.ReadInt();
        }
    }

    /* .-----------------------------------------------------------------------
       |		
       |  Class DatabaseCache
       |
       '-----------------------------------------------------------------------
    */
    //public class DatabaseCache : ISearchEngineCache
    //{ 
    //}
}