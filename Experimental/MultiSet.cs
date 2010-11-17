/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    MultiSet.cs
 *  Desc:    Set data structure with multiple equal items
 *  Created: Mar-2010
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Latino.Experimental
{
    /* .-----------------------------------------------------------------------
       |
       |  Class MultiSet<T>
       |
       '-----------------------------------------------------------------------
    */
    public class MultiSet<T> : ICollection<T>, ICollection, IEnumerable<T>, ICloneable<MultiSet<T>>, IDeeplyCloneable<MultiSet<T>>, IContentEquatable<MultiSet<T>>, ISerializable
    {
        private Dictionary<T, int> mItems
            = new Dictionary<T, int>();

        public MultiSet()
        {
        }

        public MultiSet(IEqualityComparer<T> comparer)
        {
            mItems = new Dictionary<T, int>(comparer);
        }

        public MultiSet(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public MultiSet(BinarySerializer reader, IEqualityComparer<T> comparer)
        {
            mItems = new Dictionary<T, int>(comparer);
            Load(reader); // throws ArgumentNullException
        }

        public MultiSet(IEnumerable<T> items)
        {
            AddRange(items); // throws ArgumentNullException
        }

        public MultiSet(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            mItems = new Dictionary<T, int>(comparer);
            AddRange(items); // throws ArgumentNullException
        }

        public void SetItems(IEnumerable<T> items)
        {
            mItems.Clear();
            AddRange(items); // throws ArgumentNullException
        }

        public void AddRange(IEnumerable<T> items)
        {
            Utils.ThrowException(items == null ? new ArgumentNullException("items") : null);
            foreach (T item in items)
            {
                if (!mItems.ContainsKey(item)) // throws ArgumentNullException
                {
                    mItems.Add(item, 1);
                }
            }
        }

        public static MultiSet<T> Union(MultiSet<T>.ReadOnly a, MultiSet<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            MultiSet<T> c = (MultiSet<T>)a.GetWritableCopy();
            c.AddRange(b);
            return c;
        }

        public static MultiSet<T> Intersection(MultiSet<T>.ReadOnly a, MultiSet<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            MultiSet<T> c = new MultiSet<T>();
            if (b.Count < a.Count) { MultiSet<T>.ReadOnly tmp; tmp = a; a = b; b = tmp; }
            foreach (T item in a)
            {
                if (b.Contains(item)) { c.Add(item); }
            }
            return c;
        }

        public static MultiSet<T> Difference(MultiSet<T>.ReadOnly a, MultiSet<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            MultiSet<T> c = new MultiSet<T>();
            foreach (T item in a)
            {
                if (!b.Contains(item)) { c.Add(item); }
            }
            return c;
        }

        public static double JaccardSimilarity(MultiSet<T>.ReadOnly a, MultiSet<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            MultiSet<T> c = Intersection(a, b);
            double div = (double)(a.Count + b.Count - c.Count);
            if (div == 0) { return 1; } // *** if both sets are empty, the similarity is 1
            return (double)c.Count / div;
        }

        public T[] ToArray()
        {
            T[] array = new T[mItems.Count];
            CopyTo(array, 0);
            return array;
        }

        public T Any
        { 
            get
            {
                foreach (KeyValuePair<T, int> item in mItems)
                {
                    return item.Key;
                }
                throw new InvalidOperationException();
            }
        }

        public NewT[] ToArray<NewT>()
        {
            return ToArray<NewT>(/*fmtProvider=*/null); // throws InvalidCastException, FormatException, OverflowException
        }

        public NewT[] ToArray<NewT>(IFormatProvider fmtProvider)
        {
            NewT[] array = new NewT[mItems.Count];
            int i = 0;
            foreach (T item in mItems.Keys)
            {
                array[i++] = (NewT)Utils.ChangeType(item, typeof(NewT), fmtProvider); // throws InvalidCastException, FormatException, OverflowException
            }
            return array;
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            Utils.ThrowException(items == null ? new ArgumentNullException("items") : null);
            foreach (T item in items)
            {
                mItems.Remove(item); // throws ArgumentNullException
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("{");
            foreach (T item in mItems.Keys)
            {
                str.Append(" ");
                str.Append(item);
            }
            str.Append(" }");
            return str.ToString();
        }

        // *** ICollection<T> interface implementation ***

        public void Add(T item)
        {
            if (!mItems.ContainsKey(item)) // throws ArgumentNullException
            {
                mItems.Add(item, 1);
            }
        }

        public void Clear()
        {
            mItems.Clear();
        }

        public bool Contains(T item)
        {
            return mItems.ContainsKey(item); // throws ArgumentNullException
        }

        public void CopyTo(T[] array, int index)
        {
            Utils.ThrowException(array == null ? new ArgumentNullException("array") : null);
            Utils.ThrowException(index + mItems.Count > array.Length ? new ArgumentOutOfRangeException("index") : null);
            foreach (T item in mItems.Keys)
            {
                array.SetValue(item, index++);
            }
        }

        public int Count
        {
            get { return mItems.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return mItems.Remove(item); // throws ArgumentNullException
        }

        // *** ICollection interface implementation ***

        void ICollection.CopyTo(Array array, int index)
        {
            Utils.ThrowException(array == null ? new ArgumentNullException("array") : null);
            Utils.ThrowException(index + mItems.Count > array.Length ? new ArgumentOutOfRangeException("index") : null);
            foreach (T item in mItems.Keys)
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
            return mItems.Keys.GetEnumerator();
        }

        // *** IEnumerable interface implementation ***

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ICloneable interface implementation ***

        public MultiSet<T> Clone()
        {
            return new MultiSet<T>(mItems.Keys);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable interface implementation ***

        public MultiSet<T> DeepClone()
        {
            MultiSet<T> clone = new MultiSet<T>();
            foreach (T item in mItems.Keys)
            {
                clone.Add((T)Utils.Clone(item, /*deepClone=*/true));
            }
            return clone;
        }

        object IDeeplyCloneable.DeepClone()
        {
            return DeepClone();
        }

        // *** IContentEquatable<MultiSet<T>> interface implementation ***

        public bool ContentEquals(MultiSet<T> other)
        {
            if (other == null || Count != other.Count) { return false; }
            foreach (T item in mItems.Keys)
            {
                if (!other.Contains(item)) { return false; }
            }
            return true;
        }

        bool IContentEquatable.ContentEquals(object other)
        {
            Utils.ThrowException((other != null && !(other is MultiSet<T>)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((MultiSet<T>)other);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            writer.WriteInt(mItems.Count); 
            foreach (KeyValuePair<T, int> item in mItems)
            {
                writer.WriteValueOrObject<T>(item.Key); 
            }
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mItems.Clear();
            // the following statements throw serialization-related exceptions 
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                mItems.Add(reader.ReadValueOrObject<T>(), 1); 
            }
        }

        // *** Implicit cast to a read-only adapter ***

        public static implicit operator MultiSet<T>.ReadOnly(MultiSet<T> set)
        {
            if (set == null) { return null; }
            return new MultiSet<T>.ReadOnly(set);
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class MultiSet<T>.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<MultiSet<T>>, ICollection, IEnumerable<T>, IEnumerable, IContentEquatable<MultiSet<T>.ReadOnly>, ISerializable
        {
            private MultiSet<T> mSet;

            public ReadOnly(MultiSet<T> set)
            {
                Utils.ThrowException(set == null ? new ArgumentNullException("set") : null);
                mSet = set;
            }

            public ReadOnly(BinarySerializer reader)
            {
                mSet = new MultiSet<T>(reader); // throws ArgumentNullException, serialization-related exceptions
            }

            public T[] ToArray()
            {
                return mSet.ToArray();
            }

            public T Any
            {
                get { return mSet.Any; }
            }

            public NewT[] ToArray<NewT>()
            {
                return mSet.ToArray<NewT>();
            }

            public NewT[] ToArray<NewT>(IFormatProvider fmtProvider)
            {
                return mSet.ToArray<NewT>(fmtProvider);
            }

            public override string ToString()
            {
                return mSet.ToString();
            }

            // *** IReadOnlyAdapter interface implementation ***

            public MultiSet<T> GetWritableCopy()
            {
                return mSet.Clone();
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
            MultiSet<T> Inner
            {
                get { return mSet; }
            }

            // *** Partial ICollection<T> interface implementation ***

            public bool Contains(T item)
            {
                return mSet.Contains(item);
            }

            public void CopyTo(T[] array, int index)
            {
                mSet.CopyTo(array, index);
            }

            public int Count
            {
                get { return mSet.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            // *** ICollection interface implementation ***

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)mSet).CopyTo(array, index);
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
                return mSet.GetEnumerator();
            }

            // *** IEnumerable interface implementation ***

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)mSet).GetEnumerator();
            }

            // *** IContentEquatable<MultiSet<T>.ReadOnly> interface implementation ***

            public bool ContentEquals(MultiSet<T>.ReadOnly other)
            {
                return other != null && mSet.ContentEquals(other.Inner);
            }

            bool IContentEquatable.ContentEquals(object other)
            {
                Utils.ThrowException((other != null && !(other is MultiSet<T>.ReadOnly)) ? new ArgumentTypeException("other") : null);
                return ContentEquals((MultiSet<T>.ReadOnly)other);
            }

            // *** ISerializable interface implementation ***

            public void Save(BinarySerializer writer)
            {
                mSet.Save(writer);
            }
        }
    }
}