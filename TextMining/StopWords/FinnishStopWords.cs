/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          FinnishStopWords.cs
 *  Version:       1.0
 *  Desc:		   Finnish stop words
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
    public static partial class StopWords
    {
        // this list is taken from http://snowball.tartarus.org/algorithms/finnish/stop.txt
        public static Set<string>.ReadOnly FinnishStopWords
            = new Set<string>.ReadOnly(new Set<string>(new string[] {
                // forms of BE
                "olla",
                "olen",
                "olet",
                "on",
                "olemme",
                "olette",
                "ovat",
                "ole",        // negative form
                "oli",
                "olisi",
                "olisit",
                "olisin",
                "olisimme",
                "olisitte",
                "olisivat",
                "olit",
                "olin",
                "olimme",
                "olitte",
                "olivat",
                "ollut",
                "olleet",
                "en",         // negation
                "et",
                "ei",
                "emme",
                "ette",
                "eiv�t",
                // I
                "min�",   
                "minun",  
                "minut",  
                "minua",  
                "minussa", 
                "minusta", 
                "minuun", 
                "minulla", 
                "minulta", 
                "minulle", 
                // you
                "sin�",   
                "sinun",  
                "sinut",  
                "sinua",  
                "sinussa", 
                "sinusta", 
                "sinuun", 
                "sinulla", 
                "sinulta", 
                "sinulle",   
                // he she
                "h�n",    
                "h�nen",  
                "h�net",  
                "h�nt�",  
                "h�ness�", 
                "h�nest�", 
                "h�neen", 
                "h�nell�", 
                "h�nelt�", 
                "h�nelle",    
                // we
                "me",     
                "meid�n", 
                "meid�t", 
                "meit�",  
                "meiss�",  
                "meist�",  
                "meihin", 
                "meill�",  
                "meilt�",  
                "meille",  
                // you
                "te",     
                "teid�n", 
                "teid�t", 
                "teit�",  
                "teiss�",  
                "teist�",  
                "teihin", 
                "teill�",  
                "teilt�",  
                "teille",    
                // they
                "he",     
                "heid�n", 
                "heid�t", 
                "heit�",  
                "heiss�",  
                "heist�",  
                "heihin", 
                "heill�",  
                "heilt�",  
                "heille",                
                // this
                "t�m�",   
                "t�m�n",         
                "t�t�",   
                "t�ss�",   
                "t�st�",   
                "t�h�n",  
                "tall�",   
                "t�lt�",   
                "t�lle",   
                "t�n�",   
                "t�ksi",  
                // that
                "tuo",    
                "tuon",          
                "tuot�",  
                "tuossa",  
                "tuosta",  
                "tuohon", 
                "tuolla",  
                "tuolta",  
                "tuolle",  
                "tuona",  
                "tuoksi", 
                // it
                "se",     
                "sen",           
                "sit�",   
                "siin�",   
                "siit�",   
                "siihen", 
                "sill�",   
                "silt�",   
                "sille",   
                "sin�",   
                "siksi",  
                // these
                "n�m�",   
                "n�iden",        
                "n�it�",  
                "n�iss�",  
                "n�ist�",  
                "n�ihin", 
                "n�ill�",  
                "n�ilt�",  
                "n�ille",  
                "n�in�",  
                "n�iksi", 
                // those
                "nuo",    
                "noiden",        
                "noita",  
                "noissa",  
                "noista",  
                "noihin", 
                "noilla",  
                "noilta",  
                "noille",  
                "noina",  
                "noiksi",
                // they
                "ne",     
                "niiden",        
                "niit�",  
                "niiss�",  
                "niist�",  
                "niihin", 
                "niill�",  
                "niilt�",  
                "niille",  
                "niin�",  
                "niiksi", 
                // who
                "kuka",   
                "kenen", 
                "kenet",   
                "ket�",   
                "keness�", 
                "kenest�", 
                "keneen", 
                "kenell�", 
                "kenelt�", 
                "kenelle", 
                "kenen�", 
                "keneksi",
                // who (pl)
                "ketk�",  
                "keiden", 
                "ketk�",  
                "keit�",  
                "keiss�",  
                "keist�",  
                "keihin", 
                "keill�",  
                "keilt�",  
                "keille",  
                "kein�",  
                "keiksi", 
                // which what
                "mik�",   
                "mink�", 
                "mink�",   
                "mit�",   
                "miss�",   
                "mist�",   
                "mihin",  
                "mill�",   
                "milt�",   
                "mille",   
                "min�",   
                "miksi",  
                // which what (pl)
                "mitk�",                                                                                    
                // who which
                "joka",   
                "jonka",         
                "jota",   
                "jossa",   
                "josta",   
                "johon",  
                "jolla",   
                "jolta",   
                "jolle",   
                "jona",   
                "joksi",  
                // who which (pl)
                "jotka",  
                "joiden",        
                "joita",  
                "joissa",  
                "joista",  
                "joihin", 
                "joilla",  
                "joilta",  
                "joille",  
                "joina",  
                "joiksi", 
                // conjunctions
                "ett�",   // that
                "ja",     // and
                "jos",    // if
                "koska",  // because
                "kuin",   // than
                "mutta",  // but
                "niin",   // so
                "sek�",   // and
                "sill�",  // for
                "tai",    // or
                "vaan",   // but
                "vai",    // or
                "vaikka", // although
                // prepositions
                "kanssa",  // with
                "mukaan",  // according to
                "noin",    // about
                "poikki",  // across
                "yli",     // over, across
                // other
                "kun",    // when
                "niin",   // so
                "nyt",    // now
                "itse"}));// self
    }
}
