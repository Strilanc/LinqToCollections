using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CollectionEmptyTest {
    [TestMethod]
    public void SingletonEmpty() {
        ReadOnlyCollection.Empty<int>().AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
        ReadOnlyCollection.Empty<bool>().AssertReferenceEquals(ReadOnlyCollection.Empty<bool>());
        ReadOnlyCollection.Empty<int>().AssertCollectionEquals(new int[0]);
    }
    [TestMethod]
    public void CountOptimizationsThroughEmpty() {
        ReadOnlyCollection.Empty<int>().Take(5).AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
        ReadOnlyCollection.Empty<int>().Skip(5).AssertReferenceEquals(ReadOnlyCollection.Empty<int>());
    }
}
