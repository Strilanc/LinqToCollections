using LinqToCollections.List;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LinqToCollectionsTest {
    [TestClass]
    public class ReadOnlyListExtensionsTest {
        [TestMethod]
        public void AsReadOnlyListTest() {
            Util.ExpectException<ArgumentException>(() => ((IList<int>)null).AsReadOnlyList());

            Assert.IsTrue(new[] { 0, 1, 2 }.AsReadOnlyList().Count == 3);
            Assert.IsTrue(new[] { 0, 2, 5, 7 }.AsReadOnlyList().SequenceEqual(new[] { 0, 2, 5, 7 }));
            var list = new List<int>() { 2, 3 };
            var ReadOnlyList = list.AsReadOnlyList();
            Assert.IsTrue(ReadOnlyList.SequenceEqual(new[] { 2, 3 }));
            list.Add(5);
            Assert.IsTrue(ReadOnlyList.SequenceEqual(new[] { 2, 3, 5 }));
        }
        [TestMethod]
        public void AsElseToReadOnlyListTest() {
            Util.ExpectException<ArgumentException>(() => ((IEnumerable<int>)null).AsElseToReadOnlyList());

            var list = new List<int>() { 2, 3 };
            var listAsReadOnlyList = list.AsElseToReadOnlyList();
            var projectionAsReadOnlyList = list.Select(i => i + 1).AsElseToReadOnlyList();
            var enumerableProjectionAsReadOnlyList = list.AsEnumerable().Select(i => i + 1).AsElseToReadOnlyList();
            
            //IReadOnlyList ==> gives back input
            Assert.ReferenceEquals(listAsReadOnlyList, listAsReadOnlyList.AsElseToReadOnlyList());

            //IList ==> viewed as ReadOnlyList, IEnumerable ==> copied
            Assert.IsTrue(listAsReadOnlyList.SequenceEqual(new[] { 2, 3 }));
            Assert.IsTrue(projectionAsReadOnlyList.SequenceEqual(new[] { 3, 4 }));
            Assert.IsTrue(enumerableProjectionAsReadOnlyList.SequenceEqual(new[] { 3, 4 }));
            
            //IList ==> viewed as ReadOnlyList, IEnumerable ==> copied
            list.RemoveAt(1);
            Assert.IsTrue(listAsReadOnlyList.SequenceEqual(new[] { 2 }));
            Assert.IsTrue(projectionAsReadOnlyList.SequenceEqual(new[] { 3 }));
            Assert.IsTrue(enumerableProjectionAsReadOnlyList.SequenceEqual(new[] { 3, 4 }));
        }
        [TestMethod]
        public void ToRistTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).ToReadOnlyList());
            
            var list = new List<int>() { 2, 3 };
            var ReadOnlyList = list.ToReadOnlyList();
            Assert.IsTrue(ReadOnlyList.SequenceEqual(new[] { 2, 3 }));
            list.Add(1);
            Assert.IsTrue(ReadOnlyList.SequenceEqual(new[] { 2, 3 }));
        }

        [TestMethod]
        public void SkipExactTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).SkipExact(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().SkipExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().SkipExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().SkipExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().SkipExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().SkipExact(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().SkipExact(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().SkipExact(1).SequenceEqual(new[] { 2, 3, 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().SkipExact(3).SequenceEqual(new[] { 4 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.SkipExact(2);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] {3}));
            L.Insert(0, 0);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 2, 3 }));
            L.Remove(3);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 2 }));
            L.Remove(1);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(M.SequenceEqual(new int[0]));
            L.Remove(2);
            Util.ExpectException<InvalidOperationException>(() => { var x = M.Count; });
            Util.ExpectException<InvalidOperationException>(() => { var x = M[0]; });
            Util.ExpectException<InvalidOperationException>(() => M.GetEnumerator());
        }
        [TestMethod]
        public void SkipLastExactTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).SkipLastExact(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().SkipLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().SkipLastExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().SkipLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().SkipLastExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().SkipLastExact(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().SkipLastExact(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().SkipLastExact(1).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().SkipLastExact(3).SequenceEqual(new[] { 1 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.SkipLastExact(2);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 1 }));
            L.Insert(0, 0);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 1 }));
            L.Remove(3);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 0 }));
            L.Remove(1);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(M.SequenceEqual(new int[0]));
            L.Remove(2);
            Util.ExpectException<InvalidOperationException>(() => { var x = M.Count; });
            Util.ExpectException<InvalidOperationException>(() => { var x = M[0]; });
            Util.ExpectException<InvalidOperationException>(() => M.GetEnumerator());
        }
        [TestMethod]
        public void TakeExactTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).TakeExact(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().TakeExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().TakeExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().TakeExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().TakeExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().TakeExact(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().TakeExact(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().TakeExact(1).SequenceEqual(new[] { 1 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().TakeExact(3).SequenceEqual(new[] { 1, 2, 3 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.TakeExact(2);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 1, 2 }));
            L.Insert(0, 0);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 1 }));
            L.Remove(3);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 1 }));
            L.Remove(1);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 2 }));
            L.Remove(2);
            Util.ExpectException<InvalidOperationException>(() => { var x = M.Count; });
            Util.ExpectException<InvalidOperationException>(() => { var x = M[0]; });
            Util.ExpectException<InvalidOperationException>(() => M.Any());
        }
        [TestMethod]
        public void TakeLastExactTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).TakeLastExact(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().TakeLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().TakeLastExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().TakeLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().TakeLastExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().TakeLastExact(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().TakeLastExact(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().TakeLastExact(1).SequenceEqual(new[] { 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().TakeLastExact(3).SequenceEqual(new[] { 2, 3, 4 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.TakeLastExact(2);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 2, 3 }));
            L.Insert(0, 0);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 2, 3 }));
            L.Remove(3);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 1, 2 }));
            L.Remove(1);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 2 }));
            L.Remove(2);
            Util.ExpectException<InvalidOperationException>(() => { var x = M.Count; });
            Util.ExpectException<InvalidOperationException>(() => { var x = M[0]; });
            Util.ExpectException<InvalidOperationException>(() => M.Any());
        }

        [TestMethod]
        public void SkipTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).Skip(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().Skip(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().Skip(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().Skip(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().Skip(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().Skip(4).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().Skip(10).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().Skip(1).SequenceEqual(new[] { 2, 3, 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().Skip(3).SequenceEqual(new[] { 4 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.Skip(2);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 3 }));
            L.Insert(0, 0);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 2, 3 }));
            L.Remove(3);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 2 }));
            L.Remove(1);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(M.SequenceEqual(new int[0]));
            L.Remove(2);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(M.SequenceEqual(new int[0]));
            L.Remove(0);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(M.SequenceEqual(new int[0]));
        }
        [TestMethod]
        public void SkipLastTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).SkipLast(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().SkipLast(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().SkipLast(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().SkipLast(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().SkipLast(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().SkipLast(4).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().SkipLast(10).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().SkipLast(1).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().SkipLast(3).SequenceEqual(new[] { 1 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.SkipLast(2);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 1 }));
            L.Insert(0, 0);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 1 }));
            L.Remove(3);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 0 }));
            L.Remove(1);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(M.SequenceEqual(new int[0]));
            L.Remove(2);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(M.SequenceEqual(new int[0]));
            L.Remove(0);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(M.SequenceEqual(new int[0]));
        }
        [TestMethod]
        public void TakeTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).Take(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().Take(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().Take(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().Take(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().Take(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().Take(4).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().Take(10).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().Take(1).SequenceEqual(new[] { 1 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().Take(3).SequenceEqual(new[] { 1, 2, 3 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.Take(2);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 1, 2 }));
            L.Insert(0, 0);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 1 }));
            L.Remove(3);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 1 }));
            L.Remove(1);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 2 }));
            L.Remove(2);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 0 }));
            L.Remove(0);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(!M.Any());
        }
        [TestMethod]
        public void MultiSkipTest() {
            var L = new List<int> { 1, 2, 3, 4, 5 };
            Assert.IsTrue(!L.SkipExact(5).Skip(1).Any());
            Util.ExpectException<ArgumentException>(() => L.Skip(1).SkipExact(5));
            Assert.IsTrue(!L.SkipExact(3).SkipExact(2).Any());
            Util.ExpectException<ArgumentException>(() => L.SkipExact(3).SkipExact(3));
            Assert.IsTrue(L.SkipExact(1).Skip(2).SkipExact(1).SequenceEqual(new[] {5}));
            Util.ExpectException<ArgumentException>(() => L.SkipExact(1).Skip(2).SkipExact(3));
            Assert.IsTrue(L.SkipLast(2).Skip(2).SequenceEqual(new[] { 3 }));
            Assert.IsTrue(L.SkipLastExact(2).Skip(2).SequenceEqual(new[] { 3 }));
            Assert.IsTrue(L.SkipLastExact(2).SkipExact(2).SequenceEqual(new[] { 3 }));
            Assert.IsTrue(L.SkipExact(2).SkipLastExact(2).SequenceEqual(new[] { 3 }));
            var M = L.Skip(0).Skip(1).SkipLast(1).SkipExact(1).SkipLastExact(1);
            Assert.IsTrue(M.SequenceEqual(new[] { 3 }));
            Assert.IsTrue(!M.SkipLastExact(1).Any());
            Assert.IsTrue(!M.SkipExact(1).Any());
            Util.ExpectException<ArgumentException>(() => M.SkipExact(2));
            Assert.IsTrue(!M.SkipExact(1).Skip(1).Any());
            Assert.IsTrue(!M.SkipExact(1).SkipLast(1).Any());
            Util.ExpectException<ArgumentException>(() => M.SkipExact(1).SkipLastExact(1));
            Util.ExpectException<ArgumentException>(() => M.Skip(1).SkipLastExact(1));
        }
        [TestMethod]
        public void TakeLastTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).TakeLast(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyList().TakeLast(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyList().TakeLast(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().TakeLast(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().TakeLast(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().TakeLast(4).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyList().TakeLast(10).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().TakeLast(1).SequenceEqual(new[] { 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyList().TakeLast(3).SequenceEqual(new[] { 2, 3, 4 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.TakeLast(2);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 2, 3 }));
            L.Insert(0, 0);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 2, 3 }));
            L.Remove(3);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 1, 2 }));
            L.Remove(1);
            Assert.IsTrue(M.Count == 2);
            Assert.IsTrue(M.SequenceEqual(new[] { 0, 2 }));
            L.Remove(2);
            Assert.IsTrue(M.Count == 1);
            Assert.IsTrue(M.SequenceEqual(new[] { 0 }));
            L.Remove(0);
            Assert.IsTrue(M.Count == 0);
            Assert.IsTrue(!M.Any());
        }

        [TestMethod]
        public void SelectTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).Select(i => i));
            Util.ExpectException<ArgumentException>(() => new int[0].AsReadOnlyList().Select((Func<int, int>)null));

            Assert.IsTrue(5.Range().Select(i => i * i).SequenceEqual(new[] { 0, 1, 4, 9, 16 }));
            Assert.IsTrue(0.Range().Select(i => i * i).SequenceEqual(new int[0]));
            Assert.IsTrue(4.Range().Select(i => i + i).SequenceEqual(new[] { 0, 2, 4, 6 }));
        }
        [TestMethod]
        public void Select2Test() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).Select((e, i) => i));
            Util.ExpectException<ArgumentException>(() => new int[0].AsReadOnlyList().Select((Func<int, int, int>)null));

            Assert.IsTrue(5.Range().Select((e, i) => i * e).SequenceEqual(new[] { 0, 1, 4, 9, 16 }));
            Assert.IsTrue(0.Range().Select((e, i) => i * e).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyList().Select((e, i) => e).SequenceEqual(new[] { 2, 3, 5 }));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyList().Select((e, i) => i).SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyList().Select((e, i) => e + i).SequenceEqual(new[] { 2, 4, 7 }));
        }
        [TestMethod]
        public void ZipTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).Zip(new int[0], (i1, i2) => i1));
            Util.ExpectException<ArgumentException>(() => new int[0].AsReadOnlyList().Zip((IReadOnlyList<int>)null, (i1, i2) => i1));
            Util.ExpectException<ArgumentException>(() => new int[0].AsReadOnlyList().Zip(new int[0], (Func<int, int, int>)null));

            Assert.IsTrue(5.Range().Zip(4.Range(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(4.Range().Zip(5.Range(), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(5.Range().Zip(new[] {true, false, true}, (e1, e2) => e2 ? e1 : -e1).SequenceEqual(new[] { 0, -1, 2 }));
        }

        [TestMethod]
        public void ReverseTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).Reverse());

            Assert.IsTrue(0.Range().Reverse().SequenceEqual(new int[0]));
            Assert.IsTrue(5.Range().Reverse().SequenceEqual(new[] { 4, 3, 2, 1, 0 }));
            Assert.IsTrue(4.Range().Reverse().SequenceEqual(new[] { 3, 2, 1, 0 }));
        }

        [TestMethod]
        public void LastTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).Last());
            Util.ExpectException<ArgumentException>(() => 0.Range().Last());

            Assert.IsTrue(1.Range().Last() == 0);
            Assert.IsTrue(2.Range().Last() == 1);
            Assert.IsTrue(int.MaxValue.Range().Last() == int.MaxValue - 1);
            Assert.IsTrue(new ReadOnlyList<int>(counter: () => 11, getter: i => { if (i < 10) throw new ArgumentException(); else return i;}).Last() == 10);
        }
        [TestMethod]
        public void LastOrDefaultTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).LastOrDefault());
            Util.ExpectException<ArgumentException>(() => 0.Range().Last());

            Assert.IsTrue(0.Range().LastOrDefault() == 0);
            Assert.IsTrue(0.Range().LastOrDefault(5) == 5);
            Assert.IsTrue(1.Range().LastOrDefault() == 0);
            Assert.IsTrue(1.Range().LastOrDefault(5) == 0);
            Assert.IsTrue(2.Range().LastOrDefault() == 1);
            Assert.IsTrue(2.Range().LastOrDefault(5) == 1);
            Assert.IsTrue(int.MaxValue.Range().LastOrDefault() == int.MaxValue - 1);
            Assert.IsTrue(new ReadOnlyList<int>(counter: () => 11, getter: i => { if (i < 10) throw new ArgumentException(); else return i; }).LastOrDefault() == 10);
        }

        [TestMethod]
        public void AsIListTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyList<int>)null).AsIList());
            
            var rawList = new List<int>() {0, 1, 2};
            var AsReadOnlyList = rawList.AsReadOnlyList();
            var AsReadOnlyListAsList = rawList.AsReadOnlyList().AsIList();
            Assert.IsTrue(AsReadOnlyListAsList.IsReadOnly);
            Assert.IsTrue(AsReadOnlyListAsList.Count == 3);
            Assert.IsTrue(AsReadOnlyListAsList.SequenceEqual(new[] { 0, 1, 2 }));
            Assert.ReferenceEquals(AsReadOnlyList, AsReadOnlyListAsList);
            Assert.ReferenceEquals(AsReadOnlyList, AsReadOnlyListAsList.AsReadOnlyList());
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyListAsList.Add(0));
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyListAsList.Remove(0));
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyListAsList.Insert(0, 0));
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyListAsList.RemoveAt(0));
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyListAsList.Clear());
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyListAsList[0] = 0);

            var asList = 5.Range().AsIList();
            var asListAsReadOnlyList = asList.AsReadOnlyList();
            Assert.ReferenceEquals(asListAsReadOnlyList, asList);
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
