using LinqToReadOnlyCollections.Collection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToReadOnlyCollectionsTest {
    [TestClass]
    public class ReadOnlyCollectionTest {
        private static IReadOnlyCollection<int> Rng(int i) {
            return Enumerable.Range(0, i).ToArray();
        }

        [TestMethod]
        public void ConstructorTest() {
            TestUtil.AssertThrows<ArgumentException>(() => new ReadOnlyCollection<int>(counter: null, iterator: Enumerable.Range(0, 0).GetEnumerator));
            TestUtil.AssertThrows<ArgumentException>(() => new ReadOnlyCollection<int>(counter: () => 0, iterator: null));

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
            TestUtil.AssertThrows<ArgumentException>((() => r.Count));
            Assert.IsTrue(r.SequenceEqual(new[] { 0, 1, 2 }));
        }

        [TestMethod]
        public void AsReadOnlyCollectionTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((ICollection<int>)null).AsReadOnlyCollection());

            Assert.IsTrue(new[] { 0, 1, 2 }.AsReadOnlyCollection().Count == 3);
            Assert.IsTrue(new[] { 0, 2, 5, 7 }.AsReadOnlyCollection().SequenceEqual(new[] { 0, 2, 5, 7 }));
            var collection = new List<int> { 2, 3 };
            var asReadOnlyCollection = collection.AsReadOnlyCollection();
            Assert.IsTrue(asReadOnlyCollection.SequenceEqual(new[] { 2, 3 }));
            collection.Add(5);
            Assert.IsTrue(asReadOnlyCollection.SequenceEqual(new[] { 2, 3, 5 }));
        }
        [TestMethod]
        public void AsElseToReadOnlyCollectionTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IEnumerable<int>)null).AsElseToReadOnlyCollection());

            var collection = new List<int> { 2, 3 };
            var collectionAsReadOnlyCollection = collection.AsElseToReadOnlyCollection();
            var projectionAsReadOnlyCollection = collection.Select(i => i + 1).AsElseToReadOnlyCollection();
            var enumerableProjectionAsReadOnlyCollection = collection.AsEnumerable().Select(i => i + 1).AsElseToReadOnlyCollection();

            //IReadOnlyCollection ==> gives back input
            Assert.AreSame(collectionAsReadOnlyCollection, collectionAsReadOnlyCollection.AsElseToReadOnlyCollection());

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
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).ToReadOnlyCollection());

            var collection = new List<int> { 2, 3 };
            var toReadOnlyCollection = collection.ToReadOnlyCollection();
            Assert.IsTrue(toReadOnlyCollection.SequenceEqual(new[] { 2, 3 }));
            collection.Add(1);
            Assert.IsTrue(toReadOnlyCollection.SequenceEqual(new[] { 2, 3 }));
        }

        [TestMethod]
        public void SkipExactTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).SkipExact(0));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipExact(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipExact(4));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipExact(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipExact(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipExact(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipExact(1).SequenceEqual(new[] { 2, 3, 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipExact(3).SequenceEqual(new[] { 4 }));

            // mutations propagate
            var li = new List<int> { 1, 2, 3 };
            var mi = li.SkipExact(2);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 3 }));
            li.Insert(0, 0);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 2, 3 }));
            li.Remove(3);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 2 }));
            li.Remove(1);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(mi.SequenceEqual(new int[0]));
            li.Remove(2);
            TestUtil.AssertThrows<InvalidOperationException>((() => mi.Count));
            TestUtil.AssertThrows<InvalidOperationException>(() => mi.GetEnumerator());
        }
        [TestMethod]
        public void SkipLastExactTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).SkipLastExact(0));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLastExact(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLastExact(4));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipLastExact(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipLastExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLastExact(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLastExact(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipLastExact(1).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipLastExact(3).SequenceEqual(new[] { 1 }));

            // mutations propagate
            var li = new List<int> { 1, 2, 3 };
            var mi = li.SkipLastExact(2);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 1 }));
            li.Insert(0, 0);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 1 }));
            li.Remove(3);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0 }));
            li.Remove(1);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(mi.SequenceEqual(new int[0]));
            li.Remove(2);
            TestUtil.AssertThrows<InvalidOperationException>((() => mi.Count));
            TestUtil.AssertThrows<InvalidOperationException>(() => mi.GetEnumerator());
        }
        [TestMethod]
        public void TakeExactTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).TakeExact(0));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeExact(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeExact(4));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeExact(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeExact(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeExact(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeExact(1).SequenceEqual(new[] { 1 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeExact(3).SequenceEqual(new[] { 1, 2, 3 }));

            // mutations propagate
            var li = new List<int> { 1, 2, 3 };
            var mi = li.TakeExact(2);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 1, 2 }));
            li.Insert(0, 0);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 1 }));
            li.Remove(3);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 1 }));
            li.Remove(1);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 2 }));
            li.Remove(2);
            TestUtil.AssertThrows<InvalidOperationException>((() => mi.Count));
            TestUtil.AssertThrows<InvalidOperationException>(() => mi.GetEnumerator());
        }
        [TestMethod]
        public void TakeLastExactTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).TakeLastExact(0));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLastExact(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLastExact(4));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeLastExact(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeLastExact(3));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLastExact(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLastExact(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeLastExact(1).SequenceEqual(new[] { 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeLastExact(3).SequenceEqual(new[] { 2, 3, 4 }));

            // mutations propagate
            var li = new List<int> { 1, 2, 3 };
            var mi = li.TakeLastExact(2);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 2, 3 }));
            li.Insert(0, 0);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 2, 3 }));
            li.Remove(3);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 1, 2 }));
            li.Remove(1);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 2 }));
            li.Remove(2);
            TestUtil.AssertThrows<InvalidOperationException>((() => mi.Count));
            TestUtil.AssertThrows<InvalidOperationException>(() => mi.GetEnumerator());
        }

        [TestMethod]
        public void SkipTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Skip(0));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().Skip(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(4).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Skip(10).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().Skip(1).SequenceEqual(new[] { 2, 3, 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().Skip(3).SequenceEqual(new[] { 4 }));

            // mutations propagate
            var li = new List<int> { 1, 2, 3 };
            var mi = li.Skip(2);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 3 }));
            li.Insert(0, 0);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 2, 3 }));
            li.Remove(3);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 2 }));
            li.Remove(1);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(mi.SequenceEqual(new int[0]));
            li.Remove(2);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(mi.SequenceEqual(new int[0]));
            li.Remove(0);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(mi.SequenceEqual(new int[0]));
        }
        [TestMethod]
        public void SkipLastTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).SkipLast(0));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().SkipLast(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(3).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(4).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().SkipLast(10).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipLast(1).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().SkipLast(3).SequenceEqual(new[] { 1 }));

            // mutations propagate
            var li = new List<int> { 1, 2, 3 };
            var mi = li.SkipLast(2);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 1 }));
            li.Insert(0, 0);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 1 }));
            li.Remove(3);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0 }));
            li.Remove(1);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(mi.SequenceEqual(new int[0]));
            li.Remove(2);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(mi.SequenceEqual(new int[0]));
            li.Remove(0);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(mi.SequenceEqual(new int[0]));
        }
        [TestMethod]
        public void TakeTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Take(0));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().Take(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(4).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().Take(10).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().Take(1).SequenceEqual(new[] { 1 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().Take(3).SequenceEqual(new[] { 1, 2, 3 }));

            // mutations propagate
            var li = new List<int> { 1, 2, 3 };
            var mi = li.Take(2);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 1, 2 }));
            li.Insert(0, 0);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 1 }));
            li.Remove(3);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 1 }));
            li.Remove(1);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 2 }));
            li.Remove(2);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0 }));
            li.Remove(0);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(!mi.Any());
        }
        [TestMethod]
        public void MultiSkipTest() {
            var li = new List<int> { 1, 2, 3, 4, 5 };
            Assert.IsTrue(!li.SkipExact(5).Skip(1).Any());
            TestUtil.AssertThrows<ArgumentException>(() => li.Skip(1).SkipExact(5));
            Assert.IsTrue(!li.SkipExact(3).SkipExact(2).Any());
            TestUtil.AssertThrows<ArgumentException>(() => li.SkipExact(3).SkipExact(3));
            Assert.IsTrue(li.SkipExact(1).Skip(2).SkipExact(1).SequenceEqual(new[] { 5 }));
            TestUtil.AssertThrows<ArgumentException>(() => li.SkipExact(1).Skip(2).SkipExact(3));
            Assert.IsTrue(li.SkipLast(2).Skip(2).SequenceEqual(new[] { 3 }));
            Assert.IsTrue(li.SkipLastExact(2).Skip(2).SequenceEqual(new[] { 3 }));
            Assert.IsTrue(li.SkipLastExact(2).SkipExact(2).SequenceEqual(new[] { 3 }));
            Assert.IsTrue(li.SkipExact(2).SkipLastExact(2).SequenceEqual(new[] { 3 }));
            var mi = li.Skip(0).Skip(1).SkipLast(1).SkipExact(1).SkipLastExact(1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 3 }));
            Assert.IsTrue(!mi.SkipLastExact(1).Any());
            Assert.IsTrue(!mi.SkipExact(1).Any());
            TestUtil.AssertThrows<ArgumentException>(() => mi.SkipExact(2));
            Assert.IsTrue(!mi.SkipExact(1).Skip(1).Any());
            Assert.IsTrue(!mi.SkipExact(1).SkipLast(1).Any());
            TestUtil.AssertThrows<ArgumentException>(() => mi.SkipExact(1).SkipLastExact(1));
            TestUtil.AssertThrows<ArgumentException>(() => mi.Skip(1).SkipLastExact(1));
        }
        [TestMethod]
        public void TakeLastTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).TakeLast(0));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(-1));
            TestUtil.AssertThrows<ArgumentException>(() => new[] { 1, 2 }.AsReadOnlyCollection().TakeLast(-1));

            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(0).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(3).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(4).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3 }.AsReadOnlyCollection().TakeLast(10).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeLast(1).SequenceEqual(new[] { 4 }));
            Assert.IsTrue(new[] { 1, 2, 3, 4 }.AsReadOnlyCollection().TakeLast(3).SequenceEqual(new[] { 2, 3, 4 }));

            // mutations propagate
            var li = new List<int> { 1, 2, 3 };
            var mi = li.TakeLast(2);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 2, 3 }));
            li.Insert(0, 0);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 2, 3 }));
            li.Remove(3);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 1, 2 }));
            li.Remove(1);
            Assert.IsTrue(mi.Count == 2);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0, 2 }));
            li.Remove(2);
            Assert.IsTrue(mi.Count == 1);
            Assert.IsTrue(mi.SequenceEqual(new[] { 0 }));
            li.Remove(0);
            Assert.IsTrue(mi.Count == 0);
            Assert.IsTrue(!mi.Any());
        }

        [TestMethod]
        public void SelectTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Select(i => i));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyCollection().Select((Func<int, int>)null));

            Assert.IsTrue(Rng(5).Select(i => i * i).SequenceEqual(new[] { 0, 1, 4, 9, 16 }));
            Assert.IsTrue(Rng(0).Select(i => i * i).SequenceEqual(new int[0]));
            Assert.IsTrue(Rng(4).Select(i => i + i).SequenceEqual(new[] { 0, 2, 4, 6 }));
        }
        [TestMethod]
        public void Select2Test() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Select((e, i) => i));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyCollection().Select((Func<int, int, int>)null));

            Assert.IsTrue(Rng(5).Select((e, i) => i * e).SequenceEqual(new[] { 0, 1, 4, 9, 16 }));
            Assert.IsTrue(Rng(0).Select((e, i) => i * e).SequenceEqual(new int[0]));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyCollection().Select((e, i) => e).SequenceEqual(new[] { 2, 3, 5 }));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyCollection().Select((e, i) => i).SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(new[] { 2, 3, 5 }.AsReadOnlyCollection().Select((e, i) => e + i).SequenceEqual(new[] { 2, 4, 7 }));
        }
        [TestMethod]
        public void ZipTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ReadOnlyCollectionExtensions.Zip<int, int, int>(null, new int[0], (i1, i2) => i1));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyCollection().Zip((IReadOnlyCollection<int>)null, (i1, i2) => i1));
            TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyCollection().Zip(new int[0], (Func<int, int, int>)null));

            Assert.IsTrue(Rng(5).Zip(Rng(4), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(Rng(4).Zip(Rng(5), (e1, e2) => e1 + e2).SequenceEqual(new[] { 0, 2, 4, 6 }));
            Assert.IsTrue(Rng(5).Zip(new[] { true, false, true }, (e1, e2) => e2 ? e1 : -e1).SequenceEqual(new[] { 0, -1, 2 }));
        }

        [TestMethod]
        public void ReverseTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).Reverse());

            Assert.IsTrue(Rng(0).Reverse().SequenceEqual(new int[0]));
            Assert.IsTrue(Rng(5).Reverse().SequenceEqual(new[] { 4, 3, 2, 1, 0 }));
            Assert.IsTrue(Rng(4).Reverse().SequenceEqual(new[] { 3, 2, 1, 0 }));
        }

        [TestMethod]
        public void LastOrDefaultTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).LastOrDefault());
            Assert.IsTrue(Rng(0).LastOrDefault() == 0);
            Assert.IsTrue(Rng(0).LastOrDefault(5) == 5);
            Assert.IsTrue(Rng(1).LastOrDefault() == 0);
            Assert.IsTrue(Rng(1).LastOrDefault(5) == 0);
            Assert.IsTrue(Rng(2).LastOrDefault() == 1);
            Assert.IsTrue(Rng(2).LastOrDefault(5) == 1);
        }

        [TestMethod]
        public void AsICollectionTest() {
            TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyCollection<int>)null).AsICollection());

            var rawCollection = new List<int> { 0, 1, 2 };
            var asReadOnlyCollection = rawCollection.AsReadOnlyCollection();
            var asReadOnlyCollectionAsCollection = asReadOnlyCollection.AsICollection();
            var asReadOnlyCollectionAsCollectionAsReadOnlyCollection = asReadOnlyCollectionAsCollection.AsReadOnlyCollection();
            Assert.IsTrue(asReadOnlyCollectionAsCollection.IsReadOnly);
            Assert.IsTrue(asReadOnlyCollectionAsCollection.Count == 3);
            Assert.IsTrue(asReadOnlyCollectionAsCollection.SequenceEqual(new[] { 0, 1, 2 }));
            Assert.AreSame(asReadOnlyCollectionAsCollectionAsReadOnlyCollection, asReadOnlyCollectionAsCollection);
            Assert.AreSame(asReadOnlyCollectionAsCollectionAsReadOnlyCollection, asReadOnlyCollectionAsCollectionAsReadOnlyCollection.AsICollection());
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyCollectionAsCollection.Add(0));
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyCollectionAsCollection.Remove(0));
            TestUtil.AssertThrows<NotSupportedException>(() => asReadOnlyCollectionAsCollection.Clear());

            var asCollection = Rng(5).AsICollection();
            var asCollectionAsReadOnlyCollection = asCollection.AsReadOnlyCollection();
            Assert.AreSame(asCollectionAsReadOnlyCollection, asCollection);
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
