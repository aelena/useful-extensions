using System.Collections.Concurrent;

namespace Aelena.CommonExtensions;

/// <summary>
/// Small functional-programming helpers that read left to right: piping a value through a function, tapping a
/// value for a side effect, memoizing a function, and turning exceptions into values.
/// </summary>
/// <remarks>
/// No new types are introduced. Results that can fail are plain tuples, so they compose with pattern matching
/// and deconstruction and never compete with a caller's own <c>Result</c> or <c>Option</c> type.
/// </remarks>
public static class FunctionalExtensions
{
    /// <param name="value">The value to operate on. May be <see langword="null"/>.</param>
    extension<T>(T value)
    {
        /// <summary>
        /// Applies <paramref name="transform"/> to the value, so nested calls read as a left-to-right chain:
        /// <c>text.Pipe(Parse).Pipe(Validate)</c> instead of <c>Validate(Parse(text))</c>.
        /// </summary>
        /// <typeparam name="TResult">The type the function produces.</typeparam>
        /// <param name="transform">The function to apply.</param>
        /// <returns>Whatever <paramref name="transform"/> returns.</returns>
        public TResult Pipe<TResult>(Func<T, TResult> transform)
        {
            Guard.NotNull(transform);

            return transform(value);
        }

        /// <summary>
        /// Runs <paramref name="action"/> on the value and returns the value unchanged, so a side effect such as
        /// logging or an assertion can sit in the middle of a chain without breaking it.
        /// </summary>
        /// <param name="action">The side effect to run.</param>
        /// <returns>The same value.</returns>
        public T Tap(Action<T> action)
        {
            Guard.NotNull(action);

            action(value);
            return value;
        }

        /// <summary>
        /// Applies <paramref name="transform"/> to the value and captures the outcome instead of throwing.
        /// </summary>
        /// <typeparam name="TResult">The type the function produces.</typeparam>
        /// <param name="transform">The function to apply.</param>
        /// <returns>
        /// <c>(true, result, null)</c> when the function returns, or <c>(false, default, exception)</c> when it throws.
        /// </returns>
        public (bool Success, TResult? Value, Exception? Error) TryPipe<TResult>(Func<T, TResult> transform)
        {
            Guard.NotNull(transform);

            return Attempt(() => transform(value));
        }
    }

    /// <param name="function">The function to wrap. Must not be <see langword="null"/>.</param>
    extension<TResult>(Func<TResult> function)
    {
        /// <summary>Invokes the function and captures the outcome instead of throwing.</summary>
        /// <returns>
        /// <c>(true, result, null)</c> when the function returns, or <c>(false, default, exception)</c> when it throws.
        /// </returns>
        public (bool Success, TResult? Value, Exception? Error) Try()
        {
            Guard.NotNull(function);

            return Attempt(function);
        }

        /// <summary>
        /// Returns a function that computes the value on first call and returns the cached value afterwards.
        /// Thread-safe; if two threads race on the first call the function may run twice, but only one result is kept.
        /// A call that throws is not cached and will be retried.
        /// </summary>
        /// <returns>The memoized function.</returns>
        public Func<TResult> Memoize()
        {
            Guard.NotNull(function);

            // A one-slot cache. Lazy<T> would do the same job but carries trimming annotations on T that
            // the public generic parameter cannot satisfy.
            var cache = new ConcurrentDictionary<byte, TResult>();
            return () => cache.GetOrAdd(0, _ => function());
        }
    }

    /// <param name="function">The function to wrap. Must not be <see langword="null"/>.</param>
    extension<T, TResult>(Func<T, TResult> function) where T : notnull
    {
        /// <summary>
        /// Returns a function that remembers its result for each distinct argument, so an expensive pure function
        /// is evaluated at most once per input. The cache grows without bound and lives as long as the returned delegate.
        /// Thread-safe; if two threads race on the same new argument the function may run twice, but only one result is
        /// kept. A call that throws is not cached and will be retried.
        /// </summary>
        /// <param name="comparer">How arguments are compared. Defaults to <see cref="EqualityComparer{T}.Default"/>.</param>
        /// <returns>The memoized function.</returns>
        public Func<T, TResult> Memoize(IEqualityComparer<T>? comparer = null)
        {
            Guard.NotNull(function);

            var cache = new ConcurrentDictionary<T, TResult>(comparer ?? EqualityComparer<T>.Default);
            return argument => cache.GetOrAdd(argument, function);
        }
    }

    /// <param name="function">The function to wrap. Must not be <see langword="null"/>.</param>
    extension<T1, T2, TResult>(Func<T1, T2, TResult> function)
    {
        /// <summary>
        /// Returns a function that remembers its result for each distinct pair of arguments. Same guarantees as the
        /// single-argument <c>Memoize</c>.
        /// </summary>
        /// <returns>The memoized function.</returns>
        public Func<T1, T2, TResult> Memoize()
        {
            Guard.NotNull(function);

            var cache = new ConcurrentDictionary<(T1, T2), TResult>();
            return (first, second) => cache.GetOrAdd((first, second), key => function(key.Item1, key.Item2));
        }
    }

    private static (bool Success, TResult? Value, Exception? Error) Attempt<TResult>(Func<TResult> function)
    {
        try
        {
            return (true, function(), null);
        }
#pragma warning disable CA1031 // The whole point is to hand every exception back to the caller as a value.
        catch (Exception error)
#pragma warning restore CA1031
        {
            return (false, default, error);
        }
    }
}
