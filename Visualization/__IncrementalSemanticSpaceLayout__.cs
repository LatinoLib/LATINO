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
        /*private*/public UnlabeledDataset<SparseVector<double>.ReadOnly> m_dataset;
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
            = 30;
        private IncrementalKMeans m_k_means
            = null;
        private Vector2D[] m_ref_pos
            = null;
        private ArrayList<Patch> m_patches
            = new ArrayList<Patch>();

        public IncrementalSemanticSpaceLayout(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            m_dataset = new UnlabeledDataset<SparseVector<double>.ReadOnly>(dataset);
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
            // clustering 
            Utils.VerboseLine("Clustering ...");
            m_k_means = new IncrementalKMeans(m_k_clust);
            m_k_means.Eps = m_k_means_eps;
            m_k_means.Random = m_random;
            m_k_means.Trials = 3;
            ClusteringResult clustering = m_k_means.Cluster(m_dataset); // throws ArgumentValueException
            // determine reference instances
            UnlabeledDataset<SparseVector<double>.ReadOnly> ds_ref_inst = new UnlabeledDataset<SparseVector<double>.ReadOnly>();
            foreach (SparseVector<double> centroid in m_k_means.GetCentroids())
            {
                ds_ref_inst.Add(centroid); // dataset of reference instances
                m_dataset.Add(centroid); // add centroids to the main dataset
            }
            // position reference instances
            Utils.VerboseLine("Positioning reference instances ...");
            SparseMatrix<double> sim_mtx = ModelUtils.GetDotProductSimilarity(ds_ref_inst, m_sim_thresh, /*full_matrix=*/false);
            StressMajorizationLayout sm = new StressMajorizationLayout(ds_ref_inst.Count, new DistFunc(sim_mtx));
            sm.Random = m_random;
            sm.MaxSteps = 10000;
            sm.MinDiff = 0;
            m_ref_pos = sm.ComputeLayout();
            // k-NN
            Utils.VerboseLine("Computing similarities ...");
            sim_mtx = ModelUtils.GetDotProductSimilarity(m_dataset, m_sim_thresh, /*full_matrix=*/true);
            Utils.VerboseLine("Constructing system of linear equations ...");
            LabeledDataset<double, SparseVector<double>.ReadOnly> lsqr_ds = new LabeledDataset<double, SparseVector<double>.ReadOnly>();
            m_patches = new ArrayList<Patch>(m_dataset.Count);
            for (int i = 0; i < m_dataset.Count; i++)
            {
                m_patches.Add(new Patch(i));
            }
            foreach (IdxDat<SparseVector<double>> sim_mtx_row in sim_mtx)
            {
                if (sim_mtx_row.Dat.Count <= 1)
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
                for (int i = 0; i < count; i++)
                {
                    m_patches[sim_mtx_row.Idx].List.Add(new KeyDat<double, Patch>(knn[i].Key, m_patches[knn[i].Dat]));                    
                }
                m_patches[sim_mtx_row.Idx].ProcessList();
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
            Vector2D[] layout = new Vector2D[m_dataset.Count - m_k_clust];
            for (int i = m_dataset.Count - m_k_clust, j = 0; i < m_dataset.Count; i++, j++)
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

        private void Output(IEnumerable<KeyDat<double, Patch>> list)
        { 
            foreach (KeyDat<double, Patch> item in list)
            {
                Console.Write("({0} {1}) ", item.Key, item.Dat.Idx);
            }
            Console.WriteLine();
        }

        public class PtInfo
        {
            public double X;
            public double Y;
            public SparseVector<double> Vec;
        }

        // TODO: exceptions
        public Vector2D[] Update(int num_dequeue, IEnumerable<SparseVector<double>.ReadOnly> new_inst, bool test, LayoutSettings settings, ref PtInfo[] pt_info)
        {
            // clustering 
            Utils.VerboseLine("Clustering ...");
            m_k_means.Eps = m_k_means_eps;
            m_k_means.Update(num_dequeue, new_inst);
            // determine reference instances
            UnlabeledDataset<SparseVector<double>.ReadOnly> ds_ref_inst = new UnlabeledDataset<SparseVector<double>.ReadOnly>();
            UnlabeledDataset<SparseVector<double>.ReadOnly> ds_new_inst = new UnlabeledDataset<SparseVector<double>.ReadOnly>(new_inst);
            foreach (SparseVector<double> centroid in m_k_means.GetCentroids())
            {
                ds_ref_inst.Add(centroid); // dataset of reference instances
                ds_new_inst.Add(centroid); // dataset of new instances
            }
            // position reference instances
            Utils.VerboseLine("Positioning reference instances ...");
            SparseMatrix<double> sim_mtx = ModelUtils.GetDotProductSimilarity(ds_ref_inst, m_sim_thresh, /*full_matrix=*/false);
            StressMajorizationLayout sm = new StressMajorizationLayout(ds_ref_inst.Count, new DistFunc(sim_mtx));
            sm.Random = m_random;
            m_ref_pos = sm.ComputeLayout(/*settings=*/null, m_ref_pos/*make this a property!!!*/);
            // k-NN
            Utils.VerboseLine("Computing similarities ...");
            // update list of neighborhoods
            m_patches.RemoveRange(m_dataset.Count - m_k_clust, m_k_clust);
            m_patches.RemoveRange(0, num_dequeue);
            // remove instances from dataset and neighborhoods
            foreach (Patch patch in m_patches)
            {                                
                if (patch.Min != null && (patch.Min.Idx < num_dequeue || patch.Max.Idx >= m_dataset.Count - m_k_clust))
                {
                    int old_count = patch.List.Count;
                    ArrayList<KeyDat<double, Patch>> tmp = new ArrayList<KeyDat<double, Patch>>();
                    foreach (KeyDat<double, Patch> item in patch.List)
                    {
                        if (item.Dat.Idx >= num_dequeue && item.Dat.Idx < m_dataset.Count - m_k_clust)
                        {
                            tmp.Add(item);
                        }
                        //else
                        //{
                        //    Console.WriteLine("Remove {0}", item.Dat.Idx - num_dequeue);
                        //}
                    }                   
                    patch.List = tmp;
                    patch.ProcessList();
                    patch.NeedUpdate = patch.List.Count < m_k_nn && old_count >= m_k_nn;
                }                
            }
            // update dataset
            m_dataset.RemoveRange(m_dataset.Count - m_k_clust, m_k_clust);
            m_dataset.RemoveRange(0, num_dequeue);
            // add new instances to dataset 
            int pre_add_count = m_dataset.Count;
            m_dataset.AddRange(ds_new_inst);
            // precompute transposed matrices
            SparseMatrix<double> tr_new_inst = ModelUtils.GetTransposedMatrix(ds_new_inst);
            SparseMatrix<double> tr_dataset = ModelUtils.GetTransposedMatrix(m_dataset);
            // add new instances to neighborhoods
            for (int i = 0; i < ds_new_inst.Count; i++)
            {
                m_patches.Add(new Patch(-1));
                m_patches.Last.NeedUpdate = true;
            }
            for (int i = 0; i < m_patches.Count; i++)
            { 
                m_patches[i].Idx = i;
            }
            for (int i = 0; i < m_patches.Count; i++)
            {
                Patch patch = m_patches[i];                
                SparseVector<double>.ReadOnly vec = m_dataset[i];
                if (vec != null)
                {
                    if (patch.NeedUpdate) // full update required
                    {
                        //if (i == 1347) { Console.WriteLine("full update"); }
                        SparseVector<double> sim_vec = ModelUtils.GetDotProductSimilarity(tr_dataset, m_dataset.Count, vec, m_sim_thresh);
                        ArrayList<KeyDat<double, int>> tmp = new ArrayList<KeyDat<double, int>>();
                        foreach (IdxDat<double> item in sim_vec)
                        {
                            if (item.Idx != i)
                            {
                                tmp.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                            }
                        }
                        tmp.Sort(new Comparer2());
                        int count = Math.Min(tmp.Count, m_k_nn_ext);
                        patch.List.Clear();
                        for (int j = 0; j < count; j++)
                        {
                            patch.List.Add(new KeyDat<double, Patch>(tmp[j].Key, m_patches[tmp[j].Dat]));
                        }
                        patch.ProcessList();
                        patch.NeedUpdate = false;
                    }
                    else // only new instances need to be considered
                    {
                        //if (i == 1347) { Console.WriteLine("partial update"); }
                        SparseVector<double> sim_vec = ModelUtils.GetDotProductSimilarity(tr_new_inst, ds_new_inst.Count, vec, m_sim_thresh);
                        // check if further processing is needed
                        bool need_merge = false;
                        if (test)
                        {
                            foreach (IdxDat<double> item in sim_vec)
                            {
                                if (item.Dat >= patch.MinSim) 
                                {
                                    need_merge = true;
                                    //Console.WriteLine("{0} {1}", item.Dat, patch.MinSim);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (IdxDat<double> item in sim_vec)
                            {
                                if (item.Dat > patch.MinSim) 
                                {
                                    need_merge = true;
                                    //Console.WriteLine("{0} {1}", item.Dat, patch.MinSim);
                                    break;
                                }
                            }
                        }
                        if (need_merge || patch.List.Count < m_k_nn)
                        {
                            //if (i == 1347) { Console.WriteLine("merge"); }
                            int old_count = patch.List.Count;
                            ArrayList<KeyDat<double, Patch>> tmp = new ArrayList<KeyDat<double, Patch>>();
                            foreach (IdxDat<double> item in sim_vec)
                            {
                                tmp.Add(new KeyDat<double, Patch>(item.Dat, m_patches[item.Idx + pre_add_count]));
                            }
                            // merge the two lists
                            // TODO: speed this up
                            patch.List.AddRange(tmp);
                            patch.List.Sort(new Comparer());
                            // trim list to size
                            if (old_count >= m_k_nn)
                            {
                                patch.List.RemoveRange(old_count, patch.List.Count - old_count);
                            }
                            patch.ProcessList();
                        }
                    }
                }
            }
            // *** Test ***
            if (test)
            {
                sim_mtx = ModelUtils.GetDotProductSimilarity(m_dataset, m_sim_thresh, /*full_matrix=*/true);
                ArrayList<Patch> patches = new ArrayList<Patch>();
                for (int i = 0; i < m_dataset.Count; i++)
                {
                    patches.Add(new Patch(i));
                }
                foreach (IdxDat<SparseVector<double>> sim_mtx_row in sim_mtx)
                {
                    if (sim_mtx_row.Dat.Count <= 1)
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
                    knn.Sort(new Comparer2());
                    int count = Math.Min(knn.Count, m_k_nn_ext);
                    for (int i = 0; i < count; i++)
                    {
                        patches[sim_mtx_row.Idx].List.Add(new KeyDat<double, Patch>(knn[i].Key, patches[knn[i].Dat]));
                    }
                    patches[sim_mtx_row.Idx].ProcessList();
                }
                // compare 
                if (patches.Count != m_patches.Count) { throw new Exception("Count mismatch."); }
                for (int i = 0; i < m_patches.Count; i++)
                {
                    if (patches[i].List.Count < m_k_nn && patches[i].List.Count != m_patches[i].List.Count)
                    {
                        Console.WriteLine(m_patches[i].List.Count);
                        Console.WriteLine(patches[i].List.Count);
                        Output(m_patches[i].List);
                        Output(patches[i].List);
                        Console.WriteLine(i);
                        throw new Exception("List count mismatch.");
                    }
                    int count = Math.Min(m_patches[i].List.Count, m_k_nn);
                    for (int j = 0; j < count; j++)
                    {
                        //Console.WriteLine("{4} {0}-{1} {2}-{3}", m_patches[i].List[j].Key, m_patches[i].List[j].Dat.Idx, patches[i].List[j].Key, patches[i].List[j].Dat.Idx, i);
                        if (m_patches[i].List[j].Key != patches[i].List[j].Key || m_patches[i].List[j].Dat.Idx != patches[i].List[j].Dat.Idx)
                        {
                            Console.WriteLine("i:{4} fast:{0}-{1} slow:{2}-{3}", m_patches[i].List[j].Key, m_patches[i].List[j].Dat.Idx, patches[i].List[j].Key, patches[i].List[j].Dat.Idx, i);
                            int idx_fast = m_patches[i].List[j].Dat.Idx;
                            int idx_slow = patches[i].List[j].Dat.Idx;
                            Console.WriteLine("slow @ fast idx: {0}", GetKey(patches[i].List, idx_fast));
                            Console.WriteLine("fast @ slow idx: {0}", GetKey(m_patches[i].List, idx_slow));
                            throw new Exception("Patch item mismatch.");
                        }
                    }
                }
            }
            // *** End of test ***
            //Console.WriteLine("Number of patches: {0}", m_patches.Count);
            //int waka = 0;
            //foreach (Patch patch in m_patches)
            //{
            //    waka += patch.List.Count;
            //}
            //Console.WriteLine("Avg list size: {0}", (double)waka / (double)m_patches.Count);
            Utils.VerboseLine("Constructing system of linear equations ...");
            LabeledDataset<double, SparseVector<double>.ReadOnly> lsqr_ds = new LabeledDataset<double, SparseVector<double>.ReadOnly>();
            Vector2D[] layout = new Vector2D[m_dataset.Count - m_k_clust];
            foreach (Patch patch in m_patches)
            {
                int count = Math.Min(patch.List.Count, m_k_nn);
                SparseVector<double> eq = new SparseVector<double>();
                double wgt = 1.0 / (double)count;
                for (int i = 0; i < count; i++)
                {
                    eq.InnerIdx.Add(patch.List[i].Dat.Idx);
                    eq.InnerDat.Add(-wgt);
                }
                eq.InnerIdx.Sort(); // *** sort only indices
                eq[patch.Idx] = 1;
                lsqr_ds.Add(0, eq);
            }
            for (int i = m_dataset.Count - m_k_clust, j = 0; i < m_dataset.Count; i++, j++)
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
            // -----------------------------------------------------------------
            // make pt_info
            // -----------------------------------------------------------------
            pt_info = new PtInfo[layout.Length];
            int ii = 0;
            foreach (Vector2D pt in layout)
            {
                pt_info[ii] = new PtInfo();
                pt_info[ii].X = pt.X;
                pt_info[ii].Y = pt.Y;
                pt_info[ii].Vec = m_dataset[ii].GetWritableCopy();
                ii++;
            }
            // -----------------------------------------------------------------
            return settings == null ? layout : settings.AdjustLayout(layout);
        }

        private double GetKey(ArrayList<KeyDat<double, Patch>> list, int idx)
        {
            foreach (KeyDat<double, Patch> item in list)
            {
                if (item.Dat.Idx == idx) { return item.Key; }
            }
            return -1;
        }

        private class Comparer : IComparer<KeyDat<double, Patch>> 
        {
            public int Compare(KeyDat<double, Patch> x, KeyDat<double, Patch> y)
            {
                if (Math.Abs(y.Key - x.Key) > 0.000001) { return y.Key.CompareTo(x.Key); }
                return y.Dat.Idx.CompareTo(x.Dat.Idx); 
            }
        }

        private class Comparer2 : IComparer<KeyDat<double, int>>
        {
            public int Compare(KeyDat<double, int> x, KeyDat<double, int> y)
            {
                if (Math.Abs(y.Key - x.Key) > 0.000001) { return y.Key.CompareTo(x.Key); }
                return y.Dat.CompareTo(x.Dat);
            }
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
           |  Class Patch
           |
           '-----------------------------------------------------------------------
        */
        private class Patch
        {
            public int Idx
                = -1;
            public ArrayList<KeyDat<double, Patch>> List
                = new ArrayList<KeyDat<double, Patch>>();
            public Patch Min
                = null;
            public Patch Max
                = null;
            public double MinSim
                = -1;
            public bool NeedUpdate
                = false;

            public Patch(int idx)
            {
                Idx = idx;
            }

            public void ProcessList()
            {
                MinSim = -1;
                if (List.Count > 0) { MinSim = List.Last.Key; }
                Min = null;
                Max = null;
                foreach (KeyDat<double, Patch> item in List)
                {
                    if (Max == null || item.Dat.Idx > Max.Idx) { Max = item.Dat; }
                    if (Min == null || item.Dat.Idx < Min.Idx) { Min = item.Dat; }
                }
            }
        }
    }
}