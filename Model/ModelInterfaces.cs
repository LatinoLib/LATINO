/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Models.cs
 *  Version:       1.0
 *  Desc:		   Fundamental ML-related interfaces
 *  Author:        Miha Grcar
 *  Created on:    Aug-2007
 *  Last modified: Oct-2008
 *  Revision:      Oct-2008
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IExampleCollection<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IExampleCollection<LblT> : IEnumerableList
    {
        Type ExampleType { get; }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IExampleCollection<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IExampleCollection<LblT, ExT> : IExampleCollection<LblT>, IEnumerableList<LabeledExample<LblT, ExT>>
    {
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IDataset<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IDataset<LblT> : IExampleCollection<LblT>, ISerializable
    {
        IDataset<LblT> ConvertDataset(Type new_ex_type, bool move);
        IDataset<LblT, ExT> ConvertDataset<ExT>(bool move);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IDataset<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IDataset<LblT, ExT> : IDataset<LblT>, IExampleCollection<LblT, ExT>
    {
    }

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
        void Train(IExampleCollection<LblT> dataset);
        ClassifierResult<LblT> Classify(object example);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IModel<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IModel<LblT, ExT> : IModel<LblT>
    {
        void Train(IExampleCollection<LblT, ExT> dataset);
        ClassifierResult<LblT> Classify(ExT example);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IClustering<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IClustering<LblT>
    {
        Type RequiredExampleType { get; }
        ClusteringResult Cluster(IExampleCollection<LblT> dataset);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IClustering<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IClustering<LblT, ExT> : IClustering<LblT>
    {
        ClusteringResult Cluster(IExampleCollection<LblT, ExT> dataset);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface ISimilarity<T>
       |
       '-----------------------------------------------------------------------
    */
    public interface ISimilarity<T> : ISerializable
    {
        double GetSimilarity(T a, T b);
    }
}