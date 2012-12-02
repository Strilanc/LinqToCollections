using System;
using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CollectionAdapterTest {
    [TestMethod]
    public void AsReadOnlyCollection() {
        // actually read-only, not just "if you don't cast" read-only
        foreach (var e in new ICollection<int>[] { new HashSet<int>(), new int[5] }) {
            e.AsReadOnlyCollection().AssertReferenceDoesNotEqual(e);
            e.AsReadOnlyCollection().AssertCollectionEquals(e);
        }
    }
    [TestMethod]
    public void AsICollection() {
        TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).AsICollection());

        var li = 5.CRange().AsICollection();
        li.AssertSequenceEquals(5.CRange());
        li.Count.AssertEquals(5);
        li.IsReadOnly.AssertIsTrue();
        foreach (var i in 10.Range().Select(e => e - 1)) {
            li.Contains(i).AssertEquals(i >= 0 && i < 5);
        }
        var d = new int[10];
        li.CopyTo(d, 2);
        d.AssertSequenceEquals(0, 0, 0, 1, 2, 3, 4, 0, 0, 0);
        
        TestUtil.AssertThrows<NotSupportedException>(() => li.Add(0));
        TestUtil.AssertThrows<NotSupportedException>(() => li.Remove(0));
        TestUtil.AssertThrows<NotSupportedException>(() => li.Clear());
    }
    [TestMethod]
    public void AdapterWrapResistance() {
        // readonly to general has no effect when already general
        var li = new List<int> { 0, 1, 2 };
        li.AsICollection().AssertReferenceEquals(li);
        // general to readonly unwraps preceeding readonly to general
        var r5 = 5.CRange();
        r5.AsICollection().AsReadOnlyCollection().AssertReferenceEquals(r5);
        // when general collection is marked readonly, no wrapping is done
        var ri = li.AsReadOnly();
        ri.AsReadOnlyCollection().AssertReferenceEquals(ri);
    }
}
