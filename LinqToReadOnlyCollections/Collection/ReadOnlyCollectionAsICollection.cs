using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToCollections.Collection {
    ///<summary>Implements a ReadOnly ICollection by delegating calls to a readable _collection.</summary>
    internal sealed class ReadOnlyCollectionAsICollection<T> : ICollection<T>, IReadOnlyCollection<T> {
        private readonly IReadOnlyCollection<T> _collection;
        
        public ReadOnlyCollectionAsICollection(IReadOnlyCollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            this._collection = collection;
        }

        public int Count { get { return this._collection.Count; } }
        public IEnumerator<T> GetEnumerator() { return this._collection.GetEnumerator(); }

        public bool IsReadOnly { get { return true; } }
        public bool Contains(T item) {
            return this._collection.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex + Count > array.Length) throw new ArgumentOutOfRangeException("arrayIndex");
            using (var e = GetEnumerator()) {
                for (var i = 0; e.MoveNext(); i++)
                    array[i + arrayIndex] = e.Current;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        void ICollection<T>.Add(T item) { throw new NotSupportedException("Collection is read-only."); }
        void ICollection<T>.Clear() { throw new NotSupportedException("Collection is read-only."); }
        bool ICollection<T>.Remove(T item) { throw new NotSupportedException("Collection is read-only."); }
    }
}
