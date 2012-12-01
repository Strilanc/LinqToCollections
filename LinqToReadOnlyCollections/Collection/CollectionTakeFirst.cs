using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.LinqToCollections {
    ///<summary>Manages the Take operation on readable collections.</summary>
    internal sealed class CollectionTakeFirst<T> : AbstractReadOnlyCollection<T>, IBoundedCount {
        public readonly IReadOnlyCollection<T> SubCollection;
        public readonly int Amount;
        public int? MaxCount { get { return this.Amount; } }
        public int MinCount { get; private set; }

        private CollectionTakeFirst(IReadOnlyCollection<T> subCollection, int amount) {
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
                return ListTakeFirst<T>.From(li, amount);

            // if nothing is taken, the result is empty
            if (amount == 0) return ListEmpty<T>.Empty;

            // when we are guaranteed to take everything, there is no need to wrap
            var max = subCollection.TryGetMaxCount();
            if (max.HasValue && max.Value <= amount)
                return subCollection;
            
            // when taking on top of taking (from the same side), the operations can be merged
            var p = subCollection as CollectionTakeFirst<T>;
            if (p != null) 
                return From(
                    p.SubCollection, 
                    Math.Min(amount, p.Amount));

            return new CollectionTakeFirst<T>(subCollection, amount);
        }

        public override int Count {
            get {
                return Math.Min(Amount, SubCollection.Count);
            }
        }

        public override IEnumerator<T> GetEnumerator() {
            // when taking everything, no need to wrap
            if (Amount >= SubCollection.Count) return SubCollection.GetEnumerator();
            // use the standard enumerator
            return Enumerable.Take(SubCollection, Amount).GetEnumerator();
        }
    }
}
