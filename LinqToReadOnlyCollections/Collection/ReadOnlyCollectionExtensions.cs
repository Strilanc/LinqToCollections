using System.Collections.Generic;
using System.Linq;
using System;

namespace LinqToReadOnlyCollections.Collection {
    ///<summary>Contains extension methods having to do with the IReadOnlyCollection interface.</summary>
    public static class ReadOnlyCollectionExtensions {
        ///<summary>Exposes a collection as a readable collection.</summary>
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this ICollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            return (collection.IsReadOnly ? collection as IReadOnlyCollection<T> : null)
                ?? new ReadOnlyCollection<T>(counter: () => collection.Count,
                                             iterator: collection.GetEnumerator);
        }
        ///<summary>Exposes a readable collection as an ICollection (readonly).</summary>
        ///<remarks>Using AsReadOnlyCollection on the result will use a cast instead of wrapping more (and AsICollection on that will also cast instead of wrap).</remarks>
        public static ICollection<T> AsICollection<T>(this IReadOnlyCollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            return collection as ICollection<T> 
                ?? new ReadOnlyCollectionAsICollection<T>(collection);
        }
        ///<summary>Creates a copy of the given sequence and exposes the copy as a readable collection.</summary>
        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            return sequence.ToArray();
        }
        ///<summary>Exposes the underlying collection of a given sequence as a readable collection, creating a copy if the underlying type is not a collection.</summary>
        ///<remarks>Just a cast when the sequence is an IReadOnlyCollection, and equivalent to AsReadOnlyCollection(ICollection) when the sequence is an ICollection.</remarks>
        public static IReadOnlyCollection<T> AsElseToReadOnlyCollection<T>(this IEnumerable<T> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");

            var asCollection = sequence as ICollection<T>;
            if (asCollection != null) return asCollection.AsReadOnlyCollection();

            var asRist = sequence as IReadOnlyCollection<T>;
            if (asRist != null) return asRist;
            
            return sequence.ToReadOnlyCollection();
        }

        ///<summary>Exposes the end of a readable collection, after skipping exactly the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> SkipExact<T>(this IReadOnlyCollection<T> collection, int exactSkipCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (exactSkipCount < 0 || exactSkipCount > collection.Count) throw new ArgumentOutOfRangeException("exactSkipCount");
            return new ReadOnlyCollection<T>(
                () => {
                    if (collection.Count < exactSkipCount) throw new InvalidOperationException("Skipped past end of list.");
                    return collection.Count - exactSkipCount;
                }, () => {
                    if (collection.Count < exactSkipCount) throw new InvalidOperationException("Skipped past end of list.");
                    return Enumerable.Skip(collection, exactSkipCount).GetEnumerator();
                });
        }
        ///<summary>Exposes the start of a readable collection, up to exactly the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> TakeExact<T>(this IReadOnlyCollection<T> collection, int exactTakeCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (exactTakeCount < 0 || exactTakeCount > collection.Count) throw new ArgumentOutOfRangeException("exactTakeCount");
            return new ReadOnlyCollection<T>(
                () => {
                    if (collection.Count < exactTakeCount) throw new InvalidOperationException("Took past end of list.");
                    return exactTakeCount;
                }, () => {
                    if (collection.Count < exactTakeCount) throw new InvalidOperationException("Took past end of list.");
                    return Enumerable.Take(collection, exactTakeCount).GetEnumerator();
                });
        }
        ///<summary>Exposes the start of a readable collection, before skipping exactly the given number of items at the end, as a readable collection.</summary>
        public static IReadOnlyCollection<T> SkipLastExact<T>(this IReadOnlyCollection<T> collection, int exactSkipCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (exactSkipCount < 0 || exactSkipCount > collection.Count) throw new ArgumentOutOfRangeException("exactSkipCount");
            return new ReadOnlyCollection<T>(
                () => {
                    if (collection.Count < exactSkipCount) throw new InvalidOperationException("Skipped past end of list.");
                    return collection.Count - exactSkipCount;
                }, () => {
                    if (collection.Count < exactSkipCount) throw new InvalidOperationException("Skipped past end of list.");
                    return Enumerable.Take(collection, collection.Count - exactSkipCount).GetEnumerator();
                });
        }
        ///<summary>Exposes the end of a readable collection, down to exactly the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> TakeLastExact<T>(this IReadOnlyCollection<T> collection, int exactTakeCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (exactTakeCount < 0 || exactTakeCount > collection.Count) throw new ArgumentOutOfRangeException("exactTakeCount");
            return new ReadOnlyCollection<T>(
                () => {
                    if (collection.Count < exactTakeCount) throw new InvalidOperationException("Took past end of list.");
                    return exactTakeCount;
                }, () => {
                    if (collection.Count < exactTakeCount) throw new InvalidOperationException("Took past end of list.");
                    return Enumerable.Skip(collection, collection.Count - exactTakeCount).GetEnumerator();
                });
        }

        ///<summary>Exposes the start of a readable collection, up to the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> Take<T>(this IReadOnlyCollection<T> collection, int maxTakeCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (maxTakeCount < 0) throw new ArgumentOutOfRangeException("maxTakeCount");
            return new ReadOnlyCollection<T>(
                () => Math.Min(collection.Count, maxTakeCount),
                () => Enumerable.Take(collection, maxTakeCount).GetEnumerator());
        }
        ///<summary>Exposes the end of a readable collection, after skipping up to the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> Skip<T>(this IReadOnlyCollection<T> collection, int maxSkipCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (maxSkipCount < 0) throw new ArgumentOutOfRangeException("maxSkipCount");
            return new ReadOnlyCollection<T>(
                () => Math.Max(0, collection.Count - maxSkipCount),
                () => Enumerable.Skip(collection, maxSkipCount).GetEnumerator());
        }
        ///<summary>Exposes the end of a readable collection, down to the given number of items, as a readable collection.</summary>
        public static IReadOnlyCollection<T> TakeLast<T>(this IReadOnlyCollection<T> collection, int maxTakeCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (maxTakeCount < 0) throw new ArgumentOutOfRangeException("maxTakeCount");
            return new ReadOnlyCollection<T>(
                () => Math.Min(collection.Count, maxTakeCount),
                () => Enumerable.Skip(collection, Math.Max(0, collection.Count - maxTakeCount)).GetEnumerator());
        }
        ///<summary>Exposes the start of a readable collection, before skipping down to the given number of items at the end, as a readable collection.</summary>
        public static IReadOnlyCollection<T> SkipLast<T>(this IReadOnlyCollection<T> collection, int maxSkipCount) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (maxSkipCount < 0) throw new ArgumentOutOfRangeException("maxSkipCount");
            return new ReadOnlyCollection<T>(
                () => Math.Max(0, collection.Count - maxSkipCount),
                () => Enumerable.Take(collection, Math.Max(0, collection.Count - maxSkipCount)).GetEnumerator());
        }

        ///<summary>Projects each element of a readable collection into a new form and exposes the results as a readable collection.</summary>
        public static IReadOnlyCollection<TOut> Select<TIn, TOut>(this IReadOnlyCollection<TIn> collection, Func<TIn, TOut> projection) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (projection == null) throw new ArgumentNullException("projection");
            return new ReadOnlyCollection<TOut>(counter: () => collection.Count,
                                                iterator: Enumerable.Select(collection, projection).GetEnumerator);
        }
        ///<summary>Projects each element of a readable collection into a new form by incorporating the element's index and exposes the results as a readable collection.</summary>
        public static IReadOnlyCollection<TOut> Select<TIn, TOut>(this IReadOnlyCollection<TIn> collection, Func<TIn, int, TOut> projection) {
            if (collection == null) throw new ArgumentNullException("collection");
            if (projection == null) throw new ArgumentNullException("projection");
            return new ReadOnlyCollection<TOut>(counter: () => collection.Count, 
                                                iterator: Enumerable.Select(collection, projection).GetEnumerator);
        }
        ///<summary>Merges two readable collections using the specified projection and exposes the results as a readable collection.</summary>
        public static IReadOnlyCollection<TOut> Zip<TIn1, TIn2, TOut>(this IReadOnlyCollection<TIn1> collection1, IReadOnlyCollection<TIn2> collection2, Func<TIn1, TIn2, TOut> projection) {
            if (collection1 == null) throw new ArgumentNullException("collection1");
            if (collection2 == null) throw new ArgumentNullException("collection2");
            if (projection == null) throw new ArgumentNullException("projection");
            return new ReadOnlyCollection<TOut>(counter: () => Math.Min(collection1.Count, collection2.Count), 
                                                iterator: Enumerable.Zip(collection1, collection2, projection).GetEnumerator);
        }
        
        ///<summary>Returns a readable collection with the same elements but in the reverse order.</summary>
        public static IReadOnlyCollection<T> Reverse<T>(this IReadOnlyCollection<T> collection) {
            if (collection == null) throw new ArgumentNullException("collection");
            return new ReadOnlyCollection<T>(counter: () => collection.Count,
                                             iterator: Enumerable.Reverse(collection).GetEnumerator);
        }
        ///<summary>Returns the last element in a non-empty readable collection, or a default value if the collection is empty.</summary>
        public static T LastOrDefault<T>(this IReadOnlyCollection<T> collection, T defaultValue = default(T)) {
            if (collection == null) throw new ArgumentNullException("collection");
            return collection.Count == 0 ? defaultValue : collection.Last();
        }
    }
}
