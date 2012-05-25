/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IModel.cs
 *  Desc:    Predictive model interface 
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
       |  Interface IModel<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IModel<LblT> : ISerializable
    {
        Type RequiredExampleType { get; }
        bool IsTrained { get; }
        void Train(ILabeledExampleCollection<LblT> dataset);
        Prediction<LblT> Predict(object example);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IModel<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IModel<LblT, ExT> : IModel<LblT>
    {
        void Train(ILabeledExampleCollection<LblT, ExT> dataset);
        Prediction<LblT> Predict(ExT example);
    }
}
