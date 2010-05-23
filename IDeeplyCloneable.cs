/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IDeeplyCloneable.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IDeeplyCloneable
       |
       '-----------------------------------------------------------------------
    */
    public interface IDeeplyCloneable
    {
        object DeepClone();
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IDeeplyCloneable<T>
       |
       '-----------------------------------------------------------------------
    */
    public interface IDeeplyCloneable<T> : IDeeplyCloneable
    {
        new T DeepClone();
    }
}