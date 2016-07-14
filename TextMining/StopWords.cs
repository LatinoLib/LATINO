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

        public StopWords(BinarySerializer reader)
        {
            Load(reader);
        }

        public StopWords(IEnumerable<string> stopWords)
        {
            mStopWords = new Set<string>(stopWords);
        }

        public StopWords(Language language)
        {
            IStopWords stopWords;
            IStemmer stemmer;
            TextMiningUtils.GetLanguageTools(language, out stopWords, out stemmer);
            mStopWords = ((StopWords)stopWords).mStopWords;
        }

        // *** IStopWords interface implementation ***

        public bool Contains(string word)
        {
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
            mStopWords.Save(writer);
        }

        public void Load(BinarySerializer reader)
        {
            mStopWords = new Set<string>(reader);
        }
    }
}
