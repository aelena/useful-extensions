namespace Aelena.CommonExtensions;

/// <summary>
/// Sequence operators that LINQ still lacks: sliding windows, consecutive pairs, running folds, splitting and
/// grouping by position, bounded top-N, a full outer join, and a one-pass partition.
/// </summary>
/// <remarks>
/// Members that return <see cref="IEnumerable{T}"/> validate their arguments immediately and defer the work.
/// Members that return lists are eager and enumerate the source exactly once.
/// </remarks>
public static class SequenceExtensions
{
    /// <param name="source">The sequence to operate on. Must not be <see langword="null"/>.</param>
    extension<T>(IEnumerable<T> source)
    {
        // ------------------------------------------------------------------ neighbours

        /// <summary>
        /// Lazily returns every run of <paramref name="size"/> consecutive elements, advancing one element at a time,
        /// so <c>[1, 2, 3, 4].Window(3)</c> yields <c>[1, 2, 3]</c> and <c>[2, 3, 4]</c>.
        /// A sequence shorter than <paramref name="size"/> yields nothing.
        /// </summary>
        /// <param name="size">The number of elements in each window. Must be positive.</param>
        /// <returns>Independent snapshots of each window, safe to keep after moving on.</returns>
        public IEnumerable<IReadOnlyList<T>> Window(int size)
        {
            Guard.NotNull(source);
            Guard.Positive(size);

            return Iterate();

            IEnumerable<IReadOnlyList<T>> Iterate()
            {
                var window = new Queue<T>(size + 1);
                foreach (var item in source)
                {
                    window.Enqueue(item);
                    if (window.Count > size)
                    {
                        window.Dequeue();
                    }

                    if (window.Count == size)
                    {
                        yield return [.. window];
                    }
                }
            }
        }

        /// <summary>
        /// Lazily returns each element together with the one before it, so <c>[1, 2, 3].Pairwise()</c> yields
        /// <c>(1, 2)</c> and <c>(2, 3)</c>. Sequences with fewer than two elements yield nothing.
        /// </summary>
        /// <returns>Consecutive (previous, current) pairs.</returns>
        public IEnumerable<(T Previous, T Current)> Pairwise()
        {
            Guard.NotNull(source);

            return Iterate();

            IEnumerable<(T Previous, T Current)> Iterate()
            {
                var hasPrevious = false;
                var previous = default(T)!;
                foreach (var item in source)
                {
                    if (hasPrevious)
                    {
                        yield return (previous, item);
                    }

                    previous = item;
                    hasPrevious = true;
                }
            }
        }

        // ------------------------------------------------------------------ folds

        /// <summary>
        /// Lazily returns the running result of folding the sequence: like <see cref="Enumerable.Aggregate{TSource, TAccumulate}(IEnumerable{TSource}, TAccumulate, Func{TAccumulate, TSource, TAccumulate})"/>,
        /// but yielding every intermediate value. <c>[1, 2, 3].Scan(0, (sum, x) => sum + x)</c> yields <c>1, 3, 6</c>.
        /// </summary>
        /// <typeparam name="TAccumulate">The type of the running value.</typeparam>
        /// <param name="seed">The value the fold starts from. It is not itself returned.</param>
        /// <param name="accumulator">Combines the running value with the next element.</param>
        /// <returns>One running value per source element.</returns>
        public IEnumerable<TAccumulate> Scan<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            Guard.NotNull(source);
            Guard.NotNull(accumulator);

            return Iterate();

            IEnumerable<TAccumulate> Iterate()
            {
                var running = seed;
                foreach (var item in source)
                {
                    running = accumulator(running, item);
                    yield return running;
                }
            }
        }

        /// <summary>
        /// Lazily returns the running result of folding the sequence, using its first element as the starting value.
        /// <c>[1, 2, 3].Scan((sum, x) => sum + x)</c> yields <c>1, 3, 6</c>.
        /// </summary>
        /// <param name="accumulator">Combines the running value with the next element.</param>
        /// <returns>One running value per source element; the first is the first element itself.</returns>
        public IEnumerable<T> Scan(Func<T, T, T> accumulator)
        {
            Guard.NotNull(source);
            Guard.NotNull(accumulator);

            return Iterate();

            IEnumerable<T> Iterate()
            {
                var started = false;
                var running = default(T)!;
                foreach (var item in source)
                {
                    running = started ? accumulator(running, item) : item;
                    started = true;
                    yield return running;
                }
            }
        }

        // ------------------------------------------------------------------ shaping

        /// <summary>
        /// Lazily returns the elements with <paramref name="separator"/> between each consecutive pair, so
        /// <c>[1, 2, 3].Intersperse(0)</c> yields <c>1, 0, 2, 0, 3</c>.
        /// </summary>
        /// <param name="separator">The element to place between neighbours. Never added at either end.</param>
        /// <returns>The interleaved sequence.</returns>
        public IEnumerable<T> Intersperse(T separator)
        {
            Guard.NotNull(source);

            return Iterate();

            IEnumerable<T> Iterate()
            {
                var first = true;
                foreach (var item in source)
                {
                    if (!first)
                    {
                        yield return separator;
                    }

                    yield return item;
                    first = false;
                }
            }
        }

        /// <summary>
        /// Splits the sequence in one pass into the elements that satisfy <paramref name="predicate"/> and the ones
        /// that do not, preserving order within each part.
        /// </summary>
        /// <param name="predicate">The condition that sends an element to <c>Matches</c>.</param>
        /// <returns>The matching elements and the others.</returns>
        public (IReadOnlyList<T> Matches, IReadOnlyList<T> Others) Partition(Func<T, bool> predicate)
        {
            Guard.NotNull(source);
            Guard.NotNull(predicate);

            var matches = new List<T>();
            var others = new List<T>();
            foreach (var item in source)
            {
                (predicate(item) ? matches : others).Add(item);
            }

            return (matches, others);
        }

        /// <summary>
        /// Lazily splits the sequence into groups at every element that satisfies <paramref name="isSeparator"/>.
        /// Separators are dropped. Like <see cref="string.Split(char[])"/>, adjacent separators produce an empty group
        /// and there is always at least one group, so an empty source yields a single empty group.
        /// </summary>
        /// <param name="isSeparator">Identifies the elements that mark a boundary.</param>
        /// <returns>The groups between separators, in order.</returns>
        public IEnumerable<IReadOnlyList<T>> SplitOn(Func<T, bool> isSeparator)
        {
            Guard.NotNull(source);
            Guard.NotNull(isSeparator);

            return Iterate();

            IEnumerable<IReadOnlyList<T>> Iterate()
            {
                var group = new List<T>();
                foreach (var item in source)
                {
                    if (isSeparator(item))
                    {
                        yield return group.ToArray();
                        group.Clear();
                    }
                    else
                    {
                        group.Add(item);
                    }
                }

                yield return group.ToArray();
            }
        }

        /// <summary>
        /// Lazily groups consecutive elements that share a key, so <c>"aabccc".ChunkBy(c => c)</c> yields the runs
        /// <c>aa</c>, <c>b</c>, <c>ccc</c>. Unlike <see cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>,
        /// a key that reappears later starts a new group.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="keySelector">Extracts the key that neighbours must share.</param>
        /// <param name="comparer">How keys are compared. Defaults to <see cref="EqualityComparer{T}.Default"/>.</param>
        /// <returns>Each run with its key, in order.</returns>
        public IEnumerable<(TKey Key, IReadOnlyList<T> Items)> ChunkBy<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
        {
            Guard.NotNull(source);
            Guard.NotNull(keySelector);

            return Iterate(comparer ?? EqualityComparer<TKey>.Default);

            IEnumerable<(TKey Key, IReadOnlyList<T> Items)> Iterate(IEqualityComparer<TKey> keyComparer)
            {
                List<T>? run = null;
                var runKey = default(TKey)!;
                foreach (var item in source)
                {
                    var key = keySelector(item);
                    if (run is not null && keyComparer.Equals(key, runKey))
                    {
                        run.Add(item);
                        continue;
                    }

                    if (run is not null)
                    {
                        yield return (runKey, run);
                    }

                    run = [item];
                    runKey = key;
                }

                if (run is not null)
                {
                    yield return (runKey, run);
                }
            }
        }

        // ------------------------------------------------------------------ ranking

        /// <summary>
        /// Returns the <paramref name="count"/> elements with the largest keys, largest first, without sorting the
        /// whole sequence: memory stays at <paramref name="count"/> elements however long the source is.
        /// Elements with equal keys keep their source order.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="count">How many elements to return at most. Zero yields an empty list.</param>
        /// <param name="keySelector">Extracts the key to rank by.</param>
        /// <param name="comparer">How keys are compared. Defaults to <see cref="Comparer{T}.Default"/>.</param>
        /// <returns>Up to <paramref name="count"/> elements, largest key first.</returns>
        public IReadOnlyList<T> TopBy<TKey>(int count, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
        {
            Guard.NotNull(source);
            Guard.NotNull(keySelector);
            Guard.NotNegative(count);

            return Extremes(source, count, keySelector, comparer ?? Comparer<TKey>.Default, largestFirst: true);
        }

        /// <summary>
        /// Returns the <paramref name="count"/> elements with the smallest keys, smallest first, without sorting the
        /// whole sequence. Elements with equal keys keep their source order.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="count">How many elements to return at most. Zero yields an empty list.</param>
        /// <param name="keySelector">Extracts the key to rank by.</param>
        /// <param name="comparer">How keys are compared. Defaults to <see cref="Comparer{T}.Default"/>.</param>
        /// <returns>Up to <paramref name="count"/> elements, smallest key first.</returns>
        public IReadOnlyList<T> BottomBy<TKey>(int count, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
        {
            Guard.NotNull(source);
            Guard.NotNull(keySelector);
            Guard.NotNegative(count);

            return Extremes(source, count, keySelector, comparer ?? Comparer<TKey>.Default, largestFirst: false);
        }

        // ------------------------------------------------------------------ joining

        /// <summary>
        /// Pairs every element with the elements of <paramref name="right"/> that share its key, and also returns
        /// the elements on either side that have no partner. LINQ's <c>Join</c> and <c>GroupJoin</c> only cover the
        /// inner and left-outer cases.
        /// </summary>
        /// <remarks>
        /// Results come in source order: each left element with each of its matches (or with <see langword="default"/>
        /// when it has none), followed by the unmatched right elements in their order.
        /// </remarks>
        /// <typeparam name="TRight">The element type of the other sequence.</typeparam>
        /// <typeparam name="TKey">The type of the join key.</typeparam>
        /// <param name="right">The sequence to join with. It is buffered; the source is streamed.</param>
        /// <param name="leftKey">Extracts the key from a source element.</param>
        /// <param name="rightKey">Extracts the key from a <paramref name="right"/> element.</param>
        /// <param name="comparer">How keys are compared. Defaults to <see cref="EqualityComparer{T}.Default"/>.</param>
        /// <returns>(left, right) pairs where either side is <see langword="default"/> when unmatched.</returns>
        public IEnumerable<(T? Left, TRight? Right)> FullOuterJoin<TRight, TKey>(
            IEnumerable<TRight> right,
            Func<T, TKey> leftKey,
            Func<TRight, TKey> rightKey,
            IEqualityComparer<TKey>? comparer = null)
        {
            Guard.NotNull(source);
            Guard.NotNull(right);
            Guard.NotNull(leftKey);
            Guard.NotNull(rightKey);

            return Iterate(comparer ?? EqualityComparer<TKey>.Default);

            IEnumerable<(T? Left, TRight? Right)> Iterate(IEqualityComparer<TKey> keyComparer)
            {
                var lookup = right.ToLookup(rightKey, keyComparer);
                var matched = new HashSet<TKey>(keyComparer);
                foreach (var item in source)
                {
                    var key = leftKey(item);
                    if (!lookup.Contains(key))
                    {
                        yield return (item, default);
                        continue;
                    }

                    matched.Add(key);
                    foreach (var partner in lookup[key])
                    {
                        yield return (item, partner);
                    }
                }

                foreach (var group in lookup)
                {
                    if (!matched.Contains(group.Key))
                    {
                        foreach (var partner in group)
                        {
                            yield return (default, partner);
                        }
                    }
                }
            }
        }
    }

    private static T[] Extremes<T, TKey>(IEnumerable<T> source, int count, Func<T, TKey> keySelector, IComparer<TKey> comparer, bool largestFirst)
    {
        if (count == 0)
        {
            return [];
        }

        var keys = new List<TKey>(count + 1);
        var items = new List<T>(count + 1);
        foreach (var item in source)
        {
            var key = keySelector(item);
            var index = keys.Count;
            while (index > 0 && Precedes(key, keys[index - 1]))
            {
                index--;
            }

            if (index == count)
            {
                continue;
            }

            keys.Insert(index, key);
            items.Insert(index, item);
            if (items.Count > count)
            {
                keys.RemoveAt(count);
                items.RemoveAt(count);
            }
        }

        return [.. items];

        // Strictly better only, so an element never overtakes an earlier one with an equal key.
        bool Precedes(TKey candidate, TKey existing)
        {
            var order = comparer.Compare(candidate, existing);
            return largestFirst ? order > 0 : order < 0;
        }
    }
}
