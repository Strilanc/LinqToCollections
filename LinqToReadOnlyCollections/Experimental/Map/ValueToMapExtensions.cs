using System;

namespace LinqToCollections.Map {
    ///<summary>Contains extension methods to convert from any type to map types.</summary>
    public static class ExtraExtensions {
        ///<summary>Creates a key value pair.</summary>
        public static IKeyValue<TKey, TValue> KeyValue<TKey, TValue>(this TKey key, TValue value) {
            if (ReferenceEquals(key, null)) throw new ArgumentNullException("key");
            return new KeyValue<TKey, TValue>(key, value);
        }
    }
}
