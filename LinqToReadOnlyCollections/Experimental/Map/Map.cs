using System;
using System.Collections.Generic;
using System.Linq;
using LinqToReadOnlyCollections.Experimental.Set;
using System.Diagnostics.CodeAnalysis;

namespace LinqToReadOnlyCollections.Experimental.Map {
    ///<summary>Utility class for implementing a readable map via a _keys set and _getter delegate.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "'Map' is better than 'Collection'")]
    public class Map<TKey, TValue> : IMap<TKey, TValue> {
        private readonly IRet<TKey> _keys;
        private readonly Func<TKey, TValue> _getter;
        
        ///<summary>Constructs a readable map implementation.</summary>
        ///<param name="keys">The set of _keys mapped to values.</param>
        ///<param name="getter">Retrieves the value mapped to by the given key.</param>
        public Map(IRet<TKey> keys, Func<TKey, TValue> getter) {
            if (keys == null) throw new ArgumentNullException("keys");
            if (getter == null) throw new ArgumentNullException("getter");
            this._keys = keys;
            this._getter = getter;
        }

        public IRet<TKey> Keys { get { return this._keys; } }
        public TValue this[TKey key] { get { return this._getter(key); } }
        public IEnumerator<IKeyValue<TKey, TValue>> GetEnumerator() { return this._keys.Select(e => new KeyValue<TKey, TValue>(e, this[e])).GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        public override string ToString() {
            const int MaxPreviewItemCount = 10;
            var initialItems = this.Take(MaxPreviewItemCount + 1).ToArray();
            var suffix = initialItems.Length == MaxPreviewItemCount + 1 ? ", ..." : "}";
            return "{" + String.Join(", ", initialItems.Take(MaxPreviewItemCount)) + suffix;
        }
    }
}
