using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class CollectionCountCheckTest {
    [TestMethod]
    public void ExactTakeSkip() {
        // matches Take
        foreach (var i in 50.CRange()) {
            50.CRange().TakeRequire(i).AssertCollectionEquals(50.CRange().Take(i));
            50.CRange().TakeLastRequire(i).AssertCollectionEquals(50.CRange().TakeLast(i));
            50.CRange().SkipRequire(i).AssertCollectionEquals(50.CRange().Skip(i));
            50.CRange().SkipLastRequire(i).AssertCollectionEquals(50.CRange().SkipLast(i));
        }

        // checks count
        TestUtil.AssertThrows<ArgumentException>(() => 50.CRange().TakeRequire(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.CRange().TakeLastRequire(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.CRange().SkipRequire(51));
        TestUtil.AssertThrows<ArgumentException>(() => 50.CRange().SkipLastRequire(51));

        // continues to check count
        var li = new HashSet<int>(51.CRange());
        var ri = li.AsReadOnlyCollection().TakeRequire(51);
        var ri2 = li.AsReadOnlyCollection().TakeLastRequire(51);
        var ri3 = li.AsReadOnlyCollection().SkipRequire(51);
        var ri4 = li.AsReadOnlyCollection().SkipLastRequire(51);
        ri.AssertCollectionEquals(li);
        ri2.AssertCollectionEquals(li);
        ri3.AssertCollectionIsEmpty();
        ri4.AssertCollectionIsEmpty();
        li.Remove(1);
        ri.AssertCollectionBroken();
        ri2.AssertCollectionBroken();
        ri3.AssertCollectionBroken();
        ri4.AssertCollectionBroken();
    }
    [TestMethod]
    public void CountCheckOptimizesAcrossMostThings() {
        var r = 5.CRange();
        r.TakeLastRequire(5).AssertReferenceEquals(r);
        r.SkipLastRequire(5).AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
        r.TakeRequire(5).AssertReferenceEquals(r);
        r.SkipRequire(5).AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
        r.TakeLastRequire(0).AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
        r.SkipLastRequire(0).AssertReferenceEquals(r);
        r.TakeRequire(0).AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
        r.SkipRequire(0).AssertReferenceEquals(r);

        var x = new Func<IReadOnlyCollection<int>, int, IReadOnlyCollection<int>>[] {
            ReadOnlyCollection.Skip, 
            ReadOnlyCollection.SkipLastRequire, 
            ReadOnlyCollection.TakeRequire, 
            ReadOnlyCollection.TakeLastRequire,
            (e,i) => (IReadOnlyCollection<int>)e.AsICollection()
        };
        foreach (var e in x) {
            foreach (var e2 in x) {
                new Action(() => {
                    var root = 6.CRange();
                    var transient = e(root, 2);
                    var result = e2(root, 1);
                    // should combine the takes/skips AND the checks
                    // resulting in the transient collection not being referenced

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
