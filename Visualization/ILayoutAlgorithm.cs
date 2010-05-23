/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ILayoutAlgorithm.cs
 *  Desc:    Layout algorithm interface
 *  Created: Nov-2009
 *
 *  Authors: Miha Grcar
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
