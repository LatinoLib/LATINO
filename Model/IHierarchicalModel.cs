/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IHierarchicalModel.cs
 *  Desc:    Hierarchical classifier interface
 *  Created: Dec-2009
 *
 *  Author:  Miha Grcar
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
