using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToLists.Extra {
    ///<summary>Contains extension methods which affect non-list types.</summary>
    ///<remarks>Not included in the main namespace to avoid unwanted pollution.</remarks>
    public static class ExtraExtensions {
        ///<summary>Exposes the non-negative integers below the count as a readable list.</summary>
        [Pure()]
        public static IRist<int> Range(this int count) {
            Contract.Requires<ArgumentException>(count >= 0);
            Contract.Ensures(Contract.Result<IRist<int>>() != null);
            Contract.Ensures(Contract.Result<IRist<int>>().Count == count);
            var r = new Rist<int>(counter: () => count, getter: i => i);
            Contract.Assume(r.Count == count);
            return r;
        }
        ///<summary>Exposes the non-negative bytes below the count as a readable list.</summary>
        [Pure()]
        public static IRist<byte> Range(this byte count) {
            Contract.Ensures(Contract.Result<IRist<byte>>() != null);
            Contract.Ensures(Contract.Result<IRist<byte>>().Count == count);
            var r = new Rist<byte>(counter: () => count, getter: i => (byte)i);
            Contract.Assume(r.Count == count);
            return r;
        }
        ///<summary>Exposes the non-negative signed 16-bit integers below the count as a readable list.</summary>
        [Pure()]
        public static IRist<short> Range(this short count) {
            Contract.Requires<ArgumentException>(count >= 0);
            Contract.Ensures(Contract.Result<IRist<short>>() != null);
            Contract.Ensures(Contract.Result<IRist<short>>().Count == count);
            var r = new Rist<short>(counter: () => count, getter: i => (short)i);
            Contract.Assume(r.Count == count);
            return r;
        }

        ///<summary>Returns a readable list composed of a repeated value.</summary>
        [Pure()]
        public static IRist<T> Repeated<T>(this T value, int count) {
            Contract.Requires<ArgumentException>(count >= 0);
            Contract.Ensures(Contract.Result<IRist<T>>() != null);
            Contract.Ensures(Contract.Result<IRist<T>>().Count == count);
            var r = new Rist<T>(counter: () => count, getter: i => value);
            Contract.Assume(r.Count == count);
            return r;
        }
    }
}
