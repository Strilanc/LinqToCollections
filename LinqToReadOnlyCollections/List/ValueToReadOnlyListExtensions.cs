using System;
using System.Collections.Generic;

namespace LinqToCollections.List {
    ///<summary>Contains extension methods to convert from any type to list types.</summary>
    public static class ValueToReadOnlyListExtensions {
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
    }
}
