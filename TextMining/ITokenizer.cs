/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Tokenizer.cs
 *  Desc:    Tokenizer interface 
 *  Created: Apr-2009
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System.Collections.Generic;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface ITokenizer
       |
       '-----------------------------------------------------------------------
    */
    public interface ITokenizer : IEnumerable<string>, ISerializable
    {
        string Text { get; set; }
    }
}
