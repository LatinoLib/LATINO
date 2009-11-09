/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          ISnowballStemmer.cs
 *  Version:       1.0
 *  Desc:		   Snowball word stemmer interface
 *  Author:        Miha Grcar
 *  Created on:    Nov-2009
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
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
