/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Stemmer.cs
 *  Desc:    Snowball word stemmer (LATINO wrapper)
 *  Created: Dec-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using SF.Snowball.Ext;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Stemmer
       |
       '-----------------------------------------------------------------------
    */
    public class Stemmer : IStemmer, ISerializable
    {
        private Language mLanguage;

        public Stemmer(Language language)
        {
            mLanguage = language;
            Utils.ThrowException(CreateStemmer() == null ? new ArgumentNotSupportedException("language") : null);
        }

        public Stemmer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        private ISnowballStemmer CreateStemmer()
        {
            switch (mLanguage)
            {
                case Language.English:
                    return new EnglishStemmer();
                case Language.German:
                    return new German2Stemmer();
                case Language.French:
                    return new FrenchStemmer();
                case Language.Spanish:
                    return new SpanishStemmer();
                case Language.Italian:
                    return new ItalianStemmer();
                case Language.Portuguese:
                    return new PortugueseStemmer();
                case Language.Danish:
                    return new DanishStemmer();
                case Language.Dutch:
                    return new DutchStemmer();
                case Language.Finnish:
                    return new FinnishStemmer();
                case Language.Norwegian:
                    return new NorwegianStemmer();
                case Language.Russian:
                    return new RussianStemmer();
                case Language.Swedish:
                    return new SwedishStemmer();
                default:
                    return null;
            }
        }

        // *** IStemmer interface implementation ***

        public string GetStem(string word)
        {
            Utils.ThrowException(word == null ? new ArgumentNullException("word") : null);
            try
            {
                ISnowballStemmer stemmer = CreateStemmer();
                stemmer.SetCurrent(word);
                stemmer.Stem();
                return stemmer.GetCurrent();
            }
            catch { return word; }
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            writer.WriteInt((int)mLanguage);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions 
            mLanguage = (Language)reader.ReadInt();
            CreateStemmer();
        }
    }
}
