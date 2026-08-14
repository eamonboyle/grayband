namespace Grayband;

public sealed record Record(
    string Artist,
    string Title,
    string? Release = null,
    int? Year = null,
    string? Id = null);
