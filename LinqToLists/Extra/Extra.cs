using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToLists.Extra {
    ///<summary>Contains extension methods which affect non-list types.</summary>
    ///<remarks>Not included in the main namespace to avoid unwanted pollution.</remarks>
    public static class ExtraExtensions {
        ///<summary>Exposes the non-negative integers below the count as a readable list.</summary>
        [Pure()]
        public static IRist<int> Range(this int count) {
            Contract.Requires<ArgumentException>(count >= 0);
            Contract.Ensures(Contract.Result<IRist<int>>() != null);
            Contract.Ensures(Contract.Result<IRist<int>>().Count == count);
            var r = new Rist<int>(counter: () => count, getter: i => i);
            Contract.Assume(r.Count == count);
            return r;
        }
    }
}
