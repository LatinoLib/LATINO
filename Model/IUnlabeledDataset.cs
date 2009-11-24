/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          IUnlabeledDataset.cs
 *  Version:       1.0
 *  Desc:		   Interface definition
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
        IUnlabeledDataset ConvertDataset(Type new_ex_type, bool move);
        IUnlabeledDataset<ExT> ConvertDataset<ExT>(bool move);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IUnlabeledDataset<ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IUnlabeledDataset<ExT> : IUnlabeledDataset, IEnumerableList<ExT>
    {
    }
}
