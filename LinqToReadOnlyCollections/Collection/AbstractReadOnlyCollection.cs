using System.Collections.Generic;

namespace Strilanc.LinqToCollections {
    ///<summary>A partially implemented readonly collection.</summary>
    public abstract class AbstractReadOnlyCollection<T> : IReadOnlyCollection<T> {
        public abstract int Count { get; }
        public abstract IEnumerator<T> GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
