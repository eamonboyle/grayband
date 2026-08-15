namespace Grayband;

public static class TokenSort
{
    // Extra featuring credits or mix suffixes are a strong match, not an exact one.
    private const double ExtraTokenPenalty = 0.001;

    public static double Similarity(string a, string b)
    {
        var leftWords = Normalize.Words(a);
        var rightWords = Normalize.Words(b);

        var left = string.Concat(leftWords.OrderBy(static w => w, StringComparer.Ordinal));
        var right = string.Concat(rightWords.OrderBy(static w => w, StringComparer.Ordinal));
        var sorted = JaroWinkler.Similarity(left, right);

        var leftSet = leftWords.ToHashSet(StringComparer.Ordinal);
        var rightSet = rightWords.ToHashSet(StringComparer.Ordinal);
        if (leftSet.Count == 0 || rightSet.Count == 0)
            return sorted;

        if (leftSet.SetEquals(rightSet))
            return 1;

        if (leftSet.IsSubsetOf(rightSet) || rightSet.IsSubsetOf(leftSet))
        {
            var extra = Math.Abs(leftSet.Count - rightSet.Count);
            return Math.Max(sorted, Math.Max(0, 1.0 - ExtraTokenPenalty * extra));
        }

        return sorted;
    }
}
