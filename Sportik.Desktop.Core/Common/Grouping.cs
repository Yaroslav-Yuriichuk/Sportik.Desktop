using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sportik.Desktop.Core.Common
{
    public sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> _values;

        public Grouping(TKey key, IEnumerable<TElement> values)
        {
            Key = key;
            _values = values;
        }

        public TKey Key { get; }

        public IEnumerator<TElement> GetEnumerator() => _values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}