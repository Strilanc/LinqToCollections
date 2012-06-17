using LinqToCollections.Set;
using LinqToCollections.Map;
using LinqToCollections.Extra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LinqToCollectionsTest {
    [TestClass()]
    public class MapExtensionsTest {
        private static readonly IMap<int, int> EMPTY_MAP = new Map<int, int>(new Ret<int>(e => false, new int[0]), e => e);

        [TestMethod()]
        public void AsMapTest() {
            Util.ExpectException<ArgumentException>(() => ((IDictionary<int, int>)null).AsMap());
            var m = new Dictionary<int, int>() { { 1, 1 }, { 2, 2 } }.AsMap();
            Assert.IsTrue(m.Keys.SetEquals(new[] { 1, 2 }.ToRet()));
            Assert.IsTrue(m[1] == 1);
            Assert.IsTrue(m[2] == 2);
        }
        [TestMethod()]
        public void ToMapTest() {
            Util.ExpectException<ArgumentException>(() => ((IDictionary<int, int>)null).ToMap());
            Util.ExpectException<ArgumentException>(() => ((IEnumerable<IKeyValue<int, int>>)null).ToMap());
            Util.ExpectException<ArgumentException>(() => ((IEnumerable<int>)null).ToMap(e => e, e => e));
            Util.ExpectException<ArgumentException>(() => new int[0].ToMap<int, int, int>(null, e => e));
            Util.ExpectException<ArgumentException>(() => new int[0].ToMap<int, int, int>(e => e, null));
            var d1 = new Dictionary<int, int>() { { 1, 1 }, { 2, 2 } };
            var m1 = d1.ToMap();
            var m2 = d1.ToMap(e => e.Key, e => e.Value);
            var m3 = m2.ToMap();
            foreach (var m in new[] { m1, m2, m3 }) {
                Assert.IsTrue(m.Keys.SetEquals(new[] { 1, 2 }.ToRet()));
                Assert.IsTrue(m.AsEnumerable().Count() == 2);
                Assert.IsTrue(m[1] == 1);
                Assert.IsTrue(m[2] == 2);
            }
        }

        [TestMethod()]
        public void MapEqualsTest() {
            Util.ExpectException<ArgumentException>(() => ((IMap<int, int>)null).MapEquals(new Dictionary<int, int>().ToMap()));
            Util.ExpectException<ArgumentException>(() => new Dictionary<int, int>().ToMap().MapEquals(null));
            var m1 = Byte.MaxValue.Range().Where(e => e % 2 == 0).ToMap(e => e, e => (byte)(e / 2));
            var m2 = new Map<byte, byte>(new Ret<byte>(e => e % 2 == 0, Byte.MaxValue.Range().Where(e => e % 2 == 0)), e => (byte)(e / 2));
            var m3 = new Map<byte, byte>(new Ret<byte>(e => e % 4 == 0, Byte.MaxValue.Range().Where(e => e % 4 == 0)), e => (byte)(e / 2));
            Assert.IsTrue(m1.MapEquals(m1));
            Assert.IsTrue(m2.MapEquals(m2));
            Assert.IsTrue(m3.MapEquals(m3));
            Assert.IsTrue(m1.MapEquals(m2));
            Assert.IsTrue(m2.MapEquals(m1));
            Assert.IsTrue(!m1.MapEquals(m3));
            Assert.IsTrue(!m3.MapEquals(m1));
            Assert.IsTrue(!m2.MapEquals(m3));
            Assert.IsTrue(!m3.MapEquals(m2));
        }

        [TestMethod()]
        public void MappedToTest() {
            Util.ExpectException<ArgumentException>(() => ((IEnumerable<int>)null).MappedTo(e => e));
            Util.ExpectException<ArgumentException>(() => new int[0].MappedTo<int, int>(null));
            var m = new[] { 2, 3, 5, 7 }.MappedTo(e => e * 2);
            Assert.IsTrue(m.Keys.SetEquals(new[] { 2, 3, 5, 7 }.ToRet()));
            Assert.IsTrue(m[2] == 4);
            Assert.IsTrue(m[3] == 6);
            Assert.IsTrue(m[5] == 10);
            Assert.IsTrue(m[7] == 14);
        }
        [TestMethod()]
        public void KeyedByTest() {
            Util.ExpectException<ArgumentException>(() => ((IEnumerable<int>)null).KeyedBy(e => e));
            Util.ExpectException<ArgumentException>(() => new int[0].KeyedBy<int, int>(null));
            var m = new[] { 2, 3, 5, 7 }.KeyedBy(e => e * 2);
            Assert.IsTrue(m.Keys.SetEquals(new[] { 4, 6, 10, 14 }.ToRet()));
            Assert.IsTrue(m[4] == 2);
            Assert.IsTrue(m[6] == 3);
            Assert.IsTrue(m[10] == 5);
            Assert.IsTrue(m[14] == 7);
        }
        [TestMethod()]
        public void ValuesTest() {
            Util.ExpectException<ArgumentException>(() => ((IMap<int, int>)null).Values());
            var m = new[] { 2, 3, 5, 7 }.MappedTo(e => e * 2).Values();
            Assert.IsTrue(m.Count() == 4);
            Assert.IsTrue(m.Contains(4));
            Assert.IsTrue(m.Contains(6));
            Assert.IsTrue(m.Contains(10));
            Assert.IsTrue(m.Contains(14));
        }
        [TestMethod()]
        public void SelectTest() {
            Util.ExpectException<ArgumentException>(() => ((IMap<int, int>)null).Select(e => e));
            Util.ExpectException<ArgumentException>(() => EMPTY_MAP.Select<int, int, int>(null));
            var m = new[] { 2, 3, 5, 7 }.MappedTo(e => e).Select(e => e * 3);
            Assert.IsTrue(m.Keys.SetEquals(new[] { 2, 3, 5, 7 }.ToRet()));
            Assert.IsTrue(m[2] == 6);
            Assert.IsTrue(m[3] == 9);
            Assert.IsTrue(m[5] == 15);
            Assert.IsTrue(m[7] == 21);
        }
        [TestMethod()]
        public void WhereTest() {
            Util.ExpectException<ArgumentException>(() => ((IMap<int, int>)null).Where(e => true));
            Util.ExpectException<ArgumentException>(() => EMPTY_MAP.Where<int, int>(null));
            var m = new[] { 2, 3, 5, 7 }.MappedTo(e => e * 2).Where(e => e % 2 == 1);
            Assert.IsTrue(m.Keys.SetEquals(new[] { 3, 5, 7 }.ToRet()));
            Assert.IsTrue(m[3] == 6);
            Assert.IsTrue(m[5] == 10);
            Assert.IsTrue(m[7] == 14);
        }
        [TestMethod()]
        public void WhereValueTest() {
            Util.ExpectException<ArgumentException>(() => ((IMap<int, int>)null).WhereValue(e => true));
            Util.ExpectException<ArgumentException>(() => EMPTY_MAP.WhereValue<int, int>(null));
            var m = new[] { 2, 3, 5, 7 }.MappedTo(e => e * 2).WhereValue(e => e % 3 == 1);
            Assert.IsTrue(m.Keys.SetEquals(new[] { 2, 5 }.ToRet()));
            Assert.IsTrue(m[2] == 4);
            Assert.IsTrue(m[5] == 10);
        }
        [TestMethod()]
        public void ZipTest() {
            Util.ExpectException<ArgumentException>(() => ((IMap<int, int>)null).Zip(EMPTY_MAP, (e1, e2) => 0));
            Util.ExpectException<ArgumentException>(() => EMPTY_MAP.Zip(((IMap<int, int>)null), (e1, e2) => 0));
            Util.ExpectException<ArgumentException>(() => EMPTY_MAP.Zip<int, int, int, int>(EMPTY_MAP, null));
            var m1 = new[] { 2, 3, 5, 7, 11 }.MappedTo(e => e * 3);
            var m2 = new[] { 2, 3, 5, 7, 13 }.MappedTo(e => e * 2);
            var m3 = m1.Zip(m2, (e1, e2) => e1 + e2);
            Assert.IsTrue(m3.Keys.SetEquals(new[] { 2, 3, 5, 7 }.ToRet()));
            Assert.IsTrue(m3[2] == 10);
            Assert.IsTrue(m3[3] == 15);
            Assert.IsTrue(m3[5] == 25);
            Assert.IsTrue(m3[7] == 35);
        }
    }
}
