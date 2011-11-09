/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IReadOnlyAdapter.cs
 *  Desc:    Read-only adapter interface
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
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