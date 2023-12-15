using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day9 : IAdventDay
{
    public string Name => "9. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day9.txt");

    private long Extrapolate(IEnumerable<int> items, bool returnLast)
    {
        var differences = new List<long[]> { items.Select(i => (long)i).ToArray() };

        bool allZeros;

        do
        {
            var prev = differences.Last();
            var cur = new long[prev.Length - 1];

            allZeros = true;

            for (int i = 0; i < cur.Length; i++)
            {
                cur[i] = prev[i + 1] - prev[i];
                allZeros &= cur[i] == 0;
            }

            differences.Add(cur);
        } while (!allZeros);

        return returnLast
            ? differences.Aggregate(0L, (cur, next) => next.Last() + cur)
            : differences.Reverse<long[]>().Aggregate(0L, (cur, next) => next.First() - cur);
    }

    public string Solve() => GetInput()
        .SplitToLines()
        .ParseIntegerSequences()
        .Sum(i => Extrapolate(i, returnLast: true))
        .ToString();

    public string SolveAdvanced() => GetInput()
        .SplitToLines()
        .ParseIntegerSequences()
        .Sum(i => Extrapolate(i, returnLast: false))
        .ToString();
}
