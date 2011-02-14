Building:
- Targets .Net 4.0
- Disable signing or choose a different strong name file
- Install Code Contracts ( http://research.microsoft.com/en-us/projects/contracts/ )
 ~ Not strictly necessary, but argument validation may be omitted without it.
 ~ Do not turn on the inclusion of runtime postcondition and invariant checks, except for testing (because some expensive-at-runtime ones are used).

--- Notes ---
How is this library useful?
- Ask only for what you need. A method taking IList<T> may modify the list, but you trivially know it won't if it takes IRist<T>.
- IEnumerable<T> is costly in time or memory when you need random access to the sequence elements.
- Several operations become much more efficient (Last and Reverse in particular)

Why doesn't IRist<T> inherit IList<T> or ICollection<T>?
- It would prevent covariance (eg. Contains is contravariant)
- It would pollute the intended-to-be-pure interface with inappropriate mutating methods.
- You would still need to do conversions via AsRist/AsIList, because the BCL collections don't implement IRist<T>.
- IRist<T> is a strict subset of IList<T>, not vice versa.

Why is the Rist<T> class used for 'custom' readable lists instead of individual custom classes?
- Line savings. A custom class takes a dozen lines instead of one.
- (This does come at the cost of some performance, due to the added indirection of the lambda.)
Followup: why is SubList implemented with a custom class?
- Prevents quadratic behavior from repeated application.
- Iteratively applying SubList/Take/Skip is an expected use case (eg. a parser keeps removing the parsed prefix of data).
