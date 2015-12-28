using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Latino
{
    public class ImmutableCollection<T> : ReadOnlyCollection<T>, ISerializable
    {
        public ImmutableCollection(params T[] items) : base(items)
        {
        }

        public ImmutableCollection(params IEnumerable<T>[] items) : base(items.SelectMany(list => list).ToList())
        {
        }

        public ImmutableCollection(BinarySerializer reader) : base(Load(reader))
        {
        }

        public void Save(BinarySerializer writer)
        {
            writer.WriteInt(Count);
            foreach (T item in Items)
            {
                writer.WriteObject(item);
            }
        }

        public static IList<T> Load(BinarySerializer reader)
        {
            int count = reader.ReadInt();
            var items = new List<T>();
            for (int i = 0; i < count; i++)
            {
                items.Add(reader.ReadObject<T>());
            }
            return items;
        }
    }


    public class Strings : ImmutableCollection<string>
    {
        public Strings(params string[] items) : base(items)
        {
        }

        public Strings(params IEnumerable<string>[] items) : base(items)
        {
        }

        public Strings(params ImmutableCollection<string>[] colls) : base(colls)
        {
        }

        public Strings(BinarySerializer reader) : base(reader)
        {
        }

        public static Strings Split(string[] separators, string input)
        {
            return new Strings(input.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
        }

        public static Strings Split(string separator, string input)
        {
            return Split(new[] { separator }, input);
        }
    }
}