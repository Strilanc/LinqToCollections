using System.Collections.Generic;

namespace Strilanc.LinqToCollections {
    ///<summary>A partially implemented readonly collection.</summary>
    public abstract class AbstractReadOnlyCollection<T> : IReadOnlyCollection<T> {
        ///<summary>The number of items in the collection.</summary>
        public abstract int Count { get; }
        ///<summary>Enumerates the items in the collection.</summary>
        public abstract IEnumerator<T> GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
