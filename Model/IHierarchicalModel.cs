/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          IHierarchicalModel.cs
 *  Version:       1.0
 *  Desc:		   Interface definition
 *  Author:        Miha Grcar
 *  Created on:    Dec-2009
 *  Last modified: Dec-2009
 *  Revision:      Dec-2009
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IHierarchicalModel
       |
       '-----------------------------------------------------------------------
    */
    public interface IHierarchicalModel : ISerializable
    {
        Type RequiredExampleType { get; }
        bool IsTrained { get; }
        void Train(IUnlabeledExampleCollection dataset, ClusteringResult hierarchy);
        Prediction<Cluster> Predict(object example);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IHierarchicalModel<ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IHierarchicalModel<ExT> : IHierarchicalModel
    {
        void Train(IUnlabeledExampleCollection<ExT> dataset, ClusteringResult hierarchy);
        Prediction<Cluster> Predict(ExT example);
    }
}
