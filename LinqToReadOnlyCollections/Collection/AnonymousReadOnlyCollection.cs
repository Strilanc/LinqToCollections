using System;
using System.Collections.Generic;

namespace Strilanc.LinqToCollections {
    ///<summary>A readonly collection implemented with delegates passed to its constructor.</summary>
    public sealed class AnonymousReadOnlyCollection<T> : AbstractReadOnlyCollection<T>, IBoundedCount {
        private readonly int? _maxCount;
        private readonly int _minCount;
        int? IBoundedCount.MaxCount { get { return _maxCount; } }
        int IBoundedCount.MinCount { get { return _minCount; } }

        private readonly Func<int> _counter;
        private readonly Func<IEnumerator<T>> _iterator;

        ///<summary>Constructs a readable collection implementation.</summary>
        ///<param name="counter">Determines the non-negative number of collection items.</param>
        ///<param name="iterator">Enumerates the items in the collection.</param>
        public AnonymousReadOnlyCollection(Func<int> counter, Func<IEnumerator<T>> iterator)
            : this(counter, null, 0, iterator) {
        }

        ///<summary>Constructs a readable collection implementation.</summary>
        ///<param name="count">The non-negative number of collection items.</param>
        ///<param name="iterator">Enumerates the items in the collection.</param>
        public AnonymousReadOnlyCollection(int count, Func<IEnumerator<T>> iterator)
            : this(() => count, count, count, iterator) {
            if (iterator == null) throw new ArgumentNullException("iterator");
            if (count < 0) throw new ArgumentOutOfRangeException("count", "count < 0");
        }

        internal AnonymousReadOnlyCollection(Func<int> counter, int? maxCount, int minCount, Func<IEnumerator<T>> iterator) {
            if (counter == null) throw new ArgumentNullException("counter");
            if (iterator == null) throw new ArgumentNullException("iterator");
            this._counter = counter;
            this._iterator = iterator;
            this._maxCount = maxCount;
            this._minCount = minCount;
        }

        public override int Count {
            get {
                var r = _counter();
                if (r < 0) throw new InvalidOperationException("Count < 0");
                return r;
            }
        }
        public override IEnumerator<T> GetEnumerator() {
            return _iterator();
        }
    }
}
