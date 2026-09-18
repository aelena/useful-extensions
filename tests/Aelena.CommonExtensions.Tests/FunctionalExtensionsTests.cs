using System.Globalization;

namespace Aelena.CommonExtensions.Tests;

public class FunctionalExtensionsTests
{
    // ------------------------------------------------------------------ Pipe

    [Fact]
    public void Pipe_applies_the_function_left_to_right()
    {
        Assert.Equal(42, "42".Pipe(s => int.Parse(s, CultureInfo.InvariantCulture)));
        Assert.Equal("6", 3.Pipe(x => x * 2).Pipe(x => x.ToString(CultureInfo.InvariantCulture)));
    }

    [Fact]
    public void Pipe_passes_null_through_to_the_function()
    {
        string? nothing = null;
        Assert.True(nothing.Pipe(s => s is null));
    }

    [Fact]
    public void Pipe_validates_the_function()
        => Assert.Throws<ArgumentNullException>(() => 1.Pipe((Func<int, int>)null!));

    // ------------------------------------------------------------------ Tap

    [Fact]
    public void Tap_runs_the_action_and_returns_the_same_value()
    {
        var seen = new List<string>();
        var list = new List<int> { 1 };

        var result = list.Tap(l => seen.Add($"count={l.Count}"));

        Assert.Same(list, result);
        Assert.Equal(["count=1"], seen);
    }

    [Fact]
    public void Tap_sits_inside_a_chain_without_changing_it()
    {
        var log = new List<int>();

        var result = 5.Tap(log.Add).Pipe(x => x + 1).Tap(log.Add);

        Assert.Equal(6, result);
        Assert.Equal([5, 6], log);
    }

    [Fact]
    public void Tap_validates_the_action()
        => Assert.Throws<ArgumentNullException>(() => 1.Tap(null!));

    // ------------------------------------------------------------------ Try / TryPipe

    [Fact]
    public void Try_captures_the_result_of_a_successful_call()
    {
        Func<int> parse = () => int.Parse("42", CultureInfo.InvariantCulture);

        var (success, value, error) = parse.Try();

        Assert.True(success);
        Assert.Equal(42, value);
        Assert.Null(error);
    }

    [Fact]
    public void Try_captures_the_exception_of_a_failing_call()
    {
        Func<int> parse = () => int.Parse("x", CultureInfo.InvariantCulture);

        var (success, value, error) = parse.Try();

        Assert.False(success);
        Assert.Equal(0, value);
        Assert.IsType<FormatException>(error);
    }

    [Fact]
    public void TryPipe_captures_success_and_failure()
    {
        Assert.Equal((true, 42, null), "42".TryPipe(s => int.Parse(s, CultureInfo.InvariantCulture)));

        var (success, value, error) = "x".TryPipe(s => int.Parse(s, CultureInfo.InvariantCulture));
        Assert.False(success);
        Assert.Equal(0, value);
        Assert.IsType<FormatException>(error);
    }

    [Fact]
    public void TryPipe_works_with_patterns()
    {
        static string Describe(string input)
        {
            return input.TryPipe(s => int.Parse(s, CultureInfo.InvariantCulture)) switch
            {
                (true, var n, _) => $"number {n}",
                (false, _, var e) => $"failed: {e!.GetType().Name}",
            };
        }

        Assert.Equal("number 7", Describe("7"));
        Assert.Equal("failed: FormatException", Describe("seven"));
    }

    [Fact]
    public void Try_validates_the_function()
    {
        Func<int> nothing = null!;
        Assert.Throws<ArgumentNullException>(() => nothing.Try());
        Assert.Throws<ArgumentNullException>(() => 1.TryPipe((Func<int, int>)null!));
    }

    // ------------------------------------------------------------------ Memoize

    [Fact]
    public void Memoize_without_arguments_computes_once()
    {
        var calls = 0;
        Func<int> expensive = () => ++calls;

        var cached = expensive.Memoize();

        Assert.Equal(1, cached());
        Assert.Equal(1, cached());
        Assert.Equal(1, calls);
    }

    [Fact]
    public void Memoize_with_one_argument_computes_once_per_distinct_argument()
    {
        var calls = new List<int>();
        Func<int, int> square = x =>
        {
            calls.Add(x);
            return x * x;
        };

        var cached = square.Memoize();

        Assert.Equal(9, cached(3));
        Assert.Equal(9, cached(3));
        Assert.Equal(16, cached(4));
        Assert.Equal([3, 4], calls);
    }

    [Fact]
    public void Memoize_with_one_argument_honours_the_comparer()
    {
        var calls = 0;
        Func<string, int> length = s =>
        {
            calls++;
            return s.Length;
        };

        var cached = length.Memoize(StringComparer.OrdinalIgnoreCase);

        Assert.Equal(3, cached("abc"));
        Assert.Equal(3, cached("ABC"));
        Assert.Equal(1, calls);
    }

    [Fact]
    public void Memoize_with_two_arguments_keys_on_the_pair()
    {
        var calls = 0;
        Func<int, int, int> add = (a, b) =>
        {
            calls++;
            return a + b;
        };

        var cached = add.Memoize();

        Assert.Equal(3, cached(1, 2));
        Assert.Equal(3, cached(1, 2));
        Assert.Equal(3, cached(2, 1));
        Assert.Equal(2, calls);
    }

    [Fact]
    public void Memoize_does_not_cache_exceptions()
    {
        var attempts = 0;
        Func<int, int> flaky = x =>
        {
            attempts++;
            return attempts == 1 ? throw new InvalidOperationException("first call fails") : x;
        };

        var cached = flaky.Memoize();

        Assert.Throws<InvalidOperationException>(() => cached(1));
        Assert.Equal(1, cached(1));
        Assert.Equal(1, cached(1));
        Assert.Equal(2, attempts);
    }

    [Fact]
    public void Memoize_is_safe_under_concurrent_use()
    {
        var cached = new Func<int, int>(x => x * 2).Memoize();

        var results = new int[1000];
        Parallel.For(0, results.Length, i => results[i] = cached(i % 10));

        Assert.All(results.Select((r, i) => (r, i)), pair => Assert.Equal(pair.i % 10 * 2, pair.r));
    }

    [Fact]
    public void Memoize_validates_the_function()
    {
        Func<int> none = null!;
        Func<int, int> oneArg = null!;
        Func<int, int, int> twoArgs = null!;

        Assert.Throws<ArgumentNullException>(none.Memoize);
        Assert.Throws<ArgumentNullException>(() => oneArg.Memoize());
        Assert.Throws<ArgumentNullException>(twoArgs.Memoize);
    }
}
