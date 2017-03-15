using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Latino.TextMining
{
    public enum TextFeatureOperation
    {
        Replace,
        Append,
        AppendDistinct,
        Custom
    }

    public class TextFeatureProcessor : ISerializable
    {
        private readonly List<TextFeature> mFeatures = new List<TextFeature>();
        private readonly List<string> mAppends = new List<string>();
        private readonly List<string> mDistinctAppends = new List<string>();

        public TextFeatureProcessor()
        {
        }

        public TextFeatureProcessor(BinarySerializer reader)
        {
            Load(reader);
        }

        static TextFeatureProcessor()
        {
            IsRecreateFeaturesAfterLoad = false;
        }

        public static bool IsRecreateFeaturesAfterLoad { get; set; }


        public List<TextFeature> Features
        {
            get
            {
                return mFeatures;
            }
        }

        public string Run(string text, Dictionary<string, object> namedValues = null)
        {
            mAppends.Clear();
            mDistinctAppends.Clear();
            foreach (TextFeature feature in mFeatures
                .SelectMany(f => f is TextFeatureGroup ? ((TextFeatureGroup)f).Features : new[] { f }))
            {
                switch (feature.Operation)
                {
                    case TextFeatureOperation.Replace:
                        text = feature.Regex.Replace(text, feature.MarkToken);
                        break;

                    case TextFeatureOperation.Custom:
                        string[] appends, distinctAppends;
                        text = feature.PerformCustomOperation(text, namedValues, out  appends, out distinctAppends);
                        if (appends != null) { mAppends.AddRange(appends); }
                        if (distinctAppends != null) { mDistinctAppends.AddRange(distinctAppends); }
                        break;

                    case TextFeatureOperation.Append:
                        if (feature.Regex.IsMatch(text))
                        {
                            mAppends.Add(feature.MarkToken);
                        }
                        break;

                    case TextFeatureOperation.AppendDistinct:
                        if (feature.Regex.IsMatch(text))
                        {
                            mDistinctAppends.Add(feature.MarkToken);
                        }
                        break;
                }
            }
            if (mAppends.Any())
            {
                text += " " + string.Join(" ", mAppends);
            }
            if (mDistinctAppends.Any())
            {
                text += " " + string.Join(" ", mDistinctAppends.Distinct());
            }
            return text;
        }

        public TextFeatureProcessor With(TextFeature feature)
        {
            mFeatures.Add(Preconditions.CheckNotNull(feature));
            return this;
        }

        public TextFeatureProcessor With(IEnumerable<TextFeature> features)
        {
            mFeatures.AddRange(Preconditions.CheckNotNull(features));
            return this;
        }

        public void Save(BinarySerializer writer)
        {
            writer.WriteInt(mFeatures.Count);
            foreach (TextFeature feature in mFeatures)
            {
                writer.WriteObject(feature);
            }
        }

        public void Load(BinarySerializer reader)
        {
            mFeatures.Clear();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var textFeature = reader.ReadObject<TextFeature>();
                if (IsRecreateFeaturesAfterLoad)
                {
                    // derived features must have default ctor (or at lleast one optional parameters only) 
                    mFeatures.Add((TextFeature)Utils.CreateInstance(textFeature.GetType()));
                }
                else
                {
                    mFeatures.Add(textFeature);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(GetType().Name).Append(":\n");
            foreach (TextFeature feature in mFeatures)
            {
                sb.AppendLine(feature.ToString());
            }
            return sb.ToString();
        }
    }


    public abstract class TextFeature : ISerializable
    {
        private string mMarkToken;
        private Regex mRegex;

        protected TextFeature(string markToken)
        {
            mMarkToken = Preconditions.CheckNotNull(markToken);
            Operation = TextFeatureOperation.Replace;
            IsEncloseMarkTokenWithSpace = false;
        }

        protected TextFeature(BinarySerializer reader)
        {
            Load(reader);
        }

        public string MarkToken
        {
            get
            {
                return IsEncloseMarkTokenWithSpace 
                    ? string.Format(" {0} ", mMarkToken) 
                    : mMarkToken;
            }
        }

        public TextFeatureOperation Operation { get; set; }
        public bool IsEncloseMarkTokenWithSpace { get; set; }

        public Regex Regex
        {
            get
            {
                if (mRegex == null)
                {
                    var regexOptions = RegexOptions.None;
                    string pattern = GetPattern(ref regexOptions);
                    mRegex = new Regex(pattern, regexOptions | RegexOptions.Compiled);
                }
                return mRegex;
            }
        }

        protected virtual string GetPattern(ref RegexOptions options)
        {
            throw new NotImplementedException();
        }

        protected internal virtual string PerformCustomOperation(string input, Dictionary<string, object> namedValues, out string[] appends, out string[] distinctAppends)
        {
            appends = distinctAppends = null;
            return PerformCustomOperation(input, namedValues);
        }

        protected internal virtual string PerformCustomOperation(string input, Dictionary<string, object> namedValues)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0} [op: {1}; mark: {2}]", GetType().Name, Operation, MarkToken);
        }

        public virtual void Save(BinarySerializer writer)
        {
            writer.WriteString(mMarkToken);
            writer.WriteValue(Operation);
            writer.WriteBool(IsEncloseMarkTokenWithSpace);
            writer.WriteString(mRegex == null ? "" : mRegex.ToString());
        }

        public void Load(BinarySerializer reader)
        {
            mMarkToken = reader.ReadString();
            Operation = reader.ReadValue<TextFeatureOperation>();
            IsEncloseMarkTokenWithSpace = reader.ReadBool();
            string pattern = reader.ReadString();
            mRegex = pattern ==  "" ? null : new Regex(pattern);
        }
    }


    public class TextFeatureGroup : TextFeature
    {
        protected TextFeatureGroup(TextFeature[] features) : base("")
        {
            Operation = TextFeatureOperation.Custom;
            Features = Preconditions.CheckNotNull(features);
        }

        public TextFeatureGroup(BinarySerializer reader) : base(reader)
        {
            Load(reader);
        }

        public TextFeature[] Features { get; protected set; }

        protected internal override string PerformCustomOperation(string input, Dictionary<string, object> namedValues)
        {
            throw new InvalidOperationException();
        }

        public new void Load(BinarySerializer reader)
        {
            int count = reader.ReadInt();
            Features = new TextFeature[count];
            for (int i = 0; i < count; i++)
            {
                var textFeature = reader.ReadObject<TextFeature>();
                if (TextFeatureProcessor.IsRecreateFeaturesAfterLoad)
                {
                    // derived features must have default ctor (or at lleast one optional parameters only) 
                    Features[i] = (TextFeature)Utils.CreateInstance(textFeature.GetType());
                }
                else
                {
                    Features[i] = textFeature;
                }
            }
        }

        public override void Save(BinarySerializer writer)
        {
            base.Save(writer);
            writer.WriteInt(Features.Length);
            foreach (TextFeature feature in Features)
            {
                writer.WriteObject(feature);
            }
        }
    }


    public class AnyOfTermsTextFeature : TextFeature
    {
        [Flags]
        public enum EncloseOption
        {
            None = 0,
            OnlyLetterEdges = 1 << 0,
            LeftEdge = 1 << 1,
            RightEdge = 1 << 2,
            BothEdges = LeftEdge | RightEdge
        }

        public AnyOfTermsTextFeature(IEnumerable<string> terms, string markToken) : base(markToken)
        {
            Terms = new Strings(terms);
        }

        public AnyOfTermsTextFeature(BinarySerializer reader) : base(reader)
        {
            Load(reader);
        }

        protected AnyOfTermsTextFeature(string markToken) : base(markToken)
        {
        }

        public Strings Terms { get; protected set; }
        public EncloseOption WordBoundEnclosing { get; set; }
        public RegexOptions RegexOptions { get; set; }

        protected override string GetPattern(ref RegexOptions options)
        {
            options |= RegexOptions;
            if (WordBoundEnclosing.HasFlag(EncloseOption.OnlyLetterEdges))
            {
                return "(" + string.Join("|", Terms
                    .OrderByDescending(s => s.Length)
                    .Select(Regex.Escape)
                    .Select(s => string.Format("{0}{1}{2}", Char.IsLetter(s.First()) ? "\\b" : "", s, Char.IsLetter(s.Last()) ? "\\b" : ""))
                    .ToArray()) + ")";
            }

            if ((WordBoundEnclosing & EncloseOption.BothEdges) != 0)
            {
                return string.Format(@"{0}({1}){2}", 
                    WordBoundEnclosing.HasFlag(EncloseOption.LeftEdge) ? "\\b" : "", 
                    string.Join("|", Terms.OrderByDescending(s => s.Length).Select(Regex.Escape).ToArray()),
                    WordBoundEnclosing.HasFlag(EncloseOption.RightEdge) ? "\\b" : ""
                );
            }

            return string.Format(@"({0})", string.Join("|", Terms.OrderByDescending(s => s.Length).Select(Regex.Escape).ToArray()));
        }

        public override void Save(BinarySerializer writer)
        {
            base.Save(writer);
            Terms.Save(writer);
            writer.WriteValue(WordBoundEnclosing);
            writer.WriteValue(RegexOptions);
        }

        public new void Load(BinarySerializer reader)
        {
            Terms = new Strings(reader);
            WordBoundEnclosing = reader.ReadValue<EncloseOption>();
            RegexOptions = reader.ReadValue<RegexOptions>();
        }
    }
}