/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    MajorityClassifier.cs
 *  Desc:    Majority classifier (baseline)
 *  Created: Oct-2010
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
       |  Class MajorityClassifier<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public class MajorityClassifier<LblT, ExT> : IModel<LblT, ExT>
    {
        private IEqualityComparer<LblT> mLblCmp;
        private Prediction<LblT> mPrediction
            = null;

        public MajorityClassifier(IEqualityComparer<LblT> lblCmp)
        {
            mLblCmp = lblCmp;
        }

        public MajorityClassifier() : this((IEqualityComparer<LblT>)null)
        {
        }

        public MajorityClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        // *** IModel<LblT, ExT> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(ExT); }
        }

        public bool IsTrained
        {
            get { return mPrediction != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, ExT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);            
            MultiSet<LblT> counter = new MultiSet<LblT>(mLblCmp);
            foreach (LabeledExample<LblT, ExT> lblEx in dataset)
            {
                counter.Add(lblEx.Label);
            }
            mPrediction = new Prediction<LblT>();
            foreach (KeyValuePair<LblT, int> keyVal in counter)
            {
                mPrediction.Inner.Add(new KeyDat<double, LblT>((double)keyVal.Value / (double)dataset.Count, keyVal.Key));
            }
            mPrediction.Inner.Sort(DescSort<KeyDat<double, LblT>>.Instance);
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, ExT>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, ExT>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(ExT example)
        {
            Utils.ThrowException(mPrediction == null ? new InvalidOperationException() : null);
            return mPrediction.Clone();
        }

        Prediction<LblT> IModel<LblT>.Predict(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is ExT) ? new ArgumentTypeException("example") : null);
            return Predict((ExT)example); // throws InvalidOperationException
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteObject(mPrediction);
            writer.WriteObject(mLblCmp);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mPrediction = reader.ReadObject<Prediction<LblT>>();
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
        }
    }
}
