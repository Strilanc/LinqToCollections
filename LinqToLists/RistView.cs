using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace LinqToLists {
    ///<summary>Exposes a contiguous subset of a readable list as a readable list.</summary>
    ///<remarks>
    ///Used to prevent multiple subview nesting from creating many levels of indirection.
    ///Assumes the underlying readable list does not change size.
    ///</remarks>
    [DebuggerDisplay("{ToString()}")]
    internal sealed class RistView<T> : IRist<T> {
        private readonly IRist<T> _subRist;
        private readonly int _offset;
        private readonly int _length;

        [ContractInvariantMethod()]
        private void ObjectInvariant() {
            Contract.Invariant(_subRist != null);
            Contract.Invariant(_offset >= 0);
            Contract.Invariant(_length >= 0);
            Contract.Invariant(_offset + _length <= _subRist.Count);
        }

        public RistView(IRist<T> subRist, int offset, int length) {
            Contract.Requires<ArgumentException>(subRist != null);
            Contract.Requires<ArgumentException>(offset >= 0);
            Contract.Requires<ArgumentException>(length >= 0);
            Contract.Requires<ArgumentException>(offset + length <= subRist.Count);
            Contract.Ensures(this.Count == length);

            this._subRist = subRist;
            this._offset = offset;
            this._length = length;
            Contract.Assume(this.Count == length);
        }

        public T this[int index] {
            get {
                Contract.Assume(_offset + index < _subRist.Count);
                return _subRist[_offset + index];
            }
        }
        public int Count { get { return _length; } }

        [Pure()]
        public RistView<T> NestedView(int relativeOffset, int relativeLength) {
            Contract.Requires<ArgumentException>(relativeOffset >= 0);
            Contract.Requires<ArgumentException>(relativeLength >= 0);
            Contract.Requires<ArgumentException>(relativeOffset + relativeLength <= Count);
            Contract.Ensures(Contract.Result<RistView<T>>() != null);
            Contract.Ensures(Contract.Result<RistView<T>>().Count == relativeLength);
            Contract.Assume(_offset + relativeOffset + relativeLength <= _subRist.Count);
            return new RistView<T>(_subRist, _offset + relativeOffset, relativeLength);
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < _length; i++)
                yield return _subRist[i + _offset];
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
