using System.Collections.Generic;
using System.Linq;
using System;
using LinqToReadOnlyCollections.Collection;

namespace LinqToReadOnlyCollections.Dictionary {
    ///<summary>Contains extension methods having to do with the IReadOnlyDictionary interface.</summary>
    public static class ReadOnlyDictionary {
        ///<summary>Exposes a dictionary as a readonly dictionary.</summary>
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            return dictionary as IReadOnlyDictionary<TKey, TValue>
                ?? new AnonymousReadOnlyDictionary<TKey, TValue>(
                        dictionary.Keys.AsReadOnlyCollection(),
                        dictionary.TryGetValue);
        }
        ///<summary>Exposes a readonly dictionary as an IDictionary (readonly).</summary>
        ///<remarks>Using AsReadOnlyDictionary on the result will use a cast instead of wrapping more (and AsIDictionary on that will also cast instead of wrap).</remarks>
        public static IDictionary<TKey, TValue> AsIDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary) {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            return dictionary as IDictionary<TKey, TValue> 
                ?? new DictionaryAdapter<TKey, TValue>(dictionary);
        }
        ///<summary>Creates a copy of the given sequence and exposes the copy as a readable dictionary.</summary>
        public static IReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            return sequence.ToDictionary(e => e.Key, e => e.Value);
        }
        ///<summary>Exposes the underlying dictionary of a given sequence as a readable dictionary, creating a copy if the underlying type is not a dictionary.</summary>
        ///<remarks>Just a cast when the sequence is an IReadOnlyDictionary, and equivalent to AsReadOnlyDictionary(IDictionary) when the sequence is an IDictionary.</remarks>
        public static IReadOnlyDictionary<TKey, TValue> AsElseToReadOnlyDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");

            var asReadOnlyDict = sequence as IReadOnlyDictionary<TKey, TValue>;
            if (asReadOnlyDict != null) return asReadOnlyDict;

            var asDict = sequence as IDictionary<TKey, TValue>;
            if (asDict != null) return asDict.AsReadOnlyDictionary();

            return sequence.ToReadOnlyDictionary();
        }

        ///<summary>Creates a readable dictionary with values derived from the given readable dictionary by the given projection function.</summary>
        public static IReadOnlyDictionary<TKey, TValueOut> Select<TKey, TValueIn, TValueOut>(this IReadOnlyDictionary<TKey, TValueIn> dictionary, Func<TKey, TValueIn, TValueOut> projection) {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyDictionary<TKey, TValueOut>(
                new ReadOnlyCollection<TKey>(() => dictionary.Count, () => dictionary.Keys.GetEnumerator()),
                (TKey k, out TValueOut v) => {
                    TValueIn vin;
                    if (!dictionary.TryGetValue(k, out vin)) {
                        v = default(TValueOut);
                        return false;
                    }
                    v = projection(k, vin);
                    return true;
                }
            );
        }

        ///<summary>Creates a readable dictionary with values derived from the given readable dictionary by the given projection function.</summary>
        public static IReadOnlyDictionary<TKey, TValueOut> Select<TKey, TValueIn, TValueOut>(this IReadOnlyDictionary<TKey, TValueIn> dictionary, Func<TValueIn, TValueOut> projection) {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (projection == null) throw new ArgumentNullException("projection");
            return dictionary.Select((k, v) => projection(v));
        }
    }
}
