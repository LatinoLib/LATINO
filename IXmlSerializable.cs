/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IXmlSerializable.cs
 *  Desc:    XML serialization interface 
 *  Created: Nov-2007
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System.Xml;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IXmlSerializable
       |
       '-----------------------------------------------------------------------
    */
    public interface IXmlSerializable
    {
        // *** note that you should implement a constructor that loads an instance if the class implements IXmlSerializable
        void SaveXml(XmlWriter writer);
    }
}