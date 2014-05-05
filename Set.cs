/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Set.cs
 *  Desc:    Set data structure based on Dictionary
 *  Created: Mar-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Set<T>
       |
       '-----------------------------------------------------------------------
    */
    public class Set<T> : ICollection<T>, ICollection, IEnumerable<T>, ICloneable<Set<T>>, IDeeplyCloneable<Set<T>>, IContentEquatable<Set<T>>, ISerializable
    {
        private Dictionary<T, object> mItems
            = new Dictionary<T, object>();

        public Set()
        {
        }

        public Set(IEqualityComparer<T> comparer)
        {
            mItems = new Dictionary<T, object>(comparer);
        }

        public Set(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public Set(BinarySerializer reader, IEqualityComparer<T> comparer)
        {
            mItems = new Dictionary<T, object>(comparer);
            Load(reader); // throws ArgumentNullException
        }

        public Set(IEnumerable<T> items)
        {
            AddRange(items); // throws ArgumentNullException
        }

        public Set(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            mItems = new Dictionary<T, object>(comparer);
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
                    mItems.Add(item, null);
                }
            }
        }

        public IEqualityComparer<T> Comparer
        {
            get { return mItems.Comparer; }
        }

        public static Set<T> Union(Set<T> a, Set<T> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            Set<T> c = a.Clone(); // *** inherits comparer from a (b is expected to have the same comparer)
            c.AddRange(b);
            return c;
        }

        public static Set<T> Union(Set<T>.ReadOnly a, Set<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            return Union(a.Inner, b.Inner);
        }

        public static Set<T> Intersection(Set<T> a, Set<T> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            Set<T> c = new Set<T>(a.Comparer); // *** inherits comparer from a (b is expected to have the same comparer)
            if (b.Count < a.Count) { Set<T> tmp; tmp = a; a = b; b = tmp; }
            foreach (T item in a)
            {
                if (b.Contains(item)) { c.Add(item); }
            }
            return c;
        }

        public static Set<T> Intersection(Set<T>.ReadOnly a, Set<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            return Intersection(a.Inner, b.Inner);
        }

        public static Set<T> Difference(Set<T> a, Set<T> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            Set<T> c = new Set<T>(a.Comparer); // *** inherits comparer from a (b is expected to have the same comparer)
            foreach (T item in a)
            {
                if (!b.Contains(item)) { c.Add(item); }
            }
            return c;
        }

        public static Set<T> Difference(Set<T>.ReadOnly a, Set<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            return Difference(a.Inner, b.Inner);
        }

        public static double JaccardSimilarity(Set<T> a, Set<T> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            Set<T> c = Intersection(a, b);
            double div = (double)(a.Count + b.Count - c.Count);
            if (div == 0) { return 1; } // *** if both sets are empty, the similarity is 1
            return (double)c.Count / div;
        }

        public static double JaccardSimilarity(Set<T>.ReadOnly a, Set<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            return JaccardSimilarity(a.Inner, b.Inner);
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
                foreach (KeyValuePair<T, object> item in mItems)
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
                mItems.Add(item, null);
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

        public Set<T> Clone()
        {
            return new Set<T>(mItems.Keys, mItems.Comparer);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable interface implementation ***

        public Set<T> DeepClone()
        {
            Set<T> clone = new Set<T>(mItems.Comparer);
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

        // *** IContentEquatable<Set<T>> interface implementation ***

        public bool ContentEquals(Set<T> other)
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
            Utils.ThrowException((other != null && !(other is Set<T>)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((Set<T>)other);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            writer.WriteInt(mItems.Count); 
            foreach (KeyValuePair<T, object> item in mItems)
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
                mItems.Add(reader.ReadValueOrObject<T>(), null); 
            }
        }

        // *** Implicit cast to a read-only adapter ***

        public static implicit operator Set<T>.ReadOnly(Set<T> set)
        {
            if (set == null) { return null; }
            return new Set<T>.ReadOnly(set);
        }

        // *** Equality comparer ***

        public static IEqualityComparer<Set<T>> GetEqualityComparer()
        {
            return SetEqualityComparer<T>.Instance;
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Set<T>.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<Set<T>>, ICollection, IEnumerable<T>, IEnumerable, IContentEquatable<Set<T>.ReadOnly>, ISerializable
        {
            private Set<T> mSet;

            public ReadOnly(Set<T> set)
            {
                Utils.ThrowException(set == null ? new ArgumentNullException("set") : null);
                mSet = set;
            }

            public ReadOnly(BinarySerializer reader)
            {
                mSet = new Set<T>(reader); // throws ArgumentNullException, serialization-related exceptions
            }

            public IEqualityComparer<T> Comparer
            {
                get { return mSet.Comparer; }
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

            public Set<T> GetWritableCopy()
            {
                return mSet.Clone();
            }

            object IReadOnlyAdapter.GetWritableCopy()
            {
                return GetWritableCopy();
            }

            public Set<T> Inner
            {
                get { return mSet; }
            }

            object IReadOnlyAdapter.Inner
            {
                get { return Inner; }
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

            // *** IContentEquatable<Set<T>.ReadOnly> interface implementation ***

            public bool ContentEquals(Set<T>.ReadOnly other)
            {
                return other != null && mSet.ContentEquals(other.Inner);
            }

            bool IContentEquatable.ContentEquals(object other)
            {
                Utils.ThrowException((other != null && !(other is Set<T>.ReadOnly)) ? new ArgumentTypeException("other") : null);
                return ContentEquals((Set<T>.ReadOnly)other);
            }

            // *** ISerializable interface implementation ***

            public void Save(BinarySerializer writer)
            {
                mSet.Save(writer);
            }

            // *** Equality comparer ***

            public static IEqualityComparer<Set<T>.ReadOnly> GetEqualityComparer()
            {
                return SetEqualityComparer<T>.Instance;
            }
        }
    }
}