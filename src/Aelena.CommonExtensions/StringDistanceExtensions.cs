namespace Aelena.CommonExtensions;

/// <summary>
/// Edit-distance members: the Levenshtein distance between two strings, and helpers that rank a
/// collection of strings by how close they are to a target or to each other.
/// </summary>
/// <remarks>
/// Distance is measured in UTF-16 code units. Case-insensitive comparison, when requested, uses
/// <see cref="char.ToUpperInvariant(char)"/> per character.
/// </remarks>
public static class StringDistanceExtensions
{
    private const int StackBufferLength = 256;

    /// <param name="s">The string to compare from. Must not be <see langword="null"/>.</param>
    extension(string s)
    {
        /// <summary>
        /// Computes the Levenshtein distance to <paramref name="other"/>: the minimum number of single-character
        /// insertions, deletions and substitutions that turn one string into the other.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>Zero for equal strings; at most the length of the longer string.</returns>
        public int LevenshteinDistance(string other, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            return Distance(s.AsSpan(), other.AsSpan(), ignoreCase);
        }
    }

    /// <param name="candidates">The strings to rank. Must not be <see langword="null"/> and must not contain <see langword="null"/>.</param>
    extension(IEnumerable<string> candidates)
    {
        /// <summary>
        /// Returns the candidates closest to <paramref name="target"/>, best match first.
        /// </summary>
        /// <param name="target">The string to measure against.</param>
        /// <param name="count">How many results to return at most. Zero yields an empty list.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>
        /// Up to <paramref name="count"/> (value, distance) pairs ordered by ascending distance.
        /// Candidates at the same distance keep their original relative order.
        /// </returns>
        public IReadOnlyList<(string Value, int Distance)> ClosestTo(string target, int count = 1, bool ignoreCase = false)
        {
            Guard.NotNull(candidates);
            Guard.NotNull(target);
            Guard.NotNegative(count);

            return count == 0
                ? []
                : [.. candidates
                    .Select(candidate => (Value: candidate, Distance: candidate.LevenshteinDistance(target, ignoreCase)))
                    .OrderBy(pair => pair.Distance)
                    .Take(count)];
        }

        /// <summary>
        /// Returns the pairs of candidates that are closest to each other, best pair first.
        /// Every unordered pair is measured, so this costs O(n²) distance computations.
        /// </summary>
        /// <param name="count">How many pairs to return at most. Zero yields an empty list.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>
        /// Up to <paramref name="count"/> (first, second, distance) triples ordered by ascending distance, where
        /// <c>First</c> precedes <c>Second</c> in the source. Pairs at the same distance keep source order.
        /// Fewer than two candidates yield an empty list.
        /// </returns>
        public IReadOnlyList<(string First, string Second, int Distance)> ClosestPairs(int count = 1, bool ignoreCase = false)
        {
            Guard.NotNull(candidates);
            Guard.NotNegative(count);

            if (count == 0)
            {
                return [];
            }

            var items = candidates.ToArray();
            var pairs = new List<(string First, string Second, int Distance)>(items.Length * (items.Length - 1) / 2);
            for (var i = 0; i < items.Length; i++)
            {
                for (var j = i + 1; j < items.Length; j++)
                {
                    pairs.Add((items[i], items[j], items[i].LevenshteinDistance(items[j], ignoreCase)));
                }
            }

            return [.. pairs.OrderBy(pair => pair.Distance).Take(count)];
        }
    }

    private static int Distance(ReadOnlySpan<char> a, ReadOnlySpan<char> b, bool ignoreCase)
    {
        // Keep the shorter string as the row so the working buffer is as small as possible.
        if (a.Length < b.Length)
        {
            var longer = b;
            b = a;
            a = longer;
        }

        if (b.IsEmpty)
        {
            return a.Length;
        }

        Span<int> previous = b.Length < StackBufferLength ? stackalloc int[StackBufferLength] : new int[b.Length + 1];
        Span<int> current = b.Length < StackBufferLength ? stackalloc int[StackBufferLength] : new int[b.Length + 1];
        previous = previous[..(b.Length + 1)];
        current = current[..(b.Length + 1)];

        for (var j = 0; j <= b.Length; j++)
        {
            previous[j] = j;
        }

        for (var i = 1; i <= a.Length; i++)
        {
            current[0] = i;
            var ca = ignoreCase ? char.ToUpperInvariant(a[i - 1]) : a[i - 1];

            for (var j = 1; j <= b.Length; j++)
            {
                var cb = ignoreCase ? char.ToUpperInvariant(b[j - 1]) : b[j - 1];
                var substitution = previous[j - 1] + (ca == cb ? 0 : 1);
                var insertion = current[j - 1] + 1;
                var deletion = previous[j] + 1;
                current[j] = Math.Min(substitution, Math.Min(insertion, deletion));
            }

            var finished = current;
            current = previous;
            previous = finished;
        }

        return previous[b.Length];
    }
}
