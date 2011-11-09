/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    FinnishStopWords.cs
 *  Desc:    Finnish stop words
 *  Created: Nov-2009
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
                "eivät",
                // I
                "minä",   
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
                "sinä",   
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
                "hän",    
                "hänen",  
                "hänet",  
                "häntä",  
                "hänessä", 
                "hänestä", 
                "häneen", 
                "hänellä", 
                "häneltä", 
                "hänelle",    
                // we
                "me",     
                "meidän", 
                "meidät", 
                "meitä",  
                "meissä",  
                "meistä",  
                "meihin", 
                "meillä",  
                "meiltä",  
                "meille",  
                // you
                "te",     
                "teidän", 
                "teidät", 
                "teitä",  
                "teissä",  
                "teistä",  
                "teihin", 
                "teillä",  
                "teiltä",  
                "teille",    
                // they
                "he",     
                "heidän", 
                "heidät", 
                "heitä",  
                "heissä",  
                "heistä",  
                "heihin", 
                "heillä",  
                "heiltä",  
                "heille",                
                // this
                "tämä",   
                "tämän",         
                "tätä",   
                "tässä",   
                "tästä",   
                "tähän",  
                "tallä",   
                "tältä",   
                "tälle",   
                "tänä",   
                "täksi",  
                // that
                "tuo",    
                "tuon",          
                "tuotä",  
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
                "sitä",   
                "siinä",   
                "siitä",   
                "siihen", 
                "sillä",   
                "siltä",   
                "sille",   
                "sinä",   
                "siksi",  
                // these
                "nämä",   
                "näiden",        
                "näitä",  
                "näissä",  
                "näistä",  
                "näihin", 
                "näillä",  
                "näiltä",  
                "näille",  
                "näinä",  
                "näiksi", 
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
                "niitä",  
                "niissä",  
                "niistä",  
                "niihin", 
                "niillä",  
                "niiltä",  
                "niille",  
                "niinä",  
                "niiksi", 
                // who
                "kuka",   
                "kenen", 
                "kenet",   
                "ketä",   
                "kenessä", 
                "kenestä", 
                "keneen", 
                "kenellä", 
                "keneltä", 
                "kenelle", 
                "kenenä", 
                "keneksi",
                // who (pl)
                "ketkä",  
                "keiden", 
                "ketkä",  
                "keitä",  
                "keissä",  
                "keistä",  
                "keihin", 
                "keillä",  
                "keiltä",  
                "keille",  
                "keinä",  
                "keiksi", 
                // which what
                "mikä",   
                "minkä", 
                "minkä",   
                "mitä",   
                "missä",   
                "mistä",   
                "mihin",  
                "millä",   
                "miltä",   
                "mille",   
                "minä",   
                "miksi",  
                // which what (pl)
                "mitkä",                                                                                    
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
                "että",   // that
                "ja",     // and
                "jos",    // if
                "koska",  // because
                "kuin",   // than
                "mutta",  // but
                "niin",   // so
                "sekä",   // and
                "sillä",  // for
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
