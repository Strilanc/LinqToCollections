using System;
using System.Collections.Generic;
using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ListAdapterTest {
    [TestMethod]
    public void AsReadOnlyList() {
        // actually read-only, not just "if you don't cast" read-only
        foreach (var e in new IList<int>[] { new List<int>(), new int[5] }) {
            e.AsReadOnlyList().AssertReferenceDoesNotEqual(e);
            e.AsReadOnlyList().AssertListEquals(e);
        }
    }
    [TestMethod]
    public void AsIList() {
        TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).AsIList());

        var li = 5.Range().AsIList();
        li.AssertSequenceEquals(5.Range());
        li.Count.AssertEquals(5);
        li.IsReadOnly.AssertIsTrue();
        foreach (var i in 10.Range().Select(e => e - 1)) {
            li.Contains(i).AssertEquals(i >= 0 && i < 5);
            li.IndexOf(i).AssertEquals(i >= 0 && i < 5 ? i : -1);
        }
        var d = new int[10];
        li.CopyTo(d, 2);
        d.AssertSequenceEquals(0, 0, 0, 1, 2, 3, 4, 0, 0, 0);
        
        TestUtil.AssertThrows<NotSupportedException>(() => li.Add(0));
        TestUtil.AssertThrows<NotSupportedException>(() => li.Remove(0));
        TestUtil.AssertThrows<NotSupportedException>(() => li.Insert(0, 0));
        TestUtil.AssertThrows<NotSupportedException>(() => li.RemoveAt(0));
        TestUtil.AssertThrows<NotSupportedException>(() => li.Clear());
        TestUtil.AssertThrows<NotSupportedException>(() => li[0] = 0);
    }
    [TestMethod]
    public void AdapterWrapResistance() {
        // readonly to general has no effect when already general
        var li = new List<int> { 0, 1, 2 };
        li.AsIList().AssertReferenceEquals(li);
        // general to readonly unwraps preceeding readonly to general
        var r5 = 5.Range();
        r5.AsIList().AsReadOnlyList().AssertReferenceEquals(r5);
        // when general list is marked readonly, no wrapping is done
        var ri = li.AsReadOnly();
        ri.AsReadOnlyList().AssertReferenceEquals(ri);
    }
    [TestMethod]
    public void CountOptimizationsThroughAdapter() {
        var ri = new int[5].AsReadOnlyList();
        ri.Skip(10).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        ri.Take(10).AssertReferenceEquals(ri);
    }
}
