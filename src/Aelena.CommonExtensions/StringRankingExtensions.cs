namespace Aelena.CommonExtensions;

/// <summary>
/// Ranks and groups a collection of strings by how close its members are to a target or to each other,
/// using any <see cref="StringDistance"/> or <see cref="StringSimilarity"/> metric.
/// </summary>
/// <remarks>
/// All rankings are stable: candidates that score the same keep their original relative order.
/// </remarks>
public static class StringRankingExtensions
{
    /// <param name="candidates">The strings to rank. Must not be <see langword="null"/> and must not contain <see langword="null"/>.</param>
    extension(IEnumerable<string> candidates)
    {
        /// <summary>Returns the candidates closest to <paramref name="target"/>, best match first.</summary>
        /// <param name="target">The string to measure against.</param>
        /// <param name="count">How many results to return at most. Zero yields an empty list.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <param name="metric">The edit distance to rank by.</param>
        /// <returns>Up to <paramref name="count"/> (value, distance) pairs ordered by ascending distance.</returns>
        public IReadOnlyList<(string Value, int Distance)> ClosestTo(
            string target,
            int count = 1,
            bool ignoreCase = false,
            StringDistance metric = StringDistance.Levenshtein)
        {
            Guard.NotNull(candidates);
            Guard.NotNull(target);
            Guard.NotNegative(count);

            return count == 0
                ? []
                : [.. candidates
                    .Select(candidate => (Value: candidate, Distance: Measure(candidate, target, metric, ignoreCase)))
                    .OrderBy(pair => pair.Distance)
                    .Take(count)];
        }

        /// <summary>Returns the candidates most similar to <paramref name="target"/>, best match first.</summary>
        /// <param name="target">The string to measure against.</param>
        /// <param name="count">How many results to return at most. Zero yields an empty list.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <param name="metric">The similarity score to rank by.</param>
        /// <returns>Up to <paramref name="count"/> (value, similarity) pairs ordered by descending similarity.</returns>
        public IReadOnlyList<(string Value, double Similarity)> MostSimilarTo(
            string target,
            int count = 1,
            bool ignoreCase = false,
            StringSimilarity metric = StringSimilarity.JaroWinkler)
        {
            Guard.NotNull(candidates);
            Guard.NotNull(target);
            Guard.NotNegative(count);

            return count == 0
                ? []
                : [.. candidates
                    .Select(candidate => (Value: candidate, Similarity: Score(candidate, target, metric, ignoreCase)))
                    .OrderByDescending(pair => pair.Similarity)
                    .Take(count)];
        }

        /// <summary>
        /// Returns the pairs of candidates that are closest to each other, best pair first.
        /// Every unordered pair is measured, so this costs O(n²) distance computations.
        /// </summary>
        /// <param name="count">How many pairs to return at most. Zero yields an empty list.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <param name="metric">The edit distance to rank by.</param>
        /// <returns>
        /// Up to <paramref name="count"/> (first, second, distance) triples ordered by ascending distance, where
        /// <c>First</c> precedes <c>Second</c> in the source. Fewer than two candidates yield an empty list.
        /// </returns>
        public IReadOnlyList<(string First, string Second, int Distance)> ClosestPairs(
            int count = 1,
            bool ignoreCase = false,
            StringDistance metric = StringDistance.Levenshtein)
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
                    pairs.Add((items[i], items[j], Measure(items[i], items[j], metric, ignoreCase)));
                }
            }

            return [.. pairs.OrderBy(pair => pair.Distance).Take(count)];
        }

        /// <summary>
        /// Groups candidates that are within <paramref name="maxDistance"/> edits of each other, in one greedy pass:
        /// each candidate joins the first existing cluster whose founding member is close enough, or founds a new one.
        /// </summary>
        /// <param name="maxDistance">The largest distance to the cluster's first member that still counts as the same cluster.</param>
        /// <param name="metric">The edit distance to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>The clusters in order of founding, each in source order. Every candidate appears in exactly one cluster.</returns>
        public IReadOnlyList<IReadOnlyList<string>> ClusterBy(
            int maxDistance,
            StringDistance metric = StringDistance.Levenshtein,
            bool ignoreCase = false)
        {
            Guard.NotNull(candidates);
            Guard.NotNegative(maxDistance);

            return Cluster(candidates, (a, b) => Measure(a, b, metric, ignoreCase) <= maxDistance);
        }

        /// <summary>
        /// Groups candidates whose similarity is at least <paramref name="minSimilarity"/>, in one greedy pass:
        /// each candidate joins the first existing cluster whose founding member is similar enough, or founds a new one.
        /// </summary>
        /// <param name="minSimilarity">The lowest similarity to the cluster's first member that still counts as the same cluster, from 0 to 1.</param>
        /// <param name="metric">The similarity score to compare with.</param>
        /// <param name="ignoreCase">Whether characters differing only in case count as equal.</param>
        /// <returns>The clusters in order of founding, each in source order. Every candidate appears in exactly one cluster.</returns>
        public IReadOnlyList<IReadOnlyList<string>> ClusterBy(
            double minSimilarity,
            StringSimilarity metric = StringSimilarity.JaroWinkler,
            bool ignoreCase = false)
        {
            Guard.NotNull(candidates);

            return minSimilarity is < 0 or > 1
                ? throw new ArgumentOutOfRangeException(nameof(minSimilarity), minSimilarity, "The similarity threshold must be between 0 and 1.")
                : Cluster(candidates, (a, b) => Score(a, b, metric, ignoreCase) >= minSimilarity);
        }
    }

    private static List<List<string>> Cluster(IEnumerable<string> candidates, Func<string, string, bool> belongTogether)
    {
        var clusters = new List<List<string>>();
        foreach (var candidate in candidates)
        {
            Guard.NotNull(candidate, nameof(candidates));
            var home = clusters.Find(cluster => belongTogether(cluster[0], candidate));
            if (home is null)
            {
                clusters.Add([candidate]);
            }
            else
            {
                home.Add(candidate);
            }
        }

        return clusters;
    }

    private static int Measure(string a, string b, StringDistance metric, bool ignoreCase) => metric switch
    {
        StringDistance.Levenshtein => a.LevenshteinDistance(b, ignoreCase),
        StringDistance.DamerauLevenshtein => a.DamerauLevenshteinDistance(b, ignoreCase),
        _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, "Unknown distance metric."),
    };

    private static double Score(string a, string b, StringSimilarity metric, bool ignoreCase) => metric switch
    {
        StringSimilarity.JaroWinkler => a.JaroWinklerSimilarity(b, ignoreCase),
        StringSimilarity.Jaro => a.JaroSimilarity(b, ignoreCase),
        StringSimilarity.Levenshtein => a.LevenshteinSimilarity(b, ignoreCase),
        StringSimilarity.LongestCommonSubsequence => a.LongestCommonSubsequenceSimilarity(b, ignoreCase),
        StringSimilarity.Dice => a.DiceSimilarity(b, ignoreCase: ignoreCase),
        StringSimilarity.Jaccard => a.JaccardSimilarity(b, ignoreCase: ignoreCase),
        _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, "Unknown similarity metric."),
    };
}
