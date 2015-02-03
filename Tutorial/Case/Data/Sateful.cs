/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Sateful.cs
 *  Desc:    Tutorial 3.1: Read-only adapters
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 **********************************************************************/

using System;
using Latino;

namespace Tutorial.Case.Data
{
    public class Sateful : Tutorial<Sateful>
    {
        class StatefulObject
        {
            private ArrayList<int> mState
                = new ArrayList<int>(new int[] { 2, 4, 6, 8 });

            // The user can read the state but is not allowed to modify 
            // it, hence the ReadOnly "modifier".
            public ArrayList<int>.ReadOnly State 
            {
                get { return mState; }
            }

            public void Add(int val)
            {
                mState.Add(val);
            }
        }

        // This function does not change the value of its parameter, 
        // hence the ReadOnly "modifier".
        static int GetMax(ArrayList<int>.ReadOnly list) 
        {
            int max = int.MinValue;
            foreach (int val in list)
            {
                if (val > max) { max = val; }
            }
            return max;
        }

        public override void Run(object[] args)
        {
            // Create an instance of StatefulObject.

            StatefulObject obj = new StatefulObject();

            // Output its state. 

            Output.WriteLine(obj.State);
            // Output: ( 2 4 6 8 )

            // Attempt to modify the state through the State property. 
            // It is not possible. The state is protected against 
            // modifications from the outside world. It is only possible
            // to add an item through the member function Add.

            //obj.State.Add(10); // This is not possible.
            //obj.State.Remove(8); // This is also not possible.
            obj.Add(10); // This is the only way to modify the state.
            Output.WriteLine(obj.State);
            // Output: ( 2 4 6 8 10 )

            // Create a "snapshot" of the state. You will get a copy 
            // that you can then modify. Naturally, the modifications to
            // the copy do not affect the original state.

            ArrayList<int> state = obj.State.GetWritableCopy();
            state.Add(12);
            Output.WriteLine(state);
            // Output: ( 2 4 6 8 10 12 )            
            Output.WriteLine(obj.State);
            // Output: ( 2 4 6 8 10 )

            // Invoke GetMax by passing the state as the parameter. 
            // Note that it is also possible to pass normal, writable 
            // instances as parameters to such functions. This is 
            // demonstrated below.

            Output.WriteLine(GetMax(obj.State));
            // Output: 10
            Output.WriteLine(GetMax(state)); // This is also possible.
            // Output: 12

            // By setting the PUBLIC_INNER conditional compilation 
            // symbol, we are able to modify a read-only instance 
            // through its Inner property. This, however, can cause the 
            // instance to become incoherent and is thus to be avoided. 

            obj.State.Inner.Add(12);
            Output.WriteLine(obj.State);
            // Output: ( 2 4 6 8 10 12 )

            // Create an ArrayList of sets of numbers and populate it.

            ArrayList<Set<int>> array = new ArrayList<Set<int>>(
                new Set<int>[] { 
                    new Set<int>(new int[] { 1, 3, 5 }), 
                    new Set<int>(new int[] { 2, 4, 6 }), 
                    new Set<int>(new int[] { 1, 2, 3 }) });
            Output.WriteLine(array);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )

            // Create a read-only adapter for the created array of sets.

            // The easiest way to do it:
            ArrayList<Set<int>>.ReadOnly readOnlyArray = array;
            // This happens behind the curtains:
            //ArrayList<Set<int>>.ReadOnly readOnlyArray 
            //  = new ArrayList<Set<int>>.ReadOnly(array); 

            Output.WriteLine(readOnlyArray);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )

            // Try to add an empty set through the read-only adapter. It 
            // is not possible. It is possible, however, to change each 
            // of the three contained sets. This means that read-only 
            // adapters are shallow.

            //readOnlyArray.Add(new Set<int>()); // Not possible.
            readOnlyArray[0].Add(7); // Still possible.
            Output.WriteLine(readOnlyArray);
            // Output: ( { 1 3 5 7 } { 2 4 6 } { 1 2 3 } )

            // To create a "deeply" read-only instance, you need to make
            // contained instances read-only as well. 

            ArrayList<Set<int>.ReadOnly>.ReadOnly readOnlyArray2
                = new ArrayList<Set<int>.ReadOnly>(
                    new Set<int>.ReadOnly[] { 
                        new Set<int>(new int[] { 1, 3, 5 }), 
                        new Set<int>(new int[] { 2, 4, 6 }), 
                        new Set<int>(new int[] { 1, 2, 3 }) });
            Output.WriteLine(readOnlyArray2);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )
            //readOnlyArray2.Add(new Set<int>()); // Not possible.
            //readOnlyArray2[0].Add(7); // Also not possible.
        }
    }
}
