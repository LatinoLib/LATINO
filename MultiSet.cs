/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    MultiSet.cs
 *  Desc:    Set data structure that can contain duplicate items
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
       |  Class MultiSet<T>
       |
       '-----------------------------------------------------------------------
    */
    public class MultiSet<T> : ICollection<T>, ICollection, IEnumerable<KeyValuePair<T, int>>, ICloneable<MultiSet<T>>, IDeeplyCloneable<MultiSet<T>>, IContentEquatable<MultiSet<T>>, ISerializable
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
                int count;
                if (mItems.TryGetValue(item, out count)) // throws ArgumentNullException
                {
                    mItems[item] = count + 1;
                }
                else
                {
                    mItems.Add(item, 1);
                }
            }
        }

        private void AddRange(IEnumerable<KeyValuePair<T, int>> items)
        {
            foreach (KeyValuePair<T, int> item in items)
            {
                int count;
                if (mItems.TryGetValue(item.Key, out count)) 
                {
                    mItems[item.Key] = count + item.Value;
                }
                else
                {
                    mItems.Add(item.Key, item.Value);
                }
            }
        }

        public IEqualityComparer<T> Comparer
        {
            get { return mItems.Comparer; }
        }

        public static MultiSet<T> Union(MultiSet<T> a, MultiSet<T> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            MultiSet<T> c = a.Clone(); // *** inherits comparer from a (b is expected to have the same comparer)
            c.AddRange((IEnumerable<KeyValuePair<T, int>>)b);
            return c;
        }

        public static MultiSet<T> Union(MultiSet<T>.ReadOnly a, MultiSet<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            return Union(a.Inner, b.Inner);
        }

        public static MultiSet<T> Intersection(MultiSet<T> a, MultiSet<T> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            MultiSet<T> c = new MultiSet<T>(a.Comparer); // *** inherits comparer from a (b is expected to have the same comparer)
            if (b.Count < a.Count) { MultiSet<T> tmp; tmp = a; a = b; b = tmp; }
            foreach (KeyValuePair<T, int> item in a)
            {
                int bCount = b.GetCount(item.Key);
                if (bCount > 0) { c.mItems.Add(item.Key, Math.Min(item.Value, bCount)); }
            }
            return c;
        }

        public static MultiSet<T> Intersection(MultiSet<T>.ReadOnly a, MultiSet<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            return Intersection(a.Inner, b.Inner);
        }

        public static MultiSet<T> Difference(MultiSet<T> a, MultiSet<T> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            MultiSet<T> c = new MultiSet<T>(a.Comparer); // *** inherits comparer from a (b is expected to have the same comparer)
            foreach (KeyValuePair<T, int> item in a)
            {
                int bCount = b.GetCount(item.Key);
                int cCount = item.Value - bCount;
                if (cCount > 0) { c.mItems.Add(item.Key, cCount); }
            }
            return c;
        }

        public static MultiSet<T> Difference(MultiSet<T>.ReadOnly a, MultiSet<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            return Difference(a.Inner, b.Inner);
        }

        public static double JaccardSimilarity(MultiSet<T> a, MultiSet<T> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            MultiSet<T> c = Intersection(a, b);
            double div = (double)(a.Count + b.Count - c.Count);
            if (div == 0) { return 1; } // *** if both sets are empty, the similarity is 1
            return (double)c.Count / div;
        }

        public static double JaccardSimilarity(MultiSet<T>.ReadOnly a, MultiSet<T>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            return JaccardSimilarity(a.Inner, b.Inner);
        }

        private int GetCount()
        {
            int sum = 0;
            foreach (int count in mItems.Values)
            {
                sum += count;
            }
            return sum;
        }

        public T[] ToArray()
        {
            T[] array = new T[GetCount()];
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
            NewT[] array = new NewT[GetCount()];
            int i = 0;
            foreach (KeyValuePair<T, int> item in mItems)
            {
                for (int j = 0; j < item.Value; j++)
                {
                    array[i++] = (NewT)Utils.ChangeType(item.Key, typeof(NewT), fmtProvider); // throws InvalidCastException, FormatException, OverflowException
                }
            }
            return array;
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            Utils.ThrowException(items == null ? new ArgumentNullException("items") : null);
            foreach (T item in items)
            {
                int count;
                if (mItems.TryGetValue(item, out count)) // throws ArgumentNullException
                {
                    if (count - 1 > 0)
                    {
                        mItems[item] = count - 1;
                    }
                    else
                    {
                        mItems.Remove(item);
                    }
                }
            }
        }

        public int CountUnique
        {
            get { return mItems.Count; }
        }

        public int GetCount(T item)
        {
            int count;
            if (mItems.TryGetValue(item, out count)) // throws ArgumentNullException
            {
                return count;
            }
            return 0;
        }

        public bool RemoveAll(T item)
        {
            if (mItems.ContainsKey(item)) // throws ArgumentNullException
            {
                mItems.Remove(item);
                return true;
            }
            return false;
        }

        public void Add(T item, int count)
        {
            Utils.ThrowException(count < 0 ? new ArgumentOutOfRangeException("count") : null);
            if (count == 0) { return; }
            int oldCount;
            if (mItems.TryGetValue(item, out oldCount)) // throws ArgumentNullException
            {
                mItems[item] = oldCount + count;
            }
            else
            {
                mItems.Add(item, count);
            }
        }

        public bool Remove(T item, int count)
        { 
            Utils.ThrowException(count < 0 ? new ArgumentOutOfRangeException("count") : null);
            if (count == 0) { return false; }
            int oldCount;
            if (mItems.TryGetValue(item, out oldCount)) // throws ArgumentNullException
            {
                if (count < oldCount) 
                { 
                    mItems[item] = oldCount - count; 
                }
                else 
                { 
                    mItems.Remove(item); 
                }
                return true;
            }
            return false;
        }

        public ArrayList<KeyDat<int, T>> ToList()
        {
            ArrayList<KeyDat<int, T>> list = new ArrayList<KeyDat<int, T>>();
            foreach (KeyValuePair<T, int> item in mItems)
            {
                list.Add(new KeyDat<int, T>(item.Value, item.Key));
            }
            return list;
        }

        public Set<T> ToUnique()
        {
            Set<T> set = new Set<T>();
            foreach (KeyValuePair<T, int> item in mItems)
            {
                set.Add(item.Key);
            }
            return set;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("{");
            foreach (KeyValuePair<T, int> item in mItems)
            {
                for (int j = 0; j < item.Value; j++)
                {
                    str.Append(" ");
                    str.Append(item.Key);
                }
            }
            str.Append(" }");
            return str.ToString();
        }

        // *** ICollection<T> interface implementation ***

        public void Add(T item)
        {
            int count;
            if (mItems.TryGetValue(item, out count)) // throws ArgumentNullException
            {
                mItems[item] = count + 1;
            }
            else
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
            Utils.ThrowException(index + GetCount() > array.Length ? new ArgumentOutOfRangeException("index") : null);
            foreach (KeyValuePair<T, int> item in mItems)
            {
                for (int j = 0; j < item.Value; j++)
                {
                    array.SetValue(item.Key, index++);
                }
            }
        }

        public int Count
        {
            get { return GetCount(); }
        }       

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            int count;
            if (mItems.TryGetValue(item, out count)) // throws ArgumentNullException
            {
                if (count - 1 > 0)
                {
                    mItems[item] = count - 1;
                }
                else
                {
                    mItems.Remove(item);
                }
                return true;
            }
            return false;
        }        

        // *** ICollection interface implementation ***

        void ICollection.CopyTo(Array array, int index) 
        {
            Utils.ThrowException(array == null ? new ArgumentNullException("array") : null);
            Utils.ThrowException(index + GetCount() > array.Length ? new ArgumentOutOfRangeException("index") : null);
            foreach (KeyValuePair<T, int> item in mItems)
            {
                for (int j = 0; j < item.Value; j++)
                {
                    array.SetValue(item, index++);
                }
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

        // *** IEnumerable interface implementation ***

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ICloneable interface implementation ***

        public MultiSet<T> Clone()
        {
            MultiSet<T> clone = new MultiSet<T>(mItems.Comparer);
            clone.AddRange(mItems);
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable interface implementation ***

        public MultiSet<T> DeepClone()
        {
            MultiSet<T> clone = new MultiSet<T>(mItems.Comparer);
            foreach (KeyValuePair<T, int> item in mItems)
            {
                clone.mItems.Add((T)Utils.Clone(item.Key, /*deepClone=*/true), item.Value);
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
            foreach (KeyValuePair<T, int> item in mItems)
            {
                int count;
                if (other.mItems.TryGetValue(item.Key, out count))
                {
                    if (count != item.Value) { return false; }
                }
                else
                {
                    return false;
                }
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
            Utils.SaveDictionary(mItems, writer); // throws serialization-related exceptions 
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mItems = Utils.LoadDictionary<T, int>(reader); // throws serialization-related exceptions 
        }

        // *** Implicit cast to a read-only adapter ***

        public static implicit operator MultiSet<T>.ReadOnly(MultiSet<T> set)
        {
            if (set == null) { return null; }
            return new MultiSet<T>.ReadOnly(set);
        }

        // *** Equality comparer ***

        public static IEqualityComparer<MultiSet<T>> GetEqualityComparer()
        {
            return MultiSetEqualityComparer<T>.Instance;
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class MultiSet<T>.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<MultiSet<T>>, ICollection, IEnumerable<KeyValuePair<T, int>>, IEnumerable, IContentEquatable<MultiSet<T>.ReadOnly>, ISerializable
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

            public int CountUnique
            {
                get { return mSet.CountUnique; }
            }

            public int GetCount(T item)
            {
                return mSet.GetCount(item);
            }

            public List<KeyDat<int, T>> ToList()
            {
                return mSet.ToList();
            }

            public Set<T> ToUnique()
            {
                return mSet.ToUnique();
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

            public MultiSet<T> Inner
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

            // *** IEnumerable interface implementation ***

            public IEnumerator<KeyValuePair<T, int>> GetEnumerator()
            {
                return mSet.GetEnumerator();
            }

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

            // *** Equality comparer ***

            public static IEqualityComparer<MultiSet<T>.ReadOnly> GetEqualityComparer()
            {
                return MultiSetEqualityComparer<T>.Instance;
            }
        }
    }
}