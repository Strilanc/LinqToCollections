using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LinqToCollections.List {
    [DebuggerDisplay("{ToString()}")]
    internal sealed class ReadOnlyList_Skip<T> : IReadOnlyList<T> {
        private readonly IReadOnlyList<T> _subList;
        private readonly int _skipExact;
        private readonly int _skipElastic;
        private readonly int _offset;

        public ReadOnlyList_Skip(IReadOnlyList<T> subList, int skipExact, int skipElastic, int offset) {
            if (subList == null) throw new ArgumentNullException("subList");
            if (skipElastic < 0) throw new ArgumentOutOfRangeException("skipElastic");
            if (skipExact < 0 || skipExact > subList.Count) throw new ArgumentOutOfRangeException("skipExact");
            if (offset < 0 || offset > skipExact + skipElastic) throw new ArgumentOutOfRangeException("offset");

            this._subList = subList;
            this._skipExact = skipExact;
            this._skipElastic = skipElastic;
            this._offset = offset;

            var p = subList as ReadOnlyList_Skip<T>;
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

        public T this[int index] {
            get {
                if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("index");
                return _subList[index + _offset];
            }
        }
        public int Count {
            get {
                if (_subList.Count < _skipExact) throw new InvalidOperationException("Skipped past end of list.");
                return Math.Max(0, _subList.Count - _skipExact - _skipElastic);
            }
        }

        public IEnumerator<T> GetEnumerator() {
            if (_subList.Count < _skipExact) throw new InvalidOperationException("Skipped past end of list.");
            return Enumerate();
        }
        private IEnumerator<T> Enumerate() {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            const int MaxPreviewItemCount = 10;
            var initialItems = String.Join(", ", System.Linq.Enumerable.Take(this, 10));
            var suffix = Count > MaxPreviewItemCount ? "..." : "]";
            return "Count: " + Count + ", Items: [" + initialItems + suffix;
        }
    }
}
