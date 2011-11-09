/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    BoilerplateRemover.cs
 *  Desc:    Extracts relevant content from HTML documents
 *           Based on "Boilerplate detection using shallow text features" (WSDM 2010)
 *  Created: Sep-2010
 *
 *  Author:  Marko Brakus
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;

namespace Latino.WebMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class BoilerplateRemover
       |
       '-----------------------------------------------------------------------
    */
    public class BoilerplateRemover
    {
        /* .-----------------------------------------------------------------------
           |
           |  Enum TextClass
           |
           '-----------------------------------------------------------------------
        */
        public enum TextClass
        {
            Unknown = 0,
            Boilerplate = 1,
            Headline = 2,
            FullText = 4,
            Supplement = 8,
            RelatedContent = 16,
            UserComment = 32,
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum SpanTagType
           |
           '-----------------------------------------------------------------------
        */
        private enum SpanTagType
        {
            NotSpanTag = 0,
            KnownOpeningSpanTag = 1,
            UnknownOpeningSpanTag = 2,
            ClosingSpanTag = 3,
            OpeningAndClosingSpanTag = 4
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum FeaturesEnum
           |
           '-----------------------------------------------------------------------
        */
        public enum FeaturesEnum
        {
            relPosition = 1,
            textClass = 2,
            charCount = 4,
            numWords = 8,
            linkDensity = 16,
            textDensity = 32,
            avgWordLen = 64,
            fullstopRatio = 128,
            capitalWordsRatio = 256,
            capitalStartWordsRatio = 512,
            numWordsQuotPC = 1024,
            textDensityQuotPC = 2048,
            isInP = 4096,
            isInH = 8192,
            all = Int32.MaxValue
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class FeatureNames
           |
           '-----------------------------------------------------------------------
        */
        public static class FeatureNames
        {
            public static string relPosition = "RelPos";
            public static string textClass = "Class";
            public static string charCount = "Chars";
            public static string numWords = "Wrds";
            public static string linkDensity = "LnkDns";
            public static string textDensity = "TxtDns";
            public static string avgWordLen = "AvgWrdLn";
            public static string fullstopRatio = "FlstpRtio";
            public static string capitalWordsRatio = "CapWrdRtio";
            public static string capitalStartWordsRatio = "CapStrtWrdRtio";
            public static string numWordsQuotPC = "WrdsQt";
            public static string textDensityQuotPC = "TxtDnsQt";
            public static string isInP = "InP";
            public static string isInH = "InH";

            public static List<string> ToString(string prec)
            {
                return ToString(prec, FeaturesEnum.all);
            }

            public static List<string> ToString(string prec, FeaturesEnum fe)
            {
                List<string> features = new List<string>();

                if ((fe & FeaturesEnum.relPosition) != 0)
                    features.Add(prec + relPosition);
                if ((fe & FeaturesEnum.numWords) != 0)
                    features.Add(prec + numWords);
                if ((fe & FeaturesEnum.charCount) != 0)
                    features.Add(prec + charCount);
                if ((fe & FeaturesEnum.linkDensity) != 0)
                    features.Add(prec + linkDensity);
                if ((fe & FeaturesEnum.textDensity) != 0)
                    features.Add(prec + textDensity);
                if ((fe & FeaturesEnum.avgWordLen) != 0)
                    features.Add(prec + avgWordLen);
                if ((fe & FeaturesEnum.fullstopRatio) != 0)
                    features.Add(prec + fullstopRatio);
                if ((fe & FeaturesEnum.capitalStartWordsRatio) != 0)
                    features.Add(prec + capitalStartWordsRatio);
                if ((fe & FeaturesEnum.capitalWordsRatio) != 0)
                    features.Add(prec + capitalWordsRatio);
                if ((fe & FeaturesEnum.numWordsQuotPC) != 0)
                    features.Add(prec + numWordsQuotPC);
                if ((fe & FeaturesEnum.textDensityQuotPC) != 0)
                    features.Add(prec + textDensityQuotPC);
                if ((fe & FeaturesEnum.isInP) != 0)
                    features.Add(prec + isInP);
                if ((fe & FeaturesEnum.isInH) != 0)
                    features.Add(prec + isInH);

                return features;
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class FeatureValue
           |
           '-----------------------------------------------------------------------
        */
        public class FeatureValue
        {
            private Object val = null;

            public bool Numeric
            {
                get { return val is Double; }
            }

            public string AsString
            {
                get { return (string)val; }
            }

            public double AsDouble
            {
                get { return (double)val; }
            }

            public FeatureValue(double numVal)
            {
                val = numVal;
            }

            public FeatureValue(string strVal)
            {
                val = strVal;
            }

            public override string ToString()
            {
                if (Numeric)
                    return AsDouble.ToString();
                else
                    return AsString;
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class Features
           |
           '-----------------------------------------------------------------------
        */
        public class Features
        {
            private Dictionary<string, FeatureValue> features = new Dictionary<string, FeatureValue>();

            public void Add(string name, FeatureValue val)
            {
                if (features.ContainsKey(name))
                    features[name] = val;
                else
                    features.Add(name, val);
            }

            public FeatureValue GetVal(string name)
            {
                if (name == null)
                    return null;
                FeatureValue val;
                if (features.TryGetValue(name, out val))
                    return val;

                return null;
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class HtmlBlock
           |
           '-----------------------------------------------------------------------
        */
        public class HtmlBlock
        {
            public static class Precedence
            {
                public static string Previous = "p.";
                public static string Current = "c.";
                public static string Next = "n.";
            }

            public TextClass textClass = TextClass.Boilerplate;
            public double relPosition = 0;
            public int numTokensInsideAnchors = 0;
            public int charCount = 0;
            public int numWords = 0;
            public int wordLengthSum = 0;
            public double linkDensity = 0.0;
            public double textDensity = 0.0;
            public double avgWordLen = 0.0;
            public int numFullstops = 0;
            public double fullstopRatio = 0.0;
            public int numCapitalWords = 0;
            public double capitalWordsRatio = 0.0;
            public int numCapitalStartWords = 0;
            public double capitalStartWordsRatio = 0.0;
            public double numWordsQuotPC = 0.0;
            public double textDensityQuotPC = 0.0;
            public bool isInP = false;
            public bool isInH = false;
            public double predConf = 0.0;
            public int tokenCount = 0;
            public string text;
            public int startListIdx = -1;
            public int endListIdx = -1;
            public string trace;

            public HtmlBlock(double pos, int startIdx)
            {
                relPosition = pos;
                startListIdx = startIdx;
            }

            public HtmlBlock(HtmlBlock b, bool isInAnchor, double pos)
            {
                if (isInAnchor)
                    numTokensInsideAnchors = b.numTokensInsideAnchors;
                textClass = b.textClass;
                relPosition = pos;
            }

            public void Add(string t, HtmlTokenizer.TokenType tokenType, bool isInAnchor, bool isTag)
            {
                tokenCount++;
                if (isTag == false)
                {
                    if (tokenType == HtmlTokenizer.TokenType.Word)
                    {
                        numWords++;
                        wordLengthSum += t.Length;
                    }
                    if (t.Equals(t.ToUpper()))
                        numCapitalWords++;
                    else if (t.Substring(0, 1).Equals(t.Substring(0, 1).ToUpper()))
                        numCapitalStartWords++;
                }
                charCount += t.Length;
                if (isInAnchor)
                    numTokensInsideAnchors++;
            }

            public string ToString(FeaturesEnum fe)
            {
                StringBuilder sb = new StringBuilder(50);

                if ((fe & FeaturesEnum.relPosition) != 0)
                    sb.Append(Math.Round(relPosition, 4) + ",");
                if ((fe & FeaturesEnum.numWords) != 0)
                    sb.Append(numWords + ",");
                if ((fe & FeaturesEnum.charCount) != 0)
                    sb.Append(charCount + ",");
                if ((fe & FeaturesEnum.linkDensity) != 0)
                    sb.Append(Math.Round(linkDensity, 4) + ",");
                if ((fe & FeaturesEnum.textDensity) != 0)
                    sb.Append(Math.Round(textDensity, 4) + ",");
                if ((fe & FeaturesEnum.avgWordLen) != 0)
                    sb.Append(Math.Round(avgWordLen, 4) + ",");
                if ((fe & FeaturesEnum.fullstopRatio) != 0)
                    sb.Append(Math.Round(fullstopRatio, 4) + ",");
                if ((fe & FeaturesEnum.capitalStartWordsRatio) != 0)
                    sb.Append(Math.Round(capitalStartWordsRatio, 4) + ",");
                if ((fe & FeaturesEnum.capitalWordsRatio) != 0)
                    sb.Append(Math.Round(capitalWordsRatio, 4) + ",");
                if ((fe & FeaturesEnum.numWordsQuotPC) != 0)
                    sb.Append(Math.Round(numWordsQuotPC, 4) + ",");
                if ((fe & FeaturesEnum.textDensityQuotPC) != 0)
                    sb.Append(Math.Round(textDensityQuotPC, 4) + ",");
                if ((fe & FeaturesEnum.isInP) != 0)
                    sb.Append(isInP ? "1," : "0,");
                if ((fe & FeaturesEnum.isInH) != 0)
                    sb.Append(isInH ? "1," : "0,");

                return sb.ToString();
            }

            public bool Empty
            {
                get { return tokenCount == 0; }
            }

            public bool HasWords
            {
                get { return numWords > 0; }
            }

            public void GetFeatures(Features feats, string precedence)
            {
                feats.Add(precedence + FeatureNames.textClass, new FeatureValue(((double)textClass)));
                feats.Add(precedence + FeatureNames.relPosition, new FeatureValue(relPosition));
                feats.Add(precedence + FeatureNames.charCount, new FeatureValue(charCount));
                feats.Add(precedence + FeatureNames.numWords, new FeatureValue(numWords));
                feats.Add(precedence + FeatureNames.linkDensity, new FeatureValue(linkDensity));
                feats.Add(precedence + FeatureNames.textDensity, new FeatureValue(textDensity));
                feats.Add(precedence + FeatureNames.avgWordLen, new FeatureValue(avgWordLen));
                feats.Add(precedence + FeatureNames.fullstopRatio, new FeatureValue(fullstopRatio));
                feats.Add(precedence + FeatureNames.capitalWordsRatio, new FeatureValue(capitalWordsRatio));
                feats.Add(precedence + FeatureNames.capitalStartWordsRatio, new FeatureValue(capitalStartWordsRatio));
                feats.Add(precedence + FeatureNames.numWordsQuotPC, new FeatureValue(numWordsQuotPC));
                feats.Add(precedence + FeatureNames.textDensityQuotPC, new FeatureValue(textDensityQuotPC));
                feats.Add(precedence + FeatureNames.isInP, new FeatureValue((double)(isInP ? 1 : 0)));
                feats.Add(precedence + FeatureNames.isInH, new FeatureValue((double)(isInH ? 1 : 0)));
            }

            public Features GetFeatures(string precedence)
            {
                Features features = new Features();
                GetFeatures(features, precedence);
                return features;
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class DecisionTree
           |
           '-----------------------------------------------------------------------
        */
        public class DecisionTree
        {
            private enum Operator
            {
                Unknown = 0,
                L = 1,
                LE = 2,
                G = 3,
                GE = 4,
                E = 5,
                NE = 6
            }

            private class Node
            {
                public Node parent = null;
                public Operator op = Operator.Unknown;
                public string featureName = null;
                public FeatureValue threshold = null;
                public TextClass textClass = TextClass.Unknown;
                public List<Node> children = new List<Node>();
                public List<double> classDist = new List<double>();

                public Node()
                    : this(null, null, Operator.Unknown, null, TextClass.Unknown, null)
                { }

                public Node(Node parent, string featureName, Operator op, FeatureValue threshold)
                    : this(parent, featureName, op, threshold, TextClass.Unknown, null)
                { }

                public Node(Node parent, string featureName, Operator op, FeatureValue threshold, TextClass textClass, List<double> classDist)
                {
                    this.parent = parent;
                    this.featureName = featureName;
                    this.op = op;
                    this.threshold = threshold;
                    this.textClass = textClass;
                    this.classDist = classDist;
                }

                public void AddChild(Node n)
                {
                    children.Add(n);
                }

                public bool Leaf
                {
                    get { return children.Count == 0; }
                }

                public new string ToString()
                {
                    string strOp = null;
                    switch (op)
                    {
                        case Operator.E:
                            strOp = "=";
                            break;
                        case Operator.G:
                            strOp = ">";
                            break;
                        case Operator.GE:
                            strOp = ">=";
                            break;
                        case Operator.L:
                            strOp = "<";
                            break;
                        case Operator.LE:
                            strOp = "<=";
                            break;
                        case Operator.NE:
                            strOp = "!=";
                            break;
                        default:
                            return null;
                    }
                    if (Leaf)
                        return featureName + " " + strOp + " " + threshold + ": " + textClass;

                    return featureName + " " + strOp + " " + threshold;
                }
            }

            private Node root = null;
            static private string nodePattern = @"[|\s\t]*(?<feature>\w+([\.-_']*\w+)?)\s(?<op>(<=|<|=|>|>=))\s(?<threshold>\d+(.\d+)?)(:\s(?<class>[^\s]+)\s\((?<distribution>[\d+.\d+]+(/[\d+.\d+]+)*)\))?";
            static private Regex nodeRegex = new Regex(nodePattern, RegexOptions.Compiled);

            public TextClass Predict(HtmlBlock prevB, HtmlBlock curB, HtmlBlock nextB)
            {
                Features features = new Features();
                prevB.GetFeatures(features, HtmlBlock.Precedence.Previous);
                curB.GetFeatures(features, HtmlBlock.Precedence.Current);
                nextB.GetFeatures(features, HtmlBlock.Precedence.Next);

                string trace;
                curB.textClass = Predict(features, out trace);
                curB.trace = trace;
                return curB.textClass;
            }

            public TextClass Predict(Features pattern, out string trace)
            {

                trace = null;
                StringBuilder traceStringBuilder = new StringBuilder();

                Node currentNode = this.root;
                if (this.root == null)
                    return TextClass.Unknown;

            CurrentNodeLoop:
                while (currentNode.Leaf == false)
                {
                    foreach (Node c in currentNode.children)
                    {
                        FeatureValue fv = pattern.GetVal(c.featureName);
                        traceStringBuilder.Append("(" + currentNode.ToString() + ")->");
                        if (fv == null)
                            return TextClass.Unknown;
                        if (fv.Numeric)
                        {
                            switch (c.op)
                            {
                                case Operator.E:
                                    if (fv.AsDouble == c.threshold.AsDouble)
                                    {
                                        currentNode = c;
                                        goto CurrentNodeLoop;
                                    }
                                    break;
                                case Operator.G:
                                    if (fv.AsDouble > c.threshold.AsDouble)
                                    {
                                        currentNode = c;
                                        goto CurrentNodeLoop;
                                    }
                                    break;
                                case Operator.GE:
                                    if (fv.AsDouble >= c.threshold.AsDouble)
                                    {
                                        currentNode = c;
                                        goto CurrentNodeLoop;
                                    }
                                    break;
                                case Operator.L:
                                    if (fv.AsDouble < c.threshold.AsDouble)
                                    {
                                        currentNode = c;
                                        goto CurrentNodeLoop;
                                    }
                                    break;
                                case Operator.LE:
                                    if (fv.AsDouble <= c.threshold.AsDouble)
                                    {
                                        currentNode = c;
                                        goto CurrentNodeLoop;
                                    }
                                    break;
                                case Operator.NE:
                                    if (fv.AsDouble != c.threshold.AsDouble)
                                    {
                                        currentNode = c;
                                        goto CurrentNodeLoop;
                                    }
                                    break;
                                default:
                                    return TextClass.Unknown;
                            }
                        }
                        else if (fv.AsString.Equals(c.threshold.AsString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            currentNode = c;
                            goto CurrentNodeLoop;
                        }
                    }
                }
                traceStringBuilder.Append("(" + currentNode.ToString() + ")");
                trace = traceStringBuilder.ToString();
                //foreach (double ds in currentNode.classDist)
                //    conf += ds;
                //conf = currentNode.classDist[0] / conf * 100;				
                return currentNode.textClass;
            }

            public void Read(TextReader reader)
            {
                // reads a weka-style decision tree
                if (this.root != null)
                {
                    this.root = null;
                    GC.Collect();
                }
                this.root = new Node();
                string line = null;
                string featureName;
                Operator op;
                FeatureValue threshold;
                TextClass textClass;
                List<double> classDist;
                int lastLevel = -1;
                Node currentNode = this.root;
                Node newNode = null;
                while ((line = reader.ReadLine()) != null)
                {
                    int currentLevel = ParseLine(line, out featureName, out op, out threshold, out textClass, out classDist);
                    if (currentLevel < 0)
                        continue;
                    int diff = lastLevel - currentLevel;
                    if (diff >= 0)
                        while (diff-- >= 0)
                            currentNode = currentNode.parent;
                    newNode = new Node(currentNode, featureName, op, threshold, textClass, classDist);
                    currentNode.AddChild(newNode);
                    currentNode = newNode;
                    lastLevel = currentLevel;
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                ToStringTraverse(this.root, 0, ref sb);
                return sb.ToString();
            }

            private void ToStringTraverse(Node n, int level, ref StringBuilder sb)
            {
                for (int i = 0; i < level; i++)
                    sb.Append("|   ");
                sb.AppendLine(n.ToString());
                if (n.Leaf)
                    return;
                foreach (Node c in n.children)
                    ToStringTraverse(c, level + 1, ref sb);
            }

            private int ParseLine(string line, out string featureName, out Operator op,
                out FeatureValue threshold, out TextClass textClass, out List<double> classDist)
            {
                featureName = null;
                op = Operator.Unknown;
                threshold = null;
                textClass = TextClass.Unknown;
                classDist = null;
                if (line.StartsWith("#") || String.IsNullOrEmpty(line))
                    return -1;

                int level = line.Split(new char[] { '|' }, StringSplitOptions.None).Length;
                Match m = nodeRegex.Match(line);
                if (m.Success == true)
                {
                    featureName = m.Result("${feature}");
                    string opStr = m.Result("${op}");
                    if (opStr == ">")
                        op = Operator.G;
                    else if (opStr == ">=")
                        op = Operator.GE;
                    else if (opStr == "<")
                        op = Operator.L;
                    else if (opStr == "<=")
                        op = Operator.LE;
                    else if (opStr == "=")
                        op = Operator.E;
                    else if (opStr == "!=")
                        op = Operator.NE;
                    else
                        op = Operator.Unknown;
                    string strThreshold = m.Result("${threshold}");
                    double numThreshold = 0.0;
                    if (Double.TryParse(strThreshold, out numThreshold))
                        threshold = new FeatureValue(numThreshold);
                    else
                        threshold = new FeatureValue(strThreshold);

                    string strClass = m.Result("${class}");
                    if (strClass.Length > 0)
                    {
                        int intTextClass = Convert.ToInt32(strClass);
                        if ((intTextClass > 5) || (intTextClass < 0))
                            textClass = TextClass.Unknown;
                        else
                            textClass = (TextClass)Math.Pow(2, intTextClass);
                    }
                    else
                        textClass = TextClass.Unknown;

                    string distribution = m.Result("${distribution}");
                    if (distribution.Length > 0)
                    {
                        classDist = new List<double>();
                        foreach (string cd in distribution.Split(new char[1] { '/' }))
                            classDist.Add(Double.Parse(cd));
                    }
                }
                return level;
            }

        }

        private static Regex annotRegex = new Regex(@"class=""x-nc-sel(?<blockClassID>[012345])""", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private HtmlTokenizer tokenizer = new HtmlTokenizer("");
        private DecisionTree decisionTree = new DecisionTree();
        //private Set<string> ignorableTags = new Set<string>(new string[] { "b", "i", "em", "strong", "sub", "sup", "u" });
        private Set<string> ignorableTags = new Set<string>(new string[] { 
            "ins", "del", "bdo", "em", "strong", "dfn", "code", "samp", "kbd", "var", "cite", "abbr", 
            "acronym", "q", "sub", "sup", "tt", "i", "b", "big", "small", "u", "s", "strike", 
            "basefont", "font" });
        private bool mIgnoreDecorativeTags
            = true;

        private static Regex mCommentRegex = new Regex(@"<!--.*?-->", RegexOptions.Compiled);

        public BoilerplateRemover()
        {
            tokenizer.Normalize = false;
        }

        public bool IgnoreDecorativeTags
        {
            get { return mIgnoreDecorativeTags; }
            set { mIgnoreDecorativeTags = value; }
        }

        public static BoilerplateRemover GetDefaultBoilerplateRemover()
        {
            BoilerplateRemover br = new BoilerplateRemover();
            br.ReadDecisionTree(new StreamReader(Utils.GetManifestResourceStream(typeof(BoilerplateRemover), "6ClassAllFeatures.tree")));
            return br;
        }

        private static bool IsTagOpening(string tagName, HtmlTokenizer.Token token)
        {
            if (tagName != "h")
                return (token.TokenType == HtmlTokenizer.TokenType.OpenTag || token.TokenType == HtmlTokenizer.TokenType.StartTag)
                    && tagName == token.TagName;
            else
                return (token.TokenType == HtmlTokenizer.TokenType.OpenTag || token.TokenType == HtmlTokenizer.TokenType.StartTag)
                    && Regex.Match(token.TagName, "^h[1-3]$").Success;
        }

        private static bool IsTagClosing(string tagName, HtmlTokenizer.Token token)
        {
            if (tagName != "h")
                return token.TokenType == HtmlTokenizer.TokenType.EndTag && tagName == token.TagName;
            else
                return token.TokenType == HtmlTokenizer.TokenType.EndTag && Regex.Match(token.TagName, "^h[1-3]$").Success;
        }

        private static SpanTagType GetSpanTagType(HtmlTokenizer.Token token, out TextClass tc)
        {            
            tc = TextClass.Unknown;
            if (token.TagName != "span")
                return SpanTagType.NotSpanTag;
            else if (token.TokenType == HtmlTokenizer.TokenType.EndTag)
                return SpanTagType.ClosingSpanTag;
            else if (token.TokenType == HtmlTokenizer.TokenType.EmptyTag)
                return SpanTagType.OpeningAndClosingSpanTag;

            Match m = annotRegex.Match(token.TokenStr);
            if (!m.Success)
                return SpanTagType.UnknownOpeningSpanTag;
            int bcid = Convert.ToInt32(m.Result("${blockClassID}"));
            if ((bcid > 5) || (bcid < 0))
                return SpanTagType.UnknownOpeningSpanTag;
            tc = (TextClass)Math.Pow(2, bcid);

            return SpanTagType.KnownOpeningSpanTag;
        }

        private HtmlTokenizer.Enumerator TokenizeHtml(TextReader reader)
        {
            tokenizer.Text = reader.ReadToEnd();

            return (HtmlTokenizer.Enumerator)tokenizer.GetEnumerator();
        }

        private List<HtmlBlock> GetHtmlBlocks(TextReader reader, bool isAnnotated)
        {
            HtmlTokenizer.Enumerator tokensEnum = TokenizeHtml(reader);
            if (tokensEnum == null)
                return null;

            int pos = 0;
            bool isInAnchor = false;
            bool isInP = false;
            bool isInH = false;

            List<HtmlBlock> blocks = new List<HtmlBlock>();
            HtmlBlock prevBlock = new HtmlBlock(pos, -1);
            blocks.Add(prevBlock);
            HtmlBlock currentBlock = new HtmlBlock(++pos, 0);

            Stack<TextClass> textClasses = new Stack<TextClass>();
            textClasses.Push(TextClass.Boilerplate);
            TextClass currentClass = TextClass.Boilerplate;
            TextClass textClass = TextClass.Unknown;
            while (tokensEnum.MoveNext())
            {
                string t = tokensEnum.Current;
                //Console.WriteLine(t + " " + tokensEnum.CurrentToken.TokenType);
                if (tokensEnum.CurrentToken.IsTag)
                {
                    if (mIgnoreDecorativeTags && ignorableTags.Contains(tokensEnum.CurrentToken.TagName))
                    {
                        continue;
                    }
                    if (IsTagOpening("a", tokensEnum.CurrentToken))
                    {
                        isInAnchor = true;
                        currentBlock.Add(t, tokensEnum.CurrentToken.TokenType, false, true);
                    }
                    else if (IsTagClosing("a", tokensEnum.CurrentToken))
                    {
                        isInAnchor = false;
                        currentBlock.Add(t, tokensEnum.CurrentToken.TokenType, false, true);
                    }
                    else
                    {
                        if (IsTagOpening("p", tokensEnum.CurrentToken))
                        {
                            isInP = true;
                        }
                        else if (IsTagOpening("h", tokensEnum.CurrentToken))
                        {
                            isInH = true;
                        }
                        if (isAnnotated)
                        {
                            SpanTagType spanType = GetSpanTagType(tokensEnum.CurrentToken, out textClass);
                            if (spanType != SpanTagType.NotSpanTag && spanType != SpanTagType.OpeningAndClosingSpanTag)
                            {
                                if ((spanType == SpanTagType.ClosingSpanTag) && (textClasses.Count > 1))
                                    textClasses.Pop();
                                else if (spanType != SpanTagType.ClosingSpanTag)
                                    textClasses.Push(textClass);
                                foreach (TextClass c in textClasses)
                                    if (c != TextClass.Unknown)
                                    {
                                        currentClass = c;
                                        break;
                                    }
                                continue;
                            }
                        }
                        if (currentBlock.HasWords)
                        {
                            currentBlock.isInP = isInP;
                            currentBlock.isInH = isInH;
                            currentBlock.endListIdx = tokensEnum.CurrentTokenListIdx;
                            CalcBlockFeatures(currentBlock, prevBlock);
                            prevBlock = currentBlock;
                            blocks.Add(currentBlock);
                            currentBlock = new HtmlBlock(++pos, tokensEnum.CurrentTokenListIdx + 1);
                        }
                        else
                        {
                            currentBlock.startListIdx = tokensEnum.CurrentTokenListIdx + 1;
                            currentBlock.endListIdx = -1;
                        }

                        if (IsTagClosing("p", tokensEnum.CurrentToken))
                        {
                            isInP = false;
                        }
                        else if (IsTagClosing("h", tokensEnum.CurrentToken))
                        {
                            isInH = false;
                        }

                    }
                }
                else
                {
                    currentBlock.Add(t, tokensEnum.CurrentToken.TokenType, isInAnchor, false);
                    if (isAnnotated)
                        currentBlock.textClass = currentClass;
                }
            }
            // flush
            if (currentBlock.HasWords)
            {
                currentBlock.endListIdx = tokenizer.Tokens.Count;
                CalcBlockFeatures(currentBlock, prevBlock);
                blocks.Add(currentBlock);
            }

            // dummy
            blocks.Add(new HtmlBlock(++pos, -1));

            // calc relative position
            foreach (HtmlBlock b in blocks)
                b.relPosition = b.relPosition / (double)(blocks.Count - 2);

            return blocks;
        }

        private string GetTextBlock(int startListIdx, int endListIdx, bool decode, bool compact)
        {
            return GetTextBlock(startListIdx, endListIdx, decode, compact, /*removeComments=*/true);
        }

        private string GetTextBlock(int startListIdx, int endListIdx, bool decode, bool compact, bool removeComments)
        {
            if (startListIdx >= tokenizer.Tokens.Count) { return ""; }
            int startTextIdx = 0;
            if (tokenizer.Tokens[startListIdx].IsTag)
            {
                startTextIdx = tokenizer.Tokens[startListIdx].StartIndex + tokenizer.Tokens[startListIdx].Length;
            }
            else if (startListIdx - 1 >= 0)
            {
                startTextIdx = tokenizer.Tokens[startListIdx - 1].StartIndex + tokenizer.Tokens[startListIdx - 1].Length;
            }
            StringBuilder textBlock = new StringBuilder();
            for (int i = startListIdx + 1; i <= endListIdx; i++)
            {
                if (i == tokenizer.Tokens.Count)
                {
                    string text = tokenizer.Text.Substring(startTextIdx, tokenizer.Text.Length - startTextIdx);
                    if (removeComments) { text = mCommentRegex.Replace(text, ""); }
                    if (decode) { text = HttpUtility.HtmlDecode(text); }
                    if (compact) { text = Utils.ToOneLine(text, /*compact=*/true); }
                    textBlock.Append(text);
                    return textBlock.ToString();
                }
                if (tokenizer.Tokens[i].IsTag)
                {
                    string text = tokenizer.Text.Substring(startTextIdx, tokenizer.Tokens[i].StartIndex - startTextIdx);
                    if (removeComments) { text = mCommentRegex.Replace(text, ""); }
                    if (decode) { text = HttpUtility.HtmlDecode(text); }
                    if (compact) { text = Utils.ToOneLine(text, /*compact=*/true); }
                    if (text != "") { textBlock.Append(text + " "); }
                    startTextIdx = tokenizer.Tokens[i].StartIndex + tokenizer.Tokens[i].Length;
                }
            }
            return textBlock.Length > 0 ? textBlock.ToString().Remove(textBlock.Length - 1) : "";
        }

        private void CalcBlockFeatures(HtmlBlock b, HtmlBlock prevB)
        {
            if (b.Empty)
            {
                b.linkDensity = 0.0;
                b.textDensity = 0.0;
                b.numWords = 0;
                return;
            }
            b.linkDensity = b.numTokensInsideAnchors / (double)b.tokenCount;
            if (b.linkDensity > 1.0)
                b.linkDensity = 1.0;
            int lines = b.charCount / 80;
            if (lines <= 1)
                b.textDensity = b.tokenCount;
            else
                b.textDensity = b.tokenCount / (double)(lines - 1);

            b.textDensityQuotPC = prevB.textDensity / b.textDensity;
            if (b.numWords > 0)
            {
                b.avgWordLen = b.wordLengthSum / (double)b.numWords;
                b.capitalWordsRatio = b.numCapitalWords / (double)b.numWords;
                b.capitalStartWordsRatio = b.numCapitalStartWords / (double)b.numWords;
                b.numWordsQuotPC = prevB.numWords / (double)b.numWords;
                b.capitalWordsRatio = b.numCapitalWords / (double)b.numWords;
            }
            else
            {
                b.avgWordLen = 0;
                b.fullstopRatio = 0;
                b.capitalWordsRatio = 0;
                b.capitalStartWordsRatio = 0;
                b.numWordsQuotPC = 0;
                b.capitalWordsRatio = 0;
            }

            b.text = GetTextBlock(b.startListIdx, b.endListIdx, /*decode=*/true, /*compact=*/true);
            int indx = -1;
            while ((indx = b.text.IndexOf(".", indx + 1)) != -1)
                b.numFullstops++;
            b.fullstopRatio = b.numFullstops / (double)b.numWords;
        }

        public void ReadDecisionTree(TextReader reader)
        {
            decisionTree.Read(reader);
        }

        public string ExtractMainContentText(TextReader reader)
        {
            List<HtmlBlock> foo;
            return ExtractText(reader, TextClass.FullText, out foo);
        }

        public string ExtractMainContentText(TextReader reader, out List<HtmlBlock> blocks)
        {
            return ExtractText(reader, TextClass.FullText, out blocks);
        }

        public string ExtractAllButBoilerplate(TextReader reader)
        {
            List<HtmlBlock> foo;
            return ExtractText(reader, TextClass.FullText | TextClass.Headline |
                TextClass.RelatedContent | TextClass.Supplement | TextClass.UserComment, out foo);
        }

        public string ExtractAllButBoilerplate(TextReader reader, out List<HtmlBlock> blocks)
        {
            return ExtractText(reader, TextClass.FullText | TextClass.Headline |
                TextClass.RelatedContent | TextClass.Supplement | TextClass.UserComment, out blocks);
        }

        public string ExtractText(TextReader reader, TextClass textClass, out List<HtmlBlock> blocks)
        {
            blocks = GetHtmlBlocks(reader, false);
            StringBuilder sb = new StringBuilder();
            TextClass tc = TextClass.Unknown;
            for (int i = 1; i < blocks.Count - 1; i++)
            {
                if ((tc = decisionTree.Predict(blocks[i - 1], blocks[i], blocks[i + 1])) != TextClass.Unknown)
                {
                    if ((tc & textClass) > 0)
                    {
                        sb.Append(blocks[i].text + " ");
                    }
                }
            }
            return sb.Length == 0 ? "" : sb.ToString().Remove(sb.Length - 1);
        }

        private string GetColorName(TextClass textClass)
        {
            switch (textClass)
            { 
                case TextClass.Boilerplate:
                    return "gray";
                case TextClass.FullText:
                    return "yellow";
                case TextClass.Headline:
                    return "red";
                default:
                    return "lime";
            }
        }

        public string AnnotateHtml(TextReader reader, bool encode)
        {
            List<HtmlBlock> blocks = GetHtmlBlocks(reader, false);
            StringBuilder sb = new StringBuilder();
            TextClass tc = TextClass.Unknown;
            int currentIdx = 0; 
            for (int i = 1; i < blocks.Count - 1; i++)
            {
                if ((tc = decisionTree.Predict(blocks[i - 1], blocks[i], blocks[i + 1])) != TextClass.Unknown)
                {
                    int startListIdx = blocks[i].startListIdx;
                    int startTextIdx = 0;
                    if (tokenizer.Tokens[startListIdx].IsTag)
                    {
                        startTextIdx = tokenizer.Tokens[startListIdx].StartIndex; 
                    }
                    else if (startListIdx - 1 >= 0)
                    {
                        startTextIdx = tokenizer.Tokens[startListIdx - 1].StartIndex + tokenizer.Tokens[startListIdx - 1].Length;
                    }
                    int endListIdx = blocks[i].endListIdx;
                    int endTextIdx = tokenizer.Text.Length;
                    if (endListIdx < tokenizer.Tokens.Count)
                    {
                        endTextIdx = tokenizer.Tokens[endListIdx].StartIndex;
                    }
                    if (currentIdx < startTextIdx) 
                    {
                        sb.Append(encode ? HttpUtility.HtmlEncode(tokenizer.Text.Substring(currentIdx, startTextIdx - currentIdx))
                            : tokenizer.Text.Substring(currentIdx, startTextIdx - currentIdx));
                    }
                    sb.Append(string.Format("<span title=\"{0}\" style=\"background-color:{1}\">", tc, GetColorName(tc)));
                    sb.Append(encode ? HttpUtility.HtmlEncode(tokenizer.Text.Substring(startTextIdx, endTextIdx - startTextIdx)) 
                        : tokenizer.Text.Substring(startTextIdx, endTextIdx - startTextIdx));
                    sb.Append("</span>");
                    currentIdx = endTextIdx;
                }
            }
            sb.Append(encode ? HttpUtility.HtmlEncode(tokenizer.Text.Substring(currentIdx, tokenizer.Text.Length - currentIdx))
                : tokenizer.Text.Substring(currentIdx, tokenizer.Text.Length - currentIdx));
            return sb.ToString();
        }
    }
}