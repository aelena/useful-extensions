namespace Aelena.CommonExtensions.Tests;

public class ValueExtensionsTests
{
    // ------------------------------------------------------------------ In

    [Theory]
    [InlineData(2, new[] { 1, 2, 3 }, true)]
    [InlineData(1, new[] { 1, 2, 3 }, true)]
    [InlineData(3, new[] { 1, 2, 3 }, true)]
    [InlineData(4, new[] { 1, 2, 3 }, false)]
    [InlineData(4, new int[0], false)]
    public void In_params_matches_any_candidate(int value, int[] candidates, bool expected)
        => Assert.Equal(expected, value.In(candidates));

    [Fact]
    public void In_params_works_with_inline_arguments()
    {
        Assert.True(2.In(1, 2, 3));
        Assert.False(4.In(1, 2, 3));
        Assert.False(4.In());
        Assert.True("b".In("a", "b"));
    }

    [Fact]
    public void In_params_handles_nulls_like_the_default_comparer()
    {
        string? nothing = null;
        Assert.True(nothing.In("a", null));
        Assert.False(nothing.In("a", "b"));
        Assert.False("a".In(null, "b"));
    }

    [Fact]
    public void In_enumerable_matches_any_candidate()
    {
        var candidates = new List<string> { "a", "b" };
        Assert.True("b".In(candidates));
        Assert.False("c".In(candidates));
        Assert.False("c".In(Enumerable.Empty<string>()));
        Assert.True(5.In(Enumerable.Range(0, 10)));
    }

    [Fact]
    public void In_enumerable_validates_argument()
        => Assert.Throws<ArgumentNullException>(() => "a".In((IEnumerable<string>)null!));

    [Fact]
    public void In_uses_default_equality_for_custom_types()
    {
        var point = new Point(1, 2);
        Assert.True(point.In(new Point(0, 0), new Point(1, 2)));
        Assert.False(point.In(new Point(0, 0), new Point(2, 1)));
    }

    private readonly record struct Point(int X, int Y);

    // ------------------------------------------------------------------ IsBetween

    [Theory]
    [InlineData(5, 1, 10, true)]
    [InlineData(1, 1, 10, true)]
    [InlineData(10, 1, 10, true)]
    [InlineData(0, 1, 10, false)]
    [InlineData(11, 1, 10, false)]
    [InlineData(5, 5, 5, true)]
    [InlineData(5, 10, 1, false)]
    public void IsBetween_inclusive_by_default(int value, int lower, int upper, bool expected)
        => Assert.Equal(expected, value.IsBetween(lower, upper));

    [Theory]
    [InlineData(5, 1, 10, true)]
    [InlineData(1, 1, 10, false)]
    [InlineData(10, 1, 10, false)]
    [InlineData(0, 1, 10, false)]
    [InlineData(11, 1, 10, false)]
    [InlineData(5, 5, 5, false)]
    public void IsBetween_exclusive_when_requested(int value, int lower, int upper, bool expected)
        => Assert.Equal(expected, value.IsBetween(lower, upper, inclusive: false));

    [Fact]
    public void IsBetween_works_for_any_comparable_type()
    {
        Assert.True(2.5.IsBetween(2.0, 3.0));
        Assert.True(1.0m.IsBetween(1.0m, 1.0m));
        Assert.False(1.0m.IsBetween(1.0m, 1.0m, inclusive: false));

        var today = new DateOnly(2026, 9, 17);
        Assert.True(today.IsBetween(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31)));
        Assert.False(today.IsBetween(new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31)));

        Assert.True("m".IsBetween("a", "z"));
        Assert.False("aardvark".IsBetween("b", "c"));
        Assert.True('m'.IsBetween('a', 'z'));
    }
}
