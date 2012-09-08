using LinqToCollections.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToCollectionsTest {
    [TestClass]
    public class RetTest {
        [TestMethod]
        public void ConstructorTest() {
            Util.ExpectException<ArgumentException>(() => new Ret<int>(container: null, iterator: new int[0]));
            Util.ExpectException<ArgumentException>(() => new Ret<int>(container: e => false, iterator: null));

            var r = new Ret<Int32>(container: e => e >= 0 && e < 5, iterator: new[] { 0, 1, 2, 3, 4 });
            Assert.IsTrue(!r.Contains(-1));
            Assert.IsTrue(r.Contains(0));
            Assert.IsTrue(r.Contains(1));
            Assert.IsTrue(r.Contains(2));
            Assert.IsTrue(r.Contains(3));
            Assert.IsTrue(r.Contains(4));
            Assert.IsTrue(!r.Contains(5));
            Assert.IsTrue(!r.Contains(Int32.MaxValue));
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2, 3, 4 }));

            var r2 = new Ret<Int32>(container: e => e == 2, iterator: new[] {2, 3});
            Assert.IsTrue(!r2.Contains(1));
            Assert.IsTrue(r2.Contains(2));
            Assert.IsTrue(!r2.Contains(3)); //container trumps iterator on Contains
            Assert.IsTrue(r2.SequenceEqual(new[] { 2, 3 }));
        }
    }
}
