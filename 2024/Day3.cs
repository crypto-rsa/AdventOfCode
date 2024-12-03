using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2024;

public class Day3 : IAdventDay
{
    public string Name => "3. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day3.txt");

    private const string MultiplicationPattern = @"mul\((\d+),(\d+)\)";

    private int Parse(Match match) => int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);

    public string Solve() => Regex.Matches(GetInput(), MultiplicationPattern)
        .Select(Parse)
        .Sum()
        .ToString();

    public string SolveAdvanced()
    {
        var input = GetInput();
        int position = 0;
        int sum = 0;
        bool enabled = true;

        while (position < input.Length)
        {
            if (enabled)
            {
                int nextDisabled = input.IndexOf("don't()", position, StringComparison.Ordinal);

                if (nextDisabled == -1)
                {
                    nextDisabled = input.Length;
                }

                sum += Regex.Matches(input[position..nextDisabled], MultiplicationPattern)
                    .Select(Parse)
                    .Sum();

                position = nextDisabled;
                enabled = false;
            }
            else
            {
                int nextEnabled = input.IndexOf("do()", position, StringComparison.Ordinal);

                if (nextEnabled == -1)
                {
                    nextEnabled = input.Length;
                }

                position = nextEnabled;
                enabled = true;
            }
        }

        return sum.ToString();
    }
}
