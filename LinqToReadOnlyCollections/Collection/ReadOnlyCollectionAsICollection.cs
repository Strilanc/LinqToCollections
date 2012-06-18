using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.Collection {
    ///<summary>Implements a ReadOnly ICollection by delegating calls to a readable collection.</summary>
    internal sealed class ReadOnlyCollectionAsICollection<T> : ICollection<T>, IReadOnlyCollection<T> {
        private readonly IReadOnlyCollection<T> collection;
        
        public ReadOnlyCollectionAsICollection(IReadOnlyCollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            this.collection = collection;
        }

        public int Count { get { return collection.Count; } }
        public IEnumerator<T> GetEnumerator() { return collection.GetEnumerator(); }

        public bool IsReadOnly { get { return true; } }
        public bool Contains(T item) {
            return collection.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex + Count > array.Length) throw new ArgumentOutOfRangeException("arrayIndex");
            using (var e = GetEnumerator()) {
                for (int i = 0; e.MoveNext(); i++)
                    array[i + arrayIndex] = e.Current;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        void ICollection<T>.Add(T item) { throw new NotSupportedException("Collection is read-only."); }
        void ICollection<T>.Clear() { throw new NotSupportedException("Collection is read-only."); }
        bool ICollection<T>.Remove(T item) { throw new NotSupportedException("Collection is read-only."); }
    }
}
