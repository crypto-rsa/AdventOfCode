using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day19 : IAdventDay
{
    public string Name => "19. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day19.txt");

    private static long GetWaysToReachEnd(HashSet<string> patterns, string input)
    {
        var matches = patterns.SelectMany(pattern => GetMatches(input, pattern)).ToList();
        var waysToReachFromStart = new long[input.Length];

        matches.Sort(Compare);

        foreach (var match in matches)
        {
            if (match.Start == 0)
            {
                waysToReachFromStart[match.End]++;
            }
            else
            {
                waysToReachFromStart[match.End] += waysToReachFromStart[match.Start - 1];
            }
        }

        return waysToReachFromStart[^1];

        static IEnumerable<(int Start, int End)> GetMatches(string input, string pattern)
        {
            int position = 0;

            while (input.IndexOf(pattern, position, StringComparison.Ordinal) is var index && index >= 0)
            {
                yield return (index, index + pattern.Length - 1);

                position = index + 1;
            }
        }

        static int Compare((int Start, int End) item1, (int Start, int End) item2)
        {
            if (item1.End < item2.Start)
                return -1;

            if (item2.End < item1.Start)
                return +1;

            return item1.Start.CompareTo(item2.Start);
        }
    }

    public string Solve()
    {
        var parts = GetInput().ParseByBlankLines().ToArray();
        var patterns = parts[0].Single().Split(", ").ToHashSet();
        var designs = parts[1].ToArray();

        return designs.Count(s => GetWaysToReachEnd(patterns, s) > 0).ToString();
    }

    public string SolveAdvanced()
    {
        var parts = GetInput().ParseByBlankLines().ToArray();
        var patterns = parts[0].Single().Split(", ").ToHashSet();
        var designs = parts[1].ToArray();

        return designs.Sum(s => GetWaysToReachEnd(patterns, s)).ToString();
    }
}
