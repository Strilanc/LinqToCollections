using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
    [TestMethod]
    public void Empty() {
        ReadOnlyDictionary.Empty<int, int>().AssertDictionaryEquals(new Dictionary<int, int>());
    }
}
