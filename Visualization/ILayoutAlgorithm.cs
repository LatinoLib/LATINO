/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ILayoutAlgorithm.cs
 *  Desc:    Layout algorithm interface
 *  Created: Nov-2009
 *
 *  Author:  Miha Grcar
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
