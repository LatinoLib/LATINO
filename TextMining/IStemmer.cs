/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IStemmer.cs
 *  Desc:    Stemmer/lemmatizer interface 
 *  Created: Dec-2008
 *
 *  Authors: Miha Grcar
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
