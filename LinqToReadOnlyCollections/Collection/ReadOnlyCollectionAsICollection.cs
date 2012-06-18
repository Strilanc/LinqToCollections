using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.Collection {
    ///<summary>Implements a ReadOnly ICollection by delegating calls to a readable collection.</summary>
    internal sealed class ReadOnlyCollectionAsICollection<T> : ICollection<T>, IReadOnlyCollection<T> {
        private readonly IReadOnlyCollection<T> collection;
        
        [ContractInvariantMethod]
        void ObjectInvariant() {
            Contract.Invariant(collection != null);
        }

        public ReadOnlyCollectionAsICollection(IReadOnlyCollection<T> collection) {
            Contract.Requires<ArgumentException>(collection != null);
            Contract.Ensures(this.Count == collection.Count);
            Contract.Ensures(this.SequenceEqual(collection));
            this.collection = collection;
            Contract.Assume(this.Count == collection.Count);
            Contract.Assume(this.SequenceEqual(collection));
        }

        public int Count { get { return collection.Count; } }
        public IEnumerator<T> GetEnumerator() { return collection.GetEnumerator(); }

        public bool IsReadOnly { get { return true; } }
        public bool Contains(T item) {
            return collection.Contains(item);
        }
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public void CopyTo(T[] array, int arrayIndex) {
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
