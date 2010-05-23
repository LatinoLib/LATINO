/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ILabeledDataset.cs
 *  Desc:    Interface definition
 *  Created: Aug-2007
 *
 *  Authors: Miha Grcar
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
        ILabeledDataset<LblT> ConvertDataset(Type newExType, bool move);
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
