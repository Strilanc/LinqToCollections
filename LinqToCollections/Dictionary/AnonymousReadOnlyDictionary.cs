using System;
using System.Collections.Generic;

namespace Strilanc.LinqToCollections {
    public delegate bool TryGetter<in TKey, TValue>(TKey key, out TValue value);

    ///<summary>A readonly dictionary implemented with delegates passed to its constructor.</summary>
    public sealed class AnonymousReadOnlyDictionary<TKey, TValue> : AbstractReadOnlyDictionary<TKey, TValue> {
        private readonly Func<int> _counter;
        private readonly IEnumerable<TKey> _keys;
        private readonly TryGetter<TKey, TValue> _getter;

        ///<summary>Creates an anonymous dictionary with a collection of keys mapped by a value getter.</summary>
        public AnonymousReadOnlyDictionary(IReadOnlyCollection<TKey> keys, TryGetter<TKey, TValue> getter)
            : this(() => keys.Count, keys, getter) {
        }
        ///<summary>Creates an anonymous dictionary with a fixed number of keys mapped by a value getter.</summary>
        public AnonymousReadOnlyDictionary(int count, IEnumerable<TKey> keys, TryGetter<TKey, TValue> getter) 
            : this(() => count, keys, getter) {
            if (count < 0) throw new ArgumentOutOfRangeException("count", "count < 0");
        }
        ///<summary>Creates an anonymous dictionary based on a counter, keys, and a getter.</summary>
        public AnonymousReadOnlyDictionary(Func<int> counter, IEnumerable<TKey> keys, TryGetter<TKey, TValue> getter) {
            if (counter == null) throw new ArgumentNullException("counter");
            if (keys == null) throw new ArgumentNullException("keys");
            if (getter == null) throw new ArgumentNullException("getter");
            _counter = counter;
            _keys = keys;
            _getter = getter;
        }

        ///<summary>The number of key/value pairs in the dictionary.</summary>
        public override int Count { get { return _counter(); } }
        ///<summary>The keys in the dictionary.</summary>
        public override IEnumerable<TKey> Keys { get { return _keys; } }
        ///<summary>Tries to get the value associated with a key, returning true when successful.</summary>
        public override bool TryGetValue(TKey key, out TValue value) {
            return _getter(key, out value);
        }
    }
}
