using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace LinqToCollections.Set {
    public static class RetExtensions {
        ///<summary>Creates a new readable set composed of items from a shallow copy of a sequence.</summary>
        public static IRet<T> ToRet<T>(this IEnumerable<T> sequence) {
            Contract.Requires<ArgumentException>(sequence != null);
            Contract.Ensures(Contract.Result<IRet<T>>() != null);
            return new HashSet<T>(sequence).AsRet();
        }
        ///<summary>Exposes a standard set as a readable set.</summary>
        public static IRet<T> AsRet<T>(this ISet<T> set) {
            Contract.Requires<ArgumentException>(set != null);
            Contract.Ensures(Contract.Result<IRet<T>>() != null);
            return new Ret<T>(e => set.Contains(e), set);
        }
        ///<summary>Filters the items in a readable set.</summary>
        public static IRet<T> Where<T>(this IRet<T> set, Func<T, bool> filter) {
            Contract.Requires<ArgumentException>(set != null);
            Contract.Requires<ArgumentException>(filter != null);
            Contract.Ensures(Contract.Result<IRet<T>>() != null);
            return new Ret<T>(e => set.Contains(e) && filter(e), set.AsEnumerable().Where(filter));
        }
        ///<summary>Returns a readable set with items from either or both of two readable sets.</summary>
        public static IRet<T> Union<T>(this IRet<T> set1, IRet<T> set2) {
            Contract.Requires<ArgumentException>(set1 != null);
            Contract.Requires<ArgumentException>(set2 != null);
            Contract.Ensures(Contract.Result<IRet<T>>() != null);
            return new Ret<T>(e => set1.Contains(e) || set2.Contains(e), set1.Concat(set2).Distinct());
        }
        ///<summary>Returns a readable set with items from both of two readable sets.</summary>
        public static IRet<T> Intersect<T>(this IRet<T> set1, IRet<T> set2) {
            Contract.Requires<ArgumentException>(set1 != null);
            Contract.Requires<ArgumentException>(set2 != null);
            Contract.Ensures(Contract.Result<IRet<T>>() != null);
            return set1.Where(e => set2.Contains(e));
        }
        ///<summary>Returns a readable set with items in one readable set but not another.</summary>
        public static IRet<T> Except<T>(this IRet<T> set1, IRet<T> set2) {
            Contract.Requires<ArgumentException>(set1 != null);
            Contract.Requires<ArgumentException>(set2 != null);
            Contract.Ensures(Contract.Result<IRet<T>>() != null);
            return set1.Where(e => !set2.Contains(e));
        }
        ///<summary>Determines if two readable sets have any items in common.</summary>
        public static bool Intersects<T>(this IRet<T> set1, IRet<T> set2) {
            Contract.Requires<ArgumentException>(set1 != null);
            Contract.Requires<ArgumentException>(set2 != null);
            return set1.Intersect(set2).Any();
        }
        ///<summary>Determines if a readable set only has items from another readable set.</summary>
        public static bool IsSubsetOf<T>(this IRet<T> set1, IRet<T> set2) {
            Contract.Requires<ArgumentException>(set1 != null);
            Contract.Requires<ArgumentException>(set2 != null);
            return set1.All(e => set2.Contains(e));
        }
        ///<summary>Determines if a readable set only has items from another readable set, but is not equivalent to the other set.</summary>
        public static bool IsStrictSubsetOf<T>(this IRet<T> set1, IRet<T> set2) {
            Contract.Requires<ArgumentException>(set1 != null);
            Contract.Requires<ArgumentException>(set2 != null);
            return set1.IsSubsetOf(set2) && !set2.IsSubsetOf(set1);
        }
        ///<summary>Determines if a readable set has the same items as another readable set.</summary>
        public static bool SetEquals<T>(this IRet<T> set1, IRet<T> set2) {
            Contract.Requires<ArgumentException>(set1 != null);
            Contract.Requires<ArgumentException>(set2 != null);
            return set1.IsSubsetOf(set2) && set2.IsSubsetOf(set1);
        }
        ///<summary>Determines if two readable sets have no items in common.</summary>
        public static bool IsDisjointFrom<T>(this IRet<T> set1, IRet<T> set2) {
            Contract.Requires<ArgumentException>(set1 != null);
            Contract.Requires<ArgumentException>(set2 != null);
            return set1.All(e => !set2.Contains(e));
        }
    }
}
