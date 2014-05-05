/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IdxDat.cs
 *  Desc:    Indexed item data structure 
 *  Created: Mar-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Struct IdxDat<T>
       |
       '-----------------------------------------------------------------------
    */
    public struct IdxDat<T> : IPair<int, T>, IComparable<IdxDat<T>>, IComparable, IEquatable<IdxDat<T>>, ISerializable
    {
        private int mIdx;
        private T mDat;

        public IdxDat(BinarySerializer reader)
        {
            mIdx = -1;
            mDat = default(T);
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public IdxDat(int idx, T dat)
        {
            mIdx = idx;
            mDat = dat;
        }

        public IdxDat(int idx)
        {
            mIdx = idx;
            mDat = default(T);
        }

        public int Idx
        {
            get { return mIdx; }
        }

        public T Dat
        {
            get { return mDat; }
            set { mDat = value; }
        }

        public override int GetHashCode()
        {
            return mIdx.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("( {0} {1} )", mIdx, mDat);
        }

        public static bool operator ==(IdxDat<T> a, IdxDat<T> b)
        {
            return a.mIdx == b.mIdx;
        }

        public static bool operator !=(IdxDat<T> a, IdxDat<T> b)
        {
            return a.mIdx != b.mIdx;
        }

        public static bool operator >(IdxDat<T> a, IdxDat<T> b)
        {
            return a.mIdx > b.mIdx;
        }

        public static bool operator <(IdxDat<T> a, IdxDat<T> b)
        {
            return a.mIdx < b.mIdx;
        }

        public static bool operator >=(IdxDat<T> a, IdxDat<T> b)
        {
            return a.mIdx >= b.mIdx;
        }

        public static bool operator <=(IdxDat<T> a, IdxDat<T> b)
        {
            return a.mIdx <= b.mIdx;
        }

        // *** IPair<int, T> interface implementation ***

        public int First
        {
            get { return mIdx; }
        }

        public T Second
        {
            get { return mDat; }
        }

        // *** IComparable<IdxDat<T>> interface implementation ***

        public int CompareTo(IdxDat<T> other)
        {
            return mIdx.CompareTo(other.Idx);
        }

        // *** IComparable interface implementation ***

        int IComparable.CompareTo(object obj)
        {
            Utils.ThrowException(!(obj is IdxDat<T>) ? new ArgumentTypeException("obj") : null);
            return CompareTo((IdxDat<T>)obj);
        }

        // *** IEquatable<IdxDat<T>> interface implementation ***

        public bool Equals(IdxDat<T> other)
        {
            return other.mIdx == mIdx;
        }

        public override bool Equals(object obj)
        {
            Utils.ThrowException(!(obj is IdxDat<T>) ? new ArgumentTypeException("obj") : null);
            return Equals((IdxDat<T>)obj);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(mIdx); 
            writer.WriteValueOrObject(mDat); 
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mIdx = reader.ReadInt(); 
            mDat = reader.ReadValueOrObject<T>(); 
        }
    }
}
