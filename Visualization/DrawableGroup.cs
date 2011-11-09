/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    DrawableGroup.cs
 *  Desc:    Group of drawable objects
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
       |  Class DrawableGroup
       |
       '-----------------------------------------------------------------------
    */
    public class DrawableGroup : IDrawableObject
    {
        private ArrayList<IDrawableObject> mDrawableObjects
            = new ArrayList<IDrawableObject>();
        public ArrayList<IDrawableObject> DrawableObjects
        {
            get { return mDrawableObjects; }
        }
        // *** IDrawableObject interface implementation ***
        public void Draw(Graphics gfx, TransformParams tr)
        {
            foreach (IDrawableObject drawableObject in mDrawableObjects)
            {
                drawableObject.Draw(gfx, tr);
            }
        }
        public void Draw(Graphics gfx, TransformParams tr, BoundingArea.ReadOnly boundingArea)
        {
            foreach (IDrawableObject drawableObject in mDrawableObjects)
            {
                if (drawableObject.GetBoundingArea(tr).IntersectsWith(boundingArea))
                {
                    drawableObject.Draw(gfx, tr, boundingArea);
                }
            }
        }
        public IDrawableObject[] GetObjectsAt(float x, float y, TransformParams tr, ref float[] distArray)
        {
            ArrayList<ObjectInfo> aux = new ArrayList<ObjectInfo>();
            for (int i = mDrawableObjects.Count - 1; i >= 0; i--)
            {
                IDrawableObject[] objectsAtXy = mDrawableObjects[i].GetObjectsAt(x, y, tr, ref distArray);
                for (int j = 0; j < objectsAtXy.Length; j++)
                {
                    aux.Add(new ObjectInfo(aux.Count, distArray[j], objectsAtXy[j]));
                }
            }
            aux.Sort();
            IDrawableObject[] result = new IDrawableObject[aux.Count];
            distArray = new float[aux.Count];
            int k = 0;
            foreach (ObjectInfo objectInfo in aux)
            {
                result[k] = objectInfo.DrawableObject;
                distArray[k++] = objectInfo.Dist;
            }            
            return result;
        }
        public IDrawableObject[] GetObjectsIn(BoundingArea.ReadOnly boundingArea, TransformParams tr)
        {
            ArrayList<IDrawableObject> result = new ArrayList<IDrawableObject>();
            for (int i = mDrawableObjects.Count - 1; i >= 0; i--)
            {
                result.AddRange(mDrawableObjects[i].GetObjectsIn(boundingArea, tr));
            }
            return result.ToArray();
        }
        public BoundingArea GetBoundingArea(TransformParams tr)
        {
            BoundingArea boundingArea = new BoundingArea();
            foreach (IDrawableObject drawableObject in mDrawableObjects)
            {
                boundingArea.AddRectangles(drawableObject.GetBoundingArea(tr).Rectangles);
            }
            // boundingArea.OptimizeArea();
            return boundingArea;
        }
        /* .-----------------------------------------------------------------------
           |		 
           |  Class ObjectInfo
           |
           '-----------------------------------------------------------------------
        */
        private class ObjectInfo : IComparable<ObjectInfo>
        {
            public float Dist;
            public int Idx;
            public IDrawableObject DrawableObject;
            public ObjectInfo(int idx, float dist, IDrawableObject drawableObject)
            {
                Idx = idx;
                Dist = dist;
                DrawableObject = drawableObject;
            }
            // *** IComparable<ObjectInfo> interface implementation ***
            public int CompareTo(ObjectInfo other)
            {
                if (Dist < other.Dist) { return -1; }
                else if (Dist > other.Dist) { return 1; }
                else if (Idx < other.Idx) { return -1; }
                else if (Idx > other.Idx) { return 1; }
                else { return 0; }
            }
        }
    }
}
