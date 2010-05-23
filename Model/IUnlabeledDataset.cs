/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IUnlabeledDataset.cs
 *  Desc:    Interface definition
 *  Created: Nov-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IUnlabeledDataset
       |
       '-----------------------------------------------------------------------
    */
    public interface IUnlabeledDataset : IUnlabeledExampleCollection, ISerializable
    {
        IUnlabeledDataset ConvertDataset(Type newExType, bool move);
        IUnlabeledDataset<ExT> ConvertDataset<ExT>(bool move);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IUnlabeledDataset<ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IUnlabeledDataset<ExT> : IUnlabeledDataset, IUnlabeledExampleCollection<ExT>
    {
    }
}
