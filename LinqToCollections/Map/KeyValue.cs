using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.Map {
    ///<summary>Basic implementation of a key value pair.</summary>
    public class KeyValue<TKey, TValue> : IKeyValue<TKey, TValue>, IEquatable<IKeyValue<TKey, TValue>> {
        private readonly TKey _key;
        private readonly TValue _value;
        
        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_key != null);
        }

        public KeyValue(TKey key, TValue value) {
            Contract.Requires(key != null);
            this._key = key; 
            this._value = value; 
        }

        public TKey Key { get { return _key; } }
        public TValue Value { get { return _value; } }

        public bool Equals(IKeyValue<TKey, TValue> other) {
            return other != null
                && EqualityComparer<TKey>.Default.Equals(this.Key, other.Key)
                && EqualityComparer<TValue>.Default.Equals(this.Value, other.Value);
        }
        public override string ToString() {
            return _key + ": " + _value;
        }
        public override bool Equals(Object obj) {
            return this.Equals(obj as IKeyValue<TKey, TValue>);
        }
        public override int GetHashCode() {
            return _key.GetHashCode() ^ _value.GetHashCode() * 3;
        }
    }
}
