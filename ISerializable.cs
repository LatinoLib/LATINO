/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ISerializable.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Authors: Miha Grcar
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