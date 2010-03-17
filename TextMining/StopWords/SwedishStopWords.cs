/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          SwedishStopWords.cs
 *  Version:       1.0
 *  Desc:		   Swedish stop words
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
        // this list is taken from http://snowball.tartarus.org/algorithms/swedish/stop.txt
        public static Set<string>.ReadOnly SwedishStopWords
            = new Set<string>.ReadOnly(new Set<string>(new string[] {
                // A Swedish stop word list. 
                // This is a ranked list (commonest to rarest) of stopwords derived from
                // a large text sample.
                // Swedish stop words occasionally exhibit homonym clashes. For example
                // s� = so, but also seed. These are indicated clearly below.
                "och",            // and
                "det",            // it, this/that
                "att",            // to (with infinitive)
                "i",              // in, at
                "en",             // a
                "jag",            // I
                "hon",            // she
                "som",            // who, that
                "han",            // he
                "p�",             // on
                "den",            // it, this/that
                "med",            // with
                "var",            // where, each
                "sig",            // him(self) etc
                "f�r",            // for
                "s�",             // so (also: seed)
                "till",           // to
                "�r",             // is
                "men",            // but
                "ett",            // a
                "om",             // if; around, about
                "hade",           // had
                "de",             // they, these/those
                "av",             // of
                "icke",           // not, no
                "mig",            // me
                "du",             // you
                "henne",          // her
                "d�",             // then, when
                "sin",            // his
                "nu",             // now
                "har",            // have
                "inte",           // inte n�gon = no one
                "hans",           // his
                "honom",          // him
                "skulle",         // 'sake'
                "hennes",         // her
                "d�r",            // there
                "min",            // my
                "man",            // one (pronoun)
                "ej",             // nor
                "vid",            // at, by, on (also: vast)
                "kunde",          // could
                "n�got",          // some etc
                "fr�n",           // from, off
                "ut",             // out
                "n�r",            // when
                "efter",          // after, behind
                "upp",            // up
                "vi",             // we
                "dem",            // them
                "vara",           // be
                "vad",            // what
                "�ver",           // over
                "�n",             // than
                "dig",            // you
                "kan",            // can
                "sina",           // his
                "h�r",            // here
                "ha",             // have
                "mot",            // towards
                "alla",           // all
                "under",          // under (also: wonder)
                "n�gon",          // some etc
                "eller",          // or (else)
                "allt",           // all
                "mycket",         // much
                "sedan",          // since
                "ju",             // why
                "denna",          // this/that
                "sj�lv",          // myself, yourself etc
                "detta",          // this/that
                "�t",             // to
                "utan",           // without
                "varit",          // was
                "hur",            // how
                "ingen",          // no
                "mitt",           // my
                "ni",             // you
                "bli",            // to be, become
                "blev",           // from bli
                "oss",            // us
                "din",            // thy
                "dessa",          // these/those
                "n�gra",          // some etc
                "deras",          // their
                "blir",           // from bli
                "mina",           // my
                "samma",          // (the) same
                "vilken",         // who, that
                "er",             // you, your
                "s�dan",          // such a
                "v�r",            // our
                "blivit",         // from bli
                "dess",           // its
                "inom",           // within
                "mellan",         // between
                "s�dant",         // such a
                "varf�r",         // why
                "varje",          // each
                "vilka",          // who, that
                "ditt",           // thy
                "vem",            // who
                "vilket",         // who, that
                "sitta",          // his
                "s�dana",         // such a
                "vart",           // each
                "dina",           // thy
                "vars",           // whose
                "v�rt",           // our
                "v�ra",           // our
                "ert",            // your
                "era",            // your
                "vilkas"}));      // whose    
    }
}
