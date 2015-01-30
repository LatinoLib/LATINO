/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    DataStructures.cs
 *  Desc:    Tutorial 1: Fundamental data structures
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using Latino;

namespace Tutorial.Case.Data
{
    public class DataStructures : Tutorial<DataStructures>
    {
        public override void Run(string[] args)
        {
            // *** ArrayList ***
            Output.WriteLine("*** ArrayList ***");
            Output.WriteLine();
            // create an ArrayList 
            Output.WriteLine("Create an ArrayList ...");
            ArrayList<int> list = new ArrayList<int>(new int[] { 1, 2, 3 });
            Output.WriteLine(list);
            // add more items
            Output.WriteLine("Add more items ...");
            list.AddRange(new int[] { 6, 5, 4 });
            Output.WriteLine(list);
            // sort descendingly
            Output.WriteLine("Sort descendingly ...");
            list.Sort(DescSort<int>.Instance);
            Output.WriteLine(list);
            // shuffle 
            Output.WriteLine("Shuffle ...");
            list.Shuffle(new Random(1));
            Output.WriteLine(list);
            // convert to array of double
            Output.WriteLine("Convert to array of double ...");
            double[] array = list.ToArray<double>();
            Output.WriteLine(new ArrayList<double>(array));
            // convert to ArrayList of string
            Output.WriteLine("Convert to ArrayList of string ...");
            ArrayList<string> list2 = new ArrayList<string>(list.ToArray<string>());
            Output.WriteLine(list2);
            // get items
            Output.WriteLine("Get items ...");
            Output.WriteLine(list[0]);
            Output.WriteLine(list[1]);
            // set items
            Output.WriteLine("Set items ...");
            list[0] = 3;
            list[1] = 2;
            Output.WriteLine(list);
            // get length
            Output.WriteLine("Get length ...");
            Output.WriteLine(list.Count);
            Output.WriteLine();

            // *** Set ***
            Output.WriteLine("*** Set ***");
            Output.WriteLine();
            // create Set 
            Output.WriteLine("Create Set ...");
            Set<int> set = new Set<int>(new int[] { 1, 2, 3 });
            Output.WriteLine(set);
            // check for items
            Output.WriteLine("Check for items ...");
            Output.WriteLine(set.Contains(1));
            Output.WriteLine(set.Contains(4));
            // add more items (note the duplicate)
            Output.WriteLine("Add more items ...");
            set.AddRange(new int[] { 6, 5, 4, 3 });
            Output.WriteLine(set);
            // remove some items
            Output.WriteLine("Remove some items ...");
            set.RemoveRange(new int[] { 1, 3 });
            set.Remove(5);
            Output.WriteLine(set);
            // create another Set
            Output.WriteLine("Create another Set ...");
            Set<int> set2 = new Set<int>(new int[] { 1, 2, 3, 4, 5 });
            Output.WriteLine(set2);
            // compute union
            Output.WriteLine("Compute union ...");
            Output.WriteLine(Set<int>.Union(set, set2));
            // compute difference
            Output.WriteLine("Compute difference ...");
            Output.WriteLine(Set<int>.Difference(set, set2));
            // compute intersection
            Output.WriteLine("Compute intersection ...");
            Output.WriteLine(Set<int>.Intersection(set, set2));
            // compute Jaccard similarity
            Output.WriteLine("Compute Jaccard similarity ...");
            Output.WriteLine(Set<int>.JaccardSimilarity(set, set2));
            // convert to array
            Output.WriteLine("Convert to array ...");
            int[] array2 = set2.ToArray();
            Output.WriteLine(new ArrayList<int>(array2));
            // convert to Set of string
            Output.WriteLine("Convert to Set of string ...");
            Set<string> set3 = new Set<string>(set2.ToArray<string>());
            Output.WriteLine(set3);
            // get length
            Output.WriteLine("Get length ...");
            Output.WriteLine(set3.Count);
            Output.WriteLine();

            /*
            // *** BinaryVector ***
            Output.WriteLine("*** BinaryVector ***");
            Output.WriteLine();
            // create BinaryVector
            Output.WriteLine("Create BinaryVector ...");
            BinaryVector<char> binVec = new BinaryVector<char>(new char[] { 'a', 'b', 'c' });            
            Output.WriteLine((object) binVec);
            // check for items
            Output.WriteLine("Check for items ...");
            Output.WriteLine((bool) binVec.Contains('a'));
            Output.WriteLine((bool) binVec.Contains('d'));
            // add more items (note the duplicate)
            Output.WriteLine("Add more items ...");
            binVec.AddRange(new char[] { 'f', 'e', 'd', 'c' });
            Output.WriteLine((object) binVec);
            // remove some items
            Output.WriteLine("Remove some items ...");
            binVec.RemoveRange(new char[] { 'a', 'c' });
            binVec.Remove('e');
            Output.WriteLine((object) binVec);
            // convert to array
            Output.WriteLine("Convert to array ...");
            char[] array3 = binVec.ToArray();
            Output.WriteLine(new ArrayList<char>(array3));
            // convert to BinaryVector of string
            Output.WriteLine("Convert to BinaryVector of string ...");
            BinaryVector<string> binVec2 = new BinaryVector<string>(binVec.ToArray<string>());
            Output.WriteLine((object) binVec2);
            // get items
            Output.WriteLine("Get items ...");
            Output.WriteLine((int) binVec2[0]);
            Output.WriteLine((int) binVec2[1]);
            // get length
            Output.WriteLine("Get length ...");
            Output.WriteLine((int) binVec2.Count);            
            Output.WriteLine();
            */

            // *** Pair ***
            Output.WriteLine("*** Pair ***");
            Output.WriteLine();
            // create Pair
            Output.WriteLine("Create Pair ...");
            Pair<int, string> pair = new Pair<int, string>(3, "dogs");
            Output.WriteLine(pair);
            // create another Pair
            Output.WriteLine("Create another Pair ...");
            Pair<int, string> pair2 = new Pair<int, string>(3, "cats");
            Output.WriteLine(pair2);
            // compare 
            Output.WriteLine("Compare ...");
            Output.WriteLine(pair == pair2);
            // make a change
            Output.WriteLine("Make a change ...");
            pair.Second = "cats";
            Output.WriteLine(pair);
            // compare again
            Output.WriteLine("Compare again ...");
            Output.WriteLine(pair == pair2);
            Output.WriteLine();

            // *** KeyDat ***
            Output.WriteLine("*** KeyDat ***");
            Output.WriteLine();
            // create KeyDat
            Output.WriteLine("Create KeyDat ...");
            KeyDat<int, string> keyDat = new KeyDat<int, string>(3, "dogs");
            Output.WriteLine(keyDat);
            // create another KeyDat
            Output.WriteLine("Create another KeyDat ...");
            KeyDat<int, string> keyDat2 = new KeyDat<int, string>(3, "cats");
            Output.WriteLine(keyDat2);
            // compare 
            Output.WriteLine("Compare ...");
            Output.WriteLine(keyDat == keyDat2);
            // make a change
            Output.WriteLine("Make a change ...");
            keyDat.Key = 4;
            Output.WriteLine(keyDat);
            // compare again
            Output.WriteLine("Compare again ...");
            Output.WriteLine(keyDat == keyDat2);
            Output.WriteLine(keyDat > keyDat2);
            Output.WriteLine();

            // *** IdxDat ***
            Output.WriteLine("*** IdxDat ***");
            Output.WriteLine();
            // create an IdxDat
            Output.WriteLine("Create an IdxDat ...");
            IdxDat<string> idxDat = new IdxDat<string>(3, "dogs");
            Output.WriteLine(idxDat);
            // create another IdxDat
            Output.WriteLine("Create another IdxDat ...");
            IdxDat<string> idxDat2 = new IdxDat<string>(4, "cats");
            Output.WriteLine(idxDat2);
            // compare 
            Output.WriteLine("Compare ...");
            Output.WriteLine(idxDat == idxDat2);
            // make a change
            //idxDat.Idx = 4; // not possible to change index
            idxDat.Dat = "cats";
            Output.WriteLine(idxDat);
            // compare again
            Output.WriteLine("Compare again ...");
            Output.WriteLine(idxDat == idxDat2);
            Output.WriteLine(idxDat < idxDat2);
            Output.WriteLine();

            // *** ArrayList of KeyDat ***
            Output.WriteLine("*** ArrayList of KeyDat ***");
            Output.WriteLine();
            // create an ArrayList of KeyDat
            Output.WriteLine("Create an ArrayList of KeyDat ...");
            ArrayList<KeyDat<double, string>> listKeyDat = new ArrayList<KeyDat<double, string>>(new KeyDat<double, string>[] {
                new KeyDat<double, string>(2.4, "cats"),
                new KeyDat<double, string>(3.3, "dogs"),
                new KeyDat<double, string>(4.2, "lizards") });
            Output.WriteLine(listKeyDat);
            // sort descendingly
            Output.WriteLine("Sort descendingly ...");
            listKeyDat.Sort(DescSort<KeyDat<double, string>>.Instance);
            Output.WriteLine(listKeyDat);
            // find item with bisection
            Output.WriteLine("Find item with bisection ...");
            int idx = listKeyDat.BinarySearch(new KeyDat<double, string>(3.3), DescSort<KeyDat<double, string>>.Instance);
            Output.WriteLine(idx);
            idx = listKeyDat.BinarySearch(new KeyDat<double, string>(3), DescSort<KeyDat<double, string>>.Instance);
            Output.WriteLine(~idx);
            // remove item
            Output.WriteLine("Remove item ...");
            listKeyDat.Remove(new KeyDat<double, string>(3.3));
            Output.WriteLine(listKeyDat);
            // get first and last item
            Output.WriteLine("Get first and last item ...");
            Output.WriteLine(listKeyDat.First);
            Output.WriteLine(listKeyDat.Last);
        }
    }
}
