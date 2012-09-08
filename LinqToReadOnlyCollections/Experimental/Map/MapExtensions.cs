using System;
using System.Collections.Generic;
using System.Linq;
using LinqToReadOnlyCollections.Experimental.Set;

namespace LinqToReadOnlyCollections.Experimental.Map {
    public static class MapExtensions {
        ///<summary>Exposes an IDictionary as a readable map.</summary>
        public static IMap<TKey, TValue> AsMap<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            return new Map<TKey, TValue>(new Ret<TKey>(dictionary.ContainsKey, dictionary.Select(e => e.Key)), e => dictionary[e]);
        }

        ///<summary>Creates a readable map by projecting sequence elements into key value pairs.</summary>
        public static IMap<TKey, TValue> ToMap<TIn, TKey, TValue>(this IEnumerable<TIn> values, Func<TIn, TKey> keySelector, Func<TIn, TValue> valueSelector) {
            if (values == null) throw new ArgumentNullException("values");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (valueSelector == null) throw new ArgumentNullException("valueSelector");
            return values.ToDictionary(keySelector, valueSelector).AsMap();
        }
        ///<summary>Creates a readable map based on key value pairs.</summary>
        public static IMap<TKey, TValue> ToMap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> values) {
            if (values == null) throw new ArgumentNullException("values");
            return values.ToMap(e => e.Key, e => e.Value);
        }
        ///<summary>Creates a readable map based on key value pairs.</summary>
        public static IMap<TKey, TValue> ToMap<TKey, TValue>(this IEnumerable<IKeyValue<TKey, TValue>> values) {
            if (values == null) throw new ArgumentNullException("values");
            return values.ToMap(e => e.Key, e => e.Value);
        }
        ///<summary>Creates a readable map by projecting keys to values.</summary>
        public static IMap<TKey, TValue> MappedTo<TKey, TValue>(this IEnumerable<TKey> keys, Func<TKey, TValue> valueSelector) {
            if (keys == null) throw new ArgumentNullException("keys");
            if (valueSelector == null) throw new ArgumentNullException("valueSelector");
            return keys.ToDictionary(e => e, valueSelector).AsMap();
        }
        ///<summary>Creates a readable map by projecting values to keys.</summary>
        public static IMap<TKey, TValue> KeyedBy<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector) {
            if (values == null) throw new ArgumentNullException("values");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            return values.ToDictionary(keySelector).AsMap();
        }

        ///<summary>Determines if two maps contain the same keys mapped to the same values.</summary>
        public static bool MapEquals<TKey, TValue>(this IMap<TKey, TValue> map1, IMap<TKey, TValue> map2, IEqualityComparer<TValue> comparer = null) {
            if (map1 == null) throw new ArgumentNullException("map1");
            if (map2 == null) throw new ArgumentNullException("map2");
            comparer = comparer ?? EqualityComparer<TValue>.Default;
            if (!map1.Keys.SetEquals(map2.Keys)) return false;
            if (map1.Keys.Any(e => !comparer.Equals(map1[e], map2[e]))) return false;
            return true;
        }

        ///<summary>Iterates the values in a readable map.</summary>
        public static IEnumerable<TValue> Values<TKey, TValue>(this IMap<TKey, TValue> map) {
            if (map == null) throw new ArgumentNullException("map");
            return map.AsEnumerable().Select(e => e.Value);
        }

        ///<summary>Creates a readable map with values derived from the given readable map by the given projection function.</summary>
        public static IMap<TKey, TOut> Select<TKey, TIn, TOut>(this IMap<TKey, TIn> map, Func<TIn, TOut> projection) {
            if (map == null) throw new ArgumentNullException("map");
            if (projection == null) throw new ArgumentNullException("projection");
            return new Map<TKey, TOut>(map.Keys, e => projection(map[e]));
        }

        ///<summary>Creates a readable map with entries from the given readable map whose keys match a filter.</summary>
        public static IMap<TKey, TValue> Where<TKey, TValue>(this IMap<TKey, TValue> map, Func<TKey, bool> keyFilter) {
            if (map == null) throw new ArgumentNullException("map");
            if (keyFilter == null) throw new ArgumentNullException("keyFilter");
            return new Map<TKey, TValue>(map.Keys.Where(keyFilter), e => map[e]);
        }
        ///<summary>Creates a readable map with entries from the given readable map whose values match a filter.</summary>
        public static IMap<TKey, TValue> WhereValue<TKey, TValue>(this IMap<TKey, TValue> map, Func<TValue, bool> valueFilter) {
            if (map == null) throw new ArgumentNullException("map");
            if (valueFilter == null) throw new ArgumentNullException("valueFilter");
            return new Map<TKey, TValue>(map.Keys.Where(e => valueFilter(map[e])), e => map[e]);
        }
        ///<summary>Combines two readable maps by mapping keys present in both to a projection of the values mapped to by the keys.</summary>
        public static IMap<TKey, TValueOut> Zip<TKey, TValueIn1, TValueIn2, TValueOut>(this IMap<TKey, TValueIn1> map1, IMap<TKey, TValueIn2> map2, Func<TValueIn1, TValueIn2, TValueOut> projection) {
            if (map1 == null) throw new ArgumentNullException("map1");
            if (map2 == null) throw new ArgumentNullException("map2");
            if (projection == null) throw new ArgumentNullException("projection");
            return new Map<TKey, TValueOut>(map1.Keys.Intersect(map2.Keys), e => projection(map1[e], map2[e]));
        }
    }
}
