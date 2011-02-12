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
    }
}
