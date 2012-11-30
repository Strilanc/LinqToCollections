using System.Collections.Generic;
using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class ListTakeTest {
    private static readonly IReadOnlyList<int> NullList = null;

    private static Func<IReadOnlyList<int>, int, IEnumerable<int>> ReferenceImplementation(bool last, bool exact) {
        if (exact)
            return (e, i) => {
                if (e.Count < i) throw new ArgumentOutOfRangeException();
                return ReferenceImplementation(last, false)(e, i);
            };
        if (last) return (e, i) => Enumerable.Skip(e, e.Count - i);
        return Enumerable.Take;
    }
    private static Func<IReadOnlyList<int>, int, IReadOnlyList<int>> Taker(bool last, bool exact) {
        if (last) {
            if (exact) return ReadOnlyList.TakeLastExact;
            return ReadOnlyList.TakeLast;
        }
        if (exact) return ReadOnlyList.TakeExact;
        return ReadOnlyList.Take;
    }

    private static IReadOnlyList<int> AssertTakesProperly(IReadOnlyList<int> list, int n, bool last, bool exact) {
        var s = Taker(last, exact);
        if (exact && n > list.Count) {
            TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => s(list, n));
            return null;
        }
        var actual = s(list, n);
        var expected = ReferenceImplementation(last, exact)(list, n).ToArray();
        actual.AssertListEquals(expected);
        return actual;
    }

    [TestMethod]
    public void TakesLikeEnumerable() {
        foreach (var last in new[] { false, true }) {
            foreach (var exact in new[] { false, true }) {
                // fails on invalid arguments
                TestUtil.AssertThrows<ArgumentException>(() => Taker(last, exact)(2.Range(), -1));
                TestUtil.AssertThrows<ArgumentException>(() => Taker(last, exact)(NullList, 0));

                // matches on valid arguments
                foreach (var i in 3.Range()) {
                    foreach (var n in 4.Range()) {
                        // fixed size (allows extra optimizations)
                        AssertTakesProperly(Enumerable.Range(0, i).ToArray(), n, last, exact);
                        // dynamic size (prevents some optimizations)
                        AssertTakesProperly(Enumerable.Range(0, i).ToList(), n, last, exact);
                    }
                }
            }
        }
    }

    [TestMethod]
    public void TakesTrackMutations() {
        foreach (var last in new[] { false, true }) {
            foreach (var exact in new[] { false, true }) {
                var li = 3.Range().ToList();
                var s = Taker(last, exact)(li, 2);
                foreach (var action in new Action[] { () => li.Add(3), () => li.Remove(2), () => li.Remove(3), () => li.Clear() }) {
                    action();
                    if (exact && li.Count < 2) {
                        s.AssertListBroken();
                    } else {
                        s.AssertListEquals(ReferenceImplementation(last, exact)(li, 2));
                    }
                }
            }
        }
    }

    [TestMethod]
    public void TakesSanely() {
        10.Range().TakeLast(5).AssertListEquals(5, 6, 7, 8, 9);
        10.Range().TakeLastExact(5).AssertListEquals(5, 6, 7, 8, 9);
        10.Range().Take(5).AssertListEquals(0, 1, 2, 3, 4);
        10.Range().TakeExact(5).AssertListEquals(0, 1, 2, 3, 4);
    }

    [TestMethod]
    public void TakeCombines() {
        5.Range().TakeLastExact(3).TakeExact(2).AssertListEquals(2, 3);

        foreach (var last in new[] { false, true }) {
            var r = 100.Range();
            foreach (var i in 20.Range().Reverse()) {
                var exact = i%2 == 0;
                var r2 = AssertTakesProperly(r, i, last, exact);
                r = r2;
            }
        }
    }

    [TestMethod]
    public void TakeOptimizes() {
        var x = new[] { ReadOnlyList.Empty<int>(), 5.Range(), new int[6], new List<int> { 1, 2 } };

        // taking none is empty
        foreach (var e in x) {
            e.Take(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
            e.TakeExact(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
            e.TakeLast(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
            e.TakeLastExact(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        }

        // taking all is ignored
        foreach (var e in x) {
            foreach (var i in new[] { e.Count, e.Count + 1, 100 }) {
                var b = !(e is List<int>);
                ReferenceEquals(e.Take(i), e).AssertEquals(b);
                ReferenceEquals(e.TakeLast(i), e).AssertEquals(b);
                if (i == e.Count) {
                    ReferenceEquals(e.TakeExact(i), e).AssertEquals(b);
                    ReferenceEquals(e.TakeLastExact(i), e).AssertEquals(b);
                }
            }
        }

        // double skipping is merged
        foreach (var last in new[] { false, true }) {
            foreach (var exact1 in new[] { false, true }) {
                foreach (var exact2 in new[] { false, true }) {
                    // scope inside an action to prevent the debugger from holding onto references
                    new Action(() => {
                        var root = 6.Range();
                        var transient = Taker(last, exact1)(root, 3);
                        var result = Taker(last, exact2)(transient, 2);

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
    }
}
