using System.Collections.Generic;
using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class ListSkipTest {
    private static readonly IReadOnlyList<int> NullList = null;

    private static Func<IReadOnlyList<int>, int, IEnumerable<int>> ReferenceImplementation(bool last) {
        if (last) return (e, i) => Enumerable.Take(e, e.Count - i);
        return Enumerable.Skip;
    }
    private static Func<IReadOnlyList<int>, int, IReadOnlyList<int>> Skipper(bool last) {
        if (last) return ReadOnlyList.SkipLast;
        return ReadOnlyList.Skip;
    }

    private static IReadOnlyList<int> AssertSkipsProperly(IReadOnlyList<int> list, int n, bool last) {
        var s = Skipper(last);
        var actual = s(list, n);
        var expected = ReferenceImplementation(last)(list, n).ToArray();
        actual.AssertListEquals(expected);
        return actual;
    }

    [TestMethod]
    public void SkipsLikeEnumerable() {
        foreach (var last in new[] {false, true}) {
            // fails on invalid arguments
            TestUtil.AssertThrows<ArgumentException>(() => Skipper(last)(2.Range(), -1));
            TestUtil.AssertThrows<ArgumentException>(() => Skipper(last)(NullList, 0));

            // matches on valid arguments
            foreach (var i in 3.Range()) {
                foreach (var n in 4.Range()) {
                    // fixed size (allows extra optimizations)
                    AssertSkipsProperly(Enumerable.Range(0, i).ToArray(), n, last);
                    // dynamic size (prevents some optimizations)
                    AssertSkipsProperly(Enumerable.Range(0, i).ToList(), n, last);
                }
            }
        }
    }

    [TestMethod]
    public void SkipsTrackMutations() {
        foreach (var last in new[] {false, true}) {
            var li = 3.Range().ToList();
            var s = Skipper(last)(li, 2);
            foreach (var action in new Action[] {() => li.Add(3), () => li.Remove(2), () => li.Remove(3), () => li.Clear()}) {
                action();
                s.AssertListEquals(ReferenceImplementation(last)(li, 2));
            }
        }
    }

    [TestMethod]
    public void SkipsSanely() {
        10.Range().Skip(5).AssertListEquals(5, 6, 7, 8, 9);
        10.Range().SkipLast(5).AssertListEquals(0, 1, 2, 3, 4);
    }

    [TestMethod]
    public void SkipCombines() {
        5.Range().SkipLast(1).Skip(1).SkipLast(1).Skip(1).AssertListEquals(2);
        5.Range().Skip(1).SkipLast(1).Skip(1).SkipLast(1).Skip(1).Skip(1).SkipLast(1).AssertListIsEmpty();

        var r = 100.Range();
        foreach (var i in 8.Range()) {
            foreach (var last in new[] {false, true}) {
                var r2 = AssertSkipsProperly(r, i, last);
                r = r2;
            }
        }
    }

    [TestMethod]
    public void SkipOptimizes() {
        var x = new[] {ReadOnlyList.Empty<int>(), 5.Range(), new int[6], new List<int> {1, 2}};

        // skipping none is ignored
        foreach (var e in x) {
            e.Skip(0).AssertReferenceEquals(e);
            e.SkipLast(0).AssertReferenceEquals(e);
        }

        // skipping all is emptied
        foreach (var e in x) {
            foreach (var i in new[] {e.Count, e.Count + 1, 100}) {
                var b = !(e is List<int>);
                ReferenceEquals(e.Skip(i), ReadOnlyList.Empty<int>()).AssertEquals(b);
                ReferenceEquals(e.SkipLast(i), ReadOnlyList.Empty<int>()).AssertEquals(b);
            }
        }

        // double skipping is merged
        foreach (var last1 in new[] {false, true}) {
            foreach (var last2 in new[] {false, true}) {
                // scope inside an action to prevent the debugger from holding onto references
                new Action(() => {
                    var root = 6.Range();
                    var transient = Skipper(last1)(root, 2);
                    var result = Skipper(last2)(transient, 2);

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
