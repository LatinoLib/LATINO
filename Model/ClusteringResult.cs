/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Cluster.cs
 *  Version:       1.0
 *  Desc:		   Custering result (output of clustering algorithms)
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 * 
 ***************************************************************************/

using System;
using System.Text;

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

        public ArrayList<Cluster> Roots
        {
            get { return m_roots; }
        }

        public ILabeledDataset<Cluster, ExT> GetClusteringDataset<LblT, ExT>(ILabeledDataset<LblT, ExT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            LabeledDataset<Cluster, ExT> clustering_dataset = new LabeledDataset<Cluster, ExT>();
            foreach (Cluster cluster in m_roots)
            {
                foreach (Pair<double, int> ex_info in cluster.Items)
                {
                    Utils.ThrowException(ex_info.Second < 0 || ex_info.Second >= dataset.Count ? new ArgumentValueException("Roots (cluster items)") : null);
                    clustering_dataset.Add(cluster, dataset[ex_info.Second].Example);
                }
            }
            return clustering_dataset;
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
