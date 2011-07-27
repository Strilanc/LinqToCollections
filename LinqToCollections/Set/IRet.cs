using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.Set {
    ///<summary>A readable set.</summary>
    [ContractClass(typeof(ContractClassForIRet<>))]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "'Ret' is better than 'Collection'")]
    public interface IRet<T> : IEnumerable<T> {
        [Pure]
        bool Contains(T item);
    }
    [ContractClassFor(typeof(IRet<>))]
    public abstract class ContractClassForIRet<T> : IRet<T> {
        public bool Contains(T item) {
            Contract.Requires(item != null);
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
