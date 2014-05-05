/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IncrementalKMeans.cs 
 *  Desc:    Incremental k-means clustering algorithm
 *  Created: Aug-2009
 *
 *  Author:  Miha Grcar 
 * 
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Text;
using Latino.TextMining;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class IncrementalKMeansClustering
       |
       '-----------------------------------------------------------------------
    */
    public class IncrementalKMeansClustering : KMeansClusteringFast, ISerializable
    {
        private UnlabeledDataset<SparseVector<double>> mDataset
            = null;
        private double mQualThresh
            = 0.2;
        private long mTopicId
            = 0;

        private IncrementalBowSpace mBowSpc // *** for debugging
            = null;
        
        public IncrementalKMeansClustering(int initK) : base(initK) // throws ArgumentOutOfRangeException
        {
            mLogger = Logger.GetInstanceLogger(typeof(IncrementalKMeansClustering));
        }

        public IncrementalKMeansClustering() : this(/*initK=*/1)
        {
        }

        public IncrementalKMeansClustering(BinarySerializer reader) : this(/*initK=*/1)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public IncrementalBowSpace BowSpace
        {
            get { return mBowSpc; }
            set { mBowSpc = value; }
        }

        public double QualThresh
        {
            get { return mQualThresh; }
            set 
            {
                Utils.ThrowException(value <= 0 || value > 1 ? new ArgumentOutOfRangeException("QualThresh") : null);
                mQualThresh = value;
            }
        }

        public override ClusteringResult Cluster(IUnlabeledExampleCollection<SparseVector<double>> batch)
        {
            return Cluster(/*numOutdated=*/0, batch); // throws ArgumentNullException, ArgumentValueException
        }

        private void GetMostSimilarClusters(out int idx1, out int idx2)
        {
            double maxSim = 0;
            idx1 = 0;
            idx2 = 1;
            for (int i1 = 0; i1 < mCentroids.Count; i1++)
            {
                for (int i2 = i1 + 1; i2 < mCentroids.Count; i2++)
                {
                    CentroidData c1 = mCentroids[i1];
                    CentroidData c2 = mCentroids[i2];
                    double sim = c1.GetDotProduct(c2.GetSparseVector());
                    if (sim > maxSim)
                    {
                        maxSim = sim;
                        idx1 = i1;
                        idx2 = i2;
                    }
                }
            }
        }

        private void OutputState()
        {
            if (mLogger.ActiveLevel > Logger.Level.Trace) { return; }
            StringBuilder str = new StringBuilder();
            str.AppendLine();
            str.AppendLine("************************** State **************************");
            if (mCentroids != null)
            {
                str.AppendLine(string.Format("mCentroids.Count = {0}", mCentroids.Count));
                str.AppendLine(string.Format("mDataset.Count = {0}", mDataset.Count));
                int SumCurrentItemsCount = 0;
                int SumItemsCount = 0;
                int i = 1;
                foreach (CentroidData centroid in mCentroids)
                {
                    str.AppendLine(string.Format("Centroid {0}", i));
                    str.AppendLine(string.Format("  CurrentItems.Count = {0}", centroid.CurrentItems.Count));
                    str.AppendLine(string.Format("  Items.Count = {0}", centroid.Items.Count));
                    if (mBowSpc != null)
                    {
                        str.AppendLine(string.Format("  Keywords = {0}", mBowSpc.GetKeywordsStr(centroid.GetSparseVector(), /*n=*/5)));
                    }
                    str.AppendLine(string.Format("  Tag = {0}", centroid.Tag));
                    SumCurrentItemsCount += centroid.CurrentItems.Count;
                    SumItemsCount += centroid.Items.Count;
                    i++;
                }
                str.AppendLine(string.Format("Sum CurrentItems.Count = {0}", SumCurrentItemsCount));
                str.AppendLine(string.Format("Sum Items.Count = {0}", SumItemsCount));
            }
            else
            {
                str.AppendLine("NULL");
            }
            str.Append(new string('*', 59));
            mLogger.Trace("OutputState", str.ToString());
        }

        public ClusteringResult Cluster(int numOutdated, IUnlabeledExampleCollection<SparseVector<double>> batch)
        {
            Utils.ThrowException(batch == null ? new ArgumentNullException("batch") : null);
            Utils.ThrowException(numOutdated < 0 ? new ArgumentOutOfRangeException("numOutdated") : null);
            if (mDataset == null)
            {
                // initialize
                mLogger.Trace("Cluster", "Initializing ...");
                Utils.ThrowException(numOutdated > 0 ? new ArgumentOutOfRangeException("numOutdated") : null);
                //Utils.ThrowException(batch.Count == 0 ? new ArgumentValueException("batch") : null);
                if (batch.Count == 0) { return new ClusteringResult(); }
                kMeans(batch, Math.Min(mK, batch.Count));
                mDataset = new UnlabeledDataset<SparseVector<double>>(batch);
                foreach (CentroidData centroid in mCentroids) { centroid.Tag = mTopicId++; }
                //OutputState();
            }
            else
            {
                // update clusters
                Utils.ThrowException(numOutdated > mDataset.Count ? new ArgumentOutOfRangeException("numOutdated") : null);
                if (numOutdated == 0 && batch.Count == 0) { return GetClusteringResult(); }
                mLogger.Trace("Cluster", "Updating clusters ...");
                // assign new instances                    
                double dummy;
                Assign(mCentroids, ModelUtils.GetTransposedMatrix(batch), batch.Count, /*offs=*/mDataset.Count, out dummy);
                mDataset.AddRange(batch);
                // remove outdated instances
                foreach (CentroidData centroid in mCentroids)
                {
                    foreach (int item in centroid.CurrentItems)
                    {
                        if (item >= numOutdated) { centroid.Items.Add(item); }
                    }
                    centroid.Update(mDataset);
                    centroid.UpdateCentroidLen();
                }
                mDataset.RemoveRange(0, numOutdated);
                ArrayList<CentroidData> centroidsNew = new ArrayList<CentroidData>(mCentroids.Count);
                foreach (CentroidData centroid in mCentroids)
                {
                    if (centroid.CurrentItems.Count > 0)
                    {
                        centroidsNew.Add(centroid);
                        Set<int> tmp = new Set<int>();
                        foreach (int idx in centroid.CurrentItems) { tmp.Add(idx - numOutdated); }
                        centroid.CurrentItems.Inner.SetItems(tmp);
                    }
                }
                if (centroidsNew.Count == 0) // reset
                {
                    mCentroids = null;
                    mDataset = null;
                    return new ClusteringResult();
                }
                mCentroids = centroidsNew;
                // execute main loop
                kMeansMainLoop(mDataset, mCentroids);
                //OutputState();
            }
            // adjust k
            double minQual; // *** not used at the moment
            int minQualIdx;
            double qual = GetClustQual(out minQual, out minQualIdx);
            if (qual < mQualThresh)
            {
                while (qual < mQualThresh) // split cluster at minQualIdx
                {
                    mLogger.Trace("Cluster", "Increasing k to {0} ...", mCentroids.Count + 1);
                    mCentroids.Add(mCentroids[minQualIdx].Clone());
                    mCentroids.Last.Tag = mTopicId++;
                    kMeansMainLoop(mDataset, mCentroids);                    
                    if (mCentroids.Last.CurrentItems.Count > mCentroids[minQualIdx].CurrentItems.Count)
                    {
                        // swap topic identifiers
                        object tmp = mCentroids.Last.Tag; 
                        mCentroids.Last.Tag = mCentroids[minQualIdx].Tag; 
                        mCentroids[minQualIdx].Tag = tmp;
                    }
                    qual = GetClustQual(out minQual, out minQualIdx);
                    //OutputState();
                }
            }
            else if (numOutdated > 0)
            {                
                while (qual > mQualThresh && mCentroids.Count > 1) // join clusters 
                {
                    mLogger.Trace("Cluster", "Decreasing k to {0} ...", mCentroids.Count - 1);
                    ArrayList<CentroidData> centroidsCopy = mCentroids.DeepClone();
                    if (mCentroids.Count == 2) // create single cluster
                    {
                        object topicId = mCentroids[0].CurrentItems.Count > mCentroids[1].CurrentItems.Count ? mCentroids[0].Tag : mCentroids[1].Tag;
                        mCentroids = new ArrayList<CentroidData>();
                        mCentroids.Add(new CentroidData());
                        for (int i = 0; i < mDataset.Count; i++) { mCentroids.Last.Items.Add(i); }
                        mCentroids.Last.Tag = topicId;
                        mCentroids.Last.Update(mDataset);
                        mCentroids.Last.UpdateCentroidLen();
                    }
                    else
                    {
                        int idx1, idx2;
                        GetMostSimilarClusters(out idx1, out idx2);
                        CentroidData c1 = mCentroids[idx1];
                        CentroidData c2 = mCentroids[idx2];
                        object topicId = c1.CurrentItems.Count > c2.CurrentItems.Count ? c1.Tag : c2.Tag;
                        mCentroids.RemoveAt(idx2);
                        c1.Items.AddRange(c1.CurrentItems);
                        c1.Items.AddRange(c2.CurrentItems);
                        c1.Tag = topicId;
                        c1.Update(mDataset);
                        c1.UpdateCentroidLen();
                        kMeansMainLoop(mDataset, mCentroids);                        
                    }
                    qual = GetClustQual();
                    if (qual >= mQualThresh) 
                    { 
                        mLogger.Trace("Cluster", "Accepted solution at k = {0}.", mCentroids.Count); 
                    }
                    else 
                    { 
                        mCentroids = centroidsCopy; 
                    }
                    //OutputState();
                }
            }
            OutputState();
            return GetClusteringResult();
        }

        private void kMeansMainLoop(IUnlabeledExampleCollection<SparseVector<double>> dataset, ArrayList<CentroidData> centroids)
        {
            double dummy;
            kMeansMainLoop(dataset, centroids, out dummy);
        }

        private double GetClustQual()
        {
            double foo;
            int bar;
            return GetClustQual(out foo, out bar);
        }

        private double GetClustQual(out double minQual, out int minQualIdx)
        {
            double clustQual = 0;
            double[] partQual = new double[mCentroids.Count];
            for (int i = 0; i < mCentroids.Count; i++)
            {
                foreach (int itemIdx in mCentroids[i].CurrentItems)
                {
                    double sim = mCentroids[i].GetDotProduct(mDataset[itemIdx]);
                    partQual[i] += sim;
                    clustQual += sim;
                }
            }
            minQual = 1;
            minQualIdx = 0;
            for (int i = 0; i < mCentroids.Count; i++)
            {
                partQual[i] /= (double)mCentroids[i].CurrentItems.Count;
                if (partQual[i] <= minQual) { minQual = partQual[i]; minQualIdx = i; }
            }
            clustQual /= (double)mDataset.Count;
            return clustQual;
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            writer.WriteDotNetObject(mRnd);
            writer.WriteDouble(mEps);
            writer.WriteInt(mTrials);
            writer.WriteInt(mK);
            writer.WriteObject(mCentroids);
            writer.WriteObject(mDataset);
            writer.WriteDouble(mQualThresh);
            writer.WriteLong(mTopicId);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions 
            mRnd = (Random)reader.ReadDotNetObject();
            mEps = reader.ReadDouble();
            mTrials = reader.ReadInt();
            mK = reader.ReadInt();
            mCentroids = reader.ReadObject<ArrayList<CentroidData>>();
            mDataset = reader.ReadObject<UnlabeledDataset<SparseVector<double>>>();
            mQualThresh = reader.ReadDouble();
            mTopicId = reader.ReadLong();
        }
    }
}