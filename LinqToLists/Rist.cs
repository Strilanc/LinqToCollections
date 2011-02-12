using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToLists {
    ///<summary>Utility class for implementing a readable list via a count and getter delegate.</summary>
    [DebuggerDisplay("{ToString()}")]
    public class Rist<T> : IRist<T> {
        private readonly int _count;
        private readonly Func<int, T> _getter;
        private readonly IEnumerable<T> _iterator;

        [ContractInvariantMethod()]
        private void ObjectInvariant() {
            Contract.Invariant(_count >= 0);
            Contract.Invariant(_getter != null);
            Contract.Invariant(_iterator != null);
        }

        ///<summary>Constructs a readable list implementation.</summary>
        ///<param name="count">The number of list items.</param>
        ///<param name="getter">Delegate used to get the list items.</param>
        ///<param name="efficientIterator">Optionally used to provide a more efficient iterator than accessing each index in turn.</param>
        public Rist(int count, Func<int, T> getter, IEnumerable<T> efficientIterator = null) {
            Contract.Requires(count >= 0);
            Contract.Requires(getter != null);
            Contract.Ensures(this.Count == count);
            this._count = count;
            this._getter = getter;
            this._iterator = efficientIterator ?? DefaultIterator(count, getter);
            Contract.Assume(this.Count == count);
        }
        private static IEnumerable<T> DefaultIterator(int count, Func<int, T> getter) {
            for (int i = 0; i < count; i++)
                yield return getter(i);
        }

        public int Count { get { return _count; } }
        public T this[int index] { get { return _getter(index); } }
        public IEnumerator<T> GetEnumerator() { return this._iterator.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        public override string ToString() {
            const int MaxPreviewItemCount = 10;
            var initialItems = String.Join(", ", System.Linq.Enumerable.Take(this, 10));
            var suffix = Count > MaxPreviewItemCount ? "..." : "]";
            return "Count: " + Count + ", Items: [" + initialItems + suffix;
        }
    }
}
