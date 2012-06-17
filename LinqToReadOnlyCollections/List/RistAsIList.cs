using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.List {
    ///<summary>Implements a ReadOnly IList by delegating calls to a readable list.</summary>
    internal sealed class RistAsIList<T> : IList<T>, IRist<T> {
        private readonly IRist<T> _list;
        
        [ContractInvariantMethod]
        void ObjectInvariant() {
            Contract.Invariant(_list != null);
        }

        public RistAsIList(IRist<T> list) {
            Contract.Requires<ArgumentException>(list != null);
            Contract.Ensures(this.Count == list.Count);
            Contract.Ensures(this.SequenceEqual(list));
            this._list = list;
            Contract.Assume(this.Count == list.Count);
            Contract.Assume(this.SequenceEqual(list));
        }

        public T this[int index] { 
            get {
                Contract.Assume(index < _list.Count);
                return _list[index]; 
            } 
        }
        public int Count { get { return _list.Count; } }
        public IEnumerator<T> GetEnumerator() { return _list.GetEnumerator(); }

        public bool IsReadOnly { get { return true; } }
        public int IndexOf(T item) {
            var eq = EqualityComparer<T>.Default;
            for (int i = 0; i < Count; i++)
                if (eq.Equals(item, this[i]))
                    return i;
            return -1;
        }
        public bool Contains(T item) {
            return IndexOf(item) != -1;
        }
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Contract cycle check creates false-positive")]
        public void CopyTo(T[] array, int arrayIndex) {
            for (int i = 0; i < Count; i++)
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
