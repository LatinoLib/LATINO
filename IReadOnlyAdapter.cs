/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IReadOnlyAdapter.cs
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
       |  Interface IReadOnlyAdapter
       |
       '-----------------------------------------------------------------------
    */
    public interface IReadOnlyAdapter
    {
        object GetWritableCopy();
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IReadOnlyAdapter<T>
       |
       '-----------------------------------------------------------------------
    */
    public interface IReadOnlyAdapter<T> : IReadOnlyAdapter
    {
        new T GetWritableCopy();
    } 
}