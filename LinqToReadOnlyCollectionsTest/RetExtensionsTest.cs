using LinqToCollections.Set;
using LinqToCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LinqToCollectionsTest {
    [TestClass]
    public class RetExtensionsTest {
        [TestMethod]
        public void ToRetTest() {
            Util.ExpectException<ArgumentException>(() => ((IEnumerable<int>)null).ToRet());

            var s0 = new int[0].ToRet();
            Assert.IsTrue(s0.SequenceEqual(new int[0]));
            Assert.IsTrue(!s0.Contains(0));

            var s1 = new[] { 1 }.ToRet();
            Assert.IsTrue(s1.SequenceEqual(new[] { 1 }));
            Assert.IsTrue(!s1.Contains(0));
            Assert.IsTrue(s1.Contains(1));
            Assert.IsTrue(!s1.Contains(2));

            var s2 = new[] { 4, 1, 3, 3 }.ToRet();
            Assert.IsTrue(s2.AsEnumerable().Contains(1));
            Assert.IsTrue(s2.AsEnumerable().Contains(3));
            Assert.IsTrue(s2.AsEnumerable().Contains(4));
            Assert.IsTrue(s2.AsEnumerable().Count() == 3);
            Assert.IsTrue(!s2.Contains(0));
            Assert.IsTrue(s2.Contains(1));
            Assert.IsTrue(!s2.Contains(2));
            Assert.IsTrue(s2.Contains(3));
            Assert.IsTrue(s2.Contains(4));
            Assert.IsTrue(!s2.Contains(5));
        }
        [TestMethod]
        public void AsRetTest() {
            Util.ExpectException<ArgumentException>(() => ((ISet<int>)null).AsRet());

            var s0 = new HashSet<int>().AsRet();
            Assert.IsTrue(s0.SequenceEqual(new int[0]));
            Assert.IsTrue(!s0.Contains(0));

            var s1 = new HashSet<int>(new[] { 1 }).AsRet();
            Assert.IsTrue(s1.SequenceEqual(new[] { 1 }));
            Assert.IsTrue(!s1.Contains(0));
            Assert.IsTrue(s1.Contains(1));
            Assert.IsTrue(!s1.Contains(2));

            var s2 = new HashSet<int>(new[] { 4, 1, 3, 3 }).AsRet();
            Assert.IsTrue(s2.AsEnumerable().Contains(1));
            Assert.IsTrue(s2.AsEnumerable().Contains(3));
            Assert.IsTrue(s2.AsEnumerable().Contains(4));
            Assert.IsTrue(s2.AsEnumerable().Count() == 3);
            Assert.IsTrue(!s2.Contains(0));
            Assert.IsTrue(s2.Contains(1));
            Assert.IsTrue(!s2.Contains(2));
            Assert.IsTrue(s2.Contains(3));
            Assert.IsTrue(s2.Contains(4));
            Assert.IsTrue(!s2.Contains(5));
        }
        [TestMethod]
        public void SetEqualsTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).SetEquals(new[] { 1 }.ToRet()));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().SetEquals(null));
            var s0A = new HashSet<int>().AsRet();
            var s0B = new Ret<int>(e => false, new int[0]);
            var s1A = new HashSet<int>(new[] { 1 }).AsRet();
            var s1B = new HashSet<int>(new[] { 1 }).AsRet();
            var s2A = new HashSet<int>(new[] { 4, 1, 3, 3 }).AsRet();
            var s2B = new HashSet<int>(new[] { 1, 3, 4 }).AsRet();

            Assert.IsTrue(s0A.SetEquals(s0B));
            Assert.IsTrue(s0B.SetEquals(s0A));
            Assert.IsTrue(s1A.SetEquals(s1B));
            Assert.IsTrue(s1B.SetEquals(s1A));
            Assert.IsTrue(s2A.SetEquals(s2B));
            Assert.IsTrue(s2B.SetEquals(s2A));
            Assert.IsTrue(!s0A.SetEquals(s1A));
            Assert.IsTrue(!s0B.SetEquals(s2B));
            Assert.IsTrue(!s1A.SetEquals(s0B));
            Assert.IsTrue(!s1B.SetEquals(s2B));
        }
        [TestMethod]
        public void WhereTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).Where(e => true));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().Where(null));

            var w0 = new[] { 2, 3, 5, 7 }.ToRet().Where(e => e % 2 == 1);
            Assert.IsTrue(w0.SetEquals(new[] { 3, 5, 7 }.ToRet()));
            Assert.IsTrue(w0.Count() == 3);
            Assert.IsTrue(w0.Where(e => true).Count() == 3);
            Assert.IsTrue(w0.Where(e => false).Count() == 0);

            var w1 = new Ret<Byte>(e => e > 10, Byte.MaxValue.Range().Skip(10)).Where(e => e % 2 == 1);
            Assert.IsTrue(!w1.Contains(9));
            Assert.IsTrue(!w1.Contains(10));
            Assert.IsTrue(w1.Contains(11));
            Assert.IsTrue(!w1.Contains(12));
            Assert.IsTrue(w1.Contains(13));
            Assert.IsTrue(w1.SequenceEqual(Byte.MaxValue.Range().Skip(10).Where(e => e % 2 == 1)));
        }
        [TestMethod]
        public void UnionTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).Union(new[] { 1 }.ToRet()));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().Union(null));

            var b1 = Byte.MaxValue.Range().Where(e => e % 2 == 1).ToRet();
            var b2 = Byte.MaxValue.Range().Where(e => e % 3 == 1).ToRet();
            var u1 = b1.Union(b2);
            Assert.IsTrue(u1.SetEquals(Byte.MaxValue.Range().Where(e => e % 2 == 1 || e % 3 == 1).ToRet()));
        }
        [TestMethod]
        public void IntersectTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).Intersect(new[] { 1 }.ToRet()));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().Intersect(null));
            var b1 = Byte.MaxValue.Range().Where(e => e % 2 == 1).ToRet();
            var b2 = Byte.MaxValue.Range().Where(e => e % 3 == 1).ToRet();
            var u1 = b1.Intersect(b2);
            Assert.IsTrue(u1.SetEquals(Byte.MaxValue.Range().Where(e => e % 2 == 1 && e % 3 == 1).ToRet()));
        }
        [TestMethod]
        public void ExceptTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).Except(new[] { 1 }.ToRet()));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().Except(null));
            var b1 = Byte.MaxValue.Range().Where(e => e % 2 == 1).ToRet();
            var b2 = Byte.MaxValue.Range().Where(e => e % 3 == 1).ToRet();
            var u1 = b1.Except(b2);
            Assert.IsTrue(u1.SetEquals(Byte.MaxValue.Range().Where(e => e % 2 == 1 && e % 3 != 1).ToRet()));
        }
        [TestMethod]
        public void IntersectsTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).Intersect(new[] { 1 }.ToRet()));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().Intersect(null));
            var b1 = Byte.MaxValue.Range().Where(e => e % 2 == 1).ToRet();
            var b2 = Byte.MaxValue.Range().Where(e => e % 3 == 1).ToRet();
            var b3 = Byte.MaxValue.Range().Where(e => e % 3 == 2).ToRet();
            Assert.IsTrue(b1.Intersects(b2));
            Assert.IsTrue(b1.Intersects(b3));
            Assert.IsTrue(!b2.Intersects(b3));
            Assert.IsTrue(b2.Intersects(b1));
            Assert.IsTrue(b3.Intersects(b1));
            Assert.IsTrue(!b3.Intersects(b2));
        }
        [TestMethod]
        public void IsSubsetOfTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).IsSubsetOf(new[] { 1 }.ToRet()));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().IsSubsetOf(null));
            var b1 = Byte.MaxValue.Range().Where(e => e % 2 == 1).ToRet();
            var b2 = Byte.MaxValue.Range().Where(e => e % 3 == 1).ToRet();
            var b3 = Byte.MaxValue.Range().Where(e => e % 4 == 1).ToRet();
            Assert.IsTrue(b1.IsSubsetOf(b1));
            Assert.IsTrue(b2.IsSubsetOf(b2));
            Assert.IsTrue(b3.IsSubsetOf(b3));
            Assert.IsTrue(!b1.IsSubsetOf(b2));
            Assert.IsTrue(!b1.IsSubsetOf(b3));
            Assert.IsTrue(!b2.IsSubsetOf(b1));
            Assert.IsTrue(!b2.IsSubsetOf(b3));
            Assert.IsTrue(!b3.IsSubsetOf(b2));
            Assert.IsTrue(b3.IsSubsetOf(b1));
        }
        [TestMethod]
        public void IsStrictSubsetOfTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).IsStrictSubsetOf(new[] { 1 }.ToRet()));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().IsStrictSubsetOf(null));
            var b1 = Byte.MaxValue.Range().Where(e => e % 2 == 1).ToRet();
            var b2 = Byte.MaxValue.Range().Where(e => e % 3 == 1).ToRet();
            var b3 = Byte.MaxValue.Range().Where(e => e % 4 == 1).ToRet();
            Assert.IsTrue(!b1.IsStrictSubsetOf(b1));
            Assert.IsTrue(!b2.IsStrictSubsetOf(b2));
            Assert.IsTrue(!b3.IsStrictSubsetOf(b3));
            Assert.IsTrue(!b1.IsStrictSubsetOf(b2));
            Assert.IsTrue(!b1.IsStrictSubsetOf(b3));
            Assert.IsTrue(!b2.IsStrictSubsetOf(b1));
            Assert.IsTrue(!b2.IsStrictSubsetOf(b3));
            Assert.IsTrue(!b3.IsStrictSubsetOf(b2));
            Assert.IsTrue(b3.IsStrictSubsetOf(b1));
        }
        [TestMethod]
        public void IsDisjointFromTest() {
            Util.ExpectException<ArgumentException>(() => ((IRet<int>)null).IsDisjointFrom(new[] { 1 }.ToRet()));
            Util.ExpectException<ArgumentException>(() => new[] { 1 }.ToRet().IsDisjointFrom(null));
            var b1 = Byte.MaxValue.Range().Where(e => e % 2 == 1).ToRet();
            var b2 = Byte.MaxValue.Range().Where(e => e % 3 == 1).ToRet();
            var b3 = Byte.MaxValue.Range().Where(e => e % 3 == 2).ToRet();
            Assert.IsTrue(!b1.IsDisjointFrom(b2));
            Assert.IsTrue(!b1.IsDisjointFrom(b3));
            Assert.IsTrue(b2.IsDisjointFrom(b3));
            Assert.IsTrue(!b2.IsDisjointFrom(b1));
            Assert.IsTrue(!b3.IsDisjointFrom(b1));
            Assert.IsTrue(b3.IsDisjointFrom(b2));
        }
    }
}
