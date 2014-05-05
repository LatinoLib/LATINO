/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    AgglomerativeKMeansClusteringFast.cs
 *  Desc:    Combination of k-means and agglomerative clustering (fast)
 *  Created: Jul-2012
 *
 *  Author:  Miha Grcar 
 * 
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.Diagnostics;
using Latino.TextMining;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class AgglomerativeKMeansClusteringFast
       |
       '-----------------------------------------------------------------------
    */
    public class AgglomerativeKMeansClusteringFast : IClustering<SparseVector<double>>
    {
        private KMeansClusteringFast mKMeansClustering
            = new KMeansClusteringFast(/*k=*/20);

        public AgglomerativeKMeansClusteringFast()
        {
            mKMeansClustering.Logger = Logger.GetLogger(typeof(AgglomerativeKMeansClusteringFast));
        }

        public Logger Logger
        {
            get { return mKMeansClustering.Logger; }
            set { mKMeansClustering.Logger = value; } // throws ArgumentNullException
        }

        public Random Random
        {
            get { return mKMeansClustering.Random; }
            set { mKMeansClustering.Random = value; } // throws ArgumentNullException
        }

        public double Eps
        {
            get { return mKMeansClustering.Eps; }
            set { mKMeansClustering.Eps = value; } // throws ArgumentOutOfRangeException
        }

        public int Trials
        {
            get { return mKMeansClustering.Trials; }
            set { mKMeansClustering.Trials = value; } // throws ArgumentOutOfRangeException
        }

        public int NumLeaves
        {
            get { return mKMeansClustering.K; }
            set { mKMeansClustering = new KMeansClusteringFast(value); } // throws ArgumentOutOfRangeException
        }

        private void Update(SparseMatrix<double> simMtx, SparseMatrix<double> clustMtxTr, int numClusters, int idx1, int idx2, ArrayList<Cluster> clusters,
            IUnlabeledExampleCollection<SparseVector<double>> dataset, double damping)
        {
            Debug.Assert(idx1 < idx2);          
            // create new parent
            Cluster c1 = clusters[idx1];
            Cluster c2 = clusters[idx2];
            Cluster parent = new Cluster();
            parent.Items.AddRange(c1.Items);
            parent.Items.AddRange(c2.Items);
            parent.ClusterInfo = Math.Max((int)c1.ClusterInfo, (int)c2.ClusterInfo) + 1;
            c1.Parent = parent;
            c2.Parent = parent;
            parent.AddChild(c1);
            parent.AddChild(c2);
            SparseVector<double> centroid = ModelUtils.ComputeCentroid(parent.Items, dataset, CentroidType.NrmL2);
            centroid = Trim(centroid, 1000, 0.8);                
            // remove clusters
            clusters.RemoveAt(idx2);
            clusters.RemoveAt(idx1);
            // add new parent
            clusters.Add(parent);
            // remove rows at idx1 and idx2
            simMtx.PurgeRowAt(idx2);
            simMtx.PurgeRowAt(idx1);
            // remove cols at idx1 and idx2
            simMtx.PurgeColAt(idx2);
            simMtx.PurgeColAt(idx1);
            clustMtxTr.PurgeColAt(idx2);
            clustMtxTr.PurgeColAt(idx1);
            // update matrices
            numClusters -= 2;
            foreach (IdxDat<double> item in centroid)
            {
                if (clustMtxTr[item.Idx] == null)
                {
                    clustMtxTr[item.Idx] = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(numClusters, item.Dat) });
                }
                else
                {
                    clustMtxTr[item.Idx].InnerIdx.Add(numClusters);
                    clustMtxTr[item.Idx].InnerDat.Add(item.Dat);
                }
            }
            double[] simVec = ModelUtils.GetDotProductSimilarity(clustMtxTr, numClusters + 1, centroid);
            for (int i = 0; i < simVec.Length; i++)
            { 
                simVec[i] *= Math.Pow(damping, (double)((int)parent.ClusterInfo + (int)clusters[i].ClusterInfo) / 2.0);
            }
            SparseMatrix<double> col = new SparseMatrix<double>();
            col[0] = new SparseVector<double>(simVec);
            simMtx.AppendCols(col.GetTransposedCopy(), numClusters);
        }

        private void FindMaxSim(SparseMatrix<double> simMtx, out int idx1, out int idx2)
        {
            double max = double.MinValue;
            idx1 = idx2 = -1;
            foreach (IdxDat<SparseVector<double>> row in simMtx)
            {
                for (int i = 1; i < row.Dat.Count; i++)
                {
                    if (row.Dat.InnerDat[i] > max)
                    {
                        max = row.Dat.InnerDat[i];
                        idx1 = row.Idx;
                        idx2 = row.Dat.InnerIdx[i];
                    }
                }
            }
        }

        // *** IClustering<LblT, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }
        
        public SparseVector<double> Trim(SparseVector<double> vec, int size, double cutPerc)
        {
            SparseVector<double> trimmed = vec;
            if (vec.Count > size)
            {
                ArrayList<KeyDat<double, int>> tmp = new ArrayList<KeyDat<double, int>>(vec.Count);
                foreach (IdxDat<double> item in vec) { tmp.Add(new KeyDat<double, int>(item.Dat, item.Idx)); }
                tmp.Sort(DescSort<KeyDat<double, int>>.Instance);
                tmp.RemoveRange(size, tmp.Count - size);
                ArrayList<IdxDat<double>> tmp2 = new ArrayList<IdxDat<double>>();
                foreach (KeyDat<double, int> item in tmp) { tmp2.Add(new IdxDat<double>(item.Dat, item.Key)); }
                tmp2.Sort();
                trimmed = new SparseVector<double>(tmp2);
            }
            ModelUtils.CutLowWeights(ref trimmed, cutPerc);
            ModelUtils.TryNrmVecL2(trimmed);
            return trimmed;
        }

        public ClusteringResult Cluster(IUnlabeledExampleCollection<SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count < NumLeaves ? new ArgumentValueException("dataset") : null);
            ClusteringResult clusters = mKMeansClustering.Cluster(dataset);            
            UnlabeledDataset<SparseVector<double>> centroids = new UnlabeledDataset<SparseVector<double>>();
            foreach (Cluster cluster in clusters.Roots) 
            {
                SparseVector<double> centroid = ModelUtils.ComputeCentroid(cluster.Items, dataset, CentroidType.NrmL2);
                centroids.Add(centroid);
                centroid = Trim(centroid, 1000, 0.8);
                cluster.ClusterInfo = 1; // cluster level
            }            
            SparseMatrix<double> simMtx = ModelUtils.GetDotProductSimilarity(centroids, /*thresh=*/0, /*fullMatrix=*/false);
            SparseMatrix<double> clustMtxTr = ModelUtils.GetTransposedMatrix(centroids);
            int iter = 1;
            while (clusters.Roots.Count > 1)
            {
                Console.WriteLine("Iteration {0} ...", iter++);
                int idx1, idx2;
                FindMaxSim(simMtx, out idx1, out idx2);
                Update(simMtx, clustMtxTr, clusters.Roots.Count, idx1, idx2, clusters.Roots.Inner, dataset, /*damping=*/0.9);
                Console.WriteLine(simMtx.ToString("E0.00"));
                Console.WriteLine();
            }
            return clusters;
        }

        ClusteringResult IClustering.Cluster(IUnlabeledExampleCollection dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            return Cluster((IUnlabeledExampleCollection<SparseVector<double>>)dataset); // throws ArgumentValueException
        }
    }
}
