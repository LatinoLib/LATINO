/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ILabeledExampleCollection.cs
 *  Desc:    Labeled example collection interface 
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
