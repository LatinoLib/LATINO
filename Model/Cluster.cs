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
        private Cluster mParent
            = null;
        private ArrayList<Cluster> mChildren
            = new ArrayList<Cluster>();
        private Set<int> mItems
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
            get { return mParent; }
            set { mParent = null; }
        }

        public ArrayList<Cluster>.ReadOnly Children
        {
            get { return mChildren; }
        }

        public void AddChild(Cluster child)
        {
            Utils.ThrowException(child == null ? new ArgumentNullException("child") : null);
            mChildren.Add(child);
        }

        public void AddChildren(IEnumerable<Cluster> children)
        {
            Utils.ThrowException(children == null ? new ArgumentNullException("children") : null);
            foreach (Cluster child in children)
            {
                Utils.ThrowException(child == null ? new ArgumentValueException("children") : null);
                mChildren.Add(child);
            }
        }

        public void RemoveChildren()
        {
            mChildren.Clear();
        }

        public Set<int> Items
        {
            get { return mItems; }
        }

        public SparseVector<double> ComputeCentroid(IUnlabeledExampleCollection<SparseVector<double>.ReadOnly> dataset, CentroidType type)
        {
            return ModelUtils.ComputeCentroid(mItems, dataset, type); // throws ArgumentValueException
        }

        public override string ToString()
        {
            return ToString("C");
        }

        public string ToString(string format)
        {
            if (format == "C") // cluster 
            {
                return string.Format("( Parent:{0} Children:{1} Items:{2} )", mParent == null, mChildren.Count, mItems);
            }
            else if (format == "CC") // cluster compact 
            {
                return mItems.ToString();
            }
            else if (format == "T") // tree 
            {
                StringBuilder str = new StringBuilder();
                ToString("", str);
                return str.ToString().TrimEnd('\n', '\r');
            }
            else if (format == "TC") // tree compact
            {
                StringBuilder str = new StringBuilder();
                ToString("", str);
                return str.ToString().TrimEnd('\n', '\r');
            }
            else
            {
                throw new ArgumentNotSupportedException("format");
            }
        }

        private void ToString(string tab, StringBuilder str)
        {
            str.Append(tab);
            str.AppendLine(ToString("CC"));
            foreach (Cluster child in mChildren)
            {
                child.ToString(tab + "\t", str);
            }
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteObject(mParent);            
            mChildren.Save(writer); // *** this will not work properly if two or more parents share a child cluster
            mItems.Save(writer);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mParent = reader.ReadObject<Cluster>();
            mChildren = new ArrayList<Cluster>(reader);
            mItems = new Set<int>(reader);
        }        
    }
}
