/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          StressMajorizationLayout.cs
 *  Version:       1.0
 *  Desc:		   Stress majorization algorithm 
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Drawing;
using Latino.Model;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Class StressMajorizationLayout
       |
       '-----------------------------------------------------------------------
    */
    public class StressMajorizationLayout : ILayoutAlgorithm
    {
        private int mMaxSteps
            = 1000;
        private double mMinDiff
            = 0.001;
        private Random mRnd
            = new Random(1);
        private int mNumPoints;
        private IDistance<int> mDistFunc;

        public StressMajorizationLayout(int numPoints, IDistance<int> distFunc)
        {
            Utils.ThrowException(numPoints <= 0 ? new ArgumentOutOfRangeException("numPoints") : null);
            Utils.ThrowException(distFunc == null ? new ArgumentNullException("distFunc") : null);
            mNumPoints = numPoints;
            mDistFunc = distFunc;
        }

        public Random Random
        {
            get { return mRnd; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Random") : null);
                mRnd = value;
            }
        }

        public double MinDiff
        {
            get { return mMinDiff; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("MinDiff") : null);
                mMinDiff = value;
            }
        }

        public int MaxSteps
        {
            get { return mMaxSteps; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("MaxSteps") : null);
                mMaxSteps = value;
            }
        }

        public int NumPoints
        {
            get { return mNumPoints; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("NumPoints") : null);
                mNumPoints = value;
            }
        }

        public IDistance<int> DistFunc
        {
            get { return mDistFunc; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("DistFunc") : null);
                mDistFunc = value;
            }
        }

        // *** ILayoutAlgorithm interface implementation ***

        public Vector2D[] ComputeLayout()
        {
            return ComputeLayout(/*settings=*/null, /*initLayout=*/null);
        }

        public Vector2D[] ComputeLayout(LayoutSettings settings)
        {
            return ComputeLayout(settings, /*initLayout=*/null);
        }

        public Vector2D[] ComputeLayout(LayoutSettings settings, Vector2D[] initLayout)
        {
            if (settings == null) { settings = new LayoutSettings(); }
            if (mNumPoints == 1) { return settings.AdjustLayout(new Vector2D[] { new Vector2D() }); } // trivial case 
            const double eps = 0.00001;
            Vector2D[] layout = new Vector2D[mNumPoints];
            // initialize layout
            if (initLayout != null)
            {
                int initLen = Math.Min(mNumPoints, initLayout.Length);
                Array.Copy(initLayout, layout, initLen);
                for (int i = initLayout.Length; i < mNumPoints; i++)
                {
                    layout[i] = new Vector2D(mRnd.NextDouble(), mRnd.NextDouble());
                }
            }
            else
            {
                for (int i = 0; i < mNumPoints; i++)
                {
                    layout[i] = new Vector2D(mRnd.NextDouble(), mRnd.NextDouble());
                }
            }
            // main optimization loop
            double globalStress = 0, stressDiff = 0;
            double oldGlobalStress = double.MaxValue;
            for (int step = 0; step < mMaxSteps; step++)
            {
                globalStress = 0;
                for (int i = 0; i < mNumPoints; i++)
                {
                    double div = 0;
                    Vector2D newPos = new Vector2D(0, 0);
                    for (int j = 0; j < mNumPoints; j++)
                    {
                        if (i != j)
                        {
                            double dIj = mDistFunc.GetDistance(i, j);
                            if (dIj < eps) { dIj = eps; }
                            double wIj = 1.0 / Math.Pow(dIj, 2);
                            double xIMinusXJ = layout[i].X - layout[j].X;
                            double yIMinusYJ = layout[i].Y - layout[j].Y;
                            double denom = Math.Sqrt(Math.Pow(xIMinusXJ, 2) + Math.Pow(yIMinusYJ, 2));
                            if (denom < eps) { denom = eps; } // avoid dividing by zero
                            div += wIj;
                            newPos.X += wIj * (layout[j].X + dIj * (xIMinusXJ / denom));
                            newPos.Y += wIj * (layout[j].Y + dIj * (yIMinusYJ / denom));
                            if (i < j)
                            {
                                Vector2D diff = layout[i] - layout[j];
                                globalStress += wIj * Math.Pow(diff.GetLength() - dIj, 2);
                            }
                        }
                    }
                    layout[i].X = newPos.X / div;
                    layout[i].Y = newPos.Y / div;
                }
                stressDiff = oldGlobalStress - globalStress;
                if ((step - 1) % 100 == 0)
                {
                    Utils.VerboseLine("Global stress: {0:0.00} Diff: {1:0.0000}", globalStress, stressDiff);
                }
                oldGlobalStress = globalStress;
                if (stressDiff <= mMinDiff) { break; }
            }
            Utils.VerboseLine("Final global stress: {0:0.00} Diff: {1:0.0000}", globalStress, stressDiff);
            return settings.AdjustLayout(layout);
        }
    }
}
