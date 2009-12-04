/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          LayoutIndex.cs
 *  Version:       1.0
 *  Desc:		   2-dimensional spatial index (k-D tree implementation)
 *  Author:        Miha Grcar
 *  Created on:    Sep-2007
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
       |  Class LayoutIndex
       |
       '-----------------------------------------------------------------------
    */
    public class LayoutIndex
    {
        private KdTreeNode m_root_node
            = null;
        private static PointComparerX m_pt_cmp_x
            = new PointComparerX();
        private static PointComparerY m_pt_cmp_y
            = new PointComparerY();
        private int m_max_pts_per_leaf
            = 1;
        private int m_num_changes
            = 0;

        public int NumChanges
        {
            get { return m_num_changes; }
        }

        public int MaxPointsPerLeaf
        {
            get { return m_max_pts_per_leaf; }
            set 
            {
                Utils.ThrowException(value < 1 ? new ArgumentOutOfRangeException("MaxPointsPerLeaf") : null);
                m_max_pts_per_leaf = value; 
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

        private double ComputeBound(ArrayList<IdxDat<Vector2D>> points, ref ArrayList<IdxDat<Vector2D>> points_right, int dim)
        {
            points.Sort(dim == 0 ? (IComparer<IdxDat<Vector2D>>)m_pt_cmp_x : (IComparer<IdxDat<Vector2D>>)m_pt_cmp_y);
            int bound_idx = (int)Math.Floor((double)points.Count / 2.0);
            double bound = GetVectCoord(points[bound_idx].Dat, dim);
            points_right = new ArrayList<IdxDat<Vector2D>>(points.Count - bound_idx);
            for (int i = bound_idx; i < points.Count; i++) { points_right.Add(points[i]); }
            points.RemoveRange(bound_idx, points_right.Count);
            return bound;
        }

        private KdTreeNode BuildSubTree(KdTreeNode parent_node, ArrayList<IdxDat<Vector2D>> points, int level)
        {
            if (points.Count <= m_max_pts_per_leaf) // terminal node
            {
                KdTreeNodeTerminal terminal_node = new KdTreeNodeTerminal(points);
                terminal_node.ParentNode = parent_node;
                return terminal_node;
            }
            else // non-terminal node
            {
                ArrayList<IdxDat<Vector2D>> points_right = null;
                double bound = ComputeBound(points, ref points_right, level % 2);
                KdTreeNodeNonTerminal node = new KdTreeNodeNonTerminal(bound);
                node.ParentNode = parent_node;
                node.LeftNode = BuildSubTree(node, points, level + 1);
                node.RightNode = BuildSubTree(node, points_right, level + 1);
                return node;
            }
        }

        // TODO: exceptions

        public void BuildIndex(IEnumerable<Vector2D> points)
        {
            ArrayList<IdxDat<Vector2D>> indexed_points = new ArrayList<IdxDat<Vector2D>>();
            int idx = 0;
            foreach (Vector2D point in points) { indexed_points.Add(new IdxDat<Vector2D>(idx++, point)); }
            m_root_node = BuildSubTree(null, indexed_points, 0);
            m_num_changes = 0;
        }

        //public bool RemovePoint(int idx, Vector2D point)
        //{
        //    ArrayList<KdTreeNode> terminal_nodes = new ArrayList<KdTreeNode>();
        //    GetTreeNodes(point, new Vector2D(), m_root_node, 0, terminal_nodes);
        //    bool success = false;
        //    foreach (KdTreeNodeTerminal terminal_node in terminal_nodes)
        //    {
        //        int old_count = terminal_node.Points.Count;
        //        terminal_node.Points.Remove(new IdxDat<Vector2D>(idx));
        //        success = old_count > terminal_node.Points.Count;
        //        if (success)
        //        {
        //            m_num_changes++;
        //            // *** the index is not updated here; it should be rebuilt manually after enough points have been removed/inserted
        //            break;
        //        }
        //    }
        //    return success;
        //}

        //public void InsertPoint(int idx, Vector2D point)
        //{
        //    ArrayList<KdTreeNode> terminal_nodes = new ArrayList<KdTreeNode>();
        //    GetTreeNodes(point, new Vector2D(), m_root_node, 0, terminal_nodes);
        //    int min = int.MaxValue;
        //    int min_idx = -1;
        //    int i = 0;
        //    foreach (KdTreeNodeTerminal terminal_node in terminal_nodes)
        //    {
        //        if (terminal_node.Points.Count < min) { min_idx = i; min = terminal_node.Points.Count; }
        //        i++;
        //    }
        //    ((KdTreeNodeTerminal)terminal_nodes[min_idx]).Points.Add(new IdxDat<Vector2D>(idx, point));            
        //    m_num_changes++;
        //    // *** the index is not updated here; it should be rebuilt manually after enough points have been removed/inserted
        //}

        private void GetTreeNodes(Vector2D ref_point, Vector2D size, KdTreeNode node, int level, ArrayList<KdTreeNode> tree_nodes) // ref_point and size define a rectangle
        {
            int dim = level % 2;
            if (node is KdTreeNodeNonTerminal)
            {
                KdTreeNodeNonTerminal non_terminal_node = (KdTreeNodeNonTerminal)node;
                double bound = non_terminal_node.Bound;
                double ref_point_coord = GetVectCoord(ref_point, dim);
                double size_coord = GetVectCoord(size, dim);
                if (bound < ref_point_coord)
                {
                    GetTreeNodes(ref_point, size, non_terminal_node.RightNode, level + 1, tree_nodes);
                }
                else if (bound > ref_point_coord + size_coord)
                {
                    GetTreeNodes(ref_point, size, non_terminal_node.LeftNode, level + 1, tree_nodes);
                }
                else
                {
                    // split the rectangle at the bound
                    Vector2D right_ref_point = ref_point;
                    Vector2D right_size = size;
                    SetVectCoord(ref size, dim, bound - ref_point_coord);
                    GetTreeNodes(ref_point, size, non_terminal_node.LeftNode, level + 1, tree_nodes);
                    double right_ref_point_coord = GetVectCoord(right_ref_point, dim);
                    double right_size_coord = GetVectCoord(right_size, dim);
                    SetVectCoord(ref right_size, dim, right_ref_point_coord + right_size_coord - bound);
                    SetVectCoord(ref right_ref_point, dim, bound);
                    GetTreeNodes(right_ref_point, right_size, non_terminal_node.RightNode, level + 1, tree_nodes);
                }
            }
            else
            {
                tree_nodes.Add(node);
            }
        }

        public ArrayList<IdxDat<Vector2D>> GetPoints(Vector2D ref_point, Vector2D size) // get points inside a rectangle
        {
            ArrayList<IdxDat<Vector2D>> ret_val = new ArrayList<IdxDat<Vector2D>>();
            ArrayList<KdTreeNode> tree_nodes = new ArrayList<KdTreeNode>();
            GetTreeNodes(ref_point, size, m_root_node, 0, tree_nodes);
            foreach (KdTreeNodeTerminal terminal_node in tree_nodes)
            {
                foreach (IdxDat<Vector2D> point in terminal_node.Points)
                {
                    bool add_point = true;
                    for (int dim = 0; dim < 2; dim++)
                    {
                        double point_coord = GetVectCoord(point.Dat, dim);
                        double ref_point_coord = GetVectCoord(ref_point, dim);
                        double size_coord = GetVectCoord(size, dim);
                        if (point_coord > ref_point_coord + size_coord || point_coord < ref_point_coord)
                        {
                            add_point = false;
                            break;
                        }
                    }
                    if (add_point) { ret_val.Add(new IdxDat<Vector2D>(point.Idx, point.Dat)); }
                }
            }
            return ret_val;
        }

        public ArrayList<IdxDat<Vector2D>> GetPoints(Vector2D center, double radius) // get points inside a circle
        {
            Vector2D radius_vec = new Vector2D(radius, radius);
            ArrayList<IdxDat<Vector2D>> ret_val = new ArrayList<IdxDat<Vector2D>>();
            ArrayList<KdTreeNode> tree_nodes = new ArrayList<KdTreeNode>();
            GetTreeNodes(center - radius_vec, 2.0 * radius_vec, m_root_node, 0, tree_nodes);
            foreach (KdTreeNodeTerminal terminal_node in tree_nodes)
            {
                foreach (IdxDat<Vector2D> point in terminal_node.Points)
                {
                    if ((point.Dat - center).GetLength() <= radius)
                    {
                        ret_val.Add(new IdxDat<Vector2D>(point.Idx, point.Dat));
                    }
                }
            }
            return ret_val;
        }

        /* .-----------------------------------------------------------------------
           |		 
           |  Class KdTreeNode
           |
           '-----------------------------------------------------------------------
        */
        private class KdTreeNode
        {
            protected KdTreeNode m_parent_node
                = null;

            public KdTreeNode ParentNode
            {
                get { return m_parent_node; }
                set { m_parent_node = value; }
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
            private double m_bound;
            private KdTreeNode m_left_node
                = null;
            private KdTreeNode m_right_node
                = null;

            public KdTreeNodeNonTerminal(double bound)
            {
                m_bound = bound;
            }

            public double Bound
            {
                get { return m_bound; }
            }

            public KdTreeNode LeftNode
            {
                get { return m_left_node; }
                set { m_left_node = value; }
            }

            public KdTreeNode RightNode
            {
                get { return m_right_node; }
                set { m_right_node = value; }
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
            private ArrayList<IdxDat<Vector2D>> m_points;

            public KdTreeNodeTerminal(ArrayList<IdxDat<Vector2D>> points)
            {
                m_points = points;
            }

            public ArrayList<IdxDat<Vector2D>> Points
            {
                get { return m_points; }
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
