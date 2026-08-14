namespace Grayband;

public enum DecisionKind
{
    Accept,
    Reject,
    Gray
}

public sealed record FieldScore(
    string Field,
    double Score,
    string Method,
    string Because);

public sealed record LlmCall(
    DecisionKind Suggested,
    string Model,
    string Because,
    bool ChangedDecision);

public sealed record MatchDecision(
    double Score,
    DecisionKind Kind,
    IReadOnlyList<FieldScore> Breakdown,
    LlmCall? Llm = null)
{
    public bool UsedModel => Llm is not null;
}
