namespace Aelena.CommonExtensions.Tests;

/// <summary>
/// Every code sample in README.md, asserted. If one of these fails, the README is lying.
/// </summary>
public class ReadmeExamplesTests
{
    private enum Status
    {
        Draft,
        Pending,
        Published,
    }

    private static IEnumerable<int> Naturals()
    {
        for (var i = 0; ; i++)
        {
            yield return i;
        }
    }

    [Fact]
    public void Headline_samples()
    {
        Assert.Equal("value", "key=value".After("="));
        Assert.Equal("bin", "/usr/local/bin".AfterLast('/'));
        Assert.Equal("bold", "<b>bold</b>".Between("<b>", "</b>"));
        Assert.Equal("a-b+c", "a-b-c".ReplaceLast("-", "+"));
        Assert.Equal("creme brulee", "crème brûlée".RemoveDiacritics());
        Assert.Equal("a very lo…", "a very long headline".Truncate(10, "…"));
        Assert.Equal(["a", "\"b,c\"", "d"], "a,\"b,c\",d".SplitOutside(',', '"'));

        Assert.Equal(1, new[] { 3, 8, 12 }.FindIndex(x => x > 5));
        Assert.Equal([0, 1, 2, 3], Naturals().TakeUntil(x => x == 3));

        var status = Status.Pending;
        Assert.True(status.In(Status.Draft, Status.Pending));
        Assert.False(Status.Published.In(Status.Draft, Status.Pending));

        var today = DateTime.Today;
        Assert.True(today.IsBetween(today.AddDays(-1), today.AddDays(1)));
    }

    [Fact]
    public void Slicing_by_marker()
    {
        Assert.Equal("value", "key=value".After("="));
        Assert.Equal("key", "key=value".Before("="));
        Assert.Equal("b=c", "a=b=c".After("="));
        Assert.Equal("c", "a=b=c".AfterLast("="));
        Assert.Equal("a=b", "a=b=c".BeforeLast("="));
        Assert.Equal("gz", "file.tar.gz".AfterLast('.'));
        Assert.Equal("file.tar", "file.tar.gz".BeforeLast('.'));

        const string noMarker = "no marker here";
        Assert.Same(noMarker, noMarker.After("="));

        Assert.Equal("", "Hello WORLD".After("world", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Text_between_two_markers()
    {
        Assert.Equal("bold", "<b>bold</b>".Between("<b>", "</b>"));
        Assert.Equal("hello", "say [hello] and [bye]".Between("[", "]"));
        Assert.Equal("", "unterminated [".Between("[", "]"));
        Assert.Equal("2", "a=[1] b=[2]".After("b=").Between("[", "]"));

        Assert.Equal(["1", "2", "3"], "[1] x [2] y [3]".AllBetween("[", "]"));
        Assert.Equal(["a", "b"], "'a' 'b' '".AllBetween("'", "'"));
    }

    [Fact]
    public void Replacing_first_or_last()
    {
        Assert.Equal("a+b-c", "a-b-c".ReplaceFirst("-", "+"));
        Assert.Equal("a-b+c", "a-b-c".ReplaceLast("-", "+"));
        Assert.Equal("one, two and three", "one, two, three".ReplaceLast(", ", " and "));
        Assert.Equal("x-B-b", "b-B-b".ReplaceFirst("B", "x", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Removing_several_values()
    {
        Assert.Equal("123", "(1) [2] {3}".RemoveAll("(", ")", "[", "]", "{", "}", " "));
        Assert.Equal(" ", "Ab aB".RemoveAll(StringComparison.OrdinalIgnoreCase, "ab"));

        var tagsToStrip = new List<string> { "<p>", "</p>" };
        Assert.Equal("text", "<p>text</p>".RemoveAll(tagsToStrip));
    }

    [Fact]
    public void Truncating()
    {
        Assert.Equal("hello", "hello world".Truncate(5));
        Assert.Equal("hello...", "hello world".Truncate(8, "..."));
        Assert.Equal("hello w…", "hello world".Truncate(8, "…"));
        Assert.Equal("..", "hello world".Truncate(2, "..."));

        const string fits = "hi";
        Assert.Same(fits, fits.Truncate(8, "…"));
    }

    [Fact]
    public void Substrings_that_never_throw()
    {
        Assert.Equal("world", "hello world".SafeSubstring(6, 50));
        Assert.Equal("", "hello world".SafeSubstring(50, 5));
        Assert.Equal("he", "hello world".SafeSubstring(-3, 5));
        Assert.Equal("world", "hello world".SafeSubstring(6));
        Assert.Equal("", "hello world".SafeSubstring(50));
    }

    [Fact]
    public void Removing_diacritics()
    {
        Assert.Equal("creme brulee", "crème brûlée".RemoveDiacritics());
        Assert.Equal("Angstrom", "Ångström".RemoveDiacritics());
        Assert.Equal("Sao Paulo", "São Paulo".RemoveDiacritics());
        Assert.Equal("łodz", "łódź".RemoveDiacritics());
    }

    [Fact]
    public void Finding_every_index()
    {
        Assert.Equal([1, 3, 5], "a-b-c-d".AllIndicesOf('-'));
        Assert.Equal([0, 3, 6], "abcabcabc".AllIndicesOf("abc"));
        Assert.Equal([0, 2], "aaaa".AllIndicesOf("aa"));
        Assert.Equal([0, 1, 2], "aaaa".AllIndicesOf("aa", overlapping: true));
        Assert.Equal([0, 4], "The the".AllIndicesOf("the", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Searching_for_any_of_several_values()
    {
        const string text = "an error occurred while saving";
        var bannedWords = new HashSet<string> { "error", "fatal" };
        var severities = new[] { "FATAL", "ERROR", "WARNING" };

        Assert.True(text.ContainsAny("error", "fatal", "panic"));
        Assert.True(text.ContainsAny(StringComparison.OrdinalIgnoreCase, "ERROR", "FATAL"));
        Assert.True(text.ContainsAny(bannedWords));

        Assert.Equal("error", text.FirstContained("fatal", "error", "warning"));
        Assert.Null(text.FirstContained("fatal", "panic"));
        Assert.Equal("ERROR", text.FirstContained(severities, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Splitting_while_respecting_quotes()
    {
        Assert.Equal(["a", "\"b,c\"", "d"], "a,\"b,c\",d".SplitOutside(',', '"'));
        Assert.Equal(["f(a, b)", "c", "g(d)"], "f(a, b), c, g(d)".SplitOutside(", ", ("(", ")")));
        Assert.Equal(["\"a,b\"", "[c;d]", "e"], "\"a,b\";[c;d],e".SplitOutside([",", ";"], [("\"", "\""), ("[", "]")]));
        Assert.Equal(["a", "b"], " a , , b ".SplitOutside(',', '"', Split.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(["a", "b", "c"], "a||b|c".SplitOutside(["|", "||"], []));
    }

    [Fact]
    public void Fuzzy_matching()
    {
        Assert.Equal(3, "kitten".LevenshteinDistance("sitting"));
        Assert.Equal(2, "flaw".LevenshteinDistance("lawn"));
        Assert.Equal(1, "colour".LevenshteinDistance("color"));
        Assert.Equal(1, "Kitten".LevenshteinDistance("kitten"));
        Assert.Equal(0, "Kitten".LevenshteinDistance("kitten", ignoreCase: true));

        var words = new[] { "cooler", "dollar", "collar", "colour" };
        Assert.Equal([("colour", 1)], words.ClosestTo("color"));
        Assert.Equal([("colour", 1), ("cooler", 2), ("collar", 2)], words.ClosestTo("color", 3));

        var commands = new[] { "build", "test", "pack", "publish" };
        var suggestion = commands.ClosestTo("pubish") is [{ Distance: <= 2 } best]
            ? $"Did you mean '{best.Value}'?"
            : null;
        Assert.Equal("Did you mean 'publish'?", suggestion);
        Assert.Null(commands.ClosestTo("xyzzy") is [{ Distance: <= 2 } other] ? other.Value : null);

        var names = new[] { "color", "colour", "dollar", "collar" };
        Assert.Equal([("color", "colour", 1)], names.ClosestPairs());
        Assert.Equal([("color", "colour", 1), ("dollar", "collar", 1)], names.ClosestPairs(2));
    }

    [Fact]
    public void Fuzzy_matching_metrics()
    {
        Assert.Equal(2, "recieve".LevenshteinDistance("receive"));
        Assert.Equal(1, "recieve".DamerauLevenshteinDistance("receive"));
        Assert.Equal(3, "karolin".HammingDistance("kathrin"));
        Assert.Equal(3, "ca".DamerauLevenshteinDistance("abc"));

        Assert.Equal(0.9444, "MARTHA".JaroSimilarity("MARHTA"), 4);
        Assert.Equal(0.9611, "MARTHA".JaroWinklerSimilarity("MARHTA"), 4);
        Assert.Equal(0.84, "DWAYNE".JaroWinklerSimilarity("DUANE"), 4);
        Assert.Equal(0.5714, "kitten".LevenshteinSimilarity("sitting"), 4);
        Assert.Equal(0.25, "night".DiceSimilarity("nacht"), 4);
        Assert.Equal(0.1429, "night".JaccardSimilarity("nacht"), 4);
        Assert.Equal(0.6154, "ABCBDAB".LongestCommonSubsequenceSimilarity("BDCABA"), 4);

        Assert.Equal(4, "ABCBDAB".LongestCommonSubsequenceLength("BDCABA"));
        Assert.Equal(" quick brown ", "the quick brown fox".LongestCommonSubstring("a quick brown dog"));
        Assert.Equal(["ab", "bc", "cd"], "abcd".NGrams(2));

        Assert.Equal([("receive", 2)], new[] { "receive", "deceive" }.ClosestTo("recieve"));
        Assert.Equal([("receive", 1)], new[] { "receive", "deceive" }.ClosestTo("recieve", metric: StringDistance.DamerauLevenshtein));

        var names = new[] { "DWAYNE", "MARHTA", "MARTHA", "DUANE" };
        var top = names.MostSimilarTo("MARTHA", 2);
        Assert.Equal(["MARTHA", "MARHTA"], top.Select(p => p.Value));
        Assert.Equal(1.0, top[0].Similarity, 4);
        Assert.Equal(0.9611, top[1].Similarity, 4);

        var words = new[] { "color", "colour", "dollar", "collar", "xyz" };
        Assert.Equal([["color", "colour"], ["dollar", "collar"], ["xyz"]], words.ClusterBy(maxDistance: 1));
        Assert.Equal([["recieve"], ["receive"]], new[] { "recieve", "receive" }.ClusterBy(1));
        Assert.Equal([["recieve", "receive"]], new[] { "recieve", "receive" }.ClusterBy(1, StringDistance.DamerauLevenshtein));

        var people = new[] { "MARTHA", "MARHTA", "DWAYNE", "DUANE" };
        Assert.Equal([["MARTHA", "MARHTA"], ["DWAYNE", "DUANE"]], people.ClusterBy(minSimilarity: 0.8));
    }

    [Fact]
    public void Sequence_operators()
    {
        var prices = new[] { 1.0, 2, 3, 4, 5 };
        Assert.Equal([2, 3, 4], prices.Window(3).Select(w => w.Average()));

        var readings = new[] { 10, 12, 15, 11 };
        Assert.Equal([2, 3, -4], readings.Pairwise().Select(p => p.Current - p.Previous));
        Assert.False(readings.Pairwise().All(p => p.Previous <= p.Current));

        var movements = new[] { 100m, -30m, 45m };
        Assert.Equal([100m, 70m, 115m], movements.Scan(0m, (balance, m) => balance + m));
        Assert.Equal([3, 3, 7, 7, 9], new[] { 3, 1, 7, 2, 9 }.Scan(Math.Max));
        Assert.Equal(["a", "ab", "abc"], new[] { 'a', 'b', 'c' }.Scan("", (acc, c) => acc + c));

        Assert.Equal("usr/local/bin", string.Concat(new[] { "usr", "local", "bin" }.Intersperse("/")));

        var (evens, odds) = new[] { 1, 2, 3, 4, 5 }.Partition(x => x % 2 == 0);
        Assert.Equal([2, 4], evens);
        Assert.Equal([1, 3, 5], odds);

        var lines = new[] { "first paragraph", "continues", "", "second paragraph" };
        Assert.Equal([["first paragraph", "continues"], ["second paragraph"]], lines.SplitOn(string.IsNullOrWhiteSpace));

        Assert.Equal(["3a", "1b", "2c"], "aaabcc".ChunkBy(c => c).Select(run => $"{run.Items.Count}{run.Key}"));

        var players = new[] { ("ann", 70), ("bob", 90), ("cid", 50), ("dee", 90), ("eve", 80) };
        Assert.Equal([("bob", 90), ("dee", 90), ("eve", 80)], players.TopBy(3, p => p.Item2));
        Assert.Equal([("cid", 50), ("ann", 70)], players.BottomBy(2, p => p.Item2));

        var expected = new[] { "a", "b", "c" };
        var actual = new[] { "b", "c", "d" };
        Assert.Equal(
            [("a", null), ("b", "b"), ("c", "c"), (null, "d")],
            expected.FullOuterJoin(actual, e => e, a => a));
    }

    [Fact]
    public void Sequence_index_lookup()
    {
        var scores = new[] { 3, 8, 12, 8 };

        Assert.Equal(1, scores.FindIndex(x => x > 5));
        Assert.Equal(3, scores.FindIndex(2, x => x == 8));
        Assert.Equal(3, scores.FindLastIndex(x => x == 8));
        Assert.Equal(-1, scores.FindIndex(x => x > 100));
        Assert.Equal([1, 3], scores.FindIndices(x => x == 8));
    }

    [Fact]
    public void Taking_up_to_and_including_a_match()
    {
        var lines = new[] { "BEGIN", "body", "END", "trailer" };

        Assert.Equal(["BEGIN", "body", "END"], lines.TakeUntil(l => l.StartsWith("END", StringComparison.Ordinal)));
        Assert.Equal(["BEGIN", "body"], lines.TakeUntil(l => l.StartsWith("END", StringComparison.Ordinal), inclusive: false));
        Assert.Equal([0, 1, 2, 3], Naturals().TakeUntil(x => x == 3));
    }

    [Fact]
    public void Null_tolerant_emptiness_checks()
    {
        List<string>? tags = ["first"];

        if (tags.HasItems())
        {
            Assert.Equal("first", tags[0]);
        }

        if (!tags.IsNullOrEmpty())
        {
            Assert.Single(tags);
        }

        tags = null;
        Assert.False(tags.HasItems());
        Assert.True(tags.IsNullOrEmpty());
    }

    [Fact]
    public void Membership()
    {
        var allowedExtensions = new HashSet<string> { ".png", ".jpg" };
        var admins = new[] { 1, 7, 42 };

        Assert.True(Status.Draft.In(Status.Draft, Status.Pending));
        Assert.True(".png".In(allowedExtensions));
        Assert.False(".gif".In(allowedExtensions));
        Assert.True(42.In(admins));
    }

    [Fact]
    public void Range_checks()
    {
        Assert.True(5.IsBetween(1, 10));
        Assert.True(10.IsBetween(1, 10));
        Assert.False(10.IsBetween(1, 10, inclusive: false));

        var today = new DateTime(2026, 9, 17, 0, 0, 0, DateTimeKind.Utc);
        Assert.True(today.IsBetween(new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc)));

        var statusCode = System.Net.HttpStatusCode.NoContent;
        Assert.True(((int)statusCode).IsBetween(200, 299));
    }
}
