using System;
using System.Collections.Generic;

namespace Strilanc.LinqToCollections {
    internal sealed class CollectionCountCheck<T> : AbstractReadOnlyCollection<T>, IBoundedCount {
        public readonly IReadOnlyCollection<T> SubCollection;
        public readonly int EnforcedMininimumCount;
        public int? MaxCount { get; private set; }
        public int MinCount { get; private set; }

        private CollectionCountCheck(IReadOnlyCollection<T> subCollection, int enforcedMinimumCount) {
            this.SubCollection = subCollection;
            this.EnforcedMininimumCount = enforcedMinimumCount;
            this.MaxCount = subCollection.TryGetMaxCount();
            this.MinCount = Math.Max(enforcedMinimumCount, subCollection.TryGetMinCount());
        }
        public static IReadOnlyCollection<T> From(IReadOnlyCollection<T> subCollection, int enforcedMinimumCount) {
            if (subCollection == null) throw new ArgumentNullException("subCollection");
            if (enforcedMinimumCount < 0) throw new ArgumentOutOfRangeException("enforcedMinimumCount", "enforcedMinimumCount < 0");
            if (subCollection.Count < enforcedMinimumCount) throw new ArgumentOutOfRangeException("subCollection", "subCollection.Count < enforcedMinimumCount");

            // use something with random access if possible
            var li = subCollection as IReadOnlyList<T>;
            if (li != null)
                return ListCountCheck<T>.From(li, enforcedMinimumCount);

            // if the sub collection is guaranteed to be big enough, no need to do anything
            if (enforcedMinimumCount <= subCollection.TryGetMinCount()) return subCollection;

            // if the subcollection is another count check, we can replace it
            // (it must have a looser condition or else the minimum count check would have succeeded)
            var c = subCollection as CollectionCountCheck<T>;
            if (c != null)
                return From(c.SubCollection, enforcedMinimumCount);

            // if the subcollection is a Take then we can push the check downwards
            var tf = subCollection as CollectionTakeFirst<T>;
            if (tf != null)
                return CollectionTakeFirst<T>.From(From(tf.SubCollection, enforcedMinimumCount), tf.Amount);

            // if the subcollection is a TakeLast then we can push the check downwards
            var tl = subCollection as CollectionTakeLast<T>;
            if (tl != null)
                return CollectionTakeLast<T>.From(From(tl.SubCollection, enforcedMinimumCount), tl.Amount);

            // if the subcollection is a Skip then we can push the check downwards
            var s = subCollection as CollectionSkip<T>;
            if (s != null)
                return CollectionSkip<T>.From(From(s.SubCollection, enforcedMinimumCount + s.Amount), s.Offset, s.Amount);

            // if the subcollection is an adapter, we can push the check downwards
            var a = subCollection as CollectionAdapter<T>;
            if (a != null)
                return CollectionAdapter<T>.From(From(a.Collection, enforcedMinimumCount));

            return new CollectionCountCheck<T>(subCollection, enforcedMinimumCount);
        }

        public override int Count { get { return CheckCount(); } }

        public int CheckCount() {
            var n = SubCollection.Count;
            if (n < EnforcedMininimumCount) throw new InvalidOperationException("Collection doesn't have enough items.");
            return n;
        }
        public override IEnumerator<T> GetEnumerator() {
            CheckCount();
            return SubCollection.GetEnumerator();
        }
    }
}
