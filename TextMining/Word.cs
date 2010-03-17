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
        internal int mIdx
            = -1;
        internal Dictionary<string, int> mForms
            = new Dictionary<string, int>();
        internal string mMostFrequentForm;
        internal int mDocFreq
            = 1;
        internal int mFreq
            = 1;
        internal double mIdf
            = -1;

        internal Word(BinarySerializer reader)
        {
            Load(reader);
        }

        internal Word(string word)
        {
            mMostFrequentForm = word;
            mForms.Add(word, 1);
        }

        public string MostFrequentForm
        {
            get { return mMostFrequentForm; }
        }

        public int DocFreq
        {
            get { return mDocFreq; }
        }

        public int Freq
        {
            get { return mFreq; }
        }

        public double Idf
        {
            get { return mIdf; }
        }

        // *** IEnumerable<KeyValuePair<string, int>> interface implementation ***

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return mForms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mForms.GetEnumerator();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(mIdx);
            writer.WriteString(mMostFrequentForm);
            writer.WriteInt(mDocFreq);
            writer.WriteInt(mFreq);
            writer.WriteDouble(mIdf);
        }

        internal void Load(BinarySerializer reader)
        {
            mIdx = reader.ReadInt();
            mMostFrequentForm = reader.ReadString();
            mDocFreq = reader.ReadInt();
            mFreq = reader.ReadInt();
            mIdf = reader.ReadDouble();
        }
    }
}
