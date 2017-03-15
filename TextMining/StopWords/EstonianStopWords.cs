/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    EstonianStopWords.cs
 *  Desc:    Estonian stop words
 *  Created: Jul-2016
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class StopWords
       |
       '-----------------------------------------------------------------------
    */
    public partial class StopWords
    {
        // this list is taken from https://github.com/6/stopwords-json
        public static StopWords EstonianStopWords
            = new StopWords(new string[] {
                "aga",
                "ei",
                "et",
                "ja",
                "jah",
                "kas",
                "kui",
                "kõik",
                "ma",
                "me",
                "mida",
                "midagi",
                "mind",
                "minu",
                "mis",
                "mu",
                "mul",
                "mulle",
                "nad",
                "nii",
                "oled",
                "olen",
                "oli",
                "oma",
                "on",
                "pole",
                "sa",
                "seda",
                "see",
                "selle",
                "siin",
                "siis",
                "ta",
                "te",
                "ära"});
    }
}
