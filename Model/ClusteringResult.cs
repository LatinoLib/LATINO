/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Cluster.cs
 *  Version:       1.0
 *  Desc:		   Custering result (output of clustering algorithms)
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Dec-2009
 *  Revision:      Dec-2009
 * 
 ***************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class ClusteringResult
       |
       '-----------------------------------------------------------------------
    */
    public class ClusteringResult : ISerializable
    {
        private ArrayList<Cluster> m_roots
            = new ArrayList<Cluster>();

        public ClusteringResult()
        { 
        }

        public ClusteringResult(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public ArrayList<Cluster>.ReadOnly Roots
        {
            get { return m_roots; }
        }

        public void AddRoot(Cluster root)
        {
            Utils.ThrowException(root == null ? new ArgumentNullException("root") : null);
            m_roots.Add(root);
        }

        public void AddRoots(IEnumerable<Cluster> roots)
        {
            Utils.ThrowException(roots == null ? new ArgumentNullException("roots") : null);
            foreach (Cluster root in roots)
            {
                Utils.ThrowException(root == null ? new ArgumentValueException("roots") : null);
                m_roots.Add(root);
            }
        }

        public void Clear()
        {
            m_roots.Clear();
        }

        private void FillClassificationDataset<ExT>(IEnumerable<Cluster> clusters, IUnlabeledExampleCollection<ExT> dataset, LabeledDataset<Cluster, ExT> classification_dataset)
        {
            foreach (Cluster cluster in clusters)
            {
                foreach (int idx in cluster.Items)
                {
                    Utils.ThrowException(idx < 0 || idx >= dataset.Count ? new ArgumentValueException("clusters") : null);
                    classification_dataset.Add(cluster, dataset[idx]);
                }
                FillClassificationDataset(cluster.Children, dataset, classification_dataset);
            }
        }

        public LabeledDataset<Cluster, ExT> GetClassificationDataset<ExT>(IUnlabeledExampleCollection<ExT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            LabeledDataset<Cluster, ExT> classification_dataset = new LabeledDataset<Cluster, ExT>();
            FillClassificationDataset(m_roots, dataset, classification_dataset);
            return classification_dataset;
        }

        public override string ToString()
        {
            return ToString("T");
        }

        public string ToString(string format)
        {
            StringBuilder str_builder = new StringBuilder();
            foreach (Cluster root in m_roots)
            {
                str_builder.AppendLine(root.ToString(format)); // throws ArgumentNotSupportedException
            }
            return str_builder.ToString().TrimEnd('\n', '\r');
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            m_roots.Save(writer); // throws serialization-related exceptions
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            m_roots = new ArrayList<Cluster>(reader); // throws serialization-related exceptions
        }
    }
}
