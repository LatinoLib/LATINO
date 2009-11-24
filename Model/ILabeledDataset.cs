/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          ILabeledDataset.cs
 *  Version:       1.0
 *  Desc:		   Interface definition
 *  Author:        Miha Grcar
 *  Created on:    Aug-2007
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface ILabeledDataset<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public interface ILabeledDataset<LblT> : ILabeledExampleCollection<LblT>, ISerializable
    {
        ILabeledDataset<LblT> ConvertDataset(Type new_ex_type, bool move);
        ILabeledDataset<LblT, ExT> ConvertDataset<ExT>(bool move);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface ILabeledDataset<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface ILabeledDataset<LblT, ExT> : ILabeledDataset<LblT>, ILabeledExampleCollection<LblT, ExT>
    {
    }
}
