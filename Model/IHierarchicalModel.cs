/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IHierarchicalModel.cs
 *  Desc:    Interface definition
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
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
