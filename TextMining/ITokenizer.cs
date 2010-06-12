/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Tokenizer.cs
 *  Desc:    Tokenizer interface 
 *  Created: Apr-2009
 *
 *  Authors: Miha Grcar
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
