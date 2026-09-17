#if NETSTANDARD2_0
using System.Text;
#endif

namespace Aelena.CommonExtensions;

/// <summary>
/// Thin wrappers over string APIs that .NET Standard 2.0 lacks. Modern targets call the BCL directly.
/// </summary>
internal static class Compat
{
    /// <summary>
    /// <c>StringSplitOptions.TrimEntries</c> has had the value 2 since .NET 5. .NET Standard 2.0 does not declare
    /// the member, so the flag is tested by value to give every target the same behaviour.
    /// </summary>
    public const StringSplitOptions TrimEntries = (StringSplitOptions)2;

    public static bool Contains(string s, string value, StringComparison comparison)
    {
#if NETSTANDARD2_0
        return s.IndexOf(value, comparison) >= 0;
#else
        return s.Contains(value, comparison);
#endif
    }

    public static string Replace(string s, string oldValue, string newValue, StringComparison comparison)
    {
#if NETSTANDARD2_0
        Guard.NotNullOrEmpty(oldValue);

        var builder = new StringBuilder();
        var position = 0;
        int index;
        while ((index = s.IndexOf(oldValue, position, comparison)) >= 0)
        {
            builder.Append(s, position, index - position).Append(newValue);
            position = index + oldValue.Length;
        }

        return position == 0 ? s : builder.Append(s, position, s.Length - position).ToString();
#else
        return s.Replace(oldValue, newValue, comparison);
#endif
    }

    public static string Concat(ReadOnlySpan<char> first, string second, ReadOnlySpan<char> third)
    {
#if NETSTANDARD2_0
        return string.Concat(first.ToString(), second, third.ToString());
#else
        return string.Concat(first, second, third);
#endif
    }

    public static string Concat(ReadOnlySpan<char> first, string second)
    {
#if NETSTANDARD2_0
        return string.Concat(first.ToString(), second);
#else
        return string.Concat(first, second);
#endif
    }
}
