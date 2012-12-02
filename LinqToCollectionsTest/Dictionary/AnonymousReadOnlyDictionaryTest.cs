using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AnonymousReadOnlyDictionaryTest {
    [TestMethod]
    public void AnonymousDictionaryConstructors() {
        var r = new AnonymousReadOnlyDictionary<int, int>(
            () => 5,
            5.Range(),
            (int k, out int v) => {
                v = k;
                return k >= 0 && k < 5;
            });
        new AnonymousReadOnlyDictionary<int, int>(
            5.Range(),
            (int k, out int v) => {
                v = k;
                return k >= 0 && k < 5;
            }).AssertDictionaryEquals(r);
        r.Count.AssertEquals(5);
        r.Keys.AssertSequenceEquals(5.Range());
        int x;
        r.TryGetValue(-1, out x).AssertIsFalse();
        r.TryGetValue(5, out x).AssertIsFalse();
        for (var i = 0; i < 5; i++) {
            r.TryGetValue(i, out x).AssertIsTrue();
            x.AssertEquals(i);
        }
    }
}
