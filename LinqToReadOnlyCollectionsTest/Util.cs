using System;

internal static class Util {
    public static void ExpectException<TExpectedException>(Action action) where TExpectedException : Exception {
        try {
            action();
        } catch (TExpectedException) {
            return;
        }
        throw new InvalidOperationException("Expected an exception.");
    }
    public static Action Ack<T>(Func<T> func) {
        return () => func();
    }
}
