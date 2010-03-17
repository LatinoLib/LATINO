/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          NorwegianStopWords.cs
 *  Version:       1.0
 *  Desc:		   Norwegian stop words
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
        // this list is taken from http://snowball.tartarus.org/algorithms/norwegian/stop.txt
        public static Set<string>.ReadOnly NorwegianStopWords
            = new Set<string>.ReadOnly(new Set<string>(new string[] {
                // This stop word list is for the dominant bokm�l dialect. Words unique
                // to nynorsk are marked *.
                // Revised by Jan Bruusgaard <Jan.Bruusgaard@ssb.no>, Jan 2005
                "og",             // and
                "i",              // in
                "jeg",            // I
                "det",            // it/this/that
                "at",             // to (w. inf.)
                "en",             // a/an
                "et",             // a/an
                "den",            // it/this/that
                "til",            // to
                "er",             // is/am/are
                "som",            // who/that
                "p�",             // on
                "de",             // they / you(formal)
                "med",            // with
                "han",            // he
                "av",             // of
                "ikke",           // not
                "ikkje",          // not *
                "der",            // there
                "s�",             // so
                "var",            // was/were
                "meg",            // me
                "seg",            // you
                "men",            // but
                "ett",            // one
                "har",            // have
                "om",             // about
                "vi",             // we
                "min",            // my
                "mitt",           // my
                "ha",             // have
                "hadde",          // had
                "hun",            // she
                "n�",             // now
                "over",           // over
                "da",             // when/as
                "ved",            // by/know
                "fra",            // from
                "du",             // you
                "ut",             // out
                "sin",            // your
                "dem",            // them
                "oss",            // us
                "opp",            // up
                "man",            // you/one
                "kan",            // can
                "hans",           // his
                "hvor",           // where
                "eller",          // or
                "hva",            // what
                "skal",           // shall/must
                "selv",           // self (reflective)
                "sj�l",           // self (reflective)
                "her",            // here
                "alle",           // all
                "vil",            // will
                "bli",            // become
                "ble",            // became
                "blei",           // became *
                "blitt",          // have become
                "kunne",          // could
                "inn",            // in
                "n�r",            // when
                "v�re",           // be
                "kom",            // come
                "noen",           // some
                "noe",            // some
                "ville",          // would
                "dere",           // you
                "som",            // who/which/that
                "deres",          // their/theirs
                "kun",            // only/just
                "ja",             // yes
                "etter",          // after
                "ned",            // down
                "skulle",         // should
                "denne",          // this
                "for",            // for/because
                "deg",            // you
                "si",             // hers/his
                "sine",           // hers/his
                "sitt",           // hers/his
                "mot",            // against
                "�",              // to
                "meget",          // much
                "hvorfor",        // why
                "dette",          // this
                "disse",          // these/those
                "uten",           // without
                "hvordan",        // how
                "ingen",          // none
                "din",            // your
                "ditt",           // your
                "blir",           // become
                "samme",          // same
                "hvilken",        // which
                "hvilke",         // which (plural)
                "s�nn",           // such a
                "inni",           // inside/within
                "mellom",         // between
                "v�r",            // our
                "hver",           // each
                "hvem",           // who
                "vors",           // us/ours
                "hvis",           // whose
                "b�de",           // both
                "bare",           // only/just
                "enn",            // than
                "fordi",          // as/because
                "f�r",            // before
                "mange",          // many
                "ogs�",           // also
                "slik",           // just
                "v�rt",           // been
                "v�re",           // to be
                "b�e",            // both *
                "begge",          // both
                "siden",          // since
                "dykk",           // your *
                "dykkar",         // yours *
                "dei",            // they *
                "deira",          // them *
                "deires",         // theirs *
                "deim",           // them *
                "di",             // your (fem.) *
                "d�",             // as/when *
                "eg",             // I *
                "ein",            // a/an *
                "eit",            // a/an *
                "eitt",           // a/an *
                "elles",          // or *
                "honom",          // he *
                "hj�",            // at *
                "ho",             // she *
                "hoe",            // she *
                "henne",          // her
                "hennar",         // her/hers
                "hennes",         // hers
                "hoss",           // how *
                "hossen",         // how *
                "ikkje",          // not *
                "ingi",           // noone *
                "inkje",          // noone *
                "korleis",        // how *
                "korso",          // how *
                "kva",            // what/which *
                "kvar",           // where *
                "kvarhelst",      // where *
                "kven",           // who/whom *
                "kvi",            // why *
                "kvifor",         // why *
                "me",             // we *
                "medan",          // while *
                "mi",             // my *
                "mine",           // my *
                "mykje",          // much *
                "no",             // now *
                "nokon",          // some (masc./neut.) *
                "noka",           // some (fem.) *
                "nokor",          // some *
                "noko",           // some *
                "nokre",          // some *
                "si",             // his/hers *
                "sia",            // since *
                "sidan",          // since *
                "so",             // so *
                "somt",           // some *
                "somme",          // some *
                "um",             // about*
                "upp",            // up *
                "vere",           // be *
                "vore",           // was *
                "verte",          // become *
                "vort",           // become *
                "varte",          // became *
                "vart"}));        // became *    
    }
}
