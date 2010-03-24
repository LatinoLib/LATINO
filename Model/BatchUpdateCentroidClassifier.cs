/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          BatchUpdateCentroidClassifier.cs
 *  Version:       1.0
 *  Desc:		   Batch-update centroid classifier 
 *  Author:        Miha Grcar
 *  Created on:    May-2009
 *  Last modified: Mar-2010
 *  Revision:      Mar-2010
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class BatchUpdateCentroidClassifier<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class BatchUpdateCentroidClassifier<LblT> : IModel<LblT, SparseVector<double>.ReadOnly>
    {
        private IEqualityComparer<LblT> mLblCmp
            = null;
        private int mIterations
            = 20;
        private double mDamping
            = 0.8;
        private bool mPositiveValuesOnly
            = false;
        private SparseMatrix<double> mCentroidMtx
            = null;
        private ArrayList<LblT> mLabels
            = null;

        public BatchUpdateCentroidClassifier()
        {
        }

        public BatchUpdateCentroidClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public IEqualityComparer<LblT> LabelEqualityComparer
        {
            get { return mLblCmp; }
            set { mLblCmp = value; }
        }

        public int Iterations
        {
            get { return mIterations; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("Iterations") : null);
                mIterations = value;
            }
        }

        public double Damping
        {
            get { return mDamping; }
            set 
            {
                Utils.ThrowException((value <= 0.0 || value > 1.0) ? new ArgumentOutOfRangeException("Damping") : null);
                mDamping = value;
            }
        }

        public bool PositiveValuesOnly
        {
            get { return mPositiveValuesOnly; }
            set { mPositiveValuesOnly = value; }
        }

        // *** IModel<LblT, SparseVector<double>.ReadOnly> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>.ReadOnly); }
        }

        public bool IsTrained
        {
            get { return mCentroidMtx != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, SparseVector<double>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            Dictionary<LblT, CentroidData> centroids = new Dictionary<LblT, CentroidData>();
            foreach (LabeledExample<LblT, SparseVector<double>.ReadOnly> labeledExample in dataset)
            {
                if (!centroids.ContainsKey(labeledExample.Label))
                {
                    CentroidData centroidData = new CentroidData();
                    centroidData.AddToSum(labeledExample.Example);
                    centroids.Add(labeledExample.Label, centroidData);
                }
                else
                {
                    CentroidData centroidData = centroids[labeledExample.Label];
                    centroidData.AddToSum(labeledExample.Example);
                }               
            }
            foreach (CentroidData cenData in centroids.Values)
            {
                cenData.UpdateCentroidLen();
            }
            double learnRate = 1;
            double[][] dotProd = null; 
            SparseMatrix<double> dsMtx = null;
            if (mIterations > 0)
            {
                dotProd = new double[centroids.Count][];
                dsMtx = ModelUtils.GetTransposedMatrix(ModelUtils.ConvertToUnlabeledDataset(dataset));
            }
            for (int iter = 1; iter <= mIterations; iter++)
            {
                Utils.VerboseLine("Iteration {0} / {1} ...", iter, mIterations);
                // compute dot products
                Utils.VerboseLine("Computing dot products ...");
                int j = 0;
                foreach (KeyValuePair<LblT, CentroidData> labeledCentroid in centroids)
                {
                    Utils.VerboseProgress("Centroid {0} / {1} ...", j + 1, centroids.Count);
                    SparseVector<double> cenVec = labeledCentroid.Value.GetSparseVector();
                    double[] dotProdSimVec = ModelUtils.GetDotProductSimilarity(dsMtx, dataset.Count, cenVec);
                    dotProd[j] = dotProdSimVec;
                    j++;
                }
                // classify training examples
                Utils.VerboseLine("Classifying training examples ...");
                int errCount = 0;
                for (int instIdx = 0; instIdx < dataset.Count; instIdx++)
                {
                    Utils.VerboseProgress("Example {0} / {1} ...", instIdx + 1, dataset.Count); 
                    double maxSim = double.MinValue;
                    CentroidData assignedCentroid = null;
                    CentroidData actualCentroid = null;
                    LabeledExample<LblT, SparseVector<double>.ReadOnly> labeledExample = dataset[instIdx];
                    SparseVector<double>.ReadOnly vec = labeledExample.Example;
                    int cenIdx = 0;
                    foreach (KeyValuePair<LblT, CentroidData> labeledCentroid in centroids)
                    {                        
                        double sim = dotProd[cenIdx][instIdx];
                        if (sim > maxSim) { maxSim = sim; assignedCentroid = labeledCentroid.Value; }
                        if (labeledCentroid.Key.Equals(labeledExample.Label)) { actualCentroid = labeledCentroid.Value; }
                        cenIdx++;
                    }                        
                    if (assignedCentroid != actualCentroid)
                    {                        
                        assignedCentroid.AddToDiff(-learnRate, vec);
                        actualCentroid.AddToDiff(learnRate, vec);
                        errCount++;
                    }                        
                }
                Utils.VerboseLine("Training set error rate: {0:0.00}%", (double)errCount / (double)dataset.Count * 100.0);
                // update centroids
                int k = 0;
                foreach (CentroidData centroidData in centroids.Values)
                {
                    Utils.VerboseProgress("Centroid {0} / {1} ...", ++k, centroids.Count);
                    centroidData.Update(mPositiveValuesOnly);
                    centroidData.UpdateCentroidLen();
                }
                learnRate *= mDamping;
            }
            mCentroidMtx = new SparseMatrix<double>();
            mLabels = new ArrayList<LblT>();
            int rowIdx = 0;
            foreach (KeyValuePair<LblT, CentroidData> labeledCentroid in centroids)
            {
                mCentroidMtx[rowIdx++] = labeledCentroid.Value.GetSparseVector();
                mLabels.Add(labeledCentroid.Key);
            }
            mCentroidMtx = mCentroidMtx.GetTransposedCopy();            
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, SparseVector<double>.ReadOnly>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, SparseVector<double>.ReadOnly>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(SparseVector<double>.ReadOnly example)
        {
            Utils.ThrowException(mCentroidMtx == null ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Prediction<LblT> result = new Prediction<LblT>();
            double[] dotProdSimVec = ModelUtils.GetDotProductSimilarity(mCentroidMtx, mLabels.Count, example);
            for (int i = 0; i < dotProdSimVec.Length; i++)
            {
                result.Items.Add(new KeyDat<double, LblT>(dotProdSimVec[i], mLabels[i]));
            }
            result.Items.Sort(new DescSort<KeyDat<double, LblT>>());
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
            writer.WriteBool(mCentroidMtx != null);
            if (mCentroidMtx != null) 
            { 
                mCentroidMtx.Save(writer);
                mLabels.Save(writer);
            }
            writer.WriteInt(mIterations);
            writer.WriteDouble(mDamping);
            writer.WriteBool(mPositiveValuesOnly);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions            
            mCentroidMtx = null;
            mLabels = null;
            if (reader.ReadBool())
            {
                mCentroidMtx = new SparseMatrix<double>(reader);
                mLabels = new ArrayList<LblT>(reader);
            }
            mIterations = reader.ReadInt();
            mDamping = reader.ReadDouble();
            mPositiveValuesOnly = reader.ReadBool();
        }
    }
}
