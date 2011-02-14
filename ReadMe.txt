Building:
- Targets .Net 4.0
- Disable signing or choose a different strong name file
- Install Code Contracts ( http://research.microsoft.com/en-us/projects/contracts/ )
 ~ Not strictly necessary, but argument validation may be omitted without it.
 ~ Activate only runtime precondition checking (some postconditions, invariants and assumptions are quite expensive to check).

--- Notes ---
How is this library useful?
- Ask only for what you need. A method taking IList<T> may modify the list, but you trivially know it won't if it takes IRist<T>.
- IEnumerable<T> is costly in time or memory when you need random access to the sequence elements.
- Several operations become much more efficient (Last and Reverse in particular).
- Some operations stay efficient (Count in particular) when operations like projection don't degrade a list into an enumerable.

Why use a new type instead of IList<T>?
- Mainly the fact that IList<T> has mutating methods. Purity being obvious is important.
- IList<T> also has several unnecessary methods, increasing the size and test surface of implementations.
- The vain hope that Microsoft will take a hint and split their collection interfaces into smaller pieces (particularly along purity and variance boundaries).

Why doesn't IRist<T> inherit IList<T> or ICollection<T>?
- It would prevent covariance (eg. Contains is contravariant)
- It would pollute the intended-to-be-pure interface with inappropriate mutating methods.
- You would still need to do conversions via AsRist/AsIList, because the BCL collections don't implement IRist<T>.
- IRist<T> is a strict subset of IList<T>, not vice versa.

Why is the Rist<T> class used for 'custom' readable lists instead of individual custom classes?
- Line savings. A custom class takes a dozen lines instead of one.
- (This does come at the cost of some performance, due to the added indirection of the lambda.)
Followup: why is SubList implemented with a custom class?
- Prevents quadratic behavior from repeated application (due to increasing indirection).
- Iteratively applying SubList/Take/Skip is an expected use case (eg. a parser keeps removing the parsed prefix of data).
 ~ There is a similar optimization in AsIList to keep bouncing between IRist<T> and IList<T> from adding indirection.
