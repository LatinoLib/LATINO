/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Cluster.cs
 *  Version:       1.0
 *  Desc:		   Holds information about a cluster
 *  Author:        Miha Grcar 
 *  Created on:    Aug-2009
 *  Last modified: Dec-2009
 *  Revision:      Dec-2009
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Cluster
       |
       '-----------------------------------------------------------------------
    */
    public class Cluster : ISerializable
    {
        private Cluster m_parent
            = null;
        private ArrayList<Cluster> m_children
            = new ArrayList<Cluster>();
        private Set<int> m_items
            = new Set<int>();

        public Cluster()
        { 
        }

        public Cluster(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public Cluster Parent
        {
            get { return m_parent; }
            set { m_parent = null; }
        }

        public ArrayList<Cluster>.ReadOnly Children
        {
            get { return m_children; }
        }

        public void AddChild(Cluster child)
        {
            Utils.ThrowException(child == null ? new ArgumentNullException("child") : null);
            m_children.Add(child);
        }

        public void AddChildren(IEnumerable<Cluster> children)
        {
            Utils.ThrowException(children == null ? new ArgumentNullException("children") : null);
            foreach (Cluster child in children)
            {
                Utils.ThrowException(child == null ? new ArgumentValueException("children") : null);
                m_children.Add(child);
            }
        }

        public void RemoveChildren()
        {
            m_children.Clear();
        }

        public Set<int> Items
        {
            get { return m_items; }
        }

        public SparseVector<double> ComputeCentroid(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, CentroidType type)
        {
            return ModelUtils.ComputeCentroid(m_items, dataset, type); // throws ArgumentValueException
        }

        public override string ToString()
        {
            return ToString("C");
        }

        public string ToString(string format)
        {
            if (format == "C") // cluster 
            {
                return string.Format("( Parent:{0} Children:{1} Items:{2} )", m_parent == null, m_children.Count, m_items);
            }
            else if (format == "CC") // cluster compact 
            {
                return m_items.ToString();
            }
            else if (format == "T") // tree 
            {
                StringBuilder str_builder = new StringBuilder();
                ToString("", str_builder);
                return str_builder.ToString().TrimEnd('\n', '\r');
            }
            else if (format == "TC") // tree compact
            {
                StringBuilder str_builder = new StringBuilder();
                ToString("", str_builder);
                return str_builder.ToString().TrimEnd('\n', '\r');
            }
            else
            {
                throw new ArgumentNotSupportedException("format");
            }
        }

        private void ToString(string tab, StringBuilder str_builder)
        {
            str_builder.Append(tab);
            str_builder.AppendLine(ToString("CC"));
            foreach (Cluster child in m_children)
            {
                child.ToString(tab + "\t", str_builder);
            }
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteObject(m_parent);            
            m_children.Save(writer); // *** this will not work properly if two or more parents share a child cluster
            m_items.Save(writer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            m_parent = reader.ReadObject<Cluster>();
            m_children = new ArrayList<Cluster>(reader);
            m_items = new Set<int>(reader);
        }        
    }
}
