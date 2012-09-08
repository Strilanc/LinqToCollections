using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.Set {
    ///<summary>Utility class for implementing a readable set via a _container delegate and an _iterator.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "'Ret' is better than 'Collection'")]
    public class Ret<T> : IRet<T> {
        private readonly Func<T, bool> _container;
        private readonly IEnumerable<T> _iterator;
        
        ///<summary>Constructs a readable set implementation.</summary>
        ///<param name="container">Determines if an item is in the set.</param>
        ///<param name="iterator">Iterates the items in the set.</param>
        public Ret(Func<T, bool> container, IEnumerable<T> iterator) {
            if (container == null) throw new ArgumentNullException("container");
            if (iterator == null) throw new ArgumentNullException("iterator");
            this._container = container;
            this._iterator = iterator;
        }
        public bool Contains(T item) { return this._container(item); }
        public IEnumerator<T> GetEnumerator() { return this._iterator.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public override string ToString() {
            const int MaxPreviewItemCount = 10;
            var initialItems = this._iterator.Take(MaxPreviewItemCount + 1).ToArray();
            var suffix = initialItems.Length == MaxPreviewItemCount + 1 ? ", ..." : "}";
            return "{" + String.Join(", ", initialItems.Take(MaxPreviewItemCount)) + suffix;
        }
    }
}
