/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    SvmBinary.cs
 *  Desc:    Binary SVM classifier based on SvmLightLib (modified SvmMulticlassFast.cs)
 *  Created: Feb-2011
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;

// TODO: throw exception if more than two classes are found in training set
// TODO: params (at least C)
// TODO: get rid of mLblCmp?

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class SvmBinary<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class SvmBinary<LblT> : IModel<LblT, SparseVector<double>>, IDisposable
    {
        private Dictionary<LblT, int> mLblToId 
            = new Dictionary<LblT, int>();
        private ArrayList<LblT> mIdxToLbl 
            = new ArrayList<LblT>();
        private IEqualityComparer<LblT> mLblCmp
            = null;
        private int mModelId
            = -1;

        public SvmBinary()
        {
        }

        public SvmBinary(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public IEqualityComparer<LblT> LabelEqualityComparer
        {
            get { return mLblCmp; }
            set { mLblCmp = value; }
        }

        // *** IModel<LblT, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }

        public bool IsTrained
        {
            get { return mModelId != -1; }
        }

        public void Train(ILabeledExampleCollection<LblT, SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            Dispose();
            int[] trainSet = new int[dataset.Count];
            int[] labels = new int[dataset.Count];
            int j = 0;
            foreach (LabeledExample<LblT, SparseVector<double>> lblEx in dataset)
            { 
                SparseVector<double> vec = lblEx.Example;
                int[] idx = new int[vec.Count];
                float[] val = new float[vec.Count];
                for (int i = 0; i < vec.Count; i++)
                {
                    idx[i] = vec.InnerIdx[i] + 1; 
                    val[i] = (float)vec.InnerDat[i]; // *** loss of precision (double -> float)
                }
                int lbl;
                if (!mLblToId.TryGetValue(lblEx.Label, out lbl))
                {
                    mLblToId.Add(lblEx.Label, lbl = mLblToId.Count + 1);
                    mIdxToLbl.Add(lblEx.Label);
                }
                trainSet[j++] = SvmLightLib.NewFeatureVector(idx.Length, idx, val, lbl == 2 ? -1 : 1);
            }
            mModelId = SvmLightLib.TrainModel(string.Format(""), trainSet.Length, trainSet);
            // delete training vectors 
            foreach (int vecIdx in trainSet) { SvmLightLib.DeleteFeatureVector(vecIdx); }
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, SparseVector<double>>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(SparseVector<double> example)
        {
            Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Prediction<LblT> result = new Prediction<LblT>();
            int[] idx = new int[example.Count];
            float[] val = new float[example.Count];
            for (int i = 0; i < example.Count; i++)
            {
                idx[i] = example.InnerIdx[i] + 1; 
                val[i] = (float)example.InnerDat[i]; // *** loss of precision (double -> float)
            }
            int vecId = SvmLightLib.NewFeatureVector(idx.Length, idx, val, 0);
            SvmLightLib.Classify(mModelId, 1, new int[] { vecId });
            double score = SvmLightLib.GetFeatureVectorClassifScore(vecId, 0);
            LblT lbl = mIdxToLbl[score > 0 ? 0 : 1];
            LblT otherLbl = mIdxToLbl[score > 0 ? 1 : 0];
            result.Inner.Add(new KeyDat<double, LblT>(Math.Abs(score), lbl));
            result.Inner.Add(new KeyDat<double, LblT>(-Math.Abs(score), otherLbl));
            SvmLightLib.DeleteFeatureVector(vecId); // delete feature vector
            return result;
        }

        Prediction<LblT> IModel<LblT>.Predict(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is SparseVector<double>) ? new ArgumentTypeException("example") : null);
            return Predict((SparseVector<double>)example); // throws InvalidOperationException
        }

        // *** IDisposable interface implementation ***

        public void Dispose()
        {
            if (mModelId != -1)
            {
                SvmLightLib.DeleteModel(mModelId);
                mLblToId.Clear();
                mIdxToLbl.Clear();
                mModelId = -1;
            }
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            //writer.WriteDouble(mC);
            //writer.WriteDouble(mEps);
            mIdxToLbl.Save(writer);
            writer.WriteObject(mLblCmp);
            writer.WriteBool(mModelId != -1);
            if (mModelId != -1)
            {
                SvmLightLib.WriteByteCallback wb = delegate(byte b) { writer.WriteByte(b); };
                SvmLightLib.SaveModelBinCallback(mModelId, wb);
                GC.KeepAlive(wb);
            }
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            Dispose();
            // the following statements throw serialization-related exceptions
            //mC = reader.ReadDouble();
            //mEps = reader.ReadDouble();
            mIdxToLbl.Load(reader);
            for (int i = 0; i < mIdxToLbl.Count; i++)
            {
                mLblToId.Add(mIdxToLbl[i], i + 1);
            }
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
            if (reader.ReadBool())
            {
                SvmLightLib.ReadByteCallback rb = delegate() { return reader.ReadByte(); };
                mModelId = SvmLightLib.LoadModelBinCallback(rb);
                GC.KeepAlive(rb);
            }
        }
    }
}
