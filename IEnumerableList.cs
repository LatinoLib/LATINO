/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    IEnumerableList.cs
 *  Desc:    Interface definition
 *  Created: Nov-2007
 *
 *  Authors: Miha Grcar
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