using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    ///<summary>Manages the TakeLast operation on readable lists.</summary>
    internal sealed class ListTakeLast<T> : AbstractReadOnlyList<T>, IBoundedCount {
        public readonly IReadOnlyList<T> SubList;
        public readonly int Amount;
        public int? MaxCount { get { return this.Amount; } }
        public int MinCount { get; private set; }

        private ListTakeLast(IReadOnlyList<T> subList, int amount) {
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
            var p = subList as ListTakeLast<T>;
            if (p != null) 
                return From(
                    p.SubList, 
                    Math.Min(amount, p.Amount));

            return new ListTakeLast<T>(subList, amount);
        }

        public override T this[int index] {
            get {
                if (index >= Amount) throw new ArgumentOutOfRangeException("index");
                var n = SubList.Count;
                return this.SubList[n - Math.Min(n, Amount) + index];
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
            // use indexing iteration to avoid scanning the start of the list
            return EnumerateByIndex();
        }
    }
}
