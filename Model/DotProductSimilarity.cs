/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    DotProductSimilarity.cs
 *  Desc:    Dot product similarity measure
 *  Created: Dec-2008
 *
 *  Authors: Miha Grcar, Matjaz Jursic
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class DotProductSimilarity
       |
       '-----------------------------------------------------------------------
    */
    public class DotProductSimilarity : ISimilarity<SparseVector<double>.ReadOnly>, ISimilarity<SparseVector<double>>
    {
        public static DotProductSimilarity mInstance
            = new DotProductSimilarity();

        public DotProductSimilarity()
        {
        }

        public DotProductSimilarity(BinarySerializer reader)
        {
        }

        public static DotProductSimilarity Instance
        {
            get { return mInstance; }
        }

        // *** ISimilarity<SparseVector<double>> interface implementation ***

        public double GetSimilarity(SparseVector<double> a, SparseVector<double> b)
        {
            return GetSimilarity(new SparseVector<double>.ReadOnly(a), new SparseVector<double>.ReadOnly(b));
        }

        // *** ISimilarity<SparseVector<double>.ReadOnly> interface implementation ***

        public double GetSimilarity(SparseVector<double>.ReadOnly a, SparseVector<double>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            double dotProd = 0;
            int i = 0, j = 0;
            int aCount = a.Count;
            int bCount = b.Count;
            if (aCount == 0 || bCount == 0) { return 0; }
            ArrayList<int> aIdx = a.Inner.InnerIdx;
            ArrayList<double> aDat = a.Inner.InnerDat;
            ArrayList<int> bIdx = b.Inner.InnerIdx;
            ArrayList<double> bDat = b.Inner.InnerDat;            
            int aIdx_i = aIdx[0];
            int bIdx_j = bIdx[0];
            while (true)
            {
                if (aIdx_i < bIdx_j)
                {
                    if (++i == aCount) { break; }
                    aIdx_i = aIdx[i];
                }
                else if (aIdx_i > bIdx_j)
                {
                    if (++j == bCount) { break; }
                    bIdx_j = bIdx[j];
                }
                else
                {
                    dotProd += aDat[i] * bDat[j];
                    if (++i == aCount || ++j == bCount) { break; }
                    aIdx_i = aIdx[i];
                    bIdx_j = bIdx[j];                    
                }
            }
            return dotProd;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
        }
    }
}