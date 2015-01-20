/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Tutorial3_3.cs
 *  Desc:    Tutorial 3.3: Serialization
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 **********************************************************************/

using System;
using System.IO;
using Latino;

namespace Tutorial.Example
{
    // This is how you implement a serializable object.
    class SerializableObject : ISerializable
    {
        public object Tag
            = null;

        public SerializableObject()
        {
        }

        // Each serializable object is required to implement the 
        // following constructor which deserializes an instance.
        public SerializableObject(BinarySerializer reader)
        {
            Tag = reader.ReadValueOrObject<object>();
        }

        // This is the only function exposed through ISerializable. 
        public void Save(BinarySerializer writer)
        {
            writer.WriteValueOrObject(Tag);
        }
    }

    public class Tutorial3_3 : Tutorial<Tutorial3_3>
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

            // Serialize the instance into memory.

            BinarySerializer memSer = new BinarySerializer();
            array.Save(memSer);
            array = null; // Loose the instance.

            // Deserialize the instance.

            memSer.Stream.Position = 0;
            array = new ArrayList<Set<int>>(memSer);
            Console.WriteLine(array);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )

            // Serialize the instance into a file.

            BinarySerializer fileSer = new BinarySerializer("array.bin",
                FileMode.Create);
            array.Save(fileSer);
            fileSer.Close();
            array = null; // Loose the instance.

            // Deserialize the instance from the file.

            fileSer = new BinarySerializer("array.bin", FileMode.Open);
            array = new ArrayList<Set<int>>(fileSer);
            fileSer.Close();
            Console.WriteLine(array);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )

            // Create an instance of SerializableObject and assign the
            // Tag member variable.

            SerializableObject serObj = new SerializableObject();
            serObj.Tag = array;

            // Serialize serObj into memory and deserialize it again. 

            Console.WriteLine(serObj.Tag);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )
            memSer = new BinarySerializer();
            serObj.Save(memSer);
            serObj = null; // Loose the instance.

            memSer.Stream.Position = 0;
            serObj = new SerializableObject(memSer);
            Console.WriteLine(serObj.Tag);
            // Output: ( { 1 3 5 } { 2 4 6 } { 1 2 3 } )
        }
    }
}