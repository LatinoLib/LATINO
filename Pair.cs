/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Pair.cs
 *  Desc:    Pair data structure 
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Struct Pair<FirstT, SecondT>
       |
       '-----------------------------------------------------------------------
    */
    public struct Pair<FirstT, SecondT> : IPair<FirstT, SecondT>, IDeeplyCloneable<Pair<FirstT, SecondT>>, IContentEquatable<Pair<FirstT, SecondT>>,
        IComparable<Pair<FirstT, SecondT>>, IComparable, ISerializable
    {
        private FirstT mFirst;
        private SecondT mSecond;

        public Pair(BinarySerializer reader)
        {
            mFirst = default(FirstT);
            mSecond = default(SecondT);
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public Pair(FirstT first, SecondT second)
        {
            mFirst = first;
            mSecond = second;
        }

        public FirstT First
        {
            get { return mFirst; }
            set { mFirst = value; }
        }

        public SecondT Second
        {
            get { return mSecond; }
            set { mSecond = value; }
        }

        public override string ToString()
        {
            return string.Format("( {0} {1} )", mFirst, mSecond);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Pair<FirstT, SecondT> first, Pair<FirstT, SecondT> second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(Pair<FirstT, SecondT> first, Pair<FirstT, SecondT> second)
        {
            return !first.Equals(second);
        }

        // *** IDeeplyCloneable interface implementation ***

        public Pair<FirstT, SecondT> DeepClone()
        {
            return new Pair<FirstT, SecondT>((FirstT)Utils.Clone(mFirst, /*deepClone=*/true), (SecondT)Utils.Clone(mSecond, /*deepClone=*/true));
        }

        object IDeeplyCloneable.DeepClone()
        {
            return DeepClone();
        }

        // *** IContentEquatable<Pair<FirstT, SecondT>> interface implementation ***

        public bool ContentEquals(Pair<FirstT, SecondT> other)
        {
            return Utils.ObjectEquals(mFirst, other.mFirst, /*deepCmp=*/true) && Utils.ObjectEquals(mSecond, other.mSecond, /*deepCmp=*/true);
        }

        bool IContentEquatable.ContentEquals(object other)
        {
            Utils.ThrowException(other == null ? new ArgumentNullException("other") : null);
            Utils.ThrowException(!(other is Pair<FirstT, SecondT>) ? new ArgumentTypeException("other") : null);
            return ContentEquals((Pair<FirstT, SecondT>)other);
        }

        // *** IComparable<Pair<FirstT, SecondT>> interface implementation ***

        public int CompareTo(Pair<FirstT, SecondT> other)
        {
            if (mFirst == null && other.mFirst == null) { return 0; }
            else if (mFirst == null) { return 1; }
            else if (other.mFirst == null) { return -1; }
            else
            {
                int val = ((IComparable<FirstT>)mFirst).CompareTo(other.mFirst); // throws InvalidCastException
                if (val != 0) { return val; }
                else if (mSecond == null && other.mSecond == null) { return 0; }
                else if (mSecond == null) { return 1; }
                else if (other.mSecond == null) { return -1; }
                else { return ((IComparable<SecondT>)mSecond).CompareTo(other.mSecond); } // throws InvalidCastException
            }
        }

        // *** IComparable interface implementation ***

        int IComparable.CompareTo(object obj)
        {
            Utils.ThrowException(!(obj == null || obj is Pair<FirstT, SecondT>) ? new ArgumentTypeException("obj") : null);
            return CompareTo((Pair<FirstT, SecondT>)obj);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteValueOrObject<FirstT>(mFirst); 
            writer.WriteValueOrObject<SecondT>(mSecond); 
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mFirst = reader.ReadValueOrObject<FirstT>(); 
            mSecond = reader.ReadValueOrObject<SecondT>(); 
        }

        // *** Equality comparer ***

        public static IEqualityComparer<Pair<FirstT, SecondT>> GetEqualityComparer()
        {
            return GenericEqualityComparer<Pair<FirstT, SecondT>>.Instance;
        }
    }
}
