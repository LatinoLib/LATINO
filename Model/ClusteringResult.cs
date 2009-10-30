/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Cluster.cs
 *  Version:       1.0
 *  Desc:		   Custering result (output of clustering algorithms)
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Aug-2009
 *  Revision:      Aug-2009
 * 
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class ClusteringResult
       |
       '-----------------------------------------------------------------------
    */
    public class ClusteringResult : ISerializable//, ICloneable<ClusteringResult>
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
        public IDataset<Cluster, ExT> GetClusteringDataset<LblT, ExT>(IDataset<LblT, ExT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Dataset<Cluster, ExT> clustering_dataset = new Dataset<Cluster, ExT>();
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
        // *** ICloneable<ClusteringResult> interface implementation ***
        // ...
        // *** ISerializable interface implementation ***
        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            m_roots.Save(writer);
        }
        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            m_roots = new ArrayList<Cluster>(reader);
        }
    }
}
