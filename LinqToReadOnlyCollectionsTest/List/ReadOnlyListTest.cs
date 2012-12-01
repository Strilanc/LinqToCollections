using System.Collections.Generic;
using LinqToReadOnlyCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LinqToReadOnlyCollectionsTest {
    [TestClass]
    public class ReadOnlyListTest {
        [TestMethod]
        public void SelectTest() {
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
        public void ZipTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Zip(new int[0], (i1, i2) => i1));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyList().Zip((IReadOnlyList<int>)null, (i1, i2) => i1));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyList().Zip(new int[0], (Func<int, int, int>)null));

            Assert.IsTrue(5.Range().Zip(4.Range(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(4.Range().Zip(5.Range(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(5.Range().Zip(new[] { true, false, true }, (e1, e2) => e2 ? e1 : -e1).SequenceEqual(new[] { 0, -1, 2 }));
        }

        [TestMethod]
        public void ReverseTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Reverse());

            Assert.IsTrue(0.Range().Reverse().SequenceEqual(new int[0]));
            Assert.IsTrue(5.Range().Reverse().SequenceEqual(new[] { 4, 3, 2, 1, 0 }));
            Assert.IsTrue(4.Range().Reverse().SequenceEqual(new[] { 3, 2, 1, 0 }));
        }

        [TestMethod]
        public void LastTest() {
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
        public void LastOrDefaultTest() {
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
        public void AsIListTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).AsIList());

            var rawList = new List<int> { 0, 1, 2 };
            var asReadOnlyList = rawList.AsReadOnlyList();
            var asReadOnlyListAsList = asReadOnlyList.AsIList();
            var asReadOnlyListAsListAsReadOnlyList = asReadOnlyListAsList.AsReadOnlyList();
            Assert.IsTrue(asReadOnlyListAsList.IsReadOnly);
            Assert.IsTrue(asReadOnlyListAsList.Count == 3);
            Assert.IsTrue(asReadOnlyListAsList.SequenceEqual(new[] { 0, 1, 2 }));
            Assert.AreSame(asReadOnlyListAsListAsReadOnlyList, asReadOnlyListAsList);
            Assert.AreSame(asReadOnlyListAsListAsReadOnlyList, asReadOnlyListAsListAsReadOnlyList.AsIList());
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyListAsList.Add(0));
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyListAsList.Remove(0));
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyListAsList.Insert(0, 0));
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyListAsList.RemoveAt(0));
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyListAsList.Clear());
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyListAsList[0] = 0);

            var asList = 5.Range().AsIList();
            var asListAsReadOnlyList = asList.AsReadOnlyList();
            Assert.AreSame(asListAsReadOnlyList, asList);
            Assert.IsTrue(asList.SequenceEqual(new[] { 0, 1, 2, 3, 4 }));
            Assert.IsTrue(asList.IsReadOnly);
            Assert.IsTrue(asList.Count == 5);
            Assert.IsTrue(asList.Contains(4));
            Assert.IsTrue(asList.Contains(1));
            Assert.IsTrue(asList.Contains(0));
            Assert.IsTrue(!asList.Contains(-1));
            Assert.IsTrue(asList.IndexOf(4) == 4);
            Assert.IsTrue(asList.IndexOf(1) == 1);
            Assert.IsTrue(asList.IndexOf(0) == 0);
            Assert.IsTrue(asList.IndexOf(-1) == -1);
            Assert.IsTrue(asList.IndexOf(-2) == -1);

            var d = new int[10];
            asList.CopyTo(d, 2);
            Assert.IsTrue(d.SequenceEqual(new[] { 0, 0, 0, 1, 2, 3, 4, 0, 0, 0 }));
        }
    }
}
