/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Language.cs
 *  Desc:    Enum of supported languages 
 *  Created: Nov-2009
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Enum Language
       |
       '-----------------------------------------------------------------------
    */
    public enum Language
    {
        Unspecified = 0,
        English,
        French,
        German,
        Spanish,
        Danish,
        Dutch,
        Finnish,
        Italian,
        Norwegian,
        Portuguese,
        Swedish,
        Serbian,     
        Slovene,     
        Romanian,
        Hungarian,
        Estonian, // *** missing stop words
        Bulgarian,
        Czech,
        Russian,
        // *** language detection only
        Greek,
        Lithuanian,
        Latvian,
        Maltese,
        Polish,
        Slovak,
        Turkish,
        Vietnamese,
        Icelandic
    }
}
