/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    CustomStemmer.cs
 *  Desc:    Custom word stemmer 
 *  Created: Aug-2010
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class CustomStemmer
       |
       '-----------------------------------------------------------------------
    */
    public class CustomStemmer : IStemmer
    {
        private Dictionary<string, string> mMappings
            = new Dictionary<string, string>();

        public CustomStemmer(StreamReader reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] mapping = line.Split('\t');
                if (mapping.Length == 2 && !mMappings.ContainsKey(mapping[0]))
                {
                    mMappings.Add(mapping[0], mapping[1]);
                }
            }
        }

        public CustomStemmer(IEnumerable<string> lines)
        {
            Utils.ThrowException(lines == null ? new ArgumentNullException("lines") : null);
            foreach (string line in lines)
            {
                if (line != null)
                {
                    string[] mapping = line.Split('\t');
                    if (mapping.Length == 2 && !mMappings.ContainsKey(mapping[0]))
                    {
                        mMappings.Add(mapping[0], mapping[1]);
                    }
                }
            }        
        }

        public CustomStemmer(IEnumerable<Pair<string, string>> mappings)
        {
            Utils.ThrowException(mappings == null ? new ArgumentNullException("mappings") : null);
            foreach (IPair<string, string> item in mappings)
            {
                if (item != null && item.First != null && item.Second != null && !mMappings.ContainsKey(item.First))
                {
                    mMappings.Add(item.First, item.Second);
                }
            }        
        }

        public CustomStemmer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public int Count
        {
            get { return mMappings.Count; }
        }

        // *** IStemmer interface implementation ***

        public string GetStem(string word)
        {
            Utils.ThrowException(word == null ? new ArgumentNullException("word") : null);
            string stem;
            return mMappings.TryGetValue(word, out stem) ? stem : word;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            Utils.SaveDictionary(mMappings, writer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions 
            mMappings = Utils.LoadDictionary<string, string>(reader);            
        }
    }
}
