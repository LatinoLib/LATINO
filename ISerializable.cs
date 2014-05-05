/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ISerializable.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface ISerializable
       |
       '-----------------------------------------------------------------------
    */
    public interface ISerializable
    {
        // *** note that you need to implement the constructor that loads an instance if the class implements ISerializable
        void Save(BinarySerializer writer);
    }
}