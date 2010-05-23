/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Ref.cs
 *  Desc:    Enables referencing a value type
 *  Created: Feb-2010
 *
 *  Authors: Miha Grcar
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

        public static implicit operator T(Ref<T> refVal)
        {
            return refVal.Val;
        }

        public static implicit operator Ref<T>(T val)
        {
            return new Ref<T>(val);
        }
    }
}
