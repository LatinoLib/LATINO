/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ILabeledDataset.cs
 *  Desc:    Labeled dataset interface
 *  Created: Aug-2007
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
