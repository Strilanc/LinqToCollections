using System;
using System.Diagnostics.Contracts;

namespace LinqToCollections.Map {
    ///<summary>A key value pair.</summary>
    [ContractClass(typeof(ContractClassForIKeyValue<,>))]
    public interface IKeyValue<out TKey, out TValue> {
        TKey Key { get; }
        TValue Value { get; }
    }
    [ContractClassFor(typeof(IKeyValue<,>))]
    public abstract class ContractClassForIKeyValue<TKey, TValue> : IKeyValue<TKey, TValue> {
        public TKey Key {
            get {
                Contract.Ensures(Contract.Result<TKey>() != null);
                throw new NotSupportedException();
            }
        }
        public TValue Value {
            get {
                throw new NotSupportedException();
            }
        }
    }
}
