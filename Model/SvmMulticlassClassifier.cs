/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SvmMulticlassFast.cs
 *  Desc:    Multiclass SVM classifier based on SvmLightLib 
 *  Created: Jan-2011
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class SvmMulticlassFast<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class SvmMulticlassClassifier<LblT> : IModel<LblT, SparseVector<double>>, IDisposable
    {
        private Dictionary<LblT, int> mLblToId // 1-based
            = new Dictionary<LblT, int>();
        private ArrayList<LblT> mIdxToLbl // 0-based
            = new ArrayList<LblT>();
        private IEqualityComparer<LblT> mLblCmp
            = null;
        private int mModelId
            = -1;
        private double mC
            = 5000;
        private double mEps
            = 0.1;

        public SvmMulticlassClassifier()
        {
        }

        public SvmMulticlassClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public IEqualityComparer<LblT> LabelEqualityComparer
        {
            get { return mLblCmp; }
            set { mLblCmp = value; }
        }

        public double C
        {
            get { return mC; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("C") : null);
                mC = value;
            }
        }

        public double Eps
        {
            get { return mEps; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("Eps") : null);
                mEps = value;
            }
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
                    idx[i] = vec.InnerIdx[i] + 1; // *** indices are 1-based in SvmLightLib
                    val[i] = (float)vec.InnerDat[i]; // *** loss of precision (double -> float)
                }
                int lbl;
                if (!mLblToId.TryGetValue(lblEx.Label, out lbl))
                {
                    mLblToId.Add(lblEx.Label, lbl = mLblToId.Count + 1); // *** labels start with 1 in SvmLightLib
                    mIdxToLbl.Add(lblEx.Label);
                }
                trainSet[j++] = SvmLightLib.NewFeatureVector(idx.Length, idx, val, lbl);
            }
            mModelId = SvmLightLib.TrainMulticlassModel(string.Format("-c {0} -e {1}", mC.ToString(CultureInfo.InvariantCulture), mEps.ToString(CultureInfo.InvariantCulture)), 
                trainSet.Length, trainSet);
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
                idx[i] = example.InnerIdx[i] + 1; // *** indices are 1-based in SvmLightLib
                val[i] = (float)example.InnerDat[i]; // *** loss of precision (double -> float)
            }
            int vecId = SvmLightLib.NewFeatureVector(idx.Length, idx, val, 0);
            SvmLightLib.MulticlassClassify(mModelId, 1, new int[] { vecId });
            int n = SvmLightLib.GetFeatureVectorClassifScoreCount(vecId);
            for (int i = 0; i < n; i++)
            {
                double score = SvmLightLib.GetFeatureVectorClassifScore(vecId, i);
                LblT lbl = mIdxToLbl[i];
                result.Inner.Add(new KeyDat<double, LblT>(score, lbl));
            }
            result.Inner.Sort(DescSort<KeyDat<double, LblT>>.Instance);
            result.Trim();
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
                SvmLightLib.DeleteMulticlassModel(mModelId);
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
            writer.WriteDouble(mC);
            writer.WriteDouble(mEps);
            mIdxToLbl.Save(writer);
            writer.WriteObject(mLblCmp);
            writer.WriteBool(mModelId != -1);
            if (mModelId != -1)
            {
                SvmLightLib.WriteByteCallback wb = delegate(byte b) { writer.WriteByte(b); };
                SvmLightLib.SaveMulticlassModelBinCallback(mModelId, wb);
                GC.KeepAlive(wb);
            }
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            Dispose();
            // the following statements throw serialization-related exceptions
            mC = reader.ReadDouble();
            mEps = reader.ReadDouble();
            mIdxToLbl.Load(reader);
            for (int i = 0; i < mIdxToLbl.Count; i++)
            {
                mLblToId.Add(mIdxToLbl[i], i + 1);
            }
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
            if (reader.ReadBool())
            {
                SvmLightLib.ReadByteCallback rb = delegate() { return reader.ReadByte(); };
                mModelId = SvmLightLib.LoadMulticlassModelBinCallback(rb);
                GC.KeepAlive(rb);
            }
        }
    }
}
