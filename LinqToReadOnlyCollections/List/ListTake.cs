using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToReadOnlyCollections.List {
    internal sealed class ListTake<T> : AbstractReadOnlyList<T>, IPotentialMaxCount {
        public readonly IReadOnlyList<T> SubList;
        public readonly int RequiredTake;
        public readonly int ActualTake;
        public readonly bool IsTakeLast;
        public int? MaxCount { get { return this.ActualTake; } }
        public int MinCount { get; private set; }

        private ListTake(IReadOnlyList<T> subList, int requiredTake, int actualTake, bool isTakeLast) {
            this.SubList = subList;
            this.RequiredTake = requiredTake;
            this.ActualTake = actualTake;
            this.IsTakeLast = isTakeLast;
            this.MinCount = Math.Min(ActualTake, subList.TryGetMinCount());
        }
        public static IReadOnlyList<T> From(IReadOnlyList<T> subList, int requiredTake, int actualTake, bool isTakeLast) {
            if (subList == null) throw new ArgumentNullException("subList");
            if (requiredTake < 0) throw new ArgumentOutOfRangeException("requiredTake", "requiredTake < 0");
            if (actualTake < 0) throw new ArgumentOutOfRangeException("actualTake", "actualTake < 0");
            if (requiredTake > subList.Count) throw new ArgumentOutOfRangeException("requiredTake", "requiredTake > subList.Count");

            // if nothing is taken, the result is empty
            if (requiredTake == 0 && actualTake == 0) return ListEmpty<T>.Empty;

            // when there is more than enough available to take, there is no need to wrap
            var min = subList.TryGetMinCount();
            var max = subList.TryGetMaxCount();
            if (min >= requiredTake && max.HasValue && max.Value <= actualTake)
                return subList;
            
            // if taking more than there can ever be, no need to try to take more
            if (max.HasValue && max < actualTake)
                return From(subList, requiredTake, max.Value, isTakeLast);

            // when taking on top of taking (from the same side), the operations can be merged
            var p = subList as ListTake<T>;
            if (p != null && p.IsTakeLast == isTakeLast) 
                return From(
                    p.SubList, 
                    Math.Max(requiredTake, p.RequiredTake),
                    Math.Min(actualTake, p.ActualTake),
                    isTakeLast);

            return new ListTake<T>(subList, requiredTake, actualTake, isTakeLast);
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
                if (n < this.RequiredTake) throw new InvalidOperationException("Took past end of list.");
                return Math.Min(this.ActualTake, n);
            }
        }

        public override IEnumerator<T> GetEnumerator() {
            var n = this.SubList.Count;
            if (n < this.RequiredTake) throw new InvalidOperationException("Took past end of list.");
            if (this.IsTakeLast && this.ActualTake < n) return base.GetEnumerator();
            return Enumerable.Take(this.SubList, this.ActualTake).GetEnumerator();
        }
    }
}
