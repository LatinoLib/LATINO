using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Latino
{
    public class ReadOnlyCollBuilder<T> : ReadOnlyCollection<T>
    {
        public ReadOnlyCollBuilder(params T[] items) : base(items)
        {
        }

        public ReadOnlyCollBuilder(params IEnumerable<T>[] items) : base(items.SelectMany(list => list).ToList())
        {
        }

        public ReadOnlyCollBuilder(params ReadOnlyCollBuilder<T>[] colls) : base(colls.SelectMany(b => b).ToList())
        {
        }
    }

    public class Strings : ReadOnlyCollBuilder<string>
    {
        public Strings(params string[] items) : base(items)
        {
        }

        public Strings(params IEnumerable<string>[] items) : base(items)
        {
        }

        public Strings(params ReadOnlyCollBuilder<string>[] colls) : base(colls)
        {
        }
    }
}