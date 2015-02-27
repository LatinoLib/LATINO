/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    TextProcessing.cs
 *  Desc:    Tutorial X: Text processing utitilite: emoticon, url, unusual punctuation handling 
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Latino;
using Latino.TextMining;
using Microsoft.VisualBasic.FileIO;

namespace Tutorial.Case.Data
{
    public class TextProcessing : Tutorial<TextProcessing>
    {
        public override void Run(object[] args)
        {
            List<string> corpus;
            corpus = new List<string>()
            {
                "This is my first document.", 
                "This is my second document.", 
                "I am angry !!!!!",
                "I love you \u2661 and sen you a @>-->--.",
                "This is funny !!!  "+char.ConvertFromUtf32(0x1F605), 
                "You are dead .",
                @"Urls http://unicode-table.com/en/2661/, http://en.wikipedia.org/wiki/List_of_emoticons#cite_note-16, http://newstream.ijs.si/?occurrence=7714&w=1&macd_params=7%2C28%2C7&chart_view=line&sentiment_scope=document&sentiment_aggregation=sum&min_occ=2&entity_tags=on&title_tags=on&rangeMin=&rangeMax=#graph",
                "twitter users @user",
                "...On that note, I hate Word. I hate Pages. I hate LaTeX. There, I said it. I hate LaTeX. All you TEXN3RDS can come kill me now.",
                ".Although today's keynote rocked, for every great announcement, AT&amp;T shit on us just a little bit more.",
                "............................As u may have noticed, not too happy about the GM situation, nor AIG, Lehman, et al",
                "RT @SmartChickPDX: Was just told that Nike layoffs started today :-(",
                "not at begginning, middle endnote, beginning of gord noticed end eof word endnot and end of sentence not. and line not",
                "Un inutile strage di poveri animali innocenti, esseri umani degradati e degradanti ...:-(((",
                "loooooooooove is niceeee, while school is not"
                

            };
            /*
            // read the data
            string filename = @"Data\testdata.manual.2009.06.14.csv";                        
            var parser = new TextFieldParser(filename, Encoding.ASCII)
            {
                TextFieldType = FieldType.Delimited
            };
            try
            {
                parser.SetDelimiters(",");                
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();                
                    corpus.Add(fields[5]);                                                   
                }                
            }
            finally
            {
                parser.Close();
            }
            */
            
            // initialize the emoticons
            var emoticonTransform = new Dictionary<Regex, string>
            {
                { SocialMediaProcessing.HappyEmoticonsRegex, "__HAPPY__"},
                { SocialMediaProcessing.SadEmoticonsRegex, "__SAD__"}
            };

            // order of application of these functions IS important
            foreach (var example in corpus)     
            {
                var str = example;
                str = SocialMediaProcessing.ReplaceUrls(str);
                str = SocialMediaProcessing.AddLengthFeatures(str);
                str = SocialMediaProcessing.IsUppercased(str);
                str = SocialMediaProcessing.ReplaceUsers(str);
                str = SocialMediaProcessing.ReplaceHashTags(str);
                str = SocialMediaProcessing.ReplaceNegations(str, Language.Italian);
                str = SocialMediaProcessing.ReplaceStockSymbol(str);
                str = SocialMediaProcessing.ReplaceMultiplePunctuation(str);
                str = SocialMediaProcessing.ReplaceEmoticons(str, emoticonTransform);          // to be used after ReplaceUrls
                str = SocialMediaProcessing.RemoveCharacterRepetition(str);
                //                str = Emoticons.ReplaceUnicodeCategory(str, UnicodeCategory.OtherSymbol, "__UnicodeEmoticon__");
                
                if (str != example) // print only modified strings
                {
                    Console.WriteLine(example);
                    Console.WriteLine(str);
                    Console.WriteLine();
                }
                                
            }
        }
    }
}
