using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    ///<summary>Implements a readonly list implemented with delegates passed to its constructor.</summary>
    public sealed class AnonymousReadOnlyList<T> : AbstractReadOnlyList<T> {
        private readonly Func<int> _counter;
        private readonly Func<int, T> _getter;
        private readonly IEnumerable<T> _efficientIterator;

        ///<summary>Constructs a readable list implementation.</summary>
        ///<param name="counter">Determines the non-negative number of list items.</param>
        ///<param name="getter">Retrieves list items by index.</param>
        ///<param name="efficientIterator">Optionally used to provide a more efficient/capable iterator than accessing each index in turn.</param>
        public AnonymousReadOnlyList(Func<int> counter, Func<int, T> getter, IEnumerable<T> efficientIterator = null) {
            if (counter == null) throw new ArgumentNullException("counter");
            if (getter == null) throw new ArgumentNullException("getter");
            this._counter = counter;
            this._getter = getter;
            this._efficientIterator = efficientIterator;
        }

        public override int Count {
            get {
                var r = _counter();
                if (r < 0) throw new InvalidOperationException("Count < 0");
                return r;
            }
        }
        public override T this[int index] { 
            get {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException("index");
                return _getter(index); 
            } 
        }
        public override IEnumerator<T> GetEnumerator() {
            return this._efficientIterator == null 
                 ? base.GetEnumerator() 
                 : this._efficientIterator.GetEnumerator();
        }
    }
}
