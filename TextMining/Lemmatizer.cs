/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Lemmatizer.cs
 *  Desc:    LemmaSharp word lemmatizer (LATINO wrapper)
 *  Created: Jan-2009
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using Latino.LemmaSharp;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Lemmatizer
       |
       '-----------------------------------------------------------------------
    */
    public class Lemmatizer : IStemmer, ISerializable
    {
        private Language mLanguage;
        private LemmaSharp.Lemmatizer mLemmatizer;

        public Lemmatizer(Language language)
        {
            mLanguage = language;
            bool success = CreateLemmatizer();
            Utils.ThrowException(!success ? new ArgumentNotSupportedException("language") : null);
        }

        public Lemmatizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        private bool CreateLemmatizer()
        {
            switch (mLanguage)
            {
                case Language.Slovene:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Slovene);
                    return true;
                case Language.Bulgarian:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Bulgarian);
                    return true;
                case Language.Czech:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Czech);
                    return true;
                case Language.Estonian:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Estonian);
                    return true;
                case Language.Hungarian:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Hungarian);
                    return true;
                case Language.Romanian:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Romanian);
                    return true;
                case Language.Serbian:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Serbian);
                    return true;
                case Language.English:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.English);
                    return true;
                case Language.French:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.French);
                    return true;
                case Language.German:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.German);
                    return true;
                case Language.Italian:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Italian);
                    return true;
                case Language.Spanish:
                    mLemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Spanish);
                    return true;
                default:
                    return false;
            }
        }

        // *** IStemmer interface implementation ***

        public string GetStem(string word)
        {
            Utils.ThrowException(word == null ? new ArgumentNullException("word") : null);
            return mLemmatizer.Lemmatize(word);
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
            CreateLemmatizer();
        }
    }
}
