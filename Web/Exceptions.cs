/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Exceptions.cs
 *  Desc:    Additional Web-related exception classes
 *  Created: Nov-2006
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Class QuotaExceededException
       |
       '-----------------------------------------------------------------------
    */
    public class QuotaExceededException : Exception
    {
        public QuotaExceededException() : base("Quota exceeded.")
        {
        }
    }
}