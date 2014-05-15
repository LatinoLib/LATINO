/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    UrlNormalizer.cs
 *  Desc:    URL normalization routines
 *  Created: Jan-2012
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.IO;

namespace Latino.WebMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class UrlNormalizer
       |
       '-----------------------------------------------------------------------
    */
    public class UrlNormalizer
    {
        /* .-----------------------------------------------------------------------
           |
           |  Class Rule
           |
           '-----------------------------------------------------------------------
        */
        private class Rule
        {
            public Regex mUrlRegex;
            public Set<string> mQueryParams;

            public Rule(string urlRegex, params string[] qParams)
            {
                mUrlRegex = new Regex(urlRegex, RegexOptions.Compiled);
                mQueryParams = new Set<string>(qParams);
            }

            public string Execute(string left, ArrayList<string> path, ArrayList<KeyDat<string, string>> query)
            {
                return UrlAsString(left, path, query, mQueryParams);
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum NormalizationMode
           |
           '-----------------------------------------------------------------------
        */
        public enum NormalizationMode
        {
            Basic,
            DropQuery,
            Heuristics
        }

        private ArrayList<string> mBlacklist
            = new ArrayList<string>();
        private ArrayList<Rule> mRules
            = new ArrayList<Rule>();

        // effective top-level domains
        private static Set<string> mTld
            = new Set<string>(); 
        private static Set<string> mNotTld
            = new Set<string>();

        static UrlNormalizer()
        {
            Stream stream = Utils.GetManifestResourceStream(typeof(UrlNormalizer), "EffectiveTldNames.dat");
            StreamReader reader = new StreamReader(stream);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line != "" && !line.StartsWith("//"))
                {
                    if (line.StartsWith("!"))
                    {
                        mNotTld.Add(line.TrimStart('!'));
                    }
                    else
                    {
                        mTld.Add(line);
                    }
                }
            }
            stream.Close();
        }

        public UrlNormalizer(string blacklistConfigKey, string rulesConfigKey)
        { 
            // load blacklist
            string blacklistFileName = null;
            if (blacklistConfigKey != null) { blacklistFileName = Utils.GetConfigValue(blacklistConfigKey, null); }
            if (blacklistFileName != null)
            {
                string[] lines = File.ReadAllLines(blacklistFileName); 
                foreach (string _line in lines)
                {
                    string line = _line.Trim();
                    if (line == "" || line.StartsWith("#")) { continue; }
                    mBlacklist.Add(line);
                }
            }
            // load rules
            string rulesFileName = null;
            if (rulesConfigKey != null) { rulesFileName = Utils.GetConfigValue(rulesConfigKey, null); }
            if (rulesFileName != null)
            {
                string[] lines = File.ReadAllLines(rulesFileName); 
                foreach (string _line in lines)
                {
                    string line = _line.Trim();
                    if (line == "" || line.StartsWith("#")) { continue; }
                    string[] items = line.Split('\t');
                    Rule rule = new Rule(items[0], line.Substring(line.IndexOf('\t') + 1).Split('\t'));
                    mRules.Add(rule);
                }
            }
        }

        public UrlNormalizer() : this("urlBlacklistFileName", "urlRulesFileName")
        {
        }

        private static string Normalize(string str)
        {
            string nStr = "";
            foreach (char ch in str.ToLower())
            {
                if (char.IsLetterOrDigit(ch)) { nStr += ch; }
            }
            return nStr;
        }

        private static string ExecuteRules(string url, string left, ArrayList<string> path, ArrayList<KeyDat<string, string>> query,
            IEnumerable<Rule> rules)
        {
            foreach (Rule rule in rules)
            {
                Match m = rule.mUrlRegex.Match(url);
                if (m.Success)
                {
                    int c = 0;
                    foreach (KeyDat<string, string> queryParam in query)
                    {
                        if (rule.mQueryParams.Contains(queryParam.Key)) { c++; }
                    }
                    if (c < rule.mQueryParams.Count) { continue; }
                    return rule.Execute(left, path, query);
                }
            }
            return null;
        }

        // *** everything after # (document fragment) is discarded
        public static void ParseUrl(string url, out string left, out ArrayList<string> path, out ArrayList<KeyDat<string, string>> queryParsed)
        {
            Uri u = new Uri(url);
            left = string.Format("{0}://{1}:{2}", u.Scheme, u.Host, u.Port);
            path = new ArrayList<string>();
            for (int i = 1; i < u.Segments.Length; i++)
            {
                string seg = HttpUtility.UrlDecode(u.Segments[i].Trim('/'));
                if (seg != "") { path.Add(seg); }
            }
            NameValueCollection query = HttpUtility.ParseQueryString(u.Query);
            queryParsed = new ArrayList<KeyDat<string, string>>();
            for (int i = 0; i < query.Count; i++)
            {
                string key = query.Keys[i];
                if (key == null) { key = "null"; }
                string val = GetValuesAsStr(query, i);
                queryParsed.Add(new KeyDat<string, string>(key, val));
            }
            queryParsed.Sort();
        }

        private static string UrlEncode(string txt, params char[] chars)
        {
            foreach (char ch in chars)
            {
                txt = txt.Replace(ch.ToString(), string.Format("%{0:X2}", (int)ch));
            }
            return txt;
        }

        private static string GetValuesAsStr(NameValueCollection query, int idx)
        {
            string[] values = query.GetValues(idx);
            Array.Sort(values);
            string valuesStr = "";
            foreach (string value in values)
            {
                valuesStr += UrlEncode(value, '%', ',') + ",";
            }
            return valuesStr.TrimEnd(',');
        }

        public static string UrlAsString(string left, IEnumerable<string> path, ArrayList<KeyDat<string, string>> query, Set<string> queryFilter)
        {
            string url = left;
            foreach (string seg in path)
            {
                url += "/" + UrlEncode(seg, '%', '/', '?');
            }
            ArrayList<KeyDat<string, string>> tmp = query;
            if (queryFilter != null)
            {
                tmp = new ArrayList<KeyDat<string, string>>();
                foreach (KeyDat<string, string> item in query)
                {
                    if (queryFilter.Contains(item.Key)) { tmp.Add(item); }
                }
            }
            if (tmp.Count > 0)
            {
                url += "?";
                foreach (KeyDat<string, string> item in tmp)
                {
                    url += UrlEncode(item.Key, '%', '&', '=') + "=" + UrlEncode(item.Dat, '&', '=') + "&";
                }
                url = url.TrimEnd('&');
            }
            return url;
        }

        public string NormalizeUrl(string url, string title, out bool blacklist, NormalizationMode mode)
        {
            blacklist = false;
            string left;
            ArrayList<string> path;
            ArrayList<KeyDat<string, string>> queryParsed;
            ParseUrl(url, out left, out path, out queryParsed);
            string content = title == null ? "" : Normalize(title);
            string cid = Utils.GetHashCode128(content).ToString("N");
            queryParsed.InsertSorted(new KeyDat<string, string>("__cid__", cid)); // inject content-id query parameter
            string url1 = UrlAsString(left, path, queryParsed, null);
            foreach (string prefix in mBlacklist)
            {
                if (url1.StartsWith(prefix)) 
                { 
                    blacklist = true; 
                    break; 
                }
            }
            if (mode == NormalizationMode.Basic) { return url1; }
            string url2 = UrlAsString(left, path, queryParsed, new Set<string>());
            if (mode == NormalizationMode.DropQuery) { return url2; }
            string url3 = ExecuteRules(url1, left, path, queryParsed, mRules);
            if (url3 == null) { url3 = url2; }            
            return url3;
        }

        public static string GetTldFromUrl(string url)
        {
            string left;
            ArrayList<string> path;
            ArrayList<KeyDat<string, string>> queryParsed;
            ParseUrl(url, out left, out path, out queryParsed);
            return GetTldFromDomainName(left.Split(':')[1].TrimStart('/'));        
        }

        public static string GetTldFromDomainName(string domainName)
        {
            string[] parts = domainName.Split('.');
            int idx = 0;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                idx += parts[i].Length + 1;
                string tail = domainName.Substring(idx);
                if (mTld.Contains(tail)) { return tail; }
                string group = "*" + domainName.Substring(idx + parts[i + 1].Length);
                if (mTld.Contains(group) && !mNotTld.Contains(tail)) { return tail; }
            }
            return new ArrayList<string>(parts).Last;
        }
    }
}
