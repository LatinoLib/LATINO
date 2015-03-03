using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    public class TextFeatureProcessor
    {
        private readonly List<TextFeature> mFeatures = new List<TextFeature>();
        private readonly List<string> mAppends = new List<string>();
        private readonly List<string> mDistinctAppends = new List<string>();

        public List<TextFeature> Features
        {
            get
            {
                return mFeatures;
            }
        }

        public string Run(string text)
        {
            mAppends.Clear();
            mDistinctAppends.Clear();
            foreach (TextFeature feature in mFeatures)
            {
                switch (feature.Operation)
                {
                    case TextFeatureOperation.Replace:
                        text = feature.Regex.Replace(text, feature.MarkToken);
                        break;

                    case TextFeatureOperation.Custom:
                        text = feature.PerformCustomOperation(text);
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
            mFeatures.Add(Preconditions.CheckNotNullArgument(feature));
            return this;
        }

        public TextFeatureProcessor With(IEnumerable<TextFeature> features)
        {
            mFeatures.AddRange(Preconditions.CheckNotNullArgument(features));
            return this;
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

    public abstract class TextFeature
    {
        private readonly string mMarkToken;
        private Regex mRegex;

        protected TextFeature(string markToken)
        {
            mMarkToken = Preconditions.CheckNotNullArgument(markToken);
            Operation = TextFeatureOperation.Replace;
            IsEmcloseMarkTokenWithSpace = false;
        }

        public string MarkToken
        {
            get
            {
                return IsEmcloseMarkTokenWithSpace 
                    ? string.Format(" {0} ", mMarkToken) 
                    : mMarkToken;
            }
        }

        public TextFeatureOperation Operation { get; set; }
        public bool IsEmcloseMarkTokenWithSpace { get; set; }

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

        protected internal virtual string PerformCustomOperation(string input)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0} [op: {1}; mark: {2}]", GetType().Name, Operation, MarkToken);
        }
    }

    public class CustomTextFeature : TextFeature
    {
        public CustomTextFeature(string markToken) : base(markToken)
        {
        }

        public Func<string> OnGetPattern { get; set; }

        protected override string GetPattern(ref RegexOptions options)
        {
            Preconditions.CheckNotNullArgument(OnGetPattern);
            return OnGetPattern();
        }
    }

    public class AnyOfTermsTextFeature : TextFeature
    {
        public AnyOfTermsTextFeature(IEnumerable<string> terms, string markToken) : base(markToken)
        {
            Terms = new Strings(terms);
        }

        protected AnyOfTermsTextFeature(string markToken) : base(markToken)
        {
        }

        public Strings Terms { get; protected set; }
        public bool IsEncloseTermLetterEdges { get; set; }
        public bool IsEncloseAllTerms { get; set; }
        public RegexOptions RegexOptions { get; set; }

        protected override string GetPattern(ref RegexOptions options)
        {
            options |= RegexOptions;
            if (IsEncloseTermLetterEdges || !IsEncloseAllTerms)
            {
                return "(" + string.Join("|", Terms
                    .OrderByDescending(s => s.Length)
                    .Select(Regex.Escape)
                    .Select(s => string.Format("{0}{1}{2}", Char.IsLetter(s.First()) ? "\\b" : "", s, Char.IsLetter(s.Last()) ? "\\b" : ""))
                    .ToArray()) + ")";
            }
            return IsEncloseAllTerms 
                ? string.Format(@"\b({0})\b", string.Join("|", Terms.OrderByDescending(s => s.Length).Select(Regex.Escape).ToArray())) 
                : string.Format(@"({0})", string.Join("|", Terms.OrderByDescending(s => s.Length).Select(Regex.Escape).ToArray()));
        }
    }
}