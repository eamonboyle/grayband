namespace Grayband;

public sealed class GraybandOptions
{
    public double AcceptAt { get; init; } = 0.88;
    public double RejectBelow { get; init; } = 0.72;
    public IReadOnlyDictionary<string, double> Weights { get; init; } =
        new Dictionary<string, double>
        {
            ["artist"] = 0.45,
            ["title"] = 0.45,
            ["release"] = 0.10
        };
}
