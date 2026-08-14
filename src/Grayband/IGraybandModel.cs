namespace Grayband;

public interface IGraybandModel
{
    Task<LlmCall> DecideAsync(
        Record left,
        Record right,
        MatchDecision deterministic,
        CancellationToken cancellationToken = default);
}
