using LinqToLists;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToListsTest {
    [TestClass()]
    public class RistTest {
        [TestMethod()]
        public void ConstructorTest() {
            var r = new Rist<Int32>(count: 5, getter: i => i);
            Assert.IsTrue(r.Count == 5);
            Assert.IsTrue(r[0] == 0);
            Assert.IsTrue(r[1] == 1);
            Assert.IsTrue(r[4] == 4);
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2, 3, 4 }));

            var r2 = new Rist<Int32>(count: 7, getter: i => 1-i);
            Assert.IsTrue(r2.Count == 7);
            Assert.IsTrue(r2[5] == -4);
            Assert.IsTrue(r2[1] == 0);
            Assert.IsTrue(r2[0] == 1);
            Assert.IsTrue(r2.SequenceEqual(new[] { 1, 0, -1, -2, -3, -4, -5 }));
        }

        [TestMethod()]
        public void EfficientIteratorTest() {
            var r = new Rist<Int32>(count: 3, getter: i => { throw new ArgumentException(); }, efficientIterator: new[] { 0, 1, 2 });
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2 }));
        }
    }
}
