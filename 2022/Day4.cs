using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day4 : IAdventDay
{
    public string Name => "4. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day4.txt");

    private static ((int Start, int End) First, (int Start, int End) Second) Parse(string input)
    {
        var match = System.Text.RegularExpressions.Regex.Match(input, @"(\d+)-(\d+),(\d+)-(\d+)");

        return ((Value(1), Value(2)), (Value(3), Value(4)));

        int Value(int index) => int.Parse(match.Groups[index].Value);
    }

    private static bool IsContained(((int Start, int End) First, (int Start, int End) Second) item)
    {
        return IsContainedInternal(item.First, item.Second) || IsContainedInternal(item.Second, item.First);
        
        bool IsContainedInternal((int Start, int End) bigger, (int Start, int End) smaller) => bigger.Start <= smaller.Start && bigger.End >= smaller.End;
    }

    private static bool IsOverlapping(((int Start, int End) First, (int Start, int End) Second) item)
    {
        return IsOverlappingInternal(item.First, item.Second) || IsOverlappingInternal(item.Second, item.First);

        bool IsOverlappingInternal((int Start, int End) first, (int Start, int End) second) => first.Start <= second.Start && second.Start <= first.End;
    }
    
    public string Solve() => GetInput()
        .SplitToLines()
        .Select(Parse)
        .Count(IsContained)
        .ToString();

    public string SolveAdvanced() => GetInput()
        .SplitToLines()
        .Select(Parse)
        .Count(IsOverlapping)
        .ToString();
}
