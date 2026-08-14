using Grayband;

namespace Grayband.Tests;

public class MatcherTests
{
    private readonly GraybandMatcher _matcher = new();

    [Fact]
    public void Exact_normalized_pair_accepts()
    {
        var decision = _matcher.Score(
            new Record("The Beatles", "Hey Jude"),
            new Record("beatles", "hey jude"));

        Assert.Equal(DecisionKind.Accept, decision.Kind);
        Assert.Equal(1, decision.Breakdown.Single(f => f.Field == "artist").Score);
        Assert.False(decision.UsedModel);
    }

    [Fact]
    public void Feat_and_live_suffix_is_explained()
    {
        var decision = _matcher.Score(
            new Record("Daft Punk", "Get Lucky"),
            new Record("Daft Punk feat. Pharrell Williams", "Get Lucky (Live)"));

        Assert.NotEqual(DecisionKind.Reject, decision.Kind);
        Assert.All(decision.Breakdown, f => Assert.False(string.IsNullOrWhiteSpace(f.Because)));
    }

    [Fact]
    public void Unrelated_pair_rejects()
    {
        var decision = _matcher.Score(
            new Record("Radiohead", "Karma Police"),
            new Record("ABBA", "Dancing Queen"));

        Assert.Equal(DecisionKind.Reject, decision.Kind);
    }

    [Fact]
    public async Task Model_is_skipped_outside_the_gray_band()
    {
        var model = new StubModel(DecisionKind.Accept, "should not run");
        var matcher = new GraybandMatcher(model: model);

        var reject = await matcher.ScoreAsync(
            new Record("Radiohead", "Karma Police"),
            new Record("ABBA", "Dancing Queen"));

        Assert.Equal(DecisionKind.Reject, reject.Kind);
        Assert.False(reject.UsedModel);
        Assert.Equal(0, model.Calls);
    }

    [Fact]
    public async Task Model_may_flip_only_a_gray_decision()
    {
        var model = new StubModel(DecisionKind.Accept, "same song, extra featuring credit");
        var options = new GraybandOptions { RejectBelow = 0.99, AcceptAt = 1.0 };
        var matcher = new GraybandMatcher(options, model);

        var decision = await matcher.ScoreAsync(
            new Record("Daft Punk", "Get Lucky"),
            new Record("Daft Punk feat. Pharrell Williams", "Get Lucky (Radio Edit)"));

        Assert.Equal(1, model.Calls);
        Assert.True(decision.UsedModel);
        Assert.Equal(DecisionKind.Accept, decision.Kind);
        Assert.True(decision.Llm!.ChangedDecision);
    }

    private sealed class StubModel(DecisionKind suggested, string because) : IGraybandModel
    {
        public int Calls { get; private set; }

        public Task<LlmCall> DecideAsync(Record left, Record right, MatchDecision deterministic, CancellationToken cancellationToken = default)
        {
            Calls++;
            return Task.FromResult(new LlmCall(suggested, "stub", because, ChangedDecision: false));
        }
    }
}
