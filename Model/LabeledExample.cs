/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    LabeledExample.cs
 *  Desc:    Labeled example data structure 
 *  Created: Jan-2009
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
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
        private LblT mLbl;
        private ExT mEx;

        public LabeledExample(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public LabeledExample(LblT lbl, ExT ex)
        {
            Utils.ThrowException(lbl == null ? new ArgumentNullException("lbl") : null); 
            Utils.ThrowException(ex == null ? new ArgumentNullException("ex") : null);
            mLbl = lbl;
            mEx = ex;
        }

        public LblT Label
        {
            get { return mLbl; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Label") : null); 
                mLbl = value; 
            }
        }

        public ExT Example
        {
            get { return mEx; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Example") : null);
                mEx = value; 
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}, {1} )", mLbl, mEx);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteValueOrObject<LblT>(mLbl);
            writer.WriteValueOrObject<ExT>(mEx);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mLbl = reader.ReadValueOrObject<LblT>();
            mEx = reader.ReadValueOrObject<ExT>();
        }
    }
}
