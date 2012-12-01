using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.LinqToCollections {
    ///<summary>Contains extension methods for readonly collections.</summary>
    public static class ReadOnlyCollection {
        ///<summary>Requires that there be a given minimum number of items in a collection, checking whenever it is accessed.</summary>
        internal static IReadOnlyCollection<T> Require<T>(this IReadOnlyCollection<T> collection, int enforcedMinimumCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (enforcedMinimumCount < 0) throw new ArgumentOutOfRangeException("enforcedMinimumCount", "enforcedMinimumCount < 0");
            return CollectionCountCheck<T>.From(collection, enforcedMinimumCount);
        }

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

        ///<summary>Exposes the end of a readable collection, after skipping up to the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> Skip<T>(this IReadOnlyCollection<T> collection, int maxSkipCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (maxSkipCount < 0) throw new ArgumentOutOfRangeException("maxSkipCount");
            return CollectionSkip<T>.From(collection, maxSkipCount, maxSkipCount);
        }
        ///<summary>Exposes the start of a readable collection, before skipping down to the given number of items at the end, as a readable collection.</summary>
        public static IReadOnlyCollection<T> SkipLast<T>(this IReadOnlyCollection<T> collection, int maxSkipCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (maxSkipCount < 0) throw new ArgumentOutOfRangeException("maxSkipCount");
            return CollectionSkip<T>.From(collection, 0, maxSkipCount);
        }
        ///<summary>Exposes the start of a readable collection, up to the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> Take<T>(this IReadOnlyCollection<T> collection, int maxTakeCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (maxTakeCount < 0) throw new ArgumentOutOfRangeException("maxTakeCount");
            return CollectionTakeFirst<T>.From(collection, maxTakeCount);
        }
        ///<summary>Exposes the end of a readable collection, down to the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> TakeLast<T>(this IReadOnlyCollection<T> collection, int maxTakeCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (maxTakeCount < 0) throw new ArgumentOutOfRangeException("maxTakeCount");
            return CollectionTakeLast<T>.From(collection, maxTakeCount);
        }
        
        ///<summary>Exposes the end of a readable collection, after skipping exactly the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> SkipRequire<T>(this IReadOnlyCollection<T> collection, int exactSkipCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (exactSkipCount < 0 || exactSkipCount > collection.Count) throw new ArgumentOutOfRangeException("exactSkipCount");
            return collection.Require(exactSkipCount).Skip(exactSkipCount);
        }
        ///<summary>Exposes the start of a readable collection, before skipping exactly the given number of items at the end, as a readable collection.</summary>
        public static IReadOnlyCollection<T> SkipLastRequire<T>(this IReadOnlyCollection<T> collection, int exactSkipCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (exactSkipCount < 0 || exactSkipCount > collection.Count) throw new ArgumentOutOfRangeException("exactSkipCount");
            return collection.Require(exactSkipCount).SkipLast(exactSkipCount);
        }
        ///<summary>Exposes the start of a readable collection, up to exactly the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> TakeRequire<T>(this IReadOnlyCollection<T> collection, int exactTakeCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (exactTakeCount < 0 || exactTakeCount > collection.Count) throw new ArgumentOutOfRangeException("exactTakeCount");
            return collection.Require(exactTakeCount).Take(exactTakeCount);
        }
        ///<summary>Exposes the end of a readable collection, down to exactly the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> TakeLastRequire<T>(this IReadOnlyCollection<T> collection, int exactTakeCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (exactTakeCount < 0 || exactTakeCount > collection.Count) throw new ArgumentOutOfRangeException("exactTakeCount");
            return collection.Require(exactTakeCount).TakeLast(exactTakeCount);
        }

        ///<summary>Projects each element of a readable collection into a new form and exposes the results as a readable collection.</summary>
        public static IReadOnlyCollection<TOut> Select<TIn, TOut>(this IReadOnlyCollection<TIn> collection, Func<TIn, TOut> projection) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyCollection<TOut>(
                () => collection.Count,
                collection.TryGetMaxCount(),
                collection.TryGetMinCount(),
                Enumerable.Select(collection, projection).GetEnumerator);
        }
        ///<summary>Projects each element of a readable collection into a new form by incorporating the element's index and exposes the results as a readable collection.</summary>
        public static IReadOnlyCollection<TOut> Select<TIn, TOut>(this IReadOnlyCollection<TIn> collection, Func<TIn, int, TOut> projection) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyCollection<TOut>(
                () => collection.Count,
                collection.TryGetMaxCount(),
                collection.TryGetMinCount(),
                Enumerable.Select(collection, projection).GetEnumerator);
        }
        ///<summary>Merges two readable collections using the specified projection and exposes the results as a readable collection.</summary>
        public static IReadOnlyCollection<TOut> Zip<TIn1, TIn2, TOut>(this IReadOnlyCollection<TIn1> collection1,
                                                                IReadOnlyCollection<TIn2> collection2,
                                                                Func<TIn1, TIn2, TOut> projection) {
            if (collection1 == null) throw new ArgumentNullException("collection1");
            if (collection2 == null) throw new ArgumentNullException("collection2");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyCollection<TOut>(
                () => Math.Min(collection1.Count, collection2.Count),
                Enumerable.Zip(collection1, collection2, projection).GetEnumerator);
        }
        
        ///<summary>Returns a readable collection with the same elements but in the reverse order.</summary>
        public static IReadOnlyCollection<T> Reverse<T>(this IReadOnlyCollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            return new AnonymousReadOnlyCollection<T>(
                () => collection.Count,
                collection.TryGetMaxCount(),
                collection.TryGetMinCount(),
                collection.Reverse().GetEnumerator);
        }

        ///<summary>Returns a readable collection with no items.</summary>
        public static IReadOnlyCollection<T> Empty<T>() {
            return ListEmpty<T>.Empty;
        }
        ///<summary>Returns a readable collection composed of a value repeated a desired number of times.</summary>
        public static IReadOnlyCollection<T> Repeat<T>(T value, int count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count", "count < 0");
            if (count == 0) return Empty<T>(); // avoid closing over value
            return new AnonymousReadOnlyCollection<T>(count, Enumerable.Repeat(value, count).GetEnumerator);
        }
    }
}
