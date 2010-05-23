/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ISimilarity.cs
 *  Desc:    Interface definition
 *  Created: Aug-2007
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface ISimilarity<T>
       |
       '-----------------------------------------------------------------------
    */
    public interface ISimilarity<T> : ISerializable
    {
        double GetSimilarity(T a, T b);
    }
}
