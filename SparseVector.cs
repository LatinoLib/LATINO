/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SparseVector.cs
 *  Desc:    Sparse vector data structure 
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
       |  Class SparseVector<T>
       |
       '-----------------------------------------------------------------------
    */
    public class SparseVector<T> : IEnumerable<IdxDat<T>>, ICloneable<SparseVector<T>>, IDeeplyCloneable<SparseVector<T>>, IContentEquatable<SparseVector<T>>,
        ISerializable
    {
        private ArrayList<int> mIdx
            = new ArrayList<int>();
        private ArrayList<T> mDat
            = new ArrayList<T>();

        public SparseVector()
        {
        }

        public SparseVector(int capacity)
        {
            mIdx = new ArrayList<int>(capacity); // throws ArgumentOutOfRangeException
            mDat = new ArrayList<T>(capacity);
        }

        public SparseVector(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public SparseVector(IEnumerable<T> vals)
        {
            AddRange(vals); // throws ArgumentNullException, ArgumentValueException
        }

        public SparseVector(IEnumerable<IdxDat<T>> sortedList)
        {
            AddRange(sortedList); // throws ArgumentNullException, ArgumentValueException
        }

        public ArrayList<int> InnerIdx
        {
            get { return mIdx; }
        }
        
        public ArrayList<T> InnerDat
        {
            get { return mDat; }
        }

        public bool ContainsAt(int index)
        {
            Utils.ThrowException(index < 0 ? new ArgumentOutOfRangeException("index") : null);
            return mIdx.BinarySearch(index) >= 0;
        }

        public int FirstNonEmptyIndex
        {
            get
            {
                if (mIdx.Count == 0) { return -1; }
                return mIdx[0];
            }
        }

        public int LastNonEmptyIndex
        {
            get
            {
                if (mIdx.Count == 0) { return -1; }
                return mIdx.Last;
            }
        }

        public IdxDat<T> First
        {
            get { return new IdxDat<T>(mIdx[0], mDat[0]); } // throws ArgumentOutOfRangeException
        }

        public IdxDat<T> Last
        {
            get { return new IdxDat<T>(mIdx.Last, mDat.Last); } // throws ArgumentOutOfRangeException
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("(");
            for (int i = 0; i < mIdx.Count; i++)
            {
                str.Append(" ");
                str.Append(string.Format("( {0} {1} )", mIdx[i], mDat[i]));
            }
            str.Append(" )");
            return str.ToString();
        }

        public void Append(SparseVector<T> otherVec, int thisVecLen)
        {
            Utils.ThrowException(otherVec == null ? new ArgumentNullException("otherVec") : null);
            Utils.ThrowException(thisVecLen <= LastNonEmptyIndex ? new ArgumentOutOfRangeException("thisVecLen") : null);
            foreach (IdxDat<T> itemInfo in otherVec)
            {
                mIdx.Add(itemInfo.Idx + thisVecLen);
                mDat.Add(itemInfo.Dat); // *** note that the elements are not cloned (you need to clone them yourself if needed)
            }
        }

        public void Append(SparseVector<T>.ReadOnly otherVec, int thisVecLen)
        {
            Utils.ThrowException(otherVec == null ? new ArgumentNullException("otherVec") : null);
            Append(otherVec.Inner, thisVecLen); // throws ArgumentOutOfRangeException
        }

        public void Merge(SparseVector<T> otherVec, Utils.BinaryOperatorDelegate<T> binaryOperator)
        {
            Utils.ThrowException(otherVec == null ? new ArgumentNullException("otherVec") : null);
            Utils.ThrowException(binaryOperator == null ? new ArgumentNullException("binaryOperator") : null);
            ArrayList<int> otherIdx = otherVec.InnerIdx;
            ArrayList<T> otherDat = otherVec.InnerDat;
            ArrayList<int> newIdx = new ArrayList<int>(mIdx.Count + otherIdx.Count);
            ArrayList<T> newDat = new ArrayList<T>(mDat.Count + otherDat.Count);
            int i = 0, j = 0;
            while (i < mIdx.Count && j < otherIdx.Count)
            {
                int aIdx = mIdx[i];
                int bIdx = otherIdx[j];
                if (aIdx == bIdx)
                {
                    T value = binaryOperator(mDat[i], otherDat[j]); 
                    if (value != null) { newIdx.Add(aIdx); newDat.Add(value); }
                    i++;
                    j++;
                }
                else if (aIdx < bIdx)
                {
                    newIdx.Add(aIdx); newDat.Add(mDat[i]); 
                    i++;
                }
                else
                {
                    newIdx.Add(bIdx); newDat.Add(otherDat[j]); 
                    j++;
                }
            }
            for (; i < mIdx.Count; i++)
            {
                newIdx.Add(mIdx[i]); newDat.Add(mDat[i]); 
            }
            for (; j < otherIdx.Count; j++)
            {
                newIdx.Add(otherIdx[j]); newDat.Add(otherDat[j]); 
            }
            mIdx = newIdx;
            mDat = newDat;
        }

        public void Merge(SparseVector<T>.ReadOnly otherVec, Utils.BinaryOperatorDelegate<T> binaryOperator)
        {
            Utils.ThrowException(otherVec == null ? new ArgumentNullException("otherVec") : null);
            Merge(otherVec.Inner, binaryOperator); // throws ArgumentNullException
        }

        public void PerformUnaryOperation(Utils.UnaryOperatorDelegate<T> unaryOperator)
        {
            Utils.ThrowException(unaryOperator == null ? new ArgumentNullException("unaryOperator") : null);
            for (int i = mDat.Count - 1; i >= 0; i--)
            {
                T value = unaryOperator(mDat[i]);
                if (value == null)
                {
                    RemoveDirect(i);
                }
                else
                {
                    SetDirect(i, value); 
                }
            }
        }

        // *** Direct access ***

        public IdxDat<T> GetDirect(int directIdx)
        {
            return new IdxDat<T>(mIdx[directIdx], mDat[directIdx]); // throws ArgumentOutOfRangeException
        }

        public int GetIdxDirect(int directIdx)
        {
            return mIdx[directIdx]; // throws ArgumentOutOfRangeException
        }

        public T GetDatDirect(int directIdx)
        {
            return mDat[directIdx]; // throws ArgumentOutOfRangeException
        }

        public void SetDirect(int directIdx, T value)
        {
            Utils.ThrowException(value == null ? new ArgumentNullException("value") : null);
            mDat[directIdx] = value; // throws ArgumentOutOfRangeException
        }

        public void RemoveDirect(int directIdx)
        {
            mIdx.RemoveAt(directIdx); // throws ArgumentOutOfRangeException
            mDat.RemoveAt(directIdx);
        }

        public int GetDirectIdx(int index)
        {
            Utils.ThrowException(index < 0 ? new ArgumentOutOfRangeException("index") : null);
            int directIdx = mIdx.BinarySearch(index);
            return directIdx;
        }

        public int GetDirectIdx(int index, int directStartIdx)
        {
            Utils.ThrowException(index < 0 ? new ArgumentOutOfRangeException("index") : null);
            Utils.ThrowException((directStartIdx < 0 || directStartIdx >= mIdx.Count) ? new ArgumentOutOfRangeException("directStartIdx") : null);
            int directIdx = mIdx.BinarySearch(directStartIdx, mIdx.Count - directStartIdx, index, /*comparer=*/null);
            return directIdx;
        }

        // *** Partial IList<T> interface implementation ***

        public int IndexOf(T item)
        {
            Utils.ThrowException(item == null ? new ArgumentNullException("item") : null);
            for (int i = 0; i < mDat.Count; i++)
            {
                if (mDat[i].Equals(item)) { return mIdx[i]; }
            }
            return -1;
        }

        public void RemoveAt(int index)
        {
            Utils.ThrowException(index < 0 ? new ArgumentOutOfRangeException("index") : null);
            int directIdx = mIdx.BinarySearch(index);
            if (directIdx >= 0) 
            { 
                mIdx.RemoveAt(directIdx); 
                mDat.RemoveAt(directIdx); 
            }
        }

        public void PurgeAt(int index)
        {
            Utils.ThrowException(index < 0 ? new ArgumentOutOfRangeException("index") : null);
            int directIdx = mIdx.BinarySearch(index);
            if (directIdx >= 0) 
            {                 
                mIdx.RemoveAt(directIdx);
                mDat.RemoveAt(directIdx);
            } 
            else 
            { 
                directIdx = ~directIdx; 
            }
            for (int i = directIdx; i < mIdx.Count; i++) 
            { 
                mIdx[i]--;
            }
        }

        public T TryGet(int index, T defaultVal)
        {
            Utils.ThrowException(index < 0 ? new ArgumentOutOfRangeException("index") : null);
            int directIdx = mIdx.BinarySearch(index);
            if (directIdx >= 0)
            {
                return mDat[directIdx];
            }
            else
            {
                return defaultVal;
            }
        }

        public T this[int index]
        {
            get
            {
                Utils.ThrowException(index < 0 ? new ArgumentOutOfRangeException("index") : null);
                int directIdx = mIdx.BinarySearch(index);
                Utils.ThrowException(directIdx < 0 ? new ArgumentValueException("index") : null);
                return mDat[directIdx];
            }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("value") : null);
                Utils.ThrowException(index < 0 ? new ArgumentOutOfRangeException("index") : null);
                int directIdx = mIdx.BinarySearch(index);
                if (directIdx >= 0)
                {
                    mDat[directIdx] = value;
                }
                else
                {
                    mIdx.Insert(~directIdx, index);
                    mDat.Insert(~directIdx, value);
                }
            }
        }

        public void Add(T val)
        {
            Utils.ThrowException(val == null ? new ArgumentNullException("val") : null);
            mIdx.Add(LastNonEmptyIndex + 1);
            mDat.Add(val);
        }

        public void AddRange(IEnumerable<T> vals)
        {
            Utils.ThrowException(vals == null ? new ArgumentNullException("vals") : null);
            int idx = LastNonEmptyIndex + 1;
            foreach (T val in vals)
            {
                Utils.ThrowException(val == null ? new ArgumentValueException("vals") : null);
                mIdx.Add(idx++);
                mDat.Add(val);
            }
        }

        public void AddRange(IEnumerable<IdxDat<T>> sortedList)
        {
            Utils.ThrowException(sortedList == null ? new ArgumentNullException("sortedList") : null);
            int i = 0;
            int oldIdx = -1;
            ArrayList<int> newIdx = new ArrayList<int>(mIdx.Count * 2);
            ArrayList<T> newDat = new ArrayList<T>(mDat.Count * 2);
            IEnumerator<IdxDat<T>> enumer = sortedList.GetEnumerator();
            bool itemAvail = enumer.MoveNext();
            IdxDat<T> item;
            if (itemAvail)
            {
                item = enumer.Current;
                Utils.ThrowException((item.Dat == null || item.Idx <= oldIdx) ? new ArgumentValueException("sortedList") : null);
                oldIdx = item.Idx;
            }
            else
            {
                return;
            }
            while (i < mIdx.Count && itemAvail)
            {
                if (item.Idx > mIdx[i])
                {
                    newIdx.Add(mIdx[i]);
                    newDat.Add(mDat[i]);
                    i++;
                }
                else if (item.Idx < mIdx[i])
                {
                    newIdx.Add(item.Idx);
                    newDat.Add(item.Dat);
                    itemAvail = enumer.MoveNext();
                    if (itemAvail)
                    {
                        item = enumer.Current;
                        Utils.ThrowException((item.Dat == null || item.Idx <= oldIdx) ? new ArgumentValueException("sortedList") : null);
                        oldIdx = item.Idx;
                    }
                }
                else
                {
                    throw new ArgumentValueException("sortedList");
                }
            }
            while (itemAvail)
            {
                newIdx.Add(item.Idx);
                newDat.Add(item.Dat);
                itemAvail = enumer.MoveNext();
                if (itemAvail)
                {
                    item = enumer.Current;
                    Utils.ThrowException((item.Dat == null || item.Idx <= oldIdx) ? new ArgumentValueException("sortedList") : null);
                    oldIdx = item.Idx;
                }
            }
            while (i < mIdx.Count)
            {
                newIdx.Add(mIdx[i]);
                newDat.Add(mDat[i]);
                i++;
            }
            mIdx = newIdx;
            mDat = newDat;
        }

        public void Sort()
        {
            IdxDat<int>[] tmp = new IdxDat<int>[mIdx.Count];
            for (int i = 0; i < mIdx.Count; i++)
            {
                tmp[i] = new IdxDat<int>(mIdx[i], i);
            }
            Array.Sort(tmp);
            ArrayList<T> newDat = new ArrayList<T>(mDat.Count);
            for (int i = 0; i < tmp.Length; i++)
            {
                mIdx[i] = tmp[i].Idx;
                newDat.Add(mDat[tmp[i].Dat]);
            }
            mDat = newDat;
        }

        // *** Partial ICollection<T> interface implementation ***

        public void Clear()
        {             
            mIdx.Clear();
            mDat.Clear();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0; // throws ArgumentNullException
        }

        public int Count
        {
            get { return mIdx.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            Utils.ThrowException(item == null ? new ArgumentNullException("item") : null);
            bool valFound = false;
            for (int directIdx = mDat.Count - 1; directIdx >= 0; directIdx--)
            {
                if (item.Equals(mDat[directIdx]))
                {
                    mIdx.RemoveAt(directIdx);
                    mDat.RemoveAt(directIdx);
                    valFound = true;
                }
            }
            return valFound;
        }

        // *** IEnumerable<IdxDat<T>> interface implementation ***

        public IEnumerator<IdxDat<T>> GetEnumerator()
        {
            return new SparseVectorEnumerator(this);
        }

        // *** IEnumerable interface implementation ***

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ICloneable interface implementation ***

        public SparseVector<T> Clone()
        {
            SparseVector<T> clone = new SparseVector<T>();
            clone.mIdx = mIdx.Clone();
            clone.mDat = mDat.Clone();
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable interface implementation ***

        public SparseVector<T> DeepClone()
        {
            SparseVector<T> clone = new SparseVector<T>();
            clone.mIdx.Capacity = mIdx.Capacity;
            clone.mDat.Capacity = mDat.Capacity;
            for (int i = 0; i < mIdx.Count; i++)
            {
                clone.mIdx.Add(mIdx[i]);
                clone.mDat.Add((T)Utils.Clone(mDat[i], /*deepClone=*/true));
            }
            return clone;
        }

        object IDeeplyCloneable.DeepClone()
        {
            return DeepClone();
        }

        // *** IContentEquatable<SparseVector<T>> interface implementation ***

        public bool ContentEquals(SparseVector<T> other)
        {
            if (other == null || mIdx.Count != other.mIdx.Count) { return false; }
            for (int i = 0; i < mIdx.Count; i++)
            {
                if (mIdx[i] != other.mIdx[i] || !Utils.ObjectEquals(mDat[i], other.mDat[i], /*deepCmp=*/true)) 
                { 
                    return false; 
                }
            }
            return true;
        }

        bool IContentEquatable.ContentEquals(object other)
        {
            Utils.ThrowException((other != null && !(other is SparseVector<T>)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((SparseVector<T>)other);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exception
            writer.WriteInt(mIdx.Count);
            for (int i = 0; i < mIdx.Count; i++)
            {
                writer.WriteInt(mIdx[i]);
                writer.WriteValueOrObject<T>(mDat[i]);
            }
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mIdx.Clear();
            mDat.Clear();
            // the following statements throw serialization-related exception
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                mIdx.Add(reader.ReadInt());
                mDat.Add(reader.ReadValueOrObject<T>());
            }
        }

        // *** Implicit cast to a read-only adapter ***

        public static implicit operator SparseVector<T>.ReadOnly(SparseVector<T> vec)
        {
            if (vec == null) { return null; }
            return new SparseVector<T>.ReadOnly(vec);
        }

        // *** Equality comparer ***

        public static IEqualityComparer<SparseVector<T>> GetEqualityComparer()
        {
            return GenericEqualityComparer<SparseVector<T>>.Instance;
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class SparseVectorEnumerator
           |
           '-----------------------------------------------------------------------
        */
        private class SparseVectorEnumerator : IEnumerator<IdxDat<T>>
        {
            private SparseVector<T> mVec;
            private int mItemIdx
                = -1;

            public SparseVectorEnumerator(SparseVector<T> vec)
            {
                mVec = vec;
            }

            // *** IEnumerator<IdxDat<T>> interface implementation ***

            public IdxDat<T> Current
            {
                get { return new IdxDat<T>(mVec.mIdx[mItemIdx], mVec.mDat[mItemIdx]); } // throws ArgumentOutOfRangeException
            }

            // *** IEnumerator interface implementation ***

            object IEnumerator.Current
            {
                get { return Current; } // throws ArgumentOutOfRangeException
            }

            public bool MoveNext()
            {
                mItemIdx++;
                if (mItemIdx == mVec.mIdx.Count)
                {
                    Reset();
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                mItemIdx = -1;
            }

            // *** IDisposable interface implementation ***

            public void Dispose()
            {
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class SparseVector<T>.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<SparseVector<T>>, IEnumerable<IdxDat<T>>, IContentEquatable<SparseVector<T>.ReadOnly>,
            ISerializable
        {
            private SparseVector<T> mVec;

            public ReadOnly(SparseVector<T> vec)
            {
                Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
                mVec = vec;
            }

            public ReadOnly(BinarySerializer reader)
            {
                mVec = new SparseVector<T>(reader); // throws ArgumentNullException, serialization-related exceptions
            }

            public bool ContainsAt(int index)
            {
                return mVec.ContainsAt(index);
            }

            public int FirstNonEmptyIndex
            {
                get { return mVec.FirstNonEmptyIndex; }
            }

            public int LastNonEmptyIndex
            {
                get { return mVec.LastNonEmptyIndex; }
            }

            public IdxDat<T> First
            {
                get { return mVec.First; }
            }

            public IdxDat<T> Last
            {
                get { return mVec.Last; }
            }

            public override string ToString()
            {
                return mVec.ToString();
            }

            // *** Direct access ***

            public IdxDat<T> GetDirect(int directIdx)
            {
                return mVec.GetDirect(directIdx);
            }

            public int GetDirectIdx(int index)
            {
                return mVec.GetDirectIdx(index);
            }

            public int GetDirectIdx(int index, int directStartIdx)
            {
                return mVec.GetDirectIdx(index, directStartIdx);
            }

            // *** IReadOnlyAdapter interface implementation ***

            public SparseVector<T> GetWritableCopy()
            {
                return mVec.Clone();
            }

            object IReadOnlyAdapter.GetWritableCopy()
            {
                return GetWritableCopy();
            }

            public SparseVector<T> Inner
            {
                get { return mVec; }
            }

            object IReadOnlyAdapter.Inner
            {
                get { return Inner; }
            }

            // *** Partial IList<T> interface implementation ***

            public int IndexOf(T item)
            {
                return mVec.IndexOf(item);
            }

            public T TryGet(int idx, T defaultVal)
            {
                return mVec.TryGet(idx, defaultVal);
            }

            public T this[int idx]
            {
                get { return mVec[idx]; }
                set { mVec[idx] = value; }
            }

            // *** Partial ICollection<T> interface implementation ***

            public bool Contains(T item)
            {
                return mVec.Contains(item);
            }

            public int Count
            {
                get { return mVec.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            // *** IEnumerable<IdxDat<T>> interface implementation ***

            public IEnumerator<IdxDat<T>> GetEnumerator()
            {
                return mVec.GetEnumerator();
            }

            // *** IEnumerable interface implementation ***

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)mVec).GetEnumerator();
            }

            // *** IContentEquatable<SparseVector<T>.ReadOnly> interface implementation ***

            public bool ContentEquals(SparseVector<T>.ReadOnly other)
            {
                return other != null && mVec.ContentEquals(other.Inner);
            }

            bool IContentEquatable.ContentEquals(object other)
            {
                Utils.ThrowException((other != null && !(other is SparseVector<T>.ReadOnly)) ? new ArgumentTypeException("other") : null);
                return ContentEquals((SparseVector<T>.ReadOnly)other);
            }

            // *** ISerializable interface implementation ***

            public void Save(BinarySerializer writer)
            {
                mVec.Save(writer);
            }

            // *** Equality comparer ***

            public static IEqualityComparer<SparseVector<T>.ReadOnly> GetEqualityComparer()
            {
                return GenericEqualityComparer<SparseVector<T>.ReadOnly>.Instance;
            }
        }
    }
}
