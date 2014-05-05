/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    LabeledDataset.cs
 *  Desc:    Labeled dataset data structure
 *  Created: Aug-2007
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
       |  Class LabeledDataset<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public class LabeledDataset<LblT, ExT> : ILabeledDataset<LblT, ExT>, IUnlabeledExampleCollection<ExT>
    {
        private ArrayList<LabeledExample<LblT, ExT>> mItems
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
            foreach (LabeledExample<LblT, object> labeledExample in examples)
            {
                mItems.Add(new LabeledExample<LblT, ExT>(labeledExample.Label, (ExT)labeledExample.Example));
            }
        }

        public LabeledDataset(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public void Add(LblT label, ExT example)
        {
            mItems.Add(new LabeledExample<LblT, ExT>(label, example)); // throws ArgumentNullException
        }

        public void Add(LabeledExample<LblT, ExT> labeledExample)
        {
            Utils.ThrowException(labeledExample == null ? new ArgumentNullException("labeledExample") : null); 
            mItems.Add(labeledExample);
        }

        public void AddRange(IEnumerable<LabeledExample<LblT, ExT>> examples)
        {
            Utils.ThrowException(examples == null ? new ArgumentNullException("examples") : null);
            foreach (LabeledExample<LblT, ExT> labeledExample in examples)
            {
                Utils.ThrowException(labeledExample == null ? new ArgumentValueException("examples") : null);
                mItems.Add(labeledExample);
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

        public void Shuffle()
        {
            mItems.Shuffle();
        }

        public void Shuffle(Random rnd)
        {
            mItems.Shuffle(rnd); // throws ArgumentNullException
        }

        public void SplitForCrossValidation(int numFolds, int fold, out LabeledDataset<LblT, ExT> trainSet, out LabeledDataset<LblT, ExT> testSet)
        {
            Utils.ThrowException(mItems.Count < 2 ? new InvalidOperationException() : null);
            Utils.ThrowException((numFolds < 2 || numFolds > mItems.Count) ? new ArgumentOutOfRangeException("numFolds") : null);
            Utils.ThrowException((fold < 1 || fold > numFolds) ? new ArgumentOutOfRangeException("fold") : null);
            trainSet = new LabeledDataset<LblT, ExT>();
            testSet = new LabeledDataset<LblT, ExT>();
            double step = (double)mItems.Count / (double)numFolds;
            double d = 0;
            for (int i = 0; i < numFolds; i++, d += step)
            {
                int endJ = (int)Math.Round(d + step);
                if (i == fold - 1)
                {
                    for (int j = (int)Math.Round(d); j < endJ; j++)
                    {
                        testSet.Add(mItems[j].Label, mItems[j].Example);
                    }
                }
                else
                {
                    for (int j = (int)Math.Round(d); j < endJ; j++)
                    {
                        trainSet.Add(mItems[j].Label, mItems[j].Example);
                    }
                }
            }
        }

        // *** ILabeledDataset<LblT, ExT> interface implementation ***

        public Type ExampleType
        {
            get { return typeof(ExT); }
        }

        public int Count
        {
            get { return mItems.Count; }
        }

        public LabeledExample<LblT, ExT> this[int index]
        {
            get { return mItems[index]; } // throws ArgumentOutOfRangeException
        }

        object IEnumerableList.this[int index]
        {
            get { return this[index]; } // throws ArgumentOutOfRangeException
        }

        public IEnumerator<LabeledExample<LblT, ExT>> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ILabeledDataset<LblT, NewExT> ConvertDataset<NewExT>(bool move)
        {
            return (ILabeledDataset<LblT, NewExT>)ConvertDataset(typeof(NewExT), move); // throws ArgumentNotSupportedException
        }

        public ILabeledDataset<LblT> ConvertDataset(Type newExType, bool move)
        {
            Utils.ThrowException(newExType == null ? new ArgumentNullException("newExType") : null);
            ILabeledDataset<LblT> newDataset = null;
            ArrayList<LabeledExample<LblT, object>> tmp = new ArrayList<LabeledExample<LblT, object>>(mItems.Count);
            for (int i = 0; i < mItems.Count; i++)
            {
                tmp.Add(new LabeledExample<LblT, object>(mItems[i].Label, ModelUtils.ConvertExample(mItems[i].Example, newExType))); // throws ArgumentValueException
                if (move) { mItems[i] = null; }
            }
            if (move) { mItems.Clear(); }
            if (newExType == typeof(SparseVector<double>))
            {
                newDataset = new LabeledDataset<LblT, SparseVector<double>>(tmp);
            }
            else if (newExType == typeof(SparseVector<double>.ReadOnly))
            {
                newDataset = new LabeledDataset<LblT, SparseVector<double>.ReadOnly>(tmp);
            }
            else if (newExType == typeof(BinaryVector))
            {
                newDataset = new LabeledDataset<LblT, BinaryVector>(tmp);
            }
            else if (newExType == typeof(BinaryVector.ReadOnly))
            {
                newDataset = new LabeledDataset<LblT, BinaryVector.ReadOnly>(tmp);
            }
            else
            {
                throw new ArgumentNotSupportedException("newExType");
            }
            return newDataset;
        }

        public UnlabeledDataset<ExT> ToUnlabeledDataset()
        {
            return ModelUtils.ConvertToUnlabeledDataset<LblT, ExT>(this);
        }

        // *** IUnlabeledExampleCollection<ExT> interface implementation ***

        ExT IEnumerableList<ExT>.this[int index]
        {
            get { return mItems[index].Example; } // throws ArgumentOutOfRangeException
        }

        IEnumerator<ExT> IEnumerable<ExT>.GetEnumerator()
        {
            return new UnlabeledDatasetEnumerator(this);
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

        /* .-----------------------------------------------------------------------
           |
           |  Class UnlabeledDatasetEnumerator
           |
           '-----------------------------------------------------------------------
        */
        public class UnlabeledDatasetEnumerator : IEnumerator<ExT>
        { 
            private LabeledDataset<LblT, ExT> mDataset;
            private int mIdx
                = -1;

            public UnlabeledDatasetEnumerator(LabeledDataset<LblT, ExT> dataset)
            {
                Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
                mDataset = dataset;
            }

            // *** IEnumerator<ExT> interface implementation ***

            public ExT Current
            {
                get { return mDataset[mIdx].Example; } // throws ArgumentOutOfRangeException
            }

            // *** IEnumerator interface implementation ***

            object IEnumerator.Current
            {
                get { return Current; } // throws ArgumentOutOfRangeException
            }

            public bool MoveNext()
            {
                mIdx++;
                if (mIdx >= mDataset.Count)
                {
                    Reset();
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                mIdx = -1;
            }

            // *** IDisposable interface implementation ***

            public void Dispose()
            {
            }        
        }
    }
}