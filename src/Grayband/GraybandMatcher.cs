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
        throw new NotImplementedException("Weekend 1: implement deterministic Score");
    }

    public Task<MatchDecision> ScoreAsync(Record left, Record right, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Weekend 1: implement ScoreAsync (model only in the gray band)");
    }
}
