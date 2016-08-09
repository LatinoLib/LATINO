﻿/*==========================================================================;
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

using System;
using System.Collections;
using System.Collections.Generic;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface ITokenizer
       |
       '-----------------------------------------------------------------------
    */
    public interface ITokenizer : ISerializable
    {
        ITokenizerEnumerable GetTokens(string text);
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface ITokenizerEnumerable
       |
       '-----------------------------------------------------------------------
    */
    public interface ITokenizerEnumerable : IEnumerable<string> 
    {
        new ITokenizerEnumerator GetEnumerator();
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface ITokenizerEnumerator
       |
       '-----------------------------------------------------------------------
    */
    public interface ITokenizerEnumerator : IEnumerator<string>
    {
        Pair<int, int> CurrentPos { get; }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class TokenizerEnumerable
       |
       '-----------------------------------------------------------------------
    */
    public class TokenizerEnumerable : ITokenizerEnumerable
    {
        private Func<ITokenizerEnumerator> mEnumConstructor;

        public TokenizerEnumerable(Func<ITokenizerEnumerator> enumConstructor)
        {
            mEnumConstructor = enumConstructor;
        }

        public ITokenizerEnumerator GetEnumerator()
        {
            return mEnumConstructor();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}