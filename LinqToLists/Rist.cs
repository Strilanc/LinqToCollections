using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToLists {
    ///<summary>Utility class for implementing a readable list via a count and getter delegate.</summary>
    [DebuggerDisplay("{ToString()}")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class Rist<T> : IRist<T> {
        private readonly Func<int> _counter;
        private readonly Func<int, T> _getter;
        private readonly IEnumerable<T> _iterator;

        [ContractInvariantMethod()]
        private void ObjectInvariant() {
            Contract.Invariant(_counter != null);
            Contract.Invariant(_getter != null);
            Contract.Invariant(_iterator != null);
        }

        ///<summary>Constructs a readable list implementation.</summary>
        ///<param name="counter">Gets the number of list items.</param>
        ///<param name="getter">Gets indexed list items.</param>
        ///<param name="efficientIterator">Optionally used to provide a more efficient iterator than accessing each index in turn.</param>
        public Rist(Func<int> counter, Func<int, T> getter, IEnumerable<T> efficientIterator = null) {
            Contract.Requires<ArgumentException>(counter != null);
            Contract.Requires<ArgumentException>(getter != null);
            this._counter = counter;
            this._getter = getter;
            this._iterator = efficientIterator ?? DefaultIterator(counter, getter);
        }
        private static IEnumerable<T> DefaultIterator(Func<int> counter, Func<int, T> getter) {
            int n = counter();
            for (int i = 0; i < n; i++)
                yield return getter(i);
        }

        public int Count {
            get {
                var r = _counter();
                if (r < 0) throw new InvalidOperationException("Invalid counter delegate.");
                return r;
            }
        }
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
