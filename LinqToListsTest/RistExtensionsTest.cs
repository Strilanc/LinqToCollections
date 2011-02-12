using LinqToLists;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LinqToListsTest {
    [TestClass()]
    public class RistExtensionsTest {
        [TestMethod()]
        public void AsRistIListTest() {
            Util.ExpectException<ArgumentException>(() => RistExtensions.AsRist((IList<int>)null));

            Assert.IsTrue(new[] { 0, 1, 2 }.AsRist().Count == 3);
            Assert.IsTrue(new[] { 0, 2, 5, 7 }.AsRist().SequenceEqual(new[] { 0, 2, 5, 7 }));
            var list = new List<int>() { 2, 3 };
            var rist = list.AsRist();
            Assert.IsTrue(rist.SequenceEqual(new[] { 2, 3 }));
            list.Add(5);
            Assert.IsTrue(rist.SequenceEqual(new[] { 2, 3, 5 }));
        }
        [TestMethod()]
        public void AsRistIEnumerableTest() {
            Util.ExpectException<ArgumentException>(() => RistExtensions.AsRist((IEnumerable<int>)null));

            var list = new List<int>() { 2, 3 };
            var listAsRist = list.AsEnumerable().AsRist();
            var projectionAsRist = list.Select(i => i + 1).AsRist();
            
            //IRist ==> gives back input
            Assert.ReferenceEquals(listAsRist, listAsRist.AsEnumerable().AsRist());

            //IList ==> viewed as Rist, IEnumerable ==> copied
            Assert.IsTrue(listAsRist.SequenceEqual(new[] { 2, 3 }));
            Assert.IsTrue(projectionAsRist.SequenceEqual(new[] { 3, 4 }));
            list.RemoveAt(1);
            Assert.IsTrue(listAsRist.SequenceEqual(new[] { 2 }));
            Assert.IsTrue(projectionAsRist.SequenceEqual(new[] { 3, 4 }));
        }
        [TestMethod()]
        public void ToRistTest() {
            Util.ExpectException<ArgumentException>(() => RistExtensions.ToRist((IEnumerable<int>)null));
            
            var list = new List<int>() { 2, 3 };
            var rist = list.ToRist();
            Assert.IsTrue(rist.SequenceEqual(new[] { 2, 3 }));
            list.Add(1);
            Assert.IsTrue(rist.SequenceEqual(new[] { 2, 3 }));
        }

        [TestMethod()]
        public void SubListTest() {
            Util.ExpectException<ArgumentException>(() => RistExtensions.SubList((IRist<int>)null, 0, 0));
            Util.ExpectException<ArgumentException>(() => RistExtensions.SubList(new[] { 1 }.AsRist(), -1, 0));
            Util.ExpectException<ArgumentException>(() => RistExtensions.SubList(new[] { 1 }.AsRist(), 0, 2));
            Util.ExpectException<ArgumentException>(() => RistExtensions.SubList(new[] { 1 }.AsRist(), 1, 1));
            Util.ExpectException<ArgumentException>(() => RistExtensions.SubList(new[] { 1 }.AsRist(), 2, 0));

            var r = Enumerable.Range(0, 10).AsRist().SubList(0, 10);
            Assert.IsTrue(r.Count == 10);
            Assert.IsTrue(r.SequenceEqual(Enumerable.Range(0, 10)));

            var r2 = Enumerable.Range(0, 10).AsRist().SubList(2, 8);
            Assert.IsTrue(r2.Count == 8);
            Assert.IsTrue(r2.SequenceEqual(Enumerable.Range(2, 8)));

            var r3 = Enumerable.Range(0, 10).AsRist().SubList(0, 8);
            Assert.IsTrue(r3.Count == 8);
            Assert.IsTrue(r3.SequenceEqual(Enumerable.Range(0, 8)));

            var r4 = Enumerable.Range(0, 10).AsRist().SubList(1, 8);
            Assert.IsTrue(r4.Count == 8);
            Assert.IsTrue(r4.SequenceEqual(Enumerable.Range(1, 8)));

            var r5 = Enumerable.Range(0, 10).AsRist().SubList(5, 0);
            Assert.IsTrue(r5.Count == 0);
            Assert.IsTrue(r5.SequenceEqual(Enumerable.Range(0, 0)));

            var r6 = Enumerable.Range(0, 10).AsRist().SubList(0, 0);
            Assert.IsTrue(r6.Count == 0);
            Assert.IsTrue(r6.SequenceEqual(Enumerable.Range(0, 0)));

            var r7 = Enumerable.Range(0, 10).AsRist().SubList(10, 0);
            Assert.IsTrue(r7.Count == 0);
            Assert.IsTrue(r7.SequenceEqual(Enumerable.Range(0, 0)));

            var r8 = Enumerable.Range(0, 10).AsRist().SubList(1, 8).SubList(1, 6);
            Assert.IsTrue(r8.Count == 6);
            Assert.IsTrue(r8.SequenceEqual(Enumerable.Range(2, 6)));
        }

        [TestMethod()]
        public void SkipExactTest() {
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().SkipExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().SkipExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().SkipExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().SkipExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().SkipExact(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().SkipExact(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().SkipExact(1).SequenceEqual(new[] { 2, 3, 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().SkipExact(3).SequenceEqual(new[] { 4 }));
        }
        [TestMethod()]
        public void SkipLastExactTest() {
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().SkipLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().SkipLastExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().SkipLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().SkipLastExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().SkipLastExact(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().SkipLastExact(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().SkipLastExact(1).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().SkipLastExact(3).SequenceEqual(new[] { 1 }));
        }
        [TestMethod()]
        public void TakeExactTest() {
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().TakeExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().TakeExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().TakeExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().TakeExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().TakeExact(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().TakeExact(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().TakeExact(1).SequenceEqual(new[] { 1 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().TakeExact(3).SequenceEqual(new[] { 1, 2, 3 }));
        }
        [TestMethod()]
        public void TakeLastExactTest() {
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().TakeLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().TakeLastExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().TakeLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().TakeLastExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().TakeLastExact(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().TakeLastExact(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().TakeLastExact(1).SequenceEqual(new[] { 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().TakeLastExact(3).SequenceEqual(new[] { 2, 3, 4 }));
        }

        [TestMethod()]
        public void SkipTest() {
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().Skip(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().Skip(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().Skip(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().Skip(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().Skip(4).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().Skip(10).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().Skip(1).SequenceEqual(new[] { 2, 3, 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().Skip(3).SequenceEqual(new[] { 4 }));
        }
        [TestMethod()]
        public void SkipLastTest() {
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().SkipLast(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().SkipLast(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().SkipLast(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().SkipLast(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().SkipLast(4).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().SkipLast(10).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().SkipLast(1).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().SkipLast(3).SequenceEqual(new[] { 1 }));
        }
        [TestMethod()]
        public void TakeTest() {
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().Take(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().Take(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().Take(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().Take(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().Take(4).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().Take(10).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().Take(1).SequenceEqual(new[] { 1 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().Take(3).SequenceEqual(new[] { 1, 2, 3 }));
        }
        [TestMethod()]
        public void TakeLastTest() {
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsRist().TakeLast(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsRist().TakeLast(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().TakeLast(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().TakeLast(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().TakeLast(4).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsRist().TakeLast(10).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().TakeLast(1).SequenceEqual(new[] { 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsRist().TakeLast(3).SequenceEqual(new[] { 2, 3, 4 }));
        }

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

        [TestMethod()]
        public void SelectTest() {
            Util.ExpectException<ArgumentException>(() => ((IRist<int>)null).Select(i => i));
            Util.ExpectException<ArgumentException>(() => new int[0].AsRist().Select((Func<int, int>)null));

            Assert.IsTrue(5.Range().Select(i => i * i).SequenceEqual(new[] { 0, 1, 4, 9, 16 }));
            Assert.IsTrue(0.Range().Select(i => i * i).SequenceEqual(new int[0]));
            Assert.IsTrue(4.Range().Select(i => i + i).SequenceEqual(new[] { 0, 2, 4, 6 }));
        }
        [TestMethod()]
        public void Select2Test() {
            Util.ExpectException<ArgumentException>(() => ((IRist<int>)null).Select((e, i) => i));
            Util.ExpectException<ArgumentException>(() => new int[0].AsRist().Select((Func<int, int, int>)null));

            Assert.IsTrue(5.Range().Select((e, i) => i * e).SequenceEqual(new[] { 0, 1, 4, 9, 16 }));
            Assert.IsTrue(0.Range().Select((e, i) => i * e).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsRist().Select((e, i) => e).SequenceEqual(new[] { 2, 3, 5 }));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsRist().Select((e, i) => i).SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsRist().Select((e, i) => e + i).SequenceEqual(new[] { 2, 4, 7 }));
        }
        [TestMethod()]
        public void ZipTest() {
            Util.ExpectException<ArgumentException>(() => ((IRist<int>)null).Zip(new int[0], (i1, i2) => i1));
            Util.ExpectException<ArgumentException>(() => new int[0].AsRist().Zip((IRist<int>)null, (i1, i2) => i1));
            Util.ExpectException<ArgumentException>(() => new int[0].AsRist().Zip(new int[0], (Func<int, int, int>)null));

            Assert.IsTrue(5.Range().Zip(4.Range(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(4.Range().Zip(5.Range(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(5.Range().Zip(new[] {true, false, true}, (e1, e2) => e2 ? e1 : -e1).SequenceEqual(new[] { 0, -1, 2 }));
        }
    }
}
