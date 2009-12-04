/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          __IncrementalKMeans__.cs (experimental)
 *  Version:       1.0
 *  Desc:		   Incremental K-means clustering algorithm 
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 * 
 ***************************************************************************/

using System;

namespace Latino.Model
{
    public class IncrementalKMeans : IClustering<SparseVector<double>.ReadOnly> 
    {
        private int m_k;
        private static ISimilarity<SparseVector<double>.ReadOnly> m_similarity
            = new DotProductSimilarity();
        private Random m_rnd
            = new Random();
        private double m_eps
            = 0.0005;
        private int m_trials
            = 1;
        private ArrayList<Centroid> m_centroids
            = null;
        private UnlabeledDataset<SparseVector<double>.ReadOnly> m_dataset
            = null;

        public IncrementalKMeans(int k)
        {
            Utils.ThrowException(k < 2 ? new ArgumentOutOfRangeException("k") : null);
            m_k = k;
        }

        public Random Random
        {
            get { return m_rnd; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Random") : null);
                m_rnd = value;
            }
        }

        public double Eps
        {
            get { return m_eps; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("Eps") : null);
                m_eps = value;
            }
        }

        public int Trials
        {
            get { return m_trials; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("Trials") : null);
                m_trials = value;
            }
        }

        // *** IClustering<LblT, SparseVector<double>.ReadOnly> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>.ReadOnly); }
        }

        public ClusteringResult Cluster(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count < m_k ? new ArgumentValueException("dataset") : null);            
            ClusteringResult clustering = null;
            double global_best_clust_qual = 0;
            for (int trial = 1; trial <= m_trials; trial++)
            {
                Utils.VerboseLine("*** CLUSTERING TRIAL {0} OF {1} ***", trial, m_trials);
                ArrayList<Centroid> centroids = new ArrayList<Centroid>(m_k);
                ArrayList<int> best_seeds = null;
                centroids.Add(new Centroid(dataset));
                int vec_len = centroids[0].VecLen;
                for (int i = 1; i < m_k; i++)
                {
                    centroids.Add(new Centroid(dataset, vec_len));
                }
                // select seed items
                double min_sim = double.MaxValue;
                ArrayList<int> tmp = new ArrayList<int>(dataset.Count);
                for (int i = 0; i < dataset.Count; i++) { tmp.Add(i); }
                for (int k = 0; k < 3; k++)
                {
                    ArrayList<SparseVector<double>.ReadOnly> seeds = new ArrayList<SparseVector<double>.ReadOnly>(m_k);
                    tmp.Shuffle(m_rnd);
                    for (int i = 0; i < m_k; i++)
                    {
                        seeds.Add(dataset[tmp[i]]);
                    }
                    // assess quality of seed items
                    double sim_avg = 0;
                    foreach (SparseVector<double>.ReadOnly seed_1 in seeds)
                    {
                        foreach (SparseVector<double>.ReadOnly seed_2 in seeds)
                        {
                            if (seed_1 != seed_2)
                            {
                                sim_avg += m_similarity.GetSimilarity(seed_1, seed_2);
                            }
                        }
                    }
                    sim_avg /= (double)(m_k * m_k - m_k);
                    //Console.WriteLine(sim_avg);
                    if (sim_avg < min_sim)
                    {
                        min_sim = sim_avg;
                        best_seeds = new ArrayList<int>(m_k);
                        for (int i = 0; i < m_k; i++) { best_seeds.Add(tmp[i]); }
                    }
                }
                for (int i = 0; i < m_k; i++)
                {
                    centroids[i].Items.Add(best_seeds[i]);
                    centroids[i].Update();
                }
                // main loop
                int iter = 0;
                double best_clust_qual = 0;
                double clust_qual;
                while (true)
                {
                    iter++;
                    clust_qual = 0;
                    // assign items to clusters
                    for (int i = 0; i < dataset.Count; i++)
                    {
                        SparseVector<double>.ReadOnly example = dataset[i];
                        double max_sim = double.MinValue;
                        ArrayList<int> candidates = new ArrayList<int>();
                        for (int j = 0; j < m_k; j++)
                        {
                            double sim = centroids[j].GetDotProduct(example);
                            if (sim > max_sim)
                            {
                                max_sim = sim;
                                candidates.Clear();
                                candidates.Add(j);
                            }
                            else if (sim == max_sim)
                            {
                                candidates.Add(j);
                            }
                        }
                        if (candidates.Count > 1)
                        {
                            candidates.Shuffle(m_rnd);
                        }
                        if (candidates.Count > 0) // *** is this always true? 
                        {
                            centroids[candidates[0]].Items.Add(i);
                            clust_qual += max_sim;
                        }
                    }
                    clust_qual /= (double)dataset.Count;
                    Utils.VerboseLine("*** Iteration {0} ***", iter);
                    Utils.VerboseLine("Quality: {0:0.0000}", clust_qual);
                    // compute new centroids
                    for (int i = 0; i < m_k; i++)
                    {
                        centroids[i].ResetNrmL2();
                        centroids[i].Update();
                        centroids[i].NormalizeL2();
                    }
                    // check if done
                    if (iter > 1 && clust_qual - best_clust_qual <= m_eps)
                    {
                        break;
                    }
                    best_clust_qual = clust_qual;
                }
                if (trial == 1 || clust_qual > global_best_clust_qual)
                {
                    global_best_clust_qual = clust_qual;
                    m_centroids = centroids;
                    // save the result
                    clustering = new ClusteringResult();
                    for (int i = 0; i < m_k; i++)
                    {
                        clustering.Roots.Add(new Cluster());
                        foreach (int idx in centroids[i].Items)
                        {
                            clustering.Roots.Last.Items.Add(new Pair<double, int>(1, idx));
                        }
                    }
                }
            }
            m_dataset = new UnlabeledDataset<SparseVector<double>.ReadOnly>(dataset); 
            return clustering;
        }

        // TODO: exceptions
        public ArrayList<SparseVector<double>.ReadOnly> GetCentroids()
        {
            ArrayList<SparseVector<double>.ReadOnly> centroids = new ArrayList<SparseVector<double>.ReadOnly>();
            foreach (Centroid centroid in m_centroids)
            {
                centroids.Add(centroid.GetSparseVector());
            }
            return centroids;
        }

        // TODO: exceptions
        public ClusteringResult Update(int dequeue_n, IEnumerableList<SparseVector<double>.ReadOnly> add_list)
        {
            // update dataset
            m_dataset.RemoveRange(0, dequeue_n);
            m_dataset.AddRange(add_list);
            // update centroid data
            foreach (Centroid centroid in m_centroids)
            {
                centroid.__UpdateDataset__(dequeue_n, m_dataset);
            }
            // k-means loop
            int iter = 0;
            double best_clust_qual = 0;
            double clust_qual;
            while (true)
            {
                iter++;
                clust_qual = 0;
                // assign items to clusters
                for (int i = 0; i < m_dataset.Count; i++)
                {
                    SparseVector<double>.ReadOnly example = m_dataset[i];
                    double max_sim = double.MinValue;
                    ArrayList<int> candidates = new ArrayList<int>();
                    for (int j = 0; j < m_k; j++)
                    {
                        double sim = m_centroids[j].GetDotProduct(example);
                        if (sim > max_sim)
                        {
                            max_sim = sim;
                            candidates.Clear();
                            candidates.Add(j);
                        }
                        else if (sim == max_sim)
                        {
                            candidates.Add(j);
                        }
                    }
                    if (candidates.Count > 1)
                    {
                        candidates.Shuffle(m_rnd);
                    }
                    if (candidates.Count > 0) // *** is this always true? 
                    {
                        m_centroids[candidates[0]].Items.Add(i);
                        clust_qual += max_sim;
                    }
                }
                clust_qual /= (double)m_dataset.Count;
                Utils.VerboseLine("*** Iteration {0} ***", iter);
                Utils.VerboseLine("Quality: {0:0.0000}", clust_qual);
                // compute new centroids
                for (int i = 0; i < m_k; i++)
                {
                    m_centroids[i].ResetNrmL2();
                    m_centroids[i].Update();
                    m_centroids[i].NormalizeL2();
                }
                // check if done
                if (iter > 1 && clust_qual - best_clust_qual <= m_eps)
                {
                    break;
                }
                best_clust_qual = clust_qual;
            }
            // save the result
            ClusteringResult clustering = new ClusteringResult();
            for (int i = 0; i < m_k; i++)
            {
                clustering.Roots.Add(new Cluster());
                foreach (int idx in m_centroids[i].Items)
                {
                    clustering.Roots.Last.Items.Add(new Pair<double, int>(1, idx));
                }
            }
            return clustering;
        }

        ClusteringResult IClustering.Cluster(IUnlabeledExampleCollection dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>.ReadOnly>) ? new ArgumentTypeException("dataset") : null);
            return Cluster((IUnlabeledExampleCollection<SparseVector<double>.ReadOnly>)dataset); // throws ArgumentValueException
        }
    }
}