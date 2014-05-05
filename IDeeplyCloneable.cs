/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IDeeplyCloneable.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
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