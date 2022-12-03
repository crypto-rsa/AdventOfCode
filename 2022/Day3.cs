using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day3 : IAdventDay
{
    public string Name => "3. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day3.txt");

    private const string Priorities = ".abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private static char GetSharedItem(string input)
    {
        int half = input.Length / 2;

        return input[..half].Intersect(input[half..]).Single();
    }

    private static char GetSharedItem(IEnumerable<string> lines)
    {
        var array = lines.ToArray();

        return array[0].Intersect(array[1]).Intersect(array[2]).Single();
    }

    public string Solve() => GetInput()
        .SplitToLines()
        .Select(s => Priorities.IndexOf(GetSharedItem(s)))
        .Sum()
        .ToString();

    public string SolveAdvanced() => GetInput()
        .SplitToLines()
        .Chunk(3)
        .Select(i => Priorities.IndexOf(GetSharedItem(i)))
        .Sum()
        .ToString();
}
