/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IDistance.cs
 *  Desc:    Distance measure interface 
 *  Created: Nov-2009
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IDistance<T>
       |
       '-----------------------------------------------------------------------
    */
    public interface IDistance<T> : ISerializable
    {
        double GetDistance(T a, T b);
    }
}
