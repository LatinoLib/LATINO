/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IUnlabeledExampleCollection.cs
 *  Desc:    Unlabeled example collection interface
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
