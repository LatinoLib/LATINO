/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    VectorF.cs
 *  Desc:    Geometric vector data structure (single precision)
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Struct VectorF
       |
       '-----------------------------------------------------------------------
    */
    public struct VectorF : IEquatable<VectorF>, ISerializable
    {
        private float mX;
        private float mY;
        public VectorF(BinarySerializer reader) : this()
        {
            Load(reader); // throws ArgumentNullException
        }
        public VectorF(float x, float y)
        {
            mX = x;
            mY = y;
        }
        public VectorF(PointF pt) : this(pt.X, pt.Y) 
        {
        }
        public VectorF(float x, float y, float x2, float y2) : this(x2 - x, y2 - y) 
        {
        }
        public VectorF(VectorF vec1, VectorF vec2) : this(vec2.X - vec1.X, vec2.Y - vec1.Y) 
        {
        }
        public VectorF(PointF pt1, PointF pt2) : this(pt2.X - pt1.X, pt2.Y - pt1.Y)
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
        public VectorF UnitVector()
        {
            VectorF unitVec = this;
            unitVec.SetLength(1); // throws InvalidOperationException
            return unitVec;
        }
        public VectorF Normal() // returns an orthogonal clockwise-oriented vector of the same length
        {
            Utils.ThrowException(GetLength() == 0 ? new InvalidOperationException() : null);
            return new VectorF(mY, -mX);
        }
        public float GetAngle() // returns a value within [0, 2PI)
        {
            VectorF unitVec = UnitVector(); // throws InvalidOperationException
            float angle;
            if (mX >= 0) { angle = (float)Math.Asin(unitVec.Y); }
            else { angle = (float)(Math.PI - Math.Asin(unitVec.Y)); }
            if (angle < 0) { angle += (float)(2.0 * Math.PI); }
            return angle;
        }
        public static VectorF GetRndVec(float len, Random rnd)
        {
            Utils.ThrowException(len < 0 ? new ArgumentOutOfRangeException("len") : null);
            Utils.ThrowException(rnd == null ? new ArgumentNullException("rnd") : null);
            VectorF rndVec;
            do
            {
                rndVec = new VectorF((float)rnd.NextDouble(), (float)rnd.NextDouble());
            }
            while (rndVec.GetLength() == 0);
            rndVec.SetLength(len);
            return rndVec;
        }
        public static VectorF GetFromAngleAndLength(float angle, float len)
        {
            Utils.ThrowException(len < 0 ? new ArgumentOutOfRangeException("length") : null);
            return new VectorF((float)Math.Cos(angle) * len, (float)Math.Sin(angle) * len); 
        }
        public static bool Intersect(VectorF A, VectorF a, VectorF B, VectorF b, ref float x, ref float y, ref bool segIntersect)
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
        public static bool Intersect(VectorF A, VectorF a, VectorF B, VectorF b, ref float x, ref float y, ref float ua, ref float ub)
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
        public static float DotProduct(VectorF vec1, VectorF vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y;
        }
        public static VectorF operator +(VectorF vec1, VectorF vec2)
        {
            return new VectorF(vec1.X + vec2.X, vec1.Y + vec2.Y);
        }
        public static VectorF operator -(VectorF vec1, VectorF vec2)
        {
            return new VectorF(vec1.X - vec2.X, vec1.Y - vec2.Y); 
        }
        public static VectorF operator *(VectorF vec, float scalar)
        {
            return new VectorF(vec.X * scalar, vec.Y * scalar); 
        }
        public static VectorF operator *(float scalar, VectorF vec)
        {
            return vec * scalar; 
        }
        public static VectorF operator /(VectorF vec, float scalar)
        {
            Utils.ThrowException(scalar == 0 ? new ArgumentOutOfRangeException("scalar") : null);
            return new VectorF(vec.X / scalar, vec.Y / scalar); 
        }
        public static VectorF operator -(VectorF vec)
        {
            return new VectorF(-vec.X, -vec.Y);
        }
        public static bool operator ==(VectorF vec1, VectorF vec2)
        {
            return vec1.mX == vec2.mX && vec1.mY == vec2.mY;
        }
        public static bool operator !=(VectorF vec1, VectorF vec2)
        {
            return !(vec1 == vec2);
        }
        public static implicit operator PointF(VectorF vec)
        {
            return new PointF(vec.mX, vec.mY);
        }
        public static implicit operator VectorF(PointF pt)
        {
            return new VectorF(pt.X, pt.Y);
        }
        public override string ToString()
        {
            return string.Format("( {0} {1} )", mX, mY);
        }
        public override int GetHashCode()
        {
            return mX.GetHashCode() ^ mY.GetHashCode();
        }
        // *** IEquatable<VectorF> interface implementation ***
        public bool Equals(VectorF other)
        {
            return mX == other.mX && mY == other.mY;
        }
        public override bool Equals(object obj)
        {
            Utils.ThrowException(!(obj is VectorF) ? new ArgumentTypeException("obj") : null);
            return Equals((VectorF)obj);
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
