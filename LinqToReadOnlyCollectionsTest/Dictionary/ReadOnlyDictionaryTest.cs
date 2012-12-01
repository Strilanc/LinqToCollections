using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using LinqToReadOnlyCollections.Dictionary;

[TestClass]
public class ReadOnlyDictionaryTest {
    [TestMethod]
    public void Select() {
        5.Range()
            .ToDictionary(i => i, i => i * i)
            .Select(e => e.Key + e.Value)
            .AssertDictionaryEquals(
                5.Range().ToDictionary(i => i, i => i * i + i));
    }
}
