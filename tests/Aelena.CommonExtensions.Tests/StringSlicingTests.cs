namespace Aelena.CommonExtensions.Tests;

public class StringSlicingTests
{
    private const string Sentence = "the quick brown fox jumps over the lazy dog";

    // ------------------------------------------------------------------ After

    [Theory]
    [InlineData(Sentence, "fox", " jumps over the lazy dog")]
    [InlineData(Sentence, "the", " quick brown fox jumps over the lazy dog")]
    [InlineData(Sentence, "dog", "")]
    [InlineData(Sentence, "cat", Sentence)]
    [InlineData("key=value", "=", "value")]
    [InlineData("a=b=c", "=", "b=c")]
    [InlineData("", "=", "")]
    public void After_returns_text_following_first_marker(string input, string marker, string expected)
        => Assert.Equal(expected, input.After(marker));

    [Fact]
    public void After_honours_comparison()
    {
        Assert.Equal(" quick brown fox jumps over the lazy dog", Sentence.After("THE", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(Sentence, Sentence.After("THE"));
    }

    [Fact]
    public void After_returns_same_instance_when_marker_missing()
        => Assert.Same(Sentence, Sentence.After("cat"));

    [Theory]
    [InlineData("key=value", '=', "value")]
    [InlineData("a=b=c", '=', "b=c")]
    [InlineData("abc", 'z', "abc")]
    [InlineData("abc", 'c', "")]
    [InlineData("", 'c', "")]
    public void After_char_returns_text_following_first_marker(string input, char marker, string expected)
        => Assert.Equal(expected, input.After(marker));

    // ------------------------------------------------------------------ AfterLast

    [Theory]
    [InlineData(Sentence, "the", " lazy dog")]
    [InlineData("a=b=c", "=", "c")]
    [InlineData("/usr/local/bin", "/", "bin")]
    [InlineData("noseparator", "/", "noseparator")]
    [InlineData("trailing/", "/", "")]
    [InlineData("", "/", "")]
    public void AfterLast_returns_text_following_last_marker(string input, string marker, string expected)
        => Assert.Equal(expected, input.AfterLast(marker));

    [Fact]
    public void AfterLast_honours_comparison()
        => Assert.Equal(" lazy dog", Sentence.AfterLast("THE", StringComparison.OrdinalIgnoreCase));

    [Theory]
    [InlineData("a=b=c", '=', "c")]
    [InlineData("file.tar.gz", '.', "gz")]
    [InlineData("noext", '.', "noext")]
    [InlineData("trailing.", '.', "")]
    public void AfterLast_char_returns_text_following_last_marker(string input, char marker, string expected)
        => Assert.Equal(expected, input.AfterLast(marker));

    // ------------------------------------------------------------------ Before

    [Theory]
    [InlineData(Sentence, " fox", "the quick brown")]
    [InlineData("key=value", "=", "key")]
    [InlineData("a=b=c", "=", "a")]
    [InlineData("=leading", "=", "")]
    [InlineData("nomarker", "=", "nomarker")]
    [InlineData("", "=", "")]
    public void Before_returns_text_preceding_first_marker(string input, string marker, string expected)
        => Assert.Equal(expected, input.Before(marker));

    [Fact]
    public void Before_honours_comparison()
        => Assert.Equal("the quick brown ", Sentence.Before("FOX", StringComparison.OrdinalIgnoreCase));

    [Theory]
    [InlineData("key=value", '=', "key")]
    [InlineData("a=b=c", '=', "a")]
    [InlineData("=leading", '=', "")]
    [InlineData("nomarker", '=', "nomarker")]
    public void Before_char_returns_text_preceding_first_marker(string input, char marker, string expected)
        => Assert.Equal(expected, input.Before(marker));

    // ------------------------------------------------------------------ BeforeLast

    [Theory]
    [InlineData("a=b=c", "=", "a=b")]
    [InlineData("/usr/local/bin", "/", "/usr/local")]
    [InlineData("nomarker", "/", "nomarker")]
    [InlineData("/leading", "/", "")]
    [InlineData("", "/", "")]
    public void BeforeLast_returns_text_preceding_last_marker(string input, string marker, string expected)
        => Assert.Equal(expected, input.BeforeLast(marker));

    [Fact]
    public void BeforeLast_honours_comparison()
        => Assert.Equal("the quick brown fox jumps over ", Sentence.BeforeLast("THE", StringComparison.OrdinalIgnoreCase));

    [Theory]
    [InlineData("a=b=c", '=', "a=b")]
    [InlineData("file.tar.gz", '.', "file.tar")]
    [InlineData("noext", '.', "noext")]
    [InlineData(".leading", '.', "")]
    public void BeforeLast_char_returns_text_preceding_last_marker(string input, char marker, string expected)
        => Assert.Equal(expected, input.BeforeLast(marker));

    // ------------------------------------------------------------------ Between

    [Theory]
    [InlineData("<b>bold</b>", "<b>", "</b>", "bold")]
    [InlineData("say [hello] and [bye]", "[", "]", "hello")]
    [InlineData("a 'quoted' word", "'", "'", "quoted")]
    [InlineData("no markers here", "[", "]", "")]
    [InlineData("only [start", "[", "]", "")]
    [InlineData("only end]", "[", "]", "")]
    [InlineData("] before [", "[", "]", "")]
    [InlineData("[]", "[", "]", "")]
    [InlineData("", "[", "]", "")]
    public void Between_returns_text_between_first_pair(string input, string start, string end, string expected)
        => Assert.Equal(expected, input.Between(start, end));

    [Fact]
    public void Between_honours_comparison()
        => Assert.Equal("bold", "<B>bold</B>".Between("<b>", "</b>", StringComparison.OrdinalIgnoreCase));

    [Fact]
    public void Between_composes_with_After_to_search_past_a_marker()
        => Assert.Equal("2", "a=[1] b=[2]".After("b=").Between("[", "]"));

    // ------------------------------------------------------------------ AllBetween

    [Fact]
    public void AllBetween_returns_every_enclosed_piece_in_order()
        => Assert.Equal(["1", "2", "3"], "[1] x [2] y [3]".AllBetween("[", "]"));

    [Fact]
    public void AllBetween_pieces_are_non_overlapping()
        => Assert.Equal(["a", "b"], "'a' 'b' '".AllBetween("'", "'"));

    [Theory]
    [InlineData("no markers")]
    [InlineData("[unterminated")]
    [InlineData("]closed before[ open")]
    [InlineData("")]
    public void AllBetween_yields_nothing_without_a_complete_pair(string input)
        => Assert.Empty(input.AllBetween("[", "]"));

    [Fact]
    public void AllBetween_honours_comparison()
        => Assert.Equal(["x", "y"], "<b>x</b><B>y</B>".AllBetween("<b>", "</b>", StringComparison.OrdinalIgnoreCase));

    [Fact]
    public void AllBetween_is_lazy_but_validates_eagerly()
    {
        var enumerable = "[1] [2]".AllBetween("[", "]");
        Assert.Equal("1", enumerable.First());

        Assert.Throws<ArgumentException>(() => "[1]".AllBetween("", "]"));
    }

    // ------------------------------------------------------------------ argument validation

    [Fact]
    public void Null_receiver_throws()
    {
        string s = null!;

        Assert.Throws<ArgumentNullException>(() => s.After("x"));
        Assert.Throws<ArgumentNullException>(() => s.After('x'));
        Assert.Throws<ArgumentNullException>(() => s.AfterLast("x"));
        Assert.Throws<ArgumentNullException>(() => s.AfterLast('x'));
        Assert.Throws<ArgumentNullException>(() => s.Before("x"));
        Assert.Throws<ArgumentNullException>(() => s.Before('x'));
        Assert.Throws<ArgumentNullException>(() => s.BeforeLast("x"));
        Assert.Throws<ArgumentNullException>(() => s.BeforeLast('x'));
        Assert.Throws<ArgumentNullException>(() => s.Between("x", "y"));
        Assert.Throws<ArgumentNullException>(() => s.AllBetween("x", "y"));
    }

    [Fact]
    public void Null_marker_throws()
    {
        string marker = null!;

        Assert.Throws<ArgumentNullException>(() => Sentence.After(marker));
        Assert.Throws<ArgumentNullException>(() => Sentence.AfterLast(marker));
        Assert.Throws<ArgumentNullException>(() => Sentence.Before(marker));
        Assert.Throws<ArgumentNullException>(() => Sentence.BeforeLast(marker));
        Assert.Throws<ArgumentNullException>(() => Sentence.Between(marker, "y"));
        Assert.Throws<ArgumentNullException>(() => Sentence.Between("x", marker));
        Assert.Throws<ArgumentNullException>(() => Sentence.AllBetween(marker, "y"));
        Assert.Throws<ArgumentNullException>(() => Sentence.AllBetween("x", marker));
    }

    [Fact]
    public void Empty_marker_throws()
    {
        Assert.Throws<ArgumentException>(() => Sentence.After(""));
        Assert.Throws<ArgumentException>(() => Sentence.AfterLast(""));
        Assert.Throws<ArgumentException>(() => Sentence.Before(""));
        Assert.Throws<ArgumentException>(() => Sentence.BeforeLast(""));
        Assert.Throws<ArgumentException>(() => Sentence.Between("", "y"));
        Assert.Throws<ArgumentException>(() => Sentence.Between("x", ""));
        Assert.Throws<ArgumentException>(() => Sentence.AllBetween("", "y"));
        Assert.Throws<ArgumentException>(() => Sentence.AllBetween("x", ""));
    }
}
