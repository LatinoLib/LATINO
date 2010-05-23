/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    CentroidClassifier.cs
 *  Desc:    Centroid classifier
 *  Created: Aug-2007
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
       |  Class CentroidClassifier<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class CentroidClassifier<LblT> : IModel<LblT, SparseVector<double>.ReadOnly>
    {
        private ArrayList<Pair<LblT, SparseVector<double>.ReadOnly>> mCentroids
            = null;
        private IEqualityComparer<LblT> mLblCmp
            = null;
        private ISimilarity<SparseVector<double>.ReadOnly> mSimilarity
            = CosineSimilarity.Instance;
        private bool mNormalize
            = false;

        public CentroidClassifier()
        {
        }

        public CentroidClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public bool NormalizeCentroids
        {
            get { return mNormalize; }
            set { mNormalize = value; }
        }

        public IEqualityComparer<LblT> LabelEqualityComparer
        {
            get { return mLblCmp; }
            set { mLblCmp = value; }
        }

        public ISimilarity<SparseVector<double>.ReadOnly> Similarity
        {
            get { return mSimilarity; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Similarity") : null);
                mSimilarity = value;
            }
        }

        public ArrayList<SparseVector<double>.ReadOnly> GetCentroids(IEnumerable<LblT> labels)
        {
            Utils.ThrowException(mCentroids == null ? new InvalidOperationException() : null);
            Utils.ThrowException(labels == null ? new ArgumentNullException("labels") : null);
            Dictionary<LblT, SparseVector<double>.ReadOnly> aux = new Dictionary<LblT, SparseVector<double>.ReadOnly>(mLblCmp); 
            ArrayList<SparseVector<double>.ReadOnly> list = new ArrayList<SparseVector<double>.ReadOnly>();
            foreach (Pair<LblT, SparseVector<double>.ReadOnly> centroid in mCentroids)
            {
                aux.Add(centroid.First, centroid.Second);
            }
            foreach (LblT label in labels)
            {
                list.Add(aux[label]); // throws ArgumentNullException, KeyNotFoundException
            }
            return list;
        }

        // *** IModel<LblT, SparseVector<double>.ReadOnly> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>.ReadOnly); }
        }

        public bool IsTrained
        {
            get { return mCentroids != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, SparseVector<double>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            mCentroids = new ArrayList<Pair<LblT, SparseVector<double>.ReadOnly>>();
            Dictionary<LblT, ArrayList<SparseVector<double>.ReadOnly>> tmp = new Dictionary<LblT, ArrayList<SparseVector<double>.ReadOnly>>(mLblCmp);
            foreach (LabeledExample<LblT, SparseVector<double>.ReadOnly> labeledExample in dataset)
            {
                if (!tmp.ContainsKey(labeledExample.Label))
                {
                    tmp.Add(labeledExample.Label, new ArrayList<SparseVector<double>.ReadOnly>(new SparseVector<double>.ReadOnly[] { labeledExample.Example }));
                }
                else
                {
                    tmp[labeledExample.Label].Add(labeledExample.Example);
                }
            }
            foreach (KeyValuePair<LblT, ArrayList<SparseVector<double>.ReadOnly>> centroidData in tmp)
            {
                SparseVector<double> centroid = ModelUtils.ComputeCentroid(centroidData.Value, mNormalize ? CentroidType.NrmL2 : CentroidType.Avg);
                mCentroids.Add(new Pair<LblT, SparseVector<double>.ReadOnly>(centroidData.Key, centroid));
            }
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, SparseVector<double>.ReadOnly>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, SparseVector<double>.ReadOnly>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(SparseVector<double>.ReadOnly example)
        {
            Utils.ThrowException(mCentroids == null ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Prediction<LblT> result = new Prediction<LblT>();
            foreach (Pair<LblT, SparseVector<double>.ReadOnly> labeledCentroid in mCentroids)
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
            Utils.ThrowException(!(example is SparseVector<double>.ReadOnly) ? new ArgumentTypeException("example") : null);
            return Predict((SparseVector<double>.ReadOnly)example); // throws InvalidOperationException
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
            mCentroids = reader.ReadObject<ArrayList<Pair<LblT, SparseVector<double>.ReadOnly>>>();
            mSimilarity = reader.ReadObject<ISimilarity<SparseVector<double>.ReadOnly>>();
            mNormalize = reader.ReadBool();
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
        }
    }
}
