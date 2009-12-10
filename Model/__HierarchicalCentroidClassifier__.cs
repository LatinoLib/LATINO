/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          __HierarchicalCentroidClassifier__.cs 
 *  Version:       1.0
 *  Desc:		   Hierarchical centroid classifier (experimental)
 *  Author:        Miha Grcar 
 *  Created on:    Dec-2009
 *  Last modified: Dec-2009
 *  Revision:      Dec-2009
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class HierarchicalCentroidClassifier
       |
       '-----------------------------------------------------------------------
    */
    public class HierarchicalCentroidClassifier : IHierarchicalModel<SparseVector<double>.ReadOnly>
    {
        private ClassifierType m_classifier_type
            = ClassifierType.Flat;
        private Dictionary<Cluster, ClusterInfo> m_model
            = null;
        private IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> m_dataset
            = null;
        private bool m_remove_duplicates
            = true;

        public HierarchicalCentroidClassifier()
        {
        }

        public HierarchicalCentroidClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public ClassifierType Type
        {
            get { return m_classifier_type; }
            set { m_classifier_type = value; }
        }

        // *** IHierarchicalModel<SparseVector<double>.ReadOnly> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>.ReadOnly); }
        }

        public bool IsTrained
        {
            get { return m_model != null; }
        }

        private Set<int> ComputeCentroid(Cluster cluster)
        {
            if (cluster.Children.Count == 0)
            {
                ClusterInfo cluster_info = new ClusterInfo();
                cluster_info.Centroid = ModelUtils.ComputeCentroid(cluster.Items, m_dataset, CentroidType.Sum);
                m_model.Add(cluster, cluster_info);
                return cluster.Items;
            }
            else
            {
                Set<int> items = new Set<int>();
                foreach (Cluster child in cluster.Children)
                {
                    items.AddRange(ComputeCentroid(child));
                }
                if (m_remove_duplicates) { items = Set<int>.Difference(cluster.Items, items); }
                ClusterInfo cluster_info = new ClusterInfo();
                cluster_info.Centroid = ModelUtils.ComputeCentroid(items, m_dataset, CentroidType.Sum);
                m_model.Add(cluster, cluster_info);
                return items;
            }
        }

        public void Train(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, ClusteringResult hierarchy)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            Utils.ThrowException(hierarchy == null ? new ArgumentNullException("hierarchy") : null);
            Utils.ThrowException(hierarchy.Roots.Count == 0 ? new ArgumentValueException("hierarchy") : null);
            m_model = new Dictionary<Cluster, ClusterInfo>();
            m_dataset = dataset;
            foreach (Cluster root in hierarchy.Roots)
            {
                ComputeCentroid(root);
            }
            m_dataset = null;
        }

        void IHierarchicalModel.Train(IUnlabeledExampleCollection dataset, ClusteringResult hierarchy)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IUnlabeledExampleCollection<SparseVector<double>.ReadOnly>) ? new ArgumentTypeException("dataset") : null);
            Train((IUnlabeledExampleCollection<SparseVector<double>.ReadOnly>)dataset, hierarchy); // throws ArgumentNullException, ArgumentValueException
        }

        public Prediction<Cluster> Predict(SparseVector<double>.ReadOnly example)
        {
            Utils.ThrowException(m_model == null ? new InvalidOperationException() : null);
            // ...
            return null;
        }

        Prediction<Cluster> IHierarchicalModel.Predict(object example)
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
            // ...
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions            
            // ...
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum ClassifierType
           |
           '-----------------------------------------------------------------------
        */
        public enum ClassifierType
        {
            Flat,
            FlatPropagate,
            //HierarchicalBestBranch,
            //HierarchicalGreedy,
            //HierarchicalBestLeaf
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class ClusterInfo
           |
           '-----------------------------------------------------------------------
        */
        private class ClusterInfo
        {
            public SparseVector<double>.ReadOnly Centroid
                = null;
        }
    }
}
