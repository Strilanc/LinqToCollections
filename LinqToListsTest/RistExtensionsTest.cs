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
            var list = new List<int>() { 2, 3 };
            var rist = list.ToRist();
            Assert.IsTrue(rist.SequenceEqual(new[] { 2, 3 }));
            list.Add(1);
            Assert.IsTrue(rist.SequenceEqual(new[] { 2, 3 }));
        }
    }
}
