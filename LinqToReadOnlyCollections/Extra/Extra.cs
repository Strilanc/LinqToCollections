using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using LinqToCollections.List;
using LinqToCollections.Map;
using LinqToCollections.Set;

namespace LinqToCollections.Extra {
    ///<summary>Contains extension methods which affect non-list types.</summary>
    ///<remarks>Not included in the main namespace to avoid unwanted pollution.</remarks>
    public static class ExtraExtensions {
        ///<summary>Exposes the non-negative integers below the count as a readable list.</summary>
        [Pure()]
        public static IReadOnlyList<int> Range(this int count) {
            Contract.Requires<ArgumentException>(count >= 0);
            Contract.Ensures(Contract.Result<IReadOnlyList<int>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<int>>().Count == count);
            var r = new ReadOnlyList<int>(counter: () => count, getter: i => i);
            Contract.Assume(r.Count == count);
            return r;
        }
        ///<summary>Exposes the non-negative bytes below the count as a readable list.</summary>
        [Pure()]
        public static IReadOnlyList<byte> Range(this byte count) {
            Contract.Ensures(Contract.Result<IReadOnlyList<byte>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<byte>>().Count == count);
            var r = new ReadOnlyList<byte>(counter: () => count, getter: i => (byte)i);
            Contract.Assume(r.Count == count);
            return r;
        }
        ///<summary>Exposes the non-negative signed 16-bit integers below the count as a readable list.</summary>
        [Pure()]
        public static IReadOnlyList<short> Range(this short count) {
            Contract.Requires<ArgumentException>(count >= 0);
            Contract.Ensures(Contract.Result<IReadOnlyList<short>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<short>>().Count == count);
            var r = new ReadOnlyList<short>(counter: () => count, getter: i => (short)i);
            Contract.Assume(r.Count == count);
            return r;
        }

        ///<summary>Returns a readable list composed of a repeated value.</summary>
        [Pure()]
        public static IReadOnlyList<T> Repeated<T>(this T value, int count) {
            Contract.Requires<ArgumentException>(count >= 0);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>().Count == count);
            var r = new ReadOnlyList<T>(counter: () => count, getter: i => value);
            Contract.Assume(r.Count == count);
            return r;
        }

        ///<summary>Creates a key value pair.</summary>
        [Pure()]
        public static IKeyValue<TKey, TValue> KeyValue<TKey, TValue>(this TKey key, TValue value) {
            Contract.Requires<ArgumentException>(key != null);
            Contract.Ensures(Contract.Result<IKeyValue<TKey, TValue>>() != null);
            return new KeyValue<TKey, TValue>(key, value);
        }
    }
}
