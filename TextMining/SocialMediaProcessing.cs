﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Latino.TextMining
{

    public abstract class TextProcessing
    {
        
    }

    public static class SocialMediaProcessing
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
        public static readonly string[] WinkSmirk = { ";-)", ";)", "*-)", "*)", ";-]", ";]", ";D", ";^)", ":-" };
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


        // punctuation emoticon sets and features

        public static readonly Strings BasicHappyEmoticons = new Strings(
                SmileyOrHappyFace, Rose, Heart
            );

        public static readonly Strings HappyEmoticons = new Strings(
                SmileyOrHappyFace, LaughingbigGrinlaughWithGlasses, VeryHappyOrDoubleChin, TearsOfHappiness, Kiss,
                WinkSmirk, TongueStickingOutCheekyPlayful, AngelSaintInnocent, HighFive, Cool, TongueInCheek,
                DumbDunceLike, Cheer, Cheerleader, Rose, Heart, LennyFace
            );

        public static readonly Strings BasicSadEmoticons = new Strings(
                FrownSad, WinkyFrowny, Angry, StraightFaceNoExpressionIndecision, BrokenHeart
            );

        public static readonly Strings SadEmoticons = new Strings(
                FrownSad, WinkyFrowny, Angry, Crying, HorrorDisgustSadnessGreatDismay, SurpriseShockYawn,
                SkepticalAnnoyedUndecidedUneasyHesitant, StraightFaceNoExpressionIndecision,
                SealedLipsOrWearingBraces, BoredYawning, DrunkConfused, BeingSick, LookOfDisapproval,
                FishSomethingFishy, BrokenHeart
            );


        // note: emoticon features are to be used after ReplaceUrls

        public class BasicHappyEmoticonsFeature : AnyOfTermsTextFeature
        {
            public BasicHappyEmoticonsFeature(string markToken = "__HAPPY_EMOTICON__")
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
            public BasicSadEmoticonsFeature(string markToken = "__SAD_EMOTICON__") : base(markToken)
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
            public LastHappyEmoticonsFeature(string markToken = "__LAST_HAPPY__") : base(markToken)
            {
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
            public LastSadEmoticonsFeature(string markToken = "__LAST_SAD__") : base(markToken)
            {
            }

            public LastSadEmoticonsFeature(BinarySerializer reader) : base(reader)
            {
            }

            protected override string GetPattern(ref RegexOptions options)
            {
                return string.Format("{0}$", base.GetPattern(ref options));
            }
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
            public HappyEmoticonsLenOverTwoFeature(string markToken = "__HAPPY_EMOTICON__") : base(markToken)
            {
                Terms = new Strings(Terms.Where(s => s.Length > 2));
            }

            public HappyEmoticonsLenOverTwoFeature(BinarySerializer reader) : base(reader)
            {
            }
        }

        public class HappySadEmoticonsLenOverTwoFeature : HappyEmoticonsFeature
        {
            
        }

        public class SadEmoticonsLenOverTwoFeature : SadEmoticonsFeature
        {
            public SadEmoticonsLenOverTwoFeature(string markToken = "__SAD_EMOTICON__") : base(markToken)
            {
                Terms = new Strings(Terms.Where(s => s.Length > 2));
            }

            public SadEmoticonsLenOverTwoFeature(BinarySerializer reader) : base(reader)
            {
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



        public static readonly TextFeature[] HappySadEmoticonsFetatures = new TextFeature[] {
            new HappyEmoticonsLenTwoFeature(),
            new SadEmoticonsLenTwoFeature(),
            new HappyEmoticonsLenOverTwoFeature(),
            new SadEmoticonsLenOverTwoFeature()
        };

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

            protected internal override string PerformCustomOperation(string input)
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

                    default:
                        throw new NotSupportedException();
                }
            }

            public PositiveWordFeature(BinarySerializer reader) : base(reader)
            {
            }
        }


        // if there is more then 3 consecutive identical characters, truncate to threee
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

            protected internal override string PerformCustomOperation(string input)
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
            }

            protected internal override string PerformCustomOperation(string input)
            {
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
                return input + " " + string.Join(" ", tokens);
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

            protected internal override string PerformCustomOperation(string input)
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

            protected internal override string PerformCustomOperation(string input)
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
    }
}