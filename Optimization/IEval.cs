/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IEval.cs 
 *  Desc:    Evaluation function interface
 *  Created: Oct-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace Latino.Optimization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Interface IEval
       |
       '-----------------------------------------------------------------------
    */
    public interface IEval
    {
        double Eval(ArrayList<double>.ReadOnly param);
    }
}
