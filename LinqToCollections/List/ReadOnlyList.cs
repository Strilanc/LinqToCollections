using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.LinqToCollections {
    ///<summary>Contains extension methods for readonly lists.</summary>
    public static class ReadOnlyList {
        ///<summary>Requires that there be a given minimum number of items in a list, checking whenever it is accessed.</summary>
        internal static IReadOnlyList<T> Require<T>(this IReadOnlyList<T> list, int enforcedMinimumCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (enforcedMinimumCount < 0) throw new ArgumentOutOfRangeException("enforcedMinimumCount", "enforcedMinimumCount < 0");
            return ListCountCheck<T>.From(list, enforcedMinimumCount);
        }

        /// <summary>
        /// Exposes a list as a read-only list.
        /// Tries to unwrap the list, removing previous AsIList overhead if possible.
        /// Tries to cast the list, unless the list is not marked as read-only.
        /// </summary>
        public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            return ListAdapter<T>.Adapt(list);
        }
        ///<summary>Exposes a read-only list as a list, using a cast if possible.</summary>
        public static IList<T> AsIList<T>(this IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            return ListAdapter<T>.Adapt(list);
        }

        ///<summary>Exposes the end of a readable list, after skipping up to the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> Skip<T>(this IReadOnlyList<T> list, int maxSkipCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (maxSkipCount < 0) throw new ArgumentOutOfRangeException("maxSkipCount");
            return ListSkip<T>.From(list, maxSkipCount, maxSkipCount);
        }
        ///<summary>Exposes the start of a readable list, before skipping down to the given number of items at the end, as a readable list.</summary>
        public static IReadOnlyList<T> SkipLast<T>(this IReadOnlyList<T> list, int maxSkipCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (maxSkipCount < 0) throw new ArgumentOutOfRangeException("maxSkipCount");
            return ListSkip<T>.From(list, 0, maxSkipCount);
        }
        ///<summary>Exposes the start of a readable list, up to the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> Take<T>(this IReadOnlyList<T> list, int maxTakeCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (maxTakeCount < 0) throw new ArgumentOutOfRangeException("maxTakeCount");
            return ListTakeFirst<T>.From(list, maxTakeCount);
        }
        ///<summary>Exposes the end of a readable list, down to the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> TakeLast<T>(this IReadOnlyList<T> list, int maxTakeCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (maxTakeCount < 0) throw new ArgumentOutOfRangeException("maxTakeCount");
            return ListTakeLast<T>.From(list, maxTakeCount);
        }
        
        ///<summary>Exposes the end of a readable list, after skipping exactly the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> SkipRequire<T>(this IReadOnlyList<T> list, int exactSkipCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (exactSkipCount < 0) throw new ArgumentOutOfRangeException("exactSkipCount", "exactSkipCount < 0");
            if (exactSkipCount > list.Count) throw new ArgumentOutOfRangeException("exactSkipCount", "exactSkipCount > list.Count");
            return list.Require(exactSkipCount).Skip(exactSkipCount);
        }
        ///<summary>Exposes the start of a readable list, before skipping exactly the given number of items at the end, as a readable list.</summary>
        public static IReadOnlyList<T> SkipLastRequire<T>(this IReadOnlyList<T> list, int exactSkipCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (exactSkipCount < 0) throw new ArgumentOutOfRangeException("exactSkipCount", "exactSkipCount < 0");
            if (exactSkipCount > list.Count) throw new ArgumentOutOfRangeException("exactSkipCount", "exactSkipCount > list.Count");
            return list.Require(exactSkipCount).SkipLast(exactSkipCount);
        }
        ///<summary>Exposes the start of a readable list, up to exactly the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> TakeRequire<T>(this IReadOnlyList<T> list, int exactTakeCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (exactTakeCount < 0) throw new ArgumentOutOfRangeException("exactTakeCount", "exactTakeCount < 0");
            if (exactTakeCount > list.Count) throw new ArgumentOutOfRangeException("exactTakeCount", "exactTakeCount > list.Count");
            return list.Require(exactTakeCount).Take(exactTakeCount);
        }
        ///<summary>Exposes the end of a readable list, down to exactly the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> TakeLastRequire<T>(this IReadOnlyList<T> list, int exactTakeCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (exactTakeCount < 0) throw new ArgumentOutOfRangeException("exactTakeCount", "exactTakeCount < 0");
            if (exactTakeCount > list.Count) throw new ArgumentOutOfRangeException("exactTakeCount", "exactTakeCount > list.Count");
            return list.Require(exactTakeCount).TakeLast(exactTakeCount);
        }

        ///<summary>Projects each element of a readable list into a new form and exposes the results as a readable list.</summary>
        public static IReadOnlyList<TOut> Select<TIn, TOut>(this IReadOnlyList<TIn> list, Func<TIn, TOut> projection) {
            if (list == null) throw new ArgumentNullException("list");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyList<TOut>(
                () => list.Count,
                list.TryGetMaxCount(),
                list.TryGetMinCount(),
                i => projection(list[i]),
                Enumerable.Select(list, projection));
        }
        ///<summary>Projects each element of a readable list into a new form by incorporating the element's index and exposes the results as a readable list.</summary>
        public static IReadOnlyList<TOut> Select<TIn, TOut>(this IReadOnlyList<TIn> list, Func<TIn, int, TOut> projection) {
            if (list == null) throw new ArgumentNullException("list");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyList<TOut>(
                () => list.Count,
                list.TryGetMaxCount(),
                list.TryGetMinCount(),
                i => projection(list[i], i),
                Enumerable.Select(list, projection));
        }
        ///<summary>Merges two readable lists using the specified projection and exposes the results as a readable list.</summary>
        public static IReadOnlyList<TOut> Zip<TIn1, TIn2, TOut>(this IReadOnlyList<TIn1> list1,
                                                                IReadOnlyList<TIn2> list2,
                                                                Func<TIn1, TIn2, TOut> projection) {
            if (list1 == null) throw new ArgumentNullException("list1");
            if (list2 == null) throw new ArgumentNullException("list2");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyList<TOut>(
                () => Math.Min(list1.Count, list2.Count),
                i => projection(list1[i], list2[i]),
                Enumerable.Zip(list1, list2, projection));
        }
        ///<summary>Merges three readable lists using the specified projection and exposes the results as a readable list.</summary>
        public static IReadOnlyList<TOut> Zip<TIn1, TIn2, TIn3, TOut>(this IReadOnlyList<TIn1> list1,
                                                                      IReadOnlyList<TIn2> list2,
                                                                      IReadOnlyList<TIn3> list3,
                                                                      Func<TIn1, TIn2, TIn3, TOut> projection) {
            if (list1 == null) throw new ArgumentNullException("list1");
            if (list2 == null) throw new ArgumentNullException("list2");
            if (list3 == null) throw new ArgumentNullException("list3");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyList<TOut>(
                () => Math.Min(Math.Min(list1.Count, list2.Count), list3.Count),
                i => projection(list1[i], list2[i], list3[i]));
        }
        ///<summary>Merges four readable lists using the specified projection and exposes the results as a readable list.</summary>
        public static IReadOnlyList<TOut> Zip<TIn1, TIn2, TIn3, TIn4, TOut>(this IReadOnlyList<TIn1> list1,
                                                                            IReadOnlyList<TIn2> list2,
                                                                            IReadOnlyList<TIn3> list3,
                                                                            IReadOnlyList<TIn4> list4,
                                                                            Func<TIn1, TIn2, TIn3, TIn4, TOut> projection) {
            if (list1 == null) throw new ArgumentNullException("list1");
            if (list2 == null) throw new ArgumentNullException("list2");
            if (list3 == null) throw new ArgumentNullException("list3");
            if (list4 == null) throw new ArgumentNullException("list4");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyList<TOut>(
                () => Math.Min(Math.Min(Math.Min(list1.Count, list2.Count), list3.Count), list4.Count),
                i => projection(list1[i], list2[i], list3[i], list4[i]));
        }
        
        ///<summary>Returns a readable list with the same elements but in the reverse order.</summary>
        public static IReadOnlyList<T> Reverse<T>(this IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            return new AnonymousReadOnlyList<T>(
                () => list.Count,
                list.TryGetMaxCount(),
                list.TryGetMinCount(),
                i => list[list.Count - 1 - i]);
        }

        ///<summary>Returns the last element in a non-empty readable list.</summary>
        public static T Last<T>(this IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            if (list.Count < 1) throw new ArgumentOutOfRangeException("list", "list.Count < 1");
            return list[list.Count - 1];
        }
        ///<summary>Returns the last element in a non-empty readable list, or a default value if the list is empty.</summary>
        public static T LastOrDefault<T>(this IReadOnlyList<T> list, T defaultValue = default(T)) {
            if (list == null) throw new ArgumentNullException("list");
            return list.Count == 0 ? defaultValue : list[list.Count - 1];
        }

        ///<summary>Returns a readable list composed of the non-negative signed bytes less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<sbyte> Range(this sbyte count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new AnonymousReadOnlyList<sbyte>(count, i => (sbyte)i);
        }
        ///<summary>Returns a readable list composed of the bytes less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<byte> Range(this byte count) {
            return new AnonymousReadOnlyList<byte>(count, i => (byte)i);
        }
        ///<summary>Returns a readable list composed of the non-negative signed shorts less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<short> Range(this short count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new AnonymousReadOnlyList<short>(count, i => (short)i);
        }
        ///<summary>Returns a readable list composed of the unsigned shorts less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<ushort> Range(this ushort count) {
            return new AnonymousReadOnlyList<ushort>(count, i => (ushort)i);
        }
        ///<summary>Returns a readable list composed of the non-negative signed integers less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<int> Range(this int count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new AnonymousReadOnlyList<int>(count, i => i);
        }

        ///<summary>Returns a readable list of all the boolean values (false and true, in that order).</summary>
        public static IReadOnlyList<bool> AllBools() {
            return new[] {false, true};
        }
        ///<summary>Returns a readable list of all the unsigned bytes, from 0 to 255, in increasing order.</summary>
        public static IReadOnlyList<byte> AllBytes() {
            return new AnonymousReadOnlyList<byte>(
                1 << 8,
                i => (byte)i);
        }
        ///<summary>Returns a readable list of all the unsigned shorts, from 0 to 65535, in increasing order.</summary>
        public static IReadOnlyList<ushort> AllUnsigned16BitIntegers() {
            return new AnonymousReadOnlyList<ushort>(
                1 << 16,
                i => (ushort)i);
        }

        ///<summary>Returns a readable list of all the signed bytes, from -128 to 127, in increasing order.</summary>
        public static IReadOnlyList<sbyte> AllSignedBytes() {
            return new AnonymousReadOnlyList<sbyte>(
                1 << 8,
                i => (sbyte)(i + sbyte.MinValue));
        }
        ///<summary>Returns a readable list of all the signed shorts, from -32768 to 32767, in increasing order.</summary>
        public static IReadOnlyList<short> AllSigned16BitIntegers() {
            return new AnonymousReadOnlyList<short>(
                1 << 16,
                i => (short)(i + short.MinValue));
        }

        ///<summary>Returns a readable list with no items.</summary>
        public static IReadOnlyList<T> Empty<T>() {
            return ListEmpty<T>.Empty;
        }
        ///<summary>Returns a readable list with a single item.</summary>
        public static IReadOnlyList<T> Singleton<T>(T item) {
            return Repeat(item, 1);
        }
        ///<summary>Returns a readable list composed of a value repeated a desired number of times.</summary>
        public static IReadOnlyList<T> Repeat<T>(T value, int count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count", "count < 0");
            if (count == 0) return Empty<T>(); // avoid closing over value
            return new AnonymousReadOnlyList<T>(count, i => value);
        }

        ///<summary>Lists the items, from the given list, whose indices are congruent to 0 (modulo the given stride length).</summary>
        public static IReadOnlyList<T> Stride<T>(this IReadOnlyList<T> list, int strideLength) {
            if (list == null) throw new ArgumentNullException("list");
            if (strideLength <= 0) throw new ArgumentOutOfRangeException("strideLength", "strideLength <= 0");
            if (strideLength == 1) return list;
            return new AnonymousReadOnlyList<T>(
                () => (list.Count + strideLength - 1) / strideLength,
                i => list[i * strideLength]);
        }

        /// <summary>
        /// Lists some lists that, when interleaved, results in the given list.
        /// Each returned list contains the items, from the given list, whose indices are congruent (modulo the given count) to the list's index in the returned list of lists.
        /// </summary>
        public static IReadOnlyList<IReadOnlyList<T>> Deinterleave<T>(this IReadOnlyList<T> list, int interleavedCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (interleavedCount <= 0) throw new ArgumentOutOfRangeException("interleavedCount", "interleavedCount <= 0");
            if (interleavedCount == 1) return Singleton(list);
            return new AnonymousReadOnlyList<IReadOnlyList<T>>(
                () => interleavedCount,
                i => list.Skip(i).Stride(interleavedCount));
        }

        /// <summary>
        /// Breaks the list into contiguous groups of items with the given size.
        /// The last group may be smaller than the group size.
        /// </summary>
        public static IReadOnlyList<IReadOnlyList<T>> Partition<T>(this IReadOnlyList<T> list, int groupSize) {
            if (list == null) throw new ArgumentNullException("list");
            if (groupSize <= 0) throw new ArgumentOutOfRangeException("groupSize", "groupSize <= 0");
            return new AnonymousReadOnlyList<IReadOnlyList<T>>(
                () => (list.Count + groupSize - 1) / groupSize,
                i => new AnonymousReadOnlyList<T>(
                    () => i == list.Count / groupSize ? list.Count % groupSize : groupSize,
                    j => list[i * groupSize + j]));
        }
    }
}
