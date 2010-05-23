/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Line.cs
 *  Desc:    Drawable line
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;
using System.IO;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class Line 
       |
       '-----------------------------------------------------------------------
    */
    public class Line : DrawnObject
    {
        private float mX1;
        private float mY1;
        private float mX2;
        private float mY2;
        private static float mHitDist
            = 3;
        private static float mMaxBoxArea
            = 1000;
        public Line(float x1, float y1, float x2, float y2)
        {
            mX1 = x1;
            mY1 = y1;
            mX2 = x2;
            mY2 = y2;
        }
        public static void Draw(float x1, float y1, float x2, float y2, Graphics gfx, Pen pen, TransformParams tr)
        {
            Utils.ThrowException(gfx == null ? new ArgumentNullException("gfx") : null);
            Utils.ThrowException(pen == null ? new ArgumentNullException("pen") : null);
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);
            VectorF pt1 = tr.Transform(new VectorF(x1, y1));
            VectorF pt2 = tr.Transform(new VectorF(x2, y2));
            gfx.DrawLine(pen, pt1, pt2);
        }
        private static bool LineIntersectVertical(VectorF pt1, VectorF pt2, float x, ref float y)
        {
            if (pt1.X > pt2.X) { VectorF tmp = pt1; pt1 = pt2; pt2 = tmp; } // swap points
            if (pt1.X < x && pt2.X > x)
            {
                float dY = pt2.Y - pt1.Y;
                if (dY == 0)
                {
                    y = pt1.Y;
                    return true;
                }
                float dX = pt2.X - pt1.X;
                float dx = x - pt1.X;
                float dy = dx * dY / dX;
                y = pt1.Y + dy;
                return true;
            }
            return false;
        }
        private static bool LineIntersectHorizontal(VectorF pt1, VectorF pt2, float y, ref float x)
        {
            return LineIntersectVertical(new VectorF(pt1.Y, pt1.X), new VectorF(pt2.Y, pt2.X), y, ref x);
        }
        private static bool LineIntersectRectangle(VectorF pt1, VectorF pt2, RectangleF rect, ref VectorF isectPt1, ref VectorF isectPt2)
        {
            float y = 0, x = 0;
            ArrayList<VectorF> points = new ArrayList<VectorF>(2); 
            if (LineIntersectVertical(pt1, pt2, rect.X, ref y))
            {
                if (y > rect.Y && y < rect.Y + rect.Height) { points.Add(new VectorF(rect.X, y)); }
            }
            if (LineIntersectVertical(pt1, pt2, rect.X + rect.Width, ref y))
            {
                if (y > rect.Y && y < rect.Y + rect.Height) { points.Add(new VectorF(rect.X + rect.Width, y)); }
            }
            if (LineIntersectHorizontal(pt1, pt2, rect.Y, ref x))
            {
                if (x > rect.X && x < rect.X + rect.Width) { points.Add(new VectorF(x, rect.Y)); }
            }
            if (LineIntersectHorizontal(pt1, pt2, rect.Y + rect.Height, ref x))
            {
                if (x > rect.X && x < rect.X + rect.Width) { points.Add(new VectorF(x, rect.Y + rect.Height)); }
            }
            if (points.Count == 2)
            {
                isectPt1 = points[0];
                isectPt2 = points[1];
                return true;
            }
            else if (points.Count == 1)
            {
                isectPt1 = points[0];
                isectPt2 = VisualizationUtils.PointInsideRect(pt1.X, pt1.Y, rect) ? pt1 : pt2;
                return true;
            }
            else if (VisualizationUtils.PointInsideRect(pt1.X, pt1.Y, rect) && VisualizationUtils.PointInsideRect(pt2.X, pt2.Y, rect)) 
            {
                isectPt1 = pt1;
                isectPt2 = pt2;
                return true;
            }
            return false;
        }
        public static void Draw(float x1, float y1, float x2, float y2, Graphics gfx, Pen pen, TransformParams tr, BoundingArea.ReadOnly boundingArea)
        {
#if !NO_PARTIAL_RENDERING
            Utils.ThrowException(gfx == null ? new ArgumentNullException("gfx") : null);
            Utils.ThrowException(pen == null ? new ArgumentNullException("pen") : null);
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);
            Utils.ThrowException(boundingArea == null ? new ArgumentNullException("boundingArea") : null);
            VectorF pt1 = tr.Transform(new VectorF(x1, y1));
            VectorF pt2 = tr.Transform(new VectorF(x2, y2));
            VectorF isectPt1 = new VectorF();
            VectorF isectPt2 = new VectorF();
            BoundingArea inflatedArea = boundingArea.GetWritableCopy();
            inflatedArea.Inflate(pen.Width / 2f + 5f, pen.Width / 2f + 5f);
            ArrayList<KeyDat<float, PointInfo>> points = new ArrayList<KeyDat<float, PointInfo>>();
            foreach (RectangleF rect in inflatedArea.Rectangles)
            {
                if (LineIntersectRectangle(pt1, pt2, rect, ref isectPt1, ref isectPt2))
                {
                    float distPt1 = (pt1 - isectPt1).GetLength(); // *** don't need sqrt here
                    float distPt2 = (pt1 - isectPt2).GetLength(); // *** don't need sqrt here
                    bool startPt1 = distPt1 < distPt2;
                    points.Add(new KeyDat<float, PointInfo>(distPt1, new PointInfo(isectPt1, startPt1)));
                    points.Add(new KeyDat<float, PointInfo>(distPt2, new PointInfo(isectPt2, !startPt1)));
                }
            }
            points.Sort();
            int refCount = 0;
            int startIdx = 0;
            for (int i = 0; i < points.Count; i++)
            {
                PointInfo pointInfo = points[i].Dat;
                if (pointInfo.IsStartPoint)
                {
                    refCount++;
                }
                else
                {
                    refCount--;
                    if (refCount == 0)
                    {
                        gfx.DrawLine(pen, points[startIdx].Dat.Point, pointInfo.Point);
                        startIdx = i + 1;
                    }
                }
            }
#else
            Draw(x1, y1, x2, y2, gfx, pen, tr);
#endif
        }
        public static BoundingArea GetBoundingArea(float x1, float y1, float x2, float y2)
        {
#if !SIMPLE_BOUNDING_AREA
            if (x1 == x2 || y1 == y2) { return new BoundingArea(VisualizationUtils.CreateRectangle(x1, y1, x2, y2)); }
            float delta = Math.Abs((x2 - x1) / (y2 - y1));
            float stepMax = (float)Math.Sqrt(mMaxBoxArea / delta + delta * mMaxBoxArea);
            VectorF line = new VectorF(x1, y1, x2, y2);
            float lineLen = line.GetLength();
            if (stepMax >= lineLen) { return new BoundingArea(VisualizationUtils.CreateRectangle(x1, y1, x2, y2)); }
            BoundingArea boundingArea = new BoundingArea();
            int steps = (int)Math.Ceiling(lineLen / stepMax);
            VectorF stepVec = line;
            stepVec.SetLength(lineLen / (float)steps);
            VectorF pt1 = new VectorF(x1, y1);
            VectorF pt2;
            for (int i = 0; i < steps - 1; i++)
            {
                pt2 = pt1 + stepVec;
                boundingArea.AddRectangles(VisualizationUtils.CreateRectangle(pt1.X, pt1.Y, pt2.X, pt2.Y));
                pt1 = pt2;
            }
            pt2 = new VectorF(x2, y2);
            boundingArea.AddRectangles(VisualizationUtils.CreateRectangle(pt1.X, pt1.Y, pt2.X, pt2.Y));
            return boundingArea;
#else
            BoundingArea boundingArea = new BoundingArea();
            boundingArea.AddRectangles(VisualizationUtils.CreateRectangle(x1, y1, x2, y2));
            return boundingArea;
#endif
        }
        public static bool IsObjectAt(float ptX, float ptY, TransformParams tr, float x1, float y1, float x2, float y2, ref float dist)
        {
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);
            VectorF pt1 = tr.Transform(new VectorF(x1, y1));
            VectorF pt2 = tr.Transform(new VectorF(x2, y2));
            return VisualizationUtils.TestLineHit(new VectorF(ptX, ptY), pt1, pt2, mHitDist, ref dist);
        }
        public float X
        {
            get { return mX1; }
            set
            {
                mX1 = value;
                InvalidateBoundingArea();
            }
        }
        public float Y
        {
            get { return mY1; }
            set
            {
                mY1 = value;
                InvalidateBoundingArea();
            }
        }
        public float X2
        {
            get { return mX2; }
            set
            {
                mX2 = value;
                InvalidateBoundingArea();
            }
        }
        public float Y2
        {
            get { return mY2; }
            set
            {
                mY2 = value;
                InvalidateBoundingArea();
            }
        }
        public static float LineHitMaxDist
        {
            get { return mHitDist; }
            set 
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("LineHitMaxDist") : null);
                mHitDist = value; 
            }
        }
        public static float MaxBoxArea
        {
            get { return mMaxBoxArea; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("MaxBoxArea") : null);
                mMaxBoxArea = value;
            }
        }
        public override void Draw(Graphics gfx, TransformParams tr)
        {
            Draw(mX1, mY1, mX2, mY2, gfx, mPen, tr); // throws ArgumentNullException, ArgumentValueException
        }
        public override void Draw(Graphics gfx, TransformParams tr, BoundingArea.ReadOnly boundingArea)
        {
            Draw(mX1, mY1, mX2, mY2, gfx, mPen, tr, boundingArea); // throws ArgumentNullException, ArgumentValueException
        }
        public override IDrawableObject GetObjectAt(float x, float y, TransformParams tr, ref float dist)
        {
            return IsObjectAt(x, y, tr, mX1, mY1, mX2, mY2, ref dist) ? this : null; // throws ArgumentValueException
        }
        public override BoundingArea GetBoundingArea()
        {
            return GetBoundingArea(mX1, mY1, mX2, mY2);
        }
        /* .-----------------------------------------------------------------------
           |		 
           |  Class PointInfo 
           |
           '-----------------------------------------------------------------------
        */
        private class PointInfo
        {
            public VectorF Point;
            public bool IsStartPoint;
            public PointInfo(VectorF pt, bool isStartPt)
            {
                Point = pt;
                IsStartPoint = isStartPt;
            }
        }
    }
}