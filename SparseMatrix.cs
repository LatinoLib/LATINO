/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SparseMatrix.cs
 *  Desc:    Sparse matrix data structure 
 *  Created: Apr-2007
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
       |  Class SparseMatrix<T>
       |
       '-----------------------------------------------------------------------
    */
    public class SparseMatrix<T> : IEnumerable<IdxDat<SparseVector<T>>>, ICloneable<SparseMatrix<T>>, IDeeplyCloneable<SparseMatrix<T>>,
        IContentEquatable<SparseMatrix<T>>, ISerializable
    {
        private ArrayList<SparseVector<T>> mRows
            = new ArrayList<SparseVector<T>>();

        public SparseMatrix()
        {
        }

        public SparseMatrix(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        private void SetRowListSize(int size)
        {
            //System.Diagnostics.Debug.Assert(size > mRows.Count);
            int addRows = size - mRows.Count;
            for (int i = 0; i < addRows; i++)
            {
                mRows.Add(new SparseVector<T>());
            }
        }

        public void TrimRows()
        {
            int rowIdx = mRows.Count - 1;
            for (; rowIdx >= 0; rowIdx--)
            {
                if (mRows[rowIdx].Count > 0) { break; }
            }
            mRows.RemoveRange(rowIdx + 1, mRows.Count - (rowIdx + 1));
        }

        public bool ContainsRowAt(int rowIdx)
        {
            Utils.ThrowException(rowIdx < 0 ? new ArgumentOutOfRangeException("rowIdx") : null);
            return rowIdx < mRows.Count && mRows[rowIdx].Count > 0;
        }

        public bool ContainsColAt(int colIdx)
        {
            Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
            foreach (SparseVector<T> row in mRows)
            {
                if (row.ContainsAt(colIdx)) { return true; }
            }
            return false;
        }

        public bool ContainsAt(int rowIdx, int colIdx)
        {
            Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
            if (!ContainsRowAt(rowIdx)) { return false; } // throws ArgumentOutOfRangeException
            return mRows[rowIdx].ContainsAt(colIdx);
        }

        public int GetFirstNonEmptyRowIdx()
        {
            for (int rowIdx = 0; rowIdx < mRows.Count; rowIdx++)
            {
                if (mRows[rowIdx].Count > 0) { return rowIdx; }
            }
            return -1;
        }

        public int GetLastNonEmptyRowIdx()
        {
            for (int rowIdx = mRows.Count - 1; rowIdx >= 0; rowIdx--)
            {
                if (mRows[rowIdx].Count > 0) { return rowIdx; }
            }
            return -1;
        }

        public int GetFirstNonEmptyColIdx()
        {
            int minIdx = int.MaxValue;
            foreach (SparseVector<T> row in mRows)
            {
                if (row.FirstNonEmptyIndex != -1 && row.FirstNonEmptyIndex < minIdx)
                {
                    minIdx = row.FirstNonEmptyIndex;
                }
            }
            return minIdx == int.MaxValue ? -1 : minIdx;
        }

        public int GetLastNonEmptyColIdx()
        {
            int maxIdx = -1;
            foreach (SparseVector<T> row in mRows)
            {
                if (row.LastNonEmptyIndex > maxIdx)
                {
                    maxIdx = row.LastNonEmptyIndex;
                }
            }
            return maxIdx;
        }

        public override string ToString()
        {
            return mRows.ToString();
        }

        public string ToString(string format)
        {
            if (format == "C") // compact format
            {
                return mRows.ToString();
            }
            else if (format.StartsWith("E")) // extended format
            {
                StringBuilder str = new StringBuilder();
                int rowCount = mRows.Count; // this will show empty rows at the end if the matrix is not trimmed
                int colCount = GetLastNonEmptyColIdx() + 1;
                string valFmt = format.Length == 1 ? "{0}" : "{0:" + format.Substring(1) + "}";
                for (int rowIdx = 0; rowIdx < rowCount; rowIdx++)
                {
                    for (int colIdx = 0; colIdx < colCount; colIdx++)
                    {
                        if (ContainsAt(rowIdx, colIdx))
                        {
                            str.Append(string.Format(valFmt, this[rowIdx, colIdx])); // throws FormatException
                        }
                        else
                        {
                            str.Append("-");
                        }
                        if (colIdx != colCount - 1) { str.Append("\t"); }
                    }
                    if (rowIdx != rowCount - 1) { str.AppendLine(); }
                }
                return str.ToString();
            }
            else
            {
                throw new ArgumentNotSupportedException("format");
            }
        }

        public bool IndexOf(T val, ref int rowIdx, ref int colIdx)
        {
            Utils.ThrowException(val == null ? new ArgumentNullException("val") : null);
            for (int i = 0; i < mRows.Count; i++)
            {
                SparseVector<T> row = mRows[i];
                foreach (IdxDat<T> item in row)
                {
                    if (item.Dat.Equals(val))
                    {
                        rowIdx = i;
                        colIdx = item.Idx;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Contains(T val)
        {
            int foo = 0, bar = 0;
            return IndexOf(val, ref foo, ref bar); // throws ArgumentNullException
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int GetRowCount()
        {
            int count = 0;
            foreach (SparseVector<T> row in mRows)
            {
                if (row.Count > 0) { count++; }
            }
            return count;
        }

        public int CountValues()
        {
            int count = 0;
            foreach (SparseVector<T> row in mRows)
            {
                count += row.Count;
            }
            return count;
        }

        public double GetSparseness(int numRows, int numCols)
        {
            Utils.ThrowException(numRows <= 0 ? new ArgumentException("numRows") : null);
            Utils.ThrowException(numCols <= 0 ? new ArgumentException("numCols") : null);
            int allValues = numRows * numCols;
            return (double)(allValues - CountValues()) / (double)allValues;
        }

        public SparseVector<T> this[int rowIdx]
        {
            get
            {
                Utils.ThrowException(rowIdx < 0 ? new ArgumentOutOfRangeException("rowIdx") : null);
                if (rowIdx >= mRows.Count) { return null; }
                return mRows[rowIdx].Count > 0 ? mRows[rowIdx] : null;
            }
            set
            {
                Utils.ThrowException(rowIdx < 0 ? new ArgumentOutOfRangeException("rowIdx") : null);
                if (value != null)
                {
                    if (rowIdx >= mRows.Count) { SetRowListSize(rowIdx + 1); }
                    mRows[rowIdx] = value;
                }
                else if (rowIdx < mRows.Count)
                {
                    mRows[rowIdx].Clear();
                }
            }
        }

        public T TryGet(int rowIdx, int colIdx, T defaultVal)
        {
            Utils.ThrowException(rowIdx < 0 ? new ArgumentOutOfRangeException("rowIdx") : null);
            Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
            if (rowIdx >= mRows.Count) { return defaultVal; }
            return mRows[rowIdx].TryGet(colIdx, defaultVal);
        }

        public T this[int rowIdx, int colIdx]
        {
            get
            {
                Utils.ThrowException(rowIdx < 0 ? new ArgumentOutOfRangeException("rowIdx") : null);
                Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
                Utils.ThrowException(rowIdx >= mRows.Count ? new ArgumentValueException("rowIdx") : null);
                return mRows[rowIdx][colIdx]; // throws ArgumentValueException
            }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("value") : null);
                Utils.ThrowException(rowIdx < 0 ? new ArgumentOutOfRangeException("rowIdx") : null);
                Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
                if (rowIdx >= mRows.Count) { SetRowListSize(rowIdx + 1); }
                mRows[rowIdx][colIdx] = value;
            }
        }

        public SparseVector<T> GetColCopy(int colIdx)
        {
            Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
            SparseVector<T> col = new SparseVector<T>();
            int rowIdx = 0;
            foreach (SparseVector<T> row in mRows)
            {
                int directIdx = row.GetDirectIdx(colIdx);
                if (directIdx >= 0)
                {
                    col.InnerIdx.Add(rowIdx);
                    col.InnerDat.Add(row.GetDirect(directIdx).Dat);
                }
                rowIdx++;
            }
            return col;
        }

        public SparseMatrix<T> GetTransposedCopy()
        {
            SparseMatrix<T> trMat = new SparseMatrix<T>();
            int rowIdx = 0;
            foreach (SparseVector<T> row in mRows)
            {
                foreach (IdxDat<T> item in row)
                {
                    if (item.Idx >= trMat.mRows.Count)
                    {
                        trMat.SetRowListSize(item.Idx + 1);
                    }
                    trMat.mRows[item.Idx].InnerIdx.Add(rowIdx);
                    trMat.mRows[item.Idx].InnerDat.Add(item.Dat);
                }
                rowIdx++;
            }
            return trMat;
        }

        public bool Remove(T val)
        {
            Utils.ThrowException(val == null ? new ArgumentNullException("val") : null);
            bool valFound = false;
            foreach (SparseVector<T> row in mRows)
            {
                valFound = row.Remove(val);
            }
            return valFound;
        }

        public void RemoveAt(int rowIdx, int colIdx)
        {
            Utils.ThrowException(rowIdx < 0 ? new ArgumentOutOfRangeException("rowIdx") : null);
            Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
            if (rowIdx < mRows.Count)
            {
                mRows[rowIdx].RemoveAt(colIdx);
            }
        }

        public void RemoveRowAt(int rowIdx)
        {
            this[rowIdx] = null; // throws ArgumentOutOfRangeException
        }

        public void RemoveColAt(int colIdx)
        {
            Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
            foreach (SparseVector<T> row in mRows)
            {
                row.RemoveAt(colIdx);
            }
        }

        public void PurgeRowAt(int rowIdx)
        {
            Utils.ThrowException(rowIdx < 0 ? new ArgumentOutOfRangeException("rowIdx") : null);
            if (rowIdx < mRows.Count)
            {
                mRows.RemoveAt(rowIdx);
            }
        }

        public void PurgeColAt(int colIdx)
        {
            Utils.ThrowException(colIdx < 0 ? new ArgumentOutOfRangeException("colIdx") : null);
            foreach (SparseVector<T> row in mRows)
            {
                row.PurgeAt(colIdx);
            }
        }

        public void Clear()
        {
            mRows.Clear();
        }

        public void AppendCols(SparseMatrix<T> otherMatrix, int thisMatrixNumCols)
        {
            Utils.ThrowException(otherMatrix == null ? new ArgumentNullException("otherMatrix") : null);
            Utils.ThrowException(thisMatrixNumCols < 0 ? new ArgumentOutOfRangeException("thisMatrixNumCols") : null);            
            int otherMatrixNumRows = otherMatrix.GetLastNonEmptyRowIdx() + 1;
            if (mRows.Count < otherMatrixNumRows)
            {
                SetRowListSize(otherMatrixNumRows);
            }
            for (int rowIdx = 0; rowIdx < otherMatrixNumRows; rowIdx++)
            {
                mRows[rowIdx].Append(otherMatrix.mRows[rowIdx], thisMatrixNumCols); // throws ArgumentOutOfRangeException
            }
        }

        public void AppendCols(SparseMatrix<T>.ReadOnly otherMatrix, int thisMatrixNumCols)
        {
            Utils.ThrowException(otherMatrix == null ? new ArgumentNullException("otherMatrix") : null);
            AppendCols(otherMatrix.Inner, thisMatrixNumCols); // throws ArgumentOutOfRangeException
        }

        public void AppendRows(SparseMatrix<T> otherMatrix, int thisMatrixNumRows)
        {
            Utils.ThrowException(otherMatrix == null ? new ArgumentNullException("otherMatrix") : null);
            Utils.ThrowException(thisMatrixNumRows < (GetLastNonEmptyRowIdx() + 1) ? new ArgumentOutOfRangeException("thisMatrixNumRows") : null);
            int totalNumRows = (otherMatrix.GetLastNonEmptyRowIdx() + 1) + thisMatrixNumRows;
            if (mRows.Count < totalNumRows)
            {
                SetRowListSize(totalNumRows);
            }
            foreach (IdxDat<SparseVector<T>> row in otherMatrix)
            {
                mRows[thisMatrixNumRows + row.Idx] = row.Dat.Clone();
            }
        }

        public void AppendRows(SparseMatrix<T>.ReadOnly otherMatrix, int thisMatrixNumRows)
        {
            Utils.ThrowException(otherMatrix == null ? new ArgumentNullException("otherMatrix") : null);
            AppendRows(otherMatrix.Inner, thisMatrixNumRows); // throws ArgumentOutOfRangeException
        }

        public void Merge(SparseMatrix<T> otherMatrix, Utils.BinaryOperatorDelegate<T> binaryOperator)
        {
            Utils.ThrowException(binaryOperator == null ? new ArgumentNullException("binaryOperator") : null);
            int otherMatrixNumRows = otherMatrix.GetLastNonEmptyRowIdx() + 1;
            if (mRows.Count < otherMatrixNumRows)
            {
                SetRowListSize(otherMatrixNumRows);
            }
            for (int rowIdx = 0; rowIdx < otherMatrixNumRows; rowIdx++)
            {
                mRows[rowIdx].Merge(otherMatrix.mRows[rowIdx], binaryOperator);       
            }
        }

        public void Merge(SparseMatrix<T>.ReadOnly otherMatrix, Utils.BinaryOperatorDelegate<T> binaryOperator)
        {
            Merge(otherMatrix.Inner, binaryOperator); // throws ArgumentNullException
        }

        public void PerformUnaryOperation(Utils.UnaryOperatorDelegate<T> unaryOperator)
        {
            Utils.ThrowException(unaryOperator == null ? new ArgumentNullException("unaryOperator") : null);
            foreach (SparseVector<T> row in mRows)
            {
                row.PerformUnaryOperation(unaryOperator); 
            }
        }

        public void Symmetrize(Utils.BinaryOperatorDelegate<T> binOp)
        {
            SparseMatrix<T> trMat = GetTransposedCopy();
            trMat.RemoveDiagonal();
            trMat.MergeSym(this, binOp); // throws ArgumentNullException
            mRows = trMat.mRows;
        }

        public bool IsSymmetric()
        {
            int rowIdx = 0;
            foreach (SparseVector<T> row in mRows)
            {
                foreach (IdxDat<T> item in row)
                {
                    if (item.Idx >= mRows.Count) { return false; }
                    int directIdx = mRows[item.Idx].GetDirectIdx(rowIdx);
                    if (directIdx < 0) { return false; }
                    T val = mRows[item.Idx].GetDirect(directIdx).Dat;
                    if (!item.Dat.Equals(val)) { return false; }
                }
                rowIdx++;
            }
            return true;
        }

        public void SetDiagonal(int mtxSize, T val)
        {
            Utils.ThrowException(mtxSize < 0 ? new ArgumentOutOfRangeException("mtxSize") : null);
            Utils.ThrowException(val == null ? new ArgumentNullException("val") : null);
            if (mtxSize > mRows.Count) { SetRowListSize(mtxSize); }
            int rowIdx = 0;
            foreach (SparseVector<T> row in mRows)
            {
                row[rowIdx++] = val;
            }
        }

        public void RemoveDiagonal()
        {
            int rowIdx = 0;
            foreach (SparseVector<T> row in mRows)
            {
                row.RemoveAt(rowIdx++);
            }
        }

        public bool ContainsDiagonalElement()
        {
            int rowIdx = 0;
            foreach (SparseVector<T> row in mRows)
            {
                if (row.ContainsAt(rowIdx++)) { return true; }
            }
            return false;
        }

        private void MergeSym(SparseMatrix<T> otherMatrix, Utils.BinaryOperatorDelegate<T> binaryOperator)
        {
            int otherMatrixNumRows = otherMatrix.GetLastNonEmptyRowIdx() + 1;
            if (mRows.Count < otherMatrixNumRows)
            {
                SetRowListSize(otherMatrixNumRows);
            }
            for (int rowIdx = 0; rowIdx < otherMatrixNumRows; rowIdx++)
            {
                mRows[rowIdx] = MergeRowsSym(mRows[rowIdx], otherMatrix.mRows[rowIdx], binaryOperator, rowIdx);
            }
        }

        private SparseVector<T> MergeRowsSym(SparseVector<T> row, SparseVector<T> otherRow, Utils.BinaryOperatorDelegate<T> binaryOperator, int rowIdx)
        {
            SparseVector<T> newRow = new SparseVector<T>();
            ArrayList<int> idx = row.InnerIdx;
            ArrayList<T> dat = row.InnerDat;
            ArrayList<int> otherIdx = otherRow.InnerIdx;
            ArrayList<T> otherDat = otherRow.InnerDat;
            newRow.InnerIdx.Capacity = idx.Count + otherIdx.Count;
            newRow.InnerDat.Capacity = dat.Count + otherDat.Count;
            ArrayList<int> newIdx = newRow.InnerIdx;
            ArrayList<T> newDat = newRow.InnerDat;
            int i = 0, j = 0;
            while (i < idx.Count && j < otherIdx.Count)
            {
                int aIdx = idx[i];
                int bIdx = otherIdx[j];
                if (aIdx == bIdx)
                {
                    T value;
                    if (aIdx < rowIdx)
                    {
                        value = binaryOperator(dat[i], otherDat[j]);
                    }
                    else
                    {
                        value = binaryOperator(otherDat[j], dat[i]);
                    }
                    if (value != null) { newIdx.Add(aIdx); newDat.Add(value); }
                    i++;
                    j++;
                }
                else if (aIdx < bIdx)
                {
                    newIdx.Add(aIdx); newDat.Add(dat[i]);
                    i++;
                }
                else
                {
                    newIdx.Add(bIdx); newDat.Add(otherDat[j]);
                    j++;
                }
            }
            for (; i < idx.Count; i++)
            {
                newIdx.Add(idx[i]); newDat.Add(dat[i]);
            }
            for (; j < otherIdx.Count; j++)
            {
                newIdx.Add(otherIdx[j]); newDat.Add(otherDat[j]);
            }
            return newRow;
        }

        // *** IEnumerable<IdxDat<SparseVector<T>>> interface implementation ***

        public IEnumerator<IdxDat<SparseVector<T>>> GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        // *** IEnumerable interface implementation ***

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ICloneable interface implementation ***

        public SparseMatrix<T> Clone()
        {
            SparseMatrix<T> clone = new SparseMatrix<T>();
            foreach (SparseVector<T> row in mRows)
            {
                clone.mRows.Add(row.Clone());
            }
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IDeeplyCloneable interface implementation ***

        public SparseMatrix<T> DeepClone()
        {
            SparseMatrix<T> clone = new SparseMatrix<T>();
            clone.mRows = mRows.DeepClone();
            return clone;
        }

        object IDeeplyCloneable.DeepClone()
        {
            return DeepClone();
        }

        // *** IContentEquatable<SparseMatrix<T>> interface implementation ***

        public bool ContentEquals(SparseMatrix<T> other)
        {
            if (other == null) { return false; }
            int rowCount = GetLastNonEmptyRowIdx() + 1;
            if (rowCount != other.GetLastNonEmptyRowIdx() + 1) { return false; }
            for (int rowIdx = 0; rowIdx < rowCount; rowIdx++)
            {
                if (!mRows[rowIdx].ContentEquals(other.mRows[rowIdx])) { return false; }
            }
            return true;
        }

        bool IContentEquatable.ContentEquals(object other)
        {
            Utils.ThrowException((other != null && !(other is SparseMatrix<T>)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((SparseMatrix<T>)other);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            int rowCount = GetLastNonEmptyRowIdx() + 1;
            // the following statements throw serialization-related exceptions            
            writer.WriteInt(rowCount);
            for (int rowIdx = 0; rowIdx < rowCount; rowIdx++)
            {
                mRows[rowIdx].Save(writer);
            }
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mRows.Clear();
            // the following statements throw serialization-related exceptions
            int rowCount = reader.ReadInt();
            SetRowListSize(rowCount);
            for (int rowIdx = 0; rowIdx < rowCount; rowIdx++)
            {
                mRows[rowIdx] = new SparseVector<T>(reader);
            }
        }

        // *** Implicit cast to a read-only adapter ***

        public static implicit operator SparseMatrix<T>.ReadOnly(SparseMatrix<T> matrix)
        {
            if (matrix == null) { return null; }
            return new SparseMatrix<T>.ReadOnly(matrix);
        }

        // *** Equality comparer ***

        // *** note that two matrices are never equal if one is trimmed and the other is not
        public static IEqualityComparer<SparseMatrix<T>> GetEqualityComparer()
        {
            return GenericEqualityComparer<SparseMatrix<T>>.Instance;
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class MatrixEnumerator
           |
           '-----------------------------------------------------------------------
        */
        private class MatrixEnumerator : IEnumerator<IdxDat<SparseVector<T>>>
        {
            private SparseMatrix<T> mMatrix;
            private int mRowIdx
                = -1;

            public MatrixEnumerator(SparseMatrix<T> matrix)
            {
                Utils.ThrowException(matrix == null ? new ArgumentNullException("matrix") : null);
                mMatrix = matrix;
            }

            // *** IEnumerator<IdxDat<SparseVector<T>>> interface implementation ***

            public IdxDat<SparseVector<T>> Current
            {
                get { return new IdxDat<SparseVector<T>>(mRowIdx, mMatrix.mRows[mRowIdx]); } // throws ArgumentOutOfRangeException
            }

            // *** IEnumerator interface implementation ***

            object IEnumerator.Current
            {
                get { return Current; } // throws ArgumentOutOfRangeException
            }

            public bool MoveNext()
            {
                mRowIdx++;
                while (mRowIdx < mMatrix.mRows.Count && mMatrix.mRows[mRowIdx].Count == 0)
                {
                    mRowIdx++;
                }
                if (mRowIdx == mMatrix.mRows.Count)
                {
                    Reset();
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                mRowIdx = -1;
            }

            // *** IDisposable interface implementation ***

            public void Dispose()
            {
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class ReadOnlyMatrixEnumerator
           |
           '-----------------------------------------------------------------------
        */
        private class ReadOnlyMatrixEnumerator : IEnumerator<IdxDat<SparseVector<T>.ReadOnly>>
        {
            private SparseMatrix<T> mMatrix;
            private int mRowIdx
                = -1;

            public ReadOnlyMatrixEnumerator(SparseMatrix<T> matrix)
            {
                Utils.ThrowException(matrix == null ? new ArgumentNullException("matrix") : null);
                mMatrix = matrix;
            }

            // *** IEnumerator<IdxDat<SparseVector<T>.ReadOnly>> interface implementation ***

            public IdxDat<SparseVector<T>.ReadOnly> Current
            {
                get { return new IdxDat<SparseVector<T>.ReadOnly>(mRowIdx, mMatrix.mRows[mRowIdx]); } // throws ArgumentOutOfRangeException
            }

            // *** IEnumerator interface implementation ***

            object IEnumerator.Current
            {
                get { return Current; } // throws ArgumentOutOfRangeException
            }

            public bool MoveNext()
            {
                mRowIdx++;
                while (mRowIdx < mMatrix.mRows.Count && mMatrix.mRows[mRowIdx].Count == 0)
                {
                    mRowIdx++;
                }
                if (mRowIdx == mMatrix.mRows.Count)
                {
                    Reset();
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                mRowIdx = -1;
            }

            // *** IDisposable interface implementation ***

            public void Dispose()
            {
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class SparseMatrix<T>.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<SparseMatrix<T>>, IEnumerable<IdxDat<SparseVector<T>.ReadOnly>>, IContentEquatable<SparseMatrix<T>.ReadOnly>,
            ISerializable
        {
            private SparseMatrix<T> mMatrix;

            public ReadOnly(SparseMatrix<T> matrix)
            {
                Utils.ThrowException(matrix == null ? new ArgumentNullException("matrix") : null);
                mMatrix = matrix;
            }

            public ReadOnly(BinarySerializer reader)
            {
                mMatrix = new SparseMatrix<T>(reader); // throws ArgumentNullException, serialization-related exceptions
            }

            public bool ContainsRowAt(int rowIdx)
            {
                return mMatrix.ContainsRowAt(rowIdx);
            }

            public bool ContainsColAt(int colIdx)
            {
                return mMatrix.ContainsColAt(colIdx);
            }

            public bool ContainsAt(int rowIdx, int colIdx)
            {
                return mMatrix.ContainsAt(rowIdx, colIdx);
            }

            public int GetFirstNonEmptyRowIdx()
            {
                return mMatrix.GetFirstNonEmptyRowIdx();
            }

            public int GetLastNonEmptyRowIdx()
            {
                return mMatrix.GetLastNonEmptyRowIdx();
            }

            public int GetFirstNonEmptyColIdx()
            {
                return mMatrix.GetFirstNonEmptyColIdx();
            }

            public int GetLastNonEmptyColIdx()
            {
                return mMatrix.GetLastNonEmptyColIdx();
            }

            public override string ToString()
            {
                return mMatrix.ToString();
            }

            public string ToString(string format)
            {
                return mMatrix.ToString(format);
            }

            public bool IndexOf(T val, ref int rowIdx, ref int colIdx)
            {
                return mMatrix.IndexOf(val, ref rowIdx, ref colIdx);
            }

            public bool Contains(T val)
            {
                return mMatrix.Contains(val);
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public int GetRowCount()
            {
                return mMatrix.GetRowCount();
            }

            public int CountValues()
            {
                return mMatrix.CountValues();
            }

            public double GetSparseness(int numRows, int numCols)
            {
                return mMatrix.GetSparseness(numRows, numCols);
            }

            public SparseVector<T>.ReadOnly this[int rowIdx]
            {
                get { return mMatrix[rowIdx]; }
            }

            public T TryGet(int rowIdx, int colIdx, T defaultVal)
            {
                return mMatrix.TryGet(rowIdx, colIdx, defaultVal);
            }

            public T this[int rowIdx, int colIdx]
            {
                get { return mMatrix[rowIdx, colIdx]; }
            }

            public SparseVector<T> GetColCopy(int colIdx)
            {
                return mMatrix.GetColCopy(colIdx);
            }

            public SparseMatrix<T> GetTransposedCopy()
            {
                return mMatrix.GetTransposedCopy();
            }

            public bool IsSymmetric()
            {
                return mMatrix.IsSymmetric();
            }

            public bool HasDiagonal()
            {
                return mMatrix.ContainsDiagonalElement();
            }

            // *** IReadOnlyAdapter interface implementation ***

            public SparseMatrix<T> GetWritableCopy()
            {
                return mMatrix.Clone();
            }

            object IReadOnlyAdapter.GetWritableCopy()
            {
                return GetWritableCopy();
            }

            public SparseMatrix<T> Inner
            {
                get { return mMatrix; }
            }

            object IReadOnlyAdapter.Inner
            {
                get { return Inner; }
            }

            // *** IEnumerable<IdxDat<SparseVector<T>.ReadOnly>> interface implementation ***

            public IEnumerator<IdxDat<SparseVector<T>.ReadOnly>> GetEnumerator()
            {
                return new ReadOnlyMatrixEnumerator(mMatrix);
            }

            // *** IEnumerable interface implementation ***

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            // *** IContentEquatable<SparseMatrix<T>.ReadOnly> interface implementation ***

            public bool ContentEquals(SparseMatrix<T>.ReadOnly other)
            {
                return other != null && mMatrix.ContentEquals(other.Inner);
            }

            bool IContentEquatable.ContentEquals(object other)
            {
                Utils.ThrowException(other != null && !(other is SparseMatrix<T>.ReadOnly) ? new ArgumentTypeException("other") : null);
                return ContentEquals((SparseMatrix<T>.ReadOnly)other);
            }

            // *** ISerializable interface implementation ***

            public void Save(BinarySerializer writer)
            {
                mMatrix.Save(writer);
            }

            // *** Equality comparer ***

            // *** note that two matrices are never equal if one is trimmed and the other is not
            public static IEqualityComparer<SparseMatrix<T>.ReadOnly> GetEqualityComparer()
            {
                return GenericEqualityComparer<SparseMatrix<T>.ReadOnly>.Instance;
            }
        }
    }
}
