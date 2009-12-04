/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          VisualizationUtils.cs
 *  Version:       1.0
 *  Desc:		   Visualization-related utilities
 *  Author:        Miha Grcar
 *  Created on:    Mar-2008
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Static class VisualizationUtils
       |
       '-----------------------------------------------------------------------
    */
    public static class VisualizationUtils
    {
        // *** Misc utils ***

        public static bool TestLineHit(VectorF test_pt, VectorF line_tail, VectorF line_head, float max_dist, ref float dist)
        {
            dist = float.MaxValue;
            if (line_tail != line_head)
            {
                VectorF edge = line_head - line_tail;                     
                VectorF edge_normal = edge.Normal();
                float intrsct_x = 0, intrsct_y = 0;
                float pos_a = 0, pos_b = 0;
                VectorF.Intersect(test_pt, edge_normal, line_tail, edge, ref intrsct_x, ref intrsct_y, ref pos_a, ref pos_b);                
                if (pos_b >= 0f && pos_b <= 1f)
                {
                    VectorF dist_vec = new VectorF(intrsct_x, intrsct_y) - test_pt;
                    dist = dist_vec.GetLength();
                }
                dist = Math.Min((line_tail - test_pt).GetLength(), dist);
            }            
            dist = Math.Min((line_head - test_pt).GetLength(), dist);
            return dist <= max_dist;
        }

        public static RectangleF CreateRectangle(float x1, float y1, float x2, float y2)
        { 
            return new RectangleF(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        public static bool PointInsideRect(float x, float y, RectangleF rect)
        {
            return x >= rect.X && x <= rect.X + rect.Width && y >= rect.Y && y <= rect.Y + rect.Height;
        }

        // *** Layout utils ***

        // *** experimental
        public static Bitmap __DrawElevation__(IEnumerable<Vector2D> layout, LayoutSettings layout_settings, int matrix_rows, int matrix_cols)
        {
            double[,] elev_data = ComputeLayoutElevation(layout, layout_settings, matrix_rows, matrix_cols);
            Bitmap bmp = new Bitmap((int)Math.Ceiling(layout_settings.Width), (int)Math.Ceiling(layout_settings.Height));
            Graphics gfx = Graphics.FromImage(bmp);
            Vector2D pix_sz = new Vector2D(layout_settings.Width / (double)matrix_cols, layout_settings.Height / (double)matrix_rows);
            int row = 0;
            int old_y = 0;
            for (double y = pix_sz.Y; row < elev_data.GetLength(0); row++, y += pix_sz.Y)
            {
                int _y = (int)Math.Round(y);                
                int col = 0;
                int old_x = 0;
                for (double x = pix_sz.X; col < elev_data.GetLength(1); col++, x += pix_sz.X)
                {
                    int _x = (int)Math.Round(x);
                    Brush brush = new SolidBrush(Color.FromArgb(255, 0, 0, (int)Math.Round(255.0 * elev_data[row, col])));
                    gfx.FillRectangle(brush, old_x, old_y, _x - old_x, _y - old_y);
                    brush.Dispose();
                    old_x = _x;
                }
                old_y = _y;
            }
            return bmp;
        }

        public static double[,] ComputeLayoutElevation(IEnumerable<Vector2D> layout, LayoutSettings layout_settings, int matrix_rows, int matrix_cols)
        {
            return ComputeLayoutElevation(layout, layout_settings, matrix_rows, matrix_cols, /*sigma=*/500, /*r=*/0.1, /*normalize=*/true, /*cut=*/false, 2);
        }

        public static double[,] ComputeLayoutElevation(IEnumerable<Vector2D> layout, LayoutSettings layout_settings, int matrix_rows, int matrix_cols,
            double sigma, double r, bool normalize, bool cut, double cut_stdev_mult)
        {
            Utils.ThrowException(layout == null ? new ArgumentNullException("layout") : null);
            Utils.ThrowException(layout_settings == null ? new ArgumentNullException("layout_settings") : null);
            Utils.ThrowException(matrix_rows < 1 ? new ArgumentOutOfRangeException("matrix_rows") : null);
            Utils.ThrowException(matrix_cols < 1 ? new ArgumentOutOfRangeException("matrix_cols") : null);
            Utils.ThrowException(sigma <= 0 ? new ArgumentOutOfRangeException("sigma") : null);
            LayoutSettings nrm_layout_settings = layout_settings.Clone();
            double f_x = 1.0 / (layout_settings.Width - 2.0 * layout_settings.MarginHoriz);
            double f_y = 1.0 / (layout_settings.Height - 2.0 * layout_settings.MarginVert);
            nrm_layout_settings.Width *= f_x;
            nrm_layout_settings.MarginHoriz *= f_x;
            nrm_layout_settings.Height *= f_y;
            nrm_layout_settings.MarginVert *= f_y;
            Vector2D[] nrm_layout = nrm_layout_settings.AdjustLayout(layout);
            LayoutIndex layout_index = new LayoutIndex();
            layout_index.MaxPointsPerLeaf = 100; // *** hardcoded max points per leaf
            if (r > 0)
            {                
                layout_index.BuildIndex(nrm_layout);
            }
            double[,] z_mtx = new double[matrix_rows, matrix_cols];
            Vector2D pix_sz = new Vector2D(nrm_layout_settings.Width / (double)matrix_cols, nrm_layout_settings.Height / (double)matrix_rows);
            double max_z = 0;
            double avg_z = 0;
            int row = 0;
            for (double y = pix_sz.Y / 2.0; y < nrm_layout_settings.Height; y += pix_sz.Y, row++)
            {
                int col = 0;
                for (double x = pix_sz.X / 2.0; x < nrm_layout_settings.Width; x += pix_sz.X, col++)
                {
                    Vector2D pt_0 = new Vector2D(x, y);
                    double z = 0;
                    if (r <= 0)
                    {
                        foreach (Vector2D pt in nrm_layout)
                        {
                            double dist = (pt - pt_0).GetLength();
                            z += Math.Exp(-sigma * dist * dist);
                        }
                    }
                    else
                    {
                        foreach (IdxDat<Vector2D> pt in layout_index.GetPoints(pt_0, r))
                        {
                            double dist = (pt.Dat - pt_0).GetLength();
                            z += Math.Exp(-sigma * dist * dist);
                        }
                    }
                    z_mtx[row, col] = z;
                    if (z > max_z) { max_z = z; }
                    avg_z += z;
                }
            }
            avg_z /= (double)(matrix_rows * matrix_cols);
            if (avg_z > 0)
            {
                if (cut)
                {
                    double stdev = 0;
                    for (row = 0; row < z_mtx.GetLength(0); row++)
                    {
                        for (int col = 0; col < z_mtx.GetLength(1); col++)
                        {
                            stdev += (z_mtx[row, col] - avg_z) * (z_mtx[row, col] - avg_z);
                        }
                    }
                    stdev = Math.Sqrt(stdev / (double)(matrix_rows * matrix_cols));
                    max_z = avg_z + stdev * cut_stdev_mult;
                    for (row = 0; row < z_mtx.GetLength(0); row++)
                    {
                        for (int col = 0; col < z_mtx.GetLength(1); col++)
                        {
                            if (z_mtx[row, col] > max_z) { z_mtx[row, col] = max_z; } 
                        }
                    }
                }
                if (normalize && max_z > 0)
                {
                    for (row = 0; row < z_mtx.GetLength(0); row++)
                    {
                        for (int col = 0; col < z_mtx.GetLength(1); col++)
                        {
                            z_mtx[row, col] /= max_z;
                        }
                    }
                }
            }
            return z_mtx;
        }
    }
}