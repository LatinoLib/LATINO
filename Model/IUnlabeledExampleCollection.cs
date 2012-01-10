/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IUnlabeledExampleCollection.cs
 *  Desc:    Interface definition
 *  Created: Nov-2009
 *
 *  Author:  Miha Grcar
 *
 *  License: GNU LGPL (http://www.gnu.org/licenses/lgpl.txt)
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
