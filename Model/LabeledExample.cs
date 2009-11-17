/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          LabeledExample.cs
 *  Version:       1.0
 *  Desc:		   Labeled example data structure 
 *  Author:        Miha Grcar
 *  Created on:    Jan-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class LabeledExample<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public class LabeledExample<LblT, ExT> : ISerializable
    {
        private LblT m_lbl;
        private ExT m_ex;

        public LabeledExample(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public LabeledExample(LblT lbl, ExT ex)
        {
            Utils.ThrowException(lbl == null ? new ArgumentNullException("Label") : null); // *** allow unlabeled examples?
            Utils.ThrowException(ex == null ? new ArgumentNullException("Example") : null);
            m_lbl = lbl;
            m_ex = ex;
        }

        public LblT Label
        {
            get { return m_lbl; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Label") : null); // *** allow unlabeled examples?
                m_lbl = value; 
            }
        }

        public ExT Example
        {
            get { return m_ex; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Example") : null);
                m_ex = value; 
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}, {1} )", m_lbl, m_ex);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteValueOrObject<LblT>(m_lbl);
            writer.WriteValueOrObject<ExT>(m_ex);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            m_lbl = reader.ReadValueOrObject<LblT>();
            m_ex = reader.ReadValueOrObject<ExT>();
        }
    }
}