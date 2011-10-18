/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IOptimizer.cs 
 *  Desc:    Optimizer interface
 *  Created: Oct-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System.Collections.Generic;

namespace Latino.Optimization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Interface IOptimizer
       |
       '-----------------------------------------------------------------------
    */
    public interface IOptimizer
    {
        ArrayList<double> Optimize(double[] paramVec, IEval eval);
    }
}
