/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ILabeledExampleCollection.cs
 *  Desc:    Interface definition
 *  Created: Aug-2007
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
       |  Interface ILabeledExampleCollection<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public interface ILabeledExampleCollection<LblT> : IEnumerableList
    {
        Type ExampleType { get; }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface ILabeledExampleCollection<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface ILabeledExampleCollection<LblT, ExT> : ILabeledExampleCollection<LblT>, IEnumerableList<LabeledExample<LblT, ExT>>
    {
    }
}
