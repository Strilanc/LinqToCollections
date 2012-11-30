using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    internal sealed class ListSkip<T> : AbstractReadOnlyList<T>, IPotentialMaxCount {
        public readonly IReadOnlyList<T> SubList;
        public readonly int MinSkip;
        public readonly int MaxSkip;
        public readonly int Offset;
        public int? MaxCount { get; private set; }

        private ListSkip(IReadOnlyList<T> subList, int offset, int minSkip, int maxSkip) {
            this.SubList = subList;
            this.MinSkip = minSkip;
            this.MaxSkip = maxSkip;
            this.Offset = offset;
            var n = subList.TryGetMaxCount() - maxSkip;
            this.MaxCount = n.HasValue ? Math.Max(0, n.Value) : (int?)null;
        }
        public static IReadOnlyList<T> From(IReadOnlyList<T> subList, int offset, int minSkip, int maxSkip) {
            if (subList == null) throw new ArgumentNullException("subList");
            if (minSkip < 0) throw new ArgumentOutOfRangeException("minSkip", "minSkip < 0");
            if (maxSkip < minSkip) throw new ArgumentOutOfRangeException("maxSkip", "maxSkip < minSkip");
            if (minSkip > subList.Count) throw new ArgumentOutOfRangeException("minSkip", "minSkip > subList.Count");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset", "offset < 0");
            if (offset > maxSkip) throw new ArgumentOutOfRangeException("offset", "offset > maxSkip");

            // if nothing is skipped, the result is unchanged
            if (maxSkip == 0) return subList;

            // when skipping more than can ever be available, the result is an empty list
            var c = subList.TryGetMaxCount();
            if (c.HasValue && c.Value <= maxSkip)
                return ListEmpty<T>.Empty;

            // when skipping on top of skipping, the operations can be merged
            var s = subList as ListSkip<T>;
            if (s != null)
                return From(
                    s.SubList,
                    s.Offset + offset,
                    minSkip == 0 ? s.MinSkip : s.MaxSkip + minSkip,
                    s.MaxSkip + maxSkip);

            return new ListSkip<T>(subList, offset, minSkip, maxSkip);
        }

        public override T this[int index] {
            get {
                if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("index");
                return this.SubList[index + this.Offset];
            }
        }
        public override int Count {
            get {
                var n = SubList.Count;
                if (n < MinSkip) throw new InvalidOperationException("Skipped past end of list.");
                return Math.Max(0, n - MaxSkip);
            }
        }

        public override IEnumerator<T> GetEnumerator() {
            if (SubList.Count < MinSkip) throw new InvalidOperationException("Skipped past end of list.");
            return base.GetEnumerator();
        }
    }
}
