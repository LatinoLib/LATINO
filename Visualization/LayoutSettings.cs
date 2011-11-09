/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    LayoutSettings.cs
 *  Desc:    Layout settings struct
 *  Created: Nov-2009
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Enum LayoutBoundsType
       |
       '-----------------------------------------------------------------------
    */
    public enum LayoutBoundsType
    {
        Rectangular,
        Circular
    }

    /* .-----------------------------------------------------------------------
       |		 
       |  Enum LayoutAdjustmentType
       |
       '-----------------------------------------------------------------------
    */
    public enum LayoutAdjustmentType
    {
        Exact,
        Soft
    }

    /* .-----------------------------------------------------------------------
       |		 
       |  Class LayoutSettings 
       |
       '-----------------------------------------------------------------------
    */
    public class LayoutSettings : ICloneable<LayoutSettings>
    {
        private double mWidth
            = 1;
        private double mHeight
            = 1;
        private double mMarginHoriz
            = 0;
        private double mMarginVert
            = 0;
        private LayoutAdjustmentType mAdjustType
            = LayoutAdjustmentType.Exact;
        private double mStdevMult
            = 2.5;
        private bool mFitToBounds
            = false;
        private LayoutBoundsType mBoundsType
            = LayoutBoundsType.Rectangular;

        public LayoutSettings()
        {
        }

        public LayoutSettings(double width, double height)
        {
            Utils.ThrowException(width <= 0 ? new ArgumentOutOfRangeException("width") : null);
            Utils.ThrowException(height <= 0 ? new ArgumentOutOfRangeException("height") : null);
            mWidth = width;
            mHeight = height;
        }

        public double Width
        {
            get { return mWidth; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("Width") : null);
                mWidth = value;
            }
        }

        public double Height
        {
            get { return mHeight; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("Height") : null);
                mHeight = value;
            }
        }

        public double MarginHoriz
        {
            get { return mMarginHoriz; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("MarginHoriz") : null);
                mMarginHoriz = value;
            }
        }

        public double MarginVert
        {
            get { return mMarginVert; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("MarginVert") : null);
                mMarginVert = value;
            }
        }

        public LayoutAdjustmentType AdjustmentType
        {
            get { return mAdjustType; }
            set { mAdjustType = value; }
        }

        public double StdDevMult
        {
            get { return mStdevMult; }
            set 
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("StdDevMult") : null);
                mStdevMult = value;
            }
        }

        public bool FitToBounds
        {
            get { return mFitToBounds; }
            set { mFitToBounds = value; }
        }

        public LayoutBoundsType BoundsType
        {
            get { return mBoundsType; }
            set { mBoundsType = value; }
        }

        public Vector2D[] AdjustLayout(IEnumerable<Vector2D> layout)
        {
            Utils.ThrowException(layout == null ? new ArgumentNullException("layout") : null);
            int ptCount = 0;
            foreach (Vector2D pt in layout) { ptCount++; }
            if (ptCount == 0) { return new Vector2D[] { }; }
            Vector2D[] newLayout = new Vector2D[ptCount];
            if (mAdjustType == LayoutAdjustmentType.Exact)
            {
                Vector2D max = new Vector2D(double.MinValue, double.MinValue);
                Vector2D min = new Vector2D(double.MaxValue, double.MaxValue);            
                foreach (Vector2D pt in layout)
                {
                    if (pt.X > max.X) { max.X = pt.X; }
                    if (pt.X < min.X) { min.X = pt.X; }
                    if (pt.Y > max.Y) { max.Y = pt.Y; }
                    if (pt.Y < min.Y) { min.Y = pt.Y; }
                }
                double innerWidth = mWidth - 2.0 * mMarginHoriz;
                Utils.ThrowException(innerWidth <= 0 ? new ArgumentOutOfRangeException("Width and/or MarginHoriz") : null);
                double innerHeight = mHeight - 2.0 * mMarginVert;
                Utils.ThrowException(innerHeight <= 0 ? new ArgumentOutOfRangeException("Height and/or MarginVert") : null);
                double actualWidth = max.X - min.X;
                double actualHeight = max.Y - min.Y;
                int i = 0;
                foreach (Vector2D pt in layout)
                {
                    double x = actualWidth > 0 ? ((pt.X - min.X) / actualWidth * innerWidth + mMarginHoriz) : (mWidth / 2.0);
                    double y = actualHeight > 0 ? ((pt.Y - min.Y) / actualHeight * innerHeight + mMarginVert) : (mHeight / 2.0);
                    newLayout[i++] = new Vector2D(x, y);
                }
            }
            else // mAdjustType == LayoutAdjustmentType.Soft
            {
                Vector2D avg = new Vector2D(0, 0);
                foreach (Vector2D pt in layout)
                {
                    avg.X += pt.X;
                    avg.Y += pt.Y;
                }                
                avg.X /= (double)ptCount;
                avg.Y /= (double)ptCount;
                Vector2D stdev = new Vector2D(0, 0);
                foreach (Vector2D pt in layout)
                {
                    stdev.X += (avg.X - pt.X) * (avg.X - pt.X);
                    stdev.Y += (avg.Y - pt.Y) * (avg.Y - pt.Y);
                }
                stdev.X = Math.Sqrt(stdev.X / (double)ptCount);
                stdev.Y = Math.Sqrt(stdev.Y / (double)ptCount);
                double innerWidth = mWidth - 2.0 * mMarginHoriz;
                Utils.ThrowException(innerWidth <= 0 ? new ArgumentOutOfRangeException("Width and/or MarginHoriz") : null);
                double innerHeight = mHeight - 2.0 * mMarginVert;
                Utils.ThrowException(innerHeight <= 0 ? new ArgumentOutOfRangeException("Height and/or MarginVert") : null);
                double actualWidth = mStdevMult * stdev.X * 2.0;
                double actualHeight = mStdevMult * stdev.Y * 2.0;
                Vector2D min = new Vector2D(avg.X - mStdevMult * stdev.X, avg.Y - mStdevMult * stdev.Y);
                int i = 0;
                foreach (Vector2D pt in layout)
                {
                    double x = actualWidth > 0 ? ((pt.X - min.X) / actualWidth * innerWidth + mMarginHoriz) : (mWidth / 2.0);
                    double y = actualHeight > 0 ? ((pt.Y - min.Y) / actualHeight * innerHeight + mMarginVert) : (mHeight / 2.0);
                    newLayout[i++] = new Vector2D(x, y);
                }                
            }
            if (mFitToBounds)
            {
                if (mBoundsType == LayoutBoundsType.Rectangular && mAdjustType == LayoutAdjustmentType.Soft)
                {
                    Vector2D lowerRight = new Vector2D(mWidth - mMarginHoriz, mHeight - mMarginVert);
                    for (int i = 0; i < newLayout.Length; i++)
                    {
                        Vector2D pt = newLayout[i];
                        if (pt.X > lowerRight.X) { pt.X = lowerRight.X; }
                        else if (pt.X < mMarginHoriz) { pt.X = mMarginHoriz; }
                        if (pt.Y > lowerRight.Y) { pt.Y = lowerRight.Y; }
                        else if (pt.Y < mMarginVert) { pt.Y = mMarginVert; }
                        newLayout[i] = pt;
                    }
                }
                else if (mBoundsType == LayoutBoundsType.Circular)
                {
                    Vector2D center = new Vector2D(mWidth / 2.0, mHeight / 2.0);
                    Vector2D size = center - new Vector2D(mMarginHoriz, mMarginVert);
                    for (int i = 0; i < newLayout.Length; i++)
                    {
                        Vector2D pt = newLayout[i];                        
                        Vector2D ptVec = pt - center;
                        double angle = ptVec.GetAngle();
                        double rBound = size.X * size.Y / Math.Sqrt(Math.Pow(size.Y * Math.Cos(angle), 2) + Math.Pow(size.X * Math.Sin(angle), 2));
                        if (ptVec.GetLength() > rBound)
                        {
                            ptVec.SetLength(rBound);
                            newLayout[i] = center + ptVec;
                        }
                    }
                }
            }
            return newLayout;
        }

        // *** ICloneable<LayoutSettings> interface implementation ***

        public LayoutSettings Clone()
        {
            LayoutSettings clone = new LayoutSettings();
            clone.mAdjustType = mAdjustType;
            clone.mBoundsType = mBoundsType;
            clone.mFitToBounds = mFitToBounds;
            clone.mHeight = mHeight;
            clone.mWidth = mWidth;
            clone.mMarginHoriz = mMarginHoriz;
            clone.mMarginVert = mMarginVert;
            clone.mStdevMult = mStdevMult;
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
