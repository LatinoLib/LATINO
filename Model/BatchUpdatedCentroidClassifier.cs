/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    BatchUpdatedCentroidClassifier.cs
 *  Desc:    Batch-updated centroid classifier 
 *  Created: May-2009
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
       |  Class BatchUpdatedCentroidClassifier<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public class BatchUpdateCentroidClassifier<LblT> : IModel<LblT, SparseVector<double>>
    {
        private IEqualityComparer<LblT> mLblCmp;
        private int mIterations
            = 20;
        private double mDamping
            = 0.8;
        private bool mPositiveValuesOnly
            = false;
        private SparseMatrix<double> mCentroidMtxTr
            = null;
        private ArrayList<LblT> mLabels
            = null;

        private Logger mLogger
            = Logger.GetLogger(typeof(BatchUpdateCentroidClassifier<LblT>));

        public BatchUpdateCentroidClassifier(IEqualityComparer<LblT> lblCmp)
        {
            mLblCmp = lblCmp;
        }

        public BatchUpdateCentroidClassifier() : this((IEqualityComparer<LblT>)null)
        {        
        }

        public BatchUpdateCentroidClassifier(BinarySerializer reader)
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

        public ArrayList<Pair<LblT, SparseVector<double>>> GetCentroids()
        {
            Utils.ThrowException(mCentroidMtxTr == null ? new InvalidOperationException() : null);
            SparseMatrix<double> cenMtx = mCentroidMtxTr.GetTransposedCopy();
            ArrayList<Pair<LblT, SparseVector<double>>> centroids = new ArrayList<Pair<LblT, SparseVector<double>>>();
            for (int i = 0; i < mLabels.Count; i++)
            {
                centroids.Add(new Pair<LblT, SparseVector<double>>(mLabels[i], cenMtx[i].Clone()));
            }
            return centroids;
        }

        // *** IModel<LblT, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }

        public bool IsTrained
        {
            get { return mCentroidMtxTr != null; }
        }

        public void Train(ILabeledExampleCollection<LblT, SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            Dictionary<LblT, CentroidData> centroids = new Dictionary<LblT, CentroidData>(mLblCmp);
            foreach (LabeledExample<LblT, SparseVector<double>> labeledExample in dataset)
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
                mLogger.Info("Train", "Iteration {0} / {1} ...", iter, mIterations);
                // compute dot products
                mLogger.Info("Train", "Computing dot products ...");
                int j = 0;
                foreach (KeyValuePair<LblT, CentroidData> labeledCentroid in centroids)
                {
                    mLogger.ProgressNormal(Logger.Level.Info, /*sender=*/this, "Train", "Centroid {0} / {1} ...", j + 1, centroids.Count);
                    SparseVector<double> cenVec = labeledCentroid.Value.GetSparseVector();
                    dotProd[j] = ModelUtils.GetDotProductSimilarity(dsMtx, dataset.Count, cenVec); 
                    j++;
                }
                // classify training examples
                mLogger.Info("Train", "Classifying training examples ...");
                int errCount = 0;
                for (int instIdx = 0; instIdx < dataset.Count; instIdx++)
                {
                    mLogger.ProgressFast(Logger.Level.Info, /*sender=*/this, "Train", "Example {0} / {1} ...", instIdx + 1, dataset.Count); 
                    double maxSim = double.MinValue;
                    CentroidData assignedCentroid = null;
                    CentroidData actualCentroid = null;
                    LabeledExample<LblT, SparseVector<double>> labeledExample = dataset[instIdx];
                    SparseVector<double> vec = labeledExample.Example;
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
                mLogger.Info("Train", "Training set error rate: {0:0.00}%", (double)errCount / (double)dataset.Count * 100.0);
                // update centroids
                int k = 0;
                foreach (CentroidData centroidData in centroids.Values)
                {
                    mLogger.ProgressNormal(Logger.Level.Info, /*sender=*/this, "Train", "Centroid {0} / {1} ...", ++k, centroids.Count);
                    centroidData.Update(mPositiveValuesOnly);
                    centroidData.UpdateCentroidLen();
                }
                learnRate *= mDamping;
            }
            mCentroidMtxTr = new SparseMatrix<double>();
            mLabels = new ArrayList<LblT>();
            int rowIdx = 0;
            foreach (KeyValuePair<LblT, CentroidData> labeledCentroid in centroids)
            {
                mCentroidMtxTr[rowIdx++] = labeledCentroid.Value.GetSparseVector();
                mLabels.Add(labeledCentroid.Key);
            }
            mCentroidMtxTr = mCentroidMtxTr.GetTransposedCopy();            
        }

        void IModel<LblT>.Train(ILabeledExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<LblT, SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<LblT, SparseVector<double>>)dataset); // throws ArgumentValueException
        }

        public Prediction<LblT> Predict(SparseVector<double> example)
        {
            Utils.ThrowException(mCentroidMtxTr == null ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Prediction<LblT> result = new Prediction<LblT>();
            double[] dotProdSimVec = ModelUtils.GetDotProductSimilarity(mCentroidMtxTr, mLabels.Count, example);
            for (int i = 0; i < dotProdSimVec.Length; i++)
            {
                result.Inner.Add(new KeyDat<double, LblT>(dotProdSimVec[i], mLabels[i]));
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
            writer.WriteObject(mCentroidMtxTr);
            writer.WriteObject(mLabels);
            writer.WriteInt(mIterations);
            writer.WriteDouble(mDamping);
            writer.WriteBool(mPositiveValuesOnly);
            writer.WriteObject(mLblCmp);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions            
            mCentroidMtxTr = reader.ReadObject<SparseMatrix<double>>();
            mLabels = reader.ReadObject<ArrayList<LblT>>();
            mIterations = reader.ReadInt();
            mDamping = reader.ReadDouble();
            mPositiveValuesOnly = reader.ReadBool();
            mLblCmp = reader.ReadObject<IEqualityComparer<LblT>>();
        }
    }
}
