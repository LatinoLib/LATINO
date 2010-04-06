/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          SetEqualityComparer.cs
 *  Version:       1.0
 *  Desc:		   Set equality comparer
 *  Author:        Miha Grcar
 *  Created on:    Nov-2007
 *  Last modified: Apr-2010
 *  Revision:      Apr-2010
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
    public class SetEqualityComparer<T> : IEqualityComparer<Set<T>>, IEqualityComparer 
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

        public int GetHashCode(Set<T> obj)
        {
            Utils.ThrowException(obj == null ? new ArgumentNullException("obj") : null);
            int hashCode = 0;
            foreach (T item in obj) { hashCode ^= item.GetHashCode(); }
            return hashCode;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            Utils.ThrowException((x != null && !(x is Set<T>)) ? new ArgumentTypeException("x") : null);
            Utils.ThrowException((y != null && !(y is Set<T>)) ? new ArgumentTypeException("y") : null);
            return Equals((Set<T>)x, (Set<T>)y);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            Utils.ThrowException((obj != null && !(obj is Set<T>)) ? new ArgumentTypeException("obj") : null);
            return GetHashCode((Set<T>)obj); // throws ArgumentNullException
        }
    }
}
