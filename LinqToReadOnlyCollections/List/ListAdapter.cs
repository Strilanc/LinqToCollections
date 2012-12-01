using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Strilanc.LinqToCollections {
    ///<summary>Exposes a readonly list as a mutable list that doesn't support mutation.</summary>
    internal sealed class ListAdapter<T> : AbstractReadOnlyList<T>, IList<T>, IBoundedCount {
        public readonly IReadOnlyList<T> List;
        public int? MaxCount { get; private set; }
        public int MinCount { get; private set; }
        
        private ListAdapter(IReadOnlyList<T> list) {
            this.List = list;
            this.MaxCount = list.TryGetMaxCount();
            this.MinCount = list.TryGetMinCount();
        }
        public static IReadOnlyList<T> From(IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            return new ListAdapter<T>(list);
        }
        public static IReadOnlyList<T> Adapt(IList<T> list) {
            if (list == null) throw new ArgumentNullException("list");

            if (list.IsReadOnly && !(list is T[])) {
                // if it's an adapter, then we can unwrap it
                var c = list as ListAdapter<T>;
                if (c != null) return c.List;

                // if it's already what we need, great!
                var r = list as IReadOnlyList<T>;
                if (r != null) return r;
            }

            // use existing readonly adapter
            return new ReadOnlyCollection<T>(list);
        }
        public static IList<T> Adapt(IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");

            // if it's already a list, we can just return it
            var r = list as IList<T>;
            if (r != null) return r;

            // otherwise we need to adapt it
            return new ListAdapter<T>(list);
        }

        // delegate methods for IReadOnlyList
        public override T this[int index] { get { return List[index]; } }
        public override int Count { get { return List.Count; } }
        public override IEnumerator<T> GetEnumerator() { return List.GetEnumerator(); }

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
