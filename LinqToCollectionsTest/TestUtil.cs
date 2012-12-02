using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Strilanc.LinqToCollections;

internal static class TestUtil {
    public static void AssertThrows<TException>(Func<object> func) where TException : Exception {
        AssertThrows<TException>(new Action(() => func()));
    }
    public static IReadOnlyCollection<int> CRange(this int count) {
        return count.Range();
    } 
    public static void AssertThrows<TException>(Action action) where TException : Exception {
        try {
            action();
            Assert.Fail("Expected an exception of type {0}, but no exception was thrown.",
                        typeof(TException).FullName);
        } catch (TException) {
            // pass!
        } catch (Exception ex) {
            Assert.Fail("Expected an exception of type {0}, but received one of type {1}: {2}.",
                        typeof(TException).FullName,
                        ex.GetType().FullName,
                        ex);
        }
    }
    public static void AssertSequenceEquals<T>(this IEnumerable<T> actual, IEnumerable<T> expected) {
        actual.AssertSequenceEquals(expected.ToArray());
    }
    public static void AssertSequenceEquals<T>(this IEnumerable<T> actual, params T[] expected) {
        if (actual.SequenceEqual(expected)) return;

        Assert.Fail(
            "Sequences not equal.{0}Expected: [{1}]{0}Actual: [{2}]", 
            Environment.NewLine, 
            String.Join(",", expected), 
            String.Join(",", actual));
    }
    public static void AssertListEquals<T>(this IReadOnlyList<T> actual, params T[] expected) {
        actual.AssertSequenceEquals(expected);
        actual.Count.AssertEquals(expected.Length);
        for (var i = 0; i < expected.Length; i++)
            actual[i].AssertEquals(expected[i]);
    }
    public static void AssertCollectionEquals<T>(this IReadOnlyCollection<T> actual, params T[] expected) {
        actual.AssertSequenceEquals(expected);
        actual.Count.AssertEquals(expected.Length);
    }
    public static void AssertCollectionEquals<T>(this IReadOnlyCollection<T> actual, IEnumerable<T> expected) {
        actual.AssertCollectionEquals(expected.ToArray());
    }
    public static void AssertDictionaryEquals<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> actual, IEnumerable<KeyValuePair<TKey, TValue>> keyVals) {
        var d = keyVals.ToDictionary(e => e.Key, e => e.Value);
        foreach (var e in actual)
            d.Contains(e).AssertIsTrue();
        foreach (var e in d)
            actual.Contains(e).AssertIsTrue();
        actual.Count.AssertEquals(d.Count);
    }
    public static void AssertListEquals<T>(this IReadOnlyList<T> actual, IEnumerable<T> expected) {
        actual.AssertListEquals(expected.ToArray());
    }
    public static void AssertListBroken<T>(this IReadOnlyList<T> list) {
        AssertThrows<InvalidOperationException>(() => list.Count);
        AssertThrows<InvalidOperationException>(() => list[0]);
        AssertThrows<InvalidOperationException>(() => list.GetEnumerator());
    }
    public static void AssertCollectionBroken<T>(this IReadOnlyCollection<T> list) {
        AssertThrows<InvalidOperationException>(() => list.Count);
        AssertThrows<InvalidOperationException>(() => list.GetEnumerator());
    }
    [DebuggerStepThrough]
    public static T AssertNotCollected<T>(this WeakReference<T> weak) where T : class {
        T val;
        weak.TryGetTarget(out val).AssertIsTrue();
        return val;
    }
    [DebuggerStepThrough]
    public static void AssertCollected<T>(this WeakReference<T> weak) where T : class {
        T val;
        weak.TryGetTarget(out val).AssertIsFalse();
    }
    [DebuggerStepThrough]
    public static void AssertListIsEmpty<T>(this IReadOnlyList<T> actual) {
        actual.Count.AssertEquals(0);
        Assert.IsTrue(actual.SequenceEqual(new T[0]));
    }
    [DebuggerStepThrough]
    public static void AssertCollectionIsEmpty<T>(this IReadOnlyCollection<T> actual) {
        actual.Count.AssertEquals(0);
        Assert.IsTrue(actual.SequenceEqual(new T[0]));
    }
    [DebuggerStepThrough]
    public static void AssertSequenceIsEmpty<T>(this IEnumerable<T> actual) {
        Assert.IsTrue(actual.SequenceEqual(new T[0]));
    }
    [DebuggerStepThrough]
    public static void AssertEquals<T1, T2>(this T1 actual, T2 expected) {
        Assert.AreEqual(actual: actual, expected: expected);
    }
    [DebuggerStepThrough]
    public static void AssertReferenceEquals<T1, T2>(this T1 actual, T2 expected) {
        Assert.AreSame(actual: actual, expected: expected);
    }
    [DebuggerStepThrough]
    public static void AssertReferenceDoesNotEqual<T1, T2>(this T1 actual, T2 notExpected) {
        Assert.AreNotSame(actual: actual, notExpected: notExpected);
    }
    [DebuggerStepThrough]
    public static void AssertIsTrue(this bool value) {
        Assert.IsTrue(value);
    }
    [DebuggerStepThrough]
    public static void AssertIsFalse(this bool value) {
        Assert.IsFalse(value);
    }
    public static WeakReference<T> WeakRef<T>(this T value) where T : class {
        return new WeakReference<T>(value);
    }
}
