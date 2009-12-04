/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          LabeledDataset.cs
 *  Version:       1.0
 *  Desc:		   Dataset for training ML models
 *  Author:        Miha Grcar
 *  Created on:    Aug-2007
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
       |  Class LabeledDataset<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public class LabeledDataset<LblT, ExT> : ILabeledDataset<LblT, ExT>, IUnlabeledExampleCollection<ExT>
    {
        private ArrayList<LabeledExample<LblT, ExT>> m_items
            = new ArrayList<LabeledExample<LblT, ExT>>();

        public LabeledDataset()
        {
        }

        public LabeledDataset(IEnumerable<LabeledExample<LblT, ExT>> examples)
        {
            AddRange(examples); // throws ArgumentNullException
        }

        private LabeledDataset(IEnumerable<LabeledExample<LblT, object>> examples)
        {           
            foreach (LabeledExample<LblT, object> labeled_example in examples)
            {
                m_items.Add(new LabeledExample<LblT, ExT>(labeled_example.Label, (ExT)labeled_example.Example));
            }
        }

        public LabeledDataset(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public void Add(LblT label, ExT example)
        {
            Utils.ThrowException(label == null ? new ArgumentNullException("label") : null); // *** allow unlabeled examples?
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            m_items.Add(new LabeledExample<LblT, ExT>(label, example));
        }

        public void Add(LabeledExample<LblT, ExT> example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null); 
            m_items.Add(example);
        }

        public void AddRange(IEnumerable<LabeledExample<LblT, ExT>> examples)
        {
            Utils.ThrowException(examples == null ? new ArgumentNullException("examples") : null);
            foreach (LabeledExample<LblT, ExT> labeled_example in examples)
            {
                Utils.ThrowException(labeled_example == null ? new ArgumentNullException("examples item") : null);
                m_items.Add(new LabeledExample<LblT, ExT>(labeled_example.Label, labeled_example.Example));
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

        public void Shuffle()
        {
            m_items.Shuffle();
        }

        public void Shuffle(Random rnd)
        {
            m_items.Shuffle(rnd); // throws ArgumentNullException
        }

        public void SplitForCrossValidation(int num_folds, int fold, ref LabeledDataset<LblT, ExT> train_set, ref LabeledDataset<LblT, ExT> test_set)
        {
            Utils.ThrowException(m_items.Count < 2 ? new InvalidOperationException() : null);
            Utils.ThrowException((num_folds < 2 || num_folds > m_items.Count) ? new ArgumentOutOfRangeException("num_folds") : null);
            Utils.ThrowException((fold < 1 || fold > num_folds) ? new ArgumentOutOfRangeException("fold") : null);
            train_set = new LabeledDataset<LblT, ExT>();
            test_set = new LabeledDataset<LblT, ExT>();
            double step = (double)m_items.Count / (double)num_folds;
            double d = 0;
            for (int i = 0; i < num_folds; i++, d += step)
            {
                int end_j = (int)Math.Round(d + step);
                if (i == fold - 1)
                {
                    for (int j = (int)Math.Round(d); j < end_j; j++)
                    {
                        test_set.Add(m_items[j].Label, m_items[j].Example);
                    }
                }
                else
                {
                    for (int j = (int)Math.Round(d); j < end_j; j++)
                    {
                        train_set.Add(m_items[j].Label, m_items[j].Example);
                    }
                }
            }
        }

        UnlabeledDataset<ExT> GetUnlabeledDataset()
        {
            UnlabeledDataset<ExT> dataset = new UnlabeledDataset<ExT>();
            foreach (LabeledExample<LblT, ExT> labeled_example in m_items)
            {
                dataset.Add(labeled_example.Example);
            }
            return dataset;
        }

        // *** ILabeledDataset<LblT, ExT> interface implementation ***

        public Type ExampleType
        {
            get { return typeof(ExT); }
        }

        public int Count
        {
            get { return m_items.Count; }
        }

        public LabeledExample<LblT, ExT> this[int index]
        {
            get { return m_items[index]; } // throws ArgumentOutOfRangeException
        }

        object IEnumerableList.this[int index]
        {
            get { return this[index]; } // throws ArgumentOutOfRangeException
        }

        public IEnumerator<LabeledExample<LblT, ExT>> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ILabeledDataset<LblT, NewExT> ConvertDataset<NewExT>(bool move)
        {
            return (ILabeledDataset<LblT, NewExT>)ConvertDataset(typeof(NewExT), move); // throws ArgumentNotSupportedException
        }

        public ILabeledDataset<LblT> ConvertDataset(Type new_ex_type, bool move)
        {
            Utils.ThrowException(new_ex_type == null ? new ArgumentNullException("new_ex_type") : null);
            ILabeledDataset<LblT> new_dataset = null;
            ArrayList<LabeledExample<LblT, object>> tmp = new ArrayList<LabeledExample<LblT, object>>(m_items.Count);
            for (int i = 0; i < m_items.Count; i++)
            {
                tmp.Add(new LabeledExample<LblT, object>(m_items[i].Label, ModelUtils.ConvertExample(m_items[i].Example, new_ex_type))); // throws ArgumentValueException
                if (move) { m_items[i] = null; }
            }
            if (move) { m_items.Clear(); }
            if (new_ex_type == typeof(SparseVector<double>))
            {
                new_dataset = new LabeledDataset<LblT, SparseVector<double>>(tmp);
            }
            else if (new_ex_type == typeof(SparseVector<double>.ReadOnly))
            {
                new_dataset = new LabeledDataset<LblT, SparseVector<double>.ReadOnly>(tmp);
            }
            else if (new_ex_type == typeof(BinaryVector<int>))
            {
                new_dataset = new LabeledDataset<LblT, BinaryVector<int>>(tmp);
            }
            else if (new_ex_type == typeof(BinaryVector<int>.ReadOnly))
            {
                new_dataset = new LabeledDataset<LblT, BinaryVector<int>.ReadOnly>(tmp);
            }
            //else if (new_ex_type == typeof(SvmFeatureVector))
            //{
            //    new_dataset = new LabeledDataset<LblT, SvmFeatureVector>(tmp);
            //}
            return new_dataset;
        }

        // *** IUnlabeledExampleCollection<ExT> interface implementation ***

        ExT IEnumerableList<ExT>.this[int index]
        {
            get { return m_items[index].Example; } // throws ArgumentOutOfRangeException
        }

        IEnumerator<ExT> IEnumerable<ExT>.GetEnumerator()
        {
            return new UnlabeledDatasetEnumerator(this);
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

        /* .-----------------------------------------------------------------------
           |
           |  Class UnlabeledDatasetEnumerator
           |
           '-----------------------------------------------------------------------
        */
        public class UnlabeledDatasetEnumerator : IEnumerator<ExT>
        { 
            private LabeledDataset<LblT, ExT> m_dataset;
            private int m_idx
                = -1;

            public UnlabeledDatasetEnumerator(LabeledDataset<LblT, ExT> dataset)
            {
                Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
                m_dataset = dataset;
            }

            // *** IEnumerator<ExT> interface implementation ***

            public ExT Current
            {
                get { return m_dataset[m_idx].Example; } // throws ArgumentOutOfRangeException
            }

            // *** IEnumerator interface implementation ***

            object IEnumerator.Current
            {
                get { return Current; } // throws ArgumentOutOfRangeException
            }

            public bool MoveNext()
            {
                m_idx++;
                if (m_idx >= m_dataset.Count)
                {
                    Reset();
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                m_idx = -1;
            }

            // *** IDisposable interface implementation ***

            public void Dispose()
            {
            }        
        }
    }
}