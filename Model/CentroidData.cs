/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          CentroidData.cs
 *  Version:       1.0
 *  Desc:		   Centroid data structure (used for speed optimizations)
 *  Author:        Miha Grcar 
 *  Created on:    May-2009
 *  Last modified: Mar-2010
 *  Revision:      Mar-2010
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
    internal class CentroidData : ISerializable
    {
        private Dictionary<int, double> CentroidSum
            = new Dictionary<int, double>();
        private Dictionary<int, double> CentroidDiff
            = new Dictionary<int, double>();
        private Set<int> mItems
            = new Set<int>();
        private Set<int> mCurrentItems
            = new Set<int>();
        private double CentroidLen
            = -1;

        public CentroidData()
        {
        }

        public CentroidData(BinarySerializer reader)
        {
            Load(reader);
        }

        public void AddToSum(SparseVector<double>.ReadOnly vec)
        {
            foreach (IdxDat<double> item in vec)
            {
                if (CentroidSum.ContainsKey(item.Idx))
                {
                    CentroidSum[item.Idx] += item.Dat;
                }
                else
                {
                    CentroidSum.Add(item.Idx, item.Dat);
                }
            }
        }

        public void AddToDiff(double mult, SparseVector<double>.ReadOnly vec)
        {
            foreach (IdxDat<double> item in vec)
            {
                if (CentroidDiff.ContainsKey(item.Idx))
                {
                    CentroidDiff[item.Idx] += item.Dat * mult;
                }
                else
                {
                    CentroidDiff.Add(item.Idx, item.Dat * mult);
                }
            }
        }

        public void Update(bool positiveValuesOnly)
        {
            foreach (KeyValuePair<int, double> item in CentroidDiff)
            {
                if (CentroidSum.ContainsKey(item.Key))
                {
                    CentroidSum[item.Key] += item.Value;
                }
                else
                {
                    CentroidSum.Add(item.Key, item.Value);
                }
            }
            CentroidDiff.Clear();
            if (positiveValuesOnly)
            {
                Dictionary<int, double> tmp = new Dictionary<int, double>();
                foreach (KeyValuePair<int, double> item in CentroidSum)
                {
                    if (item.Value > 1E-6)
                    {
                        tmp.Add(item.Key, item.Value);
                    }
                }
                CentroidSum = tmp;
            }
        }

        public void Update(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            Set<int> addIdx = Set<int>.Difference(mItems, mCurrentItems);
            Set<int> rmvIdx = Set<int>.Difference(mCurrentItems, mItems);
            foreach (int itemIdx in addIdx)
            {
                SparseVector<double>.ReadOnly vec = dataset[itemIdx];
                AddToSum(vec);
            }
            foreach (int itemIdx in rmvIdx)
            {
                SparseVector<double>.ReadOnly vec = dataset[itemIdx];
                AddToDiff(-1, vec);
            }
            mCurrentItems = mItems;
            mItems = new Set<int>();
            Update(/*positiveValuesOnly=*/true);
        }

        public void UpdateCentroidLen()
        {
            CentroidLen = 0;
            foreach (double val in CentroidSum.Values)
            {
                CentroidLen += val * val;
            }
            CentroidLen = Math.Sqrt(CentroidLen);
        }

        public Set<int> Items
        {
            get { return mItems; }
        }

        public Set<int>.ReadOnly CurrentItems
        {
            get { return mCurrentItems; }
        }

        public double GetDotProduct(SparseVector<double>.ReadOnly vec)
        {
            double result = 0;
            foreach (IdxDat<double> item in vec)
            {
                if (CentroidSum.ContainsKey(item.Idx))
                {
                    result += item.Dat * CentroidSum[item.Idx];
                }
            }
            return result / CentroidLen;
        }

        public SparseVector<double> GetSparseVector()
        {
            SparseVector<double> vec = new SparseVector<double>();
            foreach (KeyValuePair<int, double> item in CentroidSum)
            {
                vec.InnerIdx.Add(item.Key);
                vec.InnerDat.Add(item.Value / CentroidLen);
            }
            vec.Sort();
            return vec;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.SaveDictionary(CentroidSum, writer);
            writer.WriteDouble(CentroidLen);
        }

        public void Load(BinarySerializer reader)
        {
            CentroidSum = Utils.LoadDictionary<int, double>(reader);
            CentroidLen = reader.ReadDouble();
        }
    }
}
