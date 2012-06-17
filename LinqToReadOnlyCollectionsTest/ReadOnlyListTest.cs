using LinqToCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToCollectionsTest {
    [TestClass()]
    public class ReadOnlyListTest {
        [TestMethod()]
        public void ConstructorTest() {
            Util.ExpectException<ArgumentException>(() => new ReadOnlyList<int>(counter: null, getter: i => i));
            Util.ExpectException<ArgumentException>(() => new ReadOnlyList<int>(counter: () => 0, getter: null));

            var r = new ReadOnlyList<Int32>(counter: () => 5, getter: i => i);
            Assert.IsTrue(r.Count == 5);
            Assert.IsTrue(r[0] == 0);
            Assert.IsTrue(r[1] == 1);
            Assert.IsTrue(r[4] == 4);
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2, 3, 4 }));
            Util.ExpectException<ArgumentException>(() => { var x = r[-1]; });
            Util.ExpectException<ArgumentException>(() => { var x = r[5]; });

            var r2 = new ReadOnlyList<Int32>(counter: () => 7, getter: i => 1 - i);
            Assert.IsTrue(r2.Count == 7);
            Assert.IsTrue(r2[6] == -5);
            Assert.IsTrue(r2[5] == -4);
            Assert.IsTrue(r2[1] == 0);
            Assert.IsTrue(r2[0] == 1);
            Assert.IsTrue(r2.SequenceEqual(new[] { 1, 0, -1, -2, -3, -4, -5 }));
            Util.ExpectException<ArgumentException>(() => { var x = r[-1]; });
            Util.ExpectException<ArgumentException>(() => { var x = r[7]; });
        }

        [TestMethod()]
        public void EfficientIteratorTest() {
            var r = new ReadOnlyList<Int32>(counter: () => { throw new ArgumentException(); }, getter: i => { throw new ArgumentException(); }, efficientIterator: new[] { 0, 1, 2 });
            Util.ExpectException<ArgumentException>(() => { var x = r[0]; });
            Util.ExpectException<ArgumentException>(() => { var x = r.Count; });
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2 }));
        }
    }
}
