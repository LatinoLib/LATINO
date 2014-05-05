/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ArrayList.cs
 *  Desc:    Dynamic array data structure 
 *  Created: Nov-2007
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
       |  Class ArrayList<T>
       |
       '-----------------------------------------------------------------------
    */
    public class ArrayList<T> : List<T>, ICloneable<ArrayList<T>>, IDeeplyCloneable<ArrayList<T>>, IContentEquatable<ArrayList<T>>, ISerializable
    {
        public ArrayList()
        {
        }

        public ArrayList(int capacity) : base(capacity) // throws ArgumentOutOfRangeException
        {
        }

        public ArrayList(IEnumerable<T> collection) : base(collection) // throws ArgumentNullException
        {
        }

        public ArrayList(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public new ArrayList<T> FindAll(Predicate<T> match)
        {
            return new ArrayList<T>(base.FindAll(match)); // throws ArgumentNullException
        }

        public new ArrayList<T> GetRange(int index, int count)
        {
            return new ArrayList<T>(base.GetRange(index, count)); // throws ArgumentException, ArgumentOutOfRangeException
        }

        public int InsertSorted(T item)
        {
            return InsertSorted(item, /*comparer=*/null);
        }

        public int InsertSorted(T item, IComparer<T> comparer)
        {
            return InsertSorted(0, Count, item, comparer);
        }

        public int InsertSorted(int index, int count, T item, IComparer<T> comparer)
        {
            int idx = BinarySearch(index, count, item, comparer); // throws ArgumentOutOfRangeException, ArgumentException, InvalidOperationException
            if (idx < 0) { idx = ~idx; }
            Insert(idx, item);
            return idx;
        }

        public void Shuffle()
        {
            Shuffle(new Random());
        }

        public void Shuffle(Random rnd)
        {
            Utils.ThrowException(rnd == null ? new ArgumentNullException("rnd") : null);
            // Durstenfeld's shuffle algorithm
            int n = Count;
            while (n > 1)
            {
                int k = rnd.Next(n);
                n--;
                T tmp = this[n];
                this[n] = this[k];
                this[k] = tmp;
            }
        }

        public T First
        {
            get
            {
                return this[0]; // throws ArgumentOutOfRangeException
            }
            set
            {
                this[0] = value; // throws ArgumentOutOfRangeException
            }
        }

        public T Last
        {
            get
            {
                return this[Count - 1]; // throws ArgumentOutOfRangeException
            }
            set
            {
                this[Count - 1] = value; // throws ArgumentOutOfRangeException
            }
        }

        public NewT[] ToArray<NewT>()
        {
            return ToArray<NewT>(/*fmtProvider=*/null); // throws InvalidCastException, FormatException, OverflowException
        }

        public NewT[] ToArray<NewT>(IFormatProvider fmtProvider)
        { 
            NewT[] array = new NewT[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = (NewT)Utils.ChangeType(this[i], typeof(NewT), fmtProvider); // throws InvalidCastException, FormatException, OverflowException
            }
            return array;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("(");
            foreach (T item in this)
            {
                str.Append(" ");
                str.Append(item);
            }
            str.Append(" )");
            return str.ToString();
        }

        // *** ICloneable interface implementation ***

        public ArrayList<T> Clone()
        {
            return new ArrayList<T>(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable interface implementation ***

        public ArrayList<T> DeepClone()
        {
            ArrayList<T> clone = new ArrayList<T>(Capacity);
            foreach (T item in this)
            {
                clone.Add((T)Utils.Clone(item, /*deepClone=*/true));
            }
            return clone;
        }

        object IDeeplyCloneable.DeepClone()
        {
            return DeepClone();
        }

        // *** IContentEquatable<ArrayList<T>> interface implementation ***

        public bool ContentEquals(ArrayList<T> other)
        {
            if (other == null || Count != other.Count) { return false; }
            for (int i = 0; i < Count; i++)
            {
                if (!Utils.ObjectEquals(this[i], other[i], /*deepCmp=*/true)) { return false; }
            }
            return true;
        }

        bool IContentEquatable.ContentEquals(object other)
        {
            Utils.ThrowException((other != null && !(other is ArrayList<T>)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((ArrayList<T>)other);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt(Count); 
            foreach (T item in this) { writer.WriteValueOrObject<T>(item); } 
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            Clear();
            // the following statements throw serialization-related exceptions
            int count = (Capacity = reader.ReadInt());
            for (int i = 0; i < count; i++) { Add(reader.ReadValueOrObject<T>()); } 
        }

        // *** Implicit cast to a read-only adapter ***

        public static implicit operator ArrayList<T>.ReadOnly(ArrayList<T> list)
        {
            if (list == null) { return null; }
            return new ArrayList<T>.ReadOnly(list);
        }

        // *** Equality comparer ***

        public static IEqualityComparer<ArrayList<T>> GetEqualityComparer()
        {
            return GenericEqualityComparer<ArrayList<T>>.Instance;
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class ArrayList<T>.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<ArrayList<T>>, ICollection, IEnumerable<T>, IEnumerable, IContentEquatable<ArrayList<T>.ReadOnly>, ISerializable
        {
            private ArrayList<T> mList;

            public ReadOnly(ArrayList<T> list)
            {
                Utils.ThrowException(list == null ? new ArgumentNullException("list") : null);
                mList = list;
            }

            public ReadOnly(BinarySerializer reader)
            {
                mList = new ArrayList<T>(reader); 
            }

            public int BinarySearch(T item)
            {
                return mList.BinarySearch(item);
            }

            public void ConvertAll<NewT>(Converter<T, NewT> converter)
            {
                mList.ConvertAll<NewT>(converter);
            }

            public bool Exists(Predicate<T> match)
            {
                return mList.Exists(match);
            }

            public T Find(Predicate<T> match)
            {
                return mList.Find(match);
            }

            public ArrayList<T> FindAll(Predicate<T> match)
            {
                return mList.FindAll(match);
            }

            public int FindIndex(Predicate<T> match)
            {
                return mList.FindIndex(match);
            }

            public T FindLast(Predicate<T> match)
            {
                return mList.FindLast(match);
            }

            public int FindLastIndex(Predicate<T> match)
            {
                return mList.FindLastIndex(match);
            }

            public void ForEach(Action<T> action)
            {
                mList.ForEach(action);
            }

            public ArrayList<T> GetRange(int index, int count)
            {
                return mList.GetRange(index, Count);
            }

            public int LastIndexOf(T item)
            {
                return mList.LastIndexOf(item);
            }

            public T[] ToArray()
            {
                return mList.ToArray();
            }

            public bool TrueForAll(Predicate<T> match)
            {
                return mList.TrueForAll(match);
            }

            public T First
            {
                get { return mList.First; }
            }

            public T Last
            {
                get { return mList.Last; }
            }

            public NewT[] ToArray<NewT>()
            {
                return mList.ToArray<NewT>();
            }

            public NewT[] ToArray<NewT>(IFormatProvider fmtProvider)
            {
                return mList.ToArray<NewT>(fmtProvider);
            }

            public override string ToString()
            {
                return mList.ToString();
            }

            // *** IReadOnlyAdapter interface implementation ***

            public ArrayList<T> GetWritableCopy()
            {
                return mList.Clone();
            }

            object IReadOnlyAdapter.GetWritableCopy()
            {
                return GetWritableCopy();
            }

            public ArrayList<T> Inner
            {
                get { return mList; }
            }

            object IReadOnlyAdapter.Inner
            {
                get { return Inner; }
            }

            // *** Partial IList<T> interface implementation ***

            public int IndexOf(T item)
            {
                return mList.IndexOf(item);
            }

            public T this[int index]
            {
                get { return mList[index]; }
            }

            // *** Partial ICollection<T> interface implementation ***

            public bool Contains(T item)
            {
                return mList.Contains(item);
            }

            public void CopyTo(T[] array, int index)
            {
                mList.CopyTo(array, index);
            }

            public int Count
            {
                get { return mList.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            // *** ICollection interface implementation ***

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)mList).CopyTo(array, index);
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
                return mList.GetEnumerator();
            }

            // *** IEnumerable interface implementation ***

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            // *** IContentEquatable<ArrayList<T>.ReadOnly> interface implementation ***

            public bool ContentEquals(ArrayList<T>.ReadOnly other)
            {
                return other != null && mList.ContentEquals(other.Inner);
            }

            bool IContentEquatable.ContentEquals(object other)
            {
                Utils.ThrowException((other != null && !(other is ArrayList<T>.ReadOnly)) ? new ArgumentTypeException("other") : null);
                return ContentEquals((ArrayList<T>.ReadOnly)other);
            }

            // *** ISerializable interface implementation ***

            public void Save(BinarySerializer writer)
            {
                mList.Save(writer);
            }

            // *** Equality comparer ***

            public static IEqualityComparer<ArrayList<T>.ReadOnly> GetEqualityComparer()
            {
                return GenericEqualityComparer<ArrayList<T>.ReadOnly>.Instance;
            }
        }
    }
}
