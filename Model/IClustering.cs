/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          IClustering.cs
 *  Version:       1.0
 *  Desc:		   Interface definition
 *  Author:        Miha Grcar
 *  Created on:    Aug-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IClustering<LblT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IClustering<LblT>
    {
        Type RequiredExampleType { get; }
        ClusteringResult Cluster(ILabeledExampleCollection<LblT> dataset);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IClustering<LblT, ExT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IClustering<LblT, ExT> : IClustering<LblT>
    {
        ClusteringResult Cluster(ILabeledExampleCollection<LblT, ExT> dataset);
    }
}
