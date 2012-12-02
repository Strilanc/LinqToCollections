using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ListEmptyTest {
    [TestMethod]
    public void SingletonEmpty() {
        ReadOnlyList.Empty<int>().AssertReferenceEquals(ReadOnlyList.Empty<int>());
        ReadOnlyList.Empty<bool>().AssertReferenceEquals(ReadOnlyList.Empty<bool>());
        ReadOnlyList.Empty<int>().AssertListEquals(new int[0]);
    }
    [TestMethod]
    public void CountOptimizationsThroughEmpty() {
        ReadOnlyList.Empty<int>().Take(5).AssertReferenceEquals(ReadOnlyList.Empty<int>());
        ReadOnlyList.Empty<int>().Skip(5).AssertReferenceEquals(ReadOnlyList.Empty<int>());
    }
}
