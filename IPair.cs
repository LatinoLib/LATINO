/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IPair.cs
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