/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IUnlabeledDataset.cs
 *  Desc:    Unlabeled dataset interface
 *  Created: Nov-2009
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
