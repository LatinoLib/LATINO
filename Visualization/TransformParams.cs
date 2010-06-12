/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    TransformParams.cs
 *  Desc:    Scale-and-translate transform parameters
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
       |  Struct TransformParams
       |
       '-----------------------------------------------------------------------
    */
    public class TransformParams 
    {
        private float mTranslateX;
        private float mTranslateY;
        private float mScaleFactor;
        private static TransformParams mIdentity
            = new TransformParams(0, 0, 1);

        public TransformParams(float translateX, float translateY, float scaleFactor)
        {
            Utils.ThrowException(scaleFactor <= 0 ? new ArgumentOutOfRangeException("scaleFactor") : null);
            mTranslateX = translateX;
            mTranslateY = translateY;
            mScaleFactor = scaleFactor;
        }

        public TransformParams(float scaleFactor) : this(0, 0, scaleFactor) // throws ArgumentOutOfRangeException
        {
        }

        public TransformParams(float translateX, float translateY) : this(translateX, translateY, 1)
        {
        }

        public float TranslateX
        {
            get { return mTranslateX; }
            set { mTranslateX = value; }
        }

        public float TranslateY
        {
            get { return mTranslateY; }
            set { mTranslateY = value; }
        }

        public float ScaleFactor
        {
            get { return mScaleFactor; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("ScaleFactor") : null);
                mScaleFactor = value;
            }
        }

        public TransformParams Inverse
        {
            get
            {
                Utils.ThrowException(mScaleFactor == 0 ? new InvalidOperationException() : null);
                return new TransformParams(-mTranslateX, -mTranslateY, 1f / mScaleFactor);
            }
        }

        public static TransformParams Identity
        {
            get { return mIdentity; }
        }

        public RectangleF Transform(RectangleF rect)
        {
            // scale
            rect.X *= mScaleFactor;
            rect.Y *= mScaleFactor;
            rect.Width *= mScaleFactor;
            rect.Height *= mScaleFactor;
            // translate
            rect.X += mTranslateX;
            rect.Y += mTranslateY;
            return rect;
        }

        public Vector2DF Transform(Vector2DF vec)
        {
            // scale
            vec.X *= mScaleFactor;
            vec.Y *= mScaleFactor;
            // translate
            vec.X += mTranslateX;
            vec.Y += mTranslateY;
            return vec;
        }

        public float Transform(float len)
        {
            return len * mScaleFactor;
        }
    }
}