using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.LinqToCollections {
    ///<summary>Manages both the Skip and SkipLast operations on readable collections.</summary>
    internal sealed class CollectionSkip<T> : AbstractReadOnlyCollection<T>, IBoundedCount {
        public readonly IReadOnlyCollection<T> SubCollection;
        public readonly int Amount;
        public readonly int Offset;
        public int? MaxCount { get; private set; }
        public int MinCount { get; private set; }

        private CollectionSkip(IReadOnlyCollection<T> subCollection, int offset, int amount) {
            this.SubCollection = subCollection;
            this.Amount = amount;
            this.Offset = offset;
            var n = subCollection.TryGetMaxCount() - amount;
            this.MaxCount = n.HasValue ? Math.Max(0, n.Value) : (int?)null;
            this.MinCount = Math.Max(0, subCollection.TryGetMinCount() - amount);
        }
        public static IReadOnlyCollection<T> From(IReadOnlyCollection<T> subCollection, int offset, int amount) {
            if (subCollection == null) throw new ArgumentNullException("subCollection");
            if (amount < 0) throw new ArgumentOutOfRangeException("amount", "amount < 0");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset", "offset < 0");
            if (offset > amount) throw new ArgumentOutOfRangeException("offset", "offset > amount");

            // use something with random access if possible
            var li = subCollection as IReadOnlyList<T>;
            if (li != null)
                return ListSkip<T>.From(li, offset, amount);

            // if nothing is skipped, the result is unchanged
            if (amount == 0) return subCollection;

            // when skipping more than can ever be available, the result is an empty collection
            var c = subCollection.TryGetMaxCount();
            if (c.HasValue && c.Value <= amount)
                return ListEmpty<T>.Empty;

            // when skipping on top of skipping, the operations can be merged
            var s = subCollection as CollectionSkip<T>;
            if (s != null)
                return From(
                    s.SubCollection,
                    s.Offset + offset,
                    s.Amount + amount);

            return new CollectionSkip<T>(subCollection, offset, amount);
        }

        public override int Count {
            get {
                return Math.Max(0, SubCollection.Count - Amount);
            }
        }
        public override IEnumerator<T> GetEnumerator() {
            // if only skipping stuff at the end of the last, we can use the normal Take enumerator
            if (Offset == 0) return Enumerable.Take(SubCollection, Count).GetEnumerator();
            // if skipping the whole collection, we're golden
            if (Count == 0) return Enumerable.Empty<T>().GetEnumerator();
            // otherwise index iterate to avoid scanning skipped sections
            return Enumerable.Skip(SubCollection, Offset).GetEnumerator();
        }
    }
}
