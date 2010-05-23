/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IDistance.cs
 *  Desc:    Interface definition
 *  Created: Nov-2009
 *
 *  Authors: Miha Grcar
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
