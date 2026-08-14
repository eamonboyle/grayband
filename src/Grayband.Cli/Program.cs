using Grayband;

if (args.Length < 4)
{
    Console.WriteLine("grayband <artist-a> <title-a> <artist-b> <title-b>");
    return 1;
}

var matcher = new GraybandMatcher();
var decision = matcher.Score(
    new Record(args[0], args[1]),
    new Record(args[2], args[3]));

Console.WriteLine($"{decision.Kind}  {decision.Score:0.000}");
foreach (var field in decision.Breakdown)
    Console.WriteLine($"  {field.Field,-8} {field.Score:0.00}  {field.Because}");
return 0;
