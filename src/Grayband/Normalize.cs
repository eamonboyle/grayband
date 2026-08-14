using System.Text.RegularExpressions;

namespace Grayband;

public static partial class Normalize
{
    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonWord();

    public static string Lookup(string value)
    {
        throw new NotImplementedException("Weekend 1: implement Normalize.Lookup");
    }

    public static IReadOnlyList<string> Words(string value)
    {
        throw new NotImplementedException("Weekend 1: implement Normalize.Words");
    }
}
