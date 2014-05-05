/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    MaxEntClassifier.cs
 *  Desc:    Maximum entropy classifier (LATINO wrapper)
 *  Created: Oct-2009
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
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
    public class MaximumEntropyClassifier<LblT> : IModel<LblT, BinaryVector>
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
        private bool mNormalize
            = false;
        private IEqualityComparer<LblT> mLblCmp;

        private Logger mLogger
            = Logger.GetLogger(typeof(MaximumEntropyClassifier<LblT>));

        public MaximumEntropyClassifier(IEqualityComparer<LblT> lblCmp)
        {
            mLblCmp = lblCmp;
        }

        public MaximumEntropyClassifier() : this((IEqualityComparer<LblT>)null)
        {
        }

        public MaximumEntropyClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public Logger Logger
        {
            get { return mLogger; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Logger") : null);
                mLogger = value;
            }
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

        public bool Normalize
        {
            get { return mNormalize; }
            set { mNormalize = value; }
        }

        // *** IModel<LblT, BinaryVector> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(BinaryVector); }
        }

        public bool IsTrained
        {
            get { return mLambda != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, BinaryVector> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            mLambda = null; // allow GC to collect this
            mLambda = MaxEnt.Gis(dataset, mCutOff, mNumIter, mMoveData, /*mtxFileName=*/null, ref mIdxToLbl, mNumThreads, /*allowedDiff=*/0, mLblCmp, mLogger); // *** allowedDiff
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, BinaryVector>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, BinaryVector>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(BinaryVector example)
        {
            Utils.ThrowException(mLambda == null ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            return MaxEnt.Classify(example, mLambda, mIdxToLbl, mNormalize); 
        }

        Prediction<LblT> IModel<LblT>.Predict(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is BinaryVector) ? new ArgumentTypeException("example") : null);
            return Predict((BinaryVector)example); // throws InvalidOperationException
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
            writer.WriteObject(mLambda);
            if (mLambda != null) { new ArrayList<LblT>(mIdxToLbl).Save(writer); }
            writer.WriteBool(mNormalize);
            writer.WriteObject(mLblCmp);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions            
            mMoveData = reader.ReadBool();
            mNumIter = reader.ReadInt();
            mCutOff = reader.ReadInt();
            mNumThreads = reader.ReadInt();
            mLambda = reader.ReadObject<SparseMatrix<double>>();
            mIdxToLbl = (mLambda != null) ? new ArrayList<LblT>(reader).ToArray() : null;
            mNormalize = reader.ReadBool();
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
        }
    }
}
