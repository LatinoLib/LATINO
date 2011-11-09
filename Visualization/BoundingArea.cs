/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    BoundingArea.cs
 *  Desc:    Bounding area as set of rectangles
 *  Created: Mar-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;
using System.Collections.Generic;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class BoundingArea 
       |
       '-----------------------------------------------------------------------
    */
    public class BoundingArea : ICloneable<BoundingArea>
    {
        private ArrayList<RectangleF> mRects
            = new ArrayList<RectangleF>();
        private RectangleF mBoundingBox
            = RectangleF.Empty;
        public BoundingArea()
        {
        }
        public BoundingArea(float left, float top, float width, float height)
        {
            mRects = new ArrayList<RectangleF>(new RectangleF[] { new RectangleF(left, top, width, height) });
            UpdateBoundingBox();
        }
        public BoundingArea(RectangleF rect)
        {
            mRects = new ArrayList<RectangleF>(new RectangleF[] { rect });
            UpdateBoundingBox();
        }
        public BoundingArea(IEnumerable<RectangleF> rects)
        {
            mRects = new ArrayList<RectangleF>(rects); // throws ArgumentNullException
            if (mRects.Count > 0) { UpdateBoundingBox(); }
        }
        public ArrayList<RectangleF>.ReadOnly Rectangles
        {
            get { return mRects; }
        }
        public void AddRectangles(params RectangleF[] rects)
        {
            AddRectangles((IEnumerable<RectangleF>)rects); // throws ArgumentNullException
        }
        public void AddRectangles(IEnumerable<RectangleF> rects)
        {
            Utils.ThrowException(rects == null ? new ArgumentNullException("rects") : null);
            mRects.AddRange(rects);
            if (mRects.Count > 0) { UpdateBoundingBox(); }
        }
        public void Transform(TransformParams tr)
        {
            Utils.ThrowException(tr == null ? new ArgumentNullException("tr") : null);
            for (int i = 0; i < mRects.Count; i++)
            {
                mRects[i] = tr.Transform(mRects[i]);
            }
            mBoundingBox = tr.Transform(mBoundingBox);
        }
        public void Inflate(float x, float y)
        {
            Utils.ThrowException(x < 0 ? new ArgumentOutOfRangeException("x") : null);
            Utils.ThrowException(y < 0 ? new ArgumentOutOfRangeException("y") : null);
            for (int i = 0; i < mRects.Count; i++)
            {
                RectangleF rect = mRects[i];
                rect.Inflate(x, y);
                mRects[i] = rect;
            }
            mBoundingBox.Inflate(x, y);
        }
        public bool IntersectsWith(BoundingArea.ReadOnly other)
        {
            Utils.ThrowException(other == null ? new ArgumentNullException("other") : null);
            if (mBoundingBox.IntersectsWith(other.BoundingBox))
            {
                foreach (RectangleF rect in mRects)
                {
                    foreach (RectangleF otherRect in other.Rectangles)
                    {
                        if (rect.IntersectsWith(otherRect)) { return true; }
                    }
                }
            }
            return false;
        }
        public RectangleF BoundingBox
        {
            get { return mBoundingBox; }
        }
        private void UpdateBoundingBox()
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;
            foreach (RectangleF rect in mRects)
            {
                if (rect.X < minX) { minX = rect.X; }
                if (rect.X + rect.Width > maxX) { maxX = rect.X + rect.Width; }
                if (rect.Y < minY) { minY = rect.Y; }
                if (rect.Y + rect.Height > maxY) { maxY = rect.Y + rect.Height; }
            }
            mBoundingBox = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
        public void Optimize()
        {
            mRects = RTree.FullyOptimizeBoundingArea(this);
        }
        // *** ICloneable<BoundingArea> interface implementation ***
        public BoundingArea Clone()
        {
            BoundingArea clone = new BoundingArea();
            clone.mRects = mRects.Clone();
            clone.mBoundingBox = mBoundingBox;
            return clone;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
        // *** Implicit cast to a read-only adapter ***
        public static implicit operator BoundingArea.ReadOnly(BoundingArea boundingArea)
        {
            if (boundingArea == null) { return null; }
            return new BoundingArea.ReadOnly(boundingArea);
        }
        /* .-----------------------------------------------------------------------
           |		 
           |  Class BoundingArea.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<BoundingArea>
        {
            private BoundingArea mBoundingArea;
            public ReadOnly(BoundingArea boundingArea)
            {
                Utils.ThrowException(boundingArea == null ? new ArgumentNullException("boundingArea") : null);
                mBoundingArea = boundingArea;
            }
            public ArrayList<RectangleF>.ReadOnly Rectangles
            {
                get { return mBoundingArea.Rectangles; }
            }
            public bool IntersectsWith(BoundingArea.ReadOnly other)
            {
                return mBoundingArea.IntersectsWith(other);
            }
            public RectangleF BoundingBox
            {
                get { return mBoundingArea.BoundingBox; }
            }
            // *** IReadOnlyAdapter interface implementation ***
            public BoundingArea GetWritableCopy()
            {
                return mBoundingArea.Clone();
            }
            object IReadOnlyAdapter.GetWritableCopy()
            {
                return GetWritableCopy();
            }
#if PUBLIC_INNER
            public
#else
            internal
#endif
            BoundingArea Inner
            {
                get { return mBoundingArea; }
            }
        }
    }
}