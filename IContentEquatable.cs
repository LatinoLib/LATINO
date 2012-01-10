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
 *  License: GNU LGPL (http://www.gnu.org/licenses/lgpl.txt)
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