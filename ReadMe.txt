How to Build:
- Open solution file in Visual Studio 2012
- Disable signing or choose a different strong name file

--- Notes ---
How is this library useful?
- Prevents lists from 'degrading' into enumerables due to projections, reversing, etc.
- Several linq operations are much more efficient on lists (Last, Count, Reverse, ...).

What's the purposes of classes like ReadOnlyList<T> that implement an interface with delegates?
- Line savings. A custom class takes a dozen lines instead of one.
- (This does come at the cost of some performance, due to the added indirection of the lambda.)

Why don't Skip/Take methods use it, then?
- Iteratively applying Take/Skip is an expected use case (eg. a parser keeps removing the parsed prefix of data).
- Need to prevents quadratic performance loss from repeated application (due to increasing indirection).
