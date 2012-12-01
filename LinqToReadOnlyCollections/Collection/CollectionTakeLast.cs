using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.LinqToCollections {
    ///<summary>Manages the TakeLast operation on readable collections.</summary>
    internal sealed class CollectionTakeLast<T> : AbstractReadOnlyCollection<T>, IBoundedCount {
        public readonly IReadOnlyCollection<T> SubCollection;
        public readonly int Amount;
        public int? MaxCount { get { return this.Amount; } }
        public int MinCount { get; private set; }

        private CollectionTakeLast(IReadOnlyCollection<T> subCollection, int amount) {
            this.SubCollection = subCollection;
            this.Amount = amount;
            this.MinCount = Math.Min(Amount, subCollection.TryGetMinCount());
        }
        public static IReadOnlyCollection<T> From(IReadOnlyCollection<T> subCollection, int amount) {
            if (subCollection == null) throw new ArgumentNullException("subCollection");
            if (amount < 0) throw new ArgumentOutOfRangeException("amount", "amount < 0");

            // use something with random access if possible
            var li = subCollection as IReadOnlyList<T>;
            if (li != null) 
                return ListTakeLast<T>.From(li, amount);

            // if nothing is taken, the result is empty
            if (amount == 0) return ListEmpty<T>.Empty;

            // when we are guaranteed to take everything, there is no need to wrap
            var max = subCollection.TryGetMaxCount();
            if (max.HasValue && max.Value <= amount)
                return subCollection;
            
            // when taking on top of taking (from the same side), the operations can be merged
            var p = subCollection as CollectionTakeLast<T>;
            if (p != null) 
                return From(
                    p.SubCollection, 
                    Math.Min(amount, p.Amount));

            return new CollectionTakeLast<T>(subCollection, amount);
        }

        public override int Count {
            get {
                return Math.Min(Amount, SubCollection.Count);
            }
        }

        public override IEnumerator<T> GetEnumerator() {
            var skipped = SubCollection.Count - Amount;
            // when taking everything, no need to wrap
            if (skipped <= 0) return SubCollection.GetEnumerator();
            // use indexing iteration to avoid scanning the start of the collection
            return Enumerable.Skip(SubCollection, skipped).GetEnumerator();
        }
    }
}
