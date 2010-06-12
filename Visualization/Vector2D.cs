/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Vector2D.cs
 *  Desc:    Geometric vector data structure 
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Struct Vector2D
       |
       '-----------------------------------------------------------------------
    */
    public struct Vector2D : IEquatable<Vector2D>, ISerializable
    {
        private double mX;
        private double mY;

        public Vector2D(BinarySerializer reader) : this()
        {
            Load(reader); // throws ArgumentNullException
        }

        public Vector2D(double x, double y)
        {
            mX = x;
            mY = y;
        }

        public Vector2D(double x, double y, double x2, double y2) : this(x2 - x, y2 - y) 
        {
        }

        public Vector2D(Vector2D vec1, Vector2D vec2) : this(vec2.X - vec1.X, vec2.Y - vec1.Y) 
        {
        }

        public double X
        {
            get { return mX; }
            set { mX = value; }
        }

        public double Y
        {
            get { return mY; }
            set { mY = value; }
        }

        public void Set(double x, double y)
        {
            mX = x;
            mY = y;
        }

        public double GetLength()
        {
            return Math.Sqrt(mX * mX + mY * mY);
        }

        public void SetLength(double newLen)
        {
            double len = GetLength(); 
            Utils.ThrowException(len == 0 ? new InvalidOperationException() : null);
            mX = mX / len * newLen;
            mY = mY / len * newLen;
        }

        public Vector2D UnitVector()
        {
            Vector2D unitVec = this;
            unitVec.SetLength(1); // throws InvalidOperationException
            return unitVec;
        }

        public Vector2D Normal() // returns an orthogonal clockwise-oriented vector of the same length
        {
            Utils.ThrowException(GetLength() == 0 ? new InvalidOperationException() : null);
            return new Vector2D(mY, -mX);
        }

        public double GetAngle() // returns a value within [0, 2PI)
        {
            Vector2D unitVec = UnitVector(); // throws InvalidOperationException
            double angle;
            if (mX >= 0) { angle = Math.Asin(unitVec.Y); }
            else { angle = Math.PI - Math.Asin(unitVec.Y); }
            if (angle < 0) { angle += 2.0 * Math.PI; }
            return angle;
        }

        public static Vector2D GetRndVec(double len, Random rnd)
        {
            Utils.ThrowException(len < 0 ? new ArgumentOutOfRangeException("len") : null);
            Utils.ThrowException(rnd == null ? new ArgumentNullException("rnd") : null);
            Vector2D rndVec;
            do
            {
                rndVec = new Vector2D(rnd.NextDouble(), rnd.NextDouble());
            }
            while (rndVec.GetLength() == 0);
            rndVec.SetLength(len);
            return rndVec;
        }

        public static Vector2D GetFromAngleAndLength(double angle, double len)
        {
            Utils.ThrowException(len < 0 ? new ArgumentOutOfRangeException("length") : null);
            return new Vector2D(Math.Cos(angle) * len, Math.Sin(angle) * len); 
        }

        public static bool Intersect(Vector2D A, Vector2D a, Vector2D B, Vector2D b, ref double x, ref double y, ref bool segIntersect)
        {
            Utils.ThrowException(a.GetLength() == 0 ? new ArgumentOutOfRangeException("a") : null);
            Utils.ThrowException(b.GetLength() == 0 ? new ArgumentOutOfRangeException("b") : null);
            double x1 = A.X;
            double y1 = A.Y;
            double x2 = A.X + a.X;
            double y2 = A.Y + a.Y;
            double x3 = B.X;
            double y3 = B.Y;
            double x4 = B.X + b.X;
            double y4 = B.Y + b.Y;
            double div = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (div == 0) { return false; } // the two vectors are parallel or coincident
            double ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / div;
            double ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / div;
            x = x1 + ua * (x2 - x1);
            y = y1 + ua * (y2 - y1);
            segIntersect = ua >= 0.0 && ua <= 1.0 && ub >= 0.0 && ub <= 1.0;
            return true;
        }

        public static bool Intersect(Vector2D A, Vector2D a, Vector2D B, Vector2D b, ref double x, ref double y, ref double ua, ref double ub)
        {
            Utils.ThrowException(a.GetLength() == 0 ? new ArgumentOutOfRangeException("a") : null);
            Utils.ThrowException(b.GetLength() == 0 ? new ArgumentOutOfRangeException("b") : null);
            double x1 = A.X;
            double y1 = A.Y;
            double x2 = A.X + a.X;
            double y2 = A.Y + a.Y;
            double x3 = B.X;
            double y3 = B.Y;
            double x4 = B.X + b.X;
            double y4 = B.Y + b.Y;
            double div = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (div == 0) { return false; } // the two vectors are parallel or coincident
            ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / div;
            ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / div;
            x = x1 + ua * (x2 - x1);
            y = y1 + ua * (y2 - y1);
            return true;
        }

        public static double DotProduct(Vector2D vec1, Vector2D vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y;
        }

        public static Vector2D operator +(Vector2D vec1, Vector2D vec2)
        {
            return new Vector2D(vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public static Vector2D operator -(Vector2D vec1, Vector2D vec2)
        {
            return new Vector2D(vec1.X - vec2.X, vec1.Y - vec2.Y); 
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

        public static bool operator ==(Vector2D vec1, Vector2D vec2)
        {
            return vec1.mX == vec2.mX && vec1.mY == vec2.mY;
        }

        public static bool operator !=(Vector2D vec1, Vector2D vec2)
        {
            return !(vec1 == vec2);
        }

        public override string ToString()
        {
            return string.Format("( {0} {1} )", mX, mY);
        }

        public override int GetHashCode()
        {
            return mX.GetHashCode() ^ mY.GetHashCode();
        }

        // *** IEquatable<Vector2D> interface implementation ***
        
        public bool Equals(Vector2D other)
        {
            return mX == other.mX && mY == other.mY;
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
            writer.WriteDouble(mX);
            writer.WriteDouble(mY);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mX = reader.ReadDouble();
            mY = reader.ReadDouble();
        }
    }
}
