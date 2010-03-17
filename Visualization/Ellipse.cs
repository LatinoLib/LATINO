/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Ellipse.cs
 *  Version:       1.0
 *  Desc:		   Drawable ellipse
 *  Author:        Miha Grcar
 *  Created on:    Mar-2008
 *  Last modified: May-2009
 *  Revision:      May-2009
 *
 ***************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class Ellipse 
       |
       '-----------------------------------------------------------------------
    */
    public class Ellipse : FilledDrawnObject
    {
        private float mX;
        private float mY;
        private float mRX;
        private float mRY;
        public Ellipse(float x, float y, float rX, float rY)
        {
            mX = x;
            mY = y;
            mRX = rX;
            mRY = rY;
        }
        public static void Draw(float x, float y, float rX, float rY, Graphics gfx, Pen pen, Brush brush, TransformParams tr)
        {
            Utils.ThrowException(gfx == null ? new ArgumentNullException("gfx") : null);
            Utils.ThrowException(pen == null ? new ArgumentNullException("pen") : null);
            Utils.ThrowException(brush == null ? new ArgumentNullException("brush") : null);
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);
            x -= rX;
            y -= rY;
            VectorF center = tr.Transform(new VectorF(x, y));
            float dX = tr.Transform(2f * rX);
            float dY = tr.Transform(2f * rY);
            lock (brush) { gfx.FillEllipse(brush, center.X, center.Y, dX, dY); }
            lock (pen) { gfx.DrawEllipse(pen, center.X, center.Y, dX, dY); }
        }
        public static BoundingArea GetBoundingArea(float x, float y, float rX, float rY)
        {
            float left = x - rX;
            float top = y - rY;
            float dX = 2f * rX;
            float dY = 2f * rY;
            return new BoundingArea(left, top, dX, dY);
        }
        public static bool IsObjectAt(float ptX, float ptY, TransformParams tr, float cX, float cY, float rX, float rY)
        {
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);
            VectorF center = tr.Transform(new VectorF(cX, cY));
            VectorF pt = new VectorF(ptX, ptY);
            if (pt == center) { return true; }
            float angle = (pt - center).GetAngle();
            float x = (float)Math.Cos(angle) * tr.Transform(rX);
            float y = (float)Math.Sin(angle) * tr.Transform(rY);
            float r = new VectorF(x, y).GetLength();
            return (center - pt).GetLength() <= r;            
        }
        public float X
        {
            get { return mX; }
            set 
            { 
                mX = value; 
                InvalidateBoundingArea(); 
            }
        }
        public float Y
        {
            get { return mY; }
            set 
            { 
                mY = value; 
                InvalidateBoundingArea(); 
            }
        }
        public float RadiusX
        {
            get { return mRX; }
            set 
            { 
                mRX = value; 
                InvalidateBoundingArea(); 
            }
        }
        public float RadiusY
        {
            get { return mRY; }
            set 
            { 
                mRY = value; 
                InvalidateBoundingArea(); 
            }
        }
        public override void Draw(Graphics gfx, TransformParams tr)
        {
            Draw(mX, mY, mRX, mRY, gfx, mPen, mBrush, tr); // throws ArgumentNullException, ArgumentValueException
        }
        public override IDrawableObject GetObjectAt(float x, float y, TransformParams tr, ref float dist)
        {
            dist = 0;
            return IsObjectAt(x, y, tr, mX, mY, mRX, mRY) ? this : null; // throws ArgumentValueException
        }
        public override BoundingArea GetBoundingArea()
        {
            return GetBoundingArea(mX, mY, mRX, mRY);
        }
    }
}