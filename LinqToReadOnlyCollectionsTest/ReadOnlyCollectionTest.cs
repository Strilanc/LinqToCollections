using LinqToCollections.Collection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToCollectionsTest {
    [TestClass()]
    public class ReadOnlyCollectionTest {
        [TestMethod()]
        public void ConstructorTest() {
            Util.ExpectException<ArgumentException>(() => new ReadOnlyCollection<int>(counter: null, iterator: Enumerable.Range(0, 0).GetEnumerator));
            Util.ExpectException<ArgumentException>(() => new ReadOnlyCollection<int>(counter: () => 0, iterator: null));

            var r = new ReadOnlyCollection<Int32>(counter: () => 5, iterator: Enumerable.Range(0, 5).GetEnumerator);
            Assert.IsTrue(r.Count == 5);
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2, 3, 4 }));

            var r2 = new ReadOnlyCollection<Int32>(counter: () => 7, iterator: Enumerable.Range(0, 7).Select(i => 1 - i).GetEnumerator);
            Assert.IsTrue(r2.Count == 7);
            Assert.IsTrue(r2.SequenceEqual(new[] { 1, 0, -1, -2, -3, -4, -5 }));
        }

        [TestMethod()]
        public void EfficientIteratorTest() {
            var r = new ReadOnlyCollection<Int32>(counter: () => { throw new ArgumentException(); }, iterator: Enumerable.Range(0, 3).GetEnumerator);
            Util.ExpectException<ArgumentException>(() => { var x = r.Count; });
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2 }));
        }
    }
}
