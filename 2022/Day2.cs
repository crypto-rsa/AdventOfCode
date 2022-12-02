using System;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day2 : IAdventDay
{
    public string Name => "2. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day2.txt");

    private static int GetScore(string game)
    {
        int score = game switch
        {
            "A Y" or "B Z" or "C X" => 6,
            "A X" or "B Y" or "C Z" => 3,
            "A Z" or "B X" or "C Y" => 0,
            _ => throw new InvalidOperationException(),
        };

        score += game[2] switch
        {
            'X' => 1,
            'Y' => 2,
            'Z' => 3,
            _ => throw new InvalidOperationException(),
        };

        return score;
    }

    private static string Transform(string game) => game switch
    {
        "A X" => "A Z",
        "B X" => "B X",
        "C X" => "C Y",
        "A Y" => "A X",
        "B Y" => "B Y",
        "C Y" => "C Z",
        "A Z" => "A Y",
        "B Z" => "B Z",
        "C Z" => "C X",
        _ => throw new InvalidOperationException(),
    };

    public string Solve() => GetInput()
        .SplitToLines()
        .Select(GetScore)
        .Sum()
        .ToString();

    public string SolveAdvanced() => GetInput()
        .SplitToLines()
        .Select(Transform)
        .Select(GetScore)
        .Sum()
        .ToString();
}
