using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    public abstract class AbstractReadOnlyList<T> : IReadOnlyList<T> {
        public abstract int Count { get; }
        public abstract T this[int index] { get; }
        public abstract IEnumerator<T> GetEnumerator();
        protected IEnumerator<T> EnumerateByIndex() {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
