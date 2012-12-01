using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class AnonymousReadOnlyListTest {
    [TestMethod]
    public void AnonListChecksArguments() {
        TestUtil.AssertThrows<ArgumentException>(() => new AnonymousReadOnlyList<int>(null, i => i));
        TestUtil.AssertThrows<ArgumentException>(() => new AnonymousReadOnlyList<int>(() => 0, null));
        TestUtil.AssertThrows<ArgumentException>(() => new AnonymousReadOnlyList<int>(0, null));
        TestUtil.AssertThrows<ArgumentException>(() => new AnonymousReadOnlyList<int>(-1, i => i));
    }
    [TestMethod]
    public void EmptyAnonList() {
        var x = new[] {
            new AnonymousReadOnlyList<int>(() => 0, i => { throw new InvalidCastException(); }),
            new AnonymousReadOnlyList<int>(0, i => { throw new InvalidCastException(); })
        };
        foreach (var r in x) {
            r.Count.AssertEquals(0);
            r.AssertSequenceEquals(new int[0]);
            TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => r[0]);
        }
    }
    [TestMethod]
    public void FixedAnonList() {
        var x = new[] {
            new AnonymousReadOnlyList<int>(() => 3, i => i * i),
            new AnonymousReadOnlyList<int>(3, i => i * i)
        };
        foreach (var r in x) {
            r.Count.AssertEquals(3);
            r[0].AssertEquals(0);
            r[1].AssertEquals(1);
            r[2].AssertEquals(4);
            r.AssertSequenceEquals(new[] {0, 1, 4});
            TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => r[-1]);
            TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => r[3]);
        }
    }
    [TestMethod]
    public void DynamicAnonList() {
        var n = 0;
        var t2 = new AnonymousReadOnlyList<int>(() => n, i => i + 10);
        t2.Count.AssertEquals(0);
        t2.AssertSequenceEquals(new int[0]);
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => t2[0]);
            
        n = 1;
        t2.Count.AssertEquals(1);
        t2[0].AssertEquals(10);
        t2.AssertSequenceEquals(new[] { 10 });
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => t2[1]);
    }
    [TestMethod]
    public void AnonListEfficientIterator() {
        var li = new List<int> {0};
        var ri1 = new AnonymousReadOnlyList<int>(() => li.Count, i => li[i]);
        var ri2 = new AnonymousReadOnlyList<int>(() => li.Count, i => li[i], li);
        using (var e1 = ri1.GetEnumerator()) {
            using (var e2 = ri2.GetEnumerator()) {
                e1.MoveNext().AssertIsTrue();
                e2.MoveNext().AssertIsTrue();
                li.Add(1);
                TestUtil.AssertThrows<InvalidOperationException>(() => e2.MoveNext()); // list modified during iteration
                e1.MoveNext().AssertIsTrue(); // not using list iterator
            }
        }
    }
}
