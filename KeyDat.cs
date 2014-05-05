/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    KeyDat.cs
 *  Desc:    Dictionary item data structure 
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
       |  Struct KeyDat<KeyT, DatT>
       |
       '-----------------------------------------------------------------------
    */
    public struct KeyDat<KeyT, DatT> : IPair<KeyT, DatT>, IComparable<KeyDat<KeyT, DatT>>, IComparable, IEquatable<KeyDat<KeyT, DatT>>, ISerializable where KeyT : IComparable<KeyT>
    {
        private KeyT mKey;
        private DatT mDat;

        public KeyDat(BinarySerializer reader)
        {
            mKey = default(KeyT);
            mDat = default(DatT);
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public KeyDat(KeyT key, DatT dat)
        {
            Utils.ThrowException(key == null ? new ArgumentNullException("key") : null);
            mKey = key;
            mDat = dat;
        }

        public KeyDat(KeyT key)
        {
            Utils.ThrowException(key == null ? new ArgumentNullException("key") : null);
            mKey = key;
            mDat = default(DatT);
        }

        public KeyT Key
        {
            get { return mKey; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Key") : null);
                mKey = value;
            }
        }

        public DatT Dat
        {
            get { return mDat; }
            set { mDat = value; }
        }

        public override int GetHashCode()
        {
            return mKey.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("( {0} {1} )", mKey, mDat);
        }

        public static bool operator ==(KeyDat<KeyT, DatT> a, KeyDat<KeyT, DatT> b)
        {
            return a.mKey.CompareTo(b.mKey) == 0;
        }

        public static bool operator !=(KeyDat<KeyT, DatT> a, KeyDat<KeyT, DatT> b)
        {
            return a.mKey.CompareTo(b.mKey) != 0;
        }

        public static bool operator >(KeyDat<KeyT, DatT> a, KeyDat<KeyT, DatT> b)
        {
            return a.mKey.CompareTo(b.mKey) > 0;
        }

        public static bool operator <(KeyDat<KeyT, DatT> a, KeyDat<KeyT, DatT> b)
        {
            return a.mKey.CompareTo(b.mKey) < 0;
        }

        public static bool operator >=(KeyDat<KeyT, DatT> a, KeyDat<KeyT, DatT> b)
        {
            return a.mKey.CompareTo(b.mKey) >= 0;
        }

        public static bool operator <=(KeyDat<KeyT, DatT> a, KeyDat<KeyT, DatT> b)
        {
            return a.mKey.CompareTo(b.mKey) <= 0;
        }

        // *** IPair<KeyT, DatT> interface implementation ***

        public KeyT First
        {
            get { return mKey; }
        }

        public DatT Second
        {
            get { return mDat; }
        }

        // *** IComparable<KeyDat<KeyT, DatT>> interface implementation ***

        public int CompareTo(KeyDat<KeyT, DatT> other)
        {
            return mKey.CompareTo(other.Key);
        }

        // *** IComparable interface implementation ***

        int IComparable.CompareTo(object obj)
        {
            Utils.ThrowException(!(obj is KeyDat<KeyT, DatT>) ? new ArgumentTypeException("obj") : null);
            return CompareTo((KeyDat<KeyT, DatT>)obj);
        }

        // *** IEquatable<KeyDat<KeyT, DatT>> interface implementation ***

        public bool Equals(KeyDat<KeyT, DatT> other)
        {
            return other.mKey.Equals(mKey);
        }

        public override bool Equals(object obj)
        {
            Utils.ThrowException(!(obj is KeyDat<KeyT, DatT>) ? new ArgumentTypeException("obj") : null);
            return Equals((KeyDat<KeyT, DatT>)obj);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteValueOrObject<KeyT>(mKey); 
            writer.WriteValueOrObject<DatT>(mDat);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mKey = reader.ReadValueOrObject<KeyT>(); 
            mDat = reader.ReadValueOrObject<DatT>(); 
        }
    }
}
