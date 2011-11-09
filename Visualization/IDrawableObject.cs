/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IDrawableObject.cs
 *  Desc:    Drawable object interface
 *  Created: Mar-2008
 *
 *  Author:  Miha Grcar
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
        void Draw(Graphics g, TransformParams t);
        void Draw(Graphics g, TransformParams t, BoundingArea.ReadOnly boundingArea);
        BoundingArea GetBoundingArea(TransformParams t);
        IDrawableObject[] GetObjectsAt(float x, float y, TransformParams t, ref float[] distArray);
        IDrawableObject[] GetObjectsIn(BoundingArea.ReadOnly area, TransformParams t);
    }
}