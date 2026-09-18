namespace Aelena.CommonExtensions.Tests;

public class SequenceExtensionsTests
{
    private static IEnumerable<int> Naturals()
    {
        for (var i = 0; ; i++)
        {
            yield return i;
        }
    }

    // ------------------------------------------------------------------ Window

    [Fact]
    public void Window_slides_one_element_at_a_time()
        => Assert.Equal([[1, 2, 3], [2, 3, 4], [3, 4, 5]], new[] { 1, 2, 3, 4, 5 }.Window(3));

    [Fact]
    public void Window_of_one_wraps_each_element()
        => Assert.Equal([[1], [2]], new[] { 1, 2 }.Window(1));

    [Fact]
    public void Window_yields_nothing_for_short_sequences()
    {
        Assert.Empty(new[] { 1, 2 }.Window(3));
        Assert.Empty(Array.Empty<int>().Window(1));
    }

    [Fact]
    public void Window_snapshots_are_independent()
    {
        var windows = new[] { 1, 2, 3 }.Window(2).ToList();
        Assert.Equal([1, 2], windows[0]);
        Assert.Equal([2, 3], windows[1]);
    }

    [Fact]
    public void Window_is_lazy_but_validates_eagerly()
    {
        Assert.Equal([0, 1, 2], Naturals().Window(3).First());

        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Window(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.Window(0));
    }

    // ------------------------------------------------------------------ Pairwise

    [Fact]
    public void Pairwise_returns_consecutive_pairs()
        => Assert.Equal([(1, 2), (2, 3), (3, 4)], new[] { 1, 2, 3, 4 }.Pairwise());

    [Fact]
    public void Pairwise_yields_nothing_for_fewer_than_two_elements()
    {
        Assert.Empty(new[] { 1 }.Pairwise());
        Assert.Empty(Array.Empty<int>().Pairwise());
    }

    [Fact]
    public void Pairwise_is_lazy_but_validates_eagerly()
    {
        Assert.Equal((0, 1), Naturals().Pairwise().First());

        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(source.Pairwise);
    }

    // ------------------------------------------------------------------ Scan

    [Fact]
    public void Scan_with_seed_yields_running_values_without_the_seed()
        => Assert.Equal([1, 3, 6, 10], new[] { 1, 2, 3, 4 }.Scan(0, (sum, x) => sum + x));

    [Fact]
    public void Scan_with_seed_can_change_type()
        => Assert.Equal(["a", "ab", "abc"], new[] { 'a', 'b', 'c' }.Scan("", (acc, c) => acc + c));

    [Fact]
    public void Scan_with_seed_on_empty_sequence_is_empty()
        => Assert.Empty(Array.Empty<int>().Scan(0, (sum, x) => sum + x));

    [Fact]
    public void Scan_without_seed_starts_from_the_first_element()
    {
        Assert.Equal([1, 3, 6, 10], new[] { 1, 2, 3, 4 }.Scan((sum, x) => sum + x));
        Assert.Equal([5], new[] { 5 }.Scan((a, b) => a + b));
        Assert.Empty(Array.Empty<int>().Scan((a, b) => a + b));
    }

    [Fact]
    public void Scan_computes_running_maximum()
        => Assert.Equal([3, 3, 7, 7, 9], new[] { 3, 1, 7, 2, 9 }.Scan(Math.Max));

    [Fact]
    public void Scan_is_lazy_but_validates_eagerly()
    {
        Assert.Equal([0, 1, 3], Naturals().Scan(0, (sum, x) => sum + x).Take(3));
        Assert.Equal([0, 1, 3], Naturals().Scan((sum, x) => sum + x).Take(3));

        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Scan(0, (a, b) => a + b));
        Assert.Throws<ArgumentNullException>(() => source.Scan((a, b) => a + b));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.Scan(0, (Func<int, int, int>)null!));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.Scan((Func<int, int, int>)null!));
    }

    // ------------------------------------------------------------------ Intersperse

    [Fact]
    public void Intersperse_places_the_separator_between_neighbours()
        => Assert.Equal([1, 0, 2, 0, 3], new[] { 1, 2, 3 }.Intersperse(0));

    [Fact]
    public void Intersperse_never_adds_a_separator_at_the_ends()
    {
        Assert.Equal([1], new[] { 1 }.Intersperse(0));
        Assert.Empty(Array.Empty<int>().Intersperse(0));
    }

    [Fact]
    public void Intersperse_is_lazy_but_validates_eagerly()
    {
        Assert.Equal([0, -1, 1], Naturals().Intersperse(-1).Take(3));

        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Intersperse(0));
    }

    // ------------------------------------------------------------------ Partition

    [Fact]
    public void Partition_splits_in_one_pass_preserving_order()
    {
        var (evens, odds) = new[] { 1, 2, 3, 4, 5 }.Partition(x => x % 2 == 0);

        Assert.Equal([2, 4], evens);
        Assert.Equal([1, 3, 5], odds);
    }

    [Fact]
    public void Partition_handles_all_or_nothing_matching()
    {
        var (all, none) = new[] { 2, 4 }.Partition(x => x % 2 == 0);
        Assert.Equal([2, 4], all);
        Assert.Empty(none);

        var (matches, others) = Array.Empty<int>().Partition(x => true);
        Assert.Empty(matches);
        Assert.Empty(others);
    }

    [Fact]
    public void Partition_validates_arguments()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Partition(x => true));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.Partition(null!));
    }

    // ------------------------------------------------------------------ SplitOn

    [Fact]
    public void SplitOn_splits_at_separators_and_drops_them()
        => Assert.Equal(
            [["a", "b"], ["c"], ["d", "e"]],
            new[] { "a", "b", "", "c", "", "d", "e" }.SplitOn(string.IsNullOrEmpty));

    [Fact]
    public void SplitOn_keeps_empty_groups_like_string_split()
    {
        Assert.Equal([[], ["a"], []], new[] { "", "a", "" }.SplitOn(string.IsNullOrEmpty));
        Assert.Equal([["a"], [], ["b"]], new[] { "a", "", "", "b" }.SplitOn(string.IsNullOrEmpty));
        Assert.Equal([[]], Array.Empty<string>().SplitOn(string.IsNullOrEmpty));
        Assert.Equal([["a", "b"]], new[] { "a", "b" }.SplitOn(string.IsNullOrEmpty));
    }

    [Fact]
    public void SplitOn_is_lazy_but_validates_eagerly()
    {
        Assert.Equal([0, 1, 2], Naturals().SplitOn(x => x == 3).First());

        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.SplitOn(x => true));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.SplitOn(null!));
    }

    // ------------------------------------------------------------------ ChunkBy

    [Fact]
    public void ChunkBy_groups_consecutive_runs_only()
    {
        var runs = "aabcccab".ChunkBy(c => c).ToList();

        Assert.Equal(['a', 'b', 'c', 'a', 'b'], runs.Select(r => r.Key));
        Assert.Equal([2, 1, 3, 1, 1], runs.Select(r => r.Items.Count));
        Assert.Equal(['a', 'a'], runs[0].Items);
    }

    [Fact]
    public void ChunkBy_uses_the_given_comparer()
    {
        var runs = new[] { "a", "A", "b" }.ChunkBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();

        Assert.Equal(2, runs.Count);
        Assert.Equal(["a", "A"], runs[0].Items);
        Assert.Equal("a", runs[0].Key);
    }

    [Fact]
    public void ChunkBy_on_empty_sequence_is_empty()
        => Assert.Empty(Array.Empty<int>().ChunkBy(x => x));

    [Fact]
    public void ChunkBy_is_lazy_but_validates_eagerly()
    {
        var (key, items) = Naturals().ChunkBy(x => x / 3).First();
        Assert.Equal(0, key);
        Assert.Equal([0, 1, 2], items);

        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.ChunkBy(x => x));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.ChunkBy((Func<int, int>)null!));
    }

    // ------------------------------------------------------------------ TopBy / BottomBy

    private static readonly (string Name, int Score)[] Players =
    [
        ("ann", 70), ("bob", 90), ("cid", 50), ("dee", 90), ("eve", 80), ("fay", 60),
    ];

    [Fact]
    public void TopBy_returns_largest_first_and_keeps_source_order_for_ties()
        => Assert.Equal(["bob", "dee", "eve"], Players.TopBy(3, p => p.Score).Select(p => p.Name));

    [Fact]
    public void BottomBy_returns_smallest_first()
        => Assert.Equal(["cid", "fay", "ann"], Players.BottomBy(3, p => p.Score).Select(p => p.Name));

    [Fact]
    public void TopBy_handles_counts_larger_than_the_sequence()
        => Assert.Equal(["bob", "dee", "eve", "ann", "fay", "cid"], Players.TopBy(10, p => p.Score).Select(p => p.Name));

    [Fact]
    public void TopBy_with_zero_count_or_empty_source_is_empty()
    {
        Assert.Empty(Players.TopBy(0, p => p.Score));
        Assert.Empty(Array.Empty<int>().TopBy(3, x => x));
        Assert.Empty(Array.Empty<int>().BottomBy(3, x => x));
    }

    [Fact]
    public void TopBy_uses_the_given_comparer()
        => Assert.Equal(["B", "b"], new[] { "a", "B", "b" }.TopBy(2, s => s, StringComparer.OrdinalIgnoreCase));

    [Fact]
    public void TopBy_matches_the_sorted_answer_on_larger_input()
    {
        var random = new Random(42);
        var data = Enumerable.Range(0, 500).Select(_ => random.Next(0, 50)).ToArray();

        Assert.Equal(data.OrderByDescending(x => x).Take(7), data.TopBy(7, x => x));
        Assert.Equal(data.OrderBy(x => x).Take(7), data.BottomBy(7, x => x));
    }

    [Fact]
    public void TopBy_validates_arguments()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.TopBy(1, x => x));
        Assert.Throws<ArgumentNullException>(() => source.BottomBy(1, x => x));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.TopBy(1, (Func<int, int>)null!));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.BottomBy(1, (Func<int, int>)null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.TopBy(-1, x => x));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.BottomBy(-1, x => x));
    }

    // ------------------------------------------------------------------ FullOuterJoin

    private static readonly (int Id, string Name)[] Employees = [(1, "ann"), (2, "bob"), (3, "cid")];
    private static readonly (int EmployeeId, string Title)[] Badges = [(2, "lead"), (4, "guest"), (1, "dev"), (1, "oncall")];

    [Fact]
    public void FullOuterJoin_pairs_matches_and_keeps_both_kinds_of_orphans()
    {
        var rows = Employees.FullOuterJoin(Badges, e => e.Id, b => b.EmployeeId).ToList();

        Assert.Equal(
            [
                ((1, "ann"), (1, "dev")),
                ((1, "ann"), (1, "oncall")),
                ((2, "bob"), (2, "lead")),
                ((3, "cid"), default),
                (default, (4, "guest")),
            ],
            rows);
    }

    [Fact]
    public void FullOuterJoin_with_reference_types_yields_null_for_missing_sides()
    {
        var rows = new[] { "a", "b" }.FullOuterJoin(["B", "c"], l => l, r => r, StringComparer.OrdinalIgnoreCase).ToList();

        Assert.Equal([("a", null), ("b", "B"), (null, "c")], rows);
    }

    [Fact]
    public void FullOuterJoin_handles_empty_sides()
    {
        Assert.Equal([(default, 1)], Array.Empty<int>().FullOuterJoin([1], x => x, x => x));
        Assert.Equal([(1, default)], new[] { 1 }.FullOuterJoin(Array.Empty<int>(), x => x, x => x));
        Assert.Empty(Array.Empty<int>().FullOuterJoin(Array.Empty<int>(), x => x, x => x));
    }

    [Fact]
    public void FullOuterJoin_is_lazy_but_validates_eagerly()
    {
        Assert.Equal((0, default), Naturals().FullOuterJoin([5], x => x, x => x).First());

        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.FullOuterJoin([1], x => x, x => x));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.FullOuterJoin((IEnumerable<int>)null!, x => x, x => x));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.FullOuterJoin([1], (Func<int, int>)null!, x => x));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.FullOuterJoin([1], x => x, (Func<int, int>)null!));
    }
}
