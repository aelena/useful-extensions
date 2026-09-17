namespace Aelena.CommonExtensions.Tests;

public class EnumerableExtensionsTests
{
    private static IEnumerable<int> Naturals()
    {
        for (var i = 0; ; i++)
        {
            yield return i;
        }
    }

    // ------------------------------------------------------------------ FindIndex

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4 }, 3, 2)]
    [InlineData(new[] { 1, 2, 3, 4 }, 1, 0)]
    [InlineData(new[] { 1, 2, 3, 4 }, 4, 3)]
    [InlineData(new[] { 1, 2, 3, 4 }, 9, -1)]
    [InlineData(new[] { 3, 3 }, 3, 0)]
    [InlineData(new int[0], 1, -1)]
    public void FindIndex_returns_position_of_first_match(int[] source, int target, int expected)
        => Assert.Equal(expected, source.FindIndex(x => x == target));

    [Fact]
    public void FindIndex_works_on_lazy_and_infinite_sequences()
        => Assert.Equal(42, Naturals().FindIndex(x => x == 42));

    [Theory]
    [InlineData(new[] { 2, 1, 2, 1, 2 }, 0, 0)]
    [InlineData(new[] { 2, 1, 2, 1, 2 }, 1, 2)]
    [InlineData(new[] { 2, 1, 2, 1, 2 }, 3, 4)]
    [InlineData(new[] { 2, 1, 2, 1, 2 }, 5, -1)]
    [InlineData(new[] { 2, 1, 2, 1, 2 }, 50, -1)]
    [InlineData(new[] { 2, 1, 1 }, 1, -1)]
    public void FindIndex_with_start_returns_absolute_position(int[] source, int startIndex, int expected)
        => Assert.Equal(expected, source.FindIndex(startIndex, x => x == 2));

    [Fact]
    public void FindIndex_validates_arguments()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.FindIndex(x => true));
        Assert.Throws<ArgumentNullException>(() => source.FindIndex(0, x => true));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.FindIndex(null!));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.FindIndex(0, null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.FindIndex(-1, x => true));
    }

    // ------------------------------------------------------------------ FindLastIndex

    [Theory]
    [InlineData(new[] { 2, 1, 2, 1 }, 2, 2)]
    [InlineData(new[] { 2, 1, 2, 1 }, 1, 3)]
    [InlineData(new[] { 2 }, 2, 0)]
    [InlineData(new[] { 2, 1 }, 9, -1)]
    [InlineData(new int[0], 1, -1)]
    public void FindLastIndex_returns_position_of_last_match(int[] source, int target, int expected)
        => Assert.Equal(expected, source.FindLastIndex(x => x == target));

    [Fact]
    public void FindLastIndex_validates_arguments()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.FindLastIndex(x => true));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.FindLastIndex(null!));
    }

    // ------------------------------------------------------------------ FindIndices

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6 }, new[] { 1, 3, 5 })]
    [InlineData(new[] { 1, 3, 5 }, new int[0])]
    [InlineData(new[] { 2, 4 }, new[] { 0, 1 })]
    [InlineData(new int[0], new int[0])]
    public void FindIndices_returns_every_matching_position(int[] source, int[] expected)
        => Assert.Equal(expected, source.FindIndices(x => x % 2 == 0));

    [Fact]
    public void FindIndices_is_lazy_but_validates_eagerly()
    {
        Assert.Equal([0, 2, 4], Naturals().FindIndices(x => x % 2 == 0).Take(3));

        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.FindIndices(x => true));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.FindIndices(null!));
    }

    // ------------------------------------------------------------------ TakeUntil

    [Fact]
    public void TakeUntil_includes_the_matching_element_by_default()
        => Assert.Equal([0, 1, 2, 3], Naturals().TakeUntil(x => x == 3));

    [Fact]
    public void TakeUntil_can_exclude_the_matching_element()
        => Assert.Equal([0, 1, 2], Naturals().TakeUntil(x => x == 3, inclusive: false));

    [Fact]
    public void TakeUntil_returns_everything_when_nothing_matches()
        => Assert.Equal([1, 2, 3], new[] { 1, 2, 3 }.TakeUntil(x => x > 10));

    [Fact]
    public void TakeUntil_stops_at_the_first_match_only()
        => Assert.Equal(["a", "STOP"], new[] { "a", "STOP", "b", "STOP" }.TakeUntil(x => x == "STOP"));

    [Fact]
    public void TakeUntil_on_empty_sequence_is_empty()
        => Assert.Empty(Array.Empty<int>().TakeUntil(x => true));

    [Fact]
    public void TakeUntil_validates_arguments()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.TakeUntil(x => true));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.TakeUntil(null!));
    }

    // ------------------------------------------------------------------ IsNullOrEmpty / HasItems

    [Fact]
    public void IsNullOrEmpty_is_true_for_null_and_empty()
    {
        IEnumerable<int>? nothing = null;
        Assert.True(nothing.IsNullOrEmpty());
        Assert.True(Array.Empty<int>().IsNullOrEmpty());
        Assert.True(Enumerable.Empty<int>().IsNullOrEmpty());
        Assert.True(new List<string>().IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_is_false_for_sequences_with_items()
    {
        Assert.False(new[] { 1 }.IsNullOrEmpty());
        Assert.False(Naturals().IsNullOrEmpty());
    }

    [Fact]
    public void HasItems_is_false_for_null_and_empty()
    {
        IEnumerable<int>? nothing = null;
        Assert.False(nothing.HasItems());
        Assert.False(Array.Empty<int>().HasItems());
        Assert.False(Enumerable.Empty<int>().HasItems());
    }

    [Fact]
    public void HasItems_is_true_for_sequences_with_items()
    {
        Assert.True(new[] { 1 }.HasItems());
        Assert.True(Naturals().HasItems());
    }

    [Fact]
    public void HasItems_narrows_nullability_for_the_compiler()
    {
        List<string>? maybe = Random.Shared.Next() >= 0 ? ["x"] : null;

        if (maybe.HasItems())
        {
            Assert.Single(maybe);
        }

        if (!maybe.IsNullOrEmpty())
        {
            Assert.Equal("x", maybe[0]);
        }
    }
}
