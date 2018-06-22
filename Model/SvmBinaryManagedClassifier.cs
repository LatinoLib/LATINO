/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SvmBinaryManagedClassifier.cs
 *  Desc:    Fully-native code for executing SvmLight models
 *  Created: Jun-2018
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class SvmBinaryManagedClassifier<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class SvmBinaryManagedClassifier<LblT> : IModel<LblT, SparseVector<double>>
    {
        private ArrayList<LblT> mIdxToLbl 
            = new ArrayList<LblT>();
        private IEqualityComparer<LblT> mLblCmp;

        private double mC;
        private bool mBiasedHyperplane;

        private SvmLightKernelType mKernelType;
        private double mKernelParamGamma;
        private double mKernelParamD;
        private double mKernelParamS;
        private double mKernelParamC;
        private bool mBiasedCostFunction;

        private double mEps;
        private int mMaxIter;

        private string mCustomParams;

        private double mBias;
        private double[] mLinearWeights;

        public SvmBinaryManagedClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public double C
        {
            get { return mC; }
        }

        public bool BiasedHyperplane
        {
            get { return mBiasedHyperplane; }
        }

        public SvmLightKernelType KernelType
        {
            get { return mKernelType; }
        }

        public double KernelParamGamma
        {
            get { return mKernelParamGamma; }
        }

        public double KernelParamD 
        {
            get { return mKernelParamD; }
        }

        public double KernelParamS 
        {
            get { return mKernelParamD; }
        }

        public double KernelParamC 
        {
            get { return mKernelParamD; }
        }

        public double Eps
        {
            get { return mEps; }
        }

        public bool BiasedCostFunction
        {
            get { return mBiasedCostFunction; }
        }

        public int MaxIter
        {
            get { return mMaxIter; }
        }

        public string CustomParams
        {
            get { return mCustomParams; }
        }

        public double Bias
        {
            get { return mBias; }
        }

        //public double GetHyperplaneBias()
        //{
        //    Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
        //    return SvmLightLib.GetHyperplaneBias(mModelId);
        //}

        public int GetInternalClassLabel(LblT label)
        {
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
            get { return true; } // the only way to create an instance is to load a trained model
        }

        public void Train(ILabeledExampleCollection<LblT, SparseVector<double>> dataset)
        {
            throw new NotImplementedException();
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            throw new NotImplementedException();
        }

        public Prediction<LblT> Predict(SparseVector<double> example)
        {
            // TODO: native code
            //Utils.ThrowException(mModelId == -1 ? new InvalidOperationException() : null);
            //Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            //Prediction<LblT> result = new Prediction<LblT>();
            //int[] idx = new int[example.Count];
            //float[] val = new float[example.Count];
            //for (int i = 0; i < example.Count; i++)
            //{
            //    idx[i] = example.InnerIdx[i] + 1; 
            //    val[i] = (float)example.InnerDat[i]; // *** cast to float
            //}
            //int vecId = SvmLightLib.NewFeatureVector(idx.Length, idx, val, 0);
            //SvmLightLib.Classify(mModelId, 1, new int[] { vecId });
            //double score = SvmLightLib.GetFeatureVectorClassifScore(vecId, 0);
            //LblT lbl = mIdxToLbl[score > 0 ? 0 : 1];
            //LblT otherLbl = mIdxToLbl[score > 0 ? 1 : 0];
            //result.Inner.Add(new KeyDat<double, LblT>(Math.Abs(score), lbl));
            //result.Inner.Add(new KeyDat<double, LblT>(-Math.Abs(score), otherLbl));
            //SvmLightLib.DeleteFeatureVector(vecId); // delete feature vector
            //return result;
            return null;
        }

        Prediction<LblT> IModel<LblT>.Predict(object example)
        {
            return Predict((SparseVector<double>)example); 
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            throw new NotImplementedException();
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            reader.ReadInt(); // verbosity level (ignore)
            mC = reader.ReadDouble();
            mBiasedHyperplane = reader.ReadBool();
            mKernelType = (SvmLightKernelType)reader.ReadInt();
            if (mKernelType != SvmLightKernelType.Linear)
            {
                throw new Exception("You are trying to load a non-linear model. This implementation does not support non-linear models.");
            }
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
            if (!reader.ReadBool())
            {
                throw new Exception("The model that you are trying to load has not been trained.");
            }
            // load SvmLight model
            int verLen = reader.ReadInt(); // int: version specifier length
            reader.ReadBytes(verLen); // byte[]: version specifier
            reader.ReadInt(); // long: kernel type
            reader.ReadInt(); // long: poly degree
            reader.ReadDouble(); // double: RBF gamma
            reader.ReadDouble(); // double: "coef lin"
            reader.ReadDouble(); // double: "coef const"
            reader.ReadBytes(50); // byte[50]: custom
            int totalWords = reader.ReadInt(); // long: total words
            mLinearWeights = new double[totalWords];
            reader.ReadInt(); // long: total docs
            int numSupVec = reader.ReadInt(); // int: num support vectors
            mBias = reader.ReadDouble(); // double: hyperplane bias 
            for (int i = 0; i < numSupVec - 1; i++)
            {
                double alpha = reader.ReadDouble(); // double: alpha
                int numFeatures = reader.ReadInt(); // int: number of features
                for (int j = 0; j < numFeatures; j++)
                {
                    int fnum = reader.ReadInt(); // int32: feature number (1-based)
                    float fval = reader.ReadFloat(); // float: feature value
                    mLinearWeights[fnum - 1] += alpha * fval;
                }
                int commentLen = reader.ReadInt(); // int: comment len       
                reader.ReadBytes(commentLen); // byte[]: comment
            }

            using (StreamWriter w = new StreamWriter(@"C:\Work\ViewGenerator\ViewGenerator\svmtest\ntive.txt"))
            {
                w.WriteLine(mBias);
                foreach (double val in mLinearWeights)
                {
                    w.WriteLine(val);
                }
            }

        }
    }
}
//void add_weight_vector_to_linear_model(MODEL* model)
///* compute weight vector in linear case and add to model */
//{
//    long i;
//    SVECTOR* f;

//    model->lin_weights = create_nvector(model->totwords);
//    clear_nvector(model->lin_weights, model->totwords);
//    for (i = 1; i < model->sv_num; i++)
//    {
//        for (f = (model->supvec[i])->fvec; f; f = f->next)
//            add_vector_ns(model->lin_weights, f, f->factor * model->alpha[i]);        // f->factor = 1.0
//    }
//}
//void add_vector_ns(double* vec_n, SVECTOR* vec_s, double faktor)
//{
//    /* Note: SVECTOR lists are not followed, but only the first
//             SVECTOR is used */
//    register WORD *ai;
//    ai = vec_s->words;
//    while (ai->wnum)
//    {
//        vec_n[ai->wnum] += (faktor * (double)ai->weight);
//        ai++;
//    }
//}