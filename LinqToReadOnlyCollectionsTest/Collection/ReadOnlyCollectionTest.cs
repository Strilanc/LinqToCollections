using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass]
public class ReadOnlyCollectionTest {
    [TestMethod]
    public void Empty() {
        ReadOnlyCollection.Empty<int>().AssertCollectionEquals(new int[0]);
    }
}
