/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Vector2D.cs
 *  Version:       1.0
 *  Desc:		   Geometric vector data structure 
 *  Author:		   Miha Grcar
 *  Created on:    Mar-2008
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.Drawing;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Struct Vector2D
       |
       '-----------------------------------------------------------------------
    */
    public struct Vector2D : IEquatable<Vector2D>, ISerializable
    {
        private double m_x;
        private double m_y;

        public Vector2D(BinarySerializer reader) : this()
        {
            Load(reader); // throws ArgumentNullException
        }

        public Vector2D(double x, double y)
        {
            m_x = x;
            m_y = y;
        }

        public Vector2D(double x, double y, double x_2, double y_2) : this(x_2 - x, y_2 - y) 
        {
        }

        public Vector2D(Vector2D vec_1, Vector2D vec_2) : this(vec_2.X - vec_1.X, vec_2.Y - vec_1.Y) 
        {
        }

        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public void Set(double x, double y)
        {
            m_x = x;
            m_y = y;
        }

        public double GetLength()
        {
            return Math.Sqrt(m_x * m_x + m_y * m_y);
        }

        public void SetLength(double new_len)
        {
            double len = GetLength(); 
            Utils.ThrowException(len == 0 ? new InvalidOperationException() : null);
            m_x = m_x / len * new_len;
            m_y = m_y / len * new_len;
        }

        public Vector2D UnitVector()
        {
            Vector2D unit_vec = this;
            unit_vec.SetLength(1); // throws InvalidOperationException
            return unit_vec;
        }

        public Vector2D Normal() // returns an orthogonal clockwise-oriented vector of the same length
        {
            Utils.ThrowException(GetLength() == 0 ? new InvalidOperationException() : null);
            return new Vector2D(m_y, -m_x);
        }

        public double GetAngle() // returns a value within [0, 2PI)
        {
            Vector2D unit_vec = UnitVector(); // throws InvalidOperationException
            double angle;
            if (m_x >= 0) { angle = Math.Asin(unit_vec.Y); }
            else { angle = Math.PI - Math.Asin(unit_vec.Y); }
            if (angle < 0) { angle += 2.0 * Math.PI; }
            return angle;
        }

        public static Vector2D GetRndVec(double len, Random rnd)
        {
            Utils.ThrowException(len < 0 ? new ArgumentOutOfRangeException("len") : null);
            Utils.ThrowException(rnd == null ? new ArgumentNullException("rnd") : null);
            Vector2D rnd_vec;
            do
            {
                rnd_vec = new Vector2D(rnd.NextDouble(), rnd.NextDouble());
            }
            while (rnd_vec.GetLength() == 0);
            rnd_vec.SetLength(len);
            return rnd_vec;
        }

        public static Vector2D GetFromAngleAndLength(double angle, double len)
        {
            Utils.ThrowException(len < 0 ? new ArgumentOutOfRangeException("length") : null);
            return new Vector2D(Math.Cos(angle) * len, Math.Sin(angle) * len); 
        }

        public static bool Intersect(Vector2D A, Vector2D a, Vector2D B, Vector2D b, ref double x, ref double y, ref bool seg_intersect)
        {
            Utils.ThrowException(a.GetLength() == 0 ? new ArgumentOutOfRangeException("a") : null);
            Utils.ThrowException(b.GetLength() == 0 ? new ArgumentOutOfRangeException("b") : null);
            double x_1 = A.X;
            double y_1 = A.Y;
            double x_2 = A.X + a.X;
            double y_2 = A.Y + a.Y;
            double x_3 = B.X;
            double y_3 = B.Y;
            double x_4 = B.X + b.X;
            double y_4 = B.Y + b.Y;
            double div = (y_4 - y_3) * (x_2 - x_1) - (x_4 - x_3) * (y_2 - y_1);
            if (div == 0) { return false; } // the two vectors are parallel or coincident
            double ua = ((x_4 - x_3) * (y_1 - y_3) - (y_4 - y_3) * (x_1 - x_3)) / div;
            double ub = ((x_2 - x_1) * (y_1 - y_3) - (y_2 - y_1) * (x_1 - x_3)) / div;
            x = x_1 + ua * (x_2 - x_1);
            y = y_1 + ua * (y_2 - y_1);
            seg_intersect = ua >= 0.0 && ua <= 1.0 && ub >= 0.0 && ub <= 1.0;
            return true;
        }

        public static bool Intersect(Vector2D A, Vector2D a, Vector2D B, Vector2D b, ref double x, ref double y, ref double ua, ref double ub)
        {
            Utils.ThrowException(a.GetLength() == 0 ? new ArgumentOutOfRangeException("a") : null);
            Utils.ThrowException(b.GetLength() == 0 ? new ArgumentOutOfRangeException("b") : null);
            double x_1 = A.X;
            double y_1 = A.Y;
            double x_2 = A.X + a.X;
            double y_2 = A.Y + a.Y;
            double x_3 = B.X;
            double y_3 = B.Y;
            double x_4 = B.X + b.X;
            double y_4 = B.Y + b.Y;
            double div = (y_4 - y_3) * (x_2 - x_1) - (x_4 - x_3) * (y_2 - y_1);
            if (div == 0) { return false; } // the two vectors are parallel or coincident
            ua = ((x_4 - x_3) * (y_1 - y_3) - (y_4 - y_3) * (x_1 - x_3)) / div;
            ub = ((x_2 - x_1) * (y_1 - y_3) - (y_2 - y_1) * (x_1 - x_3)) / div;
            x = x_1 + ua * (x_2 - x_1);
            y = y_1 + ua * (y_2 - y_1);
            return true;
        }

        public static double DotProduct(Vector2D vec_1, Vector2D vec_2)
        {
            return vec_1.X * vec_2.X + vec_1.Y * vec_2.Y;
        }

        public static Vector2D operator +(Vector2D vec_1, Vector2D vec_2)
        {
            return new Vector2D(vec_1.X + vec_2.X, vec_1.Y + vec_2.Y);
        }

        public static Vector2D operator -(Vector2D vec_1, Vector2D vec_2)
        {
            return new Vector2D(vec_1.X - vec_2.X, vec_1.Y - vec_2.Y); 
        }

        public static Vector2D operator *(Vector2D vec, double scalar)
        {
            return new Vector2D(vec.X * scalar, vec.Y * scalar); 
        }

        public static Vector2D operator *(double scalar, Vector2D vec)
        {
            return vec * scalar; 
        }

        public static Vector2D operator /(Vector2D vec, double scalar)
        {
            Utils.ThrowException(scalar == 0 ? new ArgumentOutOfRangeException("scalar") : null);
            return new Vector2D(vec.X / scalar, vec.Y / scalar); 
        }

        public static Vector2D operator -(Vector2D vec)
        {
            return new Vector2D(-vec.X, -vec.Y);
        }

        public static bool operator ==(Vector2D vec_1, Vector2D vec_2)
        {
            return vec_1.m_x == vec_2.m_x && vec_1.m_y == vec_2.m_y;
        }

        public static bool operator !=(Vector2D vec_1, Vector2D vec_2)
        {
            return !(vec_1 == vec_2);
        }

        public override string ToString()
        {
            return string.Format("( {0} {1} )", m_x, m_y);
        }

        public override int GetHashCode()
        {
            return m_x.GetHashCode() ^ m_y.GetHashCode();
        }

        // *** IEquatable<Vector2D> interface implementation ***
        
        public bool Equals(Vector2D other)
        {
            return m_x == other.m_x && m_y == other.m_y;
        }

        public override bool Equals(object obj)
        {
            Utils.ThrowException(!(obj is Vector2D) ? new ArgumentTypeException("obj") : null);
            return Equals((Vector2D)obj);
        }

        // *** ISerializable interface implementation ***
        
        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            writer.WriteDouble(m_x);
            writer.WriteDouble(m_y);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            m_x = reader.ReadDouble();
            m_y = reader.ReadDouble();
        }
    }
}
