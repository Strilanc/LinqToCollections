using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.Dictionary {
    public delegate bool TryGetter<in TKey, TValue>(TKey key, out TValue value);

    ///<summary>A readonly dictionary implemented with delegates passed to its constructor.</summary>
    public sealed class AnonymousReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
        private readonly IReadOnlyCollection<TKey> _keys;
        private readonly TryGetter<TKey, TValue> _getter;

        public AnonymousReadOnlyDictionary(IReadOnlyCollection<TKey> keys, TryGetter<TKey, TValue> getter) {
            if (keys == null) throw new ArgumentNullException("keys");
            if (getter == null) throw new ArgumentNullException("getter");
            _keys = keys;
            _getter = getter;
        }

        public int Count { get { return _keys.Count; } }
        public IEnumerable<TKey> Keys { get { return _keys; } }
        public IEnumerable<TValue> Values { get { return _keys.Select(e => this[e]); } }

        public bool ContainsKey(TKey key) {
            TValue v;
            return _getter(key, out v);
        }
        public bool TryGetValue(TKey key, out TValue value) {
            return _getter(key, out value);
        }
        public TValue this[TKey key] { 
            get {
                TValue value;
                if (!_getter(key, out value)) throw new ArgumentOutOfRangeException("key", "!ContainsKey(key)");
                return value;
            }
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _keys.Select(e => new KeyValuePair<TKey, TValue>(e, this[e])).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
