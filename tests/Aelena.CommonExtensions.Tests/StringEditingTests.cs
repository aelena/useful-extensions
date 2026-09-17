using System.Text;

namespace Aelena.CommonExtensions.Tests;

public class StringEditingTests
{
    // ------------------------------------------------------------------ ReplaceFirst

    [Theory]
    [InlineData("a-b-c", "-", "+", "a+b-c")]
    [InlineData("aaa", "a", "b", "baa")]
    [InlineData("aaa", "a", "", "aa")]
    [InlineData("abc", "abc", "x", "x")]
    [InlineData("abc", "z", "x", "abc")]
    [InlineData("", "z", "x", "")]
    [InlineData("hello world", "world", "there, world", "hello there, world")]
    public void ReplaceFirst_replaces_only_the_first_occurrence(string input, string oldValue, string newValue, string expected)
        => Assert.Equal(expected, input.ReplaceFirst(oldValue, newValue));

    [Fact]
    public void ReplaceFirst_honours_comparison()
        => Assert.Equal("x-B-b", "b-B-b".ReplaceFirst("B", "x", StringComparison.OrdinalIgnoreCase));

    [Fact]
    public void ReplaceFirst_returns_same_instance_when_nothing_matches()
    {
        const string input = "abc";
        Assert.Same(input, input.ReplaceFirst("z", "x"));
    }

    // ------------------------------------------------------------------ ReplaceLast

    [Theory]
    [InlineData("a-b-c", "-", "+", "a-b+c")]
    [InlineData("aaa", "a", "b", "aab")]
    [InlineData("aaa", "a", "", "aa")]
    [InlineData("abc", "abc", "x", "x")]
    [InlineData("abc", "z", "x", "abc")]
    [InlineData("", "z", "x", "")]
    [InlineData("one, two, three", ", ", " and ", "one, two and three")]
    public void ReplaceLast_replaces_only_the_last_occurrence(string input, string oldValue, string newValue, string expected)
        => Assert.Equal(expected, input.ReplaceLast(oldValue, newValue));

    [Fact]
    public void ReplaceLast_honours_comparison()
        => Assert.Equal("b-B-x", "b-B-b".ReplaceLast("B", "x", StringComparison.OrdinalIgnoreCase));

    [Fact]
    public void ReplaceLast_returns_same_instance_when_nothing_matches()
    {
        const string input = "abc";
        Assert.Same(input, input.ReplaceLast("z", "x"));
    }

    [Fact]
    public void Replace_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.ReplaceFirst("a", "b"));
        Assert.Throws<ArgumentNullException>(() => s.ReplaceLast("a", "b"));

        Assert.Throws<ArgumentNullException>(() => "abc".ReplaceFirst(null!, "b"));
        Assert.Throws<ArgumentNullException>(() => "abc".ReplaceLast(null!, "b"));
        Assert.Throws<ArgumentException>(() => "abc".ReplaceFirst("", "b"));
        Assert.Throws<ArgumentException>(() => "abc".ReplaceLast("", "b"));

        Assert.Throws<ArgumentNullException>(() => "abc".ReplaceFirst("a", null!));
        Assert.Throws<ArgumentNullException>(() => "abc".ReplaceLast("a", null!));
    }

    // ------------------------------------------------------------------ RemoveAll

    [Fact]
    public void RemoveAll_params_removes_every_value()
        => Assert.Equal("123", "(1) [2] {3}".RemoveAll("(", ")", "[", "]", "{", "}", " "));

    [Fact]
    public void RemoveAll_params_with_comparison_removes_every_value()
        => Assert.Equal(" ", "Ab aB".RemoveAll(StringComparison.OrdinalIgnoreCase, "ab"));

    [Fact]
    public void RemoveAll_enumerable_removes_every_value()
    {
        var values = new List<string> { "<", ">" };
        Assert.Equal("b", "<b>".RemoveAll(values));
        Assert.Equal("", "<B>".RemoveAll(["b", "<", ">"], StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RemoveAll_returns_same_instance_when_nothing_matches()
    {
        const string input = "abc";
        Assert.Same(input, input.RemoveAll("z"));
        Assert.Same(input, input.RemoveAll(StringComparison.OrdinalIgnoreCase, "Z"));
        Assert.Same(input, input.RemoveAll(new[] { "z" }.AsEnumerable(), StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RemoveAll_with_comparison_removes_every_occurrence_including_at_the_ends()
    {
        Assert.Equal("--", "xAB-ab-AB".RemoveAll(StringComparison.OrdinalIgnoreCase, "ab", "X"));
        Assert.Equal("B", "aBa".RemoveAll(StringComparison.OrdinalIgnoreCase, "A"));
    }

    [Fact]
    public void RemoveAll_with_no_values_returns_same_instance()
    {
        const string input = "abc";
        Assert.Same(input, input.RemoveAll());
        Assert.Same(input, input.RemoveAll(Array.Empty<string>().AsEnumerable()));
    }

    [Fact]
    public void RemoveAll_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.RemoveAll("a"));
        Assert.Throws<ArgumentNullException>(() => s.RemoveAll(new List<string> { "a" }));
        Assert.Throws<ArgumentNullException>(() => "abc".RemoveAll((IEnumerable<string>)null!));
        Assert.Throws<ArgumentException>(() => "abc".RemoveAll(""));
        Assert.Throws<ArgumentException>(() => "abc".RemoveAll(new List<string> { "" }));
        Assert.Throws<ArgumentException>(() => "abc".RemoveAll(StringComparison.OrdinalIgnoreCase, ""));
        Assert.Throws<ArgumentNullException>(() => "abc".RemoveAll(new List<string> { null! }));
        Assert.Throws<ArgumentNullException>(() => "abc".RemoveAll(StringComparison.OrdinalIgnoreCase, [null!]));
    }

    // ------------------------------------------------------------------ Truncate

    [Theory]
    [InlineData("hello world", 5, "", "hello")]
    [InlineData("hello world", 11, "", "hello world")]
    [InlineData("hello world", 50, "", "hello world")]
    [InlineData("hello world", 0, "", "")]
    [InlineData("", 0, "", "")]
    [InlineData("hello world", 8, "...", "hello...")]
    [InlineData("hello world", 8, "…", "hello w…")]
    [InlineData("hello world", 3, "...", "...")]
    [InlineData("hello world", 2, "...", "..")]
    [InlineData("hello world", 0, "...", "")]
    [InlineData("hi", 5, "...", "hi")]
    public void Truncate_limits_length_and_appends_suffix(string input, int maxLength, string suffix, string expected)
        => Assert.Equal(expected, input.Truncate(maxLength, suffix));

    [Fact]
    public void Truncate_returns_same_instance_when_it_already_fits()
    {
        const string input = "short";
        Assert.Same(input, input.Truncate(10));
        Assert.Same(input, input.Truncate(5, "..."));
    }

    [Fact]
    public void Truncate_validates_arguments()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.Truncate(3));
        Assert.Throws<ArgumentNullException>(() => "abc".Truncate(3, null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => "abc".Truncate(-1));
    }

    // ------------------------------------------------------------------ SafeSubstring

    [Theory]
    [InlineData("hello world", 0, 5, "hello")]
    [InlineData("hello world", 6, 5, "world")]
    [InlineData("hello world", 6, 50, "world")]
    [InlineData("hello world", 11, 5, "")]
    [InlineData("hello world", 50, 5, "")]
    [InlineData("hello world", -3, 5, "he")]
    [InlineData("hello world", -50, 5, "")]
    [InlineData("hello world", 0, 0, "")]
    [InlineData("hello world", 0, -1, "")]
    [InlineData("hello world", 3, int.MaxValue, "lo world")]
    [InlineData("", 0, 5, "")]
    public void SafeSubstring_with_length_clips_to_the_string(string input, int start, int length, string expected)
        => Assert.Equal(expected, input.SafeSubstring(start, length));

    [Theory]
    [InlineData("hello world", 6, "world")]
    [InlineData("hello world", 0, "hello world")]
    [InlineData("hello world", 11, "")]
    [InlineData("hello world", 50, "")]
    [InlineData("hello world", -3, "hello world")]
    [InlineData("", 0, "")]
    public void SafeSubstring_clips_start_to_the_string(string input, int start, string expected)
        => Assert.Equal(expected, input.SafeSubstring(start));

    [Fact]
    public void SafeSubstring_validates_receiver()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(() => s.SafeSubstring(0));
        Assert.Throws<ArgumentNullException>(() => s.SafeSubstring(0, 1));
    }

    // ------------------------------------------------------------------ RemoveDiacritics

    [Theory]
    [InlineData("crème brûlée", "creme brulee")]
    [InlineData("Ångström", "Angstrom")]
    [InlineData("São Paulo", "Sao Paulo")]
    [InlineData("naïve façade", "naive facade")]
    [InlineData("Ελληνικά", "Ελληνικα")]
    [InlineData("plain ascii", "plain ascii")]
    [InlineData("łódź", "łodz")]
    [InlineData("", "")]
    public void RemoveDiacritics_strips_combining_marks(string input, string expected)
        => Assert.Equal(expected, input.RemoveDiacritics());

    [Fact]
    public void RemoveDiacritics_handles_decomposed_input()
    {
        var decomposed = "é";
        Assert.Equal("e", decomposed.RemoveDiacritics());
    }

    [Fact]
    public void RemoveDiacritics_returns_same_instance_when_nothing_changes()
    {
        const string input = "nothing to strip";
        Assert.Same(input, input.RemoveDiacritics());
    }

    [Fact]
    public void RemoveDiacritics_handles_strings_longer_than_the_stack_buffer()
    {
        var input = string.Concat(Enumerable.Repeat("é", 300));
        var expected = new string('e', 300);

        Assert.Equal(expected, input.RemoveDiacritics());
    }

    [Fact]
    public void RemoveDiacritics_output_is_in_normalization_form_c()
        => Assert.True("crème".RemoveDiacritics().IsNormalized(NormalizationForm.FormC));

    [Fact]
    public void RemoveDiacritics_validates_receiver()
    {
        string s = null!;
        Assert.Throws<ArgumentNullException>(s.RemoveDiacritics);
    }
}
