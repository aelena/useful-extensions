namespace Aelena.CommonExtensions.Tests;

public class StringDistanceTests
{
    // ------------------------------------------------------------------ LevenshteinDistance

    [Theory]
    [InlineData("kitten", "sitting", 3)]
    [InlineData("flaw", "lawn", 2)]
    [InlineData("Saturday", "Sunday", 3)]
    [InlineData("intention", "execution", 5)]
    [InlineData("book", "back", 2)]
    [InlineData("abc", "abc", 0)]
    [InlineData("abc", "abd", 1)]
    [InlineData("abc", "ab", 1)]
    [InlineData("ab", "abc", 1)]
    [InlineData("a", "b", 1)]
    [InlineData("", "", 0)]
    [InlineData("", "abc", 3)]
    [InlineData("abc", "", 3)]
    [InlineData("café", "cafe", 1)]
    [InlineData("Kitten", "kitten", 1)]
    public void LevenshteinDistance_counts_edits(string a, string b, int expected)
        => Assert.Equal(expected, a.LevenshteinDistance(b));

    [Theory]
    [InlineData("kitten", "sitting")]
    [InlineData("", "abc")]
    [InlineData("Saturday", "Sunday")]
    [InlineData("intention", "execution")]
    public void LevenshteinDistance_is_symmetric(string a, string b)
        => Assert.Equal(a.LevenshteinDistance(b), b.LevenshteinDistance(a));

    [Fact]
    public void LevenshteinDistance_can_ignore_case()
    {
        Assert.Equal(0, "Kitten".LevenshteinDistance("kITTEN", ignoreCase: true));
        Assert.Equal(1, "ABC".LevenshteinDistance("abd", ignoreCase: true));
        Assert.Equal(3, "ABC".LevenshteinDistance("abd", ignoreCase: false));
    }

    [Fact]
    public void LevenshteinDistance_handles_strings_longer_than_the_stack_buffer()
    {
        var a = new string('a', 300);
        var b = new string('a', 300) + "b";
        var c = new string('b', 300);

        Assert.Equal(1, a.LevenshteinDistance(b));
        Assert.Equal(1, b.LevenshteinDistance(a));
        Assert.Equal(300, a.LevenshteinDistance(c));
        Assert.Equal(300, a.LevenshteinDistance(c.ToUpperInvariant(), ignoreCase: true));
    }

    [Fact]
    public void LevenshteinDistance_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.LevenshteinDistance("a"));
        Assert.Throws<ArgumentNullException>(() => "a".LevenshteinDistance(null!));
    }

    // ------------------------------------------------------------------ ClosestTo

    private static readonly string[] Words = ["color", "cooler", "dollar", "collar", "colour"];

    [Fact]
    public void ClosestTo_returns_the_single_best_match_by_default()
    {
        Assert.Equal([("color", 0)], Words.ClosestTo("color"));
        Assert.Equal([("colour", 1)], Words.Skip(1).ClosestTo("color"));
    }

    [Fact]
    public void ClosestTo_orders_by_distance_and_keeps_source_order_for_ties()
        => Assert.Equal([("color", 0), ("colour", 1), ("cooler", 2), ("collar", 2), ("dollar", 3)], Words.ClosestTo("color", 10));

    [Fact]
    public void ClosestTo_includes_exact_matches_at_distance_zero()
        => Assert.Equal([("color", 0), ("colour", 1)], Words.ClosestTo("color", 2).Where(p => p.Distance < 2));

    [Fact]
    public void ClosestTo_can_ignore_case()
    {
        Assert.Equal([("color", 0), ("colour", 1)], Words.ClosestTo("COLOR", 2, ignoreCase: true));
        Assert.Equal(5, Words.ClosestTo("COLOR")[0].Distance);
    }

    [Fact]
    public void ClosestTo_with_zero_count_or_no_candidates_is_empty()
    {
        Assert.Empty(Words.ClosestTo("color", 0));
        Assert.Empty(Array.Empty<string>().ClosestTo("color"));
    }

    [Fact]
    public void ClosestTo_validates_arguments()
    {
        IEnumerable<string> candidates = null!;
        Assert.Throws<ArgumentNullException>(() => candidates.ClosestTo("a"));
        Assert.Throws<ArgumentNullException>(() => Words.ClosestTo(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => Words.ClosestTo("a", -1));
        Assert.Throws<ArgumentNullException>(() => new string[] { null! }.ClosestTo("a"));
    }

    // ------------------------------------------------------------------ ClosestPairs

    [Fact]
    public void ClosestPairs_returns_the_single_best_pair_by_default()
        => Assert.Equal([("color", "colour", 1)], new[] { "color", "colour", "dollar", "collar" }.ClosestPairs());

    [Fact]
    public void ClosestPairs_orders_by_distance_and_keeps_source_order_for_ties()
    {
        var pairs = new[] { "color", "colour", "dollar", "collar" }.ClosestPairs(10);

        Assert.Equal(6, pairs.Count);
        Assert.Equal(
            [("color", "colour", 1), ("dollar", "collar", 1), ("color", "collar", 2), ("colour", "collar", 2)],
            pairs.Take(4));
        Assert.Equal([3, 3], pairs.Skip(4).Select(p => p.Distance));
    }

    [Fact]
    public void ClosestPairs_can_ignore_case()
    {
        Assert.Equal([("Color", "COLOR", 0)], new[] { "Color", "COLOR", "dollar" }.ClosestPairs(ignoreCase: true));
        Assert.Equal(4, new[] { "Color", "COLOR" }.ClosestPairs()[0].Distance);
    }

    [Fact]
    public void ClosestPairs_needs_at_least_two_candidates()
    {
        Assert.Empty(Array.Empty<string>().ClosestPairs());
        Assert.Empty(new[] { "alone" }.ClosestPairs());
        Assert.Empty(new[] { "a", "b" }.ClosestPairs(0));
    }

    [Fact]
    public void ClosestPairs_validates_arguments()
    {
        IEnumerable<string> candidates = null!;
        Assert.Throws<ArgumentNullException>(() => candidates.ClosestPairs());
        Assert.Throws<ArgumentOutOfRangeException>(() => Words.ClosestPairs(-1));
        Assert.Throws<ArgumentNullException>(() => new[] { "a", null! }.ClosestPairs());
    }
}
