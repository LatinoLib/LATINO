/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          IStemmer.cs
 *  Version:       1.0
 *  Desc:		   Word stemmer interface 
 *  Author:        Miha Grcar
 *  Created on:    Dec-2008
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
