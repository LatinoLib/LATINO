/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    ISerializable.cs
 *  Desc:    Binary serialization interface
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
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
        // *** note that you need to implement a constructor that loads an instance if the class implements ISerializable
        void Save(BinarySerializer writer);
    }
}