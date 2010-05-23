/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ISnowballStemmer.cs
 *  Desc:    Snowball word stemmer interface
 *  Created: Nov-2009
 *
 *  Authors: Miha Grcar
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
