/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          DotProductSimilarity.cs
 *  Version:       1.0
 *  Desc:		   Similarity implementation
 *  Authors:       Miha Grcar, Matjaz Jursic
 *  Created on:    Dec-2008
 *  Last modified: Oct-2009
 *  Revision:      Oct-2009
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
    public class DotProductSimilarity : ISimilarity<SparseVector<double>.ReadOnly>
    {
        public DotProductSimilarity()
        {
        }

        public DotProductSimilarity(BinarySerializer reader)
        {
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
            int aIdxI = aIdx[0];
            int bIdxJ = bIdx[0];
            while (true)
            {
                if (aIdxI < bIdxJ)
                {
                    if (++i == aCount) { break; }
                    aIdxI = aIdx[i];
                }
                else if (aIdxI > bIdxJ)
                {
                    if (++j == bCount) { break; }
                    bIdxJ = bIdx[j];
                }
                else
                {
                    dotProd += aDat[i] * bDat[j];
                    if (++i == aCount || ++j == bCount) { break; }
                    aIdxI = aIdx[i];
                    bIdxJ = bIdx[j];                    
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