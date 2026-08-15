namespace Grayband;

public static class JaroWinkler
{
    public static double Similarity(string a, string b)
    {
        if (a.Length == 0 && b.Length == 0)
            return 1;
        if (a.Length == 0 || b.Length == 0)
            return 0;
        if (a == b)
            return 1;

        var jaro = Jaro(a, b);
        var prefix = 0;
        var maxPrefix = Math.Min(4, Math.Min(a.Length, b.Length));
        while (prefix < maxPrefix && a[prefix] == b[prefix])
            prefix++;

        return jaro + (prefix * 0.1 * (1 - jaro));
    }

    private static double Jaro(string a, string b)
    {
        var matchDistance = Math.Max(0, Math.Max(a.Length, b.Length) / 2 - 1);
        var aMatches = new bool[a.Length];
        var bMatches = new bool[b.Length];

        var matches = 0;
        for (var i = 0; i < a.Length; i++)
        {
            var start = Math.Max(0, i - matchDistance);
            var end = Math.Min(i + matchDistance + 1, b.Length);

            for (var j = start; j < end; j++)
            {
                if (bMatches[j] || a[i] != b[j])
                    continue;

                aMatches[i] = true;
                bMatches[j] = true;
                matches++;
                break;
            }
        }

        if (matches == 0)
            return 0;

        var transpositions = 0;
        var k = 0;
        for (var i = 0; i < a.Length; i++)
        {
            if (!aMatches[i])
                continue;

            while (!bMatches[k])
                k++;

            if (a[i] != b[k])
                transpositions++;

            k++;
        }

        return (matches / (double)a.Length + matches / (double)b.Length + (matches - transpositions / 2.0) / matches) / 3.0;
    }
}
