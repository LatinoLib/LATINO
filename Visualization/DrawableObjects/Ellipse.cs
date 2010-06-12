/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Ellipse.cs
 *  Desc:    Ellipse object
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

        public static void Draw(float x, float y, float rX, float rY, Graphics g, Pen pen, Brush brush, TransformParams t)
        {
            Utils.ThrowException(g == null ? new ArgumentNullException("g") : null);
            Utils.ThrowException(pen == null ? new ArgumentNullException("pen") : null);
            Utils.ThrowException(brush == null ? new ArgumentNullException("brush") : null);
            Utils.ThrowException(t == null ? new ArgumentNullException("t") : null);
            x -= rX;
            y -= rY;
            Vector2DF center = t.Transform(new Vector2DF(x, y));
            float dX = t.Transform(2f * rX);
            float dY = t.Transform(2f * rY);
            g.FillEllipse(brush, center.X, center.Y, dX, dY); 
            g.DrawEllipse(pen, center.X, center.Y, dX, dY); 
        }

        public static BoundingArea GetBoundingArea(float x, float y, float rX, float rY)
        {
            float left = x - rX;
            float top = y - rY;
            float dX = 2f * rX;
            float dY = 2f * rY;
            return new BoundingArea(left, top, dX, dY);
        }

        public static bool IsObjectAt(float ptX, float ptY, TransformParams t, float cX, float cY, float rX, float rY)
        {
            Utils.ThrowException(t == null ? new ArgumentNullException("t") : null);
            Vector2DF center = t.Transform(new Vector2DF(cX, cY));
            Vector2DF pt = new Vector2DF(ptX, ptY);
            if (pt == center) { return true; }
            float angle = (pt - center).GetAngle();
            float x = (float)Math.Cos(angle) * t.Transform(rX);
            float y = (float)Math.Sin(angle) * t.Transform(rY);
            float r = new Vector2DF(x, y).GetLength();
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

        public override void Draw(Graphics g, TransformParams t)
        {
            Draw(mX, mY, mRX, mRY, g, mPen, mBrush, t); // throws ArgumentNullException
        }

        public override IDrawableObject GetObjectAt(float x, float y, TransformParams t, ref float dist)
        {
            dist = 0;
            return IsObjectAt(x, y, t, mX, mY, mRX, mRY) ? this : null; // throws ArgumentNullException
        }

        public override BoundingArea GetBoundingArea()
        {
            return GetBoundingArea(mX, mY, mRX, mRY);
        }
    }
}