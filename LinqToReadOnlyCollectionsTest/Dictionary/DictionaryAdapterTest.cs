using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using LinqToReadOnlyCollections.Dictionary;

[TestClass]
public class DictionaryAdapterTest {
    private static readonly IReadOnlyDictionary<int, int> ReadOnlyDic = new AnonymousReadOnlyDictionary<int, int>(
        10.Range(),
        (int k, out int v) => {
            v = k;
            return v >= 0 && v < 10;
        });

    [TestMethod]
    public void AsReadOnlyDictionary() {
        var d = new Dictionary<int, int> { { 2, 4 }, { 3, 9 } };
        var r = d.AsReadOnlyDictionary();
        d.AssertReferenceDoesNotEqual(r);
        d.AssertDictionaryEquals(r);
    }
    [TestMethod]
    public void AsIDictionary() {
        var d = ReadOnlyDic.AsIDictionary();
        d.AssertReferenceDoesNotEqual(ReadOnlyDic);
        ReadOnlyDic.AssertDictionaryEquals(d);
    }
    [TestMethod]
    public void DictionaryAdapterAvoidsWrapping() {
        var d = 5.Range().ToDictionary(i => i, i => i*i);
        
        // existing readonly dictionary class is not wrapped
        var rd = new ReadOnlyDictionary<int, int>(d);
        rd.AsReadOnlyDictionary().AssertReferenceEquals(rd);
        rd.AsIDictionary().AssertReferenceEquals(rd);

        // back-and-forth of an IReadOnlyDictionary has no effect
        var ar = d.AsReadOnlyDictionary();
        ar.AsIDictionary().AssertReferenceEquals(ar);
        ReadOnlyDic.AsIDictionary().AsReadOnlyDictionary().AssertReferenceEquals(ReadOnlyDic);
    }
}
