using System.Globalization;
using System.Text;

namespace Aelena.Extensions;

/// <summary>
/// Extension members for <see cref="string"/> that fill gaps the BCL still leaves open:
/// marker-based slicing, count-limited replacement, safe truncation and slicing,
/// diacritics removal, multi-needle searching and quote-aware splitting.
/// </summary>
/// <remarks>
/// All members throw <see cref="ArgumentNullException"/> when the receiver is <see langword="null"/>.
/// Every comparison defaults to <see cref="StringComparison.Ordinal"/>.
/// </remarks>
public static class StringExtensions
{
    /// <param name="s">The string to operate on. Must not be <see langword="null"/>.</param>
    extension(string s)
    {
        // ------------------------------------------------------------------ marker slicing

        /// <summary>Returns the part of the string that follows the first occurrence of <paramref name="marker"/>.</summary>
        /// <param name="marker">The marker to search for. Must not be empty.</param>
        /// <param name="comparison">How the marker is matched.</param>
        /// <returns>The substring after the marker, or the whole string when the marker is not found.</returns>
        public string After(string marker, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(marker);

            var i = s.IndexOf(marker, comparison);
            return i < 0 ? s : s[(i + marker.Length)..];
        }

        /// <summary>Returns the part of the string that follows the first occurrence of <paramref name="marker"/>.</summary>
        /// <param name="marker">The character to search for.</param>
        /// <returns>The substring after the marker, or the whole string when the marker is not found.</returns>
        public string After(char marker)
        {
            ArgumentNullException.ThrowIfNull(s);

            var i = s.IndexOf(marker);
            return i < 0 ? s : s[(i + 1)..];
        }

        /// <summary>Returns the part of the string that follows the last occurrence of <paramref name="marker"/>.</summary>
        /// <param name="marker">The marker to search for. Must not be empty.</param>
        /// <param name="comparison">How the marker is matched.</param>
        /// <returns>The substring after the last marker, or the whole string when the marker is not found.</returns>
        public string AfterLast(string marker, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(marker);

            var i = s.LastIndexOf(marker, comparison);
            return i < 0 ? s : s[(i + marker.Length)..];
        }

        /// <summary>Returns the part of the string that follows the last occurrence of <paramref name="marker"/>.</summary>
        /// <param name="marker">The character to search for.</param>
        /// <returns>The substring after the last marker, or the whole string when the marker is not found.</returns>
        public string AfterLast(char marker)
        {
            ArgumentNullException.ThrowIfNull(s);

            var i = s.LastIndexOf(marker);
            return i < 0 ? s : s[(i + 1)..];
        }

        /// <summary>Returns the part of the string that precedes the first occurrence of <paramref name="marker"/>.</summary>
        /// <param name="marker">The marker to search for. Must not be empty.</param>
        /// <param name="comparison">How the marker is matched.</param>
        /// <returns>The substring before the marker, or the whole string when the marker is not found.</returns>
        public string Before(string marker, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(marker);

            var i = s.IndexOf(marker, comparison);
            return i < 0 ? s : s[..i];
        }

        /// <summary>Returns the part of the string that precedes the first occurrence of <paramref name="marker"/>.</summary>
        /// <param name="marker">The character to search for.</param>
        /// <returns>The substring before the marker, or the whole string when the marker is not found.</returns>
        public string Before(char marker)
        {
            ArgumentNullException.ThrowIfNull(s);

            var i = s.IndexOf(marker);
            return i < 0 ? s : s[..i];
        }

        /// <summary>Returns the part of the string that precedes the last occurrence of <paramref name="marker"/>.</summary>
        /// <param name="marker">The marker to search for. Must not be empty.</param>
        /// <param name="comparison">How the marker is matched.</param>
        /// <returns>The substring before the last marker, or the whole string when the marker is not found.</returns>
        public string BeforeLast(string marker, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(marker);

            var i = s.LastIndexOf(marker, comparison);
            return i < 0 ? s : s[..i];
        }

        /// <summary>Returns the part of the string that precedes the last occurrence of <paramref name="marker"/>.</summary>
        /// <param name="marker">The character to search for.</param>
        /// <returns>The substring before the last marker, or the whole string when the marker is not found.</returns>
        public string BeforeLast(char marker)
        {
            ArgumentNullException.ThrowIfNull(s);

            var i = s.LastIndexOf(marker);
            return i < 0 ? s : s[..i];
        }

        /// <summary>
        /// Returns the text between the first occurrence of <paramref name="start"/> and the first
        /// occurrence of <paramref name="end"/> that follows it.
        /// </summary>
        /// <param name="start">The opening marker. Must not be empty.</param>
        /// <param name="end">The closing marker. Must not be empty.</param>
        /// <param name="comparison">How the markers are matched.</param>
        /// <returns>The enclosed text, or <see cref="string.Empty"/> when either marker is missing.</returns>
        public string Between(string start, string end, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(start);
            ArgumentException.ThrowIfNullOrEmpty(end);

            var i = s.IndexOf(start, comparison);
            if (i < 0)
            {
                return string.Empty;
            }

            i += start.Length;
            var j = s.IndexOf(end, i, comparison);
            return j < 0 ? string.Empty : s[i..j];
        }

        /// <summary>
        /// Lazily returns every non-overlapping piece of text enclosed by <paramref name="start"/> and <paramref name="end"/>,
        /// scanning left to right.
        /// </summary>
        /// <param name="start">The opening marker. Must not be empty.</param>
        /// <param name="end">The closing marker. Must not be empty.</param>
        /// <param name="comparison">How the markers are matched.</param>
        /// <returns>The enclosed pieces in document order. An unterminated opening marker yields nothing.</returns>
        public IEnumerable<string> AllBetween(string start, string end, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(start);
            ArgumentException.ThrowIfNullOrEmpty(end);

            return Iterate();

            IEnumerable<string> Iterate()
            {
                var position = 0;
                while (true)
                {
                    var i = s.IndexOf(start, position, comparison);
                    if (i < 0)
                    {
                        yield break;
                    }

                    i += start.Length;
                    var j = s.IndexOf(end, i, comparison);
                    if (j < 0)
                    {
                        yield break;
                    }

                    yield return s[i..j];
                    position = j + end.Length;
                }
            }
        }

        // ------------------------------------------------------------------ replacing

        /// <summary>Replaces only the first occurrence of <paramref name="oldValue"/>.</summary>
        /// <param name="oldValue">The text to replace. Must not be empty.</param>
        /// <param name="newValue">The replacement text.</param>
        /// <param name="comparison">How <paramref name="oldValue"/> is matched.</param>
        /// <returns>A new string, or the original instance when there is nothing to replace.</returns>
        public string ReplaceFirst(string oldValue, string newValue, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(oldValue);
            ArgumentNullException.ThrowIfNull(newValue);

            var i = s.IndexOf(oldValue, comparison);
            return i < 0 ? s : string.Concat(s.AsSpan(0, i), newValue, s.AsSpan(i + oldValue.Length));
        }

        /// <summary>Replaces only the last occurrence of <paramref name="oldValue"/>.</summary>
        /// <param name="oldValue">The text to replace. Must not be empty.</param>
        /// <param name="newValue">The replacement text.</param>
        /// <param name="comparison">How <paramref name="oldValue"/> is matched.</param>
        /// <returns>A new string, or the original instance when there is nothing to replace.</returns>
        public string ReplaceLast(string oldValue, string newValue, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(oldValue);
            ArgumentNullException.ThrowIfNull(newValue);

            var i = s.LastIndexOf(oldValue, comparison);
            return i < 0 ? s : string.Concat(s.AsSpan(0, i), newValue, s.AsSpan(i + oldValue.Length));
        }

        /// <summary>Removes every occurrence of each of the given values, in order.</summary>
        /// <param name="values">The values to remove. None may be empty.</param>
        /// <returns>A new string with all the values removed.</returns>
        public string RemoveAll(params ReadOnlySpan<string> values) => s.RemoveAll(StringComparison.Ordinal, values);

        /// <summary>Removes every occurrence of each of the given values, in order.</summary>
        /// <param name="comparison">How the values are matched.</param>
        /// <param name="values">The values to remove. None may be empty.</param>
        /// <returns>A new string with all the values removed.</returns>
        public string RemoveAll(StringComparison comparison, params ReadOnlySpan<string> values)
        {
            ArgumentNullException.ThrowIfNull(s);

            var result = s;
            foreach (var value in values)
            {
                result = result.Replace(value, string.Empty, comparison);
            }

            return result;
        }

        /// <summary>Removes every occurrence of each of the given values, in order.</summary>
        /// <param name="values">The values to remove. None may be empty.</param>
        /// <param name="comparison">How the values are matched.</param>
        /// <returns>A new string with all the values removed.</returns>
        public string RemoveAll(IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentNullException.ThrowIfNull(values);

            var result = s;
            foreach (var value in values)
            {
                result = result.Replace(value, string.Empty, comparison);
            }

            return result;
        }

        // ------------------------------------------------------------------ sizing

        /// <summary>Shortens the string so that it is at most <paramref name="maxLength"/> characters long.</summary>
        /// <param name="maxLength">The maximum length of the result, including the suffix.</param>
        /// <param name="suffix">Text appended when truncation happens, for example an ellipsis. Counts toward <paramref name="maxLength"/>.</param>
        /// <returns>The original instance when it already fits, otherwise a shortened copy ending in <paramref name="suffix"/>.</returns>
        public string Truncate(int maxLength, string suffix = "")
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentNullException.ThrowIfNull(suffix);
            ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

            return s.Length <= maxLength ? s
                : suffix.Length >= maxLength ? suffix[..maxLength]
                : string.Concat(s.AsSpan(0, maxLength - suffix.Length), suffix);
        }

        /// <summary>
        /// A <see cref="string.Substring(int, int)"/> that never throws: the requested range is clipped to the string.
        /// </summary>
        /// <param name="start">The zero-based start index. May be negative or past the end.</param>
        /// <param name="length">The number of characters wanted. Zero or negative yields an empty string.</param>
        /// <returns>The part of the requested range that lies inside the string.</returns>
        public string SafeSubstring(int start, int length)
        {
            ArgumentNullException.ThrowIfNull(s);

            if (length <= 0)
            {
                return string.Empty;
            }

            var from = Math.Max(start, 0);
            var to = (int)Math.Min((long)start + length, s.Length);
            return to <= from ? string.Empty : s[from..to];
        }

        /// <summary>A <see cref="string.Substring(int)"/> that never throws: the start index is clipped to the string.</summary>
        /// <param name="start">The zero-based start index. May be negative or past the end.</param>
        /// <returns>Everything from <paramref name="start"/> to the end, or an empty string when <paramref name="start"/> is past the end.</returns>
        public string SafeSubstring(int start)
        {
            ArgumentNullException.ThrowIfNull(s);

            return start >= s.Length ? string.Empty : s[Math.Max(start, 0)..];
        }

        // ------------------------------------------------------------------ unicode

        /// <summary>
        /// Strips combining diacritical marks, so that <c>"crème brûlée"</c> becomes <c>"creme brulee"</c>.
        /// </summary>
        /// <remarks>
        /// Works by decomposing to Unicode normalization form D and dropping non-spacing marks. Letters that
        /// are not canonically decomposable, such as <c>ł</c> or <c>ø</c>, are left untouched.
        /// </remarks>
        /// <returns>The original instance when there was nothing to strip, otherwise a new string in normalization form C.</returns>
        public string RemoveDiacritics()
        {
            ArgumentNullException.ThrowIfNull(s);

            var decomposed = s.Normalize(NormalizationForm.FormD);
            Span<char> buffer = decomposed.Length <= 256 ? stackalloc char[256] : new char[decomposed.Length];

            var kept = 0;
            foreach (var c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    buffer[kept++] = c;
                }
            }

            return kept == decomposed.Length
                ? s
                : new string(buffer[..kept]).Normalize(NormalizationForm.FormC);
        }

        // ------------------------------------------------------------------ searching

        /// <summary>Lazily returns the index of every occurrence of <paramref name="value"/>.</summary>
        /// <param name="value">The text to look for. Must not be empty.</param>
        /// <param name="comparison">How <paramref name="value"/> is matched.</param>
        /// <param name="overlapping">
        /// When <see langword="true"/>, matches may overlap (so <c>"aaa".AllIndicesOf("aa")</c> yields 0 and 1).
        /// When <see langword="false"/>, scanning resumes after each match (yielding only 0).
        /// </param>
        /// <returns>The zero-based indices in ascending order.</returns>
        public IEnumerable<int> AllIndicesOf(string value, StringComparison comparison = StringComparison.Ordinal, bool overlapping = false)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentException.ThrowIfNullOrEmpty(value);

            return Iterate();

            IEnumerable<int> Iterate()
            {
                var step = overlapping ? 1 : value.Length;
                var position = 0;
                while ((position = s.IndexOf(value, position, comparison)) >= 0)
                {
                    yield return position;
                    position += step;
                }
            }
        }

        /// <summary>Lazily returns the index of every occurrence of <paramref name="value"/>.</summary>
        /// <param name="value">The character to look for.</param>
        /// <returns>The zero-based indices in ascending order.</returns>
        public IEnumerable<int> AllIndicesOf(char value)
        {
            ArgumentNullException.ThrowIfNull(s);

            return Iterate();

            IEnumerable<int> Iterate()
            {
                var position = 0;
                while ((position = s.IndexOf(value, position)) >= 0)
                {
                    yield return position;
                    position++;
                }
            }
        }

        /// <summary>Tells whether the string contains at least one of the given values.</summary>
        /// <param name="values">The candidates to look for.</param>
        /// <returns><see langword="true"/> when any candidate is found.</returns>
        public bool ContainsAny(params ReadOnlySpan<string> values) => s.ContainsAny(StringComparison.Ordinal, values);

        /// <summary>Tells whether the string contains at least one of the given values.</summary>
        /// <param name="comparison">How the candidates are matched.</param>
        /// <param name="values">The candidates to look for.</param>
        /// <returns><see langword="true"/> when any candidate is found.</returns>
        public bool ContainsAny(StringComparison comparison, params ReadOnlySpan<string> values)
        {
            ArgumentNullException.ThrowIfNull(s);

            foreach (var value in values)
            {
                if (s.Contains(value, comparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Tells whether the string contains at least one of the given values.</summary>
        /// <param name="values">The candidates to look for.</param>
        /// <param name="comparison">How the candidates are matched.</param>
        /// <returns><see langword="true"/> when any candidate is found.</returns>
        public bool ContainsAny(IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentNullException.ThrowIfNull(values);

            foreach (var value in values)
            {
                if (s.Contains(value, comparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Returns the first of the given values that the string contains.</summary>
        /// <param name="values">The candidates to look for, in priority order.</param>
        /// <returns>The first candidate found, or <see langword="null"/> when none is.</returns>
        public string? FirstContained(params ReadOnlySpan<string> values) => s.FirstContained(StringComparison.Ordinal, values);

        /// <summary>Returns the first of the given values that the string contains.</summary>
        /// <param name="comparison">How the candidates are matched.</param>
        /// <param name="values">The candidates to look for, in priority order.</param>
        /// <returns>The first candidate found, or <see langword="null"/> when none is.</returns>
        public string? FirstContained(StringComparison comparison, params ReadOnlySpan<string> values)
        {
            ArgumentNullException.ThrowIfNull(s);

            foreach (var value in values)
            {
                if (s.Contains(value, comparison))
                {
                    return value;
                }
            }

            return null;
        }

        /// <summary>Returns the first of the given values that the string contains.</summary>
        /// <param name="values">The candidates to look for, in priority order.</param>
        /// <param name="comparison">How the candidates are matched.</param>
        /// <returns>The first candidate found, or <see langword="null"/> when none is.</returns>
        public string? FirstContained(IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentNullException.ThrowIfNull(values);

            foreach (var value in values)
            {
                if (s.Contains(value, comparison))
                {
                    return value;
                }
            }

            return null;
        }

        // ------------------------------------------------------------------ splitting

        /// <summary>
        /// Splits on <paramref name="separator"/>, except where it appears between two <paramref name="quote"/> characters.
        /// </summary>
        /// <param name="separator">The separator character.</param>
        /// <param name="quote">The quote character that protects separators. Quotes are kept in the output.</param>
        /// <param name="options">Whether to trim entries or drop empty ones.</param>
        /// <returns>The pieces, lazily.</returns>
        public IEnumerable<string> SplitOutside(char separator, char quote, StringSplitOptions options = StringSplitOptions.None)
        {
            var q = quote.ToString();
            return s.SplitOutside([separator.ToString()], [(q, q)], options);
        }

        /// <summary>
        /// Splits on <paramref name="separator"/>, except where it appears inside <paramref name="enclosure"/>.
        /// </summary>
        /// <param name="separator">The separator text. Must not be empty.</param>
        /// <param name="enclosure">The opening and closing markers that protect separators. Markers are kept in the output.</param>
        /// <param name="options">Whether to trim entries or drop empty ones.</param>
        /// <returns>The pieces, lazily.</returns>
        public IEnumerable<string> SplitOutside(string separator, (string Open, string Close) enclosure, StringSplitOptions options = StringSplitOptions.None)
            => s.SplitOutside([separator], [enclosure], options);

        /// <summary>
        /// Splits on any of <paramref name="separators"/>, except where they appear inside one of the <paramref name="enclosures"/>.
        /// </summary>
        /// <remarks>
        /// Matching is ordinal. Longer separators win over shorter ones at the same position, so <c>"||"</c> beats <c>"|"</c>.
        /// An enclosure that is never closed runs to the end of the string. Enclosures do not nest and there is no escape
        /// syntax: the first closing marker after an opening one ends the enclosure.
        /// </remarks>
        /// <param name="separators">The separator texts. At least one is required and none may be empty.</param>
        /// <param name="enclosures">Opening and closing marker pairs that protect separators. Markers are kept in the output. None may be empty.</param>
        /// <param name="options">Whether to trim entries or drop empty ones.</param>
        /// <returns>The pieces, lazily.</returns>
        public IEnumerable<string> SplitOutside(
            IEnumerable<string> separators,
            IEnumerable<(string Open, string Close)> enclosures,
            StringSplitOptions options = StringSplitOptions.None)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentNullException.ThrowIfNull(separators);
            ArgumentNullException.ThrowIfNull(enclosures);

            var seps = separators.ToArray();
            if (seps.Length == 0)
            {
                throw new ArgumentException("At least one separator is required.", nameof(separators));
            }

            foreach (var sep in seps)
            {
                ArgumentException.ThrowIfNullOrEmpty(sep, nameof(separators));
            }

            var pairs = enclosures.ToArray();
            foreach (var (open, close) in pairs)
            {
                ArgumentException.ThrowIfNullOrEmpty(open, nameof(enclosures));
                ArgumentException.ThrowIfNullOrEmpty(close, nameof(enclosures));
            }

            Array.Sort(seps, static (a, b) => b.Length.CompareTo(a.Length));

            return Iterate();

            IEnumerable<string> Iterate()
            {
                var tokenStart = 0;
                var i = 0;
                while (i < s.Length)
                {
                    var closeAt = SkipEnclosure(i);
                    if (closeAt >= 0)
                    {
                        i = closeAt;
                        continue;
                    }

                    var sep = SeparatorAt(i);
                    if (sep is null)
                    {
                        i++;
                        continue;
                    }

                    if (TryPrepare(s.AsSpan(tokenStart, i - tokenStart), out var token))
                    {
                        yield return token;
                    }

                    i += sep.Length;
                    tokenStart = i;
                }

                if (TryPrepare(s.AsSpan(tokenStart), out var last))
                {
                    yield return last;
                }
            }

            int SkipEnclosure(int at)
            {
                foreach (var (open, close) in pairs)
                {
                    if (s.AsSpan(at).StartsWith(open, StringComparison.Ordinal))
                    {
                        var closeAt = s.IndexOf(close, at + open.Length, StringComparison.Ordinal);
                        return closeAt < 0 ? s.Length : closeAt + close.Length;
                    }
                }

                return -1;
            }

            string? SeparatorAt(int at)
            {
                foreach (var sep in seps)
                {
                    if (s.AsSpan(at).StartsWith(sep, StringComparison.Ordinal))
                    {
                        return sep;
                    }
                }

                return null;
            }

            bool TryPrepare(ReadOnlySpan<char> raw, out string token)
            {
                if (options.HasFlag(StringSplitOptions.TrimEntries))
                {
                    raw = raw.Trim();
                }

                if (raw.IsEmpty && options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
                {
                    token = string.Empty;
                    return false;
                }

                token = raw.ToString();
                return true;
            }
        }
    }
}
