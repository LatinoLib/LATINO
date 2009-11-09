/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Lemmatizer.cs
 *  Version:       1.0
 *  Desc:		   LemmaSharp word lemmatizer (LATINO wrapper)
 *  Author:        Miha Grcar
 *  Created on:    Jan-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using LemmaSharp;

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
        private Language m_language;
        private LemmaSharp.Lemmatizer m_lemmatizer;

        public Lemmatizer(Language language)
        {
            m_language = language;
            bool success = CreateLemmatizer();
            Utils.ThrowException(!success ? new ArgumentNotSupportedException("language") : null);
        }

        public Lemmatizer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        private bool CreateLemmatizer()
        {
            switch (m_language)
            {
                case Language.Slovene:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Slovene);
                    return true;
                case Language.Bulgarian:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Bulgarian);
                    return true;
                case Language.Czech:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Czech);
                    return true;
                case Language.Estonian:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Estonian);
                    return true;
                case Language.Hungarian:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Hungarian);
                    return true;
                case Language.Romanian:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Romanian);
                    return true;
                case Language.Serbian:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Serbian);
                    return true;
                case Language.English:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.English);
                    return true;
                case Language.French:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.French);
                    return true;
                case Language.German:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.German);
                    return true;
                case Language.Italian:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Italian);
                    return true;
                case Language.Spanish:
                    m_lemmatizer = new LemmatizerPrebuiltCompressed(LanguagePrebuilt.Spanish);
                    return true;
                default:
                    return false;
            }
        }

        // *** IStemmer interface implementation ***

        public string GetStem(string word)
        {
            Utils.ThrowException(word == null ? new ArgumentNullException("word") : null);
            return m_lemmatizer.Lemmatize(word);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            writer.WriteInt((int)m_language);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions 
            m_language = (Language)reader.ReadInt();
            CreateLemmatizer();
        }
    }
}
