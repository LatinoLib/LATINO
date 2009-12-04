/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          UnlabeledDataset.cs
 *  Version:       1.0
 *  Desc:		   Dataset for clustering
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
        private ArrayList<ExT> m_items
            = new ArrayList<ExT>();

        public UnlabeledDataset()
        {
        }

        public UnlabeledDataset(IEnumerable<ExT> examples)
        {
            m_items.AddRange(examples); // throws ArgumentNullException
        }

        private UnlabeledDataset(IEnumerable<object> examples)
        {
            foreach (object example in examples)
            {
                m_items.Add((ExT)example);
            }
        }

        public UnlabeledDataset(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public void Add(ExT example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            m_items.Add(example);
        }

        public void AddRange(IEnumerable<ExT> examples)
        {
            Utils.ThrowException(examples == null ? new ArgumentNullException("examples") : null);
            foreach (ExT example in examples)
            {
                Utils.ThrowException(example == null ? new ArgumentNullException("examples item") : null);
                m_items.Add(example);
            }
        }

        public void RemoveAt(int index)
        {
            m_items.RemoveAt(index); // throws ArgumentOutOfRangeException
        }

        public void RemoveRange(int index, int count)
        {
            m_items.RemoveRange(index, count); // throws ArgumentOutOfRangeException, ArgumentException
        }

        public void Clear()
        {
            m_items.Clear();
        }

        // *** IUnlabeledDataset<ExT> interface implementation ***

        public Type ExampleType
        {
            get { return typeof(ExT); }
        }

        public int Count
        {
            get { return m_items.Count; }
        }

        public ExT this[int index]
        {
            get { return m_items[index]; } // throws ArgumentOutOfRangeException
        }

        object IEnumerableList.this[int index]
        {
            get { return this[index]; } // throws ArgumentOutOfRangeException
        }

        public IEnumerator<ExT> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IUnlabeledDataset<NewExT> ConvertDataset<NewExT>(bool move)
        {
            return (IUnlabeledDataset<NewExT>)ConvertDataset(typeof(NewExT), move); // throws ArgumentNotSupportedException
        }

        public IUnlabeledDataset ConvertDataset(Type new_ex_type, bool move)
        {
            Utils.ThrowException(new_ex_type == null ? new ArgumentNullException("new_ex_type") : null);
            Utils.ThrowException(move && typeof(ExT).IsValueType ? new ArgumentValueException("new_ex_type") : null);
            IUnlabeledDataset new_dataset = null;
            ArrayList<object> tmp = new ArrayList<object>(m_items.Count);
            for (int i = 0; i < m_items.Count; i++)
            {
                tmp.Add(ModelUtils.ConvertExample(m_items[i], new_ex_type)); // throws ArgumentValueException
                if (move) { m_items[i] = default(ExT); } // *** this is guaranteed to be null by the second assertion
            }
            if (move) { m_items.Clear(); }
            if (new_ex_type == typeof(SparseVector<double>))
            {
                new_dataset = new UnlabeledDataset<SparseVector<double>>(tmp);
            }
            else if (new_ex_type == typeof(SparseVector<double>.ReadOnly))
            {
                new_dataset = new UnlabeledDataset<SparseVector<double>.ReadOnly>(tmp);
            }
            else if (new_ex_type == typeof(BinaryVector<int>))
            {
                new_dataset = new UnlabeledDataset<BinaryVector<int>>(tmp);
            }
            else if (new_ex_type == typeof(BinaryVector<int>.ReadOnly))
            {
                new_dataset = new UnlabeledDataset<BinaryVector<int>.ReadOnly>(tmp);
            }
            //else if (new_ex_type == typeof(SvmFeatureVector))
            //{
            //    new_dataset = new UnlabeledDataset<SvmFeatureVector>(tmp);
            //}
            return new_dataset;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            m_items.Save(writer); // throws serialization-related exceptions
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            m_items.Load(reader); // throws serialization-related exceptions
        }
    }
}