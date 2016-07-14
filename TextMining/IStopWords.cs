/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IStopWords.cs
 *  Desc:    Stop words interface 
 *  Created: Jul-2016
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IStopWords
       |
       '-----------------------------------------------------------------------
    */
    public interface IStopWords : ISerializable
    {
        bool Contains(string word);
    }
}
