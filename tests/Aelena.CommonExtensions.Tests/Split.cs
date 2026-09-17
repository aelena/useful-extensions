namespace Aelena.CommonExtensions.Tests;

/// <summary>
/// <c>StringSplitOptions.TrimEntries</c> is not declared on .NET Framework, so the tests use this constant,
/// which has the same value (2) everywhere.
/// </summary>
internal static class Split
{
#if NETFRAMEWORK
    public const StringSplitOptions TrimEntries = (StringSplitOptions)2;
#else
    public const StringSplitOptions TrimEntries = StringSplitOptions.TrimEntries;
#endif
}
