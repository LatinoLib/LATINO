/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          MaxEntClassifier.cs
 *  Version:       1.0
 *  Desc:		   Maximum entropy classifier (LATINO wrapper)
 *  Author:        Miha Grcar
 *  Created on:    Oct-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class MaximumEntropyClassifier<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class MaximumEntropyClassifier<LblT> : IModel<LblT, BinaryVector<int>.ReadOnly>
    {
        private bool mMoveData
            = false;
        private int mNumIter
            = 100;
        private int mCutOff
            = 0;
        private int mNumThreads
            = 1;
        private SparseMatrix<double> mLambda
            = null;
        private LblT[] mIdxToLbl
            = null;

        public MaximumEntropyClassifier()
        {
        }

        public MaximumEntropyClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public bool MoveData
        {
            get { return mMoveData; }
            set { mMoveData = value; }
        }

        public int NumIter
        {
            get { return mNumIter; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("NumIter") : null);
                mNumIter = value;
            }
        }

        public int CutOff
        {
            get { return mCutOff; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("CutOff") : null);
                mCutOff = value;
            }
        }

        public int NumThreads
        {
            get { return mNumThreads; }
            set 
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("NumThreads") : null);
                mNumThreads = value;             
            }
        }

        // *** IModel<LblT, BinaryVector<int>.ReadOnly> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(BinaryVector<int>.ReadOnly); }
        }

        public bool IsTrained
        {
            get { return mLambda != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, BinaryVector<int>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            mLambda = null; // allow GC to collect this
            mLambda = MaxEnt.Gis(dataset, mCutOff, mNumIter, mMoveData, /*mtxFileName=*/null, ref mIdxToLbl, mNumThreads, /*allowedDiff=*/0); // *** allowedDiff
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, BinaryVector<int>.ReadOnly>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, BinaryVector<int>.ReadOnly>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(BinaryVector<int>.ReadOnly example)
        {
            Utils.ThrowException(mLambda == null ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            return MaxEnt.Classify(example, mLambda, mIdxToLbl, /*normalize=*/false); // *** normalize
        }

        Prediction<LblT> IModel<LblT>.Predict(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is BinaryVector<int>.ReadOnly) ? new ArgumentTypeException("example") : null);
            return Predict((BinaryVector<int>.ReadOnly)example); // throws InvalidOperationException
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteBool(mMoveData);
            writer.WriteInt(mNumIter);
            writer.WriteInt(mCutOff);
            writer.WriteInt(mNumThreads);
            mLambda.Save(writer);
            new ArrayList<LblT>(mIdxToLbl).Save(writer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions            
            mMoveData = reader.ReadBool();
            mNumIter = reader.ReadInt();
            mCutOff = reader.ReadInt();
            mNumThreads = reader.ReadInt();
            mLambda = new SparseMatrix<double>(reader);
            mIdxToLbl = new ArrayList<LblT>(reader).ToArray();
        }
    }
}
