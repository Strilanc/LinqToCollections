using System;
using System.Collections;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    internal interface IBoundedCount {
        int? MaxCount { get; }
        int MinCount { get; }
    }
    internal static class BoundedCount {
        public static int? TryGetMaxCount<T>(this IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            var r = list as IBoundedCount;
            if (r != null) return r.MaxCount;
            var a = list as IList;
            if (a != null && a.IsFixedSize) return a.Count;
            return null;
        }
        public static int TryGetMinCount<T>(this IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            var r = list as IBoundedCount;
            if (r != null) return r.MinCount;
            var a = list as IList;
            if (a != null && a.IsFixedSize) return a.Count;
            return 0;
        }
    }
}