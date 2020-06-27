using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LinqToCollectionsTest")]
namespace Strilanc.LinqToCollections {
    internal sealed class ListCountCheck<T> : AbstractReadOnlyList<T>, IBoundedCount {
        public readonly IReadOnlyList<T> SubList;
        public readonly int EnforcedMininimumCount;
        public int? MaxCount { get; private set; }
        public int MinCount { get; private set; }

        private ListCountCheck(IReadOnlyList<T> subList, int enforcedMinimumCount) {
            this.SubList = subList;
            this.EnforcedMininimumCount = enforcedMinimumCount;
            this.MaxCount = subList.TryGetMaxCount();
            this.MinCount = Math.Max(enforcedMinimumCount, subList.TryGetMinCount());
        }
        public static IReadOnlyList<T> From(IReadOnlyList<T> subList, int enforcedMinimumCount) {
            if (subList == null) throw new ArgumentNullException("subList");
            if (enforcedMinimumCount < 0) throw new ArgumentOutOfRangeException("enforcedMinimumCount", "enforcedMinimumCount < 0");
            if (subList.Count < enforcedMinimumCount) throw new ArgumentOutOfRangeException("subList", "subList.Count < enforcedMinimumCount");

            // if the sub list is guaranteed to be big enough, no need to do anything
            if (enforcedMinimumCount <= subList.TryGetMinCount()) return subList;

            // if the sublist is another count check, we can replace it
            // (it must have a looser condition or else the minimum count check would have succeeded)
            var c = subList as ListCountCheck<T>;
            if (c != null)
                return From(c.SubList, enforcedMinimumCount);

            // if the sublist is a Take then we can push the check downwards
            var tf = subList as ListTakeFirst<T>;
            if (tf != null)
                return ListTakeFirst<T>.From(From(tf.SubList, enforcedMinimumCount), tf.Amount);

            // if the sublist is a TakeLast then we can push the check downwards
            var tl = subList as ListTakeLast<T>;
            if (tl != null)
                return ListTakeLast<T>.From(From(tl.SubList, enforcedMinimumCount), tl.Amount);

            // if the sublist is a Skip then we can push the check downwards
            var s = subList as ListSkip<T>;
            if (s != null)
                return ListSkip<T>.From(From(s.SubList, enforcedMinimumCount + s.Amount), s.Offset, s.Amount);

            // if the sublist is an adapter, we can push the check downwards
            var a = subList as ListAdapter<T>;
            if (a != null)
                return ListAdapter<T>.From(From(a.List, enforcedMinimumCount));

            return new ListCountCheck<T>(subList, enforcedMinimumCount);
        }

        public override T this[int index] { 
            get {
                CheckCount();
                return SubList[index];
            } 
        }
        public override int Count { get { return CheckCount(); } }

        public int CheckCount() {
            var n = SubList.Count;
            if (n < EnforcedMininimumCount) throw new InvalidOperationException("List doesn't have enough items.");
            return n;
        }
        public override IEnumerator<T> GetEnumerator() {
            CheckCount();
            return SubList.GetEnumerator();
        }
    }
}
