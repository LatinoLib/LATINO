/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          ILayoutAlgorithm.cs
 *  Version:       1.0
 *  Desc:		   Layout algorithm interface
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface ILayoutAlgorithm
       |
       '-----------------------------------------------------------------------
    */
    interface ILayoutAlgorithm
    {
        Vector2D[] ComputeLayout(LayoutSettings settings);
    }
}
