/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Tutorial1.cs
 *  Desc:    Tutorial 1: Fundamental data structures
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using Latino;

namespace Latino.Tutorial
{
    class Tutorial1
    {
        static void Main(string[] args)
        {
            // *** ArrayList ***
            Console.WriteLine("*** ArrayList ***");
            Console.WriteLine();
            // create an ArrayList 
            Console.WriteLine("Create an ArrayList ...");
            ArrayList<int> list = new ArrayList<int>(new int[] { 1, 2, 3 });            
            Console.WriteLine(list);
            // add more items
            Console.WriteLine("Add more items ...");
            list.AddRange(new int[] { 6, 5, 4 });
            Console.WriteLine(list);
            // sort descendingly
            Console.WriteLine("Sort descendingly ...");
            list.Sort(DescSort<int>.Instance);
            Console.WriteLine(list);
            // shuffle 
            Console.WriteLine("Shuffle ...");
            list.Shuffle(new Random(1));
            Console.WriteLine(list);
            // convert to array of double
            Console.WriteLine("Convert to array of double ...");
            double[] array = list.ToArray<double>();
            Console.WriteLine(new ArrayList<double>(array));
            // convert to ArrayList of string
            Console.WriteLine("Convert to ArrayList of string ...");
            ArrayList<string> list2 = new ArrayList<string>(list.ToArray<string>());
            Console.WriteLine(list2);
            // get items
            Console.WriteLine("Get items ...");
            Console.WriteLine(list[0]);
            Console.WriteLine(list[1]);
            // set items
            Console.WriteLine("Set items ...");
            list[0] = 3;
            list[1] = 2;
            Console.WriteLine(list);
            // get length
            Console.WriteLine("Get length ...");
            Console.WriteLine(list.Count);
            Console.WriteLine();

            // *** Set ***
            Console.WriteLine("*** Set ***");
            Console.WriteLine();
            // create Set 
            Console.WriteLine("Create Set ...");
            Set<int> set = new Set<int>(new int[] { 1, 2, 3 });
            Console.WriteLine(set);
            // check for items
            Console.WriteLine("Check for items ...");
            Console.WriteLine(set.Contains(1));
            Console.WriteLine(set.Contains(4));
            // add more items (note the duplicate)
            Console.WriteLine("Add more items ...");
            set.AddRange(new int[] { 6, 5, 4, 3 });
            Console.WriteLine(set);
            // remove some items
            Console.WriteLine("Remove some items ...");
            set.RemoveRange(new int[] { 1, 3 });
            set.Remove(5);
            Console.WriteLine(set);
            // create another Set
            Console.WriteLine("Create another Set ...");
            Set<int> set2 = new Set<int>(new int[] { 1, 2, 3, 4, 5 });
            Console.WriteLine(set2);
            // compute union
            Console.WriteLine("Compute union ...");
            Console.WriteLine(Set<int>.Union(set, set2));
            // compute difference
            Console.WriteLine("Compute difference ...");
            Console.WriteLine(Set<int>.Difference(set, set2));
            // compute intersection
            Console.WriteLine("Compute intersection ...");
            Console.WriteLine(Set<int>.Intersection(set, set2));
            // compute Jaccard similarity
            Console.WriteLine("Compute Jaccard similarity ...");
            Console.WriteLine(Set<int>.JaccardSimilarity(set, set2));
            // convert to array
            Console.WriteLine("Convert to array ...");
            int[] array2 = set2.ToArray();
            Console.WriteLine(new ArrayList<int>(array2)); 
            // convert to Set of string
            Console.WriteLine("Convert to Set of string ...");
            Set<string> set3 = new Set<string>(set2.ToArray<string>());
            Console.WriteLine(set3);
            // get length
            Console.WriteLine("Get length ...");
            Console.WriteLine(set3.Count);
            Console.WriteLine();
            
            /*
            // *** BinaryVector ***
            Console.WriteLine("*** BinaryVector ***");
            Console.WriteLine();
            // create BinaryVector
            Console.WriteLine("Create BinaryVector ...");
            BinaryVector<char> binVec = new BinaryVector<char>(new char[] { 'a', 'b', 'c' });            
            Console.WriteLine((object) binVec);
            // check for items
            Console.WriteLine("Check for items ...");
            Console.WriteLine((bool) binVec.Contains('a'));
            Console.WriteLine((bool) binVec.Contains('d'));
            // add more items (note the duplicate)
            Console.WriteLine("Add more items ...");
            binVec.AddRange(new char[] { 'f', 'e', 'd', 'c' });
            Console.WriteLine((object) binVec);
            // remove some items
            Console.WriteLine("Remove some items ...");
            binVec.RemoveRange(new char[] { 'a', 'c' });
            binVec.Remove('e');
            Console.WriteLine((object) binVec);
            // convert to array
            Console.WriteLine("Convert to array ...");
            char[] array3 = binVec.ToArray();
            Console.WriteLine(new ArrayList<char>(array3));
            // convert to BinaryVector of string
            Console.WriteLine("Convert to BinaryVector of string ...");
            BinaryVector<string> binVec2 = new BinaryVector<string>(binVec.ToArray<string>());
            Console.WriteLine((object) binVec2);
            // get items
            Console.WriteLine("Get items ...");
            Console.WriteLine((int) binVec2[0]);
            Console.WriteLine((int) binVec2[1]);
            // get length
            Console.WriteLine("Get length ...");
            Console.WriteLine((int) binVec2.Count);            
            Console.WriteLine();
            */

            // *** Pair ***
            Console.WriteLine("*** Pair ***");
            Console.WriteLine();
            // create Pair
            Console.WriteLine("Create Pair ...");
            Pair<int, string> pair = new Pair<int, string>(3, "dogs");
            Console.WriteLine(pair);
            // create another Pair
            Console.WriteLine("Create another Pair ...");
            Pair<int, string> pair2 = new Pair<int, string>(3, "cats");
            Console.WriteLine(pair2);
            // compare 
            Console.WriteLine("Compare ...");
            Console.WriteLine(pair == pair2);
            // make a change
            Console.WriteLine("Make a change ...");
            pair.Second = "cats";
            Console.WriteLine(pair);
            // compare again
            Console.WriteLine("Compare again ...");
            Console.WriteLine(pair == pair2);
            Console.WriteLine();

            // *** KeyDat ***
            Console.WriteLine("*** KeyDat ***");
            Console.WriteLine();
            // create KeyDat
            Console.WriteLine("Create KeyDat ...");
            KeyDat<int, string> keyDat = new KeyDat<int, string>(3, "dogs");
            Console.WriteLine(keyDat);
            // create another KeyDat
            Console.WriteLine("Create another KeyDat ...");
            KeyDat<int, string> keyDat2 = new KeyDat<int, string>(3, "cats");
            Console.WriteLine(keyDat2);
            // compare 
            Console.WriteLine("Compare ...");
            Console.WriteLine(keyDat == keyDat2);
            // make a change
            Console.WriteLine("Make a change ...");
            keyDat.Key = 4;
            Console.WriteLine(keyDat);
            // compare again
            Console.WriteLine("Compare again ...");
            Console.WriteLine(keyDat == keyDat2);
            Console.WriteLine(keyDat > keyDat2);
            Console.WriteLine();

            // *** IdxDat ***
            Console.WriteLine("*** IdxDat ***");
            Console.WriteLine();
            // create an IdxDat
            Console.WriteLine("Create an IdxDat ...");
            IdxDat<string> idxDat = new IdxDat<string>(3, "dogs");
            Console.WriteLine(idxDat);
            // create another IdxDat
            Console.WriteLine("Create another IdxDat ...");
            IdxDat<string> idxDat2 = new IdxDat<string>(4, "cats");
            Console.WriteLine(idxDat2);
            // compare 
            Console.WriteLine("Compare ...");
            Console.WriteLine(idxDat == idxDat2);
            // make a change
            //idxDat.Idx = 4; // not possible to change index
            idxDat.Dat = "cats";
            Console.WriteLine(idxDat);
            // compare again
            Console.WriteLine("Compare again ...");
            Console.WriteLine(idxDat == idxDat2);
            Console.WriteLine(idxDat < idxDat2);
            Console.WriteLine();

            // *** ArrayList of KeyDat ***
            Console.WriteLine("*** ArrayList of KeyDat ***");
            Console.WriteLine();
            // create an ArrayList of KeyDat
            Console.WriteLine("Create an ArrayList of KeyDat ...");
            ArrayList<KeyDat<double, string>> listKeyDat = new ArrayList<KeyDat<double, string>>(new KeyDat<double, string>[] {
                new KeyDat<double, string>(2.4, "cats"),
                new KeyDat<double, string>(3.3, "dogs"),
                new KeyDat<double, string>(4.2, "lizards") });
            Console.WriteLine(listKeyDat);
            // sort descendingly
            Console.WriteLine("Sort descendingly ...");
            listKeyDat.Sort(DescSort<KeyDat<double, string>>.Instance);
            Console.WriteLine(listKeyDat);
            // find item with bisection
            Console.WriteLine("Find item with bisection ...");
            int idx = listKeyDat.BinarySearch(new KeyDat<double, string>(3.3), DescSort<KeyDat<double, string>>.Instance);
            Console.WriteLine(idx);
            idx = listKeyDat.BinarySearch(new KeyDat<double, string>(3), DescSort<KeyDat<double, string>>.Instance);
            Console.WriteLine(~idx);
            // remove item
            Console.WriteLine("Remove item ...");
            listKeyDat.Remove(new KeyDat<double, string>(3.3));
            Console.WriteLine(listKeyDat);
            // get first and last item
            Console.WriteLine("Get first and last item ...");
            Console.WriteLine(listKeyDat.First);
            Console.WriteLine(listKeyDat.Last);
        }
    }
}
