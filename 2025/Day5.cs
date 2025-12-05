using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2025;

public class Day5 : IAdventDay
{
    public string Name => "5. 12. 2025";

    private static string GetInput() => File.ReadAllText("2025/Resources/day5.txt");

    private record Range(long Min, long Max);

    public string Solve()
    {
        int total = 0;
        var parts = GetInput().ParseByBlankLines().ToArray();
        var freshRanges = parts[0].Select(ParseRange).ToArray();

        foreach (string ingredient in parts[1])
        {
            var value = long.Parse(ingredient);

            if (freshRanges.Any(r => r.Min <= value && value <= r.Max))
            {
                total++;
            }
        }

        return total.ToString();
    }

    private Range ParseRange(string line)
    {
        var limits = line.Split('-');

        return new Range(long.Parse(limits[0]), long.Parse(limits[1]));
    }

    public string SolveAdvanced()
    {
        long total = 0;
        var freshRanges = GetInput().ParseByBlankLines().First().Select(ParseRange).OrderBy(r => r.Min).ToArray();
        var consolidated = new List<Range>();
        Range? current = null;

        foreach (Range range in freshRanges)
        {
            if (current is null)
            {
                current = range;

                continue;
            }

            if (range.Min > current.Max)
            {
                consolidated.Add(current);
                current = range;
            }
            else
            {
                current = current with { Max = System.Math.Max(range.Max, current.Max) };
            }
        }

        if (current != null)
        {
            consolidated.Add(current);
        }

        foreach (var range in consolidated)
        {
            total += range.Max - range.Min + 1;
        }

        return total.ToString();
    }
}
