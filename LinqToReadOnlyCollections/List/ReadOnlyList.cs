using System.Collections.Generic;
using System.Linq;
using System;

namespace LinqToReadOnlyCollections.List {
    ///<summary>Contains extension methods related to read-only lists.</summary>
    public static class ReadOnlyList {
        ///<summary>
        ///Exposes a list as a read-only list, using a cast if possible (unless the list is not marked read-only).
        ///The result is guaranteed to still implement IList, so that further usage of AsIList and AsReadOnlyList do not cause more wrapping.
        ///</summary>
        public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            return (list.IsReadOnly ? list as IReadOnlyList<T> : null) 
                ?? new AnonymousReadOnlyList<T>(() => list.Count, i => list[i], list);
        }
        ///<summary>Exposes a read-only list as a list, using a cast if possible.</summary>
        public static IList<T> AsIList<T>(this IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            return list as IList<T> ?? new ListCombo<T>(list);
        }

        ///<summary>Exposes the end of a readable list, after skipping up to the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> Skip<T>(this IReadOnlyList<T> list, int maxSkipCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (maxSkipCount < 0) throw new ArgumentOutOfRangeException("maxSkipCount");
            if (maxSkipCount == 0) return list;
            return new ListSkip<T>(list, 0, maxSkipCount, maxSkipCount);
        }
        ///<summary>Exposes the start of a readable list, before skipping down to the given number of items at the end, as a readable list.</summary>
        public static IReadOnlyList<T> SkipLast<T>(this IReadOnlyList<T> list, int maxSkipCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (maxSkipCount < 0) throw new ArgumentOutOfRangeException("maxSkipCount");
            if (maxSkipCount == 0) return list;
            return new ListSkip<T>(list, 0, maxSkipCount, 0);
        }
        ///<summary>Exposes the end of a readable list, after skipping exactly the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> SkipExact<T>(this IReadOnlyList<T> list, int exactSkipCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (exactSkipCount < 0 || exactSkipCount > list.Count) throw new ArgumentOutOfRangeException("exactSkipCount");
            if (exactSkipCount == 0) return list;
            return new ListSkip<T>(list, exactSkipCount, 0, exactSkipCount);
        }
        ///<summary>Exposes the start of a readable list, before skipping exactly the given number of items at the end, as a readable list.</summary>
        public static IReadOnlyList<T> SkipLastExact<T>(this IReadOnlyList<T> list, int exactSkipCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (exactSkipCount < 0 || exactSkipCount > list.Count) throw new ArgumentOutOfRangeException("exactSkipCount");
            if (exactSkipCount == 0) return list;
            return new ListSkip<T>(list, exactSkipCount, 0, 0);
        }

        ///<summary>Exposes the start of a readable list, up to the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> Take<T>(this IReadOnlyList<T> list, int maxTakeCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (maxTakeCount < 0) throw new ArgumentOutOfRangeException("maxTakeCount");
            return new AnonymousReadOnlyList<T>(
                () => Math.Min(maxTakeCount, list.Count),
                i => list[i],
                Enumerable.Take(list, maxTakeCount));
        }
        ///<summary>Exposes the end of a readable list, down to the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> TakeLast<T>(this IReadOnlyList<T> list, int maxTakeCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (maxTakeCount < 0) throw new ArgumentOutOfRangeException("maxTakeCount");
            return new AnonymousReadOnlyList<T>(
                () => Math.Min(maxTakeCount, list.Count),
                i => list[Math.Max(list.Count - maxTakeCount, 0) + i]);
        }
        ///<summary>Exposes the start of a readable list, up to exactly the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> TakeExact<T>(this IReadOnlyList<T> list, int exactTakeCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (exactTakeCount < 0 || exactTakeCount > list.Count) throw new ArgumentOutOfRangeException("exactTakeCount");
            return new AnonymousReadOnlyList<T>(
                () => {
                    if (list.Count < exactTakeCount) throw new InvalidOperationException("Took past end of list.");
                    return exactTakeCount;
                }, i => list[i]);
        }
        ///<summary>Exposes the end of a readable list, down to exactly the given number of items, as a readable list.</summary>
        public static IReadOnlyList<T> TakeLastExact<T>(this IReadOnlyList<T> list, int exactTakeCount) {
            if (list == null) throw new ArgumentNullException("list");
            if (exactTakeCount < 0 || exactTakeCount > list.Count) throw new ArgumentOutOfRangeException("exactTakeCount");
            return new AnonymousReadOnlyList<T>(
                () => {
                    if (list.Count < exactTakeCount) throw new InvalidOperationException("Took past end of list.");
                    return exactTakeCount;
                }, i => list[list.Count - exactTakeCount + i]);
        }

        ///<summary>Projects each element of a readable list into a new form and exposes the results as a readable list.</summary>
        public static IReadOnlyList<TOut> Select<TIn, TOut>(this IReadOnlyList<TIn> list, Func<TIn, TOut> projection) {
            if (list == null) throw new ArgumentNullException("list");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyList<TOut>(
                () => list.Count, 
                i => projection(list[i]),
                Enumerable.Select(list, projection));
        }
        ///<summary>Projects each element of a readable list into a new form by incorporating the element's index and exposes the results as a readable list.</summary>
        public static IReadOnlyList<TOut> Select<TIn, TOut>(this IReadOnlyList<TIn> list, Func<TIn, int, TOut> projection) {
            if (list == null) throw new ArgumentNullException("list");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyList<TOut>(
                () => list.Count,
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
        public static IReadOnlyList<short> Range(this sbyte count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new AnonymousReadOnlyList<short>(counter: () => count, getter: i => (short)i);
        }
        ///<summary>Returns a readable list composed of the bytes less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<byte> Range(this byte count) {
            return new AnonymousReadOnlyList<byte>(counter: () => count, getter: i => (byte)i);
        }
        ///<summary>Returns a readable list composed of the non-negative signed shorts less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<short> Range(this short count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new AnonymousReadOnlyList<short>(counter: () => count, getter: i => (short)i);
        }
        ///<summary>Returns a readable list composed of the unsigned shorts less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<ushort> Range(this ushort count) {
            return new AnonymousReadOnlyList<ushort>(counter: () => count, getter: i => (ushort)i);
        }
        ///<summary>Returns a readable list composed of the non-negative signed integers less than the given count, in increasing order starting at 0.</summary>
        public static IReadOnlyList<int> Range(this int count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new AnonymousReadOnlyList<int>(counter: () => count, getter: i => i);
        }

        ///<summary>Returns a readable list of all the unsigned bytes, from 0 to 255, in increasing order.</summary>
        public static IReadOnlyList<byte> AllBytes() {
            return new AnonymousReadOnlyList<byte>(
                () => 1 << 8,
                i => (byte)i);
        }
        ///<summary>Returns a readable list of all the unsigned shorts, from 0 to 65535, in increasing order.</summary>
        public static IReadOnlyList<ushort> AllUnsigned16BitIntegers() {
            return new AnonymousReadOnlyList<ushort>(
                () => 1 << 16,
                i => (ushort)i);
        }

        ///<summary>Returns a readable list of all the signed bytes, from -128 to 127, in increasing order.</summary>
        public static IReadOnlyList<sbyte> AllSignedBytes() {
            return new AnonymousReadOnlyList<sbyte>(
                () => 1 << 8,
                i => (sbyte)(i + sbyte.MinValue));
        }
        ///<summary>Returns a readable list of all the signed shorts, from -32768 to 32767, in increasing order.</summary>
        public static IReadOnlyList<short> AllSigned16BitIntegers() {
            return new AnonymousReadOnlyList<short>(
                () => 1 << 16,
                i => (short)(i + short.MinValue));
        }

        ///<summary>Returns a readable list with no items.</summary>
        public static IReadOnlyList<T> Empty<T>() {
            return new AnonymousReadOnlyList<T>(() => 0, i => default(T));
        }
        ///<summary>Returns a readable list composed of a value repeated a desired number of times.</summary>
        public static IReadOnlyList<T> Repeated<T>(T value, int count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            if (count == 0) return Empty<T>(); // avoid closing over count or value
            return new AnonymousReadOnlyList<T>(() => count, i => value);
        }
    }
}
