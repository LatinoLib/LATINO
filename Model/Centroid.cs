/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Centroid.cs
 *  Version:       1.0
 *  Desc:		   Centroid vector structure (used for speed optimizations)
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Internal class Centroid
       |
       '-----------------------------------------------------------------------
    */
    internal class Centroid
    {
        private IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> mDataset;
        private Set<int> mCurrentItems
            = new Set<int>();
        private Set<int> mItems
            = new Set<int>();
        private Set<int> mNonZeroIdx
            = new Set<int>();
        private double mDiv
            = 1;
        private double[] mVec;

        public Centroid(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, int vecLen)
        {
            mVec = new double[vecLen];
            mDataset = dataset;
        }

        public Centroid(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            int maxIdx = -1;
            foreach (SparseVector<double>.ReadOnly example in dataset)
            {
                int lastIdx = example.LastNonEmptyIndex;
                if (lastIdx > maxIdx) { maxIdx = lastIdx; }
            }
            mVec = new double[maxIdx + 1];
            mDataset = dataset;
        }

        public Set<int> Items
        {
            get { return mItems; }
        }

        public Set<int>.ReadOnly CurrentItems
        {
            get { return mCurrentItems; }
        }

        public int VecLen
        {
            get { return mVec.Length; }
        }

        public void Update()
        { 
            Set<int> addIdx = Set<int>.Difference(mItems, mCurrentItems);
            Set<int> rmvIdx = Set<int>.Difference(mCurrentItems, mItems);
            foreach (int itemIdx in addIdx)
            {
                SparseVector<double>.ReadOnly vec = mDataset[itemIdx];
                foreach (IdxDat<double> item in vec)
                {
                    if (item.Dat != 0)
                    {
                        if (item.Idx >= mVec.Length)
                        { 
                            // extend vector
                            double[] newVec = new double[item.Idx + 1];
                            foreach (int idx in mNonZeroIdx)
                            {
                                newVec[idx] = mVec[idx];
                            }
                            mVec = newVec;
                        }
                        if (Math.Abs(mVec[item.Idx]) < 1E-6) { mNonZeroIdx.Add(item.Idx); }
                        else if (Math.Abs(mVec[item.Idx] + item.Dat) < 1E-6) { mNonZeroIdx.Remove(item.Idx); }
                        mVec[item.Idx] += item.Dat;
                    }
                }
            }
            foreach (int itemIdx in rmvIdx)
            {
                SparseVector<double>.ReadOnly vec = mDataset[itemIdx];
                foreach (IdxDat<double> item in vec)
                {
                    if (item.Dat != 0)
                    {
                        if (Math.Abs(mVec[item.Idx]) < 1E-6) { mNonZeroIdx.Add(item.Idx); }
                        else if (Math.Abs(mVec[item.Idx] - item.Dat) < 1E-6) { mNonZeroIdx.Remove(item.Idx); }
                        mVec[item.Idx] -= item.Dat;
                    }
                }
            }
            mCurrentItems = mItems;
            mItems = new Set<int>();
        }

        public void NormalizeL2()
        {
            if (mDiv == 1)
            {
                double len = 0;
                foreach (int idx in mNonZeroIdx)
                {
                    len += mVec[idx] * mVec[idx];
                }
                len = Math.Sqrt(len);
                foreach (int idx in mNonZeroIdx)
                {
                    mVec[idx] /= len;
                }
                mDiv = len;
            }
        }

        public void ResetNrmL2()
        {
            if (mDiv != 1)
            {
                foreach (int idx in mNonZeroIdx)
                {
                    mVec[idx] *= mDiv;
                }
                mDiv = 1;
            }
        }

        public void Clear()
        {
            foreach (int idx in mNonZeroIdx)
            {
                mVec[idx] = 0;
            }
            mNonZeroIdx.Clear();            
        }

        public SparseVector<double> GetSparseVector()
        {
            SparseVector<double> vec = new SparseVector<double>();
            foreach (int idx in mNonZeroIdx)
            {
                vec.InnerIdx.Add(idx);
                vec.InnerDat.Add(mVec[idx]);
            }
            return vec;
        }

        public double GetDotProduct(SparseVector<double>.ReadOnly vec)
        {
            double dotProd = 0;
            foreach (IdxDat<double> item in vec)
            {
                if (item.Idx < mVec.Length)
                {
                    dotProd += item.Dat * mVec[item.Idx];
                }
            }
            return dotProd;
        }
    }
}
