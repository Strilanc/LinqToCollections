using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LinqToReadOnlyCollectionsTest {
    [TestClass]
    public class ValueToTest {
        [TestMethod]
        public void RangeTestInt32() {
            TestUtil.AssertThrows<ArgumentException>(() => (-1).Range());
            TestUtil.AssertThrows<ArgumentException>(() => int.MinValue.Range());
            TestUtil.AssertThrows<ArgumentException>((() => int.MaxValue.Range()[int.MaxValue]));
            TestUtil.AssertThrows<ArgumentException>((() => 0.Range()[0]));
            TestUtil.AssertThrows<ArgumentException>((() => 1.Range()[1]));

            Assert.IsTrue(int.MaxValue.Range().Count == int.MaxValue);
            Assert.IsTrue(int.MaxValue.Range()[0] == 0);
            Assert.IsTrue(int.MaxValue.Range()[71] == 71);
            Assert.IsTrue(int.MaxValue.Range()[int.MaxValue - 1] == int.MaxValue - 1);
            Assert.IsTrue(0.Range().Count == 0);
            Assert.IsTrue(1.Range().Count == 1);
            Assert.IsTrue(10.Range().SequenceEqual(Enumerable.Range(0, 10)));
        }
        [TestMethod]
        public void RangeTestInt16() {
            TestUtil.AssertThrows<ArgumentException>(() => ((short)-1).Range());
            TestUtil.AssertThrows<ArgumentException>(() => short.MinValue.Range());
            TestUtil.AssertThrows<ArgumentException>((() => short.MaxValue.Range()[short.MaxValue]));
            TestUtil.AssertThrows<ArgumentException>((() => ((short)0).Range()[0]));
            TestUtil.AssertThrows<ArgumentException>((() => ((short)1).Range()[1]));

            Assert.IsTrue(short.MaxValue.Range().Count == short.MaxValue);
            Assert.IsTrue(short.MaxValue.Range()[0] == 0);
            Assert.IsTrue(short.MaxValue.Range()[71] == 71);
            Assert.IsTrue(short.MaxValue.Range()[short.MaxValue - 1] == short.MaxValue - 1);
            Assert.IsTrue(((short)0).Range().Count == 0);
            Assert.IsTrue(((short)1).Range().Count == 1);
            Assert.IsTrue(((short)10).Range().SequenceEqual(Enumerable.Range(0, 10).Select(i => (short)i)));
        }
        [TestMethod]
        public void RangeTestUInt8() {
            TestUtil.AssertThrows<ArgumentException>((() => byte.MaxValue.Range()[byte.MaxValue]));
            TestUtil.AssertThrows<ArgumentException>((() => ((byte)0).Range()[0]));
            TestUtil.AssertThrows<ArgumentException>((() => ((byte)1).Range()[1]));

            Assert.IsTrue(byte.MaxValue.Range().Count == byte.MaxValue);
            Assert.IsTrue(byte.MaxValue.Range()[0] == 0);
            Assert.IsTrue(byte.MaxValue.Range()[71] == 71);
            Assert.IsTrue(byte.MaxValue.Range()[byte.MaxValue - 1] == byte.MaxValue - 1);
            Assert.IsTrue(((byte)0).Range().Count == 0);
            Assert.IsTrue(((byte)1).Range().Count == 1);
            Assert.IsTrue(((byte)10).Range().SequenceEqual(Enumerable.Range(0, 10).Select(i => (byte)i)));
        }

        //[TestMethod]
        //public void RepeatedTest() {
        //    TestUtil.AssertThrows<ArgumentException>(() => 1.Repeated(-1));

        //    Assert.IsTrue(((Object)null).Repeated(0).Count == 0);
        //    Assert.IsTrue(2.Repeated(int.MaxValue)[int.MaxValue - 1] == 2);
        //    Assert.IsTrue(3.Repeated(0).SequenceEqual(new int[0]));
        //    Assert.IsTrue(5.Repeated(3).SequenceEqual(new[] { 5, 5, 5 }));
        //}

        //[TestMethod]
        //public void KeyValueTest() {
        //    TestUtil.AssertThrows<ArgumentException>(() => ((Object)null).KeyValue(1));
        //    Assert.IsTrue(1.KeyValue(2).Key == 1);
        //    Assert.IsTrue(1.KeyValue(2).Value == 2);
        //}
    }
}
