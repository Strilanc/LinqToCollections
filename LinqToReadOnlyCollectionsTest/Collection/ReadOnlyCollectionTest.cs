using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class ReadOnlyCollectionTest {
    [TestMethod]
    public void Select() {
        TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Select(i => i));
        TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyCollection().Select((Func<int, int>)null));
        TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Select((e, i) => i));
        TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyCollection().Select((Func<int, int, int>)null));

        3.CRange().Select(i => i * i).AssertSequenceEquals(0, 1, 4);
        5.CRange().Select(i => i * i).AssertCollectionEquals(Enumerable.Range(0, 5).Select(i => i * i));
        10.CRange().Select(i => i + i).AssertCollectionEquals(Enumerable.Range(0, 10).Select(i => i + i));
        0.CRange().Select(i => i).AssertSequenceIsEmpty();
    }
    [TestMethod]
    public void Zip() {
        Assert.IsTrue(5.CRange().Zip(4.CRange(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
        Assert.IsTrue(4.CRange().Zip(5.CRange(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
        Assert.IsTrue(5.CRange().Zip(new[] { true, false, true }, (e1, e2) => e2 ? e1 : -e1).SequenceEqual(new[] { 0, -1, 2 }));
    }

    [TestMethod]
    public void Reverse() {
        TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Reverse());

        Assert.IsTrue(0.CRange().Reverse().SequenceEqual(new int[0]));
        Assert.IsTrue(5.CRange().Reverse().SequenceEqual(new[] { 4, 3, 2, 1, 0 }));
        Assert.IsTrue(4.CRange().Reverse().SequenceEqual(new[] { 3, 2, 1, 0 }));
    }

    [TestMethod]
    public void Repeat() {
        ReadOnlyCollection.Repeat("a", 20).AssertCollectionEquals(Enumerable.Repeat("a", 20));
        ReadOnlyCollection.Repeat("b", 10).AssertCollectionEquals(Enumerable.Repeat("b", 10));
    }
}
