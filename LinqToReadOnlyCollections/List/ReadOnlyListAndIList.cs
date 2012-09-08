using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    ///<summary>Implements a ReadOnly IList by delegating calls to a readable list.</summary>
    internal sealed class ReadOnlyListIList<T> : IList<T>, IReadOnlyList<T> {
        private readonly IReadOnlyList<T> _list;
        
        public ReadOnlyListIList(IReadOnlyList<T> list) {
            if (list == null) throw new ArgumentNullException("list");
            this._list = list;
        }

        public T this[int index] { 
            get {
                return _list[index]; 
            } 
        }
        public int Count { get { return _list.Count; } }
        public IEnumerator<T> GetEnumerator() { return _list.GetEnumerator(); }

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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
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
