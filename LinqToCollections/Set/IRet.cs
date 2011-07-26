using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.Set {
    ///<summary>A readable set.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "'Ret' is better than 'Collection'")]
    public interface IRet<T> : IEnumerable<T> {
        [Pure]
        bool Contains(T item);
    }
}
