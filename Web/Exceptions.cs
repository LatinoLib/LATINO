/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Exceptions.cs
 *  Desc:    Additional Web-related exception classes
 *  Created: Nov-2006
 *
 *  Authors: Miha Grcar
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