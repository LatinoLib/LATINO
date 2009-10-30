/*==========================================================================;
 *
 *  (c) 2007-09 JSI.  All rights reserved.
 *
 *  File:          KnnClassifier.cs
 *  Version:       1.0
 *  Desc:		   K-nearest neighbors classifier 
 *  Author:        Miha Grcar
 *  Created on:    Aug-2007
 *  Last modified: Jan-2009
 *  Revision:      Jan-2009
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class KnnClassifier<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public class KnnClassifier<LblT, ExT> : IModel<LblT, ExT>
    {
        private ArrayList<LabeledExample<LblT, ExT>> m_examples
            = null;
        private IEqualityComparer<LblT> m_lbl_cmp
            = null;
        private ISimilarity<ExT> m_similarity
            = null;
        public KnnClassifier()
        {
        }
        public KnnClassifier(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }
        public IEqualityComparer<LblT> LabelEqualityComparer
        {
            get { return m_lbl_cmp; }
            set { m_lbl_cmp = value; }
        }
        public ISimilarity<ExT> Similarity
        {
            get { return m_similarity; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Similarity") : null);
                m_similarity = value;
            }
        }
        // *** IModel<LblT, ExT> interface implementation ***
        public Type RequiredExampleType
        {
            get { return typeof(ExT); }
        }
        public bool IsTrained
        {
            get { return m_examples != null; }
        }
        public void Train(IExampleCollection<LblT, ExT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            m_examples = new ArrayList<LabeledExample<LblT, ExT>>(dataset);
        }
        void IModel<LblT>.Train(IExampleCollection<LblT> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is IExampleCollection<LblT, ExT>) ? new ArgumentTypeException("dataset") : null);
            Train((IExampleCollection<LblT, ExT>)dataset); // throws ArgumentValueException
        }
        public ClassifierResult<LblT> Classify(ExT example)
        {
            Utils.ThrowException((m_examples == null || m_similarity == null) ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            // ...
            return null;
        }
        ClassifierResult<LblT> IModel<LblT>.Classify(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is ExT) ? new ArgumentTypeException("example") : null);
            return Classify((ExT)example); // throws InvalidOperationException
        }
        // *** ISerializable interface implementation ***
        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            //Utils.ThrowException(m_examples == null ? new InvalidOperationException() : null);
            // the following statements throw serialization-related exceptions
            // ...
        }
        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            // ...
        }
    }
}
