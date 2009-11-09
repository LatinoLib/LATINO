/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          PortugueseStopWords.cs
 *  Version:       1.0
 *  Desc:		   Portuguese stop words
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
       |  Static class StopWords
       |
       '-----------------------------------------------------------------------
    */
    public static partial class StopWords
    {
        // this list is taken from http://snowball.tartarus.org/algorithms/portuguese/stop.txt
        public static Set<string>.ReadOnly PortugueseStopWords
            = new Set<string>.ReadOnly(new Set<string>(new string[] {
                // A Portuguese stop word list. 
                // The following is a ranked list (commonest to rarest) of stopwords
                // deriving from a large sample of text.
                // Extra words have been added at the end.
                "de",             //  of, from
                "a",              //  the; to, at; her
                "o",              //  the; him
                "que",            //  who, that
                "e",              //  and
                "do",             //  de + o
                "da",             //  de + a
                "em",             //  in
                "um",             //  a
                "para",           //  for
                  // �          from SER
                "com",            //  with
                "n�o",            //  not, no
                "uma",            //  a
                "os",             //  the; them
                "no",             //  em + o
                "se",             //  himself etc
                "na",             //  em + a
                "por",            //  for
                "mais",           //  more
                "as",             //  the; them
                "dos",            //  de + os
                "como",           //  as, like
                "mas",            //  but
                  // foi        from SER
                "ao",             //  a + o
                "ele",            //  he
                "das",            //  de + as
                  // tem        from TER
                "�",              //  a + a
                "seu",            //  his
                "sua",            //  her
                "ou",             //  or
                  // ser        from SER
                "quando",         //  when
                "muito",          //  much
                  // h�         from HAV
                "nos",            //  em + os; us
                "j�",             //  already, now
                  // est�       from EST
                "eu",             //  I
                "tamb�m",         //  also
                "s�",             //  only, just
                "pelo",           //  per + o
                "pela",           //  per + a
                "at�",            //  up to
                "isso",           //  that
                "ela",            //  he
                "entre",          //  between
                  // era        from SER
                "depois",         //  after
                "sem",            //  without
                "mesmo",          //  same
                "aos",            //  a + os
                  // ter        from TER
                "seus",           //  his
                "quem",           //  whom
                "nas",            //  em + as
                "me",             //  me
                "esse",           //  that
                "eles",           //  they
                  // est�o      from EST
                "voc�",           //  you
                  // tinha      from TER
                  // foram      from SER
                "essa",           //  that
                "num",            //  em + um
                "nem",            //  nor
                "suas",           //  her
                "meu",            //  my
                "�s",             //  a + as
                "minha",          //  my
                  // t�m        from TER
                "numa",           //  em + uma
                "pelos",          //  per + os
                "elas",           //  they
                  // havia      from HAV
                  // seja       from SER
                "qual",           //  which
                  // ser�       from SER
                "n�s",            //  we
                  // tenho      from TER
                "lhe",            //  to him, her
                "deles",          //  of them
                "essas",          //  those
                "esses",          //  those
                "pelas",          //  per + as
                "este",           //  this
                  // fosse      from SER
                "dele",           //  of him
                 // other words. There are many contractions such as naquele = em+aquele,
                 // mo = me+o, but they are rare.
                 // Indefinite article plural forms are also rare.
                "tu",             //  thou
                "te",             //  thee
                "voc�s",          //  you (plural)
                "vos",            //  you
                "lhes",           //  to them
                "meus",           //  my
                "minhas",
                "teu",            //  thy
                "tua",
                "teus",
                "tuas",
                "nosso",          // our
                "nossa",
                "nossos",
                "nossas",
                "dela",           //  of her
                "delas",          //  of them
                "esta",           //  this
                "estes",          //  these
                "estas",          //  these
                "aquele",         //  that
                "aquela",         //  that
                "aqueles",        //  those
                "aquelas",        //  those
                "isto",           //  this
                "aquilo",         //  that
                               // forms of estar, to be (not including the infinitive):
                "estou",
                "est�",
                "estamos",
                "est�o",
                "estive",
                "esteve",
                "estivemos",
                "estiveram",
                "estava",
                "est�vamos",
                "estavam",
                "estivera",
                "estiv�ramos",
                "esteja",
                "estejamos",
                "estejam",
                "estivesse",
                "estiv�ssemos",
                "estivessem",
                "estiver",
                "estivermos",
                "estiverem",
                               // forms of haver, to have (not including the infinitive):
                "hei",
                "h�",
                "havemos",
                "h�o",
                "houve",
                "houvemos",
                "houveram",
                "houvera",
                "houv�ramos",
                "haja",
                "hajamos",
                "hajam",
                "houvesse",
                "houv�ssemos",
                "houvessem",
                "houver",
                "houvermos",
                "houverem",
                "houverei",
                "houver�",
                "houveremos",
                "houver�o",
                "houveria",
                "houver�amos",
                "houveriam",
                               // forms of ser, to be (not including the infinitive):
                "sou",
                "somos",
                "s�o",
                "era",
                "�ramos",
                "eram",
                "fui",
                "foi",
                "fomos",
                "foram",
                "fora",
                "f�ramos",
                "seja",
                "sejamos",
                "sejam",
                "fosse",
                "f�ssemos",
                "fossem",
                "for",
                "formos",
                "forem",
                "serei",
                "ser�",
                "seremos",
                "ser�o",
                "seria",
                "ser�amos",
                "seriam",
                               // forms of ter, to have (not including the infinitive):
                "tenho",
                "tem",
                "temos",
                "t�m",
                "tinha",
                "t�nhamos",
                "tinham",
                "tive",
                "teve",
                "tivemos",
                "tiveram",
                "tivera",
                "tiv�ramos",
                "tenha",
                "tenhamos",
                "tenham",
                "tivesse",
                "tiv�ssemos",
                "tivessem",
                "tiver",
                "tivermos",
                "tiverem",
                "terei",
                "ter�",
                "teremos",
                "ter�o",
                "teria",
                "ter�amos",
                "teriam"}));
    }
}
