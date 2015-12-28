/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    TextProcessing.cs
 *  Desc:    Tutorial X: Text processing utitilite: emoticon, url, unusual punctuation handling 
 *  Created: Dec-2015
 *
 *  Authors: petra
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Latino.TextMining;

namespace Tutorial.Case.Data
{
    public class TextProcessing : Tutorial<TextProcessing>
    {
        public override void Run(object[] args)
        {
            var corpus = new[]
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
                    "............................As u may have noticed, not too happy about the GM situation, nor AIG, Lehman, et al?",
                    "RT @SmartChickPDX: Was just told that Nike layoffs started today :-(",
                    "not at begginning, middle endnote, beginning of gord noticed end eof word endnot and end of sentence not. and line not",
                    "Un inutile strage di poveri animali innocenti, esseri umani degradati e degradanti ...:-(((",
                    "loooooooooove is niceeee, while school is not",
                    "YESSS...",
                    "che cazzo dici cazzo",
                    "MERDA !!!",
                    "Grande popolo sterminato senza pietŕ č ricordato da nessuno!",
                    "Solo che qui non li bruciano nei forni e neanche li uccidono con il gas Comunque stanno solo in fila per fare i controlli di sicurezza, in aeroporto ci sono file anche piů lunghe, ma tanto Carmelo vuo vedere solo quello che vuole vedere. A Carmč ripigliate !",
                    "r.i.p.",
                    "esattamente,e poi bastava guardare il video fino alla fine.. http://www.tankerenemy.com/2010/11/pseudomonas-syringae-il-batterio-dei.html?m=1",
                    "la prossima volta PIU' BOTTE :-)",
                    "GRANDE VERITA'....POSSIBILE CHE L'UOMO NON CAPISCA..!!!",
                    "Č i vegani mangiando fagioli , e scoreggiando sono causa del buco nell ozono? Moralizzate questa",
                    "Rispetto totALE!!!!",
                    "non ci possiamo fidare piů di nessunoooooo ora pure i medici si ci mettono???????? vergogna giocano con la nostra vita!!!!!! ma chi DIO ce la mandi buona!!!!",
                    "Bellissima ;) E alquanto realistica purtroppo -.-",
                    "Bionda se ti vedo io ti prendo a pedate e ti appendo a testa in giů.",
                    "Un esempio chiave Mario monti e loro si puliscono il culo col vostro voto e anche della costituzione e della cosě detta democrazia.  Monti = goldman sachs , trilaterale, massone, aspen.",

                    @":-) :) :D :o) :] :3 :c) :> =] 8) =) :} :^) :っ)	Smiley or happy face.[4][5][6]",
                    @":-D 8-D 8D x-D xD X-D XD =-D =D =-3 =3 B^D	Laughing,[4] big grin,[5][6] laugh with glasses[7]",
                    @":-))	Very happy or double chin[7]",
                    @">:[ :-( :(  :-c :c :-<  :っC :< :-[ :[ :{	Frown,[4][5][6] sad[8]",
                    @";(	Winky frowny, used to signify sadness, with a bit of sarcasm. It is easily misunderstood.[9]",
                    @":-|| :@ >:(	Angry[7]",
                    @":'-( :'(	Crying[8]",
                    @":'-) :')	Tears of happiness[8]",
                    @"D:< D: D8 D; D= DX v.v D-':	Horror, disgust, sadness, great dismay[5][6]",
                    @">:O :-O :O :-o :o 8-0 O_O o-o O_o o_O o_o O-O	Surprise,[3] shock,[4][10] yawn[11]",
                    @":* :^* ( '}{' )	Kiss, couple kissing[7]",
                    @";-) ;) *-) *) ;-] ;] ;D ;^) :-,	Wink,[4][5][6] smirk[10][11]",
                    @">:P :-P :P X-P x-p xp XP :-p :p =p :-Þ :Þ :þ :-þ :-b :b d:	Tongue sticking out, cheeky/playful,[4] blowing a raspberry",
                    @">:\ >:/ :-/ :-. :/ :\ =/ =\ :L =L :S >.<	Skeptical, annoyed, undecided, uneasy, hesitant[4]",
                    @":| :-|	Straight face[5] no expression, indecision[8]",
                    @":$	Embarrassed,[6] blushing[7]",
                    @":-X :X :-# :#	Sealed lips or wearing braces[4]",
                    @"O:-) 0:-3 0:3 0:-) 0:) 0;^)	Angel,[4][5][10] saint,[8] innocent",
                    @">:) >;) >:-)	Evil[5]",
                    @"}:-) }:) 3:-) 3:)	Devilish[8]",
                    @"o/\o ^5 >_>^ ^<_<	High five[11]",
                    @"|;-) |-O	Cool,[8] bored/yawning[10]",
                    @":-J	Tongue-in-cheek[12]",
                    @":-& :&	Tongue-tied[8]",
                    @"#-)	Partied all night[8]",
                    @"%-) %)	Drunk,[8] confused",
                    @":-###.. :###..	Being sick[8]",
                    @"<:-|	Dumb, dunce-like[10]",
                    @"ಠ_ಠ	Look of disapproval[13] The Unicode character ಠ is from the Kannada alphabet and can be called differently in HTML notation: &#3232; and &#x0CA0; (for Unicode)",
                    @"<*)))-{ ><(((*> ><>	Fish, something's fishy,[10] Christian fish[7]",
                    @"*\0/*	Cheerleader[7]",
                    @"@}-;-'--- @>-->--	Rose[4][10]",
                    @"~(_8^(I)	Homer Simpson[10]",
                    @"5:-) ~:-\	Elvis Presley[10][11]",
                    @"//0-0\\	John Lennon[10]",
                    @"*<|:-)	Santa Claus[14]",
                    @"=:o]	Bill Clinton[14]",
                    @",:-) 7:^]	Ronald Reagan[14]",
                    @"<3 </3	Heart and broken-heart (reverse-rotation)[15]",
                    @"( ͡° ͜ʖ ͡°)",
                    @":-) :-"
                };

            var processor = new TextFeatureProcessor()
                .With(new SocialMediaProcessing.NormalizeDiacriticalCharactersFeature())
                .With(new SocialMediaProcessing.UrlFeature())
                //.With(new SocialMediaProcessing.MessageLengthFeature())
                .With(new SocialMediaProcessing.StockSymbolFeature())
                .With(new SocialMediaProcessing.UppercasedFeature())
                .With(new SocialMediaProcessing.TwitterUserFeature())
                .With(new SocialMediaProcessing.HashTagFeature())
                .With(new SocialMediaProcessing.NegationFeature(Language.Italian))

                .With(new SocialMediaProcessing.SwearingFeature(Language.Italian))
                .With(new SocialMediaProcessing.PositiveWordFeature(Language.Italian))

                .With(new SocialMediaProcessing.LastExclamationFeature())
                .With(new SocialMediaProcessing.LastQuestionMarkFeature())
                .With(new SocialMediaProcessing.SingleExclamationFeature())
                .With(new SocialMediaProcessing.SingleQuestionMarkFeature())
                .With(new SocialMediaProcessing.MultipleMixedPunctuationFeature())
                .With(new SocialMediaProcessing.MultipleQuestionMarkFeature())
                .With(new SocialMediaProcessing.MultipleExclamationFeature())

                .With(new SocialMediaProcessing.LastSadEmoticonsFeature())
                .With(new SocialMediaProcessing.LastHappyEmoticonsFeature())
                .With(new SocialMediaProcessing.HappySadEmoticonsFeature())

                .With(new SocialMediaProcessing.RepetitionFeature());


            // order of application of these functions IS important
            foreach (string example in corpus)     
            {
                
                if (example.Contains("cazzo"))
                {

                string str = processor.Run(example);
                
                if (str != example) // print only modified strings
                {
                    Output.WriteLine(example);
                    Output.WriteLine(str);
                    Output.WriteLine();
                    Output.Flush();
                }
                }
            }

            // counting the emoticons
            var counts = new Dictionary<string, int>();
            foreach (string corpu in corpus)
            {
                int count = EmoticonCounter.Count(corpu, ref counts);
                Output.WriteLine("{0} found in  {1}", count, corpu);
            }
            Output.WriteLine("\nResults:");
            foreach (KeyValuePair<string, int> kv in counts)
            {
                Output.WriteLine("{0} - {1}", kv.Key, kv.Value);
            }

            Output.Flush();
        }
    }
}
