using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    ///<summary>Implements a readonly list implemented with delegates passed to its constructor.</summary>
    public sealed class AnonymousReadOnlyList<T> : AbstractReadOnlyList<T>, IPotentialMaxCount {
        private readonly int? _maxCount;
        int? IPotentialMaxCount.MaxCount { get { return _maxCount; } }

        private readonly Func<int> _counter;
        private readonly Func<int, T> _getter;
        private readonly IEnumerable<T> _optionalEfficientIterator;

        ///<summary>Constructs a readable list implementation.</summary>
        ///<param name="counter">Determines the non-negative number of list items.</param>
        ///<param name="getter">Retrieves list items by index.</param>
        ///<param name="optionalEfficientIterator">Optionally used to provide a more efficient/capable iterator than accessing each index in turn.</param>
        public AnonymousReadOnlyList(Func<int> counter, Func<int, T> getter, IEnumerable<T> optionalEfficientIterator = null)
            : this(counter, null, getter, optionalEfficientIterator) {
        }

        ///<summary>Constructs a readable list implementation.</summary>
        ///<param name="count">The non-negative number of list items.</param>
        ///<param name="getter">Retrieves list items by index.</param>
        ///<param name="optionalEfficientIterator">Optionally used to provide a more efficient/capable iterator than accessing each index in turn.</param>
        public AnonymousReadOnlyList(int count, Func<int, T> getter, IEnumerable<T> optionalEfficientIterator = null)
            : this(() => count, count, getter, optionalEfficientIterator) {
            if (count < 0) throw new ArgumentOutOfRangeException("count", "count < 0");
        }

        internal AnonymousReadOnlyList(Func<int> counter, int? maxCount, Func<int, T> getter, IEnumerable<T> optionalEfficientIterator = null) {
            if (counter == null) throw new ArgumentNullException("counter");
            if (getter == null) throw new ArgumentNullException("getter");
            this._counter = counter;
            this._getter = getter;
            this._optionalEfficientIterator = optionalEfficientIterator;
            this._maxCount = maxCount;
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
            return this._optionalEfficientIterator == null 
                 ? base.GetEnumerator() 
                 : this._optionalEfficientIterator.GetEnumerator();
        }
    }
}
