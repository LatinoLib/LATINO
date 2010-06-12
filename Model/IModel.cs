﻿/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IModel.cs
 *  Desc:    Prediction model nterface 
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
