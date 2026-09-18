namespace Aelena.CommonExtensions.Tests;

public class StringMetricsTests
{
    private const int Precision = 4;

    // ------------------------------------------------------------------ DamerauLevenshteinDistance

    [Theory]
    [InlineData("gato", "gato", 0)]
    [InlineData("gato", "gaot", 1)]
    [InlineData("gato", "gat", 1)]
    [InlineData("gato", "gatos", 1)]
    [InlineData("gato", "goti", 2)]
    [InlineData("recieve", "receive", 1)]
    [InlineData("abcd", "acbd", 1)]
    [InlineData("abcd", "badc", 2)]
    [InlineData("ca", "abc", 3)]
    [InlineData("kitten", "sitting", 3)]
    [InlineData("", "", 0)]
    [InlineData("", "abc", 3)]
    [InlineData("abc", "", 3)]
    [InlineData("aa", "aa", 0)]
    public void DamerauLevenshteinDistance_counts_transpositions_as_one_edit(string a, string b, int expected)
    {
        Assert.Equal(expected, a.DamerauLevenshteinDistance(b));
        Assert.Equal(expected, b.DamerauLevenshteinDistance(a));
    }

    [Fact]
    public void DamerauLevenshteinDistance_never_exceeds_levenshtein()
    {
        foreach (var (a, b) in new[] { ("recieve", "receive"), ("abcd", "badc"), ("Saturday", "Sunday") })
        {
            Assert.True(a.DamerauLevenshteinDistance(b) <= a.LevenshteinDistance(b));
        }
    }

    [Fact]
    public void DamerauLevenshteinDistance_can_ignore_case()
    {
        Assert.Equal(1, "GATO".DamerauLevenshteinDistance("gaot", ignoreCase: true));
        Assert.Equal(4, "GATO".DamerauLevenshteinDistance("gaot"));
    }

    [Fact]
    public void DamerauLevenshteinDistance_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.DamerauLevenshteinDistance("a"));
        Assert.Throws<ArgumentNullException>(() => "a".DamerauLevenshteinDistance(null!));
    }

    // ------------------------------------------------------------------ HammingDistance

    [Theory]
    [InlineData("karolin", "kathrin", 3)]
    [InlineData("1011101", "1001001", 2)]
    [InlineData("abc", "abc", 0)]
    [InlineData("abc", "xyz", 3)]
    [InlineData("", "", 0)]
    public void HammingDistance_counts_differing_positions(string a, string b, int expected)
        => Assert.Equal(expected, a.HammingDistance(b));

    [Fact]
    public void HammingDistance_can_ignore_case()
    {
        Assert.Equal(3, "ABC".HammingDistance("abc"));
        Assert.Equal(0, "ABC".HammingDistance("abc", ignoreCase: true));
    }

    [Fact]
    public void HammingDistance_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.HammingDistance("a"));
        Assert.Throws<ArgumentNullException>(() => "a".HammingDistance(null!));
        Assert.Throws<ArgumentException>(() => "abc".HammingDistance("ab"));
    }

    // ------------------------------------------------------------------ LongestCommonSubsequenceLength

    [Theory]
    [InlineData("ABCBDAB", "BDCABA", 4)]
    [InlineData("abc", "abc", 3)]
    [InlineData("abc", "def", 0)]
    [InlineData("abc", "", 0)]
    [InlineData("", "abc", 0)]
    [InlineData("", "", 0)]
    [InlineData("AGGTAB", "GXTXAYB", 4)]
    public void LongestCommonSubsequenceLength_counts_ordered_shared_characters(string a, string b, int expected)
    {
        Assert.Equal(expected, a.LongestCommonSubsequenceLength(b));
        Assert.Equal(expected, b.LongestCommonSubsequenceLength(a));
    }

    [Fact]
    public void LongestCommonSubsequenceLength_can_ignore_case()
    {
        Assert.Equal(0, "ABC".LongestCommonSubsequenceLength("abc"));
        Assert.Equal(3, "ABC".LongestCommonSubsequenceLength("abc", ignoreCase: true));
    }

    [Fact]
    public void LongestCommonSubsequenceLength_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.LongestCommonSubsequenceLength("a"));
        Assert.Throws<ArgumentNullException>(() => "a".LongestCommonSubsequenceLength(null!));
    }

    // ------------------------------------------------------------------ LongestCommonSubstring

    [Theory]
    [InlineData("the quick brown fox", "a quick brown dog", " quick brown ")]
    [InlineData("abc", "def", "")]
    [InlineData("abc", "abc", "abc")]
    [InlineData("xabcx", "yabcy", "abc")]
    [InlineData("ab", "ba", "a")]
    [InlineData("", "abc", "")]
    [InlineData("abc", "", "")]
    public void LongestCommonSubstring_returns_the_shared_run(string a, string b, string expected)
        => Assert.Equal(expected, a.LongestCommonSubstring(b));

    [Fact]
    public void LongestCommonSubstring_preserves_the_receivers_casing_and_prefers_the_earliest_run()
    {
        Assert.Equal("abc", "ABCabc".LongestCommonSubstring("xabcx"));
        Assert.Equal("ABC", "ABCabc".LongestCommonSubstring("xabcx", ignoreCase: true));
    }

    [Fact]
    public void LongestCommonSubstring_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.LongestCommonSubstring("a"));
        Assert.Throws<ArgumentNullException>(() => "a".LongestCommonSubstring(null!));
    }

    // ------------------------------------------------------------------ NGrams

    [Theory]
    [InlineData("abcd", 2, new[] { "ab", "bc", "cd" })]
    [InlineData("abcd", 1, new[] { "a", "b", "c", "d" })]
    [InlineData("abcd", 4, new[] { "abcd" })]
    [InlineData("abcd", 5, new string[0])]
    [InlineData("aaa", 2, new[] { "aa", "aa" })]
    [InlineData("", 2, new string[0])]
    public void NGrams_returns_every_run_of_the_given_size(string input, int size, string[] expected)
        => Assert.Equal(expected, input.NGrams(size));

    [Fact]
    public void NGrams_is_lazy_but_validates_eagerly()
    {
        Assert.Equal("ab", "abc".NGrams(2).First());

        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.NGrams(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => "abc".NGrams(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => "abc".NGrams(-1));
    }

    // ------------------------------------------------------------------ similarities

    [Theory]
    [InlineData("kitten", "sitting", 0.5714)]
    [InlineData("abc", "abc", 1)]
    [InlineData("abc", "", 0)]
    [InlineData("", "", 1)]
    public void LevenshteinSimilarity_scales_the_distance_to_a_score(string a, string b, double expected)
        => Assert.Equal(expected, a.LevenshteinSimilarity(b), Precision);

    [Theory]
    [InlineData("MARTHA", "MARHTA", 0.9444)]
    [InlineData("DWAYNE", "DUANE", 0.8222)]
    [InlineData("DIXON", "DICKSONX", 0.7667)]
    [InlineData("CRATE", "TRACE", 0.7333)]
    [InlineData("abc", "abc", 1)]
    [InlineData("abc", "xyz", 0)]
    [InlineData("abc", "", 0)]
    [InlineData("", "abc", 0)]
    [InlineData("", "", 1)]
    [InlineData("a", "a", 1)]
    [InlineData("ab", "ba", 0)]
    public void JaroSimilarity_matches_the_reference_values(string a, string b, double expected)
    {
        Assert.Equal(expected, a.JaroSimilarity(b), Precision);
        Assert.Equal(expected, b.JaroSimilarity(a), Precision);
    }

    [Theory]
    [InlineData("MARTHA", "MARHTA", 0.9611)]
    [InlineData("DWAYNE", "DUANE", 0.84)]
    [InlineData("DIXON", "DICKSONX", 0.8133)]
    [InlineData("CRATE", "TRACE", 0.7333)]
    [InlineData("abc", "abc", 1)]
    [InlineData("abcdefgh", "abcdxxxx", 0.8)]
    [InlineData("", "", 1)]
    [InlineData("abc", "xyz", 0)]
    public void JaroWinklerSimilarity_matches_the_reference_values(string a, string b, double expected)
        => Assert.Equal(expected, a.JaroWinklerSimilarity(b), Precision);

    [Fact]
    public void JaroWinklerSimilarity_prefix_scale_can_be_tuned()
    {
        var jaro = "MARTHA".JaroSimilarity("MARHTA");
        Assert.Equal(jaro, "MARTHA".JaroWinklerSimilarity("MARHTA", prefixScale: 0), Precision);
        Assert.True("MARTHA".JaroWinklerSimilarity("MARHTA", prefixScale: 0.25) > "MARTHA".JaroWinklerSimilarity("MARHTA"));
        Assert.Throws<ArgumentOutOfRangeException>(() => "a".JaroWinklerSimilarity("a", prefixScale: 0.3));
        Assert.Throws<ArgumentOutOfRangeException>(() => "a".JaroWinklerSimilarity("a", prefixScale: -0.1));
    }

    [Fact]
    public void Jaro_family_can_ignore_case()
    {
        Assert.Equal(1, "martha".JaroSimilarity("MARTHA", ignoreCase: true), Precision);
        Assert.Equal(1, "martha".JaroWinklerSimilarity("MARTHA", ignoreCase: true), Precision);
        Assert.Equal(0, "martha".JaroSimilarity("MARTHA"), Precision);
    }

    [Theory]
    [InlineData("ABCBDAB", "BDCABA", 0.6154)]
    [InlineData("abc", "abc", 1)]
    [InlineData("abc", "xyz", 0)]
    [InlineData("", "", 1)]
    public void LongestCommonSubsequenceSimilarity_scales_the_shared_length(string a, string b, double expected)
        => Assert.Equal(expected, a.LongestCommonSubsequenceSimilarity(b), Precision);

    [Theory]
    [InlineData("night", "nacht", 0.25)]
    [InlineData("abc", "abc", 1)]
    [InlineData("abc", "xyz", 0)]
    [InlineData("", "", 1)]
    [InlineData("a", "a", 1)]
    [InlineData("a", "b", 0)]
    [InlineData("ab", "", 0)]
    public void DiceSimilarity_compares_bigram_sets(string a, string b, double expected)
    {
        Assert.Equal(expected, a.DiceSimilarity(b), Precision);
        Assert.Equal(expected, b.DiceSimilarity(a), Precision);
    }

    [Theory]
    [InlineData("night", "nacht", 0.1429)]
    [InlineData("abc", "abc", 1)]
    [InlineData("abc", "xyz", 0)]
    [InlineData("", "", 1)]
    [InlineData("a", "b", 0)]
    public void JaccardSimilarity_compares_bigram_sets(string a, string b, double expected)
        => Assert.Equal(expected, a.JaccardSimilarity(b), Precision);

    [Fact]
    public void Gram_similarities_accept_other_sizes_and_case_folding()
    {
        Assert.Equal(1, "NIGHT".DiceSimilarity("night", ignoreCase: true), Precision);
        Assert.Equal(0, "NIGHT".DiceSimilarity("night"), Precision);
        Assert.Equal(1, "NIGHT".JaccardSimilarity("night", ignoreCase: true), Precision);
        Assert.Equal(0.6, "night".DiceSimilarity("nacht", size: 1), Precision);
    }

    [Fact]
    public void Similarities_validate_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.LevenshteinSimilarity("a"));
        Assert.Throws<ArgumentNullException>(() => "a".LevenshteinSimilarity(null!));
        Assert.Throws<ArgumentNullException>(() => s.JaroSimilarity("a"));
        Assert.Throws<ArgumentNullException>(() => "a".JaroSimilarity(null!));
        Assert.Throws<ArgumentNullException>(() => s.JaroWinklerSimilarity("a"));
        Assert.Throws<ArgumentNullException>(() => "a".JaroWinklerSimilarity(null!));
        Assert.Throws<ArgumentNullException>(() => s.LongestCommonSubsequenceSimilarity("a"));
        Assert.Throws<ArgumentNullException>(() => "a".LongestCommonSubsequenceSimilarity(null!));
        Assert.Throws<ArgumentNullException>(() => s.DiceSimilarity("a"));
        Assert.Throws<ArgumentNullException>(() => "a".DiceSimilarity(null!));
        Assert.Throws<ArgumentNullException>(() => s.JaccardSimilarity("a"));
        Assert.Throws<ArgumentNullException>(() => "a".JaccardSimilarity(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => "a".DiceSimilarity("a", size: 0));
    }

    // ------------------------------------------------------------------ ranking with metrics

    [Fact]
    public void ClosestTo_and_ClosestPairs_accept_a_distance_metric()
    {
        var words = new[] { "receive", "deceive" };

        Assert.Equal([("receive", 2)], words.ClosestTo("recieve"));
        Assert.Equal([("receive", 1)], words.ClosestTo("recieve", metric: StringDistance.DamerauLevenshtein));

        var pairs = new[] { "recieve", "receive", "xyz" };
        Assert.Equal([("recieve", "receive", 2)], pairs.ClosestPairs());
        Assert.Equal([("recieve", "receive", 1)], pairs.ClosestPairs(metric: StringDistance.DamerauLevenshtein));
    }

    [Fact]
    public void MostSimilarTo_ranks_by_descending_similarity()
    {
        var names = new[] { "DWAYNE", "MARHTA", "MARTHA", "DUANE" };

        var top = names.MostSimilarTo("MARTHA", 2);
        Assert.Equal(["MARTHA", "MARHTA"], top.Select(p => p.Value));
        Assert.Equal(1, top[0].Similarity, Precision);
        Assert.Equal(0.9611, top[1].Similarity, Precision);

        Assert.Equal("MARTHA", names.MostSimilarTo("martha", ignoreCase: true)[0].Value);
        Assert.Equal(["MARTHA", "MARHTA", "DWAYNE", "DUANE"], names.MostSimilarTo("MARTHA", 10, metric: StringSimilarity.Levenshtein).Select(p => p.Value));
    }

    [Fact]
    public void MostSimilarTo_keeps_source_order_for_ties()
    {
        var ranked = new[] { "abc", "abc", "xyz" }.MostSimilarTo("abc", 3);
        Assert.Equal(["abc", "abc", "xyz"], ranked.Select(p => p.Value));
        Assert.Equal([1, 1, 0], ranked.Select(p => p.Similarity));
    }

    [Theory]
    [InlineData(StringSimilarity.JaroWinkler)]
    [InlineData(StringSimilarity.Jaro)]
    [InlineData(StringSimilarity.Levenshtein)]
    [InlineData(StringSimilarity.LongestCommonSubsequence)]
    [InlineData(StringSimilarity.Dice)]
    [InlineData(StringSimilarity.Jaccard)]
    public void MostSimilarTo_supports_every_metric(StringSimilarity metric)
    {
        var ranked = new[] { "xyz", "night", "nacht" }.MostSimilarTo("night", 3, metric: metric);
        Assert.Equal("night", ranked[0].Value);
        Assert.Equal(1, ranked[0].Similarity, Precision);
        Assert.Equal("xyz", ranked[2].Value);
    }

    [Fact]
    public void MostSimilarTo_validates_arguments()
    {
        IEnumerable<string> candidates = null!;
        Assert.Throws<ArgumentNullException>(() => candidates.MostSimilarTo("a"));
        Assert.Throws<ArgumentNullException>(() => new[] { "a" }.MostSimilarTo(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { "a" }.MostSimilarTo("a", -1));
        Assert.Empty(new[] { "a" }.MostSimilarTo("a", 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { "a" }.MostSimilarTo("a", metric: (StringSimilarity)99));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { "a" }.ClosestTo("a", metric: (StringDistance)99));
    }

    // ------------------------------------------------------------------ ClusterBy

    [Fact]
    public void ClusterBy_distance_groups_near_duplicates_greedily()
    {
        var words = new[] { "color", "colour", "dollar", "collar", "xyz" };

        Assert.Equal(
            [["color", "colour"], ["dollar", "collar"], ["xyz"]],
            words.ClusterBy(maxDistance: 1));
        Assert.Equal(
            [["color", "colour", "collar"], ["dollar"], ["xyz"]],
            words.ClusterBy(maxDistance: 2));
        Assert.Equal(
            [["color"], ["colour"], ["dollar"], ["collar"], ["xyz"]],
            words.ClusterBy(maxDistance: 0));
    }

    [Fact]
    public void ClusterBy_distance_honours_metric_and_case()
    {
        var words = new[] { "recieve", "receive" };

        Assert.Equal([["recieve"], ["receive"]], words.ClusterBy(1));
        Assert.Equal([["recieve", "receive"]], words.ClusterBy(1, StringDistance.DamerauLevenshtein));
        Assert.Equal([["Color", "color"]], new[] { "Color", "color" }.ClusterBy(0, ignoreCase: true));
        Assert.Equal([["Color"], ["color"]], new[] { "Color", "color" }.ClusterBy(0));
    }

    [Fact]
    public void ClusterBy_similarity_groups_by_threshold()
    {
        var names = new[] { "MARTHA", "MARHTA", "DWAYNE", "DUANE" };

        Assert.Equal([["MARTHA", "MARHTA"], ["DWAYNE", "DUANE"]], names.ClusterBy(0.8));
        Assert.Equal([["MARTHA", "MARHTA"], ["DWAYNE"], ["DUANE"]], names.ClusterBy(0.9));
        Assert.Equal([["MARTHA", "MARHTA", "DWAYNE", "DUANE"]], names.ClusterBy(0.0));
        Assert.Equal([["martha", "MARTHA"]], new[] { "martha", "MARTHA" }.ClusterBy(1.0, StringSimilarity.Levenshtein, ignoreCase: true));
    }

    [Fact]
    public void ClusterBy_handles_empty_input_and_validates_arguments()
    {
        Assert.Empty(Array.Empty<string>().ClusterBy(1));
        Assert.Empty(Array.Empty<string>().ClusterBy(0.5));

        IEnumerable<string> candidates = null!;
        Assert.Throws<ArgumentNullException>(() => candidates.ClusterBy(1));
        Assert.Throws<ArgumentNullException>(() => candidates.ClusterBy(0.5));
        Assert.Throws<ArgumentNullException>(() => new[] { "a", null! }.ClusterBy(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { "a" }.ClusterBy(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { "a" }.ClusterBy(-0.1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { "a" }.ClusterBy(1.1));
    }
}
