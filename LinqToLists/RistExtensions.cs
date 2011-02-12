using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LinqToLists {
    public static class RistExtensions {
        ///<summary>Exposes a list as a readable list.</summary>
        [Pure()]
        public static IRist<T> AsRist<T>(this IList<T> list) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == list.Count);
            Contract.Ensures(Contract.Result<IRist<T>>().SequenceEqual(list));
            var r =  new Rist<T>(getter: i => list[i],
                                 counter: () => list.Count,
                                 efficientIterator: list);
            Contract.Assume(r.Count == list.Count);
            Contract.Assume(r.SequenceEqual(list));
            return r;
        }
        ///<summary>Creates a copy of the given sequence and exposes the copy as a readable list.</summary>
        [Pure()]
        public static IRist<T> ToRist<T>(this IEnumerable<T> sequence) {
            Contract.Requires<ArgumentException>(sequence != null);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().SequenceEqual(sequence));
            var r = sequence.ToArray().AsRist();
            Contract.Assume(r.SequenceEqual(sequence));
            return r;
        }
        ///<summary>Exposes the underlying list of a given sequence as a readable list, creating a copy if the underlying type is not a list.</summary>
        ///<remarks>Just a cast when the sequence is an IRist, and equivalent to AsRist(IList) when the sequence is an IList.</remarks>
        [Pure()]
        public static IRist<T> AsRist<T>(this IEnumerable<T> sequence) {
            Contract.Requires<ArgumentException>(sequence != null);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().SequenceEqual(sequence));
            
            var asRist = sequence as IRist<T>;
            if (asRist != null) {
                Contract.Assume(asRist.SequenceEqual(sequence));
                return asRist;
            }
            
            var asList = sequence as IList<T>;
            if (asList != null) {
                var r = asList.AsRist();
                Contract.Assume(r.SequenceEqual(sequence));
                return r;
            }
            
            return sequence.ToRist();
        }

        ///<summary>Exposes a contiguous subset of a readable list as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> SubList<T>(this IRist<T> list, int offset, int length) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(offset >= 0);
            Contract.Requires<ArgumentException>(length >= 0);
            Contract.Requires<ArgumentException>(offset + length <= list.Count);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == length);

            //prevent trivial indirection explosion
            var view = list as RistView<T>;
            if (view != null) {
                Contract.Assume(offset + length <= view.Count);
                return view.NestedView(offset, length);
            }

            return new RistView<T>(list, offset, length);
        }

        ///<summary>Exposes the end of a readable list, after skipping exactly the given number of items, as a readable list.</summary>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> SkipExact<T>(this IRist<T> list, int exactSkipCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(exactSkipCount >= 0);
            Contract.Requires<ArgumentException>(exactSkipCount <= list.Count);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == list.Count - exactSkipCount);
            return list.SubList(exactSkipCount, list.Count - exactSkipCount);
        }
        ///<summary>Exposes the start of a readable list, up to exactly the given number of items, as a readable list.</summary>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> TakeExact<T>(this IRist<T> list, int exactTakeCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(exactTakeCount >= 0);
            Contract.Requires<ArgumentException>(exactTakeCount <= list.Count);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == exactTakeCount);
            return list.SubList(0, exactTakeCount);
        }
        ///<summary>Exposes the start of a readable list, before skipping exactly the given number of items at the end, as a readable list.</summary>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> SkipLastExact<T>(this IRist<T> list, int exactSkipCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(exactSkipCount >= 0);
            Contract.Requires<ArgumentException>(exactSkipCount <= list.Count);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == list.Count - exactSkipCount);
            return list.SubList(0, list.Count - exactSkipCount);
        }
        ///<summary>Exposes the end of a readable list, down to exactly the given number of items, as a readable list.</summary>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> TakeLastExact<T>(this IRist<T> list, int exactTakeCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(exactTakeCount >= 0);
            Contract.Requires<ArgumentException>(exactTakeCount <= list.Count);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == exactTakeCount);
            return list.SubList(list.Count - exactTakeCount, exactTakeCount);
        }

        ///<summary>Exposes the end of a readable list, after skipping up to the given number of items, as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> Take<T>(this IRist<T> list, int maxTakeCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(maxTakeCount >= 0);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == Math.Min(list.Count, maxTakeCount));
            return list.TakeExact(Math.Min(list.Count, maxTakeCount));
        }
        ///<summary>Exposes the start of a readable list, up to the given number of items, as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> Skip<T>(this IRist<T> list, int maxSkipCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(maxSkipCount >= 0);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == list.Count - Math.Min(list.Count, maxSkipCount));
            return list.SkipExact(Math.Min(list.Count, maxSkipCount));
        }
        ///<summary>Exposes the start of a readable list, before skipping up to the given number of items at the end, as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> TakeLast<T>(this IRist<T> list, int maxTakeCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(maxTakeCount >= 0);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == Math.Min(list.Count, maxTakeCount));
            return list.TakeLastExact(Math.Min(list.Count, maxTakeCount));
        }
        ///<summary>Exposes the end of a readable list, down to the given number of items, as a readable list.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public static IRist<T> SkipLast<T>(this IRist<T> list, int maxSkipCount) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Requires<ArgumentException>(maxSkipCount >= 0);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == list.Count - Math.Min(list.Count, maxSkipCount));
            return list.SkipLastExact(Math.Min(list.Count, maxSkipCount));
        }
    }
}
