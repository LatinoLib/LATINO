/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          ILabeledExampleCollection.cs
 *  Version:       1.0
 *  Desc:		   Labeled example collection interface definition
 *  Author:        Miha Grcar
 *  Created on:    Aug-2007
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
