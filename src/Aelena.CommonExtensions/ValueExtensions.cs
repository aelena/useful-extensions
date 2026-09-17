namespace Aelena.CommonExtensions;

/// <summary>
/// Extension members that read naturally on any value: set membership and range checks.
/// </summary>
public static class ValueExtensions
{
    /// <param name="value">The value to test.</param>
    extension<T>(T value)
    {
        /// <summary>Tells whether the value equals any of the given candidates, using <see cref="EqualityComparer{T}.Default"/>.</summary>
        /// <remarks>For compile-time constants prefer a pattern: <c>x is "a" or "b"</c>. This member is for values only known at run time.</remarks>
        /// <param name="candidates">The values to compare against.</param>
        /// <returns><see langword="true"/> when a candidate is equal to the value.</returns>
        public bool In(params ReadOnlySpan<T> candidates)
        {
            foreach (var candidate in candidates)
            {
                if (EqualityComparer<T>.Default.Equals(value, candidate))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Tells whether the value equals any element of <paramref name="candidates"/>, using <see cref="EqualityComparer{T}.Default"/>.</summary>
        /// <param name="candidates">The values to compare against.</param>
        /// <returns><see langword="true"/> when a candidate is equal to the value.</returns>
        public bool In(IEnumerable<T> candidates)
        {
            Guard.NotNull(candidates);

            return candidates.Contains(value);
        }
    }

    /// <param name="value">The value to test.</param>
    extension<T>(T value) where T : IComparable<T>
    {
        /// <summary>Tells whether the value lies between <paramref name="lower"/> and <paramref name="upper"/>.</summary>
        /// <param name="lower">The lower bound.</param>
        /// <param name="upper">The upper bound.</param>
        /// <param name="inclusive">
        /// When <see langword="true"/> (the default) the bounds themselves count as inside;
        /// when <see langword="false"/> the check is strict on both ends.
        /// </param>
        /// <returns><see langword="true"/> when the value is within the bounds.</returns>
        public bool IsBetween(T lower, T upper, bool inclusive = true)
        {
            var fromLower = value.CompareTo(lower);
            var fromUpper = value.CompareTo(upper);

            return inclusive
                ? fromLower >= 0 && fromUpper <= 0
                : fromLower > 0 && fromUpper < 0;
        }
    }
}
