/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SvmBinaryClassifier.cs
 *  Desc:    SVM Light LATINO wrapper (based on SvmLightLib)
 *  Created: Feb-2011
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
       |  Class SvmBinaryClassifier<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class SvmBinaryClassifier<LblT> : IModel<LblT, SparseVector<double>>, IDisposable
    {
        private int mModelId
            = -1;

        private ArrayList<LblT> mIdxToLbl 
            = new ArrayList<LblT>();
        private IEqualityComparer<LblT> mLblCmp
            = null;

        private SvmLightVerbosityLevel mVerbosityLevel
            = SvmLightVerbosityLevel.Info; // -v 1

        private double mC
            = 0; // default value ([avg. x*x]^-1)
        private bool mBiasedHyperplane
            = true; // -b 1

        private SvmLightKernelType mKernelType
            = SvmLightKernelType.Linear; // -t 0
        private double mKernelParamGamma
            = 1; // -g 1
        private double mKernelParamD
            = 1; // -d 1
        private double mKernelParamS
            = 1; // -s 1
        private double mKernelParamC
            = 0; // -r 0
        private bool mBiasedCostFunction
            = false; // -j

        private double mEps
            = 0.001; // -e 0.001
        private int mMaxIter
            = 100000; // -# 100000

        private string mCustomParams
            = null;

        private double[] mWeights
            = null;

        public SvmBinaryClassifier(IEqualityComparer<LblT> lblCmp)
        {
            mLblCmp = lblCmp;
        }

        public SvmBinaryClassifier() : this((IEqualityComparer<LblT>)null)
        { 
        }

        public SvmBinaryClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public SvmLightVerbosityLevel VerbosityLevel
        {
            get { return mVerbosityLevel; }
            set { mVerbosityLevel = value; }
        }

        public double C
        {
            get { return mC; }
            set 
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("C") : null);
                mC = value; 
            }
        }

        public bool BiasedHyperplane
        {
            get { return mBiasedHyperplane; }
            set { mBiasedHyperplane = value; }
        }

        public SvmLightKernelType KernelType
        {
            get { return mKernelType; }
            set 
            {
                Utils.ThrowException(mModelId != -1 ? new InvalidOperationException() : null);
                mKernelType = value; 
            }
        }

        public double KernelParamGamma
        {
            get { return mKernelParamGamma; }
            set 
            {
                Utils.ThrowException(mModelId != -1 ? new InvalidOperationException() : null);
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("KernelParamGamma") : null);
                mKernelParamGamma = value; 
            }
        }

        public double KernelParamD // range? 
        {
            get { return mKernelParamD; }
            set 
            {
                Utils.ThrowException(mModelId != -1 ? new InvalidOperationException() : null);    
                mKernelParamD = value; 
            } 
        }

        public double KernelParamS // range? 
        {
            get { return mKernelParamD; }
            set 
            {
                Utils.ThrowException(mModelId != -1 ? new InvalidOperationException() : null);    
                mKernelParamD = value; 
            } 
        }

        public double KernelParamC // range? 
        {
            get { return mKernelParamD; }
            set 
            {
                Utils.ThrowException(mModelId != -1 ? new InvalidOperationException() : null);    
                mKernelParamD = value; 
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

        public bool BiasedCostFunction
        {
            get { return mBiasedCostFunction; }
            set { mBiasedCostFunction = value; }
        }

        public int MaxIter
        {
            get { return mMaxIter; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("MaxIter") : null);
                mMaxIter = value;
            }
        }

        public string CustomParams
        {
            get { return mCustomParams; }
            set { mCustomParams = value; } // null is OK here
        }

        public double GetHyperplaneBias()
        {
            Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            return SvmLightLib.GetHyperplaneBias(mModelId);
        }

        public void LoadModel(string fileName)
        {
            Utils.ThrowException(typeof(LblT) != typeof(int) ? new InvalidOperationException() : null);
            Utils.ThrowException(!Utils.VerifyFileNameOpen(fileName) ? new ArgumentValueException("fileName") : null);
            Dispose();
            mIdxToLbl.Add((LblT)(object)1);
            mIdxToLbl.Add((LblT)(object)-1);
            mModelId = SvmLightLib.LoadModel(fileName);
        }

        public void SaveModel(string fileName)
        {
            Utils.ThrowException(!Utils.VerifyFileNameCreate(fileName) ? new ArgumentValueException("fileName") : null);
            Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            SvmLightLib.SaveModel(mModelId, fileName);
        }

        public ArrayList<IdxDat<double>> GetAlphas() // returns pairs (support vector index, alpha * y)
        {
            Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            ArrayList<IdxDat<double>> alphas = new ArrayList<IdxDat<double>>();
            for (int i = 0; i < SvmLightLib.GetSupportVectorCount(mModelId); i++)
            {
                double alpha = SvmLightLib.GetSupportVectorAlpha(mModelId, i);
                int idx = SvmLightLib.GetSupportVectorIndex(mModelId, i);
                alphas.Add(new IdxDat<double>(idx, alpha));
            }
            return alphas;
        }        

        public double[] GetLinearWeights()
        {
            Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            Utils.ThrowException(mKernelType != SvmLightKernelType.Linear ? new InvalidOperationException() : null);
            if (mWeights != null) { return mWeights; }
            int featureCount = SvmLightLib.GetFeatureCount(mModelId);
            double[] weights = new double[featureCount];
            for (int i = 0; i < featureCount; i++)
            {
                weights[i] = SvmLightLib.GetLinearWeight(mModelId, i);
            }
            mWeights = weights;
            return weights;
        }

        private SparseVector<double> GetSupportVector(int idx)
        {
            int featCount = SvmLightLib.GetSupportVectorFeatureCount(mModelId, idx);
            SparseVector<double> vec = new SparseVector<double>();
            for (int i = 0; i < featCount; i++)
            {
                vec.InnerIdx.Add(SvmLightLib.GetSupportVectorFeature(mModelId, idx, i));
                vec.InnerDat.Add(SvmLightLib.GetSupportVectorWeight(mModelId, idx, i));
            }
            return vec;
        }

        private double Mult(double[][] m1, double[][] m2, int row1, int col2)
        {
            double[] row = m1[row1];
            double val = 0;
            for (int i = 0; i < row.Length; i++)
            {
                val += row[i] * m2[i][col2];
            }
            return val;
        }

        private double[][] MatrixMultiply(double[][] m1, double[][] m2)
        {
            double[][] m = new double[m1.Length][];
            for (int row1 = 0; row1 < m1.Length; row1++)
            {
                double[] row = new double[m2[0].Length];
                for (int col2 = 0; col2 < m2[0].Length; col2++)
                {
                    row[col2] = Mult(m1, m2, row1, col2);
                }
                m[row1] = row;
            }
            return m;
        }

        private double ComputeCost(int rmvFeatIdx) // computes alphaT * H * alpha, where H = [yi * yj * K(xi, xj)]
        {             
            ArrayList<IdxDat<double>> alphas = GetAlphas();
            double[][] alphaT = new double[1][];
            double[][] alpha = new double[alphas.Count][];
            alphaT[0] = new double[alphas.Count];
            for (int i = 0; i < alphas.Count; i++) 
            {
                alpha[i] = new double[] { alphas[i].Dat };
                alphaT[0][i] = alphas[i].Dat; 
            }
            double[][] k = GetKernel(rmvFeatIdx);
            double[][] m = MatrixMultiply(MatrixMultiply(alphaT, k), alpha);
            return m[0][0];
        }

        public ArrayList<KeyDat<double, int>> RankFeatures() // Guyon et al. 2002
        {
            Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            ArrayList<KeyDat<double, int>> result = new ArrayList<KeyDat<double, int>>();
            if (mKernelType != SvmLightKernelType.Linear)
            {
                // any kernel
                int numFeat = SvmLightLib.GetFeatureCount(mModelId);
                double allFeat = 0.5 * ComputeCost(-1);
                for (int i = 0; i < numFeat; i++)
                {
                    //Console.WriteLine("{0} / {1}", i + 1, numFeat);
                    double featScore = Math.Abs(allFeat - 0.5 * ComputeCost(/*rmvFeatIdx=*/i));
                    result.Add(new KeyDat<double, int>(featScore, i));
                }
            }
            else
            {
                // linear kernel (fast)
                double[] w = GetLinearWeights();
                for (int i = 0; i < w.Length; i++)
                {
                    result.Add(new KeyDat<double, int>(0.5 * w[i] * w[i], i));
                }
            }
            result.Sort(DescSort<KeyDat<double, int>>.Instance);
            return result;
        }

        private double[][] GetKernel(int rmvFeatIdx)
        {
            int numSv = SvmLightLib.GetSupportVectorCount(mModelId);
            // initialize matrix
            double[][] kernel = new double[numSv][];
            // compute linear kernel
            SparseMatrix<double> m = new SparseMatrix<double>();
            for (int i = 0; i < numSv; i++)
            {
                SparseVector<double> sv = GetSupportVector(i);
                m[i] = sv;
            }
            if (rmvFeatIdx >= 0) { m.RemoveColAt(rmvFeatIdx); } 
            SparseMatrix<double> mTr = m.GetTransposedCopy();
            for (int i = 0; i < numSv; i++)
            {
                double[] innerProd = ModelUtils.GetDotProductSimilarity(mTr, numSv, m[i]);
                kernel[i] = innerProd;
            }
            // compute non-linear kernel
            switch (mKernelType)
            { 
                case SvmLightKernelType.Polynomial:
                    for (int row = 0; row < kernel.Length; row++)
                    {
                        for (int col = 0; col < kernel.Length; col++)
                        {
                            kernel[row][col] = Math.Pow(mKernelParamS * kernel[row][col] + mKernelParamC, mKernelParamD);
                        }
                    }
                    break;
                case SvmLightKernelType.RadialBasisFunction:
                    double[] diag = new double[kernel.Length];
                    for (int i = 0; i < kernel.Length; i++) { diag[i] = kernel[i][i]; } // save diagonal
                    for (int row = 0; row < kernel.Length; row++)
                    {
                        for (int col = 0; col < kernel.Length; col++)
                        {
                            kernel[row][col] = Math.Exp(-mKernelParamGamma * (diag[row] + diag[col] - 2.0 * kernel[row][col]));
                        }
                    }
                    break;
                case SvmLightKernelType.Sigmoid:
                    for (int row = 0; row < kernel.Length; row++)
                    {
                        for (int col = 0; col < kernel.Length; col++)
                        {
                            kernel[row][col] = Math.Tanh(mKernelParamS * kernel[row][col] + mKernelParamC);
                        }
                    }
                    break;
            }
            return kernel;
        }

        public int GetInternalClassLabel(LblT label)
        {
            Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            Utils.ThrowException(label == null ? new ArgumentNullException("label") : null);
            for (int i = 0; i < mIdxToLbl.Count; i++)
            {
                if ((mLblCmp != null && mLblCmp.Equals(mIdxToLbl[i], label)) || 
                    (mLblCmp == null && mIdxToLbl[i].Equals(label))) { return i == 0 ? 1 : -1; }
            }
            throw new ArgumentValueException("label");
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
            Dictionary<LblT, int> lblToIdx = new Dictionary<LblT, int>(mLblCmp);
            MultiSet<int> lblCount = new MultiSet<int>();
            int j = 0;
            foreach (LabeledExample<LblT, SparseVector<double>> lblEx in dataset)
            { 
                SparseVector<double> vec = lblEx.Example;
                int[] idx = new int[vec.Count];
                float[] val = new float[vec.Count];
                for (int i = 0; i < vec.Count; i++)
                {
                    idx[i] = vec.InnerIdx[i] + 1; 
                    val[i] = (float)vec.InnerDat[i]; // *** cast to float
                }
                int lbl;
                if (!lblToIdx.TryGetValue(lblEx.Label, out lbl))
                {
                    lblToIdx.Add(lblEx.Label, lbl = lblToIdx.Count);
                    mIdxToLbl.Add(lblEx.Label);
                }
                Utils.ThrowException(lbl == 2 ? new ArgumentValueException("dataset") : null);
                trainSet[j++] = SvmLightLib.NewFeatureVector(idx.Length, idx, val, lbl == 0 ? 1 : -1);
                lblCount.Add(lbl == 0 ? 1 : -1);
            }
            string costFactor = "";
            if (mBiasedCostFunction)
            {
                costFactor = "-j " + ((double)lblCount.GetCount(-1) / (double)lblCount.GetCount(1));                
            }
            mModelId = SvmLightLib.TrainModel(string.Format(CultureInfo.InvariantCulture, "-v {0} -c {1} -t {2} -g {3} -d {4} -s {5} -r {6} -b {7} -e {8} -# {9} {10} {11}", 
                (int)mVerbosityLevel, mC, (int)mKernelType, mKernelParamGamma, mKernelParamD, mKernelParamS, mKernelParamC, mBiasedHyperplane ? 1 : 0,
                mEps, mMaxIter, mCustomParams, costFactor), trainSet.Length, trainSet);
            // delete training vectors 
            foreach (int vecIdx in trainSet) { SvmLightLib.DeleteFeatureVector(vecIdx); }
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, SparseVector<double>>)dataset); // throws ArgumentValueException
        }

        public ArrayList<KeyDat<double, int>> Explain(SparseVector<double> example)
        {
            Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(mKernelType != SvmLightKernelType.Linear ? new InvalidOperationException() : null);
            double b = GetHyperplaneBias();
            double[] w = GetLinearWeights();
            ArrayList<KeyDat<double, int>> result = new ArrayList<KeyDat<double, int>>();
            foreach (IdxDat<double> xi in example) 
            {
                if (xi.Idx < w.Length)
                {
                    result.Add(new KeyDat<double, int>(xi.Dat * w[xi.Idx], xi.Idx));
                }
            }
            result.Add(new KeyDat<double, int>(-b, -1)); // bias
            result.Sort(DescSort<KeyDat<double, int>>.Instance);
            return result;
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
                val[i] = (float)example.InnerDat[i]; // *** cast to float
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
                mIdxToLbl.Clear();
                mModelId = -1;
                mWeights = null;
            }
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteInt((int)mVerbosityLevel);
            writer.WriteDouble(mC);
            writer.WriteBool(mBiasedHyperplane);
            writer.WriteInt((int)mKernelType);
            writer.WriteDouble(mKernelParamGamma);
            writer.WriteDouble(mKernelParamD);
            writer.WriteDouble(mKernelParamS);
            writer.WriteDouble(mKernelParamC);
            writer.WriteBool(mBiasedCostFunction);
            writer.WriteString(mCustomParams);
            writer.WriteDouble(mEps);
            writer.WriteInt(mMaxIter);
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
            mVerbosityLevel = (SvmLightVerbosityLevel)reader.ReadInt();
            mC = reader.ReadDouble();
            mBiasedHyperplane = reader.ReadBool();
            mKernelType = (SvmLightKernelType)reader.ReadInt();
            mKernelParamGamma = reader.ReadDouble();
            mKernelParamD = reader.ReadDouble();
            mKernelParamS = reader.ReadDouble();
            mKernelParamC = reader.ReadDouble();
            mBiasedCostFunction = reader.ReadBool();
            mCustomParams = reader.ReadString();
            mEps = reader.ReadDouble();
            mMaxIter = reader.ReadInt();
            mIdxToLbl.Load(reader);
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
