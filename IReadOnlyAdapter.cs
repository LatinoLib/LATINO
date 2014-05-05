/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IReadOnlyAdapter.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
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
        object Inner { get; }
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
        new T Inner { get; }
    } 
}