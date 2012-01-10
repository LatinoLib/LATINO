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
 *  License: GNU LGPL (http://www.gnu.org/licenses/lgpl.txt)
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