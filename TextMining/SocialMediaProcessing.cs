using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Latino.TextMining
{

    public static class Emoticons
    {

        // punctuation emoticons
        // source: http://en.wikipedia.org/wiki/List_of_emoticons Feb., 2015
        public static readonly string[] SmileyOrHappyFace =
            {
                ":-)", ":)", ":D", ":o)", ":]", ":3", ":c)", ":>", "=]", "8)", "=)", ":}", ":^)", ":っ)"
            };
        public static readonly string[] LaughingbigGrinlaughWithGlasses =
            {
                ":-D", "8-D", "8D", "x-D", "xD", "X-D", "XD", "=-D", "=D", "=-3", "=3", "B^D"
            };
        public static readonly string[] VeryHappyOrDoubleChin = { ":-))" };
        public static readonly string[] FrownSad =
            {
                ">:[", ":-(", ":(", ":-c", ":c", ":-<", ":っC", ":<", ":-[", ":[", ":{"
            };
        public static readonly string[] WinkyFrowny = { ";(" };
        public static readonly string[] Angry = { ":-||", ":@", ">:(" };
        public static readonly string[] Crying = { ":'-(", ":'(" };
        public static readonly string[] TearsOfHappiness = { ":'-)", ":')" };
        public static readonly string[] HorrorDisgustSadnessGreatDismay = { "D:<", "D:", "D8", "D;", "D=", "DX", "v.v", "D-':" };
        public static readonly string[] SurpriseShockYawn =
            {
                ">:O", ":-O", ":O", ":-o", ":o", "8-0", "O_O", "o-o", "O_o", "o_O", "o_o", "O-O"
            };
        public static readonly string[] Kiss = { ":*", ":^*", "('}{')" };
        public static readonly string[] WinkSmirk = { ";-)", ";)", "*-)", "*)", ";-]", ";]", ";D", ";^)", ":-" };
        public static readonly string[] TongueStickingOutCheekyPlayful =
            {
                ">:P", ":-P", ":P", "X-P", "x-p", "xp", "XP", ":-p", ":p", "=p", ":-Þ", ":Þ", ":þ", ":-þ", ":-b", ":b", "d:"
            };
        public static readonly string[] SkepticalAnnoyedUndecidedUneasyHesitant =
            {
                @">:\", ">:/", ":-/", ":-.", ":/", @":\", "=/", @"=\ :L", "=L :S", ">.<", "-_-", "-.-"
            };

        public static readonly string[] StraightFaceNoExpressionIndecision = { ":|", ":-|" };
        public static readonly string[] EmbarrassedBlushing = { ":$" };
        public static readonly string[] SealedLipsOrWearingBraces = { ":-X", ":X", ":-#", ":#" };
        public static readonly string[] AngelSaintInnocent = { "O:-)", "0:-3", "0:3", "0:-)", "0:)", "0;^)" };
        public static readonly string[] Evil = { ">:)", ">;)", ">:-)" };
        public static readonly string[] Devilish = { "}:-)", "}:)", "3:-)", "3:)" };
        public static readonly string[] HighFive = { @"o/\o", "^5", ">_>^", "^<_<" };
        public static readonly string[] BoredYawning = { "|-O" };
        public static readonly string[] Cool = { "|;-)" };
        public static readonly string[] TongueInCheek = { "-J" };
        public static readonly string[] TongueTied = { ":-&", ":&" };
        public static readonly string[] PartiedAllNight = { "#-)" };
        public static readonly string[] DrunkConfused = { "%-)", "%)" };
        public static readonly string[] BeingSick = { ":-###..", ":###.." };
        public static readonly string[] DumbDunceLike = { "<:-|" };
        public static readonly string[] LookOfDisapproval = { "ಠ_ಠ" };
        public static readonly string[] FishSomethingFishy = { "<*)))-{", "><(((*>", "><>" };
        public static readonly string[] Cheer = { @"\o/" };
        public static readonly string[] Cheerleader = { @"*\0/*" };
        public static readonly string[] Rose = { "@}-;-'---", "@>-->--" };
        public static readonly string[] HomerSimpson = { "~(_8^(I)" };
        public static readonly string[] ElvisPresley = { "5:-)", @"~:-\" };
        public static readonly string[] JohnLennon = { @"//0-0\\" };
        public static readonly string[] SantaClaus = { "*<|:-)" };
        public static readonly string[] BillClinton = { "=:o]" };
        public static readonly string[] RonaldReagan = { ",:-)", "7:^]" };
        public static readonly string[] Heart = { "<3" };
        public static readonly string[] BrokenHeart = { "</3" };
        public static readonly string[] LennyFace = { "( ͡° ͜ʖ ͡°)" };
    }


    public static class SocialMediaProcessing
    {

        public static readonly Strings BasicHappyEmoticons = new Strings(
            Emoticons.SmileyOrHappyFace, Emoticons.Rose, Emoticons.Heart
        );

        public static readonly Strings HappyEmoticons = new Strings(
            Emoticons.SmileyOrHappyFace, Emoticons.LaughingbigGrinlaughWithGlasses, 
            Emoticons.VeryHappyOrDoubleChin, Emoticons.TearsOfHappiness, Emoticons.Kiss,
            Emoticons.WinkSmirk, Emoticons.TongueStickingOutCheekyPlayful, Emoticons.AngelSaintInnocent, 
            Emoticons.HighFive, Emoticons.Cool, Emoticons.TongueInCheek, Emoticons.DumbDunceLike, 
            Emoticons.Cheer, Emoticons.Cheerleader, Emoticons.Rose, Emoticons.Heart, Emoticons.LennyFace
        );

        public static readonly Strings BasicSadEmoticons = new Strings(
            Emoticons.FrownSad, Emoticons.WinkyFrowny, Emoticons.Angry, Emoticons.StraightFaceNoExpressionIndecision, Emoticons.BrokenHeart
        );

        public static readonly Strings SadEmoticons = new Strings(
            Emoticons.FrownSad, Emoticons.WinkyFrowny, Emoticons.Angry, Emoticons.Crying, Emoticons.HorrorDisgustSadnessGreatDismay, 
            Emoticons.SurpriseShockYawn, Emoticons.SkepticalAnnoyedUndecidedUneasyHesitant, Emoticons.StraightFaceNoExpressionIndecision,
            Emoticons.SealedLipsOrWearingBraces, Emoticons.BoredYawning, Emoticons.DrunkConfused, Emoticons.BeingSick, 
            Emoticons.LookOfDisapproval, Emoticons.FishSomethingFishy, Emoticons.BrokenHeart
        );


        // note: emoticon features are to be used after ReplaceUrls
        public class BasicHappyEmoticonsFeature : AnyOfTermsTextFeature
        {
            public BasicHappyEmoticonsFeature(string markToken = "__BASIC_HAPPY_EMOTICON__")
                : base(markToken)
            {
                Terms = BasicHappyEmoticons;
                Operation = TextFeatureOperation.Replace;
                WordBoundEnclosing = EncloseOption.OnlyLetterEdges;
            }

            public BasicHappyEmoticonsFeature(BinarySerializer reader) : base(reader)
            {
            }
        }

        public class BasicSadEmoticonsFeature : AnyOfTermsTextFeature
        {
            public BasicSadEmoticonsFeature(string markToken = "__BASIC_SAD_EMOTICON__") : base(markToken)
            {
                Terms = BasicSadEmoticons;
                Operation = TextFeatureOperation.Replace;
                WordBoundEnclosing = EncloseOption.OnlyLetterEdges;
            }

            public BasicSadEmoticonsFeature(BinarySerializer reader) : base(reader)
            {
            }
        }

        public class HappyEmoticonsFeature : AnyOfTermsTextFeature
        {
            public HappyEmoticonsFeature(string markToken = "__HAPPY_EMOTICON__")
                : base(markToken)
            {
                Terms = HappyEmoticons;
                Operation = TextFeatureOperation.Replace;
                WordBoundEnclosing = EncloseOption.OnlyLetterEdges;
            }

            public HappyEmoticonsFeature(BinarySerializer reader) : base(reader)
            {
            }
        }

        public class SadEmoticonsFeature : AnyOfTermsTextFeature
        {
            public SadEmoticonsFeature(string markToken = "__SAD_EMOTICON__") : base(markToken)
            {
                Terms = SadEmoticons;
                Operation = TextFeatureOperation.Replace;
                WordBoundEnclosing = EncloseOption.OnlyLetterEdges;
            }

            public SadEmoticonsFeature(BinarySerializer reader) : base(reader)
            {
            }
        }

        public class LastHappyEmoticonsFeature : HappyEmoticonsFeature
        {
            public LastHappyEmoticonsFeature(string markToken = "__LAST_HAPPY_EMOTICON__") : base(markToken)
            {
                Operation = TextFeatureOperation.Append;
            }

            public LastHappyEmoticonsFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return string.Format("{0}$", base.GetPattern(ref options));
            }
        }

        public class LastSadEmoticonsFeature : SadEmoticonsFeature
        {
            public LastSadEmoticonsFeature(string markToken = "__LAST_SAD_EMOTICON__") : base(markToken)
            {
                Operation = TextFeatureOperation.Append;
            }

            public LastSadEmoticonsFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return string.Format("{0}$", base.GetPattern(ref options));
            }
        }

        public class HappySadEmoticonsFeature : TextFeatureGroup
        {

            public HappySadEmoticonsFeature(string markToken = null) : base(GetFeatures(markToken))
            {
            }

            public HappySadEmoticonsFeature(BinarySerializer reader) : base(reader)
            {
            }

            private static TextFeature[] GetFeatures(string markToken)
            {
                if (markToken == null)
                {
                    return new TextFeature[] {
                        new HappyEmoticonsLenOverTwoFeature(),
                        new SadEmoticonsLenOverTwoFeature(),
                        new HappyEmoticonsLenTwoFeature(),
                        new SadEmoticonsLenTwoFeature() 
                    };
                }
                return new TextFeature[] {
                    new HappyEmoticonsLenOverTwoFeature(markToken),
                    new SadEmoticonsLenOverTwoFeature(markToken),
                    new HappyEmoticonsLenTwoFeature(markToken),
                    new SadEmoticonsLenTwoFeature(markToken) 
                };
            }

            public class HappyEmoticonsLenTwoFeature : HappyEmoticonsFeature
            {
                public HappyEmoticonsLenTwoFeature(string markToken = "__HAPPY_EMOTICON__") : base(markToken)
                {
                    Terms = new Strings(Terms.Where(s => s.Length <= 2));
                }

                public HappyEmoticonsLenTwoFeature(BinarySerializer reader) : base(reader)
                {
                }
            }

            public class SadEmoticonsLenTwoFeature : SadEmoticonsFeature
            {
                public SadEmoticonsLenTwoFeature(string markToken = "__SAD_EMOTICON__") : base(markToken)
                {
                    Terms = new Strings(Terms.Where(s => s.Length <= 2));
                }

                public SadEmoticonsLenTwoFeature(BinarySerializer reader) : base(reader)
                {
                }
            }

            public class HappyEmoticonsLenOverTwoFeature : HappyEmoticonsFeature
            {
                public HappyEmoticonsLenOverTwoFeature(string markToken = "__HAPPY_EMOTICON__")
                    : base(markToken)
                {
                    Terms = new Strings(Terms.Where(s => s.Length > 2));
                }

                public HappyEmoticonsLenOverTwoFeature(BinarySerializer reader)
                    : base(reader)
                {
                }
            }

            public class SadEmoticonsLenOverTwoFeature : SadEmoticonsFeature
            {
                public SadEmoticonsLenOverTwoFeature(string markToken = "__SAD_EMOTICON__")
                    : base(markToken)
                {
                    Terms = new Strings(Terms.Where(s => s.Length > 2));
                }

                public SadEmoticonsLenOverTwoFeature(BinarySerializer reader)
                    : base(reader)
                {
                }
            }
        }


        // unicode emoticons
        // official from http://www.unicode.org/charts/PDF/U1F600.pdf

        // Faces		
        public static readonly string GrinningFace = char.ConvertFromUtf32(0x1F600); //      😀
        public static readonly string GrinningFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F601); //      😁
        public static readonly string FaceWithTearsOfJoy = char.ConvertFromUtf32(0x1F602); //      😂
        public static readonly string SmilingFaceWithOpenMouth = char.ConvertFromUtf32(0x1F603); //      😃
        public static readonly string SmilingFaceWithOpenMouthAndSmilingEyes = char.ConvertFromUtf32(0x1F604); //      😄
        public static readonly string SmilingFaceWithOpenMouthAndColdSweat = char.ConvertFromUtf32(0x1F605); //      😅
        public static readonly string SmilingFaceWithOpenMouthAndTightlyClosedEyes = char.ConvertFromUtf32(0x1F606); //      😆
        public static readonly string SmilingFaceWithHalo = char.ConvertFromUtf32(0x1F607); //      😇
        public static readonly string SmilingFaceWithHorns = char.ConvertFromUtf32(0x1F608); //      😈
        public static readonly string WinkingFace = char.ConvertFromUtf32(0x1F609); //      😉
        public static readonly string SmilingFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F60A); //      😊
        public static readonly string FaceSavouringDeliciousFood = char.ConvertFromUtf32(0x1F60B); //      😋
        public static readonly string RelievedFace = char.ConvertFromUtf32(0x1F60C); //      😌
        public static readonly string SmilingFaceWithHeartShapedEyes = char.ConvertFromUtf32(0x1F60D); //      😍
        public static readonly string SmilingFaceWithSunglasses = char.ConvertFromUtf32(0x1F60E); //      😎
        public static readonly string SmirkingFace = char.ConvertFromUtf32(0x1F60F); //      😏
        public static readonly string NeutralFace = char.ConvertFromUtf32(0x1F610); //      😐
        public static readonly string ExpressionlessFace = char.ConvertFromUtf32(0x1F611); //      😑
        public static readonly string UnamusedFace = char.ConvertFromUtf32(0x1F612); //      😒
        public static readonly string FaceWithColdSweat = char.ConvertFromUtf32(0x1F613); //      😓
        public static readonly string PensiveFace = char.ConvertFromUtf32(0x1F614); //      😔
        public static readonly string ConfusedFace = char.ConvertFromUtf32(0x1F615); //      😕
        public static readonly string ConfoundedFace = char.ConvertFromUtf32(0x1F616); //      😖
        public static readonly string KissingFace = char.ConvertFromUtf32(0x1F617); //      😗
        public static readonly string FaceThrowingAKiss = char.ConvertFromUtf32(0x1F618); //      😘
        public static readonly string KissingFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F619); //      😙
        public static readonly string KissingFaceWithClosedEyes = char.ConvertFromUtf32(0x1F61A); //      😚
        public static readonly string FaceWithStuckOutTongue = char.ConvertFromUtf32(0x1F61B); //      😛
        public static readonly string FaceWithStuckOutTongueAndWinkingEye = char.ConvertFromUtf32(0x1F61C); //      😜
        public static readonly string FaceWithStuckOutTongueAndTightlyClosedEyes = char.ConvertFromUtf32(0x1F61D); //      😝
        public static readonly string DisappointedFace = char.ConvertFromUtf32(0x1F61E); //      😞
        public static readonly string WorriedFace = char.ConvertFromUtf32(0x1F61F); //      😟
        public static readonly string AngryFace = char.ConvertFromUtf32(0x1F620); //      😠
        public static readonly string PoutingFace = char.ConvertFromUtf32(0x1F621); //      😡
        public static readonly string CryingFace = char.ConvertFromUtf32(0x1F622); //      😢
        public static readonly string PerseveringFace = char.ConvertFromUtf32(0x1F623); //      😣
        public static readonly string FaceWithLookOfTriumph = char.ConvertFromUtf32(0x1F624); //      😤
        public static readonly string DisappointedButRelievedFace = char.ConvertFromUtf32(0x1F625); //      😥
        public static readonly string FrowningFaceWithOpenMouth = char.ConvertFromUtf32(0x1F626); //      😦
        public static readonly string AnguishedFace = char.ConvertFromUtf32(0x1F627); //      😧
        public static readonly string FearfulFace = char.ConvertFromUtf32(0x1F628); //      😨
        public static readonly string WearyFace = char.ConvertFromUtf32(0x1F629); //      😩
        public static readonly string SleepyFace = char.ConvertFromUtf32(0x1F62A); //      😪
        public static readonly string TiredFace = char.ConvertFromUtf32(0x1F62B); //      😫
        public static readonly string GrimacingFace = char.ConvertFromUtf32(0x1F62C); //      😬
        public static readonly string LoudlyCryingFace = char.ConvertFromUtf32(0x1F62D); //      😭
        public static readonly string FaceWithOpenMouth = char.ConvertFromUtf32(0x1F62E); //      😮
        public static readonly string HushedFace = char.ConvertFromUtf32(0x1F62F); //      😯
        public static readonly string FaceWithOpenMouthAndColdSweat = char.ConvertFromUtf32(0x1F630); //      😰
        public static readonly string AstonishedFace = char.ConvertFromUtf32(0x1F632); //      😲
        public static readonly string FlushedFace = char.ConvertFromUtf32(0x1F633); //      😳
        public static readonly string SleepingFace = char.ConvertFromUtf32(0x1F634); //      😴
        public static readonly string DizzyFace = char.ConvertFromUtf32(0x1F635); //      😵
        public static readonly string FaceWithoutMouth = char.ConvertFromUtf32(0x1F636); //      😶
        public static readonly string WhiteCircleWithTwoDots = char.ConvertFromUtf32(0x2687); //      ⚇ 
        public static readonly string FaceWithMedicalMask = char.ConvertFromUtf32(0x1F637); //      😷
        public static readonly string SlightlyFrowningFace = char.ConvertFromUtf32(0x1F641); //      🙁
        public static readonly string WhiteFrowningFace = char.ConvertFromUtf32(0x2639); //      ☹ 
        public static readonly string SlightlySmilingFace = char.ConvertFromUtf32(0x1F642); //      🙂
        public static readonly string WhiteSmilingFace = char.ConvertFromUtf32(0x263A); //      ☺ 

        // Cat faces            
        public static readonly string GrinningCatFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F638); //      😸
        public static readonly string CatFaceWithTearsOfJoy = char.ConvertFromUtf32(0x1F639); //      😹
        public static readonly string SmilingCatFaceWithOpenMouth = char.ConvertFromUtf32(0x1F63A); //      😺
        public static readonly string SmilingCatFaceWithHeartShapedEyes = char.ConvertFromUtf32(0x1F63B); //      😻
        public static readonly string CatFaceWithWrySmile = char.ConvertFromUtf32(0x1F63C); //      😼
        public static readonly string KissingCatFaceWithClosedEyes = char.ConvertFromUtf32(0x1F63D); //      😽
        public static readonly string PoutingCatFace = char.ConvertFromUtf32(0x1F63E); //      😾
        public static readonly string CryingCatFace = char.ConvertFromUtf32(0x1F63F); //      😿
        public static readonly string WearyCatFace = char.ConvertFromUtf32(0x1F640); //      🙀

        // Gesture symbols              
        public static readonly string FaceWithNoGoodGesture = char.ConvertFromUtf32(0x1F645); //      🙅
        public static readonly string FaceWithOkGesture = char.ConvertFromUtf32(0x1F646); //      🙆
        public static readonly string PersonBowingDeeply = char.ConvertFromUtf32(0x1F647); //      🙇
        public static readonly string SeeNoEvilMonkey = char.ConvertFromUtf32(0x1F648); //      🙈
        public static readonly string HearNoEvilMonkey = char.ConvertFromUtf32(0x1F649); //      🙉
        public static readonly string SpeakNoEvilMonkey = char.ConvertFromUtf32(0x1F64A); //      🙊
        public static readonly string HappyPersonRaisingOneHand = char.ConvertFromUtf32(0x1F64B); //      🙋
        public static readonly string PersonRaisingBothHandsInCelebration = char.ConvertFromUtf32(0x1F64C); //      🙌
        public static readonly string PersonFrowning = char.ConvertFromUtf32(0x1F64D); //      🙍
        public static readonly string PersonWithPoutingFace = char.ConvertFromUtf32(0x1F64E); //      🙎
        public static readonly string PersonWithFoldedHands = char.ConvertFromUtf32(0x1F64F); //      🙏

        // Not official emoticons from  http://www.unicode.org/charts/PDF/U2600.pdf           
        public static readonly string Snowman = char.ConvertFromUtf32(0x2603); //      ☃ 
        public static readonly string HiteFrowningFace = char.ConvertFromUtf32(0x2639); //      ☹ 
        public static readonly string HiteSmilingFace = char.ConvertFromUtf32(0x263a); //      ☺ 
        public static readonly string LackSmilingFace = char.ConvertFromUtf32(0x263b); //      ☻ 
        public static readonly string NowmanWithoutSnow = char.ConvertFromUtf32(0x26c4); //      ⛄ 
        public static readonly string KullAndCrossbones = char.ConvertFromUtf32(0x2620); //      ☠


        public class TwitterUserFeature : TextFeature
        {
            public TwitterUserFeature(string markToken = "__USER__") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
            }

            public TwitterUserFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"@(\w+)";
            }
        }

        public class StockSymbolFeature : TextFeature
        {
            public StockSymbolFeature(string markToken = "__STOCK__") : base(markToken)
            {
                Operation = TextFeatureOperation.Append;
            }

            public StockSymbolFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"\$(\p{L}\w*)";
            }
        }

        public class UrlFeature : TextFeature
        {
            public UrlFeature(string markToken = "__URL__") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
            }

            public UrlFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"http(\S)*|www(\S)*";
            }
        }

        public class HashTagFeature : TextFeature
        {
            public HashTagFeature(string markToken = "__HASH__")
                : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
            }

            public HashTagFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"#(\w+)";
            }
        }

        public class HashTagCounterFeature : HashTagFeature
        {
            public HashTagCounterFeature(string markToken = "__HASH__") : base(markToken)
            {
                Operation = TextFeatureOperation.Custom;
            }

            public HashTagCounterFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected internal override string PerformCustomOperation(string input, Dictionary<string, object> namedValues)
            {
                MatchCollection matches = Regex.Matches(input);
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        string val = match.Groups[1].Value;
                        if (!namedValues.ContainsKey(val))
                        {
                            namedValues.Add(val, 1);
                        }
                        else
                        {
                            namedValues[val] = (int)namedValues[val] + 1;
                        }
                    }
                }
                return Regex.Replace(input, MarkToken);
            }
        }

        public class RetweetFeature : TextFeature
        {
            public RetweetFeature(string markToken = "__RETWEET__") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
            }

            public RetweetFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                options |= RegexOptions.IgnoreCase;
                return @"^(RT\s+)";
            }
        }

        public class PunctuationFeature : TextFeatureGroup
        {
            public PunctuationFeature() : base(GetFeatures())
            {
            }

            public PunctuationFeature(BinarySerializer reader) : base(reader)
            {
            }

            private static TextFeature[] GetFeatures()
            {
                return new TextFeature[] {
                    new LastExclamationFeature(),
                    new LastQuestionMarkFeature(),
                    new SingleExclamationFeature(),
                    new SingleQuestionMarkFeature(),
                    new MultipleMixedPunctuationFeature(),
                    new MultipleQuestionMarkFeature(),
                    new MultipleExclamationFeature(),
                    new MultipleDotFeature()
                };
            }
        }

        public class SingleExclamationFeature : TextFeature
        {
            public SingleExclamationFeature(string markToken = "__PUNCT_SINGLE_EXCLAMATION__") : base(" " + markToken)
            {
                Operation = TextFeatureOperation.Replace;
            }

            public SingleExclamationFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"(?<!!+)(!)(?!!+)";
            }
        }

        public class LastExclamationFeature : TextFeature
        {
            public LastExclamationFeature(string markToken = "__PUNCT_LAST_EXCLAMATION__") : base(" " + markToken)
            {
                Operation = TextFeatureOperation.Append;
            }

            public LastExclamationFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"(!)$";
            }
        }

        public class SingleQuestionMarkFeature : TextFeature
        {
            public SingleQuestionMarkFeature(string markToken = "__PUNCT_SINGLE_QUESTION__") : base(" " + markToken)
            {
                Operation = TextFeatureOperation.Replace;
            }

            public SingleQuestionMarkFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"(?<!\?+)(\?)(?!\?+)";
            }
        }

        public class LastQuestionMarkFeature : TextFeature
        {
            public LastQuestionMarkFeature(string markToken = "__PUNCT_LAST_QUESTION__") : base(" " + markToken)
            {
                Operation = TextFeatureOperation.Append;
            }

            public LastQuestionMarkFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"(\?)$";
            }
        }

        public class MultipleMixedPunctuationFeature : TextFeature
        {
            public MultipleMixedPunctuationFeature(string markToken = "__PUNCT_MULTIMIX__") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
                IsEncloseMarkTokenWithSpace = true;
            }

            public MultipleMixedPunctuationFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"((!+\?+)+!*)|((\?+!+)+\?*)";
            }
        }

        public class MultipleExclamationFeature : TextFeature
        {
            public MultipleExclamationFeature(string markToken = "__PUNCT_MULTI_EXCLAMATION__") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
                IsEncloseMarkTokenWithSpace = true;
            }

            public MultipleExclamationFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"!{2,}";
            }
        }

        public class MultipleDotFeature : TextFeature
        {
            public MultipleDotFeature(string markToken = "__PUNCT_MULTI_DOT__") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
                IsEncloseMarkTokenWithSpace = true;
            }

            public MultipleDotFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"\.{4,}";
            }
        }

        public class MultipleQuestionMarkFeature : TextFeature
        {
            public MultipleQuestionMarkFeature(string markToken = "__PUNCT_MULTI_QUESTION__") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
                IsEncloseMarkTokenWithSpace = true;
            }

            public MultipleQuestionMarkFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"\?{2,}";
            }

            protected internal override string PerformCustomOperation(string input, Dictionary<string, object> namedValues)
            {
                input = Regex.Replace(input, " " + MarkToken);
                return input;
            }
        }

        public class UppercasedFeature : TextFeature
        {
            public UppercasedFeature(string markToken = "__UPPERCASED__") : base(markToken)
            {
                Operation = TextFeatureOperation.Append;
            }

            public UppercasedFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"\b[A-Z]{4,}\b";
            }
        }

        public class TextBeginFeature : TextFeature
        {
            public TextBeginFeature(string markToken = "__TEXT_BEGIN__ ") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
            }

            public TextBeginFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"^";
            }
        }

        public class TextEndFeature : TextFeature
        {
            public TextEndFeature(string markToken = " __TEXT_END__") : base(markToken)
            {
                Operation = TextFeatureOperation.Replace;
            }

            public TextEndFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return @"$";
            }
        }

        public class NegationFeature : AnyOfTermsTextFeature
        {
            private readonly Language mLanguage;

            public NegationFeature(Language language, string markToken = "__NEGATED__") : base(markToken)
            {
                mLanguage = language;
                Operation = TextFeatureOperation.Replace;
                RegexOptions = RegexOptions.IgnoreCase;
                WordBoundEnclosing = EncloseOption.BothEdges;

                switch (mLanguage)
                {
                    case Language.English:
                        Terms = Strings.Split(",", @"
                            not,isn't,aren't,wasn't,weren't,hasn't,haven't,hadn't,doesn't,don't,didn't,cannot,didnot,havenot
                            ");
                        break;

                    case Language.Italian:
                        Terms = Strings.Split(",", @"
                            mai,nessuno,niente,nulla,né,nessun,neanche,nemmeno,neppure,no
                            ");
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            public NegationFeature(BinarySerializer reader) : base(reader)
            {
            }
        }


        public class SwearingFeature : AnyOfTermsTextFeature
        {
            private readonly Language mLanguage;

            public SwearingFeature(Language language, string markToken = "__SWEARING__")
                : base(markToken)
            {
                mLanguage = language;
                Operation = TextFeatureOperation.Append;
                RegexOptions = RegexOptions.IgnoreCase;
                WordBoundEnclosing = EncloseOption.LeftEdge;

                switch (mLanguage)
                {
                    case Language.Italian:  // http://en.wikipedia.org/wiki/Italian_profanity and others
                        Terms = Strings.Split(",", @"
                            bastardo,bocchino,cagna,carogna,cazzate,cazzo,coglione,coglioni,cornuto,culo,dio dannato,fanculo,finocchio,
                            fottiti,frocio,gnocca,li mortacci tua,mannaggia,merda,merdoso,mignotta,minchia,non mi rompere,pigliainculo,
                            pompino,porca,puttana,rottinculo,stronzo,succhiacazzi,troia,vaffanculo,zoccola
                            "
                            +
                            @"p.....a,cazzi,p*****a,co*****i,c****,m*****a,m***a,m...,vigliacco,inc...a,shit,stronzate,schif,m___a,ca__o,cagare,
                            pinocchio,m_ _ _a,rimbambit,sigh,fott,lumaca,sh*t,catzo"
                            );
                        break;
                    case Language.English: // http://fffff.at/googles-official-list-of-bad-words/ , should be improved since bad word != swearing
                        Terms = Strings.Split(",", @"4r5e,5h1t,5hit,a55,anal,anus,ar5e,arrse,arse,ass,ass-fucker,asses,assfucker,assfukka,
                            asshole,assholes,asswhole,a_s_s,b!tch,b00bs,b17ch,b1tch,ballbag,balls,ballsack,bastard,beastial,beastiality,bellend,
                            bestial,bestiality,bi+ch,biatch,bitch,bitcher,bitchers,bitches,bitchin,bitching,bloody,blow job,blowjob,blowjobs,boiolas,
                            bollock,bollok,boner,boob,boobs,booobs,boooobs,booooobs,booooooobs,breasts,buceta,bugger,bum,bunny fucker,butt,butthole,
                            buttmuch,buttplug,c0ck,c0cksucker,carpet muncher,cawk,chink,cipa,cl1t,clit,clitoris,clits,cnut,cock,cock-sucker,cockface,
                            cockhead,cockmunch,cockmuncher,cocks,cocksuck,cocksucked,cocksucker,cocksucking,cocksucks,cocksuka,cocksukka,cok,cokmuncher,
                            coksucka,coon,cox,crap,cum,cummer,cumming,cums,cumshot,cunilingus,cunillingus,cunnilingus,cunt,cuntlick,cuntlicker,cuntlicking,
                            cunts,cyalis,cyberfuc,cyberfuck,cyberfucked,cyberfucker,cyberfuckers,cyberfucking,d1ck,damn,dick,dickhead,dildo,dildos,dink,
                            dinks,dirsa,dlck,dog-fucker,doggin,dogging,donkeyribber,doosh,duche,dyke,ejaculate,ejaculated,ejaculates,ejaculating,ejaculatings,
                            ejaculation,ejakulate,f u c k,f u c k e r,f4nny,fag,fagging,faggitt,faggot,faggs,fagot,fagots,fags,fanny,fannyflaps,fannyfucker,fanyy,
                            fatass,fcuk,fcuker,fcuking,feck,fecker,felching,fellate,fellatio,fingerfuck,fingerfucked,fingerfucker,fingerfuckers,fingerfucking,
                            fingerfucks,fistfuck,fistfucked,fistfucker,fistfuckers,fistfucking,fistfuckings,fistfucks,flange,fook,fooker,fuck,fucka,fucked,fucker,
                            fuckers,fuckhead,fuckheads,fuckin,fucking,fuckings,fuckingshitmotherfucker,fuckme,fucks,fuckwhit,fuckwit,fudge packer,fudgepacker,
                            fuk,fuker,fukker,fukkin,fuks,fukwhit,fukwit,fux,fux0r,f_u_c_k,gangbang,gangbanged,gangbangs,gaylord,gaysex,goatse,God,god-dam,
                            god-damned,goddamn,goddamned,hardcoresex,hell,heshe,hoar,hoare,hoer,homo,hore,horniest,horny,hotsex,jack-off,jackoff,jap,jerk-off,
                            jism,jiz,jizm,jizz,kawk,knob,knobead,knobed,knobend,knobhead,knobjocky,knobjokey,kock,kondum,kondums,kum,kummer,kumming,kums,kunilingus,
                            l3i+ch,l3itch,labia,lmfao,lust,lusting,m0f0,m0fo,m45terbate,ma5terb8,ma5terbate,masochist,master-bate,masterb8,masterbat*,masterbat3,
                            masterbate,masterbation,masterbations,masturbate,mo-fo,mof0,mofo,mothafuck,mothafucka,mothafuckas,mothafuckaz,mothafucked,mothafucker,
                            mothafuckers,mothafuckin,mothafucking,mothafuckings,mothafucks,mother fucker,motherfuck,motherfucked,motherfucker,motherfuckers,motherfuckin,
                            motherfucking,motherfuckings,motherfuckka,motherfucks,muff,mutha,muthafecker,muthafuckker,muther,mutherfucker,n1gga,n1gger,nazi,nigg3r,nigg4h,
                            nigga,niggah,niggas,niggaz,nigger,niggers,nob,nob jokey,nobhead,nobjocky,nobjokey,numbnuts,nutsack,orgasim,orgasims,orgasm,orgasms,p0rn,pawn,
                            pecker,penis,penisfucker,phonesex,phuck,phuk,phuked,phuking,phukked,phukking,phuks,phuq,pigfucker,pimpis,piss,pissed,pisser,pissers,pisses,
                            pissflaps,pissin,pissing,pissoff,poop,porn,porno,pornography,pornos,prick,pricks,pron,pube,pusse,pussi,pussies,pussy,pussys,rectum,retard,
                            rimjaw,rimming,s hit,s.o.b.,sadist,schlong,screwing,scroat,scrote,scrotum,semen,sex,sh!+,sh!t,sh1t,shag,shagger,shaggin,shagging,shemale,
                            shi+,shit,shitdick,shite,shited,shitey,shitfuck,shitfull,shithead,shiting,shitings,shits,shitted,shitter,shitters,shitting,shittings,shitty,
                            skank,slut,sluts,smegma,smut,snatch,son-of-a-bitch,spac,spunk,s_h_i_t,t1tt1e5,t1tties,teets,teez,testical,testicle,tit,titfuck,tits,titt,
                            tittie5,tittiefucker,titties,tittyfuck,tittywank,titwank,tosser,turd,tw4t,twat,twathead,twatty,twunt,twunter,v14gra,v1gra,vagina,viagra,vulva,
                            w00se,wang,wank,wanker,wanky,whoar,whore,willies,willy,xrated,xxx");
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            public SwearingFeature(BinarySerializer reader) : base(reader)
            {
            }
        }


        public class PositiveWordFeature : AnyOfTermsTextFeature
        {
            private readonly Language mLanguage;

            public PositiveWordFeature(Language language, string markToken = "__POSITIVE_WORD__")
                : base(markToken)
            {
                mLanguage = language;
                Operation = TextFeatureOperation.Append;
                RegexOptions = RegexOptions.IgnoreCase;
                WordBoundEnclosing = EncloseOption.LeftEdge;

                switch (mLanguage)
                {
                    case Language.Italian:  // https://scienzanewthought.wordpress.com/tag/dizionario-delle-parole-positive/  transformed to word roots
                        Terms = Strings.Split(",", @"
                            abbondan,affabili,affett,aiuta,allegria,altruism,amabili,ama,amici,ammira,amor,amorevolez,anima,appagament,
                            apprezzament,approva,armoni,autocontroll,autoguarigi,autoironi,autostima,beatitudin,bellez,ben,benefici,benessere, 
                            benevolen,bontà,buonumore,buonsens,calma,canta,cari,clemen,coeren,compassi,compliment,comprensi,concordi,confida, 
                            confort,consapevolez,consola,contempla,contentez,Coraggi,cordiali,correttez,cortesia,costan,cred,cura,dedi,diligen,dinamism,
                            diO,discerniment,disciplin,disponibili,distensi,divertiment,dolcez,dona,educa,elogi,elogia,empatia,energia,entusiasm, 
                            equilibri,esultan,esulta,estasi,euforia,fede,fedeltà,felici,fermez,fervor,fiduci,focalizza,for,fratellan,gaudi,gaiez,generosi,
                            gentilez,gioi,gioviali,giovinez,giubil,giusti,gratitudine,grazia,guarigi,ilari,imparziali,impegn,incorruttibili,indulgen,integri,
                            intui,ispira,lealtà,leggerez,leti,liberali,loda,lode,luce,magnanimi,mansuetud,medita,medita,metod,misericord,mitez,modestia, 
                            modera,morali,motiva,natura,oculatez,onest,onor,operosi,ottimism,pace,passi,pazien,perdona,perdon,perfe,perseveran,poten,prega, 
                            preghiera,preziosi,prezios,prospera,prosperi,puntuali,purez,quiet,rallegrar,relax,rettitudi,ricchez,riconoscen,rider,riflessi, 
                            rilassament,ringrazia,ringraziament,risat,rispett,riveren,saggez,salute,santi,sapien,semplici,sereni,seriet,signorili,silenzi, 
                            simpatia,sinceri,soavi,soddisfa,solidar,sorrid,sorris,speran,spirituali,stima,success,temperan,tenerez,tolleran,tranquilli, 
                            uguaglian,umilt,uni,valor,valorizza,veri,virtù,vita,vitali,volere,volont,zelo
                            " +
                            "hahaha,ahahah,brav,grand,buon,divertent,content,viva,Bellissim,EHEHEH,Idem,yes,cool,angel,ottim,god"
                            );
                        break;
                    case Language.English:   // http://www.enchantedlearning.com/wordlist/positivewords.shtml
                        Terms = Strings.Split(",", @"absolutely,adorable,accepted,acclaimed,accomplish,accomplishment,achievement,action,active,admire,
                            adventure,affirmative,affluent,agree,agreeable,amazing,angelic,appealing,approve,aptitude,attractive,awesome,beaming,beautiful,
                            believe,beneficial,bliss,bountiful,bounty,brave,bravo,brilliant,bubbly,calm,celebrated,certain,champ,champion,charming,cheery,
                            choice,classic,classical,clean,commend,composed,congratulation,constant,cool,courageous,creative,cute,dazzling,delight,delightful,
                            distinguished,divine,earnest,easy,ecstatic,effective,effervescent,efficient,effortless,electrifying,elegant,enchanting,encouraging,
                            endorsed,energetic,energized,engaging,enthusiastic,essential,esteemed,ethical,excellent,exciting,exquisite,fabulous,fair,familiar,
                            famous,fantastic,favorable,fetching,fine,fitting,flourishing,fortunate,free,fresh,friendly,fun,funny,generous,genius,genuine,giving,
                            glamorous,glowing,good,gorgeous,graceful,great,green,grin,growing,handsome,happy,harmonious,healing,healthy,hearty,heavenly,honest,
                            honorable,honored,hug,ideideal,imaginative,imagine,impressive,independent,innovate,innovative,instant,instantaneous,instinctive,
                            intuitive,intellectual,intelligent,inventive,jovial,joy,jubilant,keen,kind,knowing,knoedgeable,laugh,legendary,light,learned,lively,
                            lovely,lucid,lucky,luminous,marvelous,masterful,meaningful,merit,meritorious,miraculous,motivating,moving,natural,nice,novel,now,
                            nurturing,nutritious,okay,one,one-hundred percent,on,optimistic,paradise,perfect,phenomenal,pleasurable,plentiful,pleasant,poised,
                            polished,popular,positive,powerful,prepared,pretty,principled,productive,progress,prominent,protected,proud,quality,quick,quiet,ready,
                            reassuring,refined,refreshing,rejoice,reliable,remarkable,resounding,respected,restored,reward,rewarding,right,robust,safe,satisfactory,
                            secure,seemly,simple,skilled,skillful,smile,soulful,sparkling,special,spirited,spiritual,stirring,stupendous,stunning,success,successful,
                            sunny,super,superb,supporting,surprising,terrific,thorough,thrilling,thriving,tops,tranquil,transforming,transformative,trusting,truthful,
                            unreal,unwavering,up,upbeat,upright,upstanding,valued,vibrant,victorious,victory,vigorous,virtuous,vital,vivacious,wealthy,welcome,well,
                            whole,wholesome,willing,wonderful,wondrous,worthy,wow,yes,yummy,zeal,zealous");
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            public PositiveWordFeature(BinarySerializer reader) : base(reader)
            {
            }
        }

        public class NegativeWordFeature : AnyOfTermsTextFeature
        {
            private readonly Language mLanguage;

            public NegativeWordFeature(Language language, string markToken = "__NEGATIVE_WORD__")
                : base(markToken)
            {
                mLanguage = language;
                Operation = TextFeatureOperation.Append;
                RegexOptions = RegexOptions.IgnoreCase;
                WordBoundEnclosing = EncloseOption.LeftEdge;

                switch (mLanguage)
                {                    
                    case Language.English:   // http://www.enchantedlearning.com/wordlist/negativewords.shtml
                        Terms = Strings.Split(",", @"abysmal,adverse,alarming,angry,annoy,anxious,apathy,appalling,atrocious,awful,bad,banal,barbed,belligerent,
                            bemoan,beneath,boring,broken,callous,can't,clumsy,coarse,cold,cold-hearted,collapse,confused,contradictory,contrary,corrosive,corrupt,
                            crazy,creepy,criminal,cruel,cry,cutting,dead,decaying,damage,damaging,dastardly,deplorable,depressed,deprived,deformed,deny,despicable,
                            detrimental,dirty,disease,disgusting,disheveled,dishonest,dishonorable,dismal,distress,don't,dreadful,dreary,enraged,eroding,evil,fail,
                            faulty,fear,feeble,fight,filthy,foul,frighten,frightful,gawky,ghastly,grave,greed,grim,grimace,gross,grotesque,gruesome,guilty,haggard,
                            hard,hard-hearted,harmful,hate,hideous,homely,horrendous,horrible,hostile,hurt,hurtful,icky,ignore,ignorant,ill,immature,imperfect,
                            impossible,inane,inelegant,infernal,injure,injurious,insane,insidious,insipid,jealous,junky,lose,lousy,lumpy,malicious,mean,menacing,
                            messy,misshapen,missing,misunderstood,moan,moldy,monstrous,naive,nasty,naughty,negate,negative,never,no,nobody,nondescript,nonsense,not,
                            noxious,objectionable,odious,offensive,old,oppressive,pain,perturb,pessimistic,petty,plain,poisonous,poor,prejudice,questionable,quirky,
                            quit,reject,renege,repellant,reptilian,repulsive,repugnant,revenge,revolting,rocky,rotten,rude,ruthless,sad,savage,scare,scary,scream,
                            severe,shoddy,shocking,sick,sickening,sinister,slimy,smelly,sobbing,sorry,spiteful,sticky,stinky,stormy,stressful,stuck,stupid,substandard,
                            suspect,suspicious,tense,terrible,terrifying,threatening,ugly,undermine,unfair,unfavorable,unhappy,unhealthy,unjust,unlucky,unpleasant,
                            upset,unsatisfactory,unsightly,untoward,unwanted,unwelcome,unwholesome,unwieldy,unwise,upset,vice,vicious,vile,villainous,vindictive,wary,
                            weary,wicked,woeful,worthless,wound,yell,yucky,zero");
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            public NegativeWordFeature(BinarySerializer reader)
                : base(reader)
            {
            }
        }

        // if there is more then 3 consecutive identical characters, truncate to three
        public class RepetitionFeature : TextFeature
        {
            public RepetitionFeature(string markToken = "__EXAGGERATED__") : base(markToken)
            {
                Operation = TextFeatureOperation.Custom;
            }

            public RepetitionFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                options |= RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
                return @"(.)(?<=\1\1\1)";
            }

            protected internal override string PerformCustomOperation(string input, Dictionary<string, object> namedValues)
            {
                if (Regex.IsMatch(input))
                {
                    return Regex.Replace(input, string.Empty) + " " + MarkToken;
                }
                return input;
            }
        }

        public class MessageLengthFeature : TextFeature
        {
            private readonly int mMaxLength;
            private readonly ConcurrentDictionary<int, string> mEqStrings = new ConcurrentDictionary<int, string>();
            private readonly ConcurrentDictionary<int, string> mGtStrings = new ConcurrentDictionary<int, string>();

            public MessageLengthFeature(int maxLength = 1000) : base("")
            {
                Operation = TextFeatureOperation.Custom;
                mMaxLength = maxLength;
            }

            public MessageLengthFeature(BinarySerializer reader) : base(reader)
            {
                mMaxLength = reader.ReadInt();
            }

            public override void Save(BinarySerializer writer)
            {
                base.Save(writer);
                writer.WriteInt(mMaxLength);
            }

            protected internal override string PerformCustomOperation(string input, 
                Dictionary<string, object> namedValues, out string[] append, out string[] distinctAppend)
            {
                append = null;
                int strLen = input.Length;
                var tokens = new List<string>();
                for (int len = 2; len < mMaxLength; len *= 2)
                {
                    string token;
                    if (strLen <= len)
                    {
                        if (!mEqStrings.TryGetValue(len, out token))
                        {
                            mEqStrings.TryAdd(len, token = string.Format("__LengthLEQ{0}__", len));
                        }
                    }
                    else
                    {
                        if (!mGtStrings.TryGetValue(len, out token))
                        {
                            mGtStrings.TryAdd(len, token = string.Format("__LengthGT{0}__", len));
                        }
                    }
                    tokens.Add(token);
                }
                distinctAppend = tokens.ToArray();
                return input;
            }
        }

        public class NormalizeDiacriticalCharactersFeature : TextFeature
        {
            public NormalizeDiacriticalCharactersFeature() : base("")
            {
                Operation = TextFeatureOperation.Custom;
            }

            public NormalizeDiacriticalCharactersFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected internal override string PerformCustomOperation(string input, Dictionary<string, object> namedValues)
            {
                return TextMiningUtils.NormalizeDiacriticalCharacters(input);
            }
        }

        public class UnicodeOtherFeature : TextFeature
        {
            public UnicodeOtherFeature(string markToken = "__UNICODE_OTHER__") : base(markToken)
            {
                Operation = TextFeatureOperation.Custom;
            }

            public UnicodeOtherFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected internal override string PerformCustomOperation(string input, Dictionary<string, object> namedValues)
            {
                var result = new StringBuilder();
                for (int i = 0; i < input.Length; i++)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(input, i) == UnicodeCategory.OtherSymbol)
                    {
                        result.Append(MarkToken);
                    }
                    else
                    {
                        result.Append(input[i]);
                    }
                }
                return result.ToString();
            }
        }

        public class FinanceTweetNormalizationFeature : TextFeature
        {
            private static readonly Regex mUrlRegex = new Regex(@"http\S*", RegexOptions.IgnoreCase);
            private static readonly Regex mStockRefRegex = new Regex(@"\$\w+", RegexOptions.IgnoreCase);
            private static readonly Regex mUserRefRegex = new Regex(@"@\w+", RegexOptions.IgnoreCase);
            private static Regex hashtagRegex = new Regex(@"#\w+", RegexOptions.IgnoreCase);
            private static readonly Regex mLetterRepetitionRegex = new Regex(@"(.)\1{2,}", RegexOptions.IgnoreCase);

            public FinanceTweetNormalizationFeature() : base("")
            {
                Operation = TextFeatureOperation.Custom;
            }

            public FinanceTweetNormalizationFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected internal override string PerformCustomOperation(string text, Dictionary<string, object> namedValues)
            {
                text = mUrlRegex.Replace(text, ""); // rmv URLs
                text = mStockRefRegex.Replace(text, ""); // rmv stock refs
                text = mUserRefRegex.Replace(text, ""); // rmv user refs
                //text = mHashtagRegex.Replace(text, ""); // rmv hashtags
                text = mLetterRepetitionRegex.Replace(text, "$1$1"); // collapse letter repetitions
                text = text.Replace("'", ""); // rmv apos
                return text;
            }
        }

    }


    public static class EmoticonCounter
    {
        private static readonly Tuple<string, Regex>[] Emoticons;
        private static readonly Regex CharRegex;

        static EmoticonCounter()
        {
            var chars = new HashSet<char>();

            var emotList = new List<Tuple<int, string, Regex>>();
            foreach (FieldInfo field in typeof(Emoticons).GetFields()
                .Where(f => f.Attributes.HasFlag(FieldAttributes.Static) && f.FieldType == typeof(string[])))
            {
                var strings = (string[])field.GetValue(null);
                foreach (IGrouping<int, string> g in strings.GroupBy(s => s.Length))
                {
                    string pattern = string.Join("|", g.Select(Regex.Escape).ToArray());
                    var regex = new Regex(pattern, RegexOptions.Compiled);
                    emotList.Add(new Tuple<int, string, Regex>(g.Key, field.Name, regex));
                }
                foreach (char ch in strings.SelectMany(s => s).Where(ch => !chars.Contains(ch))) { chars.Add(ch); }
            }
            // search longer patterns first 
            Emoticons = emotList.OrderByDescending(e => e.Item1).Select(e => new Tuple<string, Regex>(e.Item2, e.Item3)).ToArray();

            // preliminary char-based filter before going for full search
            CharRegex = new Regex(string.Join("|", chars.Where(ch => new[] { ' ', '.' }.Contains(ch))
                .Select(ch => Regex.Escape(char.ToString(ch))).ToArray()), RegexOptions.Compiled);
        }

        public static int Count(string text, ref Dictionary<string, int> counts)
        {
            if (string.IsNullOrEmpty(text)) { return 0; }

            int total = 0;
            if (CharRegex.IsMatch(text))
            {
                Dictionary<string, int> counts_ = counts;
                foreach (Tuple<string, Regex> nameRegex in Emoticons)
                {
                    Tuple<string, Regex> nameRegex_ = nameRegex;
                    text = nameRegex.Item2.Replace(text, match =>
                    {
                        int count;
                        if (counts_.TryGetValue(nameRegex_.Item1, out count)) { count++; } else { count = 1; }
                        counts_[nameRegex_.Item1] = count;
                        total++;
                        return "";
                    });
                }
            }
            return total;
        }
    }
}