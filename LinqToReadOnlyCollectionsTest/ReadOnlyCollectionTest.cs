using LinqToCollections.Collection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToCollectionsTest {
    [TestClass]
    public class ReadOnlyCollectionTest {
        private static IReadOnlyCollection<int> rng(int i) {
            return Enumerable.Range(0, i).ToArray();
        }

        [TestMethod]
        public void ConstructorTest() {
            Util.ExpectException<ArgumentException>(() => new ReadOnlyCollection<int>(counter: null, iterator: Enumerable.Range(0, 0).GetEnumerator));
            Util.ExpectException<ArgumentException>(() => new ReadOnlyCollection<int>(counter: () => 0, iterator: null));

            var r = new ReadOnlyCollection<Int32>(counter: () => 5, iterator: Enumerable.Range(0, 5).GetEnumerator);
            Assert.IsTrue(r.Count == 5);
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2, 3, 4 }));

            var r2 = new ReadOnlyCollection<Int32>(counter: () => 7, iterator: Enumerable.Range(0, 7).Select(i => 1 - i).GetEnumerator);
            Assert.IsTrue(r2.Count == 7);
            Assert.IsTrue(r2.SequenceEqual(new[] { 1, 0, -1, -2, -3, -4, -5 }));
        }

        [TestMethod]
        public void EfficientIteratorTest() {
            var r = new ReadOnlyCollection<Int32>(counter: () => { throw new ArgumentException(); }, iterator: Enumerable.Range(0, 3).GetEnumerator);
            Util.ExpectException<ArgumentException>(() => { var x = r.Count; });
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2 }));
        }

        [TestMethod]
        public void AsReadOnlyCollectionTest() {
            Util.ExpectException<ArgumentException>(() => ((ICollection<int>)null).AsReadOnlyCollection());

            Assert.IsTrue(new[] { 0, 1, 2 }.AsReadOnlyCollection().Count == 3);
            Assert.IsTrue(new[] { 0, 2, 5, 7 }.AsReadOnlyCollection().SequenceEqual(new[] { 0, 2, 5, 7 }));
            var collection = new List<int>() { 2, 3 };
            var ReadOnlyCollection = collection.AsReadOnlyCollection();
            Assert.IsTrue(ReadOnlyCollection.SequenceEqual(new[] { 2, 3 }));
            collection.Add(5);
            Assert.IsTrue(ReadOnlyCollection.SequenceEqual(new[] { 2, 3, 5 }));
        }
        [TestMethod]
        public void AsElseToReadOnlyCollectionTest() {
            Util.ExpectException<ArgumentException>(() => ((IEnumerable<int>)null).AsElseToReadOnlyCollection());

            var collection = new List<int>() { 2, 3 };
            var collectionAsReadOnlyCollection = collection.AsElseToReadOnlyCollection();
            var projectionAsReadOnlyCollection = collection.Select(i => i + 1).AsElseToReadOnlyCollection();
            var enumerableProjectionAsReadOnlyCollection = collection.AsEnumerable().Select(i => i + 1).AsElseToReadOnlyCollection();

            //IReadOnlyCollection ==> gives back input
            Assert.ReferenceEquals(collectionAsReadOnlyCollection, collectionAsReadOnlyCollection.AsElseToReadOnlyCollection());

            //ICollection ==> viewed as ReadOnlyCollection, IEnumerable ==> copied
            Assert.IsTrue(collectionAsReadOnlyCollection.SequenceEqual(new[] { 2, 3 }));
            Assert.IsTrue(projectionAsReadOnlyCollection.SequenceEqual(new[] { 3, 4 }));
            Assert.IsTrue(enumerableProjectionAsReadOnlyCollection.SequenceEqual(new[] { 3, 4 }));

            //ICollection ==> viewed as ReadOnlyCollection, IEnumerable ==> copied
            collection.RemoveAt(1);
            Assert.IsTrue(collectionAsReadOnlyCollection.SequenceEqual(new[] { 2 }));
            Assert.IsTrue(projectionAsReadOnlyCollection.SequenceEqual(new[] { 3 }));
            Assert.IsTrue(enumerableProjectionAsReadOnlyCollection.SequenceEqual(new[] { 3, 4 }));
        }
        [TestMethod]
        public void ToRistTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).ToReadOnlyCollection());

            var collection = new List<int>() { 2, 3 };
            var ReadOnlyCollection = collection.ToReadOnlyCollection();
            Assert.IsTrue(ReadOnlyCollection.SequenceEqual(new[] { 2, 3 }));
            collection.Add(1);
            Assert.IsTrue(ReadOnlyCollection.SequenceEqual(new[] { 2, 3 }));
        }

        [TestMethod]
        public void SkipExactTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).SkipExact(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipExact(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipExact(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipExact(1).SequenceEqual(new[] { 2, 3, 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipExact(3).SequenceEqual(new[] { 4 }));

            // mutations propagate
            var L = new List<int> { 1, 2, 3 };
            var M = L.SkipExact(2);
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
            Util.ExpectException<InvalidOperationException>(() => { var x = M.Count; });
            Util.ExpectException<InvalidOperationException>(() => M.GetEnumerator());
        }
        [TestMethod]
        public void SkipLastExactTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).SkipLastExact(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLastExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipLastExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLastExact(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLastExact(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipLastExact(1).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipLastExact(3).SequenceEqual(new[] { 1 }));

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
            Util.ExpectException<InvalidOperationException>(() => M.GetEnumerator());
        }
        [TestMethod]
        public void TakeExactTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).TakeExact(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeExact(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeExact(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeExact(1).SequenceEqual(new[] { 1 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeExact(3).SequenceEqual(new[] { 1, 2, 3 }));

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
            Util.ExpectException<InvalidOperationException>(() => M.GetEnumerator());
        }
        [TestMethod]
        public void TakeLastExactTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).TakeLastExact(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLastExact(4));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeLastExact(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeLastExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLastExact(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLastExact(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeLastExact(1).SequenceEqual(new[] { 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeLastExact(3).SequenceEqual(new[] { 2, 3, 4 }));

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
            Util.ExpectException<InvalidOperationException>(() => M.GetEnumerator());
        }

        [TestMethod]
        public void SkipTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Skip(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().Skip(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(4).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(10).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().Skip(1).SequenceEqual(new[] { 2, 3, 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().Skip(3).SequenceEqual(new[] { 4 }));

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
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).SkipLast(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipLast(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(4).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(10).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipLast(1).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipLast(3).SequenceEqual(new[] { 1 }));

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
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Take(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().Take(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(4).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(10).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().Take(1).SequenceEqual(new[] { 1 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().Take(3).SequenceEqual(new[] { 1, 2, 3 }));

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
            Assert.IsTrue(L.SkipExact(1).Skip(2).SkipExact(1).SequenceEqual(new[] { 5 }));
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
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).TakeLast(0));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(-1));
            Util.ExpectException<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeLast(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(4).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(10).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeLast(1).SequenceEqual(new[] { 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeLast(3).SequenceEqual(new[] { 2, 3, 4 }));

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
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Select(i => i));
            Util.ExpectException<ArgumentException>(() => new int[0].AsReadOnlyCollection().Select((Func<int, int>)null));

            Assert.IsTrue(rng(5).Select(i => i * i).SequenceEqual(new[] { 0, 1, 4, 9, 16 }));
            Assert.IsTrue(rng(0).Select(i => i * i).SequenceEqual(new int[0]));
            Assert.IsTrue(rng(4).Select(i => i + i).SequenceEqual(new[] { 0, 2, 4, 6 }));
        }
        [TestMethod]
        public void Select2Test() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Select((e, i) => i));
            Util.ExpectException<ArgumentException>(() => new int[0].AsReadOnlyCollection().Select((Func<int, int, int>)null));

            Assert.IsTrue(rng(5).Select((e, i) => i * e).SequenceEqual(new[] { 0, 1, 4, 9, 16 }));
            Assert.IsTrue(rng(0).Select((e, i) => i * e).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyCollection().Select((e, i) => e).SequenceEqual(new[] { 2, 3, 5 }));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyCollection().Select((e, i) => i).SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyCollection().Select((e, i) => e + i).SequenceEqual(new[] { 2, 4, 7 }));
        }
        [TestMethod]
        public void ZipTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Zip(new int[0], (i1, i2) => i1));
            Util.ExpectException<ArgumentException>(() => new int[0].AsReadOnlyCollection().Zip((IReadOnlyCollection<int>)null, (i1, i2) => i1));
            Util.ExpectException<ArgumentException>(() => new int[0].AsReadOnlyCollection().Zip(new int[0], (Func<int, int, int>)null));

            Assert.IsTrue(rng(5).Zip(rng(4), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(rng(4).Zip(rng(5), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(rng(5).Zip(new[] { true, false, true }, (e1, e2) => e2 ? e1 : -e1).SequenceEqual(new[] { 0, -1, 2 }));
        }

        [TestMethod]
        public void ReverseTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Reverse());

            Assert.IsTrue(rng(0).Reverse().SequenceEqual(new int[0]));
            Assert.IsTrue(rng(5).Reverse().SequenceEqual(new[] { 4, 3, 2, 1, 0 }));
            Assert.IsTrue(rng(4).Reverse().SequenceEqual(new[] { 3, 2, 1, 0 }));
        }

        [TestMethod]
        public void LastOrDefaultTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).LastOrDefault());
            Assert.IsTrue(rng(0).LastOrDefault() == 0);
            Assert.IsTrue(rng(0).LastOrDefault(5) == 5);
            Assert.IsTrue(rng(1).LastOrDefault() == 0);
            Assert.IsTrue(rng(1).LastOrDefault(5) == 0);
            Assert.IsTrue(rng(2).LastOrDefault() == 1);
            Assert.IsTrue(rng(2).LastOrDefault(5) == 1);
        }

        [TestMethod]
        public void AsICollectionTest() {
            Util.ExpectException<ArgumentException>(() => ((IReadOnlyCollection<int>)null).AsICollection());

            var rawCollection = new List<int>() { 0, 1, 2 };
            var AsReadOnlyCollection = rawCollection.AsReadOnlyCollection();
            var AsReadOnlyCollectionAsCollection = rawCollection.AsReadOnlyCollection().AsICollection();
            Assert.IsTrue(AsReadOnlyCollectionAsCollection.IsReadOnly);
            Assert.IsTrue(AsReadOnlyCollectionAsCollection.Count == 3);
            Assert.IsTrue(AsReadOnlyCollectionAsCollection.SequenceEqual(new[] { 0, 1, 2 }));
            Assert.ReferenceEquals(AsReadOnlyCollection, AsReadOnlyCollectionAsCollection);
            Assert.ReferenceEquals(AsReadOnlyCollection, AsReadOnlyCollectionAsCollection.AsReadOnlyCollection());
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyCollectionAsCollection.Add(0));
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyCollectionAsCollection.Remove(0));
            Util.ExpectException<NotSupportedException>(() => AsReadOnlyCollectionAsCollection.Clear());

            var asCollection = rng(5).AsICollection();
            var asCollectionAsReadOnlyCollection = asCollection.AsReadOnlyCollection();
            Assert.ReferenceEquals(asCollectionAsReadOnlyCollection, asCollection);
            Assert.IsTrue(asCollection.SequenceEqual(new[] { 0, 1, 2, 3, 4 }));
            Assert.IsTrue(asCollection.IsReadOnly);
            Assert.IsTrue(asCollection.Count == 5);
            Assert.IsTrue(asCollection.Contains(4));
            Assert.IsTrue(asCollection.Contains(1));
            Assert.IsTrue(asCollection.Contains(0));
            Assert.IsTrue(!asCollection.Contains(-1));

            var d = new int[10];
            asCollection.CopyTo(d, 2);
            Assert.IsTrue(d.SequenceEqual(new[] { 0, 0, 0, 1, 2, 3, 4, 0, 0, 0 }));
        }
    }
}
