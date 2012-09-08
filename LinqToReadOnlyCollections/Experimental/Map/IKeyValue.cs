using System;

namespace LinqToCollections.Map {
    ///<summary>A key value pair.</summary>
    public interface IKeyValue<out TKey, out TValue> {
        TKey Key { get; }
        TValue Value { get; }
    }
}
