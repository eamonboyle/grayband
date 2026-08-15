using Grayband;
using Xunit;

namespace Grayband.Tests;

public class NormalizeTests
{
    [Theory]
    [InlineData("The Beatles", "beatles")]
    [InlineData("Beyoncé", "beyonce")]
    [InlineData("Hey Jude!", "heyjude")]
    public void Lookup_folds_punctuation_accents_and_one_leading_article(string value, string expected)
    {
        Assert.Equal(expected, Normalize.Lookup(value));
    }

    [Fact]
    public void Words_drops_at_most_one_leading_article()
    {
        Assert.DoesNotContain("the", Normalize.Words("The Beatles"));
        Assert.Equal(["beatles"], Normalize.Words("The Beatles"));
        Assert.Equal(["the"], Normalize.Words("The The"));
        Assert.Equal("the", Normalize.Lookup("The The"));
        Assert.Equal(["the"], Normalize.Words("The"));
    }
}

public class JaroWinklerTests
{
    [Fact]
    public void Similarity_matches_known_values()
    {
        Assert.Equal(1, JaroWinkler.Similarity("martha", "martha"));
        Assert.Equal(1, JaroWinkler.Similarity("a", "a"));
        Assert.Equal(1, JaroWinkler.Similarity("", ""));
        Assert.Equal(0, JaroWinkler.Similarity("", "x"));
        Assert.Equal(0.961, JaroWinkler.Similarity("martha", "marhta"), 3);
        Assert.Equal(0.84, JaroWinkler.Similarity("dwayne", "duane"), 2);
    }
}

public class ReleaseScoringTests
{
    [Fact]
    public void One_sided_release_is_missing_with_score_zero()
    {
        var matcher = new GraybandMatcher();
        var decision = matcher.Score(
            new Record("The Beatles", "Hey Jude", Release: "Hey Jude"),
            new Record("The Beatles", "Hey Jude"));

        var release = Assert.Single(decision.Breakdown, f => f.Field == "release");
        Assert.Equal("missing", release.Method);
        Assert.Equal(0, release.Score);
    }
}
