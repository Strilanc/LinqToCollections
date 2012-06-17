using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.Set {
    ///<summary>Utility class for implementing a readable set via a container delegate and an iterator.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "'Ret' is better than 'Collection'")]
    public class Ret<T> : IRet<T> {
        private readonly Func<T, bool> container;
        private readonly IEnumerable<T> iterator;
        
        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(container != null);
            Contract.Invariant(iterator != null);
        }

        ///<summary>Constructs a readable set implementation.</summary>
        ///<param name="container">Determines if an item is in the set.</param>
        ///<param name="iterator">Iterates the items in the set.</param>
        public Ret(Func<T, bool> container, IEnumerable<T> iterator) {
            Contract.Requires<ArgumentException>(container != null);
            Contract.Requires<ArgumentException>(iterator != null);
            this.container = container;
            this.iterator = iterator;
        }
        public bool Contains(T item) { return container(item); }
        public IEnumerator<T> GetEnumerator() { return iterator.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public override string ToString() {
            const int MaxPreviewItemCount = 10;
            var initialItems = iterator.Take(MaxPreviewItemCount + 1);
            var suffix = initialItems.Count() == MaxPreviewItemCount + 1 ? ", ..." : "}";
            return "{" + String.Join(", ", initialItems.Take(MaxPreviewItemCount)) + suffix;
        }
    }
}
