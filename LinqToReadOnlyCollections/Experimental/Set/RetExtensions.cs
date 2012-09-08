using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToReadOnlyCollections.Experimental.Set {
    public static class RetExtensions {
        ///<summary>Creates a new readable set composed of items from a shallow copy of a sequence.</summary>
        public static IRet<T> ToRet<T>(this IEnumerable<T> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            return new HashSet<T>(sequence).AsRet();
        }
        ///<summary>Exposes a standard set as a readable set.</summary>
        public static IRet<T> AsRet<T>(this ISet<T> set) {
            if (set == null) throw new ArgumentNullException("set");
            return new Ret<T>(set.Contains, set);
        }
        ///<summary>Filters the items in a readable set.</summary>
        public static IRet<T> Where<T>(this IRet<T> set, Func<T, bool> filter) {
            if (set == null) throw new ArgumentNullException("set");
            if (filter == null) throw new ArgumentNullException("filter");
            return new Ret<T>(e => set.Contains(e) && filter(e), set.AsEnumerable().Where(filter));
        }
        ///<summary>Returns a readable set with items from either or both of two readable sets.</summary>
        public static IRet<T> Union<T>(this IRet<T> set1, IRet<T> set2) {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            return new Ret<T>(e => set1.Contains(e) || set2.Contains(e), set1.Concat(set2).Distinct());
        }
        ///<summary>Returns a readable set with items from both of two readable sets.</summary>
        public static IRet<T> Intersect<T>(this IRet<T> set1, IRet<T> set2) {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            return set1.Where(set2.Contains);
        }
        ///<summary>Returns a readable set with items in one readable set but not another.</summary>
        public static IRet<T> Except<T>(this IRet<T> set1, IRet<T> set2) {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            return set1.Where(e => !set2.Contains(e));
        }
        ///<summary>Determines if two readable sets have any items in common.</summary>
        public static bool Intersects<T>(this IRet<T> set1, IRet<T> set2) {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            return set1.Intersect(set2).Any();
        }
        ///<summary>Determines if a readable set only has items from another readable set.</summary>
        public static bool IsSubsetOf<T>(this IRet<T> set1, IRet<T> set2) {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            return set1.All(set2.Contains);
        }
        ///<summary>Determines if a readable set only has items from another readable set, but is not equivalent to the other set.</summary>
        public static bool IsStrictSubsetOf<T>(this IRet<T> set1, IRet<T> set2) {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            return set1.IsSubsetOf(set2) && !set2.IsSubsetOf(set1);
        }
        ///<summary>Determines if a readable set has the same items as another readable set.</summary>
        public static bool SetEquals<T>(this IRet<T> set1, IRet<T> set2) {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            return set1.IsSubsetOf(set2) && set2.IsSubsetOf(set1);
        }
        ///<summary>Determines if two readable sets have no items in common.</summary>
        public static bool IsDisjointFrom<T>(this IRet<T> set1, IRet<T> set2) {
            if (set1 == null) throw new ArgumentNullException("set1");
            if (set2 == null) throw new ArgumentNullException("set2");
            return set1.All(e => !set2.Contains(e));
        }
    }
}
