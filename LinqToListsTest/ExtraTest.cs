using LinqToLists;
using LinqToLists.Extra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LinqToListsTest {
    [TestClass()]
    public class ExtraTest {
        [TestMethod()]
        public void RangeTest() {
            Util.ExpectException<ArgumentException>(() => (-1).Range());
            Util.ExpectException<ArgumentException>(() => int.MinValue.Range());
            Util.ExpectException<ArgumentException>(() => { var x = int.MaxValue.Range()[int.MaxValue]; });
            Util.ExpectException<ArgumentException>(() => { var x = 0.Range()[0]; });
            Util.ExpectException<ArgumentException>(() => { var x = 1.Range()[1]; });

            Assert.IsTrue(int.MaxValue.Range().Count == int.MaxValue);
            Assert.IsTrue(int.MaxValue.Range()[0] == 0);
            Assert.IsTrue(int.MaxValue.Range()[71] == 71);
            Assert.IsTrue(int.MaxValue.Range()[int.MaxValue - 1] == int.MaxValue - 1);
            Assert.IsTrue(0.Range().Count == 0);
            Assert.IsTrue(1.Range().Count == 1);
            Assert.IsTrue(10.Range().SequenceEqual(Enumerable.Range(0, 10)));
        }
    }
}
