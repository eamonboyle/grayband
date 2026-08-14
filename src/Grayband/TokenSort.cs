namespace Grayband;

public static class TokenSort
{
    public static double Similarity(string a, string b)
    {
        var left = string.Concat(Normalize.Words(a).OrderBy(static w => w, StringComparer.Ordinal));
        var right = string.Concat(Normalize.Words(b).OrderBy(static w => w, StringComparer.Ordinal));
        return JaroWinkler.Similarity(left, right);
    }
}
