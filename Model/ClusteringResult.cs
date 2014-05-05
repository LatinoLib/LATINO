/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ClusteringResult.cs
 *  Desc:    Custering result (output of clustering algorithms)
 *  Created: Aug-2009
 *
 *  Author:  Miha Grcar 
 * 
 *  License: MIT (http://opensource.org/licenses/MIT) 
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
        private ArrayList<Cluster> mRoots
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
            get { return mRoots; }
        }

        public void AddRoot(Cluster root)
        {
            Utils.ThrowException(root == null ? new ArgumentNullException("root") : null);
            mRoots.Add(root);
        }

        public void AddRoots(IEnumerable<Cluster> roots)
        {
            Utils.ThrowException(roots == null ? new ArgumentNullException("roots") : null);
            foreach (Cluster root in roots)
            {
                Utils.ThrowException(root == null ? new ArgumentValueException("roots") : null);
                mRoots.Add(root);
            }
        }

        public void Clear()
        {
            mRoots.Clear();
        }

        private void FillClassificationDataset<ExT>(IEnumerable<Cluster> clusters, IUnlabeledExampleCollection<ExT> dataset, LabeledDataset<Cluster, ExT> classificationDataset)
        {
            foreach (Cluster cluster in clusters)
            {
                foreach (int item in cluster.Items)
                {
                    Utils.ThrowException(item < 0 || item >= dataset.Count ? new ArgumentValueException("clusters") : null);
                    classificationDataset.Add(cluster, dataset[item]);
                }
                FillClassificationDataset(cluster.Children, dataset, classificationDataset);
            }
        }

        public LabeledDataset<Cluster, ExT> GetClassificationDataset<ExT>(IUnlabeledExampleCollection<ExT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            LabeledDataset<Cluster, ExT> classificationDataset = new LabeledDataset<Cluster, ExT>();
            FillClassificationDataset(mRoots, dataset, classificationDataset); // throws ArgumentValueException
            return classificationDataset;
        }

        public override string ToString()
        {
            return ToString("T");
        }

        public string ToString(string format)
        {
            StringBuilder str = new StringBuilder();
            foreach (Cluster root in mRoots)
            {
                str.AppendLine(root.ToString(format)); // throws ArgumentNotSupportedException
            }
            return str.ToString().TrimEnd('\n', '\r');
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            mRoots.Save(writer); // throws serialization-related exceptions
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mRoots = new ArrayList<Cluster>(reader); // throws serialization-related exceptions
        }
    }
}
