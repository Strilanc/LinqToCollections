using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    internal interface IPotentialMaxCount {
        int? MaxCount { get; }
    }
    internal static class PotentialMaxCount {
        public static int? TryGetMaxCount<T>(this IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            var r = list as IPotentialMaxCount;
            if (r == null) return null;
            return r.MaxCount;
        }
    }
}