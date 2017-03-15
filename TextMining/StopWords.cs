/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    StopWords.cs
 *  Desc:    Default stop words 
 *  Created: Jul-2016
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class StopWords
       |
       '-----------------------------------------------------------------------
    */
    public partial class StopWords : IStopWords, IEnumerable<string>
    {
        private Set<string> mStopWords;
        private StringComparison mStrComparison;

        public StopWords(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public StopWords(IEnumerable<string> stopWords, StringComparison strComparison = StringComparison.OrdinalIgnoreCase)
        {
            Utils.ThrowException(stopWords == null ? new ArgumentNullException("stopWords") : null);
            mStopWords = new Set<string>(stopWords, GetStringComparer(strComparison));
            mStrComparison = strComparison;
        }

        public StopWords(Language language, StringComparison strComparison = StringComparison.OrdinalIgnoreCase)
        {
            IStopWords stopWords;
            IStemmer stemmer;
            TextMiningUtils.GetLanguageTools(language, out stopWords, out stemmer); // throws ArgumentNotSupportedException
            mStopWords = new Set<string>((StopWords)stopWords, GetStringComparer(strComparison));
            mStrComparison = strComparison;
        }

        public Set<string> Internal
        {
            get { return mStopWords; }
        }

        private StringComparer GetStringComparer(StringComparison strComparison)
        {
            switch (strComparison)
            {
                case StringComparison.CurrentCulture:
                    return StringComparer.CurrentCulture;
                case StringComparison.CurrentCultureIgnoreCase:
                    return StringComparer.CurrentCultureIgnoreCase;
                case StringComparison.InvariantCulture:
                    return StringComparer.InvariantCulture;
                case StringComparison.InvariantCultureIgnoreCase:
                    return StringComparer.InvariantCultureIgnoreCase;
                case StringComparison.Ordinal:
                    return StringComparer.Ordinal;
                case StringComparison.OrdinalIgnoreCase:
                    return StringComparer.OrdinalIgnoreCase;
                default:
                    throw new ArgumentValueException("strComparison");
            }
        }

        // *** IStopWords interface implementation ***

        public bool Contains(string word)
        {
            Utils.ThrowException(word == null ? new ArgumentNullException("word") : null);
            return mStopWords.Contains(word);
        }

        // *** IEnumerable<string> interface implementation ***

        public IEnumerator<string> GetEnumerator()
        {
            return mStopWords.GetEnumerator();
        }

        // *** IEnumerable interface implementation ***

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt((int)mStrComparison);
            mStopWords.Save(writer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mStrComparison = (StringComparison)reader.ReadInt();
            mStopWords = new Set<string>(reader, GetStringComparer(mStrComparison));
        }
    }
}
