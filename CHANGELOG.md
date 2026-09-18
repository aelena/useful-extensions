# Changelog

All notable changes to this project are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project uses
[Semantic Versioning](https://semver.org/).

## [2.3.1] - 2026-09-18

### Added

- Functional helpers: `Pipe`, `Tap`, `TryPipe` on any value; `Try` and `Memoize` on `Func<TResult>`;
  `Memoize` on `Func<T, TResult>` (with optional comparer) and `Func<T1, T2, TResult>`.
- README badges for NuGet version and downloads, license, test count and release.

## [2.3.0] - 2026-09-18

### Added

- Edit distances: `DamerauLevenshteinDistance` (optimal string alignment) and `HammingDistance`.
- Shared text: `LongestCommonSubsequenceLength`, `LongestCommonSubstring`, `NGrams`.
- Similarity scores from 0 to 1: `JaroSimilarity`, `JaroWinklerSimilarity`, `LevenshteinSimilarity`,
  `LongestCommonSubsequenceSimilarity`, `DiceSimilarity`, `JaccardSimilarity`.
- `StringDistance` and `StringSimilarity` enums; `ClosestTo` and `ClosestPairs` take a `metric`.
- `MostSimilarTo`: ranks candidates by any similarity score, best first.
- `ClusterBy`: greedy near-duplicate grouping by a distance threshold or a similarity threshold.
- Sequence operators: `Window`, `Pairwise`, `Scan` (with and without seed), `Intersperse`, `Partition`,
  `SplitOn`, `ChunkBy`, `TopBy`, `BottomBy`, `FullOuterJoin`.

## [2.2.0] - 2026-09-17

### Added

- `netstandard2.0` target, so the library runs on .NET Framework 4.6.2+, .NET Core 2+, Mono, Unity and
  in other .NET Standard 2.0 libraries. Behaviour is identical on every target; the test suite runs the
  .NET Standard build on .NET Framework 4.8.

### Changed

- On .NET Standard 2.0, `SplitOutside` reads the trim flag by value (`(StringSplitOptions)2`) because that
  framework does not declare `StringSplitOptions.TrimEntries`.

## [2.1.0] - 2026-09-17

### Added

- `LevenshteinDistance` on `string`, with an `ignoreCase` option.
- `ClosestTo` on `IEnumerable<string>`: ranks candidates against a target, best match first.
- `ClosestPairs` on `IEnumerable<string>`: finds the nearest pairs inside a collection.

## [2.0.2] - 2026-09-17

### Added

- MIT license file and `PackageLicenseExpression` in the package metadata. No code changes.

## [2.0.1] - 2026-09-17

First published 2.x release. Version 2.0.0 was tagged but never reached nuget.org; it was
withdrawn before publishing to rename the assembly and namespace to `Aelena.CommonExtensions`.

Complete rewrite for .NET 8, 10 and 11, published under the existing NuGet id `Common-Extensions`
(previous release: 0.8.0). Earlier versions targeted .NET Framework 4.5;
nothing in 2.0 is source-compatible with it. See the
[migration table](README.md#migrating-from-the-1x-library) in the README.

### Added

- `string`: `After`, `AfterLast`, `Before`, `BeforeLast` (string and char overloads),
  `Between`, `AllBetween`, `ReplaceFirst`, `ReplaceLast`, `RemoveAll`, `Truncate`,
  `SafeSubstring`, `RemoveDiacritics`, `AllIndicesOf`, `ContainsAny`, `FirstContained`,
  `SplitOutside`.
- `IEnumerable<T>`: `FindIndex`, `FindIndex(startIndex)`, `FindLastIndex`, `FindIndices`,
  `TakeUntil`, `IsNullOrEmpty`, `HasItems`.
- Any value: `In` (allocation-free `params ReadOnlySpan<T>` overload), `IsBetween`.
- Multi-targeting for `net8.0`, `net10.0` and `net11.0`; trimmable and AOT-compatible.
- Symbols package and Source Link.

### Changed

- Single namespace `Aelena.CommonExtensions` replaces the three `*.Extensions` classes of 1.x.
- All string comparisons default to `StringComparison.Ordinal` (1.x used the current culture).
- Methods that return `IEnumerable<T>` validate their arguments eagerly and are lazy otherwise.

### Removed

- Every 1.x member that the BCL or the language now covers (`ToStringSafe`, `JoinTogether`,
  `Penultimate`, `Split<T>`, `Take(from, to)`, and about thirty more).
- `Interpolate`, `AreAllNull`, `ReplaceWord`, `InsertMultiple` and `ParseToStringSafe`,
  whose behaviour was incorrect or undefined.

[2.3.1]: https://github.com/aelena/useful-extensions/releases/tag/v2.3.1
[2.3.0]: https://github.com/aelena/useful-extensions/releases/tag/v2.3.0
[2.2.0]: https://github.com/aelena/useful-extensions/releases/tag/v2.2.0
[2.1.0]: https://github.com/aelena/useful-extensions/releases/tag/v2.1.0
[2.0.2]: https://github.com/aelena/useful-extensions/releases/tag/v2.0.2
[2.0.1]: https://github.com/aelena/useful-extensions/releases/tag/v2.0.1
