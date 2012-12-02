using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.LinqToCollections {
    ///<summary>Contains extension methods for readonly dictionaries.</summary>
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

        ///<summary>Returns a readable dictionary with the same keys, but values determined by projecting the existing key/value pairs.</summary>
        public static IReadOnlyDictionary<TKey, TResult> Select<TKey, TValue, TResult>(this IReadOnlyDictionary<TKey, TValue> dictionary,
                                                                                       Func<KeyValuePair<TKey, TValue>, TResult> projection) {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (projection == null) throw new ArgumentNullException("projection");
            return new AnonymousReadOnlyDictionary<TKey, TResult>(
                () => dictionary.Count,
                dictionary.Keys,
                (TKey k, out TResult r) => {
                    TValue v;
                    if (!dictionary.TryGetValue(k, out v)) {
                        r = default(TResult);
                        return false;
                    }
                    r = projection(new KeyValuePair<TKey, TValue>(k, v));
                    return true;
                });
        }
        
        ///<summary>Returns a readable dictionary with no items.</summary>
        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() {
            return new AnonymousReadOnlyDictionary<TKey, TValue>(
                () => 0,
                Enumerable.Empty<TKey>(),
                (TKey key, out TValue value) => {
                    value = default(TValue);
                    return false;
                });
        }
    }
}
