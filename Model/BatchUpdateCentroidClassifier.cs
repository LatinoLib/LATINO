/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          BatchUpdateCentroidClassifier.cs
 *  Version:       1.0
 *  Desc:		   Batch-update centroid classifier 
 *  Author:        Miha Grcar
 *  Created on:    May-2009
 *  Last modified: Dec-2009
 *  Revision:      Dec-2009
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
        private Dictionary<LblT, CentroidData> mCentroids
            = null;
        private IEqualityComparer<LblT> mLblCmp
            = null;
        private int mIterations
            = 20;
        private double mDamping
            = 0.8;
        private bool mPositiveValuesOnly
            = false;

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

        public ArrayList<SparseVector<double>> GetCentroids(IEnumerable<LblT> labels)
        {
            Utils.ThrowException(mCentroids == null ? new InvalidOperationException() : null);
            Utils.ThrowException(labels == null ? new ArgumentNullException("labels") : null);
            ArrayList<SparseVector<double>> centroids = new ArrayList<SparseVector<double>>();
            foreach (LblT label in labels)
            {
                CentroidData data = mCentroids[label]; // throws ArgumentNullException, KeyNotFoundException
                centroids.Add(data.GetCentroid());
            }
            return centroids;
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
            mCentroids = new Dictionary<LblT, CentroidData>();
            foreach (LabeledExample<LblT, SparseVector<double>.ReadOnly> labeledExample in dataset)
            {
                if (!mCentroids.ContainsKey(labeledExample.Label))
                {
                    CentroidData centroidData = new CentroidData();
                    centroidData.AddToSum(labeledExample.Example);
                    mCentroids.Add(labeledExample.Label, centroidData);
                }
                else
                {
                    CentroidData centroidData = mCentroids[labeledExample.Label];
                    centroidData.AddToSum(labeledExample.Example);
                }               
            }
            foreach (CentroidData vecData in mCentroids.Values)
            {
                vecData.UpdateCentroidLen();
            }
            double learnRate = 1;
            for (int iter = 1; iter <= mIterations; iter++)
            {
                Utils.VerboseLine("Iteration {0} / {1} ...", iter, mIterations);
                // classify training documents
                int i = 0;
                int numMiscfy = 0;
                foreach (LabeledExample<LblT, SparseVector<double>.ReadOnly> labeledExample in dataset)
                {
                    Utils.Verbose("\rExample {0} / {1} ...", ++i, dataset.Count);
                    double maxSim = double.MinValue;
                    CentroidData assignedCentroid = null;
                    CentroidData actualCentroid = null;
                    SparseVector<double>.ReadOnly vec = labeledExample.Example;
                    foreach (KeyValuePair<LblT, CentroidData> labeledCentroid in mCentroids)
                    {                        
                        double sim = labeledCentroid.Value.GetDotProduct(vec);
                        if (sim > maxSim) { maxSim = sim; assignedCentroid = labeledCentroid.Value; }
                        if (labeledCentroid.Key.Equals(labeledExample.Label)) { actualCentroid = labeledCentroid.Value; }
                    }                        
                    if (assignedCentroid != actualCentroid)
                    {                        
                        assignedCentroid.AddToDiff(-learnRate, vec);
                        actualCentroid.AddToDiff(learnRate, vec);
                        numMiscfy++;
                    }                        
                }
                Utils.VerboseLine();
                Utils.VerboseLine("Training set error rate: {0:0.00}%", (double)numMiscfy / (double)dataset.Count * 100.0);
                // update centroids
                i = 0;
                foreach (CentroidData centroidData in mCentroids.Values)
                {
                    Utils.Verbose("\rCentroid {0} / {1} ...", ++i, mCentroids.Count);
                    centroidData.UpdateCentroid(mPositiveValuesOnly);
                    centroidData.UpdateCentroidLen();
                }
                Utils.VerboseLine();
                learnRate *= mDamping;
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
            foreach (KeyValuePair<LblT, CentroidData> labeledCentroid in mCentroids)
            {
                double sim = labeledCentroid.Value.GetDotProduct(example);
                result.Items.Add(new KeyDat<double, LblT>(sim, labeledCentroid.Key));
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
            writer.WriteBool(mCentroids != null);
            if (mCentroids != null) 
            { 
                Utils.SaveDictionary(mCentroids, writer); 
            }
            writer.WriteInt(mIterations);
            writer.WriteDouble(mDamping);
            writer.WriteBool(mPositiveValuesOnly);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions            
            mCentroids = reader.ReadBool() ? Utils.LoadDictionary<LblT, CentroidData>(reader) : null;
            mIterations = reader.ReadInt();
            mDamping = reader.ReadDouble();
            mPositiveValuesOnly = reader.ReadBool();
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class CentroidData
           |
           '-----------------------------------------------------------------------
        */
        // *** would it make sense to use Centroid<LblT> within/instead of CentroidData?
        private class CentroidData : ISerializable
        {
            public Dictionary<int, double> CentroidSum
                = new Dictionary<int, double>();
            public Dictionary<int, double> CentroidDiff
                = new Dictionary<int, double>();
            public double CentroidLen
                = -1;

            public CentroidData()
            { 
            }

            public CentroidData(BinarySerializer reader)
            {
                Load(reader); 
            }

            public void AddToSum(SparseVector<double>.ReadOnly vec)
            {
                foreach (IdxDat<double> item in vec)
                {
                    if (CentroidSum.ContainsKey(item.Idx))
                    {
                        CentroidSum[item.Idx] += item.Dat;
                    }
                    else
                    {
                        CentroidSum.Add(item.Idx, item.Dat);
                    }
                }
            }

            public void AddToDiff(double mult, SparseVector<double>.ReadOnly vec)
            {                
                foreach (IdxDat<double> item in vec)
                {
                    if (CentroidDiff.ContainsKey(item.Idx))
                    {
                        CentroidDiff[item.Idx] += item.Dat * mult;
                    }
                    else
                    {
                        CentroidDiff.Add(item.Idx, item.Dat * mult);
                    }
                }
            }

            public void UpdateCentroid(bool positiveValuesOnly)
            {
                foreach (KeyValuePair<int, double> item in CentroidDiff)
                {
                    if (CentroidSum.ContainsKey(item.Key))
                    {
                        CentroidSum[item.Key] += item.Value;
                    }
                    else 
                    {
                        CentroidSum.Add(item.Key, item.Value);
                    }
                }
                CentroidDiff.Clear();
                if (positiveValuesOnly)
                {
                    Dictionary<int, double> tmp = new Dictionary<int, double>();
                    foreach (KeyValuePair<int, double> item in CentroidSum)
                    {
                        if (item.Value > 0)
                        {
                            tmp.Add(item.Key, item.Value);
                        }
                    }
                    CentroidSum = tmp;
                }
            }

            public void UpdateCentroidLen()
            {
                CentroidLen = 0;
                foreach (double val in CentroidSum.Values)
                {
                    CentroidLen += val * val;
                }
                //Utils.VerboseLine(CentroidLen); // *** CentroidLen overflow?
                CentroidLen = Math.Sqrt(CentroidLen);
            }

            public double GetDotProduct(SparseVector<double>.ReadOnly vec)
            {
                double result = 0;
                foreach (IdxDat<double> item in vec)
                {
                    if (CentroidSum.ContainsKey(item.Idx))
                    {
                        result += item.Dat * CentroidSum[item.Idx]; 
                    }
                }
                return result / CentroidLen;
            }

            public SparseVector<double> GetCentroid()
            {
                SparseVector<double> vec = new SparseVector<double>();
                foreach (KeyValuePair<int, double> item in CentroidSum)
                {
                    vec.InnerIdx.Add(item.Key);
                    vec.InnerDat.Add(item.Value / CentroidLen);
                }
                vec.Sort();
                return vec;
            }

            // *** ISerializable interface implementation ***

            public void Save(BinarySerializer writer)
            {
                Utils.SaveDictionary(CentroidSum, writer);
                writer.WriteDouble(CentroidLen);
            }

            public void Load(BinarySerializer reader)
            {
                CentroidSum = Utils.LoadDictionary<int, double>(reader);
                CentroidLen = reader.ReadDouble();
            }
        }
    }
}
