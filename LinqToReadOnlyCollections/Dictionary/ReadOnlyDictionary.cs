using System.Collections.Generic;
using System.Linq;
using System;
using LinqToReadOnlyCollections.Collection;

namespace LinqToReadOnlyCollections.Dictionary {
    ///<summary>Contains extension methods having to do with the IReadOnlyDictionary interface.</summary>
    public static class ReadOnlyDictionary {
        /// <summary>
        /// Exposes a dictionary as a read-only dictionary.
        /// Tries to unwrap the dictionary, removing previous AsIList overhead if possible.
        /// Tries to cast the dictionary, unless the dictionary is not marked as read-only.
        /// </summary>
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            return DictionaryAdapter<TKey, TValue>.Adapt(dictionary);
        }
        ///<summary>Exposes a read-only dictionary as a dictionary, using a cast if possible.</summary>
        public static IDictionary<TKey, TValue> AsIDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary) {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            return DictionaryAdapter<TKey, TValue>.Adapt(dictionary);
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
