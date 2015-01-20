/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Tutorial3_2.cs
 *  Desc:    Tutorial 3.2: Cloning
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 **********************************************************************/

using System;
using Latino;

namespace Tutorial.Example
{
    public class Tutorial3_2 : Tutorial<Tutorial3_2>
    {
        public override void Run(string[] args)
        {
            // Create an ArrayList of sets of numbers and populate it.

            ArrayList<Set<int>> array = new ArrayList<Set<int>>(
                new Set<int>[] { 
                    new Set<int>(new int[] { 1, 3, 5 }), 
                    new Set<int>(new int[] { 2, 4, 6 }), 
                    new Set<int>(new int[] { 1, 2, 3 }) });
            Console.WriteLine(array);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )

            // Create a shallow clone. This means that the array is 
            // duplicated but the contained sets are merely referenced. 
            // Modifications to a set from the clone thus reflect in the 
            // original instance as demonstrated below.

            ArrayList<Set<int>> shallowClone = array.Clone();
            Console.WriteLine(shallowClone);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )
            shallowClone.Add(new Set<int>(new int[] { 2, 3, 5 }));
            shallowClone[0].Add(7);
            Console.WriteLine(shallowClone);
            // Output: ( { 1 3 5 7 } { 2 4 6 } { 1 2 3 } { 2 3 5 } )
            Console.WriteLine(array);
            // Output: ( { 1 3 5 7 } { 2 4 6 } { 1 2 3 } )

            // Now create a deep clone. The array and its contents are
            // all duplicated. 

            ArrayList<Set<int>> deepClone = array.DeepClone();
            Console.WriteLine(deepClone);
            // Output: ( { 1 3 5 7 } { 2 4 6 } { 1 2 3 } )
            deepClone.Add(new Set<int>(new int[] { 2, 3, 5 }));
            deepClone[0].Add(9);
            Console.WriteLine(deepClone);
            // Output: ( { 1 3 5 7 9 } { 2 4 6 } { 1 2 3 } { 2 3 5 } )
            Console.WriteLine(array);
            // Output: ( { 1 3 5 7 } { 2 4 6 } { 1 2 3 } )
        }
    }
}