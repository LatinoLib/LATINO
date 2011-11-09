/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Picture.cs
 *  Desc:    Picture object
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
    public class Picture : DrawnObject // change this to IDrawableObject
    {
        private Image mImage;
        private float mX;
        private float mY;

        public Picture(Image image, float x, float y)
        {
            // TODO: exceptions
            mImage = image;
            mX = x;
            mY = y;
        }

        public static void Draw(Image image, float x, float y, Graphics g, TransformParams t)
        {
            Utils.ThrowException(g == null ? new ArgumentNullException("g") : null);
            Utils.ThrowException(t == null ? new ArgumentNullException("t") : null);
            // TODO: throw other exceptions
            float width = t.Transform(image.Width);
            float height = t.Transform(image.Height);
            Vector2DF pos = t.Transform(new Vector2DF(x, y));
            PixelOffsetMode pixelOffsetMode = g.PixelOffsetMode;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.DrawImage(image, pos.X, pos.Y, width, height);
            g.PixelOffsetMode = pixelOffsetMode;
        }

        public static void Draw(Image image, float x, float y, Graphics g, TransformParams t, BoundingArea.ReadOnly boundingArea)
        {
//#if !NO_PARTIAL_RENDERING
//            Utils.ThrowException(g == null ? new ArgumentNullException("g") : null);
//            Utils.ThrowException(t == null ? new ArgumentNullException("t") : null);
//            // TODO: throw other exceptions
//            float width = t.Transform(image.Width);
//            float height = t.Transform(image.Height);
//            Vector2DF pos = t.Transform(new Vector2DF(x, y));
//            BoundingArea inflatedArea = boundingArea.GetWritableCopy();
//            inflatedArea.Inflate(5f, 5f);
//            // *** the following code is using relatively slow GDI+ clipping :-/
//            GraphicsPath gPath = new GraphicsPath();
//            foreach (RectangleF rect in inflatedArea.Rectangles)
//            {
//                gPath.AddRectangle(rect);
//            }
//            g.SetClip(gPath, CombineMode.Union);
//            PixelOffsetMode pixelOffsetMode = g.PixelOffsetMode;
//            g.PixelOffsetMode = PixelOffsetMode.Half;
//            g.DrawImage(image, pos.X, pos.Y, width, height);
//            g.ResetClip();
//            g.PixelOffsetMode = pixelOffsetMode;
//#else
            Draw(image, x, y, g, t);
//#endif
        }

        public static BoundingArea GetBoundingArea(Image image, float x, float y)
        {
            return new BoundingArea(x, y, image.Width, image.Height);
        }

        public static bool IsObjectAt(Image image, float x, float y, float ptX, float ptY, TransformParams t)
        {
            Utils.ThrowException(t == null ? new ArgumentNullException("t") : null);
            // TODO: other exceptions
            float width = t.Transform(image.Width);
            float height = t.Transform(image.Height);
            Vector2DF pos = t.Transform(new Vector2DF(x, y));
            return VisualizationUtils.PointInsideRect(ptX, ptY, new RectangleF(pos.X, pos.Y, width, height));
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

        // TODO: Image

        public override void Draw(Graphics g, TransformParams t)
        {
            Draw(mImage, mX, mY, g, t); // throws ArgumentNullException
        }

        public override void Draw(Graphics g, TransformParams t, BoundingArea.ReadOnly boundingArea)
        {
            Draw(mImage, mX, mY, g, t, boundingArea); // throws ArgumentNullException
        }

        public override IDrawableObject GetObjectAt(float x, float y, TransformParams t, ref float dist)
        {
            dist = 0;
            return IsObjectAt(mImage, mX, mY, x, y, t) ? this : null; // throws ArgumentNullException
        }

        public override BoundingArea GetBoundingArea()
        {
            return GetBoundingArea(mImage, mX, mY);
        }
    }
}
