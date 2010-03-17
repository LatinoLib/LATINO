/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          CosineSimilarity.cs
 *  Version:       1.0
 *  Desc:		   Similarity implementation
 *  Authors:       Miha Grcar, Matjaz Jursic
 *  Created on:    Dec-2008
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class CosineSimilarity
       |
       '-----------------------------------------------------------------------
    */
    public class CosineSimilarity : ISimilarity<SparseVector<double>.ReadOnly>
    {
        public CosineSimilarity()
        {
        }

        public CosineSimilarity(BinarySerializer reader)
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
            Utils.ThrowException(aCount == 0 ? new ArgumentValueException("a") : null);
            int bCount = b.Count;
            Utils.ThrowException(bCount == 0 ? new ArgumentValueException("b") : null);
            ArrayList<int> aIdx = a.Inner.InnerIdx;
            ArrayList<double> aDat = a.Inner.InnerDat;
            ArrayList<int> bIdx = b.Inner.InnerIdx;
            ArrayList<double> bDat = b.Inner.InnerDat;
            int aIdxI = aCount == 0 ? 0 : aIdx[0];
            int bIdxJ = bCount == 0 ? 0 : bIdx[0];
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
            double lenA = Utils.GetVecLenL2(a);
            Utils.ThrowException(lenA == 0 ? new ArgumentValueException("a") : null);
            double lenB = Utils.GetVecLenL2(b);
            Utils.ThrowException(lenB == 0 ? new ArgumentValueException("b") : null);
            double lenMult = lenA * lenB;
            return dotProd / lenMult;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
        }
    }
}