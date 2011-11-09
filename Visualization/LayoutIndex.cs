/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    LayoutIndex.cs
 *  Desc:    2-dimensional spatial index (k-D tree)
 *  Created: Sep-2007
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class LayoutIndex
       |
       '-----------------------------------------------------------------------
    */
    public class LayoutIndex
    {
        private KdTreeNode mRootNode
            = null;
        private static PointComparerX mPtCmpX
            = new PointComparerX();
        private static PointComparerY mPtCmpY
            = new PointComparerY();
        private int mMaxPtsPerLeaf
            = 1;
        private int mNumChanges
            = 0;

        public int NumChanges
        {
            get { return mNumChanges; }
        }

        public int MaxPointsPerLeaf
        {
            get { return mMaxPtsPerLeaf; }
            set 
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("MaxPointsPerLeaf") : null);
                mMaxPtsPerLeaf = value; 
            }
        }

        private void SetVectCoord(ref Vector2D vec, int dim, double val)
        {
            if (dim == 0) { vec.X = val; } else { vec.Y = val; }
        }

        private double GetVectCoord(Vector2D vec, int dim)
        {
            return dim == 0 ? vec.X : vec.Y;
        }

        private double ComputeBound(ArrayList<IdxDat<Vector2D>> points, ref ArrayList<IdxDat<Vector2D>> pointsRight, int dim)
        {
            points.Sort(dim == 0 ? (IComparer<IdxDat<Vector2D>>)mPtCmpX : (IComparer<IdxDat<Vector2D>>)mPtCmpY);
            int boundIdx = (int)Math.Floor((double)points.Count / 2.0);
            double bound = GetVectCoord(points[boundIdx].Dat, dim);
            pointsRight = new ArrayList<IdxDat<Vector2D>>(points.Count - boundIdx);
            for (int i = boundIdx; i < points.Count; i++) { pointsRight.Add(points[i]); }
            points.RemoveRange(boundIdx, pointsRight.Count);
            return bound;
        }

        private KdTreeNode BuildSubTree(KdTreeNode parentNode, ArrayList<IdxDat<Vector2D>> points, int level)
        {
            if (points.Count <= mMaxPtsPerLeaf) // terminal node
            {
                KdTreeNodeTerminal terminalNode = new KdTreeNodeTerminal(points);
                terminalNode.ParentNode = parentNode;
                return terminalNode;
            }
            else // non-terminal node
            {
                ArrayList<IdxDat<Vector2D>> pointsRight = null;
                double bound = ComputeBound(points, ref pointsRight, level % 2);
                KdTreeNodeNonTerminal node = new KdTreeNodeNonTerminal(bound);
                node.ParentNode = parentNode;
                node.LeftNode = BuildSubTree(node, points, level + 1);
                node.RightNode = BuildSubTree(node, pointsRight, level + 1);
                return node;
            }
        }

        // TODO: exceptions

        public void BuildIndex(IEnumerable<Vector2D> points)
        {
            ArrayList<IdxDat<Vector2D>> indexedPoints = new ArrayList<IdxDat<Vector2D>>();
            int idx = 0;
            foreach (Vector2D point in points) { indexedPoints.Add(new IdxDat<Vector2D>(idx++, point)); }
            mRootNode = BuildSubTree(null, indexedPoints, 0);
            mNumChanges = 0;
        }

        //public bool RemovePoint(int idx, Vector2D point)
        //{
        //    ArrayList<KdTreeNode> terminalNodes = new ArrayList<KdTreeNode>();
        //    GetTreeNodes(point, new Vector2D(), mRootNode, 0, terminalNodes);
        //    bool success = false;
        //    foreach (KdTreeNodeTerminal terminalNode in terminalNodes)
        //    {
        //        int oldCount = terminalNode.Points.Count;
        //        terminalNode.Points.Remove(new IdxDat<Vector2D>(idx));
        //        success = oldCount > terminalNode.Points.Count;
        //        if (success)
        //        {
        //            mNumChanges++;
        //            // *** the index is not updated here; it should be rebuilt manually after enough points have been removed/inserted
        //            break;
        //        }
        //    }
        //    return success;
        //}

        //public void InsertPoint(int idx, Vector2D point)
        //{
        //    ArrayList<KdTreeNode> terminalNodes = new ArrayList<KdTreeNode>();
        //    GetTreeNodes(point, new Vector2D(), mRootNode, 0, terminalNodes);
        //    int min = int.MaxValue;
        //    int minIdx = -1;
        //    int i = 0;
        //    foreach (KdTreeNodeTerminal terminalNode in terminalNodes)
        //    {
        //        if (terminalNode.Points.Count < min) { minIdx = i; min = terminalNode.Points.Count; }
        //        i++;
        //    }
        //    ((KdTreeNodeTerminal)terminalNodes[minIdx]).Points.Add(new IdxDat<Vector2D>(idx, point));            
        //    mNumChanges++;
        //    // *** the index is not updated here; it should be rebuilt manually after enough points have been removed/inserted
        //}

        private void GetTreeNodes(Vector2D refPoint, Vector2D size, KdTreeNode node, int level, ArrayList<KdTreeNode> treeNodes) // refPoint and size define a rectangle
        {
            int dim = level % 2;
            if (node is KdTreeNodeNonTerminal)
            {
                KdTreeNodeNonTerminal nonTerminalNode = (KdTreeNodeNonTerminal)node;
                double bound = nonTerminalNode.Bound;
                double refPointCoord = GetVectCoord(refPoint, dim);
                double sizeCoord = GetVectCoord(size, dim);
                if (bound < refPointCoord)
                {
                    GetTreeNodes(refPoint, size, nonTerminalNode.RightNode, level + 1, treeNodes);
                }
                else if (bound > refPointCoord + sizeCoord)
                {
                    GetTreeNodes(refPoint, size, nonTerminalNode.LeftNode, level + 1, treeNodes);
                }
                else
                {
                    // split the rectangle at the bound
                    Vector2D rightRefPoint = refPoint;
                    Vector2D rightSize = size;
                    SetVectCoord(ref size, dim, bound - refPointCoord);
                    GetTreeNodes(refPoint, size, nonTerminalNode.LeftNode, level + 1, treeNodes);
                    double rightRefPointCoord = GetVectCoord(rightRefPoint, dim);
                    double rightSizeCoord = GetVectCoord(rightSize, dim);
                    SetVectCoord(ref rightSize, dim, rightRefPointCoord + rightSizeCoord - bound);
                    SetVectCoord(ref rightRefPoint, dim, bound);
                    GetTreeNodes(rightRefPoint, rightSize, nonTerminalNode.RightNode, level + 1, treeNodes);
                }
            }
            else
            {
                treeNodes.Add(node);
            }
        }

        public ArrayList<IdxDat<Vector2D>> GetPoints(Vector2D refPoint, Vector2D size) // get points inside a rectangle
        {
            ArrayList<IdxDat<Vector2D>> retVal = new ArrayList<IdxDat<Vector2D>>();
            ArrayList<KdTreeNode> treeNodes = new ArrayList<KdTreeNode>();
            GetTreeNodes(refPoint, size, mRootNode, 0, treeNodes);
            foreach (KdTreeNodeTerminal terminalNode in treeNodes)
            {
                foreach (IdxDat<Vector2D> point in terminalNode.Points)
                {
                    bool addPoint = true;
                    for (int dim = 0; dim < 2; dim++)
                    {
                        double pointCoord = GetVectCoord(point.Dat, dim);
                        double refPointCoord = GetVectCoord(refPoint, dim);
                        double sizeCoord = GetVectCoord(size, dim);
                        if (pointCoord > refPointCoord + sizeCoord || pointCoord < refPointCoord)
                        {
                            addPoint = false;
                            break;
                        }
                    }
                    if (addPoint) { retVal.Add(new IdxDat<Vector2D>(point.Idx, point.Dat)); }
                }
            }
            return retVal;
        }

        public ArrayList<IdxDat<Vector2D>> GetPoints(Vector2D center, double radius) // get points inside a circle
        {
            Vector2D radiusVec = new Vector2D(radius, radius);
            ArrayList<IdxDat<Vector2D>> retVal = new ArrayList<IdxDat<Vector2D>>();
            ArrayList<KdTreeNode> treeNodes = new ArrayList<KdTreeNode>();
            GetTreeNodes(center - radiusVec, 2.0 * radiusVec, mRootNode, 0, treeNodes);
            foreach (KdTreeNodeTerminal terminalNode in treeNodes)
            {
                foreach (IdxDat<Vector2D> point in terminalNode.Points)
                {
                    if ((point.Dat - center).GetLength() <= radius)
                    {
                        retVal.Add(new IdxDat<Vector2D>(point.Idx, point.Dat));
                    }
                }
            }
            return retVal;
        }

        /* .-----------------------------------------------------------------------
           |		 
           |  Class KdTreeNode
           |
           '-----------------------------------------------------------------------
        */
        private class KdTreeNode
        {
            protected KdTreeNode mParentNode
                = null;

            public KdTreeNode ParentNode
            {
                get { return mParentNode; }
                set { mParentNode = value; }
            }
        }

        /* .-----------------------------------------------------------------------
           |		 
           |  Class KdTreeNodeNonTerminal
           |
           '-----------------------------------------------------------------------
        */
        private class KdTreeNodeNonTerminal : KdTreeNode
        {
            private double mBound;
            private KdTreeNode mLeftNode
                = null;
            private KdTreeNode mRightNode
                = null;

            public KdTreeNodeNonTerminal(double bound)
            {
                mBound = bound;
            }

            public double Bound
            {
                get { return mBound; }
            }

            public KdTreeNode LeftNode
            {
                get { return mLeftNode; }
                set { mLeftNode = value; }
            }

            public KdTreeNode RightNode
            {
                get { return mRightNode; }
                set { mRightNode = value; }
            }
        }

        /* .-----------------------------------------------------------------------
           |		 
           |  Class KdTreeNodeTerminal
           |
           '-----------------------------------------------------------------------
        */
        private class KdTreeNodeTerminal : KdTreeNode
        {
            private ArrayList<IdxDat<Vector2D>> mPoints;

            public KdTreeNodeTerminal(ArrayList<IdxDat<Vector2D>> points)
            {
                mPoints = points;
            }

            public ArrayList<IdxDat<Vector2D>> Points
            {
                get { return mPoints; }
            }
        }

        /* .-----------------------------------------------------------------------
           |		 
           |  Class PointComparerX
           |
           '-----------------------------------------------------------------------
        */
        private class PointComparerX : IComparer<IdxDat<Vector2D>>
        {
            public int Compare(IdxDat<Vector2D> x, IdxDat<Vector2D> y)
            {
                return x.Dat.X.CompareTo(y.Dat.X);
            }
        }

        /* .-----------------------------------------------------------------------
           |		 
           |  Class PointComparerY      
           |
           '-----------------------------------------------------------------------
        */
        private class PointComparerY : IComparer<IdxDat<Vector2D>>
        {
            public int Compare(IdxDat<Vector2D> x, IdxDat<Vector2D> y)
            {
                return x.Dat.Y.CompareTo(y.Dat.Y);
            }
        }
    }
}
