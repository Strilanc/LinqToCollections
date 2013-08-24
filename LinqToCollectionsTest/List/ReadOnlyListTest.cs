using System.Collections.Generic;
using Strilanc.LinqToCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class ReadOnlyListTest {
    [TestMethod]
    public void Select() {
        TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Select(i => i));
        TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyList().Select((Func<int, int>)null));
        TestUtil.AssertThrows<ArgumentException>(() => ((IReadOnlyList<int>)null).Select((e, i) => i));
        TestUtil.AssertThrows<ArgumentException>(() => new int[0].AsReadOnlyList().Select((Func<int, int, int>)null));

        3.Range().Select(i => i * i).AssertListEquals(0, 1, 4);
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
        ReadOnlyList.Repeat("a", 20).AssertListEquals(Enumerable.Repeat("a", 20));
        ReadOnlyList.Repeat("b", 10).AssertListEquals(Enumerable.Repeat("b", 10));
    }
    [TestMethod]
    public void AllValues() {
        ReadOnlyList.AllBytes().Select(e => (int)e).AssertListEquals(Enumerable.Range(Byte.MinValue, Byte.MaxValue + 1 - Byte.MinValue));
        ReadOnlyList.AllSignedBytes().Select(e => (int)e).AssertListEquals(Enumerable.Range(SByte.MinValue, SByte.MaxValue + 1 - SByte.MinValue));
        ReadOnlyList.AllUnsigned16BitIntegers().Select(e => (int)e).AssertListEquals(Enumerable.Range(UInt16.MinValue, UInt16.MaxValue + 1 - UInt16.MinValue));
        ReadOnlyList.AllSigned16BitIntegers().Select(e => (int)e).AssertListEquals(Enumerable.Range(Int16.MinValue, Int16.MaxValue + 1 - Int16.MinValue));
        ReadOnlyList.AllBools().AssertListEquals(false, true);
    }
    [TestMethod]
    public void Partition() {
        TestUtil.AssertThrows<ArgumentNullException>(() => (null as IReadOnlyList<int>).Partition(1));
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => 1.Range().Partition(0));
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => 1.Range().Partition(-1));

        var ps = 6.Range().Select(e => e.Range().Partition(3).Select(f => f.ToArray()).ToArray()).ToArray();
        ps[0].AssertListIsEmpty();
        ps[1].Length.AssertEquals(1);
        ps[2].Length.AssertEquals(1);
        ps[3].Length.AssertEquals(1);
        ps[4].Length.AssertEquals(2);
        ps[5].Length.AssertEquals(2);

        ps[1][0].AssertListEquals(0);
        ps[2][0].AssertListEquals(0, 1);
        ps[3][0].AssertListEquals(0, 1, 2);
        ps[4][0].AssertListEquals(0, 1, 2);
        ps[4][1].AssertListEquals(3);
        ps[5][0].AssertListEquals(0, 1, 2);
        ps[5][1].AssertListEquals(3, 4);

        100.Range().Partition(1).Select(e => e.Single()).AssertListEquals(100.Range());
        100.Range().Partition(503).Single().AssertListEquals(100.Range());
    }

    [TestMethod]
    public void Stride() {
        TestUtil.AssertThrows<ArgumentNullException>(() => (null as IReadOnlyList<int>).Stride(1));
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => 1.Range().Stride(0));
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => 1.Range().Stride(-1));

        var ps = 6.Range().Select(e => e.Range().Stride(3).ToArray()).ToArray();
        ps[0].AssertListIsEmpty();
        ps[1].AssertListEquals(0);
        ps[2].AssertListEquals(0);
        ps[3].AssertListEquals(0);
        ps[4].AssertListEquals(0, 3);
        ps[5].AssertListEquals(0, 3);

        var qs = 8.Range().Select(e => 5.Range().Stride(e + 1).ToArray()).ToArray();
        qs[0].AssertListEquals(0, 1, 2, 3, 4);
        qs[1].AssertListEquals(0, 2, 4);
        qs[2].AssertListEquals(0, 3);
        qs[3].AssertListEquals(0, 4);
        qs[4].AssertListEquals(0);
        qs[5].AssertListEquals(0);
        qs[6].AssertListEquals(0);
        qs[7].AssertListEquals(0);
    }
    [TestMethod]
    public void Deinterleave() {
        TestUtil.AssertThrows<ArgumentNullException>(() => (null as IReadOnlyList<int>).Deinterleave(1));
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => 1.Range().Deinterleave(0));
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() => 1.Range().Deinterleave(-1));

        var ps = 6.Range().Select(e => e.Range().Deinterleave(3).Select(f => f.ToArray()).ToArray()).ToArray();
        Assert.IsTrue(ps.All(e => e.Length == 3));

        ps[0][0].AssertListIsEmpty();
        ps[0][1].AssertListIsEmpty();
        ps[0][2].AssertListIsEmpty();
        ps[1][0].AssertListEquals(0);
        ps[1][1].AssertListIsEmpty();
        ps[1][2].AssertListIsEmpty();
        ps[2][0].AssertListEquals(0);
        ps[2][1].AssertListEquals(1);
        ps[2][2].AssertListIsEmpty();
        ps[3][0].AssertListEquals(0);
        ps[3][1].AssertListEquals(1);
        ps[3][2].AssertListEquals(2);
        ps[4][0].AssertListEquals(0, 3);
        ps[4][1].AssertListEquals(1);
        ps[4][2].AssertListEquals(2);
        ps[5][0].AssertListEquals(0, 3);
        ps[5][1].AssertListEquals(1, 4);
        ps[5][2].AssertListEquals(2);

        Assert.IsTrue(100.Range().Deinterleave(1).Single().SequenceEqual(100.Range()));
        Assert.IsTrue(100.Range().Deinterleave(503).Select(e => e.Select(f => (int?)f).SingleOrDefault()).SequenceEqual(
            100.Range().Select(e => (int?)e).Concat(Enumerable.Repeat((int?)null, 403))));
    }
}
