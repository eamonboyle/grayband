using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Grayband;

public static partial class Normalize
{
    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonWord();

    public static string Lookup(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        // Strip leading articles first (before normalization)
        var stripped = value.Trim().ToLowerInvariant();
        foreach (var article in new[] { "the ", "a ", "an " })
        {
            if (stripped.StartsWith(article))
            {
                stripped = stripped.Substring(article.Length).Trim();
                break;
            }
        }

        var folded = stripped.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(folded.Length);
        foreach (var c in folded)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;
            sb.Append(char.ToLowerInvariant(c));
        }

        return NonWord().Replace(sb.ToString(), "");
    }

    public static IReadOnlyList<string> Words(string value) =>
        NonWord().Split(value)
            .Select(Lookup)
            .Where(static w => w.Length > 0)
            .ToArray();
}
