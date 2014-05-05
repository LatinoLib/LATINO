/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    DescSort.cs
 *  Desc:    Utility class for descending sort
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
       |  Class DescSort
       |
       '-----------------------------------------------------------------------
    */
    public class DescSort : IComparer<object>, IComparer
    {
        private static DescSort mInstance
            = new DescSort();

        public static DescSort Instance
        {
            get { return mInstance; }
        }

        public int Compare(object x, object y)
        {
            if (x == null && y == null) { return 0; }
            else if (x == null) { return -1; }
            else if (y == null) { return 1; }
            else { return ((IComparable)y).CompareTo(x); } // throws InvalidCastException
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class DescSort<T>
       |
       '-----------------------------------------------------------------------
    */
    public class DescSort<T> : IComparer<T> where T : IComparable<T>
    {
        private static DescSort<T> mInstance
            = new DescSort<T>();

        public static DescSort<T> Instance
        {
            get { return mInstance; }
        }

        public int Compare(T x, T y)
        {
            if (x == null && y == null) { return 0; }
            else if (x == null) { return -1; }
            else if (y == null) { return 1; }
            else { return y.CompareTo(x); } 
        }
    }
}
