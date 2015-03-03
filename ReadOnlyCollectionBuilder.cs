using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Latino
{
    public class ReadOnlyCollectionBuilder<T> : ReadOnlyCollection<T>
    {
        public ReadOnlyCollectionBuilder(params T[] items) : base(items)
        {
        }

        public ReadOnlyCollectionBuilder(params IEnumerable<T>[] items) : base(items.SelectMany(list => list).ToList())
        {
        }

        public ReadOnlyCollectionBuilder(params ReadOnlyCollectionBuilder<T>[] colls) : base(colls.SelectMany(b => b).ToList())
        {
        }
    }

    public class Strings : ReadOnlyCollectionBuilder<string>
    {
        public Strings(params string[] items) : base(items)
        {
        }

        public Strings(params IEnumerable<string>[] items) : base(items)
        {
        }

        public Strings(params ReadOnlyCollectionBuilder<string>[] colls) : base(colls)
        {
        }
    }
}