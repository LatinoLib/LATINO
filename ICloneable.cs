/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ICloneable.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface ICloneable<T>
       |
       '-----------------------------------------------------------------------
    */
    public interface ICloneable<T> : ICloneable
    {
        new T Clone();
    }
}