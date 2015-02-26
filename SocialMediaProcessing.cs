using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Latino.TextMining;

namespace Latino
{
    public static class SocialMediaProcessing
    {

        public static string ReplaceUsers(string text, String replacement)
        {
            return Regex.Replace(text, @"@(\w+)", replacement);
        }

        public static string ReplaceStockSymbol(string text, String replacement)
        // matches also $700
        {
            return Regex.Replace(text, @"\$(\w+)", replacement);
        }

        public static string ReplaceUrls(string text, String replacement)
        {
            return Regex.Replace(text, @"http(\S)*|www(\S)*", replacement);
        }

        public static string ReplaceHashTags(string text, String replacement)
        {
            return Regex.Replace(text, @"#(\w+)", replacement);
        }

        public static string ReplaceMultiplePunctuation(string text, String replacement)
        {
            string newText = text;
            newText = Regex.Replace(newText, @"((!+\?+)+!*)|((\?+!+)+\?*)", " ***MULTIMIX*** ");
            newText = Regex.Replace(newText, @"!{2,}", " ***MULTIPLEEXCLAMATION*** ");
            newText = Regex.Replace(newText, @"!{1}", " ***SINGLEEXCLAMATION*** ");
            newText = Regex.Replace(newText, @"\?{2,}", " ***MULTIPLEQUESTION*** ");
            newText = Regex.Replace(newText, @"\?{1}", " ***SINGLEQUESTION*** ");
            return newText;
        }

        //        public static string ReplaceLetterRepetition(String text, String replacement)
        //        {
        //            return Regex.Replace(text, regexLetterRepetition, "replacement"); 
        //        }

        public static string IsUppercased(string text, string newFeature)
        {
            Regex regex = new Regex(@"\b[A-Z]{4}\b", RegexOptions.Compiled);   // at least 4 consecutive capital letters
            if (regex.IsMatch(text))
            {
                return text + " " + newFeature;
            }
            return text;
        }


        public static string ReplaceNegations(string text, Language language, string replacement)
        {
            string negations;
            switch (language)
            {
                case Language.English:
                    negations = @"not|isn't|aren't|wasn't|weren't|hasn't|haven't|hadn't|doesn't|don't|didn't|cannot|didnot|havenot";
                    break;
                case Language.Italian:
                    negations = @"mai|nessuno|niente|nulla|né|nessun|neanche|nemmeno|neppure|no";
                    break;
                default: throw new ArgumentValueException("language not supported");
            }
            Regex regex = new Regex(@"\b(" + negations + @")\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.Replace(text, replacement);
        }

        public static string RemoveCharacterRepetition(string text)
        {
            Regex r = new Regex("(.)(?<=\\1\\1\\1)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(text, String.Empty);
        }


        public static string ReplaceEmoticons(string str, Dictionary<Regex, string> emoticons)
        // to be used after ReplaceUrls
        {
            return emoticons.Aggregate(str, (current, emoticonPair) => emoticonPair.Key.Replace(current, emoticonPair.Value));
        }

        
/*
        // Unicode 'Symbol, Other' Category, # of characters = 5082 
        // includes unicode emoticons from http://en.wikipedia.org/wiki/List_of_emoticons
        // list is here unicode ranges http://www.fileformat.info/info/unicode/category/So/list.htm
        public static string ReplaceUnicodeCategory(string str, UnicodeCategory unicodeCategory = UnicodeCategory.OtherSymbol, string repacement = "***OtherSymbol***")
        {
            var result = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(str, i) == unicodeCategory)
                {
                    Console.WriteLine("*****unicodechar*********: "+((int)str[i]).ToString("X4") + str[i]);
                    result.Append(repacement);
                }
                else
                {
                    result.Append(str[i]);
                }
            }            
            return result.ToString();
        }
        */

        // static stuff

        public static readonly string[]  SmileyOrHappyFace       = {     ":-)", ":)", ":D", ":o)", ":]", ":3", ":c)", ":>", "=]", "8)", "=)", ":}", ":^)", ":っ)" };
        public static readonly string[]  LaughingbigGrinlaughWithGlasses = {     ":-D", "8-D", "8D", "x-D", "xD", "X-D", "XD", "=-D", "=D", "=-3", "=3", "B^D"   };
        public static readonly string[]  VeryHappyOrDoubleChin   = {     ":-))"  };
        public static readonly string[]  Frownsad        = {     ">:[", ":-(", ":(", ":-c", ":c", ":-<", ":っC", ":<", ":-[", ":[", ":{"  };
        public static readonly string[]  WinkyFrowny     = {     ";("    };
        public static readonly string[]  Angry   = {     ":-||", ":@", ">:("     };
        public static readonly string[]  Crying  = {     ":'-(", ":'("   };
        public static readonly string[]  TearsOfHappiness        = {     ":'-)", ":')"   };
        //public static readonly string[]  HorrorDisgustSadnessGreatDismay = {     "D:<", "D:", "D8", "D;", "D=", "DX", "v.v", "D-':"      };
        public static readonly string[]  HorrorDisgustSadnessGreatDismay = { "D:<", "D:", "D8", "D;", "D=", "v.v", "D-':" };
        public static readonly string[]  SurpriseShockYawn       = {     ">:O", ":-O", ":O", ":-o", ":o", "8-0", "O_O", "o-o", "O_o", "o_O", "o_o", "O-O"        };
        public static readonly string[]  Kiss    = {     ":*", ":^*", "('}{')"   };
        //public static readonly string[]  WinkSmirk       = {     ";-)", ";)", "*-)", "*)", ";-]", ";]", ";D", ";^)", ":-"        };
        public static readonly string[] WinkSmirk = { ";-)", ";)", "*-)", "*)", ";-]", ";]", ";D", ";^)" };
        //public static readonly string[]  TongueStickingOutCheekyPlayful  = {     ">:P", ":-P", ":P", "X-P", "x-p", "xp", "XP", ":-p", ":p", "=p", ":-Þ", ":Þ", ":þ", ":-þ", ":-b", ":b", "d:"    };        
        public static readonly string[] TongueStickingOutCheekyPlayful = { ">:P", ":-P", ":P", "X-P", "x-p", ":-p", ":p", "=p", ":-Þ", ":Þ", ":þ", ":-þ", ":-b", ":b", "d:" };        
        public static readonly string[]  SkepticalAnnoyedUndecidedUneasyHesitant = {     @">:\", ">:/", ":-/", ":-.", ":/", @":\", "=/", @"=\ :L", "=L :S", ">.<"        };
        public static readonly string[]  StraightFaceNoExpressionIndecision      = {     ":|", ":-|"     };
        public static readonly string[]  EmbarrassedBlushing     = {     ":$"    };
        public static readonly string[]  SealedLipsOrWearingBraces       = {     ":-X", ":X", ":-#", ":#"        };
        public static readonly string[]  AngelSaintInnocent      = {     "O:-)", "0:-3", "0:3", "0:-)", "0:)", "0;^)"    };
        public static readonly string[]  Evil    = {     ">:)", ">;)", ">:-)"    };
        public static readonly string[]  Devilish        = {     "}:-)", "}:)", "3:-)", "3:)"    };
        public static readonly string[]  HighFive        = {     @"o/\o", "^5", ">_>^", "^<_<"   };
        public static readonly string[]  BoredYawning    = {     "|-O"   };
        public static readonly string[]  Cool    = {     "|;-)"  };
        public static readonly string[]  TongueInCheek   = {     "-J"    };
        public static readonly string[]  TongueTied      = {     ":-&", ":&"     };
        public static readonly string[]  PartiedAllNight = {     "#-)"   };
        public static readonly string[]  DrunkConfused   = {     "%-)", "%)"     };
        public static readonly string[]  BeingSick       = {     ":-###..", ":###.."     };
        public static readonly string[]  DumbDunceLike   = {     "<:-|"  };
        public static readonly string[]  LookOfDisapproval       = {     "ಠ_ಠ"   };
        public static readonly string[]  FishSomethingFishy      = {     "<*)))-{", "><(((*>", "><>"     };
        public static readonly string[]  Cheer   = {     @"\o/"  };
        public static readonly string[]  Cheerleader     = {     @"*\0/*"        };
        public static readonly string[]  Rose    = {     "@}-;-'---", "@>-->--"  };
        public static readonly string[]  HomerSimpson    = {     "~(_8^(I)"      };
        public static readonly string[]  ElvisPresley    = {     "5:-)", @"~:-\" };
        public static readonly string[]  JohnLennon      = {     @"//0-0\\"      };
        public static readonly string[]  SantaClaus      = {     "*<|:-)"        };
        public static readonly string[]  BillClinton     = {     "=:o]"  };
        public static readonly string[]  RonaldReagan    = {     ",:-)", "7:^]"  };
        public static readonly string[]  Heart   = {     "<3"    };
        public static readonly string[]  BrokenHeart     = {     "</3"   };
        public static readonly string[]  LennyFace       = {     "( ͡° ͜ʖ ͡°)"   };


        internal static string[] BasicHappyEmoticons = new[] { SmileyOrHappyFace, Rose, Heart  }.SelectMany(ss => ss).ToArray();
        internal static string[] HappyEmoticons = new[] { SmileyOrHappyFace, LaughingbigGrinlaughWithGlasses, VeryHappyOrDoubleChin, TearsOfHappiness, Kiss, WinkSmirk, TongueStickingOutCheekyPlayful, AngelSaintInnocent, HighFive, Cool, TongueInCheek, DumbDunceLike, Cheer, Cheerleader, Rose, Heart, LennyFace }.SelectMany(ss => ss).ToArray();
        internal static string[] BasicSadEmoticons = new[] { Frownsad, WinkyFrowny, Angry, StraightFaceNoExpressionIndecision, BrokenHeart }.SelectMany(ss => ss).ToArray();
        internal static string[] SadEmoticons = new[] { Frownsad, WinkyFrowny, Angry, Crying, HorrorDisgustSadnessGreatDismay, SurpriseShockYawn, SkepticalAnnoyedUndecidedUneasyHesitant, StraightFaceNoExpressionIndecision, SealedLipsOrWearingBraces, Evil, BoredYawning, DrunkConfused, BeingSick, LookOfDisapproval, FishSomethingFishy, BrokenHeart }.SelectMany(ss => ss).ToArray();


        public static Regex HappyEmoticonsRegex = new Regex(@"(" + string.Join("|", HappyEmoticons.Select(Regex.Escape).ToArray()) + @")", RegexOptions.Compiled);
        public static Regex BasicHappyEmoticonsRegex = new Regex(@"(" + string.Join("|", BasicHappyEmoticons.Select(Regex.Escape).ToArray()) + @")", RegexOptions.Compiled);
        public static Regex SadEmoticonsRegex = new Regex(@"(" + string.Join("|", SadEmoticons.Select(Regex.Escape).ToArray()) + @")", RegexOptions.Compiled);
        public static Regex BasicSadEmoticonsRegex = new Regex(@"(" + string.Join("|", BasicSadEmoticons.Select(Regex.Escape).ToArray()) + @")", RegexOptions.Compiled);

  
        /*
        // unicode emoticons
        // official from http://www.unicode.org/charts/PDF/U1F600.pdf
        // Faces		
        public static readonly string GrinningFace = char.ConvertFromUtf32(0x1F600);                                                       //      😀
        public static readonly string GrinningFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F601);                                        //      😁
        public static readonly string FaceWithTearsOfJoy = char.ConvertFromUtf32(0x1F602);                                                 //      😂
        public static readonly string SmilingFaceWithOpenMouth = char.ConvertFromUtf32(0x1F603);                                           //      😃
        public static readonly string SmilingFaceWithOpenMouthAndSmilingEyes = char.ConvertFromUtf32(0x1F604);                                                    //      😄
        public static readonly string SmilingFaceWithOpenMouthAndColdSweat = char.ConvertFromUtf32(0x1F605);                                                    //      😅
        public static readonly string SmilingFaceWithOpenMouthAndTightlyClosedEyes = char.ConvertFromUtf32(0x1F606);                                                    //      😆
        public static readonly string SmilingFaceWithHalo = char.ConvertFromUtf32(0x1F607);                                                    //      😇
        public static readonly string SmilingFaceWithHorns = char.ConvertFromUtf32(0x1F608);                                                    //      😈
        public static readonly string WinkingFace = char.ConvertFromUtf32(0x1F609);                                                    //      😉
        public static readonly string SmilingFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F60A);                                                    //      😊
        public static readonly string FaceSavouringDeliciousFood = char.ConvertFromUtf32(0x1F60B);                                                    //      😋
        public static readonly string RelievedFace = char.ConvertFromUtf32(0x1F60C);                                                    //      😌
        public static readonly string SmilingFaceWithHeartShapedEyes = char.ConvertFromUtf32(0x1F60D);                                                    //      😍
        public static readonly string SmilingFaceWithSunglasses = char.ConvertFromUtf32(0x1F60E);                                                    //      😎
        public static readonly string SmirkingFace = char.ConvertFromUtf32(0x1F60F);                                                    //      😏
        public static readonly string NeutralFace = char.ConvertFromUtf32(0x1F610);                                                    //      😐
        public static readonly string ExpressionlessFace = char.ConvertFromUtf32(0x1F611);                                                    //      😑
        public static readonly string UnamusedFace = char.ConvertFromUtf32(0x1F612);                                                    //      😒
        public static readonly string FaceWithColdSweat = char.ConvertFromUtf32(0x1F613);                                                    //      😓
        public static readonly string PensiveFace = char.ConvertFromUtf32(0x1F614);                                                    //      😔
        public static readonly string ConfusedFace = char.ConvertFromUtf32(0x1F615);                                                    //      😕
        public static readonly string ConfoundedFace = char.ConvertFromUtf32(0x1F616);                                                    //      😖
        public static readonly string KissingFace = char.ConvertFromUtf32(0x1F617);                                                    //      😗
        public static readonly string FaceThrowingAKiss = char.ConvertFromUtf32(0x1F618);                                                    //      😘
        public static readonly string KissingFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F619);                                                    //      😙
        public static readonly string KissingFaceWithClosedEyes = char.ConvertFromUtf32(0x1F61A);                                                    //      😚
        public static readonly string FaceWithStuckOutTongue = char.ConvertFromUtf32(0x1F61B);                                                    //      😛
        public static readonly string FaceWithStuckOutTongueAndWinkingEye = char.ConvertFromUtf32(0x1F61C);                                                    //      😜
        public static readonly string FaceWithStuckOutTongueAndTightlyClosedEyes = char.ConvertFromUtf32(0x1F61D);                                                    //      😝
        public static readonly string DisappointedFace = char.ConvertFromUtf32(0x1F61E);                                                    //      😞
        public static readonly string WorriedFace = char.ConvertFromUtf32(0x1F61F);                                                    //      😟
        public static readonly string AngryFace = char.ConvertFromUtf32(0x1F620);                                                    //      😠
        public static readonly string PoutingFace = char.ConvertFromUtf32(0x1F621);                                                    //      😡
        public static readonly string CryingFace = char.ConvertFromUtf32(0x1F622);                                                    //      😢
        public static readonly string PerseveringFace = char.ConvertFromUtf32(0x1F623);                                                    //      😣
        public static readonly string FaceWithLookOfTriumph = char.ConvertFromUtf32(0x1F624);                                                    //      😤
        public static readonly string DisappointedButRelievedFace = char.ConvertFromUtf32(0x1F625);                                                    //      😥
        public static readonly string FrowningFaceWithOpenMouth = char.ConvertFromUtf32(0x1F626);                                                    //      😦
        public static readonly string AnguishedFace = char.ConvertFromUtf32(0x1F627);                                                    //      😧
        public static readonly string FearfulFace = char.ConvertFromUtf32(0x1F628);                                                    //      😨
        public static readonly string WearyFace = char.ConvertFromUtf32(0x1F629);                                                    //      😩
        public static readonly string SleepyFace = char.ConvertFromUtf32(0x1F62A);                                                    //      😪
        public static readonly string TiredFace = char.ConvertFromUtf32(0x1F62B);                                                    //      😫
        public static readonly string GrimacingFace = char.ConvertFromUtf32(0x1F62C);                                                    //      😬
        public static readonly string LoudlyCryingFace = char.ConvertFromUtf32(0x1F62D);                                                    //      😭
        public static readonly string FaceWithOpenMouth = char.ConvertFromUtf32(0x1F62E);                                                    //      😮
        public static readonly string HushedFace = char.ConvertFromUtf32(0x1F62F);                                                    //      😯
        public static readonly string FaceWithOpenMouthAndColdSweat = char.ConvertFromUtf32(0x1F630);                                                    //      😰
        public static readonly string FaceScreamingInFear = char.ConvertFromUtf32(0x1F631);                                                    //      😱
        public static readonly string AstonishedFace = char.ConvertFromUtf32(0x1F632);                                                    //      😲
        public static readonly string FlushedFace = char.ConvertFromUtf32(0x1F633);                                                    //      😳
        public static readonly string SleepingFace = char.ConvertFromUtf32(0x1F634);                                                    //      😴
        public static readonly string DizzyFace = char.ConvertFromUtf32(0x1F635);                                                    //      😵
        public static readonly string FaceWithoutMouth = char.ConvertFromUtf32(0x1F636);                                                    //      😶
        public static readonly string WhiteCircleWithTwoDots = char.ConvertFromUtf32(0x2687);                                                    //      ⚇ 
        public static readonly string FaceWithMedicalMask = char.ConvertFromUtf32(0x1F637);                                                    //      😷
        public static readonly string SlightlyFrowningFace = char.ConvertFromUtf32(0x1F641);                                                    //      🙁
        public static readonly string WhiteFrowningFace = char.ConvertFromUtf32(0x2639);                                                    //      ☹ 
        public static readonly string SlightlySmilingFace = char.ConvertFromUtf32(0x1F642);                                                    //      🙂
        public static readonly string WhiteSmilingFace = char.ConvertFromUtf32(0x263A);                                                    //      ☺ 

        // Cat faces            
        public static readonly string GrinningCatFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F638);                                                    //      😸
        public static readonly string CatFaceWithTearsOfJoy = char.ConvertFromUtf32(0x1F639);                                                    //      😹
        public static readonly string SmilingCatFaceWithOpenMouth = char.ConvertFromUtf32(0x1F63A);                                                    //      😺
        public static readonly string SmilingCatFaceWithHeartShapedEyes = char.ConvertFromUtf32(0x1F63B);                                                    //      😻
        public static readonly string CatFaceWithWrySmile = char.ConvertFromUtf32(0x1F63C);                                                    //      😼
        public static readonly string KissingCatFaceWithClosedEyes = char.ConvertFromUtf32(0x1F63D);                                                    //      😽
        public static readonly string PoutingCatFace = char.ConvertFromUtf32(0x1F63E);                                                    //      😾
        public static readonly string CryingCatFace = char.ConvertFromUtf32(0x1F63F);                                                    //      😿
        public static readonly string WearyCatFace = char.ConvertFromUtf32(0x1F640);                                                    //      🙀

        // Gesture symbols              
        public static readonly string FaceWithNoGoodGesture = char.ConvertFromUtf32(0x1F645);                                                    //      🙅
        public static readonly string FaceWithOkGesture = char.ConvertFromUtf32(0x1F646);                                                    //      🙆
        public static readonly string PersonBowingDeeply = char.ConvertFromUtf32(0x1F647);                                                    //      🙇
        public static readonly string SeeNoEvilMonkey = char.ConvertFromUtf32(0x1F648);                                                    //      🙈
        public static readonly string HearNoEvilMonkey = char.ConvertFromUtf32(0x1F649);                                                    //      🙉
        public static readonly string SpeakNoEvilMonkey = char.ConvertFromUtf32(0x1F64A);                                                    //      🙊
        public static readonly string HappyPersonRaisingOneHand = char.ConvertFromUtf32(0x1F64B);                                                    //      🙋
        public static readonly string PersonRaisingBothHandsInCelebration = char.ConvertFromUtf32(0x1F64C);                                                    //      🙌
        public static readonly string PersonFrowning = char.ConvertFromUtf32(0x1F64D);                                                    //      🙍
        public static readonly string PersonWithPoutingFace = char.ConvertFromUtf32(0x1F64E);                                                    //      🙎
        public static readonly string PersonWithFoldedHands = char.ConvertFromUtf32(0x1F64F);                                                    //      🙏

        // Not official emoticons from  http://www.unicode.org/charts/PDF/U2600.pdf           
        public static readonly string Snowman = char.ConvertFromUtf32(0x2603);                                                     //      ☃ 
        public static readonly string HiteFrowningFace = char.ConvertFromUtf32(0x2639);                                                     //      ☹ 
        public static readonly string HiteSmilingFace = char.ConvertFromUtf32(0x263a);                                                     //      ☺ 
        public static readonly string LackSmilingFace = char.ConvertFromUtf32(0x263b);                                                     //      ☻ 
        public static readonly string NowmanWithoutSnow = char.ConvertFromUtf32(0x26c4);                                                     //      ⛄ 
        public static readonly string KullAndCrossbones = char.ConvertFromUtf32(0x2620);                                                     //      ☠ 		

        */

    }
}
