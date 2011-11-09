/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IPair.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IPair<FirstT, SecondT>
       |
       '-----------------------------------------------------------------------
    */
    public interface IPair<FirstT, SecondT>
    {
        FirstT First { get; }
        SecondT Second { get; }
    }
}