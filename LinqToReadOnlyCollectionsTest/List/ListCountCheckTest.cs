using System.Collections.Generic;
using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class ListCountCheckTest {
    [TestMethod]
    public void ExactTakeSkip() {
        // matches Take
        foreach (var i in 50.Range()) {
            50.Range().TakeExact(i).AssertListEquals(50.Range().Take(i));
            50.Range().TakeLastExact(i).AssertListEquals(50.Range().TakeLast(i));
            50.Range().SkipExact(i).AssertListEquals(50.Range().Skip(i));
            50.Range().SkipLastExact(i).AssertListEquals(50.Range().SkipLast(i));
        }

        // checks count
        TestUtil.AssertThrows<ArgumentException>(() => 50.Range().TakeExact(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.Range().TakeLastExact(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.Range().SkipExact(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.Range().SkipLastExact(51));

        // continues to check count
        var li = 51.Range().ToList();
        var ri = li.TakeExact(51);
        var ri2 = li.TakeLastExact(51);
        var ri3 = li.SkipExact(51);
        var ri4 = li.SkipLastExact(51);
        ri.AssertListEquals(li);
        ri2.AssertListEquals(li);
        ri3.AssertListIsEmpty();
        ri4.AssertListIsEmpty();
        li.Remove(1);
        ri.AssertListBroken();
        ri2.AssertListBroken();
        ri3.AssertListBroken();
        ri4.AssertListBroken();
    }
    [TestMethod]
    public void CountCheckOptimizesAcrossMostThings() {
        var r = 5.Range();
        r.TakeLastExact(5).AssertReferenceEquals(r);
        r.SkipLastExact(5).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.TakeExact(5).AssertReferenceEquals(r);
        r.SkipExact(5).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.TakeLastExact(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.SkipLastExact(0).AssertReferenceEquals(r);
        r.TakeExact(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.SkipExact(0).AssertReferenceEquals(r);

        var x = new Func<IReadOnlyList<int>, int, IReadOnlyList<int>>[] {
            ReadOnlyList.Skip, 
            ReadOnlyList.SkipLastExact, 
            ReadOnlyList.TakeExact, 
            ReadOnlyList.TakeLastExact,
            (e,i) => (IReadOnlyList<int>)e.AsIList()
        };
        foreach (var e in x) {
            foreach (var e2 in x) {
                new Action(() => {
                    var root = 6.Range();
                    var transient = e(root, 2);
                    var result = e2(root, 1);
                    // should combine the takes/skips AND the checks
                    // resulting in the transient list not being referenced

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
