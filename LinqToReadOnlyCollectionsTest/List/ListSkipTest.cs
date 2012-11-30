using System.Collections.Generic;
using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class ListSkipTest {
    private static readonly IReadOnlyList<int> NullList = null;

    private static Func<IReadOnlyList<int>, int, IEnumerable<int>> ReferenceImplementation(bool last, bool exact) {
        if (exact)
            return (e, i) => {
                if (e.Count < i) throw new ArgumentOutOfRangeException();
                return ReferenceImplementation(last, false)(e, i);
            };
        if (last) return (e, i) => Enumerable.Take(e, e.Count - i);
        return Enumerable.Skip;
    }
    private static Func<IReadOnlyList<int>, int, IReadOnlyList<int>> Skipper(bool last, bool exact) {
        if (last) {
            if (exact) return ReadOnlyList.SkipLastExact;
            return ReadOnlyList.SkipLast;
        }
        if (exact) return ReadOnlyList.SkipExact;
        return ReadOnlyList.Skip;
    }

    private static IReadOnlyList<int> AssertSkipsProperly(IReadOnlyList<int> list, int n, bool last, bool exact) {
        var s = Skipper(last, exact);
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
    public void SkipsLikeEnumerable() {
        foreach (var last in new[] {false, true}) {
            foreach (var exact in new[] {false, true}) {
                // fails on invalid arguments
                TestUtil.AssertThrows<ArgumentException>(() => Skipper(last, exact)(2.Range(), -1));
                TestUtil.AssertThrows<ArgumentException>(() => Skipper(last, exact)(NullList, 0));

                // matches on valid arguments
                foreach (var i in 3.Range()) {
                    foreach (var n in 4.Range()) {
                        // fixed size (allows extra optimizations)
                        AssertSkipsProperly(Enumerable.Range(0, i).ToArray(), n, last, exact);
                        // dynamic size (prevents some optimizations)
                        AssertSkipsProperly(Enumerable.Range(0, i).ToList(), n, last, exact);
                    }
                }
            }
        }
    }

    [TestMethod]
    public void SkipsTrackMutations() {
        foreach (var last in new[] {false, true}) {
            foreach (var exact in new[] {false, true}) {
                var li = 3.Range().ToList();
                var s = Skipper(last, exact)(li, 2);
                foreach (var action in new Action[] {() => li.Add(3), () => li.Remove(2), () => li.Remove(3), () => li.Clear()}) {
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
    public void SkipsSanely() {
        10.Range().Skip(5).AssertListEquals(5, 6, 7, 8, 9);
        10.Range().SkipExact(5).AssertListEquals(5, 6, 7, 8, 9);
        10.Range().SkipLast(5).AssertListEquals(0, 1, 2, 3, 4);
        10.Range().SkipLastExact(5).AssertListEquals(0, 1, 2, 3, 4);
    }

    [TestMethod]
    public void SkipCombines() {
        5.Range().SkipExact(1).SkipLast(1).Skip(1).SkipLastExact(1).AssertListEquals(2);
        5.Range().SkipExact(1).SkipLast(1).Skip(1).SkipLastExact(1).SkipExact(1).Skip(1).SkipLast(1).AssertListIsEmpty();

        var r = 100.Range();
        foreach (var i in 4.Range()) {
            foreach (var last in new[] {false, true}) {
                foreach (var exact in new[] {false, true}) {
                    var r2 = AssertSkipsProperly(r, i, last, exact);
                    r = r2;
                }
            }
        }
    }

    [TestMethod]
    public void SkipOptimizes() {
        var x = new[] {ReadOnlyList.Empty<int>(), 5.Range(), new int[6], new List<int> {1, 2}};

        // skipping none is ignored
        foreach (var e in x) {
            e.Skip(0).AssertReferenceEquals(e);
            e.SkipExact(0).AssertReferenceEquals(e);
            e.SkipLast(0).AssertReferenceEquals(e);
            e.SkipLastExact(0).AssertReferenceEquals(e);
        }

        // skipping all is emptied
        foreach (var e in x) {
            foreach (var i in new[] {e.Count, e.Count + 1, 100}) {
                var b = !(e is List<int>);
                ReferenceEquals(e.Skip(i), ReadOnlyList.Empty<int>()).AssertEquals(b);
                ReferenceEquals(e.SkipLast(i), ReadOnlyList.Empty<int>()).AssertEquals(b);
                if (i == e.Count) {
                    ReferenceEquals(e.SkipExact(i), ReadOnlyList.Empty<int>()).AssertEquals(b);
                    ReferenceEquals(e.SkipLastExact(i), ReadOnlyList.Empty<int>()).AssertEquals(b);
                }
            }
        }

        // double skipping is merged
        foreach (var last1 in new[] {false, true}) {
            foreach (var exact1 in new[] {false, true}) {
                foreach (var last2 in new[] {false, true}) {
                    foreach (var exact2 in new[] {false, true}) {
                        // scope inside an action to prevent the debugger from holding onto references
                        new Action(() => {
                            var root = 6.Range();
                            var transient = Skipper(last1, exact1)(root, 2);
                            var result = Skipper(last2, exact2)(transient, 2);

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
}
