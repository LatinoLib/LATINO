/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ISimilarity.cs
 *  Desc:    Similarity measure interface 
 *  Created: Aug-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
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
