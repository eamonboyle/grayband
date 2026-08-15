using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Grayband;

public static partial class Normalize
{
    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonWord();

    public static string Lookup(string value) => string.Concat(Words(value));

    public static IReadOnlyList<string> Words(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return [];

        var words = NonWord().Split(Fold(value))
            .Where(static w => w.Length > 0)
            .ToList();

        if (words.Count > 1 && IsLeadingArticle(words[0]))
            words.RemoveAt(0);

        return words;
    }

    private static bool IsLeadingArticle(string word) => word is "the" or "a" or "an";

    private static string Fold(string value)
    {
        var folded = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(folded.Length);
        foreach (var c in folded)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;
            sb.Append(char.ToLowerInvariant(c));
        }

        return sb.ToString();
    }
}
