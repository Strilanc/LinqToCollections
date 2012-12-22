using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

[TestClass]
public class ReadOnlyDictionaryTest {
    [TestMethod]
    public void SelectValue() {
        5.Range()
            .ToDictionary(i => i, i => i * i)
            .SelectValue(e => e.Key + e.Value)
            .AssertDictionaryEquals(
                5.Range().ToDictionary(i => i, i => i * i + i));

        // no ambiguity with Enumerable.Select
        5.Range()
            .ToDictionary(i => i, i => i * i)
            .Select(e => e.Key)
            .OrderBy(e => e)
            .AssertSequenceEquals(5.Range());
    }
    [TestMethod]
    public void Empty() {
        ReadOnlyDictionary.Empty<int, int>().AssertDictionaryEquals(new Dictionary<int, int>());
    }
}
