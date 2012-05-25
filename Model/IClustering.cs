/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IClustering.cs
 *  Desc:    Clustering algorithm interface 
 *  Created: Aug-2009
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
