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
    public void CountCheckOptimizesAwayWhenZero() {
        var r = 5.Range().ToList();
        r.TakeLastRequire(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.SkipLastRequire(0).AssertReferenceEquals(r);
        r.TakeRequire(0).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.SkipRequire(0).AssertReferenceEquals(r);
    }
    [TestMethod]
    public void CountCheckOptimizesAwayWhenSizeIsKnown() {
        var r = 5.Range();

        r.TakeLastRequire(5).AssertReferenceEquals(r);
        r.TakeRequire(5).AssertReferenceEquals(r);
        r.SkipLastRequire(5).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        r.SkipRequire(5).AssertReferenceEquals(ReadOnlyList.Empty<int>());

        TestUtil.AssertThrows(() => r.TakeRequire(6));
        TestUtil.AssertThrows(() => r.TakeLastRequire(6));
        TestUtil.AssertThrows(() => r.SkipRequire(6));
        TestUtil.AssertThrows(() => r.SkipLastRequire(6));
    }
    [TestMethod]
    public void CountCheckDoesNotOptimizeAwayWhenSizeIsVariable() {
        var r = 5.Range().ToList();
        r.TakeLastRequire(5).AssertReferenceDoesNotEqual(r);
        r.SkipLastRequire(5).AssertReferenceDoesNotEqual(ReadOnlyList.Empty<int>());
        r.TakeRequire(5).AssertReferenceDoesNotEqual(r);
        r.SkipRequire(5).AssertReferenceDoesNotEqual(ReadOnlyList.Empty<int>());
    }

    private static T Match<T, TIn, T1, T2, T3, T4>(TIn input, Func<T1, T> match1, Func<T2, T> match2, Func<T3, T> match3, Func<T4, T> match4) {
        if (input is T1) return match1((T1)(object)input);
        if (input is T2) return match2((T2)(object)input);
        if (input is T3) return match3((T3)(object)input);
        if (input is T4) return match4((T4)(object)input);
        Assert.Fail(input.GetType().ToString());
        throw new InvalidProgramException();
    }
    [TestMethod]
    public void CountCheckOptimizesWhenDoubledUp() {
        var reqs = new Func<IReadOnlyList<int>, int, IReadOnlyList<int>>[] {
            ReadOnlyList.SkipRequire, 
            ReadOnlyList.SkipLastRequire, 
            ReadOnlyList.TakeRequire, 
            ReadOnlyList.TakeLastRequire
        };
        foreach (var req1 in reqs) {
            foreach (var req2 in reqs) {
                var root = 6.Range().ToList();
                var r = req2(req1(root, 3), 2);

                var s = Match(r,
                              (ListCountCheck<int> e) => e,
                              (ListSkip<int> e) => e.SubList,
                              (ListTakeFirst<int> e) => e.SubList,
                              (ListTakeLast<int> e) => e.SubList);
                var s2 = (ListCountCheck<int>)Match(s,
                                                    (ListCountCheck<int> e) => e,
                                                    (ListSkip<int> e) => e.SubList,
                                                    (ListTakeFirst<int> e) => e.SubList,
                                                    (ListTakeLast<int> e) => e.SubList);
                s2.SubList.AssertReferenceEquals(root);
            }
        }
    }
}
