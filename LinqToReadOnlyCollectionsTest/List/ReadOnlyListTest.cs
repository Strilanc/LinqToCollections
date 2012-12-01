using System.Collections.Generic;
using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LinqToReadOnlyCollectionsTest {
    [TestClass]
    public class ReadOnlyListTest {
        [TestMethod]
        public void Select() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Select(i => i));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyList().Select((Func<int, int>)null));
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Select((e, i) => i));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyList().Select((Func<int, int, int>)null));

            3.Range().Select(i => i * i).AssertSequenceEquals(0, 1, 4);
            5.Range().Select(i => i * i).AssertListEquals(Enumerable.Range(0, 5).Select(i => i * i));
            10.Range().Select(i => i + i).AssertListEquals(Enumerable.Range(0, 10).Select(i => i + i));
            0.Range().Select(i => i).AssertSequenceIsEmpty();
        }
        [TestMethod]
        public void Zip() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Zip(new int[0], (i1, i2) => i1));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyList().Zip((IReadOnlyList<int>)null, (i1, i2) => i1));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyList().Zip(new int[0], (Func<int, int, int>)null));

            Assert.IsTrue(5.Range().Zip(4.Range(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(4.Range().Zip(5.Range(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(5.Range().Zip(new[] { true, false, true }, (e1, e2) => e2 ? e1 : -e1).SequenceEqual(new[] { 0, -1, 2 }));
            Assert.IsTrue(5.Range().Zip(4.Range(), 3.Range(), (e1, e2, e3) => e1 + e2 + e3).SequenceEqual(new[] { 0, 3, 6 }));
            Assert.IsTrue(5.Range().Zip(4.Range(), 3.Range(), 4.Range(), (e1, e2, e3, e4) => e1 + e2 + e3 + e4).SequenceEqual(new[] { 0, 4, 8 }));
        }

        [TestMethod]
        public void Reverse() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Reverse());

            Assert.IsTrue(0.Range().Reverse().SequenceEqual(new int[0]));
            Assert.IsTrue(5.Range().Reverse().SequenceEqual(new[] { 4, 3, 2, 1, 0 }));
            Assert.IsTrue(4.Range().Reverse().SequenceEqual(new[] { 3, 2, 1, 0 }));
        }

        [TestMethod]
        public void Last() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Last());
            TestUtil.AssertThrows<ArgumentException>(() => 0.Range().Last());

            Assert.IsTrue(1.Range().Last() == 0);
            Assert.IsTrue(2.Range().Last() == 1);
            Assert.IsTrue(int.MaxValue.Range().Last() == int.MaxValue - 1);
            Assert.IsTrue(new AnonymousReadOnlyList<int>(counter: () => 11, getter: i => {
                if (i < 10) throw new ArgumentException();
                return i;
            }).Last() == 10);
        }
        [TestMethod]
        public void LastOrDefault() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).LastOrDefault());
            TestUtil.AssertThrows<ArgumentException>(() => 0.Range().Last());

            Assert.IsTrue(0.Range().LastOrDefault() == 0);
            Assert.IsTrue(0.Range().LastOrDefault(5) == 5);
            Assert.IsTrue(1.Range().LastOrDefault() == 0);
            Assert.IsTrue(1.Range().LastOrDefault(5) == 0);
            Assert.IsTrue(2.Range().LastOrDefault() == 1);
            Assert.IsTrue(2.Range().LastOrDefault(5) == 1);
            Assert.IsTrue(int.MaxValue.Range().LastOrDefault() == int.MaxValue - 1);
            Assert.IsTrue(new AnonymousReadOnlyList<int>(counter: () => 11, getter: i => {
                if (i < 10) throw new ArgumentException();
                return i;
            }).LastOrDefault() == 10);
        }

        [TestMethod]
        public void Range() {
            for (var i = 0; i < 5; i++) {
                i.Range().AssertListEquals(Enumerable.Range(0, i));
                ((byte)i).Range().AssertListEquals(Enumerable.Range(0, i).Select(e => (byte)e));
                ((ushort)i).Range().AssertListEquals(Enumerable.Range(0, i).Select(e => (ushort)e));
                ((sbyte)i).Range().AssertListEquals(Enumerable.Range(0, i).Select(e => (sbyte)e));
                ((short)i).Range().AssertListEquals(Enumerable.Range(0, i).Select(e => (short)e));
            }
        }
        [TestMethod]
        public void Repeat() {
            ReadOnlyList.Repeated("a", 20).AssertListEquals(Enumerable.Repeat("a", 20));
            ReadOnlyList.Repeated("b", 10).AssertListEquals(Enumerable.Repeat("a", 10));
        }
        [TestMethod]
        public void AllValues() {
            ReadOnlyList.AllBytes().Select(e => (int)e).AssertListEquals(Enumerable.Range(Byte.MinValue, Byte.MaxValue - (int)Byte.MinValue + 1));
            ReadOnlyList.AllSignedBytes().Select(e => (int)e).AssertListEquals(Enumerable.Range(SByte.MinValue, SByte.MaxValue - (int)SByte.MinValue + 1));
            ReadOnlyList.AllUnsigned16BitIntegers().Select(e => (int)e).AssertListEquals(Enumerable.Range(UInt16.MinValue, UInt16.MaxValue - (int)UInt16.MinValue + 1));
            ReadOnlyList.AllSigned16BitIntegers().Select(e => (int)e).AssertListEquals(Enumerable.Range(Int16.MinValue, Int16.MaxValue - (int)Int16.MinValue + 1));
        }
    }
}
