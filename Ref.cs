/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Ref.cs
 *  Version:       1.0
 *  Desc:		   Enables referencing a value type
 *  Author:        Miha Grcar
 *  Created on:    Feb-2010
 *  Last modified: Feb-2010
 *  Revision:      Feb-2010
 *
 ***************************************************************************/

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Ref<T>
       |
       '-----------------------------------------------------------------------
    */
    public class Ref<T>
    {
        public T Val;

        public Ref()
        {
        }

        public Ref(T val)
        {
            Val = val;
        }

        public static implicit operator T(Ref<T> ref_val)
        {
            return ref_val.Val;
        }

        public static implicit operator Ref<T>(T val)
        {
            return new Ref<T>(val);
        }
    }
}
