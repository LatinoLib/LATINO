/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Exceptions.cs
 *  Version:       1.0
 *  Desc:		   Additional exception classes
 *  Author:		   Miha Grcar
 *  Created on:    Feb-2008
 *  Last modified: May-2008
 *  Revision:      Oct-2009
 *
 ***************************************************************************/

using System;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class ArgumentNotSupportedException
       |
       '-----------------------------------------------------------------------
    */
    public class ArgumentNotSupportedException : ArgumentException
    {
        public ArgumentNotSupportedException(string paramName) : base("The argument is not supported.", paramName)
        {
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class ArgumentTypeException
       |
       '-----------------------------------------------------------------------
    */
    public class ArgumentTypeException : ArgumentException
    {
        public ArgumentTypeException(string paramName) : base("The argument is not of one of the expected types.", paramName)
        {
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class ArgumentValueException
       |
       '-----------------------------------------------------------------------
    */
    public class ArgumentValueException : ArgumentException
    {
        public ArgumentValueException(string paramName) : base("The argument value or state is not valid.", paramName)
        {
        }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class XmlFormatException
       |
       '-----------------------------------------------------------------------
    */
    public class XmlFormatException : Exception
    {
        public XmlFormatException() : base("The XML document is not in the expected format.")
        {
        }
    }
}
