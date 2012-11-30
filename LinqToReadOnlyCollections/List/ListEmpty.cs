using System;
using System.Collections.Generic;

namespace LinqToReadOnlyCollections.List {
    internal sealed class ListEmpty<T> : AbstractReadOnlyList<T>, IPotentialMaxCount {
        public override T this[int index] { get { throw new ArgumentOutOfRangeException("index"); } }
        public override int Count { get { return 0; } }
        public override IEnumerator<T> GetEnumerator() { yield break; }
        int? IPotentialMaxCount.MaxCount { get { return 0; } }
    }
}
