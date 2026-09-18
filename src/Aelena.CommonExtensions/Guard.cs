using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Aelena.CommonExtensions;

/// <summary>
/// Argument validation that behaves identically on every target framework. Modern targets delegate to
/// the BCL throw helpers; .NET Standard 2.0 gets equivalent hand-written checks.
/// </summary>
internal static class Guard
{
    public static void NotNull([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
#if NETSTANDARD2_0
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
#else
        ArgumentNullException.ThrowIfNull(value, parameterName);
#endif
    }

    public static void NotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
#if NETSTANDARD2_0
        NotNull(value, parameterName);
        if (value.Length == 0)
        {
            throw new ArgumentException("The value cannot be an empty string.", parameterName);
        }
#else
        ArgumentException.ThrowIfNullOrEmpty(value, parameterName);
#endif
    }

    public static void Positive(int value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
#if NETSTANDARD2_0
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "The value must be positive.");
        }
#else
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, parameterName);
#endif
    }

    public static void NotNegative(int value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
#if NETSTANDARD2_0
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "The value must be non-negative.");
        }
#else
        ArgumentOutOfRangeException.ThrowIfNegative(value, parameterName);
#endif
    }
}
