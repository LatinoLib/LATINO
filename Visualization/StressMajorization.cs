/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          StressMajorization.cs
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
       |  Class StressMajorization
       |
       '-----------------------------------------------------------------------
    */
    public class StressMajorization : ILayoutAlgorithm
    {
        private int m_max_steps
            = 1000;
        private double m_min_diff
            = 0.001;
        private Random m_rnd
            = new Random(1);
        private int m_num_points;
        private IDistance<int> m_dist_func;

        public StressMajorization(int num_points, IDistance<int> dist_func)
        {
            Utils.ThrowException(num_points <= 0 ? new ArgumentOutOfRangeException("num_points") : null);
            Utils.ThrowException(dist_func == null ? new ArgumentNullException("dist_func") : null);
            m_num_points = num_points;
            m_dist_func = dist_func;
        }

        public Random Random
        {
            get { return m_rnd; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Random") : null);
                m_rnd = value;
            }
        }

        public double MinDiff
        {
            get { return m_min_diff; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("MinDiff") : null);
                m_min_diff = value;
            }
        }

        public int MaxSteps
        {
            get { return m_max_steps; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("MaxSteps") : null);
                m_max_steps = value;
            }
        }

        public int NumPoints
        {
            get { return m_num_points; }
            set
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("NumPoints") : null);
                m_num_points = value;
            }
        }

        public IDistance<int> DistFunc
        {
            get { return m_dist_func; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("DistFunc") : null);
                m_dist_func = value;
            }
        }

        private void FitToBounds(Vector2D[] points)
        {
            double min_x = double.MaxValue, max_x = double.MinValue;
            double min_y = double.MaxValue, max_y = double.MinValue;
            foreach (Vector2D pt in points)
            {
                if (pt.X > max_x) { max_x = pt.X; }
                if (pt.X < min_x) { min_x = pt.X; }
                if (pt.Y > max_y) { max_y = pt.Y; }
                if (pt.Y < min_y) { min_y = pt.Y; }
            }
            double scale_x = max_x - min_x;
            double scale_y = max_y - min_y;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2D pt = points[i];
                double new_x = scale_x == 0 ? 0.5 : ((pt.X - min_x) / scale_x);
                double new_y = scale_y == 0 ? 0.5 : ((pt.Y - min_y) / scale_y);
                points[i] = new Vector2D(new_x, new_y);
            }
        }

        // *** ILayoutAlgorithm interface implementation ***

        public Vector2D[] ComputeLayout()
        {
            if (m_num_points == 1) { return new Vector2D[] { new Vector2D(0.5, 0.5) }; } // trivial case
            const double eps = 0.00001;
            Vector2D[] vert_pos = new Vector2D[m_num_points];
            // initialize layout
            for (int i = 0; i < m_num_points; i++)
            {
                vert_pos[i] = new Vector2D(m_rnd.NextDouble(), m_rnd.NextDouble());
            }
            // main optimization loop
            double global_stress = 0, stress_diff = 0;
            double old_global_stress = double.MaxValue;
            for (int step = 0; step < m_max_steps; step++)
            {
                global_stress = 0;
                for (int i = 0; i < m_num_points; i++)
                {
                    double div = 0;
                    Vector2D new_pos = new Vector2D(0, 0);
                    for (int j = 0; j < m_num_points; j++)
                    {
                        if (i != j)
                        {
                            double d_ij = m_dist_func.GetDistance(i, j);
                            if (d_ij < eps) { d_ij = eps; }
                            double w_ij = 1.0 / Math.Pow(d_ij, 2);
                            double x_i_minus_x_j = vert_pos[i].X - vert_pos[j].X;
                            double y_i_minus_y_j = vert_pos[i].Y - vert_pos[j].Y;
                            double denom = Math.Sqrt(Math.Pow(x_i_minus_x_j, 2) + Math.Pow(y_i_minus_y_j, 2));
                            if (denom < eps) { denom = eps; } // avoid dividing by zero
                            div += w_ij;
                            new_pos.X += w_ij * (vert_pos[j].X + d_ij * (x_i_minus_x_j / denom));
                            new_pos.Y += w_ij * (vert_pos[j].Y + d_ij * (y_i_minus_y_j / denom));
                            if (i < j)
                            {
                                Vector2D diff = vert_pos[i] - vert_pos[j];
                                global_stress += w_ij * Math.Pow(diff.GetLength() - d_ij, 2);
                            }
                        }
                    }
                    vert_pos[i].X = new_pos.X / div;
                    vert_pos[i].Y = new_pos.Y / div;
                }
                stress_diff = old_global_stress - global_stress;
                if ((step - 1) % 100 == 0)
                {
                    Utils.VerboseLine("Global stress: {0:0.00} Diff: {1:0.0000}", global_stress, stress_diff);
                }
                old_global_stress = global_stress;
                if (stress_diff <= m_min_diff) { break; }
            }
            Utils.VerboseLine("Final global stress: {0:0.00} Diff: {1:0.0000}", global_stress, stress_diff);
            FitToBounds(vert_pos);
            return vert_pos;
        }
    }
}
