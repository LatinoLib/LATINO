﻿/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    GenericEqualityComparer.cs
 *  Desc:    Generic equality comparer
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System.Collections.Generic;
using System.Collections;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class GenericEqualityComparer<T>
       |
       '-----------------------------------------------------------------------
    */
    public class GenericEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer where T : ISerializable
    {
        private static GenericEqualityComparer<T> mInstance
            = new GenericEqualityComparer<T>();

        public static GenericEqualityComparer<T> Instance
        {
            get { return mInstance; }
        }

        public bool Equals(T x, T y)
        {
            return Utils.ObjectEquals(x, y, /*deepCmp=*/true);
        }

        public int GetHashCode(T obj)
        {
            return Utils.GetHashCode(obj); // throws ArgumentNullException
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            Utils.ThrowException((x != null && !(x is T)) ? new ArgumentTypeException("x") : null);
            Utils.ThrowException((y != null && !(y is T)) ? new ArgumentTypeException("y") : null);
            return Equals((T)x, (T)y);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            Utils.ThrowException((obj != null && !(obj is T)) ? new ArgumentTypeException("obj") : null);
            return GetHashCode((T)obj); // throws ArgumentNullException
        }
    }
}
