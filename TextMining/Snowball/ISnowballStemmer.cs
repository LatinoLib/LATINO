/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ISnowballStemmer.cs
 *  Desc:    Snowball word stemmer interface
 *  Created: Nov-2009
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace SF.Snowball.Ext
{
    public interface ISnowballStemmer
    {            
        void SetCurrent(string word);
        bool Stem();
        string GetCurrent();
    }
}
