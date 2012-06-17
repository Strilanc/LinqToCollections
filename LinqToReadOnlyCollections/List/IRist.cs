using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.List {
    ///<summary>A readable list of items with known length.</summary>
    ///<remarks>Shorthand for 'IReadableList'.</remarks>
    ///<typeparam name="T">The type of items in the list.</typeparam>
    [ContractClass(typeof(IRistContractClass<>))]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IRist<out T> : IEnumerable<T> {
        ///<summary>Gets the number of items in the list.</summary>
        int Count { get; }
        ///<summary>Gets the list item at the given index.</summary>
        T this[int index] { get; }
    }

    [ContractClassFor(typeof(IRist<>))]
    public abstract class IRistContractClass<T> : IRist<T> {
        public int Count {
            get {
                Contract.Ensures(Contract.Result<int>() >= 0);
                throw new NotImplementedException();
            }
        }
        public T this[int index] {
            get {
                Contract.Requires<ArgumentException>(index >= 0);
                Contract.Requires<ArgumentException>(index < this.Count);
                throw new NotImplementedException();
            }
        }

        public IEnumerator<T> GetEnumerator() {
            throw new NotImplementedException();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
