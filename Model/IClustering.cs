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
