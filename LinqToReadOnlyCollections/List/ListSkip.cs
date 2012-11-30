using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    internal sealed class ListSkip<T> : AbstractReadOnlyList<T> {
        private readonly IReadOnlyList<T> _subList;
        private readonly int _skipExact;
        private readonly int _skipElastic;
        private readonly int _offset;

        public ListSkip(IReadOnlyList<T> subList, int skipExact, int skipElastic, int offset) {
            if (subList == null) throw new ArgumentNullException("subList");
            if (skipElastic < 0) throw new ArgumentOutOfRangeException("skipElastic");
            if (skipExact < 0 || skipExact > subList.Count) throw new ArgumentOutOfRangeException("skipExact");
            if (offset < 0 || offset > skipExact + skipElastic) throw new ArgumentOutOfRangeException("offset");

            this._subList = subList;
            this._skipExact = skipExact;
            this._skipElastic = skipElastic;
            this._offset = offset;

            var p = subList as ListSkip<T>;
            if (p != null) {
                if (this._skipExact > 0) {
                    this._skipExact += p._skipElastic;
                } else {
                    this._skipElastic += p._skipElastic;
                }
                this._subList = p._subList;
                this._skipExact += p._skipExact;
                this._offset += p._offset;
            }
        }

        public override T this[int index] {
            get {
                if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("index");
                return _subList[index + _offset];
            }
        }
        public override int Count {
            get {
                if (_subList.Count < _skipExact) throw new InvalidOperationException("Skipped past end of list.");
                return Math.Max(0, _subList.Count - _skipExact - _skipElastic);
            }
        }

        public override IEnumerator<T> GetEnumerator() {
            if (_subList.Count < _skipExact) throw new InvalidOperationException("Skipped past end of list.");
            return base.GetEnumerator();
        }
    }
}
