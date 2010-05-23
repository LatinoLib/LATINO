/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    VisualizationInterfaces.cs
 *  Desc:    Visualization-related interfaces
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System.Drawing;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Interface IDrawableObject
       |
       '-----------------------------------------------------------------------
    */
    public interface IDrawableObject
    {
        void Draw(Graphics gfx, TransformParams tr);
        void Draw(Graphics gfx, TransformParams tr, BoundingArea.ReadOnly boundingArea);
        BoundingArea GetBoundingArea(TransformParams tr);
        IDrawableObject[] GetObjectsAt(float x, float y, TransformParams tr, ref float[] distArray);
        IDrawableObject[] GetObjectsIn(BoundingArea.ReadOnly area, TransformParams tr);
    }
}