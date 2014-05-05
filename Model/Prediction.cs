/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Prediction.cs
 *  Desc:    Prediction data structure (output of predictive models)
 *  Created: Aug-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Prediction<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class Prediction<LblT> : IEnumerableList<KeyDat<double, LblT>>, ISerializable, ICloneable<Prediction<LblT>>
    {
        private ArrayList<KeyDat<double, LblT>> mClassScores
            = new ArrayList<KeyDat<double, LblT>>();

        public Prediction()
        {
        }

        public Prediction(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public Prediction(IEnumerable<KeyDat<double, LblT>> classScores)
        {
            Utils.ThrowException(classScores == null ? new ArgumentNullException("classScores") : null);
            AddRange(classScores);
        }

        public void AddRange(IEnumerable<KeyDat<double, LblT>> classScores)
        {
            foreach (KeyDat<double, LblT> classScore in classScores)
            {
                mClassScores.Add(classScore);
            }
            mClassScores.Sort(DescSort<KeyDat<double, LblT>>.Instance);
        }

        public ArrayList<KeyDat<double, LblT>> Inner
        {
            get { return mClassScores; }
        }

        public double GetScoreAt(int idx)
        {
            Utils.ThrowException((idx < 0 || idx >= mClassScores.Count) ? new ArgumentOutOfRangeException("idx") : null);
            return mClassScores[idx].Key;
        }

        public LblT GetClassLabelAt(int idx)
        {
            Utils.ThrowException((idx < 0 || idx >= mClassScores.Count) ? new ArgumentOutOfRangeException("idx") : null);
            return mClassScores[idx].Dat;
        }

        public double BestScore
        {
            get
            {
                Utils.ThrowException(mClassScores.Count == 0 ? new InvalidOperationException() : null);
                return mClassScores[0].Key;
            }
        }

        public LblT BestClassLabel
        {
            get
            {
                Utils.ThrowException(mClassScores.Count == 0 ? new InvalidOperationException() : null);
                return mClassScores[0].Dat;
            }
        }

        public void Trim()
        {
            ArrayList<KeyDat<double, LblT>> scores = new ArrayList<KeyDat<double, LblT>>();
            foreach (KeyDat<double, LblT> item in mClassScores)
            {
                if (item.Key == 0) { break; }
                scores.Add(item);
            }
            mClassScores = scores;
        }

        public override string ToString()
        {
            return mClassScores.ToString();
        }

        // *** ICloneable<Prediction<LblT>> interface implementation ***

        public Prediction<LblT> Clone()
        {
            Prediction<LblT> clone = new Prediction<LblT>();
            clone.Inner.AddRange(mClassScores);
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IEnumerableList<KeyDat<double, LblT>> interface implementation ***

        public int Count
        {
            get { return mClassScores.Count; }
        }

        public KeyDat<double, LblT> this[int idx]
        {
            get
            {
                Utils.ThrowException((idx < 0 || idx >= mClassScores.Count) ? new ArgumentOutOfRangeException("idx") : null);
                return mClassScores[idx];
            }
        }

        object IEnumerableList.this[int idx]
        {
            get { return this[idx]; } // throws ArgumentOutOfRangeException
        }

        public IEnumerator<KeyDat<double, LblT>> GetEnumerator()
        {
            return new ListEnum<KeyDat<double, LblT>>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ListEnum(this);
        }

        // *** ISerializable interface implementation

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            mClassScores.Save(writer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mClassScores = new ArrayList<KeyDat<double, LblT>>(reader);
        }
    }
}