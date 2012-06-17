using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LinqToCollections.List {
    ///<summary>Contains extension methods having to do with the IReadOnlyList interface.</summary>
    public static class ReadOnlyListExtensions {
        ///<summary>Exposes a list as a readable list.</summary>
        [Pure()]
        public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> list) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == list.Count);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().SequenceEqual(list));

            return (list.IsReadOnly ? list as IReadOnlyList<T> : null)
                ?? new ReadOnlyList<T>(getter: i => list[i],
                                       counter: () => list.Count,
                                       efficientIterator: list);
        }
        ///<summary>Exposes a readable list as an IList (readonly).</summary>
        ///<remarks>Using AsReadOnlyList on the result will use a cast instead of wrapping more (and AsIList on that will also cast instead of wrap).</remarks>
        [Pure()]
        public static IList<T> AsIList<T>(this IReadOnlyList<T> list) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Ensures(Contract.Result<IList<T>>() != null);
            Contract.Ensures(Contract.Result<IList<T>>().Count == list.Count);
            Contract.Ensures(Contract.Result<IList<T>>().SequenceEqual(list));
            return list as IList<T> 
                ?? new ReadOnlyListAsIList<T>(list);
        }
        ///<summary>Creates a copy of the given sequence and exposes the copy as a readable list.</summary>
        [Pure()]
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> sequence) {
            Contract.Requires<ArgumentException>(sequence != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().SequenceEqual(sequence));
            return sequence.ToArray();
        }
        ///<summary>Exposes the underlying list of a given sequence as a readable list, creating a copy if the underlying type is not a list.</summary>
        ///<remarks>Just a cast when the sequence is an IReadOnlyList, and equivalent to AsReadOnlyList(IList) when the sequence is an IList.</remarks>
        [Pure()]
        public static IReadOnlyList<T> AsElseToReadOnlyList<T>(this IEnumerable<T> sequence) {
            Contract.Requires<ArgumentException>(sequence != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().SequenceEqual(sequence));

            var asList = sequence as IList<T>;
            if (asList != null) return asList.AsReadOnlyList();

            var asRist = sequence as IReadOnlyList<T>;
            if (asRist != null) return asRist;
            
            return sequence.ToReadOnlyList();
        }

        ///<summary>Exposes a contiguous subset of a readable list as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> SubList<T>(this IReadOnlyList<T> list, int offset, int length) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(offset >= 0);
            Contract.Requires<ArgumentException>(length >= 0);
            Contract.Requires<ArgumentException>(offset + length <= list.Count);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == length);

            //prevent trivial indirection explosion
            var view = list as ListView<T>;
            if (view != null) {
                Contract.Assume(offset + length <= view.Count);
                return view.NestedView(offset, length);
            }

            return new ListView<T>(list, offset, length);
        }

        ///<summary>Exposes the end of a readable list, after skipping exactly the given number of items, as a readable list.</summary>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> SkipExact<T>(this IReadOnlyList<T> list, int exactSkipCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(exactSkipCount >= 0);
            Contract.Requires<ArgumentException>(exactSkipCount <= list.Count);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == list.Count - exactSkipCount);
            return list.SubList(exactSkipCount, list.Count - exactSkipCount);
        }
        ///<summary>Exposes the start of a readable list, up to exactly the given number of items, as a readable list.</summary>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> TakeExact<T>(this IReadOnlyList<T> list, int exactTakeCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(exactTakeCount >= 0);
            Contract.Requires<ArgumentException>(exactTakeCount <= list.Count);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == exactTakeCount);
            return list.SubList(0, exactTakeCount);
        }
        ///<summary>Exposes the start of a readable list, before skipping exactly the given number of items at the end, as a readable list.</summary>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> SkipLastExact<T>(this IReadOnlyList<T> list, int exactSkipCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(exactSkipCount >= 0);
            Contract.Requires<ArgumentException>(exactSkipCount <= list.Count);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == list.Count - exactSkipCount);
            return list.SubList(0, list.Count - exactSkipCount);
        }
        ///<summary>Exposes the end of a readable list, down to exactly the given number of items, as a readable list.</summary>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> TakeLastExact<T>(this IReadOnlyList<T> list, int exactTakeCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(exactTakeCount >= 0);
            Contract.Requires<ArgumentException>(exactTakeCount <= list.Count);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == exactTakeCount);
            return list.SubList(list.Count - exactTakeCount, exactTakeCount);
        }

        ///<summary>Exposes the start of a readable list, up to the given number of items, as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> Take<T>(this IReadOnlyList<T> list, int maxTakeCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(maxTakeCount >= 0);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == Math.Min(list.Count, maxTakeCount));
            return list.TakeExact(Math.Min(list.Count, maxTakeCount));
        }
        ///<summary>Exposes the end of a readable list, after skipping up to the given number of items, as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> Skip<T>(this IReadOnlyList<T> list, int maxSkipCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(maxSkipCount >= 0);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == list.Count - Math.Min(list.Count, maxSkipCount));
            return list.SkipExact(Math.Min(list.Count, maxSkipCount));
        }
        ///<summary>Exposes the end of a readable list, down to the given number of items, as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> TakeLast<T>(this IReadOnlyList<T> list, int maxTakeCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(maxTakeCount >= 0);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == Math.Min(list.Count, maxTakeCount));
            return list.TakeLastExact(Math.Min(list.Count, maxTakeCount));
        }
        ///<summary>Exposes the start of a readable list, before skipping down to the given number of items at the end, as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IReadOnlyList<T> SkipLast<T>(this IReadOnlyList<T> list, int maxSkipCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(maxSkipCount >= 0);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == list.Count - Math.Min(list.Count, maxSkipCount));
            return list.SkipLastExact(Math.Min(list.Count, maxSkipCount));
        }

        ///<summary>Projects each element of a readable list into a new form and exposes the results as a readable list.</summary>
        [Pure()]
        public static IReadOnlyList<TOut> Select<TIn, TOut>(this IReadOnlyList<TIn> list, Func<TIn, TOut> projection) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(projection != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<TOut>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<TOut>>().Count == list.Count);
            return new ReadOnlyList<TOut>(counter: () => list.Count, getter: i => projection(list[i]));
        }
        ///<summary>Projects each element of a readable list into a new form by incorporating the element's index and exposes the results as a readable list.</summary>
        [Pure()]
        public static IReadOnlyList<TOut> Select<TIn, TOut>(this IReadOnlyList<TIn> list, Func<TIn, int, TOut> projection) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(projection != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<TOut>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<TOut>>().Count == list.Count);
            return new ReadOnlyList<TOut>(counter: () => list.Count, getter: i => projection(list[i], i));
        }
        ///<summary>Merges two readable lists using the specified projection and exposes the results as a readable list.</summary>
        [Pure()]
        public static IReadOnlyList<TOut> Zip<TIn1, TIn2, TOut>(this IReadOnlyList<TIn1> list1, IReadOnlyList<TIn2> list2, Func<TIn1, TIn2, TOut> projection) {
            Contract.Requires<ArgumentException>(list1 != null);
            Contract.Requires<ArgumentException>(list2 != null);
            Contract.Requires<ArgumentException>(projection != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<TOut>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<TOut>>().Count == Math.Min(list1.Count, list2.Count));
            return new ReadOnlyList<TOut>(counter: () => Math.Min(list1.Count, list2.Count), getter: i => projection(list1[i], list2[i]));
        }
        
        ///<summary>Returns a readable list with the same elements but in the reverse order.</summary>
        [Pure()]
        public static IReadOnlyList<T> Reverse<T>(this IReadOnlyList<T> list) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == list.Count);
            return new ReadOnlyList<T>(counter: () => list.Count, getter: i => list[list.Count - 1 - i]);
        }

        ///<summary>Returns the last element in a non-empty readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static T Last<T>(this IReadOnlyList<T> list) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(list.Count > 0);
            return list[list.Count - 1];
        }
        ///<summary>Returns the last element in a non-empty readable list, or a default value if the list is empty.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static T LastOrDefault<T>(this IReadOnlyList<T> list, T defaultValue = default(T)) {
            Contract.Requires<ArgumentException>(list != null);
            return list.Count == 0 ? defaultValue : list[list.Count - 1];
        }
    }
}
