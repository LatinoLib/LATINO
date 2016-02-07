/*==========================================================================;
 *
 *  (c) 2012 IINTECH. All rights reserved.
 *
 *  File:    Preconditions.cs
 *  Desc:    google-collections like Preconditions class
 *  Created: Dec-2012
 *
 *  Author:  saxo
 *
 ***************************************************************************/

using System;

namespace Latino
{
    public static class Preconditions
    {
        public static T CheckNotNull<T>(T argument) where T : class 
        {
            if (argument == null) { throw new ArgumentNullException(); }
            return argument;
        }

        public static T CheckNotNull<T>(T argument, string paramName) where T : class 
        {
            if (argument == null) { throw new ArgumentNullException(paramName); }
            return argument;
        }

        public static void CheckArgument(bool valid)
        {
            if (!valid) { throw new ArgumentException(); }
        }

        public static void CheckArgument(bool valid, string message, params object[] args)
        {
            if (!valid) { throw new ArgumentException(string.Format(message, args)); }
        }

        public static void CheckArgumentRange(bool valid)
        {
            if (!valid) { throw new ArgumentOutOfRangeException(); }
        }

        public static void CheckArgumentRange(bool valid, string message, params object[] args)
        {
            if (!valid) { throw new ArgumentOutOfRangeException(string.Format(message, args)); }
        }

        public static void CheckState(bool valid)
        {
            if (!valid) { throw new InvalidOperationException(); }
        }

        public static void CheckState(bool valid, string message, params object[] args)
        {
            if (!valid) { throw new InvalidOperationException(string.Format(message, args)); }
        }
    }
}