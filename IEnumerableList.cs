/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    IEnumerableList.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System.Collections;
using System.Collections.Generic;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Interface IEnumerableList
       |
       '-----------------------------------------------------------------------
    */
    public interface IEnumerableList : IEnumerable
    {
        object this[int index] { get; }
        int Count { get; }
    }

    /* .-----------------------------------------------------------------------
       |
       |  Interface IEnumerableList<T>
       |
       '-----------------------------------------------------------------------
    */
    public interface IEnumerableList<T> : IEnumerable<T>, IEnumerableList
    {
        new T this[int index] { get; }
    }
}