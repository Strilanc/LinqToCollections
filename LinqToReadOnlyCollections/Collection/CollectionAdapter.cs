using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.LinqToCollections {
    ///<summary>Exposes a readonly collection as a mutable collection that doesn't support mutation.</summary>
    internal sealed class CollectionAdapter<T> : AbstractReadOnlyCollection<T>, ICollection<T>, IBoundedCount {
        public readonly IReadOnlyCollection<T> Collection;
        public int? MaxCount { get; private set; }
        public int MinCount { get; private set; }
        
        private CollectionAdapter(IReadOnlyCollection<T> collection) {
            this.Collection = collection;
            this.MaxCount = collection.TryGetMaxCount();
            this.MinCount = collection.TryGetMinCount();
        }
        public static IReadOnlyCollection<T> From(IReadOnlyCollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            return new CollectionAdapter<T>(collection);
        }
        public static IReadOnlyCollection<T> Adapt(ICollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");

            if (collection.IsReadOnly && !(collection is T[])) {
                // if it's an adapter, then we can unwrap it
                var c = collection as CollectionAdapter<T>;
                if (c != null) return c.Collection;

                // if it's already what we need, great!
                var r = collection as IReadOnlyCollection<T>;
                if (r != null) return r;
            }

            // use existing readonly adapter
            return new CollectionAdapter<T>(new AnonymousReadOnlyCollection<T>(
                () => collection.Count,
                collection.TryGetMaxCount(),
                collection.TryGetMinCount(),
                collection.GetEnumerator));
        }
        public static ICollection<T> Adapt(IReadOnlyCollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");

            // if it's already a collection, we can just return it
            var r = collection as ICollection<T>;
            if (r != null) return r;

            // otherwise we need to adapt it
            return new CollectionAdapter<T>(collection);
        }

        // delegate methods for IReadOnlyCollection
        public override int Count { get { return Collection.Count; } }
        public override IEnumerator<T> GetEnumerator() { return Collection.GetEnumerator(); }

        // implement non-mutating methods for ICollection
        public bool IsReadOnly { get { return true; } }
        public bool Contains(T item) {
            var eq = EqualityComparer<T>.Default;
            return this.Any(e => eq.Equals(item, e));
        }
        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex + Count > array.Length) throw new ArgumentOutOfRangeException("arrayIndex");
            var i = 0;
            foreach (var e in this) {
                array[i + arrayIndex] = e;
                i += 1;
            }
        }

        // don't support mutating methods for ICollection
        void ICollection<T>.Add(T item) { throw new NotSupportedException("Collection is read-only."); }
        void ICollection<T>.Clear() { throw new NotSupportedException("Collection is read-only."); }
        bool ICollection<T>.Remove(T item) { throw new NotSupportedException("Collection is read-only."); }
    }
}
