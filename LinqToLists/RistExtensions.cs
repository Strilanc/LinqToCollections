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
    }
}
