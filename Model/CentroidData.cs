/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    CentroidData.cs
 *  Desc:    Centroid data structure (used for speed optimizations)
 *  Created: May-2009
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT) 
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class CentroidData
       |
       '-----------------------------------------------------------------------
    */
    internal class CentroidData : ISerializable, ICloneable<CentroidData>
    {
        private Dictionary<int, double> mCentroidSum
            = new Dictionary<int, double>();
        private Dictionary<int, double> mCentroidDiff
            = new Dictionary<int, double>();
        private Set<int> mItems
            = new Set<int>();
        private Set<int> mCurrentItems
            = new Set<int>();
        private double mCentroidLen
            = -1;
        private object mTag
            = null;

        public CentroidData()
        {
        }

        public CentroidData(BinarySerializer reader)
        {
            Load(reader);
        }

        public object Tag
        {
            get { return mTag; }
            set { mTag = value; }
        }

        public void AddToSum(SparseVector<double> vec)
        {
            foreach (IdxDat<double> item in vec)
            {
                if (mCentroidSum.ContainsKey(item.Idx))
                {
                    mCentroidSum[item.Idx] += item.Dat;
                }
                else
                {
                    mCentroidSum.Add(item.Idx, item.Dat);
                }
            }
        }

        public void AddToDiff(double mult, SparseVector<double> vec)
        {
            foreach (IdxDat<double> item in vec)
            {
                if (mCentroidDiff.ContainsKey(item.Idx))
                {
                    mCentroidDiff[item.Idx] += item.Dat * mult;
                }
                else
                {
                    mCentroidDiff.Add(item.Idx, item.Dat * mult);
                }
            }
        }

        public void Update(bool positiveValuesOnly)
        {
            foreach (KeyValuePair<int, double> item in mCentroidDiff)
            {
                if (mCentroidSum.ContainsKey(item.Key))
                {
                    mCentroidSum[item.Key] += item.Value;
                }
                else
                {
                    mCentroidSum.Add(item.Key, item.Value);
                }
            }
            mCentroidDiff.Clear();
            Dictionary<int, double> tmp = new Dictionary<int, double>();
            foreach (KeyValuePair<int, double> item in mCentroidSum)
            {
                if ((!positiveValuesOnly && Math.Abs(item.Value) > 1E-6) || (positiveValuesOnly && item.Value > 1E-6))
                {
                    tmp.Add(item.Key, item.Value);
                }
            }
            mCentroidSum = tmp;
        }

        public void Update(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            Set<int> addIdx = Set<int>.Difference(mItems, mCurrentItems);
            Set<int> rmvIdx = Set<int>.Difference(mCurrentItems, mItems);
            foreach (int itemIdx in addIdx)
            {
                SparseVector<double> vec = dataset[itemIdx];
                AddToSum(vec);
            }
            foreach (int itemIdx in rmvIdx)
            {
                SparseVector<double> vec = dataset[itemIdx];
                AddToDiff(-1, vec);
            }
            mCurrentItems = mItems;
            mItems = new Set<int>();
            Update(/*positiveValuesOnly=*/false);
        }

        public void UpdateCentroidLen()
        {
            mCentroidLen = 0;
            foreach (double val in mCentroidSum.Values)
            {
                mCentroidLen += val * val;
            }
            mCentroidLen = Math.Sqrt(mCentroidLen);
        }

        public Set<int> Items
        {
            get { return mItems; }
        }

        public Set<int>.ReadOnly CurrentItems
        {
            get { return mCurrentItems; }
        }

        public double GetDotProduct(SparseVector<double> vec)
        {
            double result = 0;
            foreach (IdxDat<double> item in vec)
            {
                double val;
                if (mCentroidSum.TryGetValue(item.Idx, out val))
                {
                    result += item.Dat * val;
                }
            }
            return result / mCentroidLen;
        }

        public SparseVector<double> GetSparseVector()
        {
            SparseVector<double> vec = new SparseVector<double>();
            foreach (KeyValuePair<int, double> item in mCentroidSum)
            {
                vec.InnerIdx.Add(item.Key);
                vec.InnerDat.Add(item.Value / mCentroidLen);
            }
            vec.Sort();
            return vec;
        }

        // *** ICloneable<CentroidData> interface implementation ***

        public CentroidData Clone()
        {
            CentroidData clone = new CentroidData();
            foreach (KeyValuePair<int, double> item in mCentroidSum)
            {
                clone.mCentroidSum.Add(item.Key, item.Value);
            }
            foreach (KeyValuePair<int, double> item in mCentroidDiff)
            {
                clone.mCentroidDiff.Add(item.Key, item.Value);
            }
            clone.mItems.AddRange(mItems);
            clone.mCurrentItems.AddRange(mCurrentItems);
            clone.mCentroidLen = mCentroidLen;
            clone.mTag = mTag; // *** tag is cloned shallowly
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.SaveDictionary(mCentroidSum, writer);
            Utils.SaveDictionary(mCentroidDiff, writer);
            mItems.Save(writer);
            mCurrentItems.Save(writer);
            writer.WriteDouble(mCentroidLen);
            writer.WriteObject(mTag);
        }

        public void Load(BinarySerializer reader)
        {
            mCentroidSum = Utils.LoadDictionary<int, double>(reader);
            mCentroidDiff = Utils.LoadDictionary<int, double>(reader);
            mItems.Load(reader);
            mCurrentItems.Load(reader);
            mCentroidLen = reader.ReadDouble();
            mTag = reader.ReadObject<object>();
        }
    }
}
