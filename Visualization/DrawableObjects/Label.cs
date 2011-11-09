/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Label.cs
 *  Desc:    Label object
 *  Created: Jun-2010
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Latino.Visualization
{
    public class Label : FilledDrawnObject // !!!
    {
        private string mLabel;
        private Font mFont;
        private float mX;
        private float mY;
        private float mWidth;
        private float mHeight;
        
        public Label(string label, Font font, float x, float y)
        {
            // TODO: exceptions
            mLabel = label;
            mFont = font;            
            mX = x;
            mY = y;
            VisualizationUtils.MeasureString(label, font, out mWidth, out mHeight);
        }

        public string Text
        {
            get { return mLabel; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Text") : null);
                mLabel = value;
                VisualizationUtils.MeasureString(mLabel, mFont, out mWidth, out mHeight);
                InvalidateBoundingArea();
            }
        }

        public Font Font
        {
            get { return mFont; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Font") : null);
                mFont = value;
                VisualizationUtils.MeasureString(mLabel, mFont, out mWidth, out mHeight);
                InvalidateBoundingArea();
            }
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

        public override void Draw(Graphics g, TransformParams t)
        {
            Utils.ThrowException(g == null ? new ArgumentNullException("g") : null);
            Utils.ThrowException(t == null ? new ArgumentNullException("t") : null);
            Vector2DF pos = t.Transform(new Vector2DF(mX - mWidth / 2f, mY - mHeight / 2f));
            using (Font font = new Font(mFont.FontFamily, t.Transform(mFont.Size), mFont.Style))
            {
                g.DrawString(mLabel, font, mBrush, pos.X, pos.Y, StringFormat.GenericTypographic);
            }
        }

        public override IDrawableObject GetObjectAt(float x, float y, TransformParams t, ref float dist)
        {
            Utils.ThrowException(t == null ? new ArgumentNullException("t") : null);
            float width = t.Transform(mWidth);
            float height = t.Transform(mHeight);
            Vector2DF pos = t.Transform(new Vector2DF(mX - mWidth / 2f, mY - mHeight / 2f));
            return VisualizationUtils.PointInsideRect(x, y, new RectangleF(pos.X, pos.Y, width, height)) ? this : null;
        }

        public override BoundingArea GetBoundingArea()
        {
            return new BoundingArea(mX - mWidth / 2f, mY - mHeight / 2f, mWidth, mHeight);
        }
    }
}
