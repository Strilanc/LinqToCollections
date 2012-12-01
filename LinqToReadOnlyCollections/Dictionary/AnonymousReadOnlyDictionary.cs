using System;
using System.Collections.Generic;

namespace Strilanc.LinqToCollections {
    public delegate bool TryGetter<in TKey, TValue>(TKey key, out TValue value);

    ///<summary>A readonly dictionary implemented with delegates passed to its constructor.</summary>
    public sealed class AnonymousReadOnlyDictionary<TKey, TValue> : AbstractReadOnlyDictionary<TKey, TValue> {
        private readonly IReadOnlyCollection<TKey> _keys;
        private readonly TryGetter<TKey, TValue> _getter;

        public AnonymousReadOnlyDictionary(IReadOnlyCollection<TKey> keys, TryGetter<TKey, TValue> getter) {
            if (keys == null) throw new ArgumentNullException("keys");
            if (getter == null) throw new ArgumentNullException("getter");
            _keys = keys;
            _getter = getter;
        }
        public AnonymousReadOnlyDictionary(Func<int> counter, IEnumerable<TKey> keys, TryGetter<TKey, TValue> getter) {
            if (counter == null) throw new ArgumentNullException("counter");
            if (keys == null) throw new ArgumentNullException("keys");
            if (getter == null) throw new ArgumentNullException("getter");
            _keys = new AnonymousReadOnlyCollection<TKey>(counter, keys.GetEnumerator);
            _getter = getter;
        }

        public override int Count { get { return _keys.Count; } }
        public override IEnumerable<TKey> Keys { get { return _keys; } }
        public override bool TryGetValue(TKey key, out TValue value) {
            return _getter(key, out value);
        }
    }
}
