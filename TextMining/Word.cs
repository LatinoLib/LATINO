/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Word.cs
 *  Version:       1.0
 *  Desc:		   Word structure (for BOW space vocabulary)
 *  Author:        Miha Grcar
 *  Created on:    Dec-2008
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Word
       |
       '-----------------------------------------------------------------------
    */
    public class Word : IEnumerable<KeyValuePair<string, int>>, ISerializable
    {
        internal int m_idx
            = -1;
        internal Dictionary<string, int> m_forms
            = new Dictionary<string, int>();
        internal string m_most_frequent_form;
        internal int m_doc_freq
            = 1;
        internal int m_freq
            = 1;
        internal double m_idf
            = -1;

        internal Word(BinarySerializer reader)
        {
            Load(reader);
        }

        internal Word(string word)
        {
            m_most_frequent_form = word;
            m_forms.Add(word, 1);
        }

        public string MostFrequentForm
        {
            get { return m_most_frequent_form; }
        }

        public int DocFreq
        {
            get { return m_doc_freq; }
        }

        public int Freq
        {
            get { return m_freq; }
        }

        public double Idf
        {
            get { return m_idf; }
        }

        // *** IEnumerable<KeyValuePair<string, int>> interface implementation ***

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return m_forms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_forms.GetEnumerator();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(m_idx);
            writer.WriteString(m_most_frequent_form);
            writer.WriteInt(m_doc_freq);
            writer.WriteInt(m_freq);
            writer.WriteDouble(m_idf);
        }

        internal void Load(BinarySerializer reader)
        {
            m_idx = reader.ReadInt();
            m_most_frequent_form = reader.ReadString();
            m_doc_freq = reader.ReadInt();
            m_freq = reader.ReadInt();
            m_idf = reader.ReadDouble();
        }
    }
}
