/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SetEqualityComparer.cs
 *  Desc:    Set equality comparer
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class SetEqualityComparer<T>
       |
       '-----------------------------------------------------------------------
    */
    public class SetEqualityComparer<T> : IEqualityComparer<Set<T>>, IEqualityComparer<Set<T>.ReadOnly>, IEqualityComparer 
    {
        private static SetEqualityComparer<T> mInstance
            = new SetEqualityComparer<T>();

        public static SetEqualityComparer<T> Instance
        {
            get { return mInstance; }
        }

        public bool Equals(Set<T> x, Set<T> y)
        {
            if (x == null && y == null) { return true; }
            if (x == null || y == null) { return false; }
            return x.Count == y.Count && Set<T>.Difference(x, y).Count == 0;
        }

        public bool Equals(Set<T>.ReadOnly x, Set<T>.ReadOnly y)
        {
            if (x == null && y == null) { return true; }
            if (x == null || y == null) { return false; }
            return Equals(x.Inner, y.Inner);
        }

        public int GetHashCode(Set<T> obj)
        {
            Utils.ThrowException(obj == null ? new ArgumentNullException("obj") : null);
            int hashCode = 0;
            foreach (T item in obj) { hashCode ^= item.GetHashCode(); }
            return hashCode;
        }

        public int GetHashCode(Set<T>.ReadOnly obj)
        {
            Utils.ThrowException(obj == null ? new ArgumentNullException("obj") : null);
            return GetHashCode(obj.Inner);
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            if (x is Set<T>) { x = new Set<T>.ReadOnly((Set<T>)x); }
            if (y is Set<T>) { y = new Set<T>.ReadOnly((Set<T>)y); }
            Utils.ThrowException((x != null && !(x is Set<T>.ReadOnly)) ? new ArgumentTypeException("x") : null);
            Utils.ThrowException((y != null && !(y is Set<T>.ReadOnly)) ? new ArgumentTypeException("y") : null);
            return Equals((Set<T>.ReadOnly)x, (Set<T>.ReadOnly)y);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            if (obj is Set<T>) { obj = new Set<T>.ReadOnly((Set<T>)obj); }
            Utils.ThrowException((obj != null && !(obj is Set<T>.ReadOnly)) ? new ArgumentTypeException("obj") : null);
            return GetHashCode((Set<T>.ReadOnly)obj); // throws ArgumentNullException
        }
    }
}
