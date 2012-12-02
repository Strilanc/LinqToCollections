using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class ListCountCheckTest {
    [TestMethod]
    public void ExactTakeSkip() {
        // matches Take
        foreach (var i in 50.Range()) {
            50.Range().TakeRequire(i).AssertListEquals(50.Range().Take(i));
            50.Range().TakeLastRequire(i).AssertListEquals(50.Range().TakeLast(i));
            50.Range().SkipRequire(i).AssertListEquals(50.Range().Skip(i));
            50.Range().SkipLastRequire(i).AssertListEquals(50.Range().SkipLast(i));
        }

        // checks count
        TestUtil.AssertThrows<ArgumentException>(() => 50.Range().TakeRequire(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.Range().TakeLastRequire(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.Range().SkipRequire(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.Range().SkipLastRequire(51));

        // continues to check count
        var li = 51.Range().ToList();
        var ri = li.TakeRequire(51);
        var ri2 = li.TakeLastRequire(51);
        var ri3 = li.SkipRequire(51);
        var ri4 = li.SkipLastRequire(51);
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
        r.TakeLastRequire(5).AssertReferenceEquals(r);
        r.SkipLastRequire(5).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.TakeRequire(5).AssertReferenceEquals(r);
        r.SkipRequire(5).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.TakeLastRequire(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.SkipLastRequire(0).AssertReferenceEquals(r);
        r.TakeRequire(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.SkipRequire(0).AssertReferenceEquals(r);

        var x = new Func<IReadOnlyList<int>, int, IReadOnlyList<int>>[] {
            ReadOnlyList.Skip, 
            ReadOnlyList.SkipLastRequire, 
            ReadOnlyList.TakeRequire, 
            ReadOnlyList.TakeLastRequire,
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
