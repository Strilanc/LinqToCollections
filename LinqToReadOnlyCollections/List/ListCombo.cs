using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    internal sealed class ListCombo<T> : AbstractReadOnlyList<T>, IList<T>, IPotentialMaxCount {
        public readonly IReadOnlyList<T> List;
        public int? MaxCount { get; private set; }
        
        public ListCombo(IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            this.List = list;
            this.MaxCount = list.TryGetMaxCount();
        }

        // delegate methods for IReadOnlyList
        public override T this[int index] { get { return this.List[index]; } }
        public override int Count { get { return this.List.Count; } }
        public override IEnumerator<T> GetEnumerator() { return this.List.GetEnumerator(); }

        // implement non-mutating methods for IList
        public bool IsReadOnly { get { return true; } }
        public int IndexOf(T item) {
            var eq = EqualityComparer<T>.Default;
            for (var i = 0; i < Count; i++)
                if (eq.Equals(item, this[i]))
                    return i;
            return -1;
        }
        public bool Contains(T item) {
            return IndexOf(item) != -1;
        }
        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex + Count > array.Length) throw new ArgumentOutOfRangeException("arrayIndex");
            for (var i = 0; i < Count; i++)
                array[i + arrayIndex] = this[i];
        }

        // don't support mutating methods for IList
        void IList<T>.Insert(int index, T item) { throw new NotSupportedException("Collection is read-only."); }
        void IList<T>.RemoveAt(int index) { throw new NotSupportedException("Collection is read-only."); }
        void ICollection<T>.Add(T item) { throw new NotSupportedException("Collection is read-only."); }
        void ICollection<T>.Clear() { throw new NotSupportedException("Collection is read-only."); }
        bool ICollection<T>.Remove(T item) { throw new NotSupportedException("Collection is read-only."); }
        T IList<T>.this[int index] {
            get { return this[index]; }
            set { throw new NotSupportedException("Collection is read-only."); }
        }
    }
}
