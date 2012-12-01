using System.Collections.Generic;
using System.Linq;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class AnonymousReadOnlyCollectionTest {
    [TestMethod]
    public void AnonCollectionChecksArguments() {
        TestUtil.AssertThrows<ArgumentException>(() => new AnonymousReadOnlyCollection<int>(null, Enumerable.Empty<int>().GetEnumerator));
        TestUtil.AssertThrows<ArgumentException>(() => new AnonymousReadOnlyCollection<int>(() => 0, null));
        TestUtil.AssertThrows<ArgumentException>(() => new AnonymousReadOnlyCollection<int>(0, null));
        TestUtil.AssertThrows<ArgumentException>(() => new AnonymousReadOnlyCollection<int>(-1, Enumerable.Empty<int>().GetEnumerator));
    }
    [TestMethod]
    public void EmptyAnonCollection() {
        var x = new[] {
            new AnonymousReadOnlyCollection<int>(() => 0, Enumerable.Empty<int>().GetEnumerator),
            new AnonymousReadOnlyCollection<int>(0, Enumerable.Empty<int>().GetEnumerator)
        };
        foreach (var r in x) {
            r.Count.AssertEquals(0);
            r.AssertSequenceEquals(new int[0]);
        }
    }
    [TestMethod]
    public void FixedAnonCollection() {
        var x = new[] {
            new AnonymousReadOnlyCollection<int>(() => 3, 3.CRange().Select(i => i * i).GetEnumerator),
            new AnonymousReadOnlyCollection<int>(3, 3.CRange().Select(i => i * i).GetEnumerator)
        };
        foreach (var r in x) {
            r.Count.AssertEquals(3);
            r.AssertSequenceEquals(new[] {0, 1, 4});
        }
    }
    [TestMethod]
    public void DynamicAnonCollection() {
        var n = 0;
        var t2 = new AnonymousReadOnlyCollection<int>(() => n, () => n.CRange().Select(i => i + 10).GetEnumerator());
        t2.Count.AssertEquals(0);
        t2.AssertSequenceEquals(new int[0]);
            
        n = 1;
        t2.Count.AssertEquals(1);
        t2.AssertSequenceEquals(new[] { 10 });
    }
    [TestMethod]
    public void AnonCollectionEfficientIterator() {
        var li = new HashSet<int> {0};
        var ri1 = new AnonymousReadOnlyCollection<int>(() => li.Count, () => li.ToArray().AsEnumerable().GetEnumerator());
        var ri2 = new AnonymousReadOnlyCollection<int>(() => li.Count, ((IEnumerable<int>)li).GetEnumerator);
        using (var e1 = ri1.GetEnumerator()) {
            using (var e2 = ri2.GetEnumerator()) {
                e1.MoveNext().AssertIsTrue();
                e2.MoveNext().AssertIsTrue();
                li.Add(1);
                TestUtil.AssertThrows<InvalidOperationException>(() => e2.MoveNext()); // collection modified during iteration
                e1.MoveNext().AssertIsTrue(); // not using collection iterator
            }
        }
    }
}
