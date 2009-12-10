/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          __IncrementalSemanticSpaceLayout__.cs
 *  Version:       1.0
 *  Desc:		   Semantic space layout algorithm (experimental)
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Latino.Model;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Class IncrementalSemanticSpaceLayout 
       |
       '-----------------------------------------------------------------------
    */
    public class IncrementalSemanticSpaceLayout : ILayoutAlgorithm
    {
        private IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> m_dataset;
        private Random m_random
            = new Random(1);
        private double m_k_means_eps
            = 0.01;
        private int m_k_clust
            = 100;
        private double m_sim_thresh
            = 0.005;
        private int m_k_nn
            = 10;
        private int m_k_nn_ext
            = 100;
        private IncrementalKMeans m_k_means
            = null;
        private Vector2D[] m_ref_pos
            = null;
        private ArrayList<Neighborhood> m_neighborhoods
            = null;

        public IncrementalSemanticSpaceLayout(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            m_dataset = dataset;
        }

        public Random Random
        {
            get { return m_random; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Random") : null);
                m_random = value; 
            }
        }

        public double KMeansEps
        {
            get { return m_k_means_eps; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("KMeansEps") : null);
                m_k_means_eps = value;
            }
        }

        public int KMeansK
        {
            get { return m_k_clust; }
            set
            {
                Utils.ThrowException(value < 2 ? new ArgumentOutOfRangeException("KMeansK") : null);
                m_k_clust = value;
            }
        }

        public double SimThresh
        {
            get { return m_sim_thresh; }
            set 
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("SimThresh") : null);
                m_sim_thresh = value;
            }
        }

        public int NeighborhoodSize
        {
            get { return m_k_nn; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("NeighborhoodSize") : null);
                m_k_nn = value;
            }
        }

        // *** ILayoutAlgorithm interface implementation ***

        public Vector2D[] ComputeLayout()
        {
            return ComputeLayout(/*settings=*/null);
        }

        public Vector2D[] ComputeLayout(LayoutSettings settings)
        {
            UnlabeledDataset<SparseVector<double>.ReadOnly> dataset = new UnlabeledDataset<SparseVector<double>.ReadOnly>(m_dataset);
            // clustering 
            Utils.VerboseLine("Clustering ...");
            m_k_means = new IncrementalKMeans(m_k_clust);
            m_k_means.Eps = m_k_means_eps;
            m_k_means.Random = m_random;
            m_k_means.Trials = 1;
            m_k_means.Cluster(m_dataset); // throws ArgumentValueException
            // determine reference instances
            UnlabeledDataset<SparseVector<double>.ReadOnly> ds_ref_inst = new UnlabeledDataset<SparseVector<double>.ReadOnly>();
            foreach (SparseVector<double> centroid in m_k_means.GetCentroids())
            {
                ds_ref_inst.Add(centroid); // dataset of reference instances
                dataset.Add(centroid); // add centroids to the main dataset
            }
            // position reference instances
            Utils.VerboseLine("Positioning reference instances ...");
            SparseMatrix<double> sim_mtx = ModelUtils.GetDotProductSimilarity(ds_ref_inst, m_sim_thresh, /*full_matrix=*/false);
            StressMajorizationLayout sm = new StressMajorizationLayout(ds_ref_inst.Count, new DistFunc(sim_mtx));
            sm.Random = m_random;
            m_ref_pos = sm.ComputeLayout();
            // k-NN
            Utils.VerboseLine("Computing similarities ...");
            sim_mtx = ModelUtils.GetDotProductSimilarity(dataset, m_sim_thresh, /*full_matrix=*/true);
            Utils.VerboseLine("Constructing system of linear equations ...");
            LabeledDataset<double, SparseVector<double>.ReadOnly> lsqr_ds = new LabeledDataset<double, SparseVector<double>.ReadOnly>();
            m_neighborhoods.Clear();
            foreach (IdxDat<SparseVector<double>> sim_mtx_row in sim_mtx)
            {
                if (sim_mtx_row.Dat.Count == 0)
                {
                    Utils.VerboseLine("*** Warning: instance #{0} has no neighborhood.", sim_mtx_row.Idx);
                }
                ArrayList<KeyDat<double, int>> knn = new ArrayList<KeyDat<double, int>>(sim_mtx_row.Dat.Count);
                foreach (IdxDat<double> item in sim_mtx_row.Dat)
                {
                    if (item.Idx != sim_mtx_row.Idx)
                    {
                        knn.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                    }
                }
                knn.Sort(new DescSort<KeyDat<double, int>>());
                int count = Math.Min(knn.Count, m_k_nn_ext);
                Neighborhood neighborhood = new Neighborhood();
                neighborhood.Neighbors = new ArrayList<KeyDat<double, int>>(count);
                for (int i = 0; i < count; i++)
                {
                    neighborhood.Neighbors.Add(knn[i]);                    
                }
                m_neighborhoods.Add(neighborhood);
                count = Math.Min(knn.Count, m_k_nn);
                SparseVector<double> eq = new SparseVector<double>();
                double wgt = 1.0 / (double)count;
                for (int i = 0; i < count; i++)
                {
                    eq.InnerIdx.Add(knn[i].Dat);
                    eq.InnerDat.Add(-wgt);
                }
                eq.InnerIdx.Sort(); // *** sort only indices
                eq[sim_mtx_row.Idx] = 1;
                lsqr_ds.Add(0, eq);
            }
            Vector2D[] layout = new Vector2D[dataset.Count - m_k_clust];
            for (int i = dataset.Count - m_k_clust, j = 0; i < dataset.Count; i++, j++)
            {
                SparseVector<double> eq = new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(i, 1) });
                lsqr_ds.Add(m_ref_pos[j].X, eq);
            }
            LSqrModel lsqr = new LSqrModel();
            lsqr.Train(lsqr_ds);
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i].X = lsqr.Solution[i];
            }
            for (int i = lsqr_ds.Count - m_k_clust, j = 0; i < lsqr_ds.Count; i++, j++)
            {
                lsqr_ds[i].Label = m_ref_pos[j].Y;
            }
            lsqr.Train(lsqr_ds);
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i].Y = lsqr.Solution[i];
            }
            return settings == null ? layout : settings.AdjustLayout(layout);
        }

        // TODO: exceptions
        public Vector2D[] Update(int dequeue_n, IEnumerableList<SparseVector<double>.ReadOnly> add_list)
        {
            // clustering 
            Utils.VerboseLine("Clustering ...");
            m_k_means.Update(dequeue_n, add_list);
            // determine reference instances
            UnlabeledDataset<SparseVector<double>.ReadOnly> ds_ref_inst = new UnlabeledDataset<SparseVector<double>.ReadOnly>();
            foreach (SparseVector<double> centroid in m_k_means.GetCentroids())
            {
                ds_ref_inst.Add(centroid); // dataset of reference instances
                //dataset.Add(centroid); // add centroids to the main dataset
            }
            // position reference instances
            Utils.VerboseLine("Positioning reference instances ...");
            SparseMatrix<double> sim_mtx = ModelUtils.GetDotProductSimilarity(ds_ref_inst, m_sim_thresh, /*full_matrix=*/false);
            StressMajorizationLayout sm = new StressMajorizationLayout(ds_ref_inst.Count, new DistFunc(sim_mtx));
            sm.Random = m_random;
            m_ref_pos = sm.ComputeLayout(/*settings=*/null, m_ref_pos/*make this a property!!!*/);
            // k-NN
            Utils.VerboseLine("Computing similarities ...");
            // ...
            return null;
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class DistFunc
           |
           '-----------------------------------------------------------------------
        */
        private class DistFunc : IDistance<int>
        {
            private SparseMatrix<double>.ReadOnly m_sim_mtx;

            public DistFunc(SparseMatrix<double>.ReadOnly sim_mtx)
            {
                m_sim_mtx = sim_mtx;
            }

            public double GetDistance(int a, int b) 
            {
                if (a > b) { return 1.0 - m_sim_mtx.TryGet(b, a, 0); }
                else { return 1.0 - m_sim_mtx.TryGet(a, b, 0); }
            }

            public void Save(BinarySerializer dummy)
            {
                throw new NotImplementedException();
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Neighborhood
           |
           '-----------------------------------------------------------------------
        */
        private class Neighborhood
        {
            public ArrayList<KeyDat<double, int>> Neighbors
                = null;
            
            public void Update(int dequeue_n, ArrayList<KeyDat<double, int>> new_inst_sim_sorted)
            {
                ArrayList<KeyDat<double, int>> aux = new ArrayList<KeyDat<double, int>>(Neighbors.Count);
                foreach (KeyDat<double, int> item in Neighbors)
                {
                    //if (item.Dat >= dequeue_n) { aux.Add(item - dequeue_n); }
                }
                Neighbors = aux;
            }
        }
    }
}