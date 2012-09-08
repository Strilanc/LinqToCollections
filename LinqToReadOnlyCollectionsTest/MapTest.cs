using LinqToCollections.Map;
using LinqToCollections.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LinqToCollectionsTest {
    [TestClass]
    public class MapTest {
        [TestMethod]
        public void ConstructorTest() {
            Util.ExpectException<ArgumentException>(() => new Map<int, int>(null, e => e));
            Util.ExpectException<ArgumentException>(() => new Map<int, int>(new Ret<int>(e => false, new int[0]), null));

            var r = new Ret<Int32>(container: e => e >= 0 && e < 5, iterator: new[] { 0, 1, 2, 3, 4 });
            var m = new Map<int, int>(r, e => e * 2);
            Assert.IsTrue(m.Keys == r);
            Assert.IsTrue(m[0] == 0);
            Assert.IsTrue(m[1] == 2);
            Assert.IsTrue(m[2] == 4);
            Assert.IsTrue(m[3] == 6);
            Assert.IsTrue(m[4] == 8);

            var r2 = new Ret<Int32>(container: e => e == 2 || e == 3, iterator: new[] {2, 3});
            var m2 = new Map<int, int>(r2, e => e * e);
            Assert.IsTrue(m2[2] == 4);
            Assert.IsTrue(m2[3] == 9);
        }
    }
}
