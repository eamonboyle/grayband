namespace Grayband;

public sealed class GraybandMatcher
{
    private readonly GraybandOptions _options;
    private readonly IGraybandModel? _model;

    public GraybandMatcher(GraybandOptions? options = null, IGraybandModel? model = null)
    {
        _options = options ?? new GraybandOptions();
        _model = model;
        if (_options.RejectBelow > _options.AcceptAt)
            throw new ArgumentException("RejectBelow must be <= AcceptAt.");
    }

    public MatchDecision Score(Record left, Record right)
    {
        // breakdown of the decision for debugging and testing
        var breakdown = new List<FieldScore>
        {
            ScoreField("artist", left.Artist, right.Artist),
            ScoreField("title", left.Title, right.Title)
        };

        // compare release if available
        if (!string.IsNullOrWhiteSpace(left.Release) && !string.IsNullOrWhiteSpace(right.Release))
            breakdown.Add(ScoreField("release", left.Release ?? "", right.Release ?? ""));

        var score = Weighted(breakdown);
        var kind = score >= _options.AcceptAt ? DecisionKind.Accept
            : score < _options.RejectBelow ? DecisionKind.Reject
            : DecisionKind.Gray;

        return new MatchDecision(Math.Round(score, 4), kind, breakdown);
    }

    public async Task<MatchDecision> ScoreAsync(Record left, Record right, CancellationToken cancellationToken = default)
    {
        var first = Score(left, right);
        if (first.Kind != DecisionKind.Gray || _model is null)
            return first;

        var suggestion = await _model.DecideAsync(left, right, first, cancellationToken).ConfigureAwait(false);
        var changed = suggestion.Suggested != first.Kind
            && suggestion.Suggested is DecisionKind.Accept or DecisionKind.Reject;

        return first with
        {
            Kind = changed ? suggestion.Suggested : first.Kind,
            Llm = suggestion with { ChangedDecision = changed }
        };
    }

    private FieldScore ScoreField(string field, string left, string right)
    {
        var a = Normalize.Lookup(left);
        var b = Normalize.Lookup(right);

        if (a.Length == 0 && b.Length == 0)
            return new FieldScore(field, 1, "empty", "both empty");
        if (a.Length == 0 || b.Length == 0)
            return new FieldScore(field, 0, "missing", "one empty");
        if (a == b)
            return new FieldScore(field, 1, "exact", "exact match");

        var jw = JaroWinkler.Similarity(a, b);
        var token = TokenSort.Similarity(left, right);
        var best = Math.Max(jw, token);
        var method = token > jw ? "token-sort" : "jaro-winkler";
        var because = $"{method} {best:0.00} on '{left}' vs '{right}'";
        
        return new FieldScore(field, Math.Round(best, 4), method, because);
    }

    private double Weighted(IReadOnlyList<FieldScore> breakdown)
    {
        var present = breakdown.Where(f => _options.Weights.ContainsKey(f.Field)).ToArray();
        var weightSum = present.Sum(f => _options.Weights[f.Field]);
        if (weightSum == 0)
            return 0;

        return present.Sum(f => f.Score * _options.Weights[f.Field]) / weightSum;
    }
}
