using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToCollections.Set;
using LinqToCollections;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace LinqToCollections.Map {
    ///<summary>A readable map from keys to values.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="'Map' is better than 'Collection'")]
    [ContractClass(typeof(ContractClassIMap<,>))]
    public interface IMap<TKey, out TValue> : IEnumerable<IKeyValue<TKey, TValue>> {
        ///<summary>Returns the readable set of keys mapped to values by this map.</summary>
        [Pure]
        IRet<TKey> Keys { get; }
        ///<summary>Returns the value mapped to by the given key.</summary>
        [Pure]
        TValue this[TKey key] { get; }
    }

    [ContractClassFor(typeof(IMap<,>))]
    public abstract class ContractClassIMap<TKey, TValue> : IMap<TKey, TValue> {
        [Pure]
        public IRet<TKey> Keys {
            get {
                Contract.Ensures(Contract.Result<IRet<TKey>>() != null);
                throw new NotImplementedException();
            }
        }

        [Pure]
        public TValue this[TKey key] {
            get {
                Contract.Requires<ArgumentException>(key != null);
                Contract.Requires<ArgumentException>(this.Keys.Contains(key));
                throw new NotImplementedException(); 
            }
        }

        public IEnumerator<IKeyValue<TKey, TValue>> GetEnumerator() {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
