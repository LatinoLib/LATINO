/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          LayoutSettings.cs
 *  Version:       1.0
 *  Desc:		   Layout settings struct
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
        private double m_width
            = 1;
        private double m_height
            = 1;
        private double m_margin_horiz
            = 0;
        private double m_margin_vert
            = 0;
        private LayoutAdjustmentType m_adjust_type
            = LayoutAdjustmentType.Exact;
        private double m_stdev_mult
            = 2.5;
        private bool m_fit_to_bounds
            = false;
        private LayoutBoundsType m_bounds_type
            = LayoutBoundsType.Rectangular;

        public LayoutSettings()
        {
        }

        public LayoutSettings(double width, double height)
        {
            Utils.ThrowException(width <= 0 ? new ArgumentOutOfRangeException("width") : null);
            Utils.ThrowException(height <= 0 ? new ArgumentOutOfRangeException("height") : null);
            m_width = width;
            m_height = height;
        }

        public double Width
        {
            get { return m_width; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("Width") : null);
                m_width = value;
            }
        }

        public double Height
        {
            get { return m_height; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("Height") : null);
                m_height = value;
            }
        }

        public double MarginHoriz
        {
            get { return m_margin_horiz; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("MarginHoriz") : null);
                m_margin_horiz = value;
            }
        }

        public double MarginVert
        {
            get { return m_margin_vert; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("MarginVert") : null);
                m_margin_vert = value;
            }
        }

        public LayoutAdjustmentType AdjustmentType
        {
            get { return m_adjust_type; }
            set { m_adjust_type = value; }
        }

        public double StdDevMult
        {
            get { return m_stdev_mult; }
            set 
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("StdDevMult") : null);
                m_stdev_mult = value;
            }
        }

        public bool FitToBounds
        {
            get { return m_fit_to_bounds; }
            set { m_fit_to_bounds = value; }
        }

        public LayoutBoundsType BoundsType
        {
            get { return m_bounds_type; }
            set { m_bounds_type = value; }
        }

        public Vector2D[] AdjustLayout(IEnumerable<Vector2D> layout)
        {
            Utils.ThrowException(layout == null ? new ArgumentNullException("layout") : null);
            int pt_count = 0;
            foreach (Vector2D pt in layout) { pt_count++; }
            if (pt_count == 0) { return new Vector2D[] { }; }
            Vector2D[] new_layout = new Vector2D[pt_count];
            if (m_adjust_type == LayoutAdjustmentType.Exact)
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
                double inner_width = m_width - 2.0 * m_margin_horiz;
                Utils.ThrowException(inner_width <= 0 ? new ArgumentOutOfRangeException("Width and/or MarginHoriz") : null);
                double inner_height = m_height - 2.0 * m_margin_vert;
                Utils.ThrowException(inner_height <= 0 ? new ArgumentOutOfRangeException("Height and/or MarginVert") : null);
                double actual_width = max.X - min.X;
                double actual_height = max.Y - min.Y;
                int i = 0;
                foreach (Vector2D pt in layout)
                {
                    double x = actual_width > 0 ? ((pt.X - min.X) / actual_width * inner_width + m_margin_horiz) : (m_width / 2.0);
                    double y = actual_height > 0 ? ((pt.Y - min.Y) / actual_height * inner_height + m_margin_vert) : (m_height / 2.0);
                    new_layout[i++] = new Vector2D(x, y);
                }
            }
            else // m_adjust_type == LayoutAdjustmentType.Soft
            {
                Vector2D avg = new Vector2D(0, 0);
                foreach (Vector2D pt in layout)
                {
                    avg.X += pt.X;
                    avg.Y += pt.Y;
                }                
                avg.X /= (double)pt_count;
                avg.Y /= (double)pt_count;
                Vector2D stdev = new Vector2D(0, 0);
                foreach (Vector2D pt in layout)
                {
                    stdev.X += (avg.X - pt.X) * (avg.X - pt.X);
                    stdev.Y += (avg.Y - pt.Y) * (avg.Y - pt.Y);
                }
                stdev.X = Math.Sqrt(stdev.X / (double)pt_count);
                stdev.Y = Math.Sqrt(stdev.Y / (double)pt_count);
                double inner_width = m_width - 2.0 * m_margin_horiz;
                Utils.ThrowException(inner_width <= 0 ? new ArgumentOutOfRangeException("Width and/or MarginHoriz") : null);
                double inner_height = m_height - 2.0 * m_margin_vert;
                Utils.ThrowException(inner_height <= 0 ? new ArgumentOutOfRangeException("Height and/or MarginVert") : null);
                double actual_width = m_stdev_mult * stdev.X * 2.0;
                double actual_height = m_stdev_mult * stdev.Y * 2.0;
                Vector2D min = new Vector2D(avg.X - m_stdev_mult * stdev.X, avg.Y - m_stdev_mult * stdev.Y);
                int i = 0;
                foreach (Vector2D pt in layout)
                {
                    double x = actual_width > 0 ? ((pt.X - min.X) / actual_width * inner_width + m_margin_horiz) : (m_width / 2.0);
                    double y = actual_height > 0 ? ((pt.Y - min.Y) / actual_height * inner_height + m_margin_vert) : (m_height / 2.0);
                    new_layout[i++] = new Vector2D(x, y);
                }                
            }
            if (m_fit_to_bounds)
            {
                if (m_bounds_type == LayoutBoundsType.Rectangular && m_adjust_type == LayoutAdjustmentType.Soft)
                {
                    Vector2D lower_right = new Vector2D(m_width - m_margin_horiz, m_height - m_margin_vert);
                    for (int i = 0; i < new_layout.Length; i++)
                    {
                        Vector2D pt = new_layout[i];
                        if (pt.X > lower_right.X) { pt.X = lower_right.X; }
                        else if (pt.X < m_margin_horiz) { pt.X = m_margin_horiz; }
                        if (pt.Y > lower_right.Y) { pt.Y = lower_right.Y; }
                        else if (pt.Y < m_margin_vert) { pt.Y = m_margin_vert; }
                        new_layout[i] = pt;
                    }
                }
                else if (m_bounds_type == LayoutBoundsType.Circular)
                {
                    Vector2D center = new Vector2D(m_width / 2.0, m_height / 2.0);
                    Vector2D size = center - new Vector2D(m_margin_horiz, m_margin_vert);
                    for (int i = 0; i < new_layout.Length; i++)
                    {
                        Vector2D pt = new_layout[i];                        
                        Vector2D pt_vec = pt - center;
                        double angle = pt_vec.GetAngle();
                        double r_bound = size.X * size.Y / Math.Sqrt(Math.Pow(size.Y * Math.Cos(angle), 2) + Math.Pow(size.X * Math.Sin(angle), 2));
                        if (pt_vec.GetLength() > r_bound)
                        {
                            pt_vec.SetLength(r_bound);
                            new_layout[i] = center + pt_vec;
                        }
                    }
                }
            }
            return new_layout;
        }

        // *** ICloneable<LayoutSettings> interface implementation ***

        public LayoutSettings Clone()
        {
            LayoutSettings clone = new LayoutSettings();
            clone.m_adjust_type = m_adjust_type;
            clone.m_bounds_type = m_bounds_type;
            clone.m_fit_to_bounds = m_fit_to_bounds;
            clone.m_height = m_height;
            clone.m_width = m_width;
            clone.m_margin_horiz = m_margin_horiz;
            clone.m_margin_vert = m_margin_vert;
            clone.m_stdev_mult = m_stdev_mult;
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
