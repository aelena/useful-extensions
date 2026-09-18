namespace Aelena.CommonExtensions;

/// <summary>
/// Edit distances and common-subsequence measures between two strings.
/// </summary>
/// <remarks>
/// Everything is measured in UTF-16 code units. Case-insensitive comparison, when requested, folds both
/// strings with <see cref="string.ToUpperInvariant"/> first. Rankings over collections live in
/// <see cref="StringRankingExtensions"/>, similarity scores in <see cref="StringSimilarityExtensions"/>.
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

            return Levenshtein(s.AsSpan(), other.AsSpan(), ignoreCase);
        }

        /// <summary>
        /// Computes the Damerau-Levenshtein distance to <paramref name="other"/>: like Levenshtein, but swapping two
        /// adjacent characters counts as one edit, so <c>"recieve"</c> is 1 away from <c>"receive"</c>.
        /// </summary>
        /// <remarks>
        /// This is the optimal string alignment variant: a transposed pair is not edited again afterwards.
        /// </remarks>
        /// <param name="other">The string to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>Zero for equal strings; at most the length of the longer string.</returns>
        public int DamerauLevenshteinDistance(string other, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            var (a, b) = Fold(s, other, ignoreCase);
            if (a.Length == 0 || b.Length == 0)
            {
                return a.Length + b.Length;
            }

            var twoAgo = new int[b.Length + 1];
            var previous = new int[b.Length + 1];
            var current = new int[b.Length + 1];
            for (var j = 0; j <= b.Length; j++)
            {
                previous[j] = j;
            }

            for (var i = 1; i <= a.Length; i++)
            {
                current[0] = i;
                for (var j = 1; j <= b.Length; j++)
                {
                    var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    current[j] = Math.Min(Math.Min(previous[j] + 1, current[j - 1] + 1), previous[j - 1] + cost);

                    if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                    {
                        current[j] = Math.Min(current[j], twoAgo[j - 2] + 1);
                    }
                }

                (twoAgo, previous, current) = (previous, current, twoAgo);
            }

            return previous[b.Length];
        }

        /// <summary>
        /// Computes the Hamming distance to <paramref name="other"/>: the number of positions at which the two
        /// strings differ. Both strings must have the same length.
        /// </summary>
        /// <param name="other">The string to compare with. Must be the same length as this one.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>Zero for equal strings; at most the length of the strings.</returns>
        /// <exception cref="ArgumentException">The strings have different lengths.</exception>
        public int HammingDistance(string other, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);
            if (s.Length != other.Length)
            {
                throw new ArgumentException("Hamming distance is only defined for strings of equal length.", nameof(other));
            }

            var (a, b) = Fold(s, other, ignoreCase);
            var distance = 0;
            for (var i = 0; i < a.Length; i++)
            {
                distance += a[i] == b[i] ? 0 : 1;
            }

            return distance;
        }

        /// <summary>
        /// Returns the length of the longest subsequence common to both strings. Characters of a subsequence keep
        /// their relative order but need not be adjacent, so <c>"ABCBDAB"</c> and <c>"BDCABA"</c> share <c>"BCBA"</c>.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>Zero when nothing is shared; at most the length of the shorter string.</returns>
        public int LongestCommonSubsequenceLength(string other, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            var (a, b) = Fold(s, other, ignoreCase);
            var previous = new int[b.Length + 1];
            var current = new int[b.Length + 1];
            for (var i = 1; i <= a.Length; i++)
            {
                for (var j = 1; j <= b.Length; j++)
                {
                    current[j] = a[i - 1] == b[j - 1] ? previous[j - 1] + 1 : Math.Max(previous[j], current[j - 1]);
                }

                (previous, current) = (current, previous);
            }

            return previous[b.Length];
        }

        /// <summary>
        /// Returns the longest run of characters that appears in both strings, taken from this string so that its
        /// original casing is preserved. When several runs tie, the one that starts earliest in this string wins.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>The shared substring, or <see cref="string.Empty"/> when the strings share no character.</returns>
        public string LongestCommonSubstring(string other, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            var (a, b) = Fold(s, other, ignoreCase);
            var previous = new int[b.Length + 1];
            var current = new int[b.Length + 1];
            var bestLength = 0;
            var bestEnd = 0;
            for (var i = 1; i <= a.Length; i++)
            {
                for (var j = 1; j <= b.Length; j++)
                {
                    current[j] = a[i - 1] == b[j - 1] ? previous[j - 1] + 1 : 0;
                    if (current[j] > bestLength)
                    {
                        bestLength = current[j];
                        bestEnd = i;
                    }
                }

                (previous, current) = (current, previous);
            }

            return s.Substring(bestEnd - bestLength, bestLength);
        }

        /// <summary>
        /// Lazily returns every run of <paramref name="size"/> consecutive characters, so <c>"abcd".NGrams(2)</c>
        /// yields <c>"ab"</c>, <c>"bc"</c>, <c>"cd"</c>. A string shorter than <paramref name="size"/> yields nothing.
        /// </summary>
        /// <param name="size">The length of each n-gram. Must be positive.</param>
        /// <returns>The n-grams in order, including duplicates.</returns>
        public IEnumerable<string> NGrams(int size)
        {
            Guard.NotNull(s);
            Guard.Positive(size);

            return Iterate();

            IEnumerable<string> Iterate()
            {
                for (var i = 0; i + size <= s.Length; i++)
                {
                    yield return s.Substring(i, size);
                }
            }
        }
    }

    internal static (string A, string B) Fold(string a, string b, bool ignoreCase)
        => ignoreCase ? (a.ToUpperInvariant(), b.ToUpperInvariant()) : (a, b);

    private static int Levenshtein(ReadOnlySpan<char> a, ReadOnlySpan<char> b, bool ignoreCase)
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
