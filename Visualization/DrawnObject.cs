/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    DrawnObject.cs
 *  Desc:    Drawable object
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;

namespace Latino.Visualization
{ 
    /* .-----------------------------------------------------------------------
       |		 
       |  Abstract class DrawnObject
       |
       '-----------------------------------------------------------------------
    */
    public abstract class DrawnObject : IDrawableObject
    {
        protected Pen mPen
            = Pens.Black;
        protected BoundingArea mBoundingArea
            = null;
        public Pen Pen
        {
            get { return mPen; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Pen") : null);
                mPen = value; 
            }
        }
        protected void InvalidateBoundingArea()
        {
            mBoundingArea = null;
        }
        // *** IDrawableObject interface implementation ***
        public virtual IDrawableObject[] GetObjectsAt(float x, float y, TransformParams tr, ref float[] distArray)
        {
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);
            float dist = 0;
            IDrawableObject drawableObject = GetObjectAt(x, y, tr, ref dist);
            if (drawableObject != null)
            {
                distArray = new float[] { dist };
                return new IDrawableObject[] { drawableObject };
            }
            else
            {
                distArray = new float[] { };
                return new IDrawableObject[] { };
            }
        }
        public virtual IDrawableObject[] GetObjectsIn(BoundingArea.ReadOnly area, TransformParams tr)
        {
            Utils.ThrowException(area == null ? new ArgumentNullException("area") : null);
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);            
            if (GetBoundingArea(tr).IntersectsWith(area)) 
            {
                return new IDrawableObject[] { this };
            }
            else
            {
                return new IDrawableObject[] { };
            }
        }
        public virtual BoundingArea GetBoundingArea(TransformParams tr)
        {
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);
            if (mBoundingArea == null) { mBoundingArea = GetBoundingArea(); }
            BoundingArea boundingArea = mBoundingArea.Clone();
            boundingArea.Transform(tr);
            lock (mPen) { boundingArea.Inflate(mPen.Width / 2f + 5f, mPen.Width / 2f + 5f); }
            return boundingArea;
        }
        public virtual void Draw(Graphics gfx, TransformParams tr, BoundingArea.ReadOnly boundingArea)
        {
            Draw(gfx, tr);
        }
        // *** The following functions need to be implemented in derived classes ***
        public abstract void Draw(Graphics gfx, TransformParams tr);
        public abstract IDrawableObject GetObjectAt(float x, float y, TransformParams tr, ref float dist); 
        public abstract BoundingArea GetBoundingArea();
    }
}