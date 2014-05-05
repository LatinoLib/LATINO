/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    NearestCentroidClassifier.cs
 *  Desc:    Nearest centroid classifier
 *  Created: Aug-2007
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
       |  Class NearestCentroidClassifier<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class NearestCentroidClassifier<LblT> : IModel<LblT, SparseVector<double>>
    {
        private ArrayList<Pair<LblT, SparseVector<double>>> mCentroids
            = null;
        private IEqualityComparer<LblT> mLblCmp;
        private ISimilarity<SparseVector<double>> mSimilarity
            = CosineSimilarity.Instance;
        private bool mNormalize
            = false;

        public NearestCentroidClassifier(IEqualityComparer<LblT> lblCmp)
        {
            mLblCmp = lblCmp;
        }

        public NearestCentroidClassifier() : this((IEqualityComparer<LblT>)null)
        {
        }

        public NearestCentroidClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public bool NormalizeCentroids
        {
            get { return mNormalize; }
            set { mNormalize = value; }
        }

        public ISimilarity<SparseVector<double>> Similarity
        {
            get { return mSimilarity; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Similarity") : null);
                mSimilarity = value;
            }
        }

        public ArrayList<Pair<LblT, SparseVector<double>>> GetCentroids()
        {
            Utils.ThrowException(mCentroids == null ? new InvalidOperationException() : null);
            return mCentroids.DeepClone();
        }

        // *** IModel<LblT, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }

        public bool IsTrained
        {
            get { return mCentroids != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            mCentroids = new ArrayList<Pair<LblT, SparseVector<double>>>();
            Dictionary<LblT, ArrayList<SparseVector<double>>> tmp = new Dictionary<LblT, ArrayList<SparseVector<double>>>(mLblCmp);
            foreach (LabeledExample<LblT, SparseVector<double>> labeledExample in dataset)
            {
                if (!tmp.ContainsKey(labeledExample.Label))
                {
                    tmp.Add(labeledExample.Label, new ArrayList<SparseVector<double>>(new SparseVector<double>[] { labeledExample.Example }));
                }
                else
                {
                    tmp[labeledExample.Label].Add(labeledExample.Example);
                }
            }
            foreach (KeyValuePair<LblT, ArrayList<SparseVector<double>>> centroidData in tmp)
            {
                SparseVector<double> centroid = ModelUtils.ComputeCentroid(centroidData.Value, mNormalize ? CentroidType.NrmL2 : CentroidType.Avg);
                mCentroids.Add(new Pair<LblT, SparseVector<double>>(centroidData.Key, centroid));
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
            Utils.ThrowException(mCentroids == null ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Prediction<LblT> result = new Prediction<LblT>();
            foreach (Pair<LblT, SparseVector<double>> labeledCentroid in mCentroids)
            {
                double sim = mSimilarity.GetSimilarity(labeledCentroid.Second, example);
                result.Inner.Add(new KeyDat<double, LblT>(sim, labeledCentroid.First));
            }
            result.Inner.Sort(DescSort<KeyDat<double, LblT>>.Instance);
            return result;
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
            writer.WriteObject(mCentroids);
            writer.WriteObject(mSimilarity);
            writer.WriteBool(mNormalize);
            writer.WriteObject(mLblCmp);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mCentroids = reader.ReadObject<ArrayList<Pair<LblT, SparseVector<double>>>>();
            mSimilarity = reader.ReadObject<ISimilarity<SparseVector<double>>>();
            mNormalize = reader.ReadBool();
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
        }
    }
}
