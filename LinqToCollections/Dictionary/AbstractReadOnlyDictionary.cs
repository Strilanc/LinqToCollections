using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Strilanc.LinqToCollections {
    ///<summary>A partially implemented readonly dictionary.</summary>
    public abstract class AbstractReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
        ///<summary>The number of key/value pairs in the dictionary.</summary>
        public abstract int Count { get; }
        ///<summary>The keys in the dictionary.</summary>
        public abstract IEnumerable<TKey> Keys { get; }
        ///<summary>Tries to get the value associated with a key, returning true when successful.</summary>
        public abstract bool TryGetValue(TKey key, out TValue value);

        ///<summary>The values in the dictionary.</summary>
        public virtual IEnumerable<TValue> Values { get { return Keys.Select(e => this[e]); } }
        ///<summary>Determines if a given key is in the dictionary.</summary>
        public virtual bool ContainsKey(TKey key) {
            TValue v;
            return TryGetValue(key, out v);
        }
        ///<summary>Returns the value associated with the given key, throwing an exception if no such key is in the dictionary.</summary>
        public virtual TValue this[TKey key] { 
            get {
                TValue value;
                if (!TryGetValue(key, out value)) throw new ArgumentOutOfRangeException("key", "!ContainsKey(key)");
                return value;
            }
        }
        ///<summary>Enumerates the key/value pairs in the dictionary.</summary>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return Keys.Select(e => new KeyValuePair<TKey, TValue>(e, this[e])).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
