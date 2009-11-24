/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          IUnlabeledExampleCollection.cs
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
       |  Interface IUnlabeledExampleCollection
       |
       '-----------------------------------------------------------------------
    */
    public interface IUnlabeledExampleCollection : IEnumerableList
    {
        Type ExampleType { get; }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IUnlabeledExampleCollection<ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IUnlabeledExampleCollection<ExT> : IUnlabeledExampleCollection, IEnumerableList<ExT>
    {
    }
}
