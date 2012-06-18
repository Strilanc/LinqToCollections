using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using LinqToCollections.List;
using LinqToCollections.Map;
using LinqToCollections.Set;

namespace LinqToCollections.Extra {
    ///<summary>Contains extension methods which affect non-list types.</summary>
    ///<remarks>Not included in the main namespace to avoid unwanted pollution.</remarks>
    public static class ExtraExtensions {
        ///<summary>Exposes the non-negative integers below the count as a readable list.</summary>
        public static IReadOnlyList<int> Range(this int count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new ReadOnlyList<int>(counter: () => count, getter: i => i);
        }
        ///<summary>Exposes the non-negative bytes below the count as a readable list.</summary>
        public static IReadOnlyList<byte> Range(this byte count) {
            return new ReadOnlyList<byte>(counter: () => count, getter: i => (byte)i);
        }
        ///<summary>Exposes the non-negative signed 16-bit integers below the count as a readable list.</summary>
        public static IReadOnlyList<short> Range(this short count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new ReadOnlyList<short>(counter: () => count, getter: i => (short)i);
        }

        ///<summary>Returns a readable list composed of a repeated value.</summary>
        public static IReadOnlyList<T> Repeated<T>(this T value, int count) {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            return new ReadOnlyList<T>(counter: () => count, getter: i => value);
        }

        ///<summary>Creates a key value pair.</summary>
        public static IKeyValue<TKey, TValue> KeyValue<TKey, TValue>(this TKey key, TValue value) {
            if (key == null) throw new ArgumentNullException("key");
            return new KeyValue<TKey, TValue>(key, value);
        }
    }
}
