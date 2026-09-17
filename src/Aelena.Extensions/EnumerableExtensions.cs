using System.Diagnostics.CodeAnalysis;

namespace Aelena.Extensions;

/// <summary>
/// Extension members for <see cref="IEnumerable{T}"/> that LINQ still lacks: predicate-based index lookup,
/// an inclusive <c>TakeWhile</c>, and null-tolerant emptiness checks.
/// </summary>
public static class EnumerableExtensions
{
    /// <param name="source">The sequence to operate on. Must not be <see langword="null"/>.</param>
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>Returns the index of the first element that satisfies <paramref name="predicate"/>.</summary>
        /// <param name="predicate">The condition to test each element against.</param>
        /// <returns>The zero-based index of the first match, or -1 when there is none.</returns>
        public int FindIndex(Func<T, bool> predicate) => source.FindIndex(0, predicate);

        /// <summary>
        /// Returns the index of the first element at or after <paramref name="startIndex"/> that satisfies <paramref name="predicate"/>.
        /// </summary>
        /// <param name="startIndex">The zero-based index where the search begins. Elements before it are skipped, not tested.</param>
        /// <param name="predicate">The condition to test each element against.</param>
        /// <returns>The zero-based index (relative to the whole sequence) of the first match, or -1 when there is none.</returns>
        public int FindIndex(int startIndex, Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentOutOfRangeException.ThrowIfNegative(startIndex);

            var i = 0;
            foreach (var item in source)
            {
                if (i >= startIndex && predicate(item))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        /// <summary>Returns the index of the last element that satisfies <paramref name="predicate"/>.</summary>
        /// <param name="predicate">The condition to test each element against.</param>
        /// <returns>The zero-based index of the last match, or -1 when there is none.</returns>
        public int FindLastIndex(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            var last = -1;
            var i = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    last = i;
                }

                i++;
            }

            return last;
        }

        /// <summary>Lazily returns the index of every element that satisfies <paramref name="predicate"/>.</summary>
        /// <param name="predicate">The condition to test each element against.</param>
        /// <returns>The zero-based indices in ascending order.</returns>
        public IEnumerable<int> FindIndices(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            return Iterate();

            IEnumerable<int> Iterate()
            {
                var i = 0;
                foreach (var item in source)
                {
                    if (predicate(item))
                    {
                        yield return i;
                    }

                    i++;
                }
            }
        }

        /// <summary>
        /// Lazily returns elements up to the first one that satisfies <paramref name="predicate"/>.
        /// Unlike <see cref="Enumerable.TakeWhile{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> the matching element itself is included by default.
        /// </summary>
        /// <param name="predicate">The condition that stops the sequence.</param>
        /// <param name="inclusive">Whether the element that satisfied <paramref name="predicate"/> is returned as the last item.</param>
        /// <returns>The leading elements, ending with (or just before) the first match.</returns>
        public IEnumerable<T> TakeUntil(Func<T, bool> predicate, bool inclusive = true)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            return Iterate();

            IEnumerable<T> Iterate()
            {
                foreach (var item in source)
                {
                    if (predicate(item))
                    {
                        if (inclusive)
                        {
                            yield return item;
                        }

                        yield break;
                    }

                    yield return item;
                }
            }
        }
    }

    /// <param name="source">The sequence to inspect. May be <see langword="null"/>.</param>
    extension<T>([NotNullWhen(false)] IEnumerable<T>? source)
    {
        /// <summary>Tells whether the sequence is <see langword="null"/> or has no elements.</summary>
        /// <remarks>Collections that expose a count are not enumerated; other sequences are enumerated at most one element.</remarks>
        /// <returns><see langword="true"/> for <see langword="null"/> or empty sequences.</returns>
        public bool IsNullOrEmpty() => source is null || !source.Any();
    }

    /// <param name="source">The sequence to inspect. May be <see langword="null"/>.</param>
    extension<T>([NotNullWhen(true)] IEnumerable<T>? source)
    {
        /// <summary>Tells whether the sequence is non-<see langword="null"/> and has at least one element.</summary>
        /// <remarks>Collections that expose a count are not enumerated; other sequences are enumerated at most one element.</remarks>
        /// <returns><see langword="true"/> for a sequence with items.</returns>
        public bool HasItems() => source is not null && source.Any();
    }
}
