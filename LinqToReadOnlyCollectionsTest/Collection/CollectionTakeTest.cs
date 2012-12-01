using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class CollectionTakeTest {
    private static readonly IReadOnlyCollection<int> NullCollection = null;

    private static Func<IReadOnlyCollection<int>, int, IEnumerable<int>> ReferenceImplementation(bool last) {
        if (last) return (e, i) => Enumerable.Skip(e, e.Count - i);
        return Enumerable.Take;
    }
    private static Func<IReadOnlyCollection<int>, int, IReadOnlyCollection<int>> Taker(bool last) {
        if (last) return ReadOnlyCollection.TakeLast;
        return ReadOnlyCollection.Take;
    }

    private static IReadOnlyCollection<int> AssertTakesProperly(IReadOnlyCollection<int> collection, int n, bool last) {
        var s = Taker(last);
        var actual = s(collection, n);
        var expected = ReferenceImplementation(last)(collection, n).ToArray();
        actual.AssertCollectionEquals(expected);
        return actual;
    }

    [TestMethod]
    public void TakesLikeEnumerable() {
        foreach (var last in new[] { false, true }) {
            // fails on invalid arguments
            TestUtil.AssertThrows<ArgumentException>(() => Taker(last)(2.CRange(), -1));
            TestUtil.AssertThrows<ArgumentException>(() => Taker(last)(NullCollection, 0));

            // matches on valid arguments
            foreach (var i in 3.CRange()) {
                foreach (var n in 4.CRange()) {
                    // fixed size (allows extra optimizations)
                    AssertTakesProperly(Enumerable.Range(0, i).ToArray(), n, last);
                    // dynamic size (prevents some optimizations)
                    AssertTakesProperly(Enumerable.Range(0, i).ToList(), n, last);
                }
            }
        }
    }

    [TestMethod]
    public void TakesTrackMutations() {
        foreach (var last in new[] { false, true }) {
            var li = 3.CRange().ToList();
            var s = Taker(last)(li, 2);
            foreach (var action in new Action[] { () => li.Add(3), () => li.Remove(2), () => li.Remove(3), () => li.Clear() }) {
                action();
                s.AssertCollectionEquals(ReferenceImplementation(last)(li, 2));
            }
        }
    }

    [TestMethod]
    public void TakesSanely() {
        10.CRange().TakeLast(5).AssertCollectionEquals(5, 6, 7, 8, 9);
        10.CRange().Take(5).AssertCollectionEquals(0, 1, 2, 3, 4);
    }

    [TestMethod]
    public void TakeCombines() {
        5.CRange().TakeLast(3).Take(2).AssertCollectionEquals(2, 3);

        var r = 100.CRange();
        foreach (var i in 20.CRange().Reverse()) {
            var last = (i/3)%2 == 0;
            var r2 = AssertTakesProperly(r, i, last);
            r = r2;
        }
    }

    [TestMethod]
    public void TakeOptimizes() {
        var x = new[] { ReadOnlyCollection.Empty<int>(), 5.CRange(), new int[6], new List<int> { 1, 2 } };

        // taking none is empty
        foreach (var e in x) {
            e.Take(0).AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
            e.TakeLast(0).AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
        }

        // taking all is ignored
        foreach (var e in x) {
            foreach (var i in new[] { e.Count, e.Count + 1, 100 }) {
                var b = !(e is List<int>);
                ReferenceEquals(e.Take(i), e).AssertEquals(b);
                ReferenceEquals(e.TakeLast(i), e).AssertEquals(b);
            }
        }

        // double taking is merged
        foreach (var last in new[] { false, true }) {
            // scope inside an action to prevent the debugger from holding onto references
            new Action(() => {
                var root = 6.CRange();
                var transient = Taker(last)(root, 3);
                var result = Taker(last)(transient, 2);

                var weakRoot = root.WeakRef();
                var weakTransient = transient.WeakRef();
                root = null;
                transient = null;
                GC.Collect();

                weakRoot.AssertNotCollected();
                weakTransient.AssertCollected();
                GC.KeepAlive(result);
            }).Invoke();
        }
    }
}
