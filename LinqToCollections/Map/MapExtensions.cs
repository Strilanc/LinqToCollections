using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using LinqToCollections.Set;

namespace LinqToCollections.Map {
    public static class MapExtensions {
        ///<summary>Exposes an IDictionary as a readable map.</summary>
        [Pure()]
        public static IMap<TKey, TValue> AsMap<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            Contract.Requires<ArgumentException>(dictionary != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValue>>() != null);
            return new Map<TKey, TValue>(new Ret<TKey>(e => dictionary.ContainsKey(e), dictionary.Select(e => e.Key)), e => dictionary[e]);
        }

        ///<summary>Creates a readable map by projecting sequence elements into key value pairs.</summary>
        [Pure()]
        public static IMap<TKey, TValue> ToMap<TIn, TKey, TValue>(this IEnumerable<TIn> values, Func<TIn, TKey> keySelector, Func<TIn, TValue> valueSelector) {
            Contract.Requires<ArgumentException>(values != null);
            Contract.Requires<ArgumentException>(keySelector != null);
            Contract.Requires<ArgumentException>(valueSelector != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValue>>() != null);
            return values.ToDictionary(keySelector, valueSelector).AsMap();
        }
        ///<summary>Creates a readable map based on key value pairs.</summary>
        [Pure()]
        public static IMap<TKey, TValue> ToMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> values) {
            Contract.Requires<ArgumentException>(values != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValue>>() != null);
            return values.ToMap(e => e.Key, e => e.Value);
        }
        ///<summary>Creates a readable map based on key value pairs.</summary>
        [Pure()]
        public static IMap<TKey, TValue> ToMap<TKey, TValue>(this IEnumerable<IKeyValue<TKey, TValue>> values) {
            Contract.Requires<ArgumentException>(values != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValue>>() != null);
            return values.ToMap(e => e.Key, e => e.Value);
        }
        ///<summary>Creates a readable map by projecting keys to values.</summary>
        [Pure()]
        public static IMap<TKey, TValue> MappedTo<TKey, TValue>(this IEnumerable<TKey> keys, Func<TKey, TValue> valueSelector) {
            Contract.Requires<ArgumentException>(keys != null);
            Contract.Requires<ArgumentException>(valueSelector != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValue>>() != null);
            return keys.ToDictionary(e => e, valueSelector).AsMap();
        }
        ///<summary>Creates a readable map by projecting values to keys.</summary>
        [Pure()]
        public static IMap<TKey, TValue> KeyedBy<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector) {
            Contract.Requires<ArgumentException>(values != null);
            Contract.Requires<ArgumentException>(keySelector != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValue>>() != null);
            return values.ToDictionary(keySelector).AsMap();
        }

        ///<summary>Determines if two maps contain the same keys mapped to the same values.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "False positive")]
        public static bool MapEquals<TKey, TValue>(this IMap<TKey, TValue> map1, IMap<TKey, TValue> map2, IEqualityComparer<TValue> comparer = null) {
            Contract.Requires<ArgumentException>(map1 != null);
            Contract.Requires<ArgumentException>(map2 != null);
            comparer = comparer ?? EqualityComparer<TValue>.Default;
            if (!map1.Keys.SetEquals(map2.Keys)) return false;
            foreach (var e in map1.Keys) {
                Contract.Assume(e != null);
                Contract.Assume(map1.Keys.Contains(e));
                Contract.Assume(map2.Keys.Contains(e));
                if (!comparer.Equals(map1[e], map2[e])) return false;
            }
            return true;
        }

        ///<summary>Iterates the values in a readable map.</summary>
        [Pure()]
        public static IEnumerable<TValue> Values<TKey, TValue>(this IMap<TKey, TValue> map) {
            Contract.Requires<ArgumentException>(map != null);
            Contract.Ensures(Contract.Result<IEnumerable<TValue>>() != null);
            return map.AsEnumerable().Select(e => e.Value);
        }

        ///<summary>Creates a readable map with values derived from the given readable map by the given projection function.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive")]
        public static IMap<TKey, TOut> Select<TKey, TIn, TOut>(this IMap<TKey, TIn> map, Func<TIn, TOut> projection) {
            Contract.Requires<ArgumentException>(map != null);
            Contract.Requires<ArgumentException>(projection != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TOut>>() != null);
            return new Map<TKey, TOut>(map.Keys, e => projection(map[e]));
        }

        ///<summary>Creates a readable map with entries from the given readable map whose keys match a filter.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive")]
        public static IMap<TKey, TValue> Where<TKey, TValue>(this IMap<TKey, TValue> map, Func<TKey, bool> keyFilter) {
            Contract.Requires<ArgumentException>(map != null);
            Contract.Requires<ArgumentException>(keyFilter != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValue>>() != null);
            return new Map<TKey, TValue>(map.Keys.Where(keyFilter), e => map[e]);
        }
        ///<summary>Creates a readable map with entries from the given readable map whose values match a filter.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive")]
        public static IMap<TKey, TValue> WhereValue<TKey, TValue>(this IMap<TKey, TValue> map, Func<TValue, bool> valueFilter) {
            Contract.Requires<ArgumentException>(map != null);
            Contract.Requires<ArgumentException>(valueFilter != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValue>>() != null);
            return new Map<TKey, TValue>(map.Keys.Where(e => valueFilter(map[e])), e => map[e]);
        }
        ///<summary>Combines two readable maps by mapping keys present in both to a projection of the values mapped to by the keys.</summary>
        [Pure()]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive")]
        public static IMap<TKey, TValueOut> Zip<TKey, TValueIn1, TValueIn2, TValueOut>(this IMap<TKey, TValueIn1> map1, IMap<TKey, TValueIn2> map2, Func<TValueIn1, TValueIn2, TValueOut> projection) {
            Contract.Requires<ArgumentException>(map1 != null);
            Contract.Requires<ArgumentException>(map2 != null);
            Contract.Requires<ArgumentException>(projection != null);
            Contract.Ensures(Contract.Result<IMap<TKey, TValueOut>>() != null);
            return new Map<TKey, TValueOut>(map1.Keys.Intersect(map2.Keys), e => projection(map1[e], map2[e]));
        }
    }
}
