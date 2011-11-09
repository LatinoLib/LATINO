/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IStemmer.cs
 *  Desc:    Stemmer/lemmatizer interface 
 *  Created: Dec-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IStemmer
       |
       '-----------------------------------------------------------------------
    */
    public interface IStemmer : ISerializable
    {
        string GetStem(string word);
    }
}
