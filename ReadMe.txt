Building:
- Targets .Net 4.5
- Disable signing or choose a different strong name file
- Install Code Contracts ( http://research.microsoft.com/en-us/projects/contracts/ )
 ~ Activate only runtime precondition checking (some postconditions, invariants and assumptions are quite expensive to check).

--- Notes ---
How is this library useful?
- Prevents lists from 'degrading' into enumerables due to projections, reversing, etc.
- Several linq operations are much more efficient on lists (Last, Count, Revers, etc).

Why is the ReadOnlyList<T> class used for 'custom' readable lists instead of individual custom classes?
- Line savings. A custom class takes a dozen lines instead of one.
- (This does come at the cost of some performance, due to the added indirection of the lambda.)
Followup: why is SubList implemented with a custom class?
- Prevents quadratic behavior from repeated application (due to increasing indirection).
- Iteratively applying SubList/Take/Skip is an expected use case (eg. a parser keeps removing the parsed prefix of data).
 ~ There is a similar optimization in AsIList to keep bouncing between IReadOnlyList<T> and IList<T> from adding indirection.
