using System.Collections.Generic;
using LinqToReadOnlyCollections.Experimental.Set;
using System.Diagnostics.CodeAnalysis;

namespace LinqToReadOnlyCollections.Experimental.Map {
    ///<summary>A readable map from keys to values.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="'Map' is better than 'Collection'")]
    public interface IMap<TKey, out TValue> : IEnumerable<IKeyValue<TKey, TValue>> {
        ///<summary>Returns the readable set of keys mapped to values by this map.</summary>
        IRet<TKey> Keys { get; }
        ///<summary>Returns the value mapped to by the given key.</summary>
        TValue this[TKey key] { get; }
    }
}
