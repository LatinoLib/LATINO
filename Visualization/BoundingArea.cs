/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    BoundingArea.cs
 *  Desc:    Bounding area as set of rectangles
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;
using System.Collections.Generic;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class BoundingArea 
       |
       '-----------------------------------------------------------------------
    */
    public class BoundingArea : ICloneable<BoundingArea>
    {
        private ArrayList<RectangleF> mRects
            = new ArrayList<RectangleF>();
        private RectangleF mBoundingBox
            = RectangleF.Empty;
        public BoundingArea()
        {
        }
        public BoundingArea(float left, float top, float width, float height)
        {
            mRects = new ArrayList<RectangleF>(new RectangleF[] { new RectangleF(left, top, width, height) });
            UpdateBoundingBox();
        }
        public BoundingArea(RectangleF rect) 
        {
            mRects = new ArrayList<RectangleF>(new RectangleF[] { rect });
            UpdateBoundingBox();
        }
        public BoundingArea(IEnumerable<RectangleF> rects) 
        {
            mRects = new ArrayList<RectangleF>(rects); // throws ArgumentNullException
            if (mRects.Count > 0) { UpdateBoundingBox(); }
        }
        public ArrayList<RectangleF>.ReadOnly Rectangles
        {
            get { return mRects; }
        }
        public void AddRectangles(params RectangleF[] rects)
        {
            AddRectangles((IEnumerable<RectangleF>)rects); // throws ArgumentNullException
        }
        public void AddRectangles(IEnumerable<RectangleF> rects)
        {
            Utils.ThrowException(rects == null ? new ArgumentNullException("rects") : null);
            mRects.AddRange(rects);
            if (mRects.Count > 0) { UpdateBoundingBox(); }
        }
        public void Transform(TransformParams tr)
        {
            Utils.ThrowException(tr.NotSet ? new ArgumentValueException("tr") : null);
            for (int i = 0; i < mRects.Count; i++)
            {
                mRects[i] = tr.Transform(mRects[i]);
            }
            mBoundingBox = tr.Transform(mBoundingBox);
        }
        public void Inflate(float x, float y)
        {
            Utils.ThrowException(x < 0 ? new ArgumentOutOfRangeException("x") : null);
            Utils.ThrowException(y < 0 ? new ArgumentOutOfRangeException("y") : null);
            for (int i = 0; i < mRects.Count; i++)
            {
                RectangleF rect = mRects[i];
                rect.Inflate(x, y);
                mRects[i] = rect;
            }
            mBoundingBox.Inflate(x, y);
        }
        public bool IntersectsWith(BoundingArea.ReadOnly other) 
        {
            Utils.ThrowException(other == null ? new ArgumentNullException("other") : null);
            if (mBoundingBox.IntersectsWith(other.BoundingBox))
            {
                foreach (RectangleF rect in mRects)
                {
                    foreach (RectangleF otherRect in other.Rectangles)
                    {
                        if (rect.IntersectsWith(otherRect)) { return true; }
                    }
                }
            }
            return false;
        }
        public RectangleF BoundingBox
        {
            get { return mBoundingBox; }
        }
        private void UpdateBoundingBox() 
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;
            foreach (RectangleF rect in mRects)
            {
                if (rect.X < minX) { minX = rect.X; }
                if (rect.X + rect.Width > maxX) { maxX = rect.X + rect.Width; }
                if (rect.Y < minY) { minY = rect.Y; }
                if (rect.Y + rect.Height > maxY) { maxY = rect.Y + rect.Height; }
            }
            mBoundingBox = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
        public void Optimize()
        {
            mRects = RTree.FullyOptimizeBoundingArea(this);
        }
        // *** ICloneable<BoundingArea> interface implementation ***
        public BoundingArea Clone()
        {
            BoundingArea clone = new BoundingArea();
            clone.mRects = mRects.Clone();
            clone.mBoundingBox = mBoundingBox;
            return clone;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
        // *** Implicit cast to a read-only adapter ***
        public static implicit operator BoundingArea.ReadOnly(BoundingArea boundingArea)
        {
            if (boundingArea == null) { return null; }
            return new BoundingArea.ReadOnly(boundingArea);
        }
        /* .-----------------------------------------------------------------------
           |		 
           |  Class BoundingArea.ReadOnly
           |
           '-----------------------------------------------------------------------
        */
        public class ReadOnly : IReadOnlyAdapter<BoundingArea>
        {
            private BoundingArea mBoundingArea;
            public ReadOnly(BoundingArea boundingArea)
            {
                Utils.ThrowException(boundingArea == null ? new ArgumentNullException("boundingArea") : null);
                mBoundingArea = boundingArea;
            }
            public ArrayList<RectangleF>.ReadOnly Rectangles
            {
                get { return mBoundingArea.Rectangles; }
            }
            public bool IntersectsWith(BoundingArea.ReadOnly other)
            {
                return mBoundingArea.IntersectsWith(other);
            }
            public RectangleF BoundingBox 
            {
                get { return mBoundingArea.BoundingBox; }
            }
            // *** IReadOnlyAdapter interface implementation ***
            public BoundingArea GetWritableCopy()
            {
                return mBoundingArea.Clone();
            }
            object IReadOnlyAdapter.GetWritableCopy()
            {
                return GetWritableCopy();
            }
#if PUBLIC_INNER
            public
#else
            internal
#endif
            BoundingArea Inner
            {
                get { return mBoundingArea; }
            }
        }
        /* .-----------------------------------------------------------------------
           |
           |  Class RTree
           |
           '-----------------------------------------------------------------------
        */
        public class RTree
        {
            private const int ENTRIES_PER_NODE
                = 3;
            private const int MIN_ENTRIES_PER_NODE
                = 1;
            private Node mRoot
                = new Node();
            //private static StreamWriter writer
            //    = new StreamWriter("c:\\rTreeLog.txt");
            // 
            // *** Utilities ***
            //
            private Node ChooseLeaf(Entry newEntry)
            {
                //writer.Write("ChooseLeaf({0})", newEntry.Id);
                // CL1: initialize
                Node node = mRoot;
                while (!node.IsLeaf) // CL2: leaf check
                {
                    // CL3: choose subtree
                    Entry minDiffEntry = null;
                    float minDiff = float.MaxValue;
                    float minArea = float.MaxValue;
                    foreach (Entry entry in node.Entries)
                    {
                        float entryArea = entry.GetArea();
                        RectangleF bb = RectangleF.Union(entry.BoundingBox, newEntry.BoundingBox);
                        float areaDiff = bb.Width * bb.Height - entryArea;
                        if (areaDiff < minDiff)
                        {
                            minDiff = areaDiff;
                            minDiffEntry = entry;
                            minArea = entryArea;
                        }
                        else if (areaDiff == minDiff && entryArea < minArea)
                        {
                            minDiffEntry = entry;
                            minArea = entryArea;
                        }
                    }
                    // CL4: descend until a leaf is reached
                    node = minDiffEntry.ChildNode;
                }
                //writer.WriteLine("->{0}", node);
                //writer.Flush();
                return node;
            }
            private void AdjustTree(Node node1, Node node2)
            {
                while (node1 != mRoot) // AT2: check if done
                {
                    Node parent1 = node1.Parent.Owner;
                    // AT3: adjust covering rectangle in parent entry
                    node1.Parent.UpdateBoundingBox();
                    // AT4: propagate node split upward
                    if (node2 != null)
                    {
                        Entry entry = new Entry();
                        entry.ChildNode = node2;
                        entry.UpdateBoundingBox();
                        if (parent1.Entries.Count < ENTRIES_PER_NODE)
                        {
                            parent1.AddEntry(entry);
                            node2 = null;
                        }
                        else
                        {
                            Node parent2 = SplitNode(parent1, entry);
                            node2 = parent2;
                            if (parent1 == mRoot)
                            {
                                // (I4: grow tree taller)
                                mRoot = new Node();
                                Entry entry1 = new Entry();
                                Entry entry2 = new Entry();
                                entry1.ChildNode = parent1;
                                entry1.UpdateBoundingBox();
                                entry2.ChildNode = parent2;
                                entry2.UpdateBoundingBox();
                                mRoot.AddEntry(entry1);
                                mRoot.AddEntry(entry2);
                                break;
                            }
                        }
                    }
                    // AT5: move up to next level
                    node1 = parent1;
                }                
            }
            private void PickSeeds(ArrayList<Entry> entries, ref Entry seed1, ref Entry seed2)
            {
                // PS1: calculate inefficiency of grouping entries together            
                float maxDiff = float.MinValue;
                Pair<int, int> maxDiffPair = new Pair<int, int>(-1, -1);
                for (int i = 0; i < entries.Count; i++)
                {
                    for (int j = 0; j < entries.Count; j++)
                    {
                        if (i != j)
                        {
                            RectangleF bb = RectangleF.Union(entries[i].BoundingBox, entries[j].BoundingBox);
                            float diff = bb.Width * bb.Height - entries[i].GetArea() - entries[j].GetArea();
                            if (diff > maxDiff)
                            {
                                maxDiff = diff;
                                maxDiffPair = new Pair<int, int>(i, j);
                            }
                        }
                    }
                }
                // PS2: choose the most wasteful pair
                seed1 = entries[maxDiffPair.First];
                seed2 = entries[maxDiffPair.Second];
                if (maxDiffPair.First > maxDiffPair.Second)
                {
                    entries[maxDiffPair.First] = entries.Last;
                    entries.RemoveRange(entries.Count - 1, 1);
                    entries[maxDiffPair.Second] = entries.Last;
                    entries.RemoveRange(entries.Count - 1, 1);
                }
                else
                {
                    entries[maxDiffPair.Second] = entries.Last;
                    entries.RemoveRange(entries.Count - 1, 1);
                    entries[maxDiffPair.First] = entries.Last;
                    entries.RemoveRange(entries.Count - 1, 1);
                }
            }
            private Entry PickNext(ArrayList<Entry> entries, RectangleF bb1, RectangleF bb2, ref float area1, ref float area2,
                ref RectangleF retBb1, ref RectangleF retBb2, ref float retDiff1, ref float retDiff2)
            {
                // PN1: determine cost of putting each entry in each group
                float maxDiff = float.MinValue;
                int maxDiffIdx = -1;
                area1 = bb1.Width * bb1.Height;
                area2 = bb2.Width * bb2.Height;
                for (int i = 0; i < entries.Count; i++)
                {
                    RectangleF newBb1 = RectangleF.Union(bb1, entries[i].BoundingBox);
                    RectangleF newBb2 = RectangleF.Union(bb2, entries[i].BoundingBox);
                    float areaDiff1 = newBb1.Width * newBb1.Height - area1;
                    float areaDiff2 = newBb2.Width * newBb2.Height - area2;
                    float diff = Math.Abs(areaDiff1 - areaDiff2);
                    if (diff > maxDiff)
                    {
                        maxDiff = diff;
                        maxDiffIdx = i;
                        retBb1 = newBb1;
                        retBb2 = newBb2;
                        retDiff1 = areaDiff1;
                        retDiff2 = areaDiff2;
                    }
                }
                // PN2: find entry with greatest preference for one group
                Entry maxDiffEntry = entries[maxDiffIdx];
                entries[maxDiffIdx] = entries.Last;
                entries.RemoveRange(entries.Count - 1, 1);
                return maxDiffEntry;
            }
            private Node SplitNode(Node node, Entry entry)
            {
                ArrayList<Entry> entries = new ArrayList<Entry>(ENTRIES_PER_NODE + 1);
                entries.Add(entry);
                entries.AddRange(node.Entries);
                // QS1: pick first entry for each group
                Entry seed1 = null, seed2 = null;
                PickSeeds(entries, ref seed1, ref seed2);
                node.Entries.Clear();
                node.AddEntry(seed1);
                Node node2 = new Node();
                node2.AddEntry(seed2);
                RectangleF bb1 = seed1.BoundingBox;
                RectangleF bb2 = seed2.BoundingBox;
                while (entries.Count > 0)
                {
                    // QS2: check if done
                    if (node.Entries.Count + entries.Count <= MIN_ENTRIES_PER_NODE)
                    {
                        node.AddEntries(entries);
                        return node2;
                    }
                    if (node2.Entries.Count + entries.Count <= MIN_ENTRIES_PER_NODE)
                    {
                        node2.AddEntries(entries);
                        return node2;
                    }
                    // QS3: select entry to assign
                    float area1 = 0, area2 = 0;
                    RectangleF newBb1 = RectangleF.Empty, newBb2 = RectangleF.Empty;
                    float areaDiff1 = 0, areaDiff2 = 0;
                    Entry nextEntry = PickNext(entries, bb1, bb2, ref area1, ref area2, ref newBb1, ref newBb2, ref areaDiff1, ref areaDiff2);
                    if (areaDiff1 < areaDiff2)
                    {
                        node.AddEntry(nextEntry);
                        bb1 = newBb1;
                    }
                    else if (areaDiff1 > areaDiff2)
                    {
                        node2.AddEntry(nextEntry);
                        bb2 = newBb2;
                    }
                    else if (area1 < area2)
                    {
                        node.AddEntry(nextEntry);
                        bb1 = newBb1;
                    }
                    else if (area1 > area2)
                    {
                        node2.AddEntry(nextEntry);
                        bb2 = newBb2;
                    }
                    else if (node.Entries.Count < node2.Entries.Count)
                    {
                        node.AddEntry(nextEntry);
                        bb1 = newBb1;
                    }
                    else
                    {
                        node2.AddEntry(nextEntry);
                        bb2 = newBb2;
                    }
                }
                return node2;
            }
            //
            // *** Insertion ***
            //
            private void Insert(Entry entry)
            {
                // I1: find position for new record
                Node leaf = ChooseLeaf(entry);
                // I2: add record to leaf node
                Node leaf2 = null;
                if (leaf.Entries.Count < ENTRIES_PER_NODE)
                {
                    leaf.AddEntry(entry);
                }
                else
                {
                    leaf2 = SplitNode(leaf, entry);
                }
                // I3: propagate changes upward
                if (leaf == mRoot)
                {
                    if (leaf2 != null)
                    {
                        // I4: grow tree taller
                        mRoot = new Node();
                        Entry entry1 = new Entry();
                        Entry entry2 = new Entry();
                        entry1.ChildNode = leaf;
                        entry1.UpdateBoundingBox();
                        entry2.ChildNode = leaf2;
                        entry2.UpdateBoundingBox();
                        mRoot.AddEntry(entry1);
                        mRoot.AddEntry(entry2);
                    }
                }
                else
                {
                    AdjustTree(leaf, leaf2);
                }
                //writer.WriteLine(ToString());
                //writer.Flush();
            }
            private Entry Insert(RectangleF rect)
            {
                Entry entry = new Entry(rect);
                Insert(entry);
                return entry;
            }
            //
            // *** Deletion (fast, no balancing) ***
            //
            private bool RemoveEntry(Entry entry)
            {
                Node node = entry.Owner;
                for (int i = 0; i < node.Entries.Count; i++)
                {
                    if (node.Entries[i] == entry)
                    {
                        node.Entries[i] = node.Entries.Last;
                        node.Entries.RemoveRange(node.Entries.Count - 1, 1);
                        break;
                    }
                }
                return node.Entries.Count == 0;
            }
            private void Delete(Entry entry)
            {
                while (RemoveEntry(entry))
                {
                    entry = entry.Owner.Parent;
                    if (entry == null) { break; }
                    entry.ChildNode = null;
                }
                if (entry != null)
                {
                    if (entry.Owner.Parent != null)
                    {
                        entry.Owner.Parent.UpdateBoundingBox();
                    }
                }
            }
            private void Delete(IEnumerable<Entry> entries)
            {
                foreach (Entry entry in entries)
                {
                    Delete(entry);
                }
            }
            //
            // *** Debugging ***
            //
            //private void ToString(Node node, string prefix, StringBuilder str)
            //{
            //    str.Append(prefix);
            //    str.AppendLine(node.ToString());
            //    if (!node.IsLeaf)
            //    {
            //        foreach (Entry entry in node.Entries)
            //        {
            //            ToString(entry.ChildNode, prefix + "\t", str);
            //        }
            //    }
            //}
            //public override string ToString()
            //{
            //    StringBuilder str = new StringBuilder();
            //    ToString(mRoot, "", str);
            //    return str.ToString();
            //}
            //
            // *** Bounding area optimization ***
            //
            private void Fetch(RectangleF rect, float rectArea, Node node, ArrayList<Entry> result)
            {
                if (node.IsLeaf)
                {
                    foreach (Entry entry in node.Entries)
                    {
                        RectangleF bb = RectangleF.Union(entry.BoundingBox, rect);
                        if (entry.GetArea() + rectArea > bb.Width * bb.Height)
                        {
                            result.Add(entry);
                        }
                    }
                }
                else
                {
                    foreach (Entry entry in node.Entries)
                    {
                        RectangleF bb = RectangleF.Union(entry.BoundingBox, rect);
                        if (entry.GetArea() + rectArea > bb.Width * bb.Height)
                        {
                            Fetch(rect, rectArea, entry.ChildNode, result);
                        }
                    }
                }
            }
            private ArrayList<Entry> Fetch(RectangleF rect)
            {
                ArrayList<Entry> result = new ArrayList<Entry>();
                Fetch(rect, rect.Width * rect.Height, mRoot, result);
                return result;
            }
            private Entry GetAnyLeafEntry()
            {
                Node node = mRoot;
                while (!node.IsLeaf) 
                { 
                    node = node.Entries[0].ChildNode; 
                }
                return node.Entries[0];
            }
            private static ArrayList<RectangleF> OptimizeBoundingArea(IEnumerable<RectangleF> boundingArea)
            {
                RTree rTree = new RTree();
                int rectCount = 0;
                foreach (RectangleF rect in boundingArea)
                {
                    rTree.Insert(rect);
                    rectCount++;
                }
                ArrayList<RectangleF> result = new ArrayList<RectangleF>(rectCount);
                while (rTree.mRoot.Entries.Count > 0)
                {
                    Entry entry = rTree.GetAnyLeafEntry();
                    RectangleF bb = entry.BoundingBox;
                    float area = 0;
                    while (bb.Width * bb.Height - area > 0.1f) // *** increase this threshold?
                    {
                        area = bb.Width * bb.Height;                            
                        ArrayList<Entry> queryResult = rTree.Fetch(bb);
                        rTree.Delete(queryResult);
                        foreach (Entry resultEntry in queryResult)
                        {
                            bb = RectangleF.Union(bb, resultEntry.BoundingBox);                                    
                        }
                    }
                    result.Add(bb);
                }
                return result;
            }
            public static ArrayList<RectangleF> OptimizeBoundingArea(BoundingArea boundingArea)
            {
                return OptimizeBoundingArea(boundingArea.Rectangles);
            }
            public static ArrayList<RectangleF> FullyOptimizeBoundingArea(BoundingArea boundingArea)
            {
                int rectCount = boundingArea.Rectangles.Count;
                ArrayList<RectangleF> rects = OptimizeBoundingArea(boundingArea.Rectangles);
                while (rects.Count < rectCount)
                {
                    rectCount = rects.Count;
                    rects = OptimizeBoundingArea(rects);
                }
                return rects;
            }
            /* .-----------------------------------------------------------------------
               |
               |  Class Entry
               |
               '-----------------------------------------------------------------------
            */
            private class Entry
            {
                //private static int mEntryId
                //    = 0; 
                private Node mOwner
                    = null;
                private Node mChildNode
                    = null;
                private RectangleF mBoundingBox
                    = RectangleF.Empty;
                //private int mId
                //    = ++mEntryId;
                public Entry()
                {
                }
                public Entry(RectangleF rect)
                {
                    mBoundingBox = rect;
                }
                //public int Id
                //{
                //    get { return mId; }
                //}
                public float GetArea()
                {
                    return BoundingBox.Width * BoundingBox.Height;
                }
                public void UpdateBoundingBox()
                {
                    if (ChildNode != null && ChildNode.Entries.Count > 0)
                    {
                        mBoundingBox = ChildNode.Entries[0].mBoundingBox;
                        for (int i = 1; i < ChildNode.Entries.Count; i++)
                        {
                            mBoundingBox = RectangleF.Union(mBoundingBox, ChildNode.Entries[i].mBoundingBox);
                        }
                    }
                }
                public Node ChildNode
                {
                    get { return mChildNode; }
                    set
                    {
                        mChildNode = value;
                        if (value != null)
                        {
                            value.Parent = this;
                        }
                    }
                }
                public Node Owner
                {
                    get { return mOwner; }
                    set { mOwner = value; } // *** used only by Node
                }
                public RectangleF BoundingBox
                {
                    get { return mBoundingBox; }
                    set { mBoundingBox = value; }
                }
            }
            /* .-----------------------------------------------------------------------
               |
               |  Class Node
               |
               '-----------------------------------------------------------------------
            */
            private class Node
            {
                private Entry mParent
                    = null;
                private ArrayList<Entry> mEntries
                    = new ArrayList<Entry>(ENTRIES_PER_NODE);
                public bool IsLeaf
                {
                    get { return mEntries.Count == 0 || mEntries[0].ChildNode == null; }
                }
                public Entry Parent
                {
                    get { return mParent; }
                    set { mParent = value; } // *** used only by Entry
                }
                public ArrayList<Entry> Entries
                {
                    get { return mEntries; }
                }
                public void AddEntry(Entry entry)
                {
                    mEntries.Add(entry);
                    entry.Owner = this;
                }
                public void AddEntries(IEnumerable<Entry> entries)
                {
                    foreach (Entry entry in entries)
                    {
                        AddEntry(entry);
                    }
                }
                //public override string ToString()
                //{
                //    string nodeStr = "(";
                //    foreach (Entry entry in mEntries)
                //    {
                //        nodeStr += entry.Id + ",";
                //    }
                //    return nodeStr.TrimEnd(',') + ")";
                //}
            }
        }
    }
}