# Grayband

A .NET library for record matching. Each pair gets a deterministic weighted score and a field-level explanation. An optional model is only consulted when the score falls in a configured gray band.

```bash
dotnet test
dotnet run --project src/Grayband.Cli -- "Daft Punk" "Get Lucky" "Daft Punk feat. Pharrell Williams" "Get Lucky (Live)"
```

## How it works

1. Normalize artist, title, and optional release fields.
2. Score each field with Jaro-Winkler and token-sort.
3. Combine the field scores with configurable weights.
4. Map the total to Accept, Gray, or Reject.
5. If the result is Gray and a model is configured, the model may change Accept or Reject. It is not called otherwise.

## Layout

```
src/Grayband/          library
src/Grayband.Cli/      grayband <artist-a> <title-a> <artist-b> <title-b>
tests/Grayband.Tests/
fixtures/pairs.csv
```

## License

MIT
