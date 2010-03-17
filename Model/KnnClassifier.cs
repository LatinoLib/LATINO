/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          KnnClassifier.cs
 *  Version:       1.0
 *  Desc:		   K-nearest neighbors classifier 
 *  Author:        Miha Grcar
 *  Created on:    Aug-2007
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
       |  Class KnnClassifier<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public class KnnClassifier<LblT, ExT> : IModel<LblT, ExT>
    {
        private ArrayList<LabeledExample<LblT, ExT>> mExamples
            = null;
        private IEqualityComparer<LblT> mLblCmp
            = null;
        private ISimilarity<ExT> mSimilarity
            = null;
        private int mK
            = 10;
        private bool mSoftVoting
            = true;

        public KnnClassifier(ISimilarity<ExT> similarity)
        {
            Similarity = similarity; // throws ArgumentNullException
        }

        public KnnClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public IEqualityComparer<LblT> LabelEqualityComparer
        {
            get { return mLblCmp; }
            set { mLblCmp = value; }
        }

        public ISimilarity<ExT> Similarity
        {
            get { return mSimilarity; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Similarity") : null);
                mSimilarity = value;
            }
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

        // *** IModel<LblT, ExT> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(ExT); }
        }

        public bool IsTrained
        {
            get { return mExamples != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, ExT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            mExamples = new ArrayList<LabeledExample<LblT, ExT>>(dataset);
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, ExT>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, ExT>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(ExT example)
        {
            Utils.ThrowException((mExamples == null || mSimilarity == null) ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            ArrayList<KeyDat<double, LabeledExample<LblT, ExT>>> tmp = new ArrayList<KeyDat<double, LabeledExample<LblT, ExT>>>(mExamples.Count);
            foreach (LabeledExample<LblT, ExT> labeledExample in mExamples)
            {
                double sim = mSimilarity.GetSimilarity(example, labeledExample.Example);
                tmp.Add(new KeyDat<double, LabeledExample<LblT, ExT>>(sim, labeledExample));
            }
            tmp.Sort(new DescSort<KeyDat<double, LabeledExample<LblT, ExT>>>());
            Dictionary<LblT, double> voting = new Dictionary<LblT, double>(mLblCmp);
            int n = Math.Min(mK, tmp.Count);
            double value;
            if (mSoftVoting) // "soft" voting
            {
                for (int i = 0; i < n; i++)
                {
                    KeyDat<double, LabeledExample<LblT, ExT>> item = tmp[i];
                    if (!voting.TryGetValue(item.Dat.Label, out value))
                    {
                        voting.Add(item.Dat.Label, item.Key);
                    }
                    else
                    {
                        voting[item.Dat.Label] = value + item.Key;
                    }
                }
            }
            else // normal voting
            {
                for (int i = 0; i < n; i++)
                {
                    KeyDat<double, LabeledExample<LblT, ExT>> item = tmp[i];
                    if (!voting.TryGetValue(item.Dat.Label, out value))
                    {
                        voting.Add(item.Dat.Label, 1);
                    }
                    else
                    {
                        voting[item.Dat.Label] = value + 1.0;
                    }
                }
            }
            Prediction<LblT> classifierResult = new Prediction<LblT>();
            foreach (KeyValuePair<LblT, double> item in voting)
            {
                classifierResult.Items.Add(new KeyDat<double, LblT>(item.Value, item.Key));
            }
            classifierResult.Items.Sort(new DescSort<KeyDat<double, LblT>>());
            return classifierResult;
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
            mExamples.Save(writer);
            writer.WriteObject<ISimilarity<ExT>>(mSimilarity);
            writer.WriteInt(mK);
            writer.WriteBool(mSoftVoting);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mExamples = new ArrayList<LabeledExample<LblT, ExT>>(reader);
            mSimilarity = reader.ReadObject<ISimilarity<ExT>>();
            mK = reader.ReadInt();
            mSoftVoting = reader.ReadBool();
        }
    }
}
