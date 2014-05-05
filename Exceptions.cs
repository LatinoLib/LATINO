/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Exceptions.cs
 *  Desc:    Additional exception classes
 *  Created: Feb-2008
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
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
}
