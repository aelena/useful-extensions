# Changelog

All notable changes to this project are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project uses
[Semantic Versioning](https://semver.org/).

## [2.0.0] - 2026-09-17

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

- Single namespace `Aelena.Extensions` replaces the three `*.Extensions` classes of 1.x.
- All string comparisons default to `StringComparison.Ordinal` (1.x used the current culture).
- Methods that return `IEnumerable<T>` validate their arguments eagerly and are lazy otherwise.

### Removed

- Every 1.x member that the BCL or the language now covers (`ToStringSafe`, `JoinTogether`,
  `Penultimate`, `Split<T>`, `Take(from, to)`, and about thirty more).
- `Interpolate`, `AreAllNull`, `ReplaceWord`, `InsertMultiple` and `ParseToStringSafe`,
  whose behaviour was incorrect or undefined.

[2.0.0]: https://github.com/aelena/useful-extensions/releases/tag/v2.0.0
