using System;
using System.Collections.Generic;

namespace Strilanc.LinqToCollections {
    ///<summary>Contains extension methods for readonly collections.</summary>
    public static class ReadOnlyCollection {
        /// <summary>
        /// Exposes a collection as a read-only collection.
        /// Tries to unwrap the collection, removing previous AsICollection overhead if possible.
        /// Tries to cast the collection, unless the collection is not marked as read-only.
        /// </summary>
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this ICollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            return CollectionAdapter<T>.Adapt(collection);
        }
        ///<summary>Exposes a read-only collection as a collection, using a cast if possible.</summary>
        public static ICollection<T> AsICollection<T>(this IReadOnlyCollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            return CollectionAdapter<T>.Adapt(collection);
        }

        ///<summary>Returns a readable collection with no items.</summary>
        public static IReadOnlyCollection<T> Empty<T>() {
            return ListEmpty<T>.Empty;
        }
    }
}
