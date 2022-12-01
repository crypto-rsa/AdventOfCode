using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day1 : IAdventDay
{
    public string Name => "1. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day1.txt");

    public string Solve() => GetInput()
        .ParseByBlankLines()
        .Select(s => s.ParseIntegers().Sum())
        .Max()
        .ToString();

    public string SolveAdvanced() => GetInput()
        .ParseByBlankLines()
        .Select(s => s.ParseIntegers().Sum())
        .OrderByDescending(i => i)
        .Take(3)
        .Sum()
        .ToString();
}
