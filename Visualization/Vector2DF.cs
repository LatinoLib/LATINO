/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Vector2DF.cs
 *  Desc:    Geometric vector data structure (single precision)
 *  Created: Mar-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Struct Vector2DF
       |
       '-----------------------------------------------------------------------
    */
    public struct Vector2DF : IEquatable<Vector2DF>, ISerializable
    {
        private float mX;
        private float mY;
        public Vector2DF(BinarySerializer reader) : this()
        {
            Load(reader); // throws ArgumentNullException
        }
        public Vector2DF(float x, float y)
        {
            mX = x;
            mY = y;
        }
        public Vector2DF(PointF pt) : this(pt.X, pt.Y) 
        {
        }
        public Vector2DF(float x, float y, float x2, float y2) : this(x2 - x, y2 - y) 
        {
        }
        public Vector2DF(Vector2DF vec1, Vector2DF vec2) : this(vec2.X - vec1.X, vec2.Y - vec1.Y) 
        {
        }
        public Vector2DF(PointF pt1, PointF pt2) : this(pt2.X - pt1.X, pt2.Y - pt1.Y)
        {
        }
        public float X
        {
            get { return mX; }
            set { mX = value; }
        }
        public float Y
        {
            get { return mY; }
            set { mY = value; }
        }
        public void Set(float x, float y)
        {
            mX = x;
            mY = y;
        }
        public float GetLength()
        {
            return (float)Math.Sqrt(mX * mX + mY * mY);
        }
        public void SetLength(float newLen)
        {
            float len = GetLength(); 
            Utils.ThrowException(len == 0 ? new InvalidOperationException() : null);
            mX = mX / len * newLen;
            mY = mY / len * newLen;
        }
        public Vector2DF UnitVector()
        {
            Vector2DF unitVec = this;
            unitVec.SetLength(1); // throws InvalidOperationException
            return unitVec;
        }
        public Vector2DF Normal() // returns an orthogonal clockwise-oriented vector of the same length
        {
            Utils.ThrowException(GetLength() == 0 ? new InvalidOperationException() : null);
            return new Vector2DF(mY, -mX);
        }
        public float GetAngle() // returns a value within [0, 2PI)
        {
            Vector2DF unitVec = UnitVector(); // throws InvalidOperationException
            float angle;
            if (mX >= 0) { angle = (float)Math.Asin(unitVec.Y); }
            else { angle = (float)(Math.PI - Math.Asin(unitVec.Y)); }
            if (angle < 0) { angle += (float)(2.0 * Math.PI); }
            return angle;
        }
        public static Vector2DF GetRndVec(float len, Random rnd)
        {
            Utils.ThrowException(len < 0 ? new ArgumentOutOfRangeException("len") : null);
            Utils.ThrowException(rnd == null ? new ArgumentNullException("rnd") : null);
            Vector2DF rndVec;
            do
            {
                rndVec = new Vector2DF((float)rnd.NextDouble(), (float)rnd.NextDouble());
            }
            while (rndVec.GetLength() == 0);
            rndVec.SetLength(len);
            return rndVec;
        }
        public static Vector2DF GetFromAngleAndLength(float angle, float len)
        {
            Utils.ThrowException(len < 0 ? new ArgumentOutOfRangeException("length") : null);
            return new Vector2DF((float)Math.Cos(angle) * len, (float)Math.Sin(angle) * len); 
        }
        public static bool Intersect(Vector2DF A, Vector2DF a, Vector2DF B, Vector2DF b, ref float x, ref float y, ref bool segIntersect)
        {
            Utils.ThrowException(a.GetLength() == 0 ? new ArgumentOutOfRangeException("a") : null);
            Utils.ThrowException(b.GetLength() == 0 ? new ArgumentOutOfRangeException("b") : null);
            float x1 = A.X;
            float y1 = A.Y;
            float x2 = A.X + a.X;
            float y2 = A.Y + a.Y;
            float x3 = B.X;
            float y3 = B.Y;
            float x4 = B.X + b.X;
            float y4 = B.Y + b.Y;
            float div = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (div == 0) { return false; } // the two vectors are parallel or coincident
            float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / div;
            float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / div;
            x = x1 + ua * (x2 - x1);
            y = y1 + ua * (y2 - y1);
            segIntersect = ua >= 0.0 && ua <= 1.0 && ub >= 0.0 && ub <= 1.0;
            return true;
        }
        public static bool Intersect(Vector2DF A, Vector2DF a, Vector2DF B, Vector2DF b, ref float x, ref float y, ref float ua, ref float ub)
        {
            Utils.ThrowException(a.GetLength() == 0 ? new ArgumentOutOfRangeException("a") : null);
            Utils.ThrowException(b.GetLength() == 0 ? new ArgumentOutOfRangeException("b") : null);
            float x1 = A.X;
            float y1 = A.Y;
            float x2 = A.X + a.X;
            float y2 = A.Y + a.Y;
            float x3 = B.X;
            float y3 = B.Y;
            float x4 = B.X + b.X;
            float y4 = B.Y + b.Y;
            float div = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (div == 0) { return false; } // the two vectors are parallel or coincident
            ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / div;
            ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / div;
            x = x1 + ua * (x2 - x1);
            y = y1 + ua * (y2 - y1);
            return true;
        }
        public static float DotProduct(Vector2DF vec1, Vector2DF vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y;
        }
        public static Vector2DF operator +(Vector2DF vec1, Vector2DF vec2)
        {
            return new Vector2DF(vec1.X + vec2.X, vec1.Y + vec2.Y);
        }
        public static Vector2DF operator -(Vector2DF vec1, Vector2DF vec2)
        {
            return new Vector2DF(vec1.X - vec2.X, vec1.Y - vec2.Y); 
        }
        public static Vector2DF operator *(Vector2DF vec, float scalar)
        {
            return new Vector2DF(vec.X * scalar, vec.Y * scalar); 
        }
        public static Vector2DF operator *(float scalar, Vector2DF vec)
        {
            return vec * scalar; 
        }
        public static Vector2DF operator /(Vector2DF vec, float scalar)
        {
            Utils.ThrowException(scalar == 0 ? new ArgumentOutOfRangeException("scalar") : null);
            return new Vector2DF(vec.X / scalar, vec.Y / scalar); 
        }
        public static Vector2DF operator -(Vector2DF vec)
        {
            return new Vector2DF(-vec.X, -vec.Y);
        }
        public static bool operator ==(Vector2DF vec1, Vector2DF vec2)
        {
            return vec1.mX == vec2.mX && vec1.mY == vec2.mY;
        }
        public static bool operator !=(Vector2DF vec1, Vector2DF vec2)
        {
            return !(vec1 == vec2);
        }
        public static implicit operator PointF(Vector2DF vec)
        {
            return new PointF(vec.mX, vec.mY);
        }
        public static implicit operator Vector2DF(PointF pt)
        {
            return new Vector2DF(pt.X, pt.Y);
        }
        public override string ToString()
        {
            return string.Format("( {0} {1} )", mX, mY);
        }
        public override int GetHashCode()
        {
            return mX.GetHashCode() ^ mY.GetHashCode();
        }
        // *** IEquatable<Vector2DF> interface implementation ***
        public bool Equals(Vector2DF other)
        {
            return mX == other.mX && mY == other.mY;
        }
        public override bool Equals(object obj)
        {
            Utils.ThrowException(!(obj is Vector2DF) ? new ArgumentTypeException("obj") : null);
            return Equals((Vector2DF)obj);
        }
        // *** ISerializable interface implementation ***
        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            writer.WriteFloat(mX);
            writer.WriteFloat(mY);
        }
        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            mX = reader.ReadFloat();
            mY = reader.ReadFloat();
        }
    }
}
