/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          IDistance.cs
 *  Version:       1.0
 *  Desc:		   Interface definition
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
