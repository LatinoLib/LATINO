/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IContentEquatable.cs
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
       |  Interface IContentEquatable
       |
       '-----------------------------------------------------------------------
    */
    public interface IContentEquatable
    {
        bool ContentEquals(object other);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IContentEquatable<T>
       |
       '-----------------------------------------------------------------------
    */
    public interface IContentEquatable<T> : IContentEquatable
    {
        bool ContentEquals(T other);
    }
}