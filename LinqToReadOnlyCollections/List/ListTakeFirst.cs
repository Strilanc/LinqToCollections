using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToReadOnlyCollections.List {
    ///<summary>Manages the Take operation on readable lists.</summary>
    internal sealed class ListTakeFirst<T> : AbstractReadOnlyList<T>, IBoundedCount {
        public readonly IReadOnlyList<T> SubList;
        public readonly int Amount;
        public int? MaxCount { get { return this.Amount; } }
        public int MinCount { get; private set; }

        private ListTakeFirst(IReadOnlyList<T> subList, int amount) {
            this.SubList = subList;
            this.Amount = amount;
            this.MinCount = Math.Min(Amount, subList.TryGetMinCount());
        }
        public static IReadOnlyList<T> From(IReadOnlyList<T> subList, int amount) {
            if (subList == null) throw new ArgumentNullException("subList");
            if (amount < 0) throw new ArgumentOutOfRangeException("amount", "amount < 0");

            // if nothing is taken, the result is empty
            if (amount == 0) return ListEmpty<T>.Empty;

            // when we are guaranteed to take everything, there is no need to wrap
            var max = subList.TryGetMaxCount();
            if (max.HasValue && max.Value <= amount)
                return subList;
            
            // when taking on top of taking (from the same side), the operations can be merged
            var p = subList as ListTakeFirst<T>;
            if (p != null) 
                return From(
                    p.SubList, 
                    Math.Min(amount, p.Amount));

            return new ListTakeFirst<T>(subList, amount);
        }

        public override T this[int index] {
            get {
                if (index >= Amount) throw new ArgumentOutOfRangeException("index");
                return SubList[index];
            }
        }
        public override int Count {
            get {
                return Math.Min(Amount, SubList.Count);
            }
        }

        public override IEnumerator<T> GetEnumerator() {
            // when taking everything, no need to wrap
            if (Amount >= SubList.Count) return SubList.GetEnumerator();
            // use the standard enumerator
            return Enumerable.Take(SubList, Amount).GetEnumerator();
        }
    }
}
