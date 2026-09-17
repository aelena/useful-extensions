namespace Aelena.Extensions.Tests;

public class StringSearchingTests
{
    // ------------------------------------------------------------------ AllIndicesOf(string)

    [Theory]
    [InlineData("a-b-c-d", "-", new[] { 1, 3, 5 })]
    [InlineData("abcabcabc", "abc", new[] { 0, 3, 6 })]
    [InlineData("aaaa", "aa", new[] { 0, 2 })]
    [InlineData("abc", "z", new int[0])]
    [InlineData("abc", "abcd", new int[0])]
    [InlineData("", "a", new int[0])]
    public void AllIndicesOf_string_returns_non_overlapping_positions(string input, string value, int[] expected)
        => Assert.Equal(expected, input.AllIndicesOf(value));

    [Fact]
    public void AllIndicesOf_string_can_report_overlapping_matches()
    {
        Assert.Equal([0, 1, 2], "aaaa".AllIndicesOf("aa", overlapping: true));
        Assert.Equal([0, 2], "aaaa".AllIndicesOf("aa", overlapping: false));
    }

    [Fact]
    public void AllIndicesOf_string_honours_comparison()
    {
        Assert.Equal([0, 4], "The the".AllIndicesOf("the", StringComparison.OrdinalIgnoreCase));
        Assert.Equal([4], "The the".AllIndicesOf("the"));
    }

    [Fact]
    public void AllIndicesOf_string_is_lazy_but_validates_eagerly()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.AllIndicesOf("a"));
        Assert.Throws<ArgumentNullException>(() => "abc".AllIndicesOf((string)null!));
        Assert.Throws<ArgumentException>(() => "abc".AllIndicesOf(""));

        var enumerable = "a-a".AllIndicesOf("a");
        Assert.Equal(0, enumerable.First());
    }

    // ------------------------------------------------------------------ AllIndicesOf(char)

    [Theory]
    [InlineData("a-b-c-d", '-', new[] { 1, 3, 5 })]
    [InlineData("aaa", 'a', new[] { 0, 1, 2 })]
    [InlineData("abc", 'z', new int[0])]
    [InlineData("", 'a', new int[0])]
    public void AllIndicesOf_char_returns_every_position(string input, char value, int[] expected)
        => Assert.Equal(expected, input.AllIndicesOf(value));

    [Fact]
    public void AllIndicesOf_char_validates_receiver()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.AllIndicesOf('a'));
    }

    // ------------------------------------------------------------------ ContainsAny

    [Fact]
    public void ContainsAny_params_finds_any_candidate()
    {
        Assert.True("the quick brown fox".ContainsAny("cat", "fox"));
        Assert.False("the quick brown fox".ContainsAny("cat", "dog"));
        Assert.False("the quick brown fox".ContainsAny());
    }

    [Fact]
    public void ContainsAny_params_with_comparison_finds_any_candidate()
    {
        Assert.True("the quick brown fox".ContainsAny(StringComparison.OrdinalIgnoreCase, "CAT", "FOX"));
        Assert.False("the quick brown fox".ContainsAny(StringComparison.Ordinal, "CAT", "FOX"));
    }

    [Fact]
    public void ContainsAny_enumerable_finds_any_candidate()
    {
        var candidates = new List<string> { "cat", "fox" };
        Assert.True("the quick brown fox".ContainsAny(candidates));
        Assert.False("the quick brown dog".ContainsAny(candidates));
        Assert.True("the quick brown FOX".ContainsAny(candidates, StringComparison.OrdinalIgnoreCase));
        Assert.False("the quick brown fox".ContainsAny(Enumerable.Empty<string>()));
    }

    [Fact]
    public void ContainsAny_accepts_an_array_argument()
    {
        string[] candidates = ["cat", "fox"];
        Assert.True("the quick brown fox".ContainsAny(candidates));
    }

    [Fact]
    public void ContainsAny_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.ContainsAny("a"));
        Assert.Throws<ArgumentNullException>(() => s.ContainsAny(new List<string> { "a" }));
        Assert.Throws<ArgumentNullException>(() => "abc".ContainsAny((IEnumerable<string>)null!));
        Assert.Throws<ArgumentNullException>(() => "abc".ContainsAny(null!, "a"));
        Assert.Throws<ArgumentNullException>(() => "abc".ContainsAny(new List<string> { null! }));
    }

    // ------------------------------------------------------------------ FirstContained

    [Fact]
    public void FirstContained_params_returns_first_candidate_in_priority_order()
    {
        Assert.Equal("fox", "the quick brown fox".FirstContained("cat", "fox", "brown"));
        Assert.Null("the quick brown fox".FirstContained("cat", "dog"));
        Assert.Null("the quick brown fox".FirstContained());
    }

    [Fact]
    public void FirstContained_params_with_comparison_returns_first_candidate()
    {
        Assert.Equal("FOX", "the quick brown fox".FirstContained(StringComparison.OrdinalIgnoreCase, "CAT", "FOX"));
        Assert.Null("the quick brown fox".FirstContained(StringComparison.Ordinal, "CAT", "FOX"));
    }

    [Fact]
    public void FirstContained_enumerable_returns_first_candidate()
    {
        var candidates = new List<string> { "cat", "fox", "brown" };
        Assert.Equal("fox", "the quick brown fox".FirstContained(candidates));
        Assert.Null("the quick red dog".FirstContained(candidates));
        Assert.Equal("fox", "the quick brown FOX".FirstContained(candidates, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void FirstContained_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.FirstContained("a"));
        Assert.Throws<ArgumentNullException>(() => s.FirstContained(new List<string> { "a" }));
        Assert.Throws<ArgumentNullException>(() => "abc".FirstContained((IEnumerable<string>)null!));
    }

    // ------------------------------------------------------------------ SplitOutside

    [Theory]
    [InlineData("a,b,c", new[] { "a", "b", "c" })]
    [InlineData("a,\"b,c\",d", new[] { "a", "\"b,c\"", "d" })]
    [InlineData("\"a,b\",\"c,d\"", new[] { "\"a,b\"", "\"c,d\"" })]
    [InlineData("a,\"b,c", new[] { "a", "\"b,c" })]
    [InlineData("\"x\"y,z", new[] { "\"x\"y", "z" })]
    [InlineData("a,,b", new[] { "a", "", "b" })]
    [InlineData("a,b,", new[] { "a", "b", "" })]
    [InlineData(",a", new[] { "", "a" })]
    [InlineData("nosep", new[] { "nosep" })]
    [InlineData("", new[] { "" })]
    public void SplitOutside_char_keeps_quoted_separators_together(string input, string[] expected)
        => Assert.Equal(expected, input.SplitOutside(',', '"'));

    [Theory]
    [InlineData("a,,b", StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
    [InlineData("", StringSplitOptions.RemoveEmptyEntries, new string[0])]
    [InlineData(" a , b ", StringSplitOptions.TrimEntries, new[] { "a", "b" })]
    [InlineData(" a , , b ", StringSplitOptions.TrimEntries, new[] { "a", "", "b" })]
    [InlineData(" a , , b ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
    [InlineData(" a , , b ", StringSplitOptions.RemoveEmptyEntries, new[] { " a ", " ", " b " })]
    public void SplitOutside_honours_split_options(string input, StringSplitOptions options, string[] expected)
        => Assert.Equal(expected, input.SplitOutside(',', '"', options));

    [Fact]
    public void SplitOutside_string_overload_uses_distinct_open_and_close_markers()
    {
        Assert.Equal(["f(a, b)", "c", "g(d)"], "f(a, b), c, g(d)".SplitOutside(", ", ("(", ")")));
        Assert.Equal(["a", "b"], "a, b".SplitOutside(", ", ("(", ")"), StringSplitOptions.TrimEntries));
    }

    [Fact]
    public void SplitOutside_supports_several_enclosures_and_separators()
    {
        var pieces = "\"a,b\";[c;d],e".SplitOutside([",", ";"], [("\"", "\""), ("[", "]")]);

        Assert.Equal(["\"a,b\"", "[c;d]", "e"], pieces);
    }

    [Fact]
    public void SplitOutside_prefers_the_longest_separator_at_a_position()
        => Assert.Equal(["a", "b", "c"], "a||b|c".SplitOutside(["|", "||"], []));

    [Fact]
    public void SplitOutside_with_no_enclosures_behaves_like_split()
        => Assert.Equal("a,b;c".Split([',', ';']), "a,b;c".SplitOutside([",", ";"], []));

    [Fact]
    public void SplitOutside_is_lazy_but_validates_eagerly()
    {
        var pieces = "a,b".SplitOutside(',', '"');
        Assert.Equal("a", pieces.First());

        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.SplitOutside(',', '"'));
        Assert.Throws<ArgumentNullException>(() => "a".SplitOutside(null!, []));
        Assert.Throws<ArgumentNullException>(() => "a".SplitOutside([","], null!));
        Assert.Throws<ArgumentException>(() => "a".SplitOutside([], []));
        Assert.Throws<ArgumentException>(() => "a".SplitOutside([""], []));
        Assert.Throws<ArgumentNullException>(() => "a".SplitOutside([null!], []));
        Assert.Throws<ArgumentException>(() => "a".SplitOutside([","], [("", ")")]));
        Assert.Throws<ArgumentException>(() => "a".SplitOutside([","], [("(", "")]));
        Assert.Throws<ArgumentNullException>(() => "a".SplitOutside([","], [(null!, ")")]));
    }
}
