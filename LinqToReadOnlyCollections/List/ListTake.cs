using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToReadOnlyCollections.List {
    internal sealed class ListTake<T> : AbstractReadOnlyList<T>, IPotentialMaxCount {
        public readonly IReadOnlyList<T> SubList;
        public readonly int MinCount;
        public readonly int MaxCount;
        public readonly bool IsTakeLast;
        int? IPotentialMaxCount.MaxCount { get { return MaxCount; } }

        private ListTake(IReadOnlyList<T> subList, int minCount, int maxCount, bool isTakeLast) {
            this.SubList = subList;
            this.MinCount = minCount;
            this.MaxCount = maxCount;
            this.IsTakeLast = isTakeLast;
        }
        public static IReadOnlyList<T> From(IReadOnlyList<T> subList, int minCount, int maxCount, bool isTakeLast) {
            if (subList == null) throw new ArgumentNullException("subList");
            if (minCount < 0) throw new ArgumentOutOfRangeException("minCount", "minCount < 0");
            if (maxCount < minCount) throw new ArgumentOutOfRangeException("maxCount", "maxCount < minCount");
            if (minCount > subList.Count) throw new ArgumentOutOfRangeException("minCount", "minCount > subList.Count");

            // when taking more than can ever be available, there is no change
            var c = subList.TryGetMaxCount();
            if (c.HasValue && c.Value <= maxCount)
                return subList;

            // when taking on top of taking (from the same side), the operations can be merged
            var p = subList as ListTake<T>;
            if (p != null && p.IsTakeLast == isTakeLast) 
                return From(
                    p.SubList, 
                    Math.Max(minCount, p.MinCount), 
                    Math.Min(maxCount, p.MaxCount),
                    isTakeLast);

            return new ListTake<T>(subList, minCount, maxCount, isTakeLast);
        }

        public override T this[int index] {
            get {
                var n = Count;
                if (index < 0 || index > n) throw new ArgumentOutOfRangeException("index");
                return this.IsTakeLast ? this.SubList[this.SubList.Count - n + index] : this.SubList[index];
            }
        }
        public override int Count {
            get {
                var n = this.SubList.Count;
                if (n < this.MinCount) throw new InvalidOperationException("Took past end of list.");
                return Math.Min(this.MaxCount, n);
            }
        }

        public override IEnumerator<T> GetEnumerator() {
            var n = this.SubList.Count;
            if (n < this.MinCount) throw new InvalidOperationException("Took past end of list.");
            if (this.IsTakeLast && this.MaxCount < n) return base.GetEnumerator();
            return Enumerable.Take(this.SubList, this.MaxCount).GetEnumerator();
        }
    }
}
