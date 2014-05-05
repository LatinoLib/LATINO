/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    BinaryVector.cs
 *  Desc:    Binary vector data structure 
 *  Created: Oct-2009
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
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
       |  Class BinaryVector
       |
       '-----------------------------------------------------------------------
    */
    public class BinaryVector : ICollection<int>, ICollection, IEnumerable<int>, ICloneable<BinaryVector>, IDeeplyCloneable<BinaryVector>, IContentEquatable<BinaryVector>, 
        ISerializable 
    {
        private ArrayList<int> mVec
            = new ArrayList<int>();

        public BinaryVector()
        {
        }

        public BinaryVector(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public BinaryVector(IEnumerable<int> items)
        {
            AddRange(items); // throws ArgumentNullException
        }

        public List<int> Inner
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

        public void AddRange(IEnumerable<int> items)
        {
            Utils.ThrowException(items == null ? new ArgumentNullException("items") : null);
            int oldLen = mVec.Count;
#if THROW_EXCEPTIONS
            foreach (int item in items) { Utils.ThrowException(item == null ? new ArgumentValueException("items") : null); }
#endif
            mVec.AddRange(items);
            if (mVec.Count == oldLen) { return; }
            mVec.Sort();
            RemoveDuplicates();
        }

        public int[] ToArray()
        {
            return mVec.ToArray();
        }

        public int this[int idx]
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

        public void RemoveRange(IEnumerable<int> items)
        {
            Utils.ThrowException(items == null ? new ArgumentNullException("items") : null);
            Set<int> tmp = new Set<int>(items); // throws ArgumentNullException
            ArrayList<int> newVec = new ArrayList<int>(mVec.Count);
            foreach (int item in mVec)
            {
                if (!tmp.Contains(item)) { newVec.Add(item); }
            }
            mVec = newVec;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("(");
            foreach (int item in mVec)
            {
                str.Append(" ");
                str.Append(item);
            }
            str.Append(" )");
            return str.ToString();
        }

        // *** ICollection<int> interface implementation ***

        public void Add(int item)
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

        public bool Contains(int item)
        {
            Utils.ThrowException(item == null ? new ArgumentNullException("item") : null);
            return mVec.BinarySearch(item) >= 0;
        }

        public void CopyTo(int[] array, int index)
        {
            Utils.ThrowException(array == null ? new ArgumentNullException("array") : null);
            Utils.ThrowException(index + mVec.Count > array.Length ? new ArgumentOutOfRangeException("index") : null);
            foreach (int item in mVec)
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

        public bool Remove(int item)
        {
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
            foreach (int item in mVec)
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

        // *** IEnumerable<int> interface implementation ***

        public IEnumerator<int> GetEnumerator()
        {
            return mVec.GetEnumerator();
        }

        // *** IEnumerable interface implementation ***

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ICloneable interface implementation ***

        public BinaryVector Clone()
        {
            BinaryVector clone = new BinaryVector();
            clone.mVec = mVec.Clone();
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable interface implementation ***

        public BinaryVector DeepClone()
        {
            BinaryVector clone = new BinaryVector();
            clone.mVec = mVec.DeepClone();
            return clone;
        }

        object IDeeplyCloneable.DeepClone()
        {
            return DeepClone();
        }

        // *** IContentEquatable<BinaryVector> interface implementation ***

        public bool ContentEquals(BinaryVector other)
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
            Utils.ThrowException((other != null && !(other is BinaryVector)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((BinaryVector)other);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            writer.WriteInt(mVec.Count);
            foreach (int item in mVec)
            {
                writer.WriteValueOrObject<int>(item);
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
                mVec.Add(reader.ReadValueOrObject<int>());
            }
        }

        // *** Implicit cast to a read-only adapter ***

        public static implicit operator BinaryVector.ReadOnly(BinaryVector vec)
        {
            if (vec == null) { return null; }
            return new BinaryVector.ReadOnly(vec);
        }

        // *** Equality comparer ***

        public static IEqualityComparer<BinaryVector> GetEqualityComparer()
        {
            return GenericEqualityComparer<BinaryVector>.Instance;
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class BinaryVector.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<BinaryVector>, ICollection, IEnumerable<int>, IEnumerable, IContentEquatable<BinaryVector.ReadOnly>, ISerializable
        {
            private BinaryVector mVec;

            public ReadOnly(BinaryVector vec)
            {
                Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
                mVec = vec;
            }

            public ReadOnly(BinarySerializer reader)
            {
                mVec = new BinaryVector(reader); 
            }

            public int[] ToArray()
            {
                return mVec.ToArray();
            }

            public int this[int idx]
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

            public BinaryVector GetWritableCopy()
            {
                return mVec.Clone();
            }

            object IReadOnlyAdapter.GetWritableCopy()
            {
                return GetWritableCopy();
            }

            public BinaryVector Inner
            {
                get { return mVec; }
            }

            object IReadOnlyAdapter.Inner
            {
                get { return Inner; }
            }

            // *** Partial ICollection<int> interface implementation ***

            public bool Contains(int item)
            {
                return mVec.Contains(item);
            }

            public void CopyTo(int[] array, int index)
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

            // *** IEnumerable<int> interface implementation ***

            public IEnumerator<int> GetEnumerator()
            {
                return mVec.GetEnumerator();
            }

            // *** IEnumerable interface implementation ***

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)mVec).GetEnumerator();
            }

            // *** IContentEquatable<BinaryVector.ReadOnly> interface implementation ***

            public bool ContentEquals(BinaryVector.ReadOnly other)
            {
                return other != null && mVec.ContentEquals(other.Inner);
            }

            bool IContentEquatable.ContentEquals(object other)
            {
                Utils.ThrowException((other != null && !(other is BinaryVector.ReadOnly)) ? new ArgumentTypeException("other") : null);
                return ContentEquals((BinaryVector.ReadOnly)other);
            }

            // *** ISerializable interface implementation ***

            public void Save(BinarySerializer writer)
            {
                mVec.Save(writer);
            }

            // *** Equality comparer ***

            public static IEqualityComparer<BinaryVector.ReadOnly> GetEqualityComparer()
            {
                return GenericEqualityComparer<BinaryVector.ReadOnly>.Instance;
            }
        }
    }
}
