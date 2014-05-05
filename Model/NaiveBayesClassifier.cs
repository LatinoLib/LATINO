/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    NaiveBayesClassifier.cs
 *  Desc:    Naive Bayes classifier 
 *  Created: Jul-2010
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
       |  Class NaiveBayesClassifier<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class NaiveBayesClassifier<LblT> : IModel<LblT, BinaryVector>
    {
        private Dictionary<int, double>[] mFeatureProb
            = null;
        private LblT[] mIdxToLbl
            = null;
        private Dictionary<int, double> mFeaturePriors
            = null;
        private int[] mExampleCount
            = null;
        private int mDatasetCount
            = -1;
        private bool mNormalize
            = false;
        private bool mLogSumExpTrick
            = true;
        private IEqualityComparer<LblT> mLblCmp;

        private Logger mLogger
            = Logger.GetLogger(typeof(NaiveBayesClassifier<LblT>));

        public NaiveBayesClassifier(IEqualityComparer<LblT> lblCmp)
        {
            mLblCmp = lblCmp;
        }

        public NaiveBayesClassifier() : this((IEqualityComparer<LblT>)null)
        { 
        }

        public NaiveBayesClassifier(BinarySerializer reader)
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

        private void PrecomputeProbabilities(ILabeledExampleCollection<LblT, BinaryVector> dataset)
        {
            mFeaturePriors = new Dictionary<int, double>();
            ArrayList<LblT> tmp = new ArrayList<LblT>();
            Dictionary<LblT, int> lblToIdx = new Dictionary<LblT, int>(mLblCmp);
            foreach (LabeledExample<LblT, BinaryVector> labeledExample in dataset)
            {
                if (!lblToIdx.ContainsKey(labeledExample.Label))
                {
                    lblToIdx.Add(labeledExample.Label, lblToIdx.Count);
                    tmp.Add(labeledExample.Label);
                }
            }
            // prepare counters
            mExampleCount = new int[tmp.Count];
            mFeatureProb = new Dictionary<int, double>[tmp.Count];
            for (int j = 0; j < mFeatureProb.Length; j++) { mFeatureProb[j] = new Dictionary<int, double>(); }
            MultiSet<int> featureCounter = new MultiSet<int>();
            // count features
            int i = 0;
            foreach (LabeledExample<LblT, BinaryVector> labeledExample in dataset)
            {
                mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "PrecomputeProbabilities", "Processing example {0} / {1}", ++i, dataset.Count);
                int lblIdx = lblToIdx[labeledExample.Label];
                mExampleCount[lblIdx]++;
                double val;
                foreach (int idx in labeledExample.Example)
                {
                    featureCounter.Add(idx);
                    if (mFeatureProb[lblIdx].TryGetValue(idx, out val))
                    {
                        mFeatureProb[lblIdx][idx] = val + 1;
                    }
                    else
                    {
                        mFeatureProb[lblIdx].Add(idx, 1);
                    }
                }
            }
            // estimate probabilities
            i = 0;
            foreach (Dictionary<int, double> probVec in mFeatureProb)
            {
                foreach (int featIdx in new ArrayList<int>(probVec.Keys))
                {
                    double p0 = ((double)featureCounter.GetCount(featIdx) + 1.0) / ((double)dataset.Count + 2.0); // rule of succession (feature prior)
                    double p = (probVec[featIdx] + 2.0 * p0) / ((double)mExampleCount[i] + 2.0); // m-estimate (m = 2)
                    probVec[featIdx] = p;
                    if (!mFeaturePriors.ContainsKey(featIdx)) { mFeaturePriors.Add(featIdx, p0); }
                }
                i++;
            }
            mIdxToLbl = tmp.ToArray();
        }

        public bool Normalize
        {
            get { return mNormalize; }
            set { mNormalize = value; }
        }

        public bool LogSumExpTrick
        {
            get { return mLogSumExpTrick; }
            set { mLogSumExpTrick = value; }
        }

        // *** IModel<LblT, ReadOnly> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(BinaryVector); }
        }

        public bool IsTrained
        {
            get { return mFeatureProb != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, BinaryVector> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            PrecomputeProbabilities(dataset);
            mDatasetCount = dataset.Count;
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, BinaryVector>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, BinaryVector>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(BinaryVector example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Prediction<LblT> pred = new Prediction<LblT>();
            double sum = 0;
            for (int i = 0; i < mIdxToLbl.Length; i++)
            {
                double pc;
                if (!mLogSumExpTrick)
                {
                    pc = ((double)mExampleCount[i] + 2.0 / (double)mIdxToLbl.Length) / ((double)mDatasetCount + 2.0); // class prior probability estimate
                    foreach (int featIdx in example)
                    {
                        double pFeat;
                        if (mFeatureProb[i].TryGetValue(featIdx, out pFeat))
                        {
                            pc *= pFeat;
                        }
                        else if (mFeaturePriors.TryGetValue(featIdx, out pFeat))
                        {
                            pc *= 2.0 * pFeat / ((double)mExampleCount[i] + 2.0); // m-estimate (m = 2, feature count = 0)
                        }
                    }
                }
                else // log-sum-exp trick (slower but prevents underflowing)
                {
                    pc = Math.Log(((double)mExampleCount[i] + 2.0 / (double)mIdxToLbl.Length) / ((double)mDatasetCount + 2.0));
                    foreach (int featIdx in example)
                    {
                        double pFeat;
                        if (mFeatureProb[i].TryGetValue(featIdx, out pFeat))
                        {
                            pc += Math.Log(pFeat);
                        }
                        else if (mFeaturePriors.TryGetValue(featIdx, out pFeat))
                        {
                            pc += Math.Log(2.0 * pFeat / ((double)mExampleCount[i] + 2.0));
                        }
                    }
                    pc = Math.Exp(pc);                    
                }
                pred.Inner.Add(new KeyDat<double, LblT>(pc, mIdxToLbl[i]));
                sum += pc;
            }
            if (mNormalize && sum > 0)
            {
                for (int i = 0; i < pred.Count; i++)
                {
                    KeyDat<double, LblT> score = pred[i];
                    pred.Inner[i] = new KeyDat<double, LblT>(score.Key / sum, score.Dat);
                }
            }
            pred.Inner.Sort(DescSort<KeyDat<double, LblT>>.Instance);
            return pred;
        }

        Prediction<LblT> IModel<LblT>.Predict(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is BinaryVector) ? new ArgumentTypeException("example") : null);
            return Predict((BinaryVector)example); 
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteBool(IsTrained);
            if (IsTrained)
            {
                writer.WriteInt(mFeatureProb.Length);
                foreach (Dictionary<int, double> dict in mFeatureProb) { Utils.SaveDictionary<int, double>(dict, writer); }
                writer.WriteInt(mIdxToLbl.Length);
                foreach (LblT lbl in mIdxToLbl) { writer.WriteObject(lbl); }
                Utils.SaveDictionary<int, double>(mFeaturePriors, writer);
                new ArrayList<int>(mExampleCount).Save(writer);
            }
            writer.WriteInt(mDatasetCount);
            writer.WriteBool(mNormalize);
            writer.WriteBool(mLogSumExpTrick);
            writer.WriteObject(mLblCmp);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            bool isTrained = reader.ReadBool();
            if (isTrained)
            {
                int len = reader.ReadInt();
                mFeatureProb = new Dictionary<int, double>[len];
                for (int i = 0; i < len; i++) { mFeatureProb[i] = Utils.LoadDictionary<int, double>(reader); }
                len = reader.ReadInt();
                mIdxToLbl = new LblT[len];
                for (int i = 0; i < len; i++) { mIdxToLbl[i] = reader.ReadObject<LblT>(); }
                mFeaturePriors = Utils.LoadDictionary<int, double>(reader);
                mExampleCount = new ArrayList<int>(reader).ToArray();
            }
            else
            {
                mFeatureProb = null;
                mIdxToLbl = null;
                mFeaturePriors = null;
                mExampleCount = null;
            }
            mDatasetCount = reader.ReadInt();
            mNormalize = reader.ReadBool();
            mLogSumExpTrick = reader.ReadBool();
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
        }
    }
}