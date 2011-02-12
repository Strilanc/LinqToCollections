using System;

internal static class Util {
    public static void ExpectException<E>(Action action) where E : Exception {
        try {
            action();
        } catch (E) {
            return;
        }
        throw new InvalidOperationException("Expected an exception.");
    }
}
