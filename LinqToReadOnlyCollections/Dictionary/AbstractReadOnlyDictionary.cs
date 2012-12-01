using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.Dictionary {
    ///<summary>A partially implemented readonly dictionary.</summary>
    public abstract class AbstractReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
        public abstract int Count { get; }
        public abstract IEnumerable<TKey> Keys { get; }
        public abstract bool TryGetValue(TKey key, out TValue value);

        public virtual IEnumerable<TValue> Values { get { return Keys.Select(e => this[e]); } }
        public virtual bool ContainsKey(TKey key) {
            TValue v;
            return TryGetValue(key, out v);
        }
        public virtual TValue this[TKey key] { 
            get {
                TValue value;
                if (!TryGetValue(key, out value)) throw new ArgumentOutOfRangeException("key", "!ContainsKey(key)");
                return value;
            }
        }
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return Keys.Select(e => new KeyValuePair<TKey, TValue>(e, this[e])).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
