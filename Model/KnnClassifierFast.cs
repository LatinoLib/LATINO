/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    KnnClassifierFast.cs
 *  Desc:    K-nearest neighbor classifier (optimized for speed)
 *  Created: Mar-2010
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
       |  Class KnnClassifierFast<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class KnnClassifierFast<LblT> : IModel<LblT, SparseVector<double>>
    {
        SparseMatrix<double> mDatasetMtx
            = null;
        ArrayList<LblT> mLabels
            = null;
        private IEqualityComparer<LblT> mLblCmp;
        private int mK
            = 10;
        private bool mSoftVoting
            = true;

        public KnnClassifierFast(IEqualityComparer<LblT> lblCmp)
        { 
            mLblCmp = lblCmp;
        }

        public KnnClassifierFast() : this((IEqualityComparer<LblT>)null)
        {        
        }

        public KnnClassifierFast(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public int K
        {
            get { return mK; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("K") : null);
                mK = value;
            }
        }

        public bool SoftVoting
        {
            get { return mSoftVoting; }
            set { mSoftVoting = value; }
        }

        // *** IModel<LblT, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }

        public bool IsTrained
        {
            get { return mDatasetMtx != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            mDatasetMtx = ModelUtils.GetTransposedMatrix(ModelUtils.ConvertToUnlabeledDataset(dataset));
            mLabels = new ArrayList<LblT>();
            foreach (LabeledExample<LblT, SparseVector<double>> labeledExample in dataset)
            {
                mLabels.Add(labeledExample.Label);
            }
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, SparseVector<double>>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(SparseVector<double> example)
        {
            Utils.ThrowException(mDatasetMtx == null ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            ArrayList<KeyDat<double, LblT>> tmp = new ArrayList<KeyDat<double, LblT>>(mLabels.Count);
            double[] dotProdSimVec = ModelUtils.GetDotProductSimilarity(mDatasetMtx, mLabels.Count, example);
            for (int i = 0; i < mLabels.Count; i++)
            { 
                tmp.Add(new KeyDat<double, LblT>(dotProdSimVec[i], mLabels[i]));
            }
            tmp.Sort(DescSort<KeyDat<double, LblT>>.Instance);
            Dictionary<LblT, double> voting = new Dictionary<LblT, double>(mLblCmp);
            int n = Math.Min(mK, tmp.Count);
            double value;
            if (mSoftVoting) // "soft" voting
            {
                for (int i = 0; i < n; i++)
                {
                    KeyDat<double, LblT> item = tmp[i];
                    if (!voting.TryGetValue(item.Dat, out value))
                    {
                        voting.Add(item.Dat, item.Key);
                    }
                    else
                    {
                        voting[item.Dat] = value + item.Key;
                    }
                }
            }
            else // normal voting
            {
                for (int i = 0; i < n; i++)
                {
                    KeyDat<double, LblT> item = tmp[i];
                    if (!voting.TryGetValue(item.Dat, out value))
                    {
                        voting.Add(item.Dat, 1);
                    }
                    else
                    {
                        voting[item.Dat] = value + 1.0;
                    }
                }
            }
            Prediction<LblT> classifierResult = new Prediction<LblT>();
            foreach (KeyValuePair<LblT, double> item in voting)
            {
                classifierResult.Inner.Add(new KeyDat<double, LblT>(item.Value, item.Key));
            }
            classifierResult.Inner.Sort(DescSort<KeyDat<double, LblT>>.Instance);
            return classifierResult;
        }

        Prediction<LblT> IModel<LblT>.Predict(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is SparseVector<double>) ? new ArgumentTypeException("example") : null);
            return Predict((SparseVector<double>)example); // throws InvalidOperationException
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteObject(mDatasetMtx);
            writer.WriteObject(mLabels);
            writer.WriteInt(mK);
            writer.WriteBool(mSoftVoting);
            writer.WriteObject(mLblCmp);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mDatasetMtx = reader.ReadObject<SparseMatrix<double>>();
            mLabels = reader.ReadObject<ArrayList<LblT>>();         
            mK = reader.ReadInt();
            mSoftVoting = reader.ReadBool();
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
        }
    }
}
