# Aelena.CommonExtensions

Published on NuGet as [**Common-Extensions**](https://www.nuget.org/packages/Common-Extensions).

[![CI](https://github.com/aelena/useful-extensions/actions/workflows/ci.yml/badge.svg)](https://github.com/aelena/useful-extensions/actions/workflows/ci.yml)
![.NET Standard 2.0 | .NET 8 | 10 | 11](https://img.shields.io/badge/.NET-Standard%202.0%20%7C%208.0%20%7C%2010.0%20%7C%2011.0-512BD4)
![C# 14](https://img.shields.io/badge/C%23-14-239120)
![Coverage 100%](https://img.shields.io/badge/coverage-100%25%20line%20%7C%20branch%20%7C%20method-brightgreen)

A small, opinionated set of extension members for `string`, `IEnumerable<T>` and comparable values.
Every member fills a gap that the .NET Base Class Library and C# still leave open in 2026. Nothing here
duplicates something you can already write as a one-liner with the framework.

```csharp
using Aelena.CommonExtensions;

"key=value".After("=");                              // "value"
"/usr/local/bin".AfterLast('/');                     // "bin"
"<b>bold</b>".Between("<b>", "</b>");                // "bold"
"a-b-c".ReplaceLast("-", "+");                       // "a-b+c"
"crème brûlée".RemoveDiacritics();                   // "creme brulee"
"a very long headline".Truncate(10, "…");            // "a very lo…"
"a,\"b,c\",d".SplitOutside(',', '"');                // ["a", "\"b,c\"", "d"]

new[] { 3, 8, 12 }.FindIndex(x => x > 5);            // 1
naturals.TakeUntil(x => x == 3);                     // 0, 1, 2, 3
prices.Window(3).Select(w => w.Average());           // moving average
movements.Scan(0m, (balance, m) => balance + m);     // running balance
players.TopBy(3, p => p.Score);                      // best three, no full sort
status.In(Status.Draft, Status.Pending);             // true / false
DateTime.Today.IsBetween(start, end);
```

## Contents

- [Why this library exists](#why-this-library-exists)
- [Installing](#installing)
- [Design rules](#design-rules)
- [Strings](#strings)
- [Fuzzy matching](#fuzzy-matching)
- [Sequences](#sequences)
- [Values](#values)
- [Migrating from the 1.x library](#migrating-from-the-1x-library)
- [Building and testing](#building-and-testing)

## Why this library exists

The first version of this library dates from the .NET Framework 4.5 era. It had about sixty methods,
and by now two thirds of them are one-liners in the framework: `string.Join`, ranges and indices,
`Enumerable.Chunk`, `Take(Range)`, null-conditional operators, pattern matching and so on.
Version 2 is a rewrite from scratch that keeps only the members that still earn their place, fixes their
semantics, and gives them modern signatures:

- **C# 14 extension members**, compiled to ordinary static extension methods so any C# compiler can call them.
- **Nullable annotations** everywhere, including `[NotNullWhen]` flow on the emptiness checks.
- **Ordinal comparison by default**, with an explicit `StringComparison` parameter where it matters.
- **Lazy iterators that validate eagerly**: a bad argument throws at the call site, not on first enumeration.
- **Zero allocation where possible**: `params ReadOnlySpan<T>`, `stackalloc`, `string.Concat` over spans.
- **.NET Standard 2.0 as well as .NET 8, 10 and 11**, so it works on .NET Framework 4.6.2+, Mono and Unity too, with identical behaviour everywhere.
- **100% line, branch and method coverage**, enforced in CI. See [Building and testing](#building-and-testing). The README examples are themselves part of the test suite.

## Installing

The package targets `netstandard2.0`, `net8.0`, `net10.0` and `net11.0`. The .NET Standard build is what
.NET Framework 4.6.2+, .NET Core 2.x through 7, Mono and Unity pick up; it depends on `System.Memory` and
nothing else. The modern builds have no dependencies at all.

```shell
dotnet add package Common-Extensions
```

The package id is `Common-Extensions` (kept from version 0.8); the assembly and namespace are `Aelena.CommonExtensions`.

Or reference the project directly:

```xml
<ProjectReference Include="path/to/src/Aelena.CommonExtensions/Aelena.CommonExtensions.csproj" />
```

Then bring the namespace into scope, ideally as a global using:

```csharp
global using Aelena.CommonExtensions;
```

## Design rules

These hold for every member, so they are stated once here rather than repeated below.

| Rule | Detail |
|---|---|
| A `null` receiver throws | `ArgumentNullException`, always. The only exceptions are `IsNullOrEmpty` and `HasItems`, whose entire point is to accept `null`. |
| Empty markers throw | Searching for `""` is never meaningful, so `After("")`, `Between("", …)`, `AllIndicesOf("")` and friends throw `ArgumentException`. |
| Ordinal by default | Every `StringComparison` parameter defaults to `Ordinal`. Pass `OrdinalIgnoreCase` or a culture-aware value when you need it. |
| Not found means unchanged | `After`, `Before`, `ReplaceFirst`, `Truncate` and others return the **same instance** when there is nothing to do, so you can rely on reference equality and avoid needless allocation. |
| Iterators validate eagerly | Methods returning `IEnumerable<T>` check their arguments immediately and only defer the work. |

## Strings

### Slicing by marker

Substring operations expressed in terms of the text you can see rather than indices you must compute.
When the marker is not found the whole string comes back, which makes chained calls on optional
segments safe (`path.AfterLast('/')` on a bare file name returns the file name).

```csharp
"key=value".After("=");                    // "value"
"key=value".Before("=");                   // "key"
"a=b=c".After("=");                        // "b=c"
"a=b=c".AfterLast("=");                    // "c"
"a=b=c".BeforeLast("=");                   // "a=b"
"file.tar.gz".AfterLast('.');              // "gz"
"file.tar.gz".BeforeLast('.');             // "file.tar"
"no marker here".After("=");               // "no marker here"  (unchanged, same instance)

"Hello WORLD".After("world", StringComparison.OrdinalIgnoreCase);  // ""
```

All four have `string` and `char` overloads.

### Text between two markers

```csharp
"<b>bold</b>".Between("<b>", "</b>");          // "bold"
"say [hello] and [bye]".Between("[", "]");     // "hello"
"unterminated [".Between("[", "]");            // ""
"a=[1] b=[2]".After("b=").Between("[", "]");   // "2"   (compose to search past a point)

"[1] x [2] y [3]".AllBetween("[", "]");        // "1", "2", "3"   (lazy, non-overlapping)
"'a' 'b' '".AllBetween("'", "'");              // "a", "b"        (dangling quote yields nothing)
```

`Between` returns `""` when either marker is missing, unlike the slicing members above, because "the
text between two markers that are not there" has no sensible fallback.

### Replacing the first or last occurrence

`string.Replace` replaces everything. These replace exactly one occurrence.

```csharp
"a-b-c".ReplaceFirst("-", "+");                    // "a+b-c"
"a-b-c".ReplaceLast("-", "+");                     // "a-b+c"
"one, two, three".ReplaceLast(", ", " and ");      // "one, two and three"
"b-B-b".ReplaceFirst("B", "x", StringComparison.OrdinalIgnoreCase);  // "x-B-b"
```

### Removing several values at once

```csharp
"(1) [2] {3}".RemoveAll("(", ")", "[", "]", "{", "}", " ");   // "123"
"Ab aB".RemoveAll(StringComparison.OrdinalIgnoreCase, "ab");   // " "
html.RemoveAll(tagsToStrip);                                    // any IEnumerable<string>
```

Values are removed in the order given, so a later value can match text exposed by an earlier removal.

### Truncating

The suffix counts toward the limit, so the result never exceeds `maxLength`.

```csharp
"hello world".Truncate(5);            // "hello"
"hello world".Truncate(8, "...");     // "hello..."
"hello world".Truncate(8, "…");       // "hello w…"
"hi".Truncate(8, "…");                // "hi"   (fits, same instance)
"hello world".Truncate(2, "...");     // ".."   (suffix itself is clipped)
```

### Substrings that never throw

`string.Substring` and range indexing throw on out-of-range arguments. `SafeSubstring` clips the requested
range to the string instead, which is what you usually want when slicing user input or log lines.

```csharp
"hello world".SafeSubstring(6, 50);   // "world"
"hello world".SafeSubstring(50, 5);   // ""
"hello world".SafeSubstring(-3, 5);   // "he"    (the range [-3, 2) clipped to [0, 2))
"hello world".SafeSubstring(6);       // "world"
"hello world".SafeSubstring(50);      // ""
```

### Removing diacritics

Decomposes to Unicode normalization form D, drops the combining marks, and recomposes. Letters without a
canonical decomposition, such as `ł` or `ø`, are left alone on purpose. When there is nothing to strip the
original instance is returned.

```csharp
"crème brûlée".RemoveDiacritics();   // "creme brulee"
"Ångström".RemoveDiacritics();       // "Angstrom"
"São Paulo".RemoveDiacritics();      // "Sao Paulo"
"łódź".RemoveDiacritics();           // "łodz"
```

### Finding every index

```csharp
"a-b-c-d".AllIndicesOf('-');                     // 1, 3, 5
"abcabcabc".AllIndicesOf("abc");                 // 0, 3, 6
"aaaa".AllIndicesOf("aa");                       // 0, 2         (non-overlapping by default)
"aaaa".AllIndicesOf("aa", overlapping: true);    // 0, 1, 2
"The the".AllIndicesOf("the", StringComparison.OrdinalIgnoreCase);  // 0, 4
```

### Searching for any of several values

```csharp
text.ContainsAny("error", "fatal", "panic");
text.ContainsAny(StringComparison.OrdinalIgnoreCase, "ERROR", "FATAL");
text.ContainsAny(bannedWords);                                   // any IEnumerable<string>

text.FirstContained("fatal", "error", "warning");                // "error", or null when none match
text.FirstContained(severities, StringComparison.OrdinalIgnoreCase);
```

`FirstContained` returns the first **candidate** that matches, in the order you listed them, which makes it a
natural priority lookup.

### Splitting while respecting quotes or brackets

`string.Split` cannot tell that a comma inside quotes is not a separator. `SplitOutside` can. Enclosure
markers are kept in the output so no information is lost, and `StringSplitOptions` work exactly as they do on
`string.Split`.

```csharp
"a,\"b,c\",d".SplitOutside(',', '"');
// "a", "\"b,c\"", "d"

"f(a, b), c, g(d)".SplitOutside(", ", ("(", ")"));
// "f(a, b)", "c", "g(d)"

"\"a,b\";[c;d],e".SplitOutside([",", ";"], [("\"", "\""), ("[", "]")]);
// "\"a,b\"", "[c;d]", "e"

" a , , b ".SplitOutside(',', '"', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
// "a", "b"

"a||b|c".SplitOutside(["|", "||"], []);
// "a", "b", "c"      (the longest separator wins at any position)
```

An enclosure that is never closed runs to the end of the string. Enclosures do not nest and there is no
escape syntax: the first closing marker after an opening one ends the enclosure. For a full CSV dialect,
use a CSV library.

## Fuzzy matching

Two families of metrics, both case-sensitive unless you pass `ignoreCase: true`:

| Kind | Members | Range | Read as |
|---|---|---|---|
| Distance | `LevenshteinDistance`, `DamerauLevenshteinDistance`, `HammingDistance` | integer edits | lower is closer |
| Similarity | `JaroWinklerSimilarity`, `JaroSimilarity`, `LevenshteinSimilarity`, `LongestCommonSubsequenceSimilarity`, `DiceSimilarity`, `JaccardSimilarity` | 0 to 1 | higher is closer |

Plus the building blocks `LongestCommonSubsequenceLength`, `LongestCommonSubstring` and `NGrams`, and the
collection rankings `ClosestTo`, `MostSimilarTo`, `ClosestPairs` and `ClusterBy`, which take any metric.

### Which metric?

- **Typos in words**: `DamerauLevenshtein`. A swapped pair of letters is the most common typing error and it
  costs 1 instead of 2.
- **Names, logins, short identifiers**: `JaroWinkler`. Rewards a shared start and tolerates length differences.
- **Titles, addresses, anything with reordered words**: `Dice` or `Jaccard` over bigrams. Order-insensitive.
- **Fixed-width codes** (hashes, ISBNs, DNA): `Hamming`. Only defined for equal lengths.
- **Diff-like "how much is shared"**: `LongestCommonSubsequence`.

### Edit distances

```csharp
"kitten".LevenshteinDistance("sitting");             // 3
"recieve".LevenshteinDistance("receive");            // 2   (two substitutions)
"recieve".DamerauLevenshteinDistance("receive");     // 1   (one transposition)
"karolin".HammingDistance("kathrin");                // 3
"Kitten".LevenshteinDistance("kitten", ignoreCase: true);  // 0
```

`DamerauLevenshteinDistance` is the optimal string alignment variant: a transposed pair is not edited again,
so `"ca"` to `"abc"` is 3, not 2. `HammingDistance` throws `ArgumentException` for strings of different length.

### Similarity scores

```csharp
"MARTHA".JaroSimilarity("MARHTA");                   // 0.9444
"MARTHA".JaroWinklerSimilarity("MARHTA");            // 0.9611  (bonus for the shared "MAR")
"DWAYNE".JaroWinklerSimilarity("DUANE");             // 0.84
"kitten".LevenshteinSimilarity("sitting");           // 0.5714  (1 - 3/7)
"night".DiceSimilarity("nacht");                     // 0.25    (bigrams ni ig gh ht vs na ac ch ht)
"night".JaccardSimilarity("nacht");                  // 0.1429
"ABCBDAB".LongestCommonSubsequenceSimilarity("BDCABA");  // 0.6154
```

Two empty strings score 1 everywhere. `JaroWinklerSimilarity` takes a `prefixScale` (default 0.1, maximum
0.25). `DiceSimilarity` and `JaccardSimilarity` take the n-gram `size` (default 2).

### Shared text

```csharp
"ABCBDAB".LongestCommonSubsequenceLength("BDCABA");  // 4   ("BCBA", not necessarily adjacent)
"the quick brown fox".LongestCommonSubstring("a quick brown dog");  // " quick brown "
"abcd".NGrams(2);                                    // "ab", "bc", "cd"   (lazy)
```

### Closest candidates to a target

`ClosestTo` ranks by a distance, best first; `MostSimilarTo` ranks by a similarity, best first. Both are
stable, so ties keep their source order, and both return the score next to each value so callers can apply a
cutoff.

```csharp
var words = new[] { "cooler", "dollar", "collar", "colour" };

words.ClosestTo("color");       // [("colour", 1)]
words.ClosestTo("color", 3);    // [("colour", 1), ("cooler", 2), ("collar", 2)]

new[] { "receive", "deceive" }.ClosestTo("recieve");                                          // [("receive", 2)]
new[] { "receive", "deceive" }.ClosestTo("recieve", metric: StringDistance.DamerauLevenshtein); // [("receive", 1)]

var names = new[] { "DWAYNE", "MARHTA", "MARTHA", "DUANE" };
names.MostSimilarTo("MARTHA", 2);   // [("MARTHA", 1.0), ("MARHTA", 0.9611)]
```

The "did you mean" idiom, with a list pattern as the cutoff:

```csharp
var suggestion = commands.ClosestTo(input) is [{ Distance: <= 2 } best]
    ? $"Did you mean '{best.Value}'?"
    : null;
```

### Closest pairs within a collection

Measures every unordered pair and returns the nearest ones, best pair first. This is O(n²) in the number
of strings, which is fine for hundreds of items and worth thinking about for hundreds of thousands.

```csharp
var names = new[] { "color", "colour", "dollar", "collar" };

names.ClosestPairs();      // [("color", "colour", 1)]
names.ClosestPairs(2);     // [("color", "colour", 1), ("dollar", "collar", 1)]
```

### Clustering near-duplicates

`ClusterBy` walks the collection once: each string joins the first existing cluster whose founding member is
close enough, or founds a new cluster. Every string lands in exactly one cluster and order is preserved, which
makes it the natural tool for deduplicating tags, names or file names before a human looks at them.

```csharp
var words = new[] { "color", "colour", "dollar", "collar", "xyz" };

words.ClusterBy(maxDistance: 1);
// [["color", "colour"], ["dollar", "collar"], ["xyz"]]

new[] { "recieve", "receive" }.ClusterBy(1);                                     // [["recieve"], ["receive"]]
new[] { "recieve", "receive" }.ClusterBy(1, StringDistance.DamerauLevenshtein);  // [["recieve", "receive"]]

var people = new[] { "MARTHA", "MARHTA", "DWAYNE", "DUANE" };
people.ClusterBy(minSimilarity: 0.8);   // [["MARTHA", "MARHTA"], ["DWAYNE", "DUANE"]]  (Jaro-Winkler)
```

Because the pass is greedy, the founding member decides membership: a string two edits from the founder does
not join a `maxDistance: 1` cluster even if it is one edit from a later member. That keeps the result
predictable and linear in the number of clusters.

Both ranking members accept `ignoreCase: true`, and both return an empty list for `count: 0`.

### A note for .NET Standard 2.0 consumers

Everything above behaves the same on every target. The one wrinkle is `StringSplitOptions.TrimEntries`,
which .NET Standard 2.0 does not declare. `SplitOutside` tests the flag by value, so pass
`(StringSplitOptions)2` there, or define a constant for it as this repository's tests do. On .NET 5 and
later use the real enum member.

## Sequences

### Index of the first, last or every match

LINQ has `First(predicate)` but not the index of it. `List<T>.FindIndex` exists, but only on `List<T>`.
These work on any `IEnumerable<T>`, stop as soon as they can, and work on infinite sequences.

```csharp
var scores = new[] { 3, 8, 12, 8 };

scores.FindIndex(x => x > 5);          // 1
scores.FindIndex(2, x => x == 8);      // 3   (search starts at index 2; result is absolute)
scores.FindLastIndex(x => x == 8);     // 3
scores.FindIndex(x => x > 100);        // -1
scores.FindIndices(x => x == 8);       // 1, 3   (lazy)
```

### Taking up to and including a match

`TakeWhile` stops *before* the element that fails the test, and there is no built-in way to include it.

```csharp
lines.TakeUntil(l => l.StartsWith("END"));                     // includes the END line
lines.TakeUntil(l => l.StartsWith("END"), inclusive: false);   // same as TakeWhile(!…)
Naturals().TakeUntil(x => x == 3);                             // 0, 1, 2, 3  (infinite source is fine)
```

### Sliding windows and neighbours

`Window(n)` yields every run of `n` consecutive elements, one step at a time; `Pairwise()` is the
two-element case with a tuple instead of a list. Both are lazy and work on infinite sequences.

```csharp
var prices = new[] { 1.0, 2, 3, 4, 5 };
prices.Window(3).Select(w => w.Average());          // 2, 3, 4          (moving average)

var readings = new[] { 10, 12, 15, 11 };
readings.Pairwise().Select(p => p.Current - p.Previous);   // 2, 3, -4   (deltas)
readings.Pairwise().All(p => p.Previous <= p.Current);     // false      (is it sorted?)
```

### Running folds

`Scan` is `Aggregate` that shows its work: it yields the running value after each element. With a seed
the type can change; without one the first element starts the fold.

```csharp
var movements = new[] { 100m, -30m, 45m };
movements.Scan(0m, (balance, m) => balance + m);    // 100, 70, 115     (running balance)

new[] { 3, 1, 7, 2, 9 }.Scan(Math.Max);             // 3, 3, 7, 7, 9    (running maximum)
new[] { 'a', 'b', 'c' }.Scan("", (acc, c) => acc + c);   // "a", "ab", "abc"
```

### Interspersing, partitioning and splitting

```csharp
string.Concat(new[] { "usr", "local", "bin" }.Intersperse("/"));   // "usr/local/bin"

var (evens, odds) = new[] { 1, 2, 3, 4, 5 }.Partition(x => x % 2 == 0);   // [2, 4] and [1, 3, 5], one pass

var lines = new[] { "first paragraph", "continues", "", "second paragraph" };
lines.SplitOn(string.IsNullOrWhiteSpace);   // [["first paragraph", "continues"], ["second paragraph"]]
```

`Partition` walks the source once and returns both halves, where `Where` twice would walk it twice.
`SplitOn` follows `string.Split` rules: separators are dropped, adjacent separators leave an empty group,
and there is always at least one group.

### Grouping consecutive runs

`GroupBy` collects a key from anywhere in the sequence. `ChunkBy` only merges neighbours, so a key that
comes back later starts a new run. That is what run-length encoding, "group these sorted log lines by day"
and "collapse repeated events" all need.

```csharp
"aaabcc".ChunkBy(c => c).Select(run => $"{run.Items.Count}{run.Key}");   // "3a", "1b", "2c"

sortedEvents.ChunkBy(e => e.Timestamp.Date);   // one group per day, in order, without a dictionary
```

### Top and bottom N without a full sort

`TopBy` and `BottomBy` keep only `count` elements in memory however long the source is, so they suit
streams and large files where `OrderByDescending(...).Take(n)` would buffer everything. Ties keep their
source order.

```csharp
var players = new[] { ("ann", 70), ("bob", 90), ("cid", 50), ("dee", 90), ("eve", 80) };

players.TopBy(3, p => p.Item2);      // ("bob", 90), ("dee", 90), ("eve", 80)
players.BottomBy(2, p => p.Item2);   // ("cid", 50), ("ann", 70)
```

### Full outer join

LINQ's `Join` drops unmatched rows on both sides and `GroupJoin` keeps only the left ones. `FullOuterJoin`
keeps everything, which is exactly what reconciliation needs: matched pairs, plus what is missing on each
side, in one pass over the left sequence.

```csharp
var expected = new[] { "a", "b", "c" };
var actual   = new[] { "b", "c", "d" };

expected.FullOuterJoin(actual, e => e, a => a);
// ("a", null)   missing from actual
// ("b", "b")
// ("c", "c")
// (null, "d")   unexpected in actual
```

Each side is `default` when unmatched, so a `null` check (or `is null` pattern) tells you which side is
missing. A key with several partners on the right yields one pair per partner, like an inner join would.

### Null-tolerant emptiness checks

Both accept `null`, never enumerate more than one element, and narrow nullability for the compiler.

```csharp
List<string>? tags = LoadTags();

if (tags.HasItems())
{
    Console.WriteLine(tags[0]);      // no warning: HasItems() proved tags is not null
}

if (tags.IsNullOrEmpty())
{
    return;
}
Console.WriteLine(tags.Count);       // no warning here either
```

## Values

### Membership

For compile-time constants a pattern is better: `x is "a" or "b"`. `In` is for values you only know at run
time. The `params ReadOnlySpan<T>` overload does not allocate.

```csharp
status.In(Status.Draft, Status.Pending);
extension.In(allowedExtensions);                 // any IEnumerable<T>
userId.In(admins);
```

Equality is `EqualityComparer<T>.Default`, so records, tuples, strings and nullable values all behave as expected.

### Range checks

Works with anything that implements `IComparable<T>`: numbers, `DateTime`, `DateOnly`, `TimeSpan`,
`Version`, strings, your own types.

```csharp
5.IsBetween(1, 10);                          // true   (inclusive by default)
10.IsBetween(1, 10);                         // true
10.IsBetween(1, 10, inclusive: false);       // false
today.IsBetween(quarterStart, quarterEnd);
((int)response.StatusCode).IsBetween(200, 299);
```

Strings compare with `string.CompareTo`, which is culture-sensitive. Use ordinal comparison directly when that matters.

## Migrating from the 1.x library

Version 1 shipped three classes all called `Extensions` in three namespaces. Version 2 has one namespace,
`Aelena.CommonExtensions`, and everything below was either removed because the framework covers it or renamed because
its semantics changed. Methods not listed here were dropped without a direct replacement because their behaviour
was unclear or buggy (`Interpolate` always returned an empty string; `AreAllNull` actually tested *any* null).

| 1.x member | Use instead |
|---|---|
| `ToStringSafe()` | `obj?.ToString() ?? ""` |
| `ToStringExpanded()` | a `record`, or `JsonSerializer.Serialize(obj)` |
| `IsFirst(list)` | `list.FirstOrDefault()?.Equals(x)` |
| `Prepend(s, n)` / `Append(s, n)` | `string.Concat`, interpolation, `new string(c, n)` |
| `FromCharListToString()` | `string.Concat(chars)` |
| `Split2(separators)` | `s.Split(separators, options)` |
| `Split2(separators, pairs)` | `s.SplitOutside(separators, pairs, options)` |
| `JoinTogether(sep)` / `JoinTogetherBetween` | `string.Join(sep, seq)` |
| `Take(from, to)` / `From(n)` / `To(n)` | `seq.Take(from..to)`, `seq.Skip(n)`, `seq.Take(n)` |
| `Penultimate()` / `ElementAtFromLast(n)` | `list[^2]`, `seq.ElementAt(^n)` |
| `ContainsAny<T>` / `ContainsAny2<T>` on sequences | `a.Intersect(b).Any()`, `b.FirstOrDefault(a.Contains)` |
| `TakeAfter(pred)` | `seq.SkipWhile(x => !pred(x)).Skip(1)` |
| `RemoveAfter(pred)` | `seq.TakeUntil(pred)` |
| `FindNth(pred, n)` | `seq.Where(pred).ElementAtOrDefault(n)` |
| `ElementsAt(indices)` | `indices.Select(i => list[i])` |
| `GetKeys()` | `pairs.Select(p => p.Key)` |
| `Split<T>(n)` on sequences | `seq.Chunk(n)` |
| `IndexOf(pred)` / `IndicesOf(pred)` | `FindIndex(pred)` / `FindIndices(pred)` |
| `IndexOf(pred, after)` | `FindIndex(start, pred)` (now returns an absolute index) |
| `HasElements()` | `HasItems()` |
| `InBetweenAny(intervals)` (7 overloads) | `intervals.Any(r => x.IsBetween(r.Low, r.High))` |
| `SubStringAfter` / `SubStringAfterLast` | `After` / `AfterLast` (ordinal by default now) |
| `TakeBetween(a, b)` | `Between(a, b)` |
| `TakeBetween(mark, a, b)` | `After(mark).Between(a, b)` |
| `TakeBetweenMultiple` / `FindAllBetween` | `AllBetween(a, b)` |
| `RemoveBetween(values, a, b)` | `s.ReplaceFirst(s.Between(a, b), s.Between(a, b).RemoveAll(values))` |
| `ContainsAny(list, wordBoundaries: true)` / `ReplaceWord` | `Regex` with `\b` and `Regex.Escape` |
| `MultipleRemove(values)` | `RemoveAll(values)` |
| `RemoveLast(x)` | `ReplaceLast(x, "")` |
| `ReplaceFirstAndLastOnly(x, y)` | `s.ReplaceFirst(x, y).ReplaceLast(x, y)` |
| `IndicesOfAll(x)` / `IndicesOfAll2(list)` | `AllIndicesOf(x)` / `list.SelectMany(v => s.AllIndicesOf(v).Select(i => (i, v)))` |
| `SubstringSafe(i, n)` / `TakeFrom(i)` / `RemoveFromEnd(n)` | `SafeSubstring(i, n)` / `SafeSubstring(i)` / `SafeSubstring(0, s.Length - n)` |
| `ParseToStringSafe<T>(format)` | `T.TryParse(s, out var v) ? v.ToString(format) : fallback` with `IParsable<T>` |

## Building and testing

Requirements: the .NET 10 SDK or newer (`global.json` rolls forward to the latest major, including .NET 11
previews), plus the .NET 8, 10 and 11 runtimes to execute the tests on every target. On Windows the test
project also targets `net48`, which runs the whole suite against the `netstandard2.0` build of the library;
CI runs on Windows for that reason.

```shell
dotnet build
dotnet test                      # 438 tests x 4 frameworks
pwsh scripts/coverage.ps1        # tests + merged coverage report + 100% gate
dotnet pack src/Aelena.CommonExtensions/Aelena.CommonExtensions.csproj -c Release
```

Tests use xUnit v3 on the Microsoft.Testing.Platform runner. Coverage is collected with
`Microsoft.Testing.Extensions.CodeCoverage`, merged across the three frameworks with ReportGenerator
(a local `dotnet tool`), and the gate fails the build when **line, branch or method coverage is below 100%**.
The HTML report lands in `TestResults/report/index.html`.

Analyzers run at `latest-recommended` with warnings as errors, and the library is marked trimmable and
AOT-compatible.

## Releasing

Releases are tag-driven. Pushing a tag `vX.Y.Z` runs the full CI gate on that commit, packs, checks that
the tag matches `<Version>` in the csproj, verifies the package contents, publishes to nuget.org via
[Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing) (no stored API key),
and creates a GitHub release with the packages attached.

```shell
# 1. bump <Version> in src/Aelena.CommonExtensions/Aelena.CommonExtensions.csproj and update CHANGELOG.md
# 2. commit, then:
git tag v2.3.0
git push origin v2.3.0
```

Run the *Release* workflow manually with `dry_run` checked to rehearse everything except the push.

## License

[MIT](LICENSE).
