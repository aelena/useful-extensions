namespace Aelena.CommonExtensions;

/// <summary>
/// Similarity scores between two strings, all in the range 0 (nothing in common) to 1 (identical).
/// </summary>
/// <remarks>
/// Two empty strings are identical, so every score returns 1 for them. Case-insensitive comparison, when
/// requested, folds both strings with <see cref="string.ToUpperInvariant"/> first.
/// </remarks>
public static class StringSimilarityExtensions
{
    private const int WinklerPrefixLimit = 4;

    /// <param name="s">The string to compare from. Must not be <see langword="null"/>.</param>
    extension(string s)
    {
        /// <summary>
        /// Levenshtein distance turned into a score: 1 minus the distance divided by the longer length.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>A score from 0 to 1.</returns>
        public double LevenshteinSimilarity(string other, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            var longest = Math.Max(s.Length, other.Length);
            return longest == 0 ? 1 : 1 - (double)s.LevenshteinDistance(other, ignoreCase) / longest;
        }

        /// <summary>
        /// Jaro similarity: rewards characters that match within a sliding window and penalises the ones that
        /// match out of order. Tolerant of length differences, which makes it a good fit for names.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>A score from 0 to 1.</returns>
        public double JaroSimilarity(string other, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            var (a, b) = StringDistanceExtensions.Fold(s, other, ignoreCase);
            if (a.Length == 0 && b.Length == 0)
            {
                return 1;
            }

            var window = Math.Max(0, Math.Max(a.Length, b.Length) / 2 - 1);
            var aMatched = new bool[a.Length];
            var bMatched = new bool[b.Length];
            var matches = 0;

            for (var i = 0; i < a.Length; i++)
            {
                var last = Math.Min(i + window, b.Length - 1);
                for (var j = Math.Max(0, i - window); j <= last; j++)
                {
                    if (!bMatched[j] && a[i] == b[j])
                    {
                        aMatched[i] = bMatched[j] = true;
                        matches++;
                        break;
                    }
                }
            }

            if (matches == 0)
            {
                return 0;
            }

            var outOfOrder = 0;
            var k = 0;
            for (var i = 0; i < a.Length; i++)
            {
                if (!aMatched[i])
                {
                    continue;
                }

                while (!bMatched[k])
                {
                    k++;
                }

                outOfOrder += a[i] == b[k] ? 0 : 1;
                k++;
            }

            double m = matches;
            var transpositions = outOfOrder / 2;
            return (m / a.Length + m / b.Length + (m - transpositions) / m) / 3;
        }

        /// <summary>
        /// Jaro-Winkler similarity: <see cref="JaroSimilarity"/> plus a bonus for a shared prefix of up to four
        /// characters, so strings that start the same way score higher.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <param name="prefixScale">
        /// How much each shared prefix character is worth. The customary value is 0.1 and it must not exceed 0.25,
        /// or the score could exceed 1.
        /// </param>
        /// <returns>A score from 0 to 1.</returns>
        public double JaroWinklerSimilarity(string other, bool ignoreCase = false, double prefixScale = 0.1)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);
            if (prefixScale is < 0 or > 0.25)
            {
                throw new ArgumentOutOfRangeException(nameof(prefixScale), prefixScale, "The prefix scale must be between 0 and 0.25.");
            }

            var jaro = s.JaroSimilarity(other, ignoreCase);
            var (a, b) = StringDistanceExtensions.Fold(s, other, ignoreCase);
            var limit = Math.Min(WinklerPrefixLimit, Math.Min(a.Length, b.Length));
            var prefix = 0;
            while (prefix < limit && a[prefix] == b[prefix])
            {
                prefix++;
            }

            return jaro + prefix * prefixScale * (1 - jaro);
        }

        /// <summary>
        /// Longest common subsequence turned into a score: twice the shared length divided by the total length.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>A score from 0 to 1.</returns>
        public double LongestCommonSubsequenceSimilarity(string other, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            var total = s.Length + other.Length;
            return total == 0 ? 1 : 2.0 * s.LongestCommonSubsequenceLength(other, ignoreCase) / total;
        }

        /// <summary>
        /// Sørensen-Dice coefficient over the sets of n-grams: twice the shared n-grams divided by the total.
        /// Insensitive to word order and to small length differences.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="size">The n-gram length. Bigrams (2) are the usual choice.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>A score from 0 to 1.</returns>
        public double DiceSimilarity(string other, int size = 2, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            var (a, b, same) = GramSets(s, other, size, ignoreCase);
            var total = a.Count + b.Count;
            return total == 0 ? same : 2.0 * a.Count(b.Contains) / total;
        }

        /// <summary>
        /// Jaccard index over the sets of n-grams: the shared n-grams divided by the distinct n-grams of both.
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <param name="size">The n-gram length. Bigrams (2) are the usual choice.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>A score from 0 to 1.</returns>
        public double JaccardSimilarity(string other, int size = 2, bool ignoreCase = false)
        {
            Guard.NotNull(s);
            Guard.NotNull(other);

            var (a, b, same) = GramSets(s, other, size, ignoreCase);
            var shared = a.Count(b.Contains);
            var union = a.Count + b.Count - shared;
            return union == 0 ? same : (double)shared / union;
        }
    }

    /// <summary>
    /// The n-gram sets of both strings, plus the score to use when neither string is long enough to have any
    /// n-gram at all: 1 when the strings are equal, 0 otherwise.
    /// </summary>
    private static (HashSet<string> A, HashSet<string> B, double WhenEmpty) GramSets(string s, string other, int size, bool ignoreCase)
    {
        var (a, b) = StringDistanceExtensions.Fold(s, other, ignoreCase);
        return ([.. a.NGrams(size)], [.. b.NGrams(size)], a == b ? 1 : 0);
    }
}
