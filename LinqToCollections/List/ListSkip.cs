using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.LinqToCollections {
    ///<summary>Manages both the Skip and SkipLast operations on readable lists.</summary>
    internal sealed class ListSkip<T> : AbstractReadOnlyList<T>, IBoundedCount {
        public readonly IReadOnlyList<T> SubList;
        public readonly int Amount;
        public readonly int Offset;
        public int? MaxCount { get; private set; }
        public int MinCount { get; private set; }

        private ListSkip(IReadOnlyList<T> subList, int offset, int amount) {
            this.SubList = subList;
            this.Amount = amount;
            this.Offset = offset;
            var n = subList.TryGetMaxCount() - amount;
            this.MaxCount = n.HasValue ? Math.Max(0, n.Value) : (int?)null;
            this.MinCount = Math.Max(0, subList.TryGetMinCount() - amount);
        }
        public static IReadOnlyList<T> From(IReadOnlyList<T> subList, int offset, int amount) {
            if (subList == null) throw new ArgumentNullException("subList");
            if (amount < 0) throw new ArgumentOutOfRangeException("amount", "amount < 0");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset", "offset < 0");
            if (offset > amount) throw new ArgumentOutOfRangeException("offset", "offset > amount");

            // if nothing is skipped, the result is unchanged
            if (amount == 0) return subList;

            // when skipping more than can ever be available, the result is an empty list
            var c = subList.TryGetMaxCount();
            if (c.HasValue && c.Value <= amount)
                return ListEmpty<T>.Empty;

            // when skipping on top of skipping, the operations can be merged
            var s = subList as ListSkip<T>;
            if (s != null)
                return From(
                    s.SubList,
                    s.Offset + offset,
                    s.Amount + amount);

            return new ListSkip<T>(subList, offset, amount);
        }

        public override T this[int index] {
            get {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException("index");
                return SubList[index + Offset];
            }
        }
        public override int Count {
            get {
                return Math.Max(0, SubList.Count - Amount);
            }
        }
        public override IEnumerator<T> GetEnumerator() {
            // if only skipping stuff at the end of the last, we can use the normal Take enumerator
            if (Offset == 0) return Enumerable.Take(SubList, Count).GetEnumerator();
            // if skipping the whole list, we're golden
            if (Count == 0) return Enumerable.Empty<T>().GetEnumerator();
            // otherwise index iterate to avoid scanning skipped sections
            return EnumerateByIndex();
        }
    }
}
