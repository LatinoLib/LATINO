/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    NaiveBayesClassifier.cs
 *  Desc:    Naive Bayes classifier 
 *  Created: Jul-2010
 *
 *  Authors: Miha Grcar
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

        private static Logger mLogger
            = Logger.GetLogger(typeof(NaiveBayesClassifier<LblT>));

        private static Dictionary<int, double>[] PrecomputeProbabilities(ILabeledExampleCollection<LblT, BinaryVector> dataset, 
            out LblT[] idxToLbl, out Dictionary<int, double> featurePriors, out int[] exampleCount)
        {
            featurePriors = new Dictionary<int, double>();
            ArrayList<LblT> tmp = new ArrayList<LblT>();
            Dictionary<LblT, int> lblToIdx = new Dictionary<LblT, int>();
            foreach (LabeledExample<LblT, BinaryVector> labeledExample in dataset)
            {
                if (!lblToIdx.ContainsKey(labeledExample.Label))
                {
                    lblToIdx.Add(labeledExample.Label, lblToIdx.Count);
                    tmp.Add(labeledExample.Label);
                }
            }
            // prepare counters
            exampleCount = new int[tmp.Count];
            Dictionary<int, double>[] perClassFeatureProb = new Dictionary<int, double>[tmp.Count];
            for (int j = 0; j < perClassFeatureProb.Length; j++) { perClassFeatureProb[j] = new Dictionary<int, double>(); }
            Dictionary<int, int> featureCounter = new Dictionary<int, int>();
            // count features
            int i = 0;
            object id = new object();
            foreach (LabeledExample<LblT, BinaryVector> labeledExample in dataset)
            {
                mLogger.ProgressFast(id, "PrecomputeProbabilities", "{0} / {1}", ++i, dataset.Count);
                int lblIdx = lblToIdx[labeledExample.Label];
                exampleCount[lblIdx]++;
                double val;
                int valInt;
                foreach (int idx in labeledExample.Example)
                {
                    if (featureCounter.TryGetValue(idx, out valInt))
                    {
                        featureCounter[idx] = valInt + 1;
                    }
                    else
                    {
                        featureCounter.Add(idx, 1);
                    }
                    if (perClassFeatureProb[lblIdx].TryGetValue(idx, out val))
                    {
                        perClassFeatureProb[lblIdx][idx] = val + 1;
                    }
                    else
                    {
                        perClassFeatureProb[lblIdx].Add(idx, 1);
                    }
                }
            }
            // estimate probabilities
            i = 0;
            foreach (Dictionary<int, double> probVec in perClassFeatureProb)
            {
                foreach (int featIdx in new ArrayList<int>(probVec.Keys))
                {
                    double p0 = ((double)featureCounter[featIdx] + 1.0) / ((double)dataset.Count + 2.0); // rule of succession (feature prior)
                    double p = (probVec[featIdx] + 2.0 * p0) / ((double)exampleCount[i] + 2.0); // m-estimate (m = 2)
                    probVec[featIdx] = p;
                    if (!featurePriors.ContainsKey(featIdx)) { featurePriors.Add(featIdx, p0); }
                }
                i++;
            }
            idxToLbl = tmp.ToArray();
            return perClassFeatureProb;
        }

        public bool Normalize
        {
            get { return mNormalize; }
            set { mNormalize = value; }
        }

        // *** IModel<LblT,ReadOnly> interface implementation ***

        public void Train(ILabeledExampleCollection<LblT, BinaryVector> dataset)
        {
            mFeatureProb = PrecomputeProbabilities(dataset, out mIdxToLbl, out mFeaturePriors, out mExampleCount);
            mDatasetCount = dataset.Count;
        }

        public Prediction<LblT> Predict(BinaryVector example)
        {
            Prediction<LblT> pred = new Prediction<LblT>();
            double sum = 0;
            for (int i = 0; i < mIdxToLbl.Length; i++)
            {
                //Console.WriteLine("Predicting for {0}", mIdxToLbl[i]);
                double pc = ((double)mExampleCount[i] + 2.0 / (double)mIdxToLbl.Length) / ((double)mDatasetCount + 2.0); // class prior probability estimate
                foreach (int featIdx in example)
                {
                    double pFeat;
                    if (mFeatureProb[i].TryGetValue(featIdx, out pFeat))
                    {
                        pc *= pFeat;
                    }
                    else if (mFeaturePriors.TryGetValue(featIdx, out pFeat))
                    {
                        pc *= 2.0 * pFeat / ((double)mExampleCount[i] + 2.0);
                    }
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

        public Type RequiredExampleType
        {
            get { return typeof(BinaryVector); }
        }

        public bool IsTrained
        {
            get { return mFeatureProb != null; }
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, BinaryVector>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, BinaryVector>)dataset); // throws ... ?
        }

        Prediction<LblT> IModel<LblT>.Predict(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is BinaryVector) ? new ArgumentTypeException("example") : null);
            return Predict((BinaryVector)example); // throws ... ?
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            // ...
        }
    }
}