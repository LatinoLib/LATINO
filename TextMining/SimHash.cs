/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    SimHash.cs
 *  Desc:    SimHash fingerprints
 *  Created: Jun-2010
 *
 *  Authors: Marko Brakus
 *
 ***************************************************************************/


using System;
using System.Collections.Generic;
using System.Collections;

namespace Latino.TextMining
{
    class Fingerprint
    {
        public ulong code;
        public ulong did;  // document id
        public Fingerprint(ulong code, ulong did)
        { this.code = code; this.did = did; }
    }

    class NearFingerprint : Fingerprint
    {
        public int dist;    // Hamming distance from the query fingerprint
        public NearFingerprint(ulong code, ulong did, int dist)
            : base(code, did)
        { this.dist = dist; }
    }

    class BitTree
    {
        private int KeyLength;
        private int[] perm;
        public BitTree(int b1, int b2, int b3, int b4, int b5)
        {
            KeyLength = (b5 == 39) ? 25 : 26;
            perm = new int[]{b1,b2,b3,b4,b5};
        }

        Node root = new Node();
        public class Node
        {
            public Node[] kids = new Node[2];
            public Node()
            {
                kids[0] = null;
                kids[1] = null;
            }
        }

        public class Leaf : Node
        {
            private List<Fingerprint> fps = new List<Fingerprint>();

            public bool Add(ulong pf, ulong did)
            {
                if (Find(pf) == true)
                    return false;

                fps.Add(new Fingerprint(pf, did));
                return true;
            }

            public bool IsEmpty()
            {
                return (fps.Count == 0);
            }

            public bool Find(ulong pf)
            {
                return (fps.Find(delegate(Fingerprint fp) { return fp.code == pf; }) != default(Fingerprint));
            }

            public bool Remove(ulong pf)
            {
                return fps.Remove(fps.Find(delegate(Fingerprint fp) { return fp.code == pf; }));
            }

            public NearFingerprint FindNearDuplicate(ulong pf, int k)
            {
                foreach (Fingerprint t in fps)
                {
                    ulong xored = t.code ^ pf;
                    int dist = (int) CountOnes(xored);
                    if (dist <= k)
                        return new NearFingerprint(t.code, t.did, dist);
                }
                return null;
            }

            public List<NearFingerprint> FindAllNearDuplicates(ulong pf, int k)
            {
                List<NearFingerprint> dups = new List<NearFingerprint>();
                foreach (Fingerprint t in fps)
                {
                    ulong xored = t.code ^ pf;
                    int dist = (int)CountOnes(xored);
                    if (dist <= k)
                        dups.Add(new NearFingerprint(t.code, t.did, dist));
                }
                return dups;
            }

            public static uint CountOnes(ulong xored)
            {
                // lower 32 bits
                uint v = (uint)(xored & 0x00000000ffffffff);
                v = v - ((v >> 1) & 0x55555555);
                v = (v & 0x33333333) + ((v >> 2) & 0x33333333);     
                uint cLower = ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;

                // upper 32 bits
                uint u = (uint)((xored & 0xffffffff00000000) >> 32);
                u = u - ((u >> 1) & 0x55555555);                    
                u = (u & 0x33333333) + ((u >> 2) & 0x33333333);     
                uint cUpper = ((u + (u >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;

                return cLower + cUpper;
            }
        }

        public ulong PermuteFingerprint(ulong f)
        {
            const ulong mask13 = 0x1FFF;
            ulong permutedf = 0;
            for(int b = 0; b < 5; b++)
                if(perm[b] >= 0)
                    permutedf |= ((f & (mask13 << b*13)) >> perm[b]);
                else
                    permutedf |= ((f & (mask13 << b*13)) << (perm[b]*-1));
            return permutedf;
        }

        public ulong InversePermuteFingerprint(ulong pf)
        {
            const ulong mask13 = 0x1FFF;
            int [] offsets = {0, 13, 26, 39, 52};
            ulong f = 0;
            for (int b = 0; b < 5; b++)
                if (perm[b] >= 0)
                    f |= ((pf & (mask13 << (offsets[b] - perm[b]))) << perm[b]);
                else
                    f |= ((pf & (mask13 << (offsets[b] - perm[b]))) >> (perm[b]*-1));
            return f;
        }

        public bool Find(ulong f)
        {
            ulong pf = PermuteFingerprint(f);

            int bit;
            Node it = root;
            for (int i = 0; i < KeyLength; i++)
            {
                bit = (int)((pf & (((ulong)1) << i)) >> i);
                if (it.kids[bit] == null)
                    if (it.kids[1-bit] == null)
                        return ((Leaf)it).Find(pf);
                    else
                        return false;
                it = it.kids[bit];
            }
            return true;
        }

        public NearFingerprint FindNearDuplicate(ulong f, int k)
        {
            ulong pf = PermuteFingerprint(f);

            int bit;
            Node it = root;
            for (int i = 0; i < KeyLength; i++)
            {
                bit = (int)((pf & (((ulong)1) << i)) >> i);
                if (it.kids[bit] == null)
                    if (it.kids[1-bit] == null)
                    {
                        NearFingerprint nf = ((Leaf)it).FindNearDuplicate(pf, k);
                        nf.code = InversePermuteFingerprint(pf);
                        return nf;
                    }
                    else
                        return null;
                it = it.kids[bit];
            }
            return null;
        }

        public List<NearFingerprint> FindAllNearDuplicates(ulong f, int k)
        {
            ulong pf = PermuteFingerprint(f);

            int bit;
            Node it = root;
            for (int i = 0; i <= KeyLength; i++)
            {
                bit = (int)((pf & (((ulong)1) << i)) >> i);
                if (it.kids[bit] == null)
                    if (it.kids[1-bit] == null)
                    {
                        List<NearFingerprint> dups = ((Leaf)it).FindAllNearDuplicates(pf, k);
                        foreach (NearFingerprint nf in dups)
                            nf.code = InversePermuteFingerprint(pf);
                        return dups;
                    }
                    else
                        return null;
                it = it.kids[bit];
            }
            return null;
        }

        public bool Insert(ulong f, ulong did)
        {
            ulong pf = PermuteFingerprint(f);

            bool isAdding = false;
            int bit;
            Node it = root;
            for (int i = 0; i < KeyLength; i++)
            {
                bit = (int)((pf & (((ulong)1) << i)) >> i);
                if (isAdding == false)
                    if (it.kids[bit] == null)
                        isAdding = true;
                    else
                    {
                        if (i == KeyLength - 1)
                            return ((Leaf)it.kids[bit]).Add(pf, did);
                        it = it.kids[bit];
                    }
                if (isAdding == true)
                    if (i == KeyLength - 1)
                    {
                        it.kids[bit] = new Leaf();
                        return ((Leaf)it.kids[bit]).Add(pf, did);
                    }
                    else
                    {
                        it.kids[bit] = new Node();
                        it = it.kids[bit];
                    }
            }
            return isAdding;
        }

        private void RemovePath(ulong pf, int cutNode)
        {
            int bit = (int)(pf & ((ulong)1));
            Node it = root;
            for (int i = 0; i < cutNode; i++)
            {
                bit = (int)((pf & (((ulong)1) << i)) >> i);
                it = it.kids[bit];
            }
            it.kids[bit] = null;
        }

        public bool Remove(ulong f)
        {
            ulong pf = PermuteFingerprint(f);

            int bit;
            int cutNode = -1;
            Node it = root;
            for (int i = 0; i < KeyLength; i++)
            {
                bit = (int)((pf & (((ulong)1) << i)) >> i);
                if (it.kids[bit] != null)
                {
                    if ((it.kids[1 - bit] == null) && (cutNode == -1))
                        cutNode = i;
                    else if (it.kids[1 - bit] != null)
                        cutNode = -1;
                    if (i == KeyLength - 1)
                    {
                        if (((Leaf)it.kids[bit]).Remove(pf) == false)
                            return false;
                        if (((Leaf)it.kids[bit]).IsEmpty() == true)
                        {
                            if(cutNode != -1)
                                RemovePath(pf, cutNode);
                            return true;
                        }
                    }
                    it = it.kids[bit];
                }
                else
                    return false;
            }

            return true;
        }
    }

    class PTables
    {
        private BitTree[] trees;

        /* permutation tables(trees)
         *
         * indx,key:
         * 0:   b2b1   
         * 1:   b3b1   
         * 2:   b4b1   
         * 3:   b5b1   
         * 
         * 4:   b3b2   
         * 5:   b4b2   
         * 6:   b5b2   
         * 
         * 7:   b4b3   
         * 8:   b5b3   
         * 
         * 9:   b5b4  
         */
        public PTables()
        {
            trees = new BitTree[10];
            trees[0] = new BitTree(0,   0,  0,  0,  0);
            trees[1] = new BitTree(0, -13,  13, 0,  0);
            trees[2] = new BitTree(0, -13, -13, 26, 0);
            trees[3] = new BitTree(0, -12, -12, -12, 39);
            trees[4] = new BitTree(-26, 13, 13,  0,  0);
            trees[5] = new BitTree(-26, 13, -13,  26, 0);
            trees[6] = new BitTree(-25, 13, -12, -12, 39);
            trees[7] = new BitTree(-26, -26, 26,  26, 0);
            trees[8] = new BitTree(-25, -25, 26, -12, 39);
            trees[9] = new BitTree(-25, -25, -25, 39, 39);
        }

        public bool Find(ulong f)
        {
            for (int t = 0; t < 9; t++)
                if (trees[t].Find(f) == true)
                    return true;

            return false;
        }

        public bool HasNearDuplicate(ulong f, int k)
        {
            for (int t = 0; t < 9; t++)
                if (trees[t].FindNearDuplicate(f, k) != null)
                    return true;
            return false;
        }

        public List<NearFingerprint> FindAllNearDuplicates(ulong f, int k)
        {
            List<NearFingerprint> dups = new List<NearFingerprint>();
            for (int t = 0; t < 9; t++)
                dups.AddRange(trees[t].FindAllNearDuplicates(f, k));
            return dups;
        }

        public bool Insert(ulong f, ulong did)
        {            
            int duplicates = 0; ;
            for (int t = 0; t < 9; t++)
                if (trees[t].Insert(f, did) == false)
                    duplicates++;

            return (duplicates == 0);
        }

        public bool Remove(ulong f)
        {
            for (int t = 0; t < 9; t++)
                if (trees[t].Remove(f) == false)
                    return false;
            return true;
        }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //    }
    //}
}
