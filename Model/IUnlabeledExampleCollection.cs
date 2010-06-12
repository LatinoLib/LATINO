/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IUnlabeledExampleCollection.cs
 *  Desc:    Unlabeled example collection interface 
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
