using System;
using System.Collections;
using System.Collections.Generic;
using LinqToReadOnlyCollections.Collection;

namespace LinqToReadOnlyCollections.Dictionary {
    ///<summary>Implements an IDictionary that is readonly by delegating calls to an IReadOnlyDictionary.</summary>
    internal sealed class ReadOnlyDictionaryAsIDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> {
        private readonly IReadOnlyDictionary<TKey, TValue> _dict;
        
        public ReadOnlyDictionaryAsIDictionary(IReadOnlyDictionary<TKey, TValue> dict) {
            if (dict == null) throw new ArgumentNullException("dict");
            this._dict = dict;
        }

        public int Count { get { return _dict.Count; } }
        public bool ContainsKey(TKey key) { return _dict.ContainsKey(key); }
        public bool TryGetValue(TKey key, out TValue value) { return _dict.TryGetValue(key, out value); }
        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] { get { return _dict[key]; } }
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys { get { return _dict.Keys; } }
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { get { return _dict.Values; } }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return _dict.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _dict.GetEnumerator(); }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            TValue value;
            return _dict.TryGetValue(item.Key, out value)
                && Equals(item.Value, value);
        }
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            if (array == null) throw new ArgumentNullException("array");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex < 0");
            if (arrayIndex + Count > array.Length) throw new ArgumentOutOfRangeException("array", "arrayIndex + Count > array.Length");
            foreach (var e in this) {
                array[arrayIndex] = e;
                arrayIndex += 1;
            }
        }
        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            get {
                return new ReadOnlyCollection<TKey>(
                    () => Count,
                    () => _dict.Keys.GetEnumerator()
                ).AsICollection();
            }
        }
        ICollection<TValue> IDictionary<TKey, TValue>.Values {
            get {
                return new ReadOnlyCollection<TValue>(
                    () => Count,
                    () => _dict.Values.GetEnumerator()
                ).AsICollection();
            }
        }
        public TValue this[TKey key] {
            get { return _dict[key]; }
            set { throw new NotSupportedException("Dictionary is read-only."); }
        }

        public bool IsReadOnly { get { return true; } }
        public bool Remove(TKey key) { throw new NotSupportedException("Dictionary is read-only."); }
        public void Add(KeyValuePair<TKey, TValue> item) { throw new NotSupportedException("Dictionary is read-only."); }
        public void Clear() { throw new NotSupportedException("Dictionary is read-only."); }
        public bool Remove(KeyValuePair<TKey, TValue> item) { throw new NotSupportedException("Dictionary is read-only."); }
        public void Add(TKey key, TValue value) { throw new NotSupportedException("Dictionary is read-only."); }
    }
}
