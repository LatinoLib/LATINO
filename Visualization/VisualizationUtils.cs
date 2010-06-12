/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    VisualizationUtils.cs
 *  Desc:    Visualization-related utilities
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class VisualizationUtils
       |
       '-----------------------------------------------------------------------
    */
    public static class VisualizationUtils
    {
        // *** Misc utils ***

        public static bool TestLineHit(Vector2DF testPt, Vector2DF lineTail, Vector2DF lineHead, float maxDist, ref float dist)
        {
            dist = float.MaxValue;
            if (lineTail != lineHead)
            {
                Vector2DF edge = lineHead - lineTail;                     
                Vector2DF edgeNormal = edge.Normal();
                float intrsctX = 0, intrsctY = 0;
                float posA = 0, posB = 0;
                Vector2DF.Intersect(testPt, edgeNormal, lineTail, edge, ref intrsctX, ref intrsctY, ref posA, ref posB);                
                if (posB >= 0f && posB <= 1f)
                {
                    Vector2DF distVec = new Vector2DF(intrsctX, intrsctY) - testPt;
                    dist = distVec.GetLength();
                }
                dist = Math.Min((lineTail - testPt).GetLength(), dist);
            }            
            dist = Math.Min((lineHead - testPt).GetLength(), dist);
            return dist <= maxDist;
        }

        public static RectangleF CreateRectangle(float x1, float y1, float x2, float y2)
        { 
            return new RectangleF(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        public static bool PointInsideRect(float x, float y, RectangleF rect)
        {
            return x >= rect.X && x <= rect.X + rect.Width && y >= rect.Y && y <= rect.Y + rect.Height;
        }

        public static void MeasureString(string str, Font font, out float width, out float height)
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    StringFormat strFmt = (StringFormat)StringFormat.GenericTypographic.Clone();
                    strFmt.FormatFlags = StringFormatFlags.NoClip;
                    strFmt.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, str.Length) });
                    Region[] strSz = g.MeasureCharacterRanges(str, font, RectangleF.Empty, strFmt);
                    width = strSz[0].GetBounds(g).Width;
                    height = strSz[0].GetBounds(g).Height;                    
                }
            }
        }

        // *** Layout utils ***

        // *** experimental
        public static Bitmap __DrawElevation__(IEnumerable<Vector2D> layout, LayoutSettings layoutSettings, int matrixRows, int matrixCols)
        {
            double[,] elevData = ComputeLayoutElevation(layout, layoutSettings, matrixRows, matrixCols);
            Bitmap bmp = new Bitmap((int)Math.Ceiling(layoutSettings.Width), (int)Math.Ceiling(layoutSettings.Height));
            Graphics gfx = Graphics.FromImage(bmp);
            Vector2D pixSz = new Vector2D(layoutSettings.Width / (double)matrixCols, layoutSettings.Height / (double)matrixRows);
            int row = 0;
            int oldY = 0;
            for (double y = pixSz.Y; row < elevData.GetLength(0); row++, y += pixSz.Y)
            {
                int _y = (int)Math.Round(y);                
                int col = 0;
                int oldX = 0;
                for (double x = pixSz.X; col < elevData.GetLength(1); col++, x += pixSz.X)
                {
                    int _x = (int)Math.Round(x);
                    Brush brush = new SolidBrush(Color.FromArgb(255, 0, 0, (int)Math.Round(255.0 * elevData[row, col])));
                    gfx.FillRectangle(brush, oldX, oldY, _x - oldX, _y - oldY);
                    brush.Dispose();
                    oldX = _x;
                }
                oldY = _y;
            }
            return bmp;
        }

        public static double[,] ComputeLayoutElevation(IEnumerable<Vector2D> layout, LayoutSettings layoutSettings, int matrixRows, int matrixCols)
        {
            return ComputeLayoutElevation(layout, layoutSettings, matrixRows, matrixCols, /*sigma=*/500, /*r=*/0.1, /*normalize=*/true, /*cut=*/false, 2);
        }

        public static double[,] ComputeLayoutElevation(IEnumerable<Vector2D> layout, LayoutSettings layoutSettings, int matrixRows, int matrixCols,
            double sigma, double r, bool normalize, bool cut, double cutStdevMult)
        {
            Utils.ThrowException(layout == null ? new ArgumentNullException("layout") : null);
            Utils.ThrowException(layoutSettings == null ? new ArgumentNullException("layoutSettings") : null);
            Utils.ThrowException(matrixRows < 1 ? new ArgumentOutOfRangeException("matrixRows") : null);
            Utils.ThrowException(matrixCols < 1 ? new ArgumentOutOfRangeException("matrixCols") : null);
            Utils.ThrowException(sigma <= 0 ? new ArgumentOutOfRangeException("sigma") : null);
            LayoutSettings nrmLayoutSettings = layoutSettings.Clone();
            double fX = 1.0 / (layoutSettings.Width - 2.0 * layoutSettings.MarginHoriz);
            double fY = 1.0 / (layoutSettings.Height - 2.0 * layoutSettings.MarginVert);
            nrmLayoutSettings.Width *= fX;
            nrmLayoutSettings.MarginHoriz *= fX;
            nrmLayoutSettings.Height *= fY;
            nrmLayoutSettings.MarginVert *= fY;
            Vector2D[] nrmLayout = nrmLayoutSettings.AdjustLayout(layout);
            LayoutIndex layoutIndex = new LayoutIndex();
            layoutIndex.MaxPointsPerLeaf = 100; // *** hardcoded max points per leaf
            if (r > 0)
            {                
                layoutIndex.BuildIndex(nrmLayout);
            }
            double[,] zMtx = new double[matrixRows, matrixCols];
            Vector2D pixSz = new Vector2D(nrmLayoutSettings.Width / (double)matrixCols, nrmLayoutSettings.Height / (double)matrixRows);
            double maxZ = 0;
            double avgZ = 0;
            int row = 0;
            for (double y = pixSz.Y / 2.0; y < nrmLayoutSettings.Height; y += pixSz.Y, row++)
            {
                int col = 0;
                for (double x = pixSz.X / 2.0; x < nrmLayoutSettings.Width; x += pixSz.X, col++)
                {
                    Vector2D pt0 = new Vector2D(x, y);
                    double z = 0;
                    if (r <= 0)
                    {
                        foreach (Vector2D pt in nrmLayout)
                        {
                            double dist = (pt - pt0).GetLength();
                            z += Math.Exp(-sigma * dist * dist);
                        }
                    }
                    else
                    {
                        foreach (IdxDat<Vector2D> pt in layoutIndex.GetPoints(pt0, r))
                        {
                            double dist = (pt.Dat - pt0).GetLength();
                            z += Math.Exp(-sigma * dist * dist);
                        }
                    }
                    zMtx[row, col] = z;
                    if (z > maxZ) { maxZ = z; }
                    avgZ += z;
                }
            }
            avgZ /= (double)(matrixRows * matrixCols);
            if (avgZ > 0)
            {
                if (cut)
                {
                    double stdev = 0;
                    for (row = 0; row < zMtx.GetLength(0); row++)
                    {
                        for (int col = 0; col < zMtx.GetLength(1); col++)
                        {
                            stdev += (zMtx[row, col] - avgZ) * (zMtx[row, col] - avgZ);
                        }
                    }
                    stdev = Math.Sqrt(stdev / (double)(matrixRows * matrixCols));
                    maxZ = avgZ + stdev * cutStdevMult;
                    for (row = 0; row < zMtx.GetLength(0); row++)
                    {
                        for (int col = 0; col < zMtx.GetLength(1); col++)
                        {
                            if (zMtx[row, col] > maxZ) { zMtx[row, col] = maxZ; } 
                        }
                    }
                }
                if (normalize && maxZ > 0)
                {
                    for (row = 0; row < zMtx.GetLength(0); row++)
                    {
                        for (int col = 0; col < zMtx.GetLength(1); col++)
                        {
                            zMtx[row, col] /= maxZ;
                        }
                    }
                }
            }
            return zMtx;
        }
    }
}