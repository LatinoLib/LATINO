/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    MultiSetEqualityComparer.cs
 *  Desc:    Multi-set equality comparer
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
       |  Class MultiSetEqualityComparer<T>
       |
       '-----------------------------------------------------------------------
    */
    public class MultiSetEqualityComparer<T> : IEqualityComparer<MultiSet<T>>, IEqualityComparer<MultiSet<T>.ReadOnly>, IEqualityComparer
    {
        private static MultiSetEqualityComparer<T> mInstance
            = new MultiSetEqualityComparer<T>();

        public static MultiSetEqualityComparer<T> Instance
        {
            get { return mInstance; }
        }

        public bool Equals(MultiSet<T> x, MultiSet<T> y)
        {
            if (x == null && y == null) { return true; }
            if (x == null || y == null) { return false; }
            return x.Count == y.Count && MultiSet<T>.Difference(x, y).Count == 0;
        }

        public bool Equals(MultiSet<T>.ReadOnly x, MultiSet<T>.ReadOnly y)
        {
            if (x == null && y == null) { return true; }
            if (x == null || y == null) { return false; }
            return Equals(x.Inner, y.Inner);
        }

        public int GetHashCode(MultiSet<T> obj)
        {
            Utils.ThrowException(obj == null ? new ArgumentNullException("obj") : null);
            int hashCode = 0;
            foreach (KeyValuePair<T, int> item in obj)
            {
                hashCode ^= item.Key.GetHashCode() ^ item.Value.GetHashCode();
            }
            return hashCode;
        }

        public int GetHashCode(MultiSet<T>.ReadOnly obj)
        {
            Utils.ThrowException(obj == null ? new ArgumentNullException("obj") : null);
            return GetHashCode(obj.Inner);
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            if (x is MultiSet<T>) { x = new MultiSet<T>.ReadOnly((MultiSet<T>)x); }
            if (y is MultiSet<T>) { y = new MultiSet<T>.ReadOnly((MultiSet<T>)y); }
            Utils.ThrowException((x != null && !(x is MultiSet<T>.ReadOnly)) ? new ArgumentTypeException("x") : null);
            Utils.ThrowException((y != null && !(y is MultiSet<T>.ReadOnly)) ? new ArgumentTypeException("y") : null);
            return Equals((MultiSet<T>.ReadOnly)x, (MultiSet<T>.ReadOnly)y);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            if (obj is MultiSet<T>) { obj = new MultiSet<T>.ReadOnly((MultiSet<T>)obj); }
            Utils.ThrowException((obj != null && !(obj is MultiSet<T>.ReadOnly)) ? new ArgumentTypeException("obj") : null);
            return GetHashCode((MultiSet<T>.ReadOnly)obj); // throws ArgumentNullException
        }
    }
}