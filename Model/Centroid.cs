/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Centroid.cs
 *  Version:       1.0
 *  Desc:		   Centroid vector structure (used for speed optimizations)
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Internal class Centroid
       |
       '-----------------------------------------------------------------------
    */
    internal class Centroid
    {
        private IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> m_dataset;
        private Set<int> m_current_items
            = new Set<int>();
        private Set<int> m_items
            = new Set<int>();
        private Set<int> m_non_zero_idx
            = new Set<int>();
        private double m_div
            = 1;
        private double[] m_vec;

        public Centroid(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, int vec_len)
        {
            m_vec = new double[vec_len];
            m_dataset = dataset;
        }

        public Centroid(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset)
        {
            int max_idx = -1;
            foreach (SparseVector<double>.ReadOnly example in dataset)
            {
                int last_idx = example.LastNonEmptyIndex;
                if (last_idx > max_idx) { max_idx = last_idx; }
            }
            m_vec = new double[max_idx + 1];
            m_dataset = dataset;
        }

        public Set<int> Items
        {
            get { return m_items; }
        }

        public Set<int>.ReadOnly CurrentItems
        {
            get { return m_current_items; }
        }

        public int VecLen
        {
            get { return m_vec.Length; }
        }

        public void Update()
        { 
            Set<int> add_idx = Set<int>.Difference(m_items, m_current_items);
            Set<int> rmv_idx = Set<int>.Difference(m_current_items, m_items);
            foreach (int item_idx in add_idx)
            {
                SparseVector<double>.ReadOnly vec = m_dataset[item_idx];
                foreach (IdxDat<double> item in vec)
                {
                    if (item.Dat != 0)
                    {
                        if (item.Idx >= m_vec.Length)
                        { 
                            // extend vector
                            double[] new_vec = new double[item.Idx + 1];
                            foreach (int idx in m_non_zero_idx)
                            {
                                new_vec[idx] = m_vec[idx];
                            }
                            m_vec = new_vec;
                        }
                        if (Math.Abs(m_vec[item.Idx]) < 1E-6) { m_non_zero_idx.Add(item.Idx); }
                        else if (Math.Abs(m_vec[item.Idx] + item.Dat) < 1E-6) { m_non_zero_idx.Remove(item.Idx); }
                        m_vec[item.Idx] += item.Dat;
                    }
                }
            }
            foreach (int item_idx in rmv_idx)
            {
                SparseVector<double>.ReadOnly vec = m_dataset[item_idx];
                foreach (IdxDat<double> item in vec)
                {
                    if (item.Dat != 0)
                    {
                        if (Math.Abs(m_vec[item.Idx]) < 1E-6) { m_non_zero_idx.Add(item.Idx); }
                        else if (Math.Abs(m_vec[item.Idx] - item.Dat) < 1E-6) { m_non_zero_idx.Remove(item.Idx); }
                        m_vec[item.Idx] -= item.Dat;
                    }
                }
            }
            m_current_items = m_items;
            m_items = new Set<int>();
        }

        public void NormalizeL2()
        {
            if (m_div == 1)
            {
                double len = 0;
                foreach (int idx in m_non_zero_idx)
                {
                    len += m_vec[idx] * m_vec[idx];
                }
                len = Math.Sqrt(len);
                foreach (int idx in m_non_zero_idx)
                {
                    m_vec[idx] /= len;
                }
                m_div = len;
            }
        }

        public void ResetNrmL2()
        {
            if (m_div != 1)
            {
                foreach (int idx in m_non_zero_idx)
                {
                    m_vec[idx] *= m_div;
                }
                m_div = 1;
            }
        }

        public void Clear()
        {
            foreach (int idx in m_non_zero_idx)
            {
                m_vec[idx] = 0;
            }
            m_non_zero_idx.Clear();            
        }

        public SparseVector<double> GetSparseVector()
        {
            SparseVector<double> vec = new SparseVector<double>();
            foreach (int idx in m_non_zero_idx)
            {
                vec.InnerIdx.Add(idx);
                vec.InnerDat.Add(m_vec[idx]);
            }
            return vec;
        }

        public double GetDotProduct(SparseVector<double>.ReadOnly vec)
        {
            double dot_prod = 0;
            foreach (IdxDat<double> item in vec)
            {
                if (item.Idx < m_vec.Length)
                {
                    dot_prod += item.Dat * m_vec[item.Idx];
                }
            }
            return dot_prod;
        }
    }
}
