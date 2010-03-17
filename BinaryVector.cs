/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          BinaryVector.cs
 *  Version:       1.0
 *  Desc:		   Binary vector data structure 
 *  Author:        Miha Grcar
 *  Created on:    Oct-2009
 *  Last modified: Oct-2009
 *  Revision:      Oct-2009
 *
 ***************************************************************************/

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class BinaryVector<T>
       |
       '-----------------------------------------------------------------------
    */
    public class BinaryVector<T> : ICollection<T>, ICollection, IEnumerable<T>, ICloneable<BinaryVector<T>>, IDeeplyCloneable<BinaryVector<T>>, IContentEquatable<BinaryVector<T>>, 
        ISerializable where T : IComparable<T>
    {
        private ArrayList<T> mVec
            = new ArrayList<T>();

        public BinaryVector()
        {
        }

        public BinaryVector(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public BinaryVector(IEnumerable<T> items)
        {
            AddRange(items); // throws ArgumentNullException
        }

#if PUBLIC_INNER
        public
#else
        internal
#endif
        List<T> Inner
        {
            get { return mVec; }
        }

        private void RemoveDuplicates()
        {
            int j = 0;
            int i = 1;
            for (; i < mVec.Count; i++)
            {
                if (mVec[j].CompareTo(mVec[i]) < 0)
                {
                    if (i != ++j)
                    {
                        mVec[j] = mVec[i];
                    }
                }
            }
            if (i != ++j) 
            { 
                mVec.RemoveRange(j, mVec.Count - j); 
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            Utils.ThrowException(items == null ? new ArgumentNullException("items") : null);
            int oldLen = mVec.Count;
#if THROW_EXCEPTIONS
            foreach (T item in items) { Utils.ThrowException(item == null ? new ArgumentValueException("items") : null); }
#endif
            mVec.AddRange(items);
            if (mVec.Count == oldLen) { return; }
            mVec.Sort();
            RemoveDuplicates();
        }

        public T[] ToArray()
        {
            T[] array = new T[mVec.Count];
            CopyTo(array, 0);
            return array;
        }

        public T this[int idx]
        {
            get { return mVec[idx]; } // throws ArgumentOutOfRangeException
        }

        public NewT[] ToArray<NewT>()
        {
            return mVec.ToArray<NewT>(/*fmtProvider=*/null); // throws InvalidCastException, FormatException, OverflowException
        }

        public NewT[] ToArray<NewT>(IFormatProvider fmtProvider)
        {
            return mVec.ToArray<NewT>(fmtProvider); // throws InvalidCastException, FormatException, OverflowException
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            Utils.ThrowException(items == null ? new ArgumentNullException("items") : null);
            Set<T> tmp = new Set<T>(items); // throws ArgumentNullException
            ArrayList<T> newVec = new ArrayList<T>(mVec.Count);
            foreach (T item in mVec)
            {
                if (!tmp.Contains(item)) { newVec.Add(item); }
            }
            mVec = newVec;
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder("(");
            foreach (T item in mVec)
            {
                strBuilder.Append(" ");
                strBuilder.Append(item);
            }
            strBuilder.Append(" )");
            return strBuilder.ToString();
        }

        // *** ICollection<T> interface implementation ***

        public void Add(T item)
        {
            Utils.ThrowException(item == null ? new ArgumentNullException("item") : null);
            int idx = mVec.BinarySearch(item);
            if (idx < 0) { idx = ~idx; }
            mVec.Insert(idx, item);
        }

        public void Clear()
        {
            mVec.Clear();
        }

        public bool Contains(T item)
        {
            Utils.ThrowException(item == null ? new ArgumentNullException("item") : null);
            return mVec.BinarySearch(item) >= 0;
        }

        public void CopyTo(T[] array, int index)
        {
            Utils.ThrowException(array == null ? new ArgumentNullException("array") : null);
            Utils.ThrowException(index + mVec.Count > array.Length ? new ArgumentOutOfRangeException("index") : null);
            foreach (T item in mVec)
            {
                array.SetValue(item, index++);
            }
        }

        public int Count
        {
            get { return mVec.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            Utils.ThrowException(item == null ? new ArgumentNullException("item") : null);
            int idx = mVec.BinarySearch(item);
            if (idx >= 0)
            {
                mVec.RemoveAt(idx);
                return true;
            }
            return false;
        }

        // *** ICollection interface implementation ***

        void ICollection.CopyTo(Array array, int index)
        {
            Utils.ThrowException(array == null ? new ArgumentNullException("array") : null);
            Utils.ThrowException(index + mVec.Count > array.Length ? new ArgumentOutOfRangeException("index") : null);
            foreach (T item in mVec)
            {
                array.SetValue(item, index++);
            }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotSupportedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        // *** IEnumerable<T> interface implementation ***

        public IEnumerator<T> GetEnumerator()
        {
            return mVec.GetEnumerator();
        }

        // *** IEnumerable interface implementation ***

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ICloneable interface implementation ***

        public BinaryVector<T> Clone()
        {
            BinaryVector<T> clone = new BinaryVector<T>();
            clone.mVec = mVec.Clone();
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable interface implementation ***

        public BinaryVector<T> DeepClone()
        {
            BinaryVector<T> clone = new BinaryVector<T>();
            clone.mVec = mVec.DeepClone();
            return clone;
        }

        object IDeeplyCloneable.DeepClone()
        {
            return DeepClone();
        }

        // *** IContentEquatable<BinaryVector<T>> interface implementation ***

        public bool ContentEquals(BinaryVector<T> other)
        {
            if (other == null || mVec.Count != other.mVec.Count) { return false; }
            for (int i = 0; i < mVec.Count; i++)
            {
                if (mVec[i].CompareTo(other.mVec[i]) != 0) { return false; }
            }
            return true;
        }

        bool IContentEquatable.ContentEquals(object other)
        {
            Utils.ThrowException((other != null && !(other is BinaryVector<T>)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((BinaryVector<T>)other);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            writer.WriteInt(mVec.Count);
            foreach (T item in mVec)
            {
                writer.WriteValueOrObject<T>(item);
            }
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mVec.Clear();
            // the following statements throw serialization-related exceptions 
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                mVec.Add(reader.ReadValueOrObject<T>());
            }
        }

        // *** Implicit cast to a read-only adapter ***

        public static implicit operator BinaryVector<T>.ReadOnly(BinaryVector<T> vec)
        {
            if (vec == null) { return null; }
            return new BinaryVector<T>.ReadOnly(vec);
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class BinaryVector<T>.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<BinaryVector<T>>, ICollection, IEnumerable<T>, IEnumerable, IContentEquatable<BinaryVector<T>.ReadOnly>, ISerializable
        {
            private BinaryVector<T> mVec;

            public ReadOnly(BinaryVector<T> vec)
            {
                Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
                mVec = vec;
            }

            public ReadOnly(BinarySerializer reader)
            {
                mVec = new BinaryVector<T>(reader); 
            }

            public T[] ToArray()
            {
                return mVec.ToArray();
            }

            public T this[int idx]
            {
                get { return mVec[idx]; }
            }

            public NewT[] ToArray<NewT>()
            {
                return mVec.ToArray<NewT>();
            }

            public NewT[] ToArray<NewT>(IFormatProvider fmtProvider)
            {
                return mVec.ToArray<NewT>(fmtProvider);
            }

            public override string ToString()
            {
                return mVec.ToString();
            }

            // *** IReadOnlyAdapter interface implementation ***

            public BinaryVector<T> GetWritableCopy()
            {
                return mVec.Clone();
            }

            object IReadOnlyAdapter.GetWritableCopy()
            {
                return GetWritableCopy();
            }

#if PUBLIC_INNER
            public
#else
            internal
#endif
            BinaryVector<T> Inner
            {
                get { return mVec; }
            }

            // *** Partial ICollection<T> interface implementation ***

            public bool Contains(T item)
            {
                return mVec.Contains(item);
            }

            public void CopyTo(T[] array, int index)
            {
                mVec.CopyTo(array, index);
            }

            public int Count
            {
                get { return mVec.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            // *** ICollection interface implementation ***

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)mVec).CopyTo(array, index);
            }

            bool ICollection.IsSynchronized
            {
                get { throw new NotSupportedException(); }
            }

            object ICollection.SyncRoot
            {
                get { throw new NotSupportedException(); }
            }

            // *** IEnumerable<T> interface implementation ***

            public IEnumerator<T> GetEnumerator()
            {
                return mVec.GetEnumerator();
            }

            // *** IEnumerable interface implementation ***

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)mVec).GetEnumerator();
            }

            // *** IContentEquatable<BinaryVector<T>.ReadOnly> interface implementation ***

            public bool ContentEquals(BinaryVector<T>.ReadOnly other)
            {
                return other != null && mVec.ContentEquals(other.Inner);
            }

            bool IContentEquatable.ContentEquals(object other)
            {
                Utils.ThrowException((other != null && !(other is BinaryVector<T>.ReadOnly)) ? new ArgumentTypeException("other") : null);
                return ContentEquals((BinaryVector<T>.ReadOnly)other);
            }

            // *** ISerializable interface implementation ***

            public void Save(BinarySerializer writer)
            {
                mVec.Save(writer);
            }
        }
    }
}
