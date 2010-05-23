/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IClustering.cs
 *  Desc:    Interface definition
 *  Created: Aug-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IClustering
       |
       '-----------------------------------------------------------------------
    */
    public interface IClustering
    {
        Type RequiredExampleType { get; }
        ClusteringResult Cluster(IUnlabeledExampleCollection dataset);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IClustering<ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IClustering<ExT> : IClustering
    {
        ClusteringResult Cluster(IUnlabeledExampleCollection<ExT> dataset);
    }
}
