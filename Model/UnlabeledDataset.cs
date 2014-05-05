/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    UnlabeledDataset.cs
 *  Desc:    Unlabeled dataset data structure
 *  Created: Nov-2009
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class UnlabeledDataset<ExT>
       |
       '-----------------------------------------------------------------------
    */
    public class UnlabeledDataset<ExT> : IUnlabeledDataset<ExT>
    {
        private ArrayList<ExT> mItems
            = new ArrayList<ExT>();

        public UnlabeledDataset()
        {
        }

        public UnlabeledDataset(IEnumerable<ExT> examples)
        {
            mItems.AddRange(examples); // throws ArgumentNullException
        }

        private UnlabeledDataset(IEnumerable examples)
        {
            foreach (object example in examples)
            {
                mItems.Add((ExT)example);
            }
        }

        public UnlabeledDataset(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public void Add(ExT example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            mItems.Add(example);
        }

        public void AddRange(IEnumerable<ExT> examples)
        {
            Utils.ThrowException(examples == null ? new ArgumentNullException("examples") : null);
            foreach (ExT example in examples)
            {
                Utils.ThrowException(example == null ? new ArgumentValueException("examples") : null);
                mItems.Add(example);
            }
        }

        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index); // throws ArgumentOutOfRangeException
        }

        public void RemoveRange(int index, int count)
        {
            mItems.RemoveRange(index, count); // throws ArgumentOutOfRangeException, ArgumentException
        }

        public void Clear()
        {
            mItems.Clear();
        }

        // *** IUnlabeledDataset<ExT> interface implementation ***

        public Type ExampleType
        {
            get { return typeof(ExT); }
        }

        public int Count
        {
            get { return mItems.Count; }
        }

        public ExT this[int index]
        {
            get { return mItems[index]; } // throws ArgumentOutOfRangeException
        }

        object IEnumerableList.this[int index]
        {
            get { return this[index]; } // throws ArgumentOutOfRangeException
        }

        public IEnumerator<ExT> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IUnlabeledDataset<NewExT> ConvertDataset<NewExT>(bool move)
        {
            return (IUnlabeledDataset<NewExT>)ConvertDataset(typeof(NewExT), move); // throws ArgumentNotSupportedException
        }

        public IUnlabeledDataset ConvertDataset(Type newExType, bool move)
        {
            Utils.ThrowException(newExType == null ? new ArgumentNullException("newExType") : null);
            Utils.ThrowException(move && typeof(ExT).IsValueType ? new ArgumentValueException("newExType") : null);
            IUnlabeledDataset newDataset = null;
            ArrayList<object> tmp = new ArrayList<object>(mItems.Count);
            for (int i = 0; i < mItems.Count; i++)
            {
                tmp.Add(ModelUtils.ConvertExample(mItems[i], newExType)); // throws ArgumentValueException
                if (move) { mItems[i] = default(ExT); } // *** this is guaranteed to be null by the second assertion
            }
            if (move) { mItems.Clear(); }
            if (newExType == typeof(SparseVector<double>))
            {
                newDataset = new UnlabeledDataset<SparseVector<double>>(tmp);
            }
            else if (newExType == typeof(SparseVector<double>.ReadOnly))
            {
                newDataset = new UnlabeledDataset<SparseVector<double>.ReadOnly>(tmp);
            }
            else if (newExType == typeof(BinaryVector))
            {
                newDataset = new UnlabeledDataset<BinaryVector>(tmp);
            }
            else if (newExType == typeof(BinaryVector.ReadOnly))
            {
                newDataset = new UnlabeledDataset<BinaryVector.ReadOnly>(tmp);
            }
            else
            {
                throw new ArgumentNotSupportedException("newExType");
            }
            return newDataset;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            mItems.Save(writer); // throws serialization-related exceptions
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mItems.Load(reader); // throws serialization-related exceptions
        }
    }
}