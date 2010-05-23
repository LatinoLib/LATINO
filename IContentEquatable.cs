/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IContentEquatable.cs
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