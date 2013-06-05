using System;
using System.Text;
using System.Collections.Generic;

namespace Latino.WebMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class UrlTree
       |
       '-----------------------------------------------------------------------
    */
    public class UrlTree
    {
        /* .-----------------------------------------------------------------------
           |
           |  Class Node
           |
           '-----------------------------------------------------------------------
        */
        private class Node
        {
            public string mKey;
            public ArrayList<Node> mChildren
                = new ArrayList<Node>();
            public int mCount
                = 0;
            public MultiSet<ulong> mHashCodes
                = new MultiSet<ulong>();
            public bool mPartOfDomain
                = false;
            public bool mPartOfTld
                = false;

            public Node(string key)
            {
                mKey = key;
            }

            public Node GetChildNode(string key)
            {
                foreach (Node child in mChildren)
                {
                    if (child.mKey == key) { return child; }
                }
                return null;
            }

            public override string ToString()
            {
                return mKey;
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class NodeInfo
           |
           '-----------------------------------------------------------------------
        */
        public class NodeInfo
        {
            private ArrayList<int> mTextBlockCounts
                = new ArrayList<int>();
            private NodeLocation mNodeLocation;
            private int mDocCount;
            private string mUrlPart;

            public NodeInfo(NodeLocation nodeLocation, int docCount, string urlPart)
            {
                mNodeLocation = nodeLocation;
                mDocCount = docCount;
                mUrlPart = urlPart;
            }

            public ArrayList<int> TextBlockCounts
            {
                get { return mTextBlockCounts; }
            }

            public NodeLocation NodeLocation
            {
                get { return mNodeLocation; }
            }

            public int NodeDocumentCount
            {
                get { return mDocCount; }
            }

            public string UrlPart
            {
                get { return mUrlPart; }
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class TextBlock
           |
           '-----------------------------------------------------------------------
        */
        public class TextBlock
        {      
            private ulong mHashCode;
            private int mBlockCount;

            public TextBlock(ulong hashCode, int nodeTextBlockCount)
            {
                mHashCode = hashCode;
                mBlockCount = nodeTextBlockCount;
            }

            public int NodeTextBlockCount
            {
                get { return mBlockCount; }
            }

            public ulong HashCode
            {
                get { return mHashCode; }
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum NodeLocation
           |
           '-----------------------------------------------------------------------
        */
        [Flags]
        public enum NodeLocation
        {
            Exact = 1,
            OffByOne = 2,
            WithinPath = 4,
            WithinDomain = 8,
            WithinTld = 16,
            Root = 32
        }

        private Node mRoot
            = new Node("#");

        private ArrayList<string> GetPath(string url, out int numDomainParts, out int numTldParts, bool fullPath)
        {          
            string leftPart;
            ArrayList<string> path;
            ArrayList<KeyDat<string, string>> queryParsed;
            UrlNormalizer.ParseUrl(url, out leftPart, out path, out queryParsed);
            string host = leftPart.Split(':')[1].Trim('/');
            string[] tldParts = UrlNormalizer.GetTldFromDomainName(host).Split('.');
            numTldParts = tldParts.Length;
            string[] hostParts = host.Split('.');
            numDomainParts = hostParts.Length;
            ArrayList<string> branch = new ArrayList<string>(hostParts);
            branch.Reverse();
            branch.AddRange(path);
            //Console.WriteLine(branch);
            if (!fullPath && path.Count > 0) 
            { 
                branch.RemoveAt(branch.Count - 1); 
            }
            return branch;
        }

        private static string Normalize(string str, bool alphaOnly, bool toLower)
        {
            StringBuilder result = new StringBuilder();
            if (alphaOnly)
            {
                foreach (char ch in str) { if (char.IsLetter(ch)) { result.Append(ch); } }
            }
            else
            {
                foreach (char ch in str) { if (char.IsLetterOrDigit(ch)) { result.Append(ch); } }
            }
            return toLower ? result.ToString().ToLower() : result.ToString();
        }

        public static ulong ComputeHashCode(string textBlock, bool alphaOnly)
        {
            string txtNormalized = Normalize(textBlock, alphaOnly, /*toLower=*/true);
            return Utils.GetHashCode64(txtNormalized);
        }

        public static ArrayList<ulong> ComputeHashCodes(IEnumerable<string> textBlocks, bool alphaOnly)
        {
            ArrayList<ulong> hashCodes = new ArrayList<ulong>();
            foreach (string textBlock in textBlocks)
            {
                hashCodes.Add(ComputeHashCode(textBlock, alphaOnly));
            }
            return hashCodes;
        }

        public NodeInfo[] Insert(string url, ArrayList<ulong> hashCodes, int minDocCount, bool fullPath, bool insertUnique, bool incDocCount)
        {
            return Query(url, hashCodes, minDocCount, fullPath, /*insert=*/true, insertUnique, incDocCount);
        }

        public NodeInfo[] Query(string url, ArrayList<ulong> hashCodes, int minDocCount, bool fullPath)
        {
            return Query(url, hashCodes, minDocCount, fullPath, /*insert=*/false, /*insertUnique=*/false, /*incDocCount=*/false);
        }

        private NodeInfo[] Query(string url, ArrayList<ulong> hashCodes, int minDocCount, bool fullPath, bool insert, bool insertUnique, bool incDocCount)
        {
            Set<ulong> hashCodesUnique = new Set<ulong>(hashCodes);
            // insert hash codes      
            int numDomainParts, numTldParts;
            ArrayList<string> path = GetPath(url, out numDomainParts, out numTldParts, fullPath);
            ArrayList<Node> crumbs = new ArrayList<Node>(path.Count);
            if (path == null) { return null; }
            Node node = mRoot;
            if (insert)
            {
                if (incDocCount) { node.mCount++; }
                if (insertUnique) { node.mHashCodes.AddRange(hashCodesUnique); }
                else { node.mHashCodes.AddRange(hashCodes); }
            }
            crumbs.Add(node);
            int i;
            for (i = 0; i < path.Count; i++)
            {
                Node child = node.GetChildNode(path[i]);
                if (child == null)
                {
                    for (int j = i; j < path.Count; j++)
                    {
                        child = new Node(path[j]);
                        if (insert)
                        {
                            node.mChildren.Add(child);
                            if (incDocCount) { child.mCount++; }
                            if (insertUnique) { child.mHashCodes.AddRange(hashCodesUnique); }
                            else { child.mHashCodes.AddRange(hashCodes); }
                        }
                        child.mPartOfDomain = numDomainParts > 0;
                        child.mPartOfTld = numTldParts > 0;
                        numDomainParts--;
                        numTldParts--;
                        crumbs.Add(child);
                        node = child;
                    }
                    break;
                }
                if (insert)
                {
                    if (incDocCount) { child.mCount++; }
                    if (insertUnique) { child.mHashCodes.AddRange(hashCodesUnique); }
                    else { child.mHashCodes.AddRange(hashCodes); }
                    child.mPartOfDomain = numDomainParts > 0;
                    child.mPartOfTld = numTldParts > 0;
                }
                numDomainParts--;
                numTldParts--;
                crumbs.Add(child); 
                node = child;
            }
            // "classify" text blocks
            ArrayList<NodeInfo> result = new ArrayList<NodeInfo>();
            if (mRoot.mCount >= minDocCount)
            {
                for (i = crumbs.Count - 1; i >= 0; i--)
                {
                    if (crumbs[i].mCount >= minDocCount)
                    {
                        NodeLocation nodeLocation;
                        if (crumbs[i] == mRoot) { nodeLocation = NodeLocation.Root; }
                        else if (crumbs[i].mPartOfTld) { nodeLocation = NodeLocation.WithinTld; }
                        else if (crumbs[i].mPartOfDomain) { nodeLocation = NodeLocation.WithinDomain; }
                        else { nodeLocation = NodeLocation.WithinPath; }
                        if (i == crumbs.Count - 1) { nodeLocation |= NodeLocation.Exact; }
                        else if (i == crumbs.Count - 2) { nodeLocation |= NodeLocation.OffByOne; }
                        NodeInfo nodeInfo = new NodeInfo(nodeLocation, crumbs[i].mCount, crumbs[i].ToString());
                        for (int j = 0; j < hashCodes.Count; j++)
                        {
                            nodeInfo.TextBlockCounts.Add(crumbs[i].mHashCodes.GetCount(hashCodes[j]));
                        }
                        result.Add(nodeInfo);
                    }
                }
            }
            else // root does not contain enough documents
            {
                NodeInfo nodeInfo = new NodeInfo(NodeLocation.Root, mRoot.mCount, mRoot.ToString());
                for (int j = 0; j < hashCodes.Count; j++)
                {
                    nodeInfo.TextBlockCounts.Add(crumbs[0].mHashCodes.GetCount(hashCodes[j]));
                }
                result.Add(nodeInfo);
            }
            return result.ToArray();
        }

        public bool Remove(string url, ArrayList<ulong> hashCodes, bool fullPath, bool unique, bool decDocCount)
        {
            Set<ulong> hashCodesUnique = new Set<ulong>(hashCodes);
            int numDomainParts, numTldParts;
            ArrayList<string> path = GetPath(url, out numDomainParts, out numTldParts, fullPath);
            if (path == null) { return false; }
            Node node = mRoot;
            if (decDocCount) { node.mCount--; }
            if (unique) { node.mHashCodes.RemoveRange(hashCodesUnique); }
            else { node.mHashCodes.RemoveRange(hashCodes); }
            if (node.mCount == 0) // cut the branch?
            { 
                node.mChildren.Clear(); 
                return true; 
            }
            for (int i = 0; i < path.Count; i++)
            {
                Node child = node.GetChildNode(path[i]);
                if (child == null) { return false; }
                if (decDocCount) { child.mCount--; }
                if (unique) { child.mHashCodes.RemoveRange(hashCodesUnique); }
                else { child.mHashCodes.RemoveRange(hashCodes); }
                if (child.mCount == 0) // cut the branch?
                {
                    child.mChildren.Clear();
                    return true;
                }
                node = child;
            }
            return true;
        }

        private void CreateString(Node node, StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + node.mKey + " (" + node.mCount + "/" + node.mHashCodes.Count + ")");
            foreach (Node child in node.mChildren)
            {
                CreateString(child, sb, prefix + "  ");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            CreateString(mRoot, sb, /*prefix=*/"");
            return sb.ToString().TrimEnd();
        }
    }
}
