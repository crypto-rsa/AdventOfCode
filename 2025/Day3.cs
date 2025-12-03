using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2025;

public class Day3 : IAdventDay
{
    public string Name => "3. 12. 2025";

    private static string[] GetInput() => File.ReadAllLines("2025/Resources/day3.txt");

    public string Solve()
    {
        long total = 0;

        foreach (string input in GetInput())
        {
            var firstDigit = input[..^1].Select((c, i) => (Index: i, Value: int.Parse(c.ToString()))).MaxBy(i => i.Value);
            var secondDigit = input[(firstDigit.Index + 1)..].Select(c => int.Parse(c.ToString())).Max();

            total += 10 * firstDigit.Value + secondDigit;
        }

        return total.ToString();
    }

    public string SolveAdvanced()
    {
        long total = 0;

        foreach (string input in GetInput())
        {
            long number = 0;
            int startIndex = 0;

            for (int remaining = 11; remaining >= 0; remaining--)
            {
                var digit = input[startIndex..^remaining].Select((c, i) => (Index: i, Value: int.Parse(c.ToString()))).MaxBy(i => i.Value);
                number = number * 10 + digit.Value;
                startIndex += digit.Index + 1;
            }

            total += number;
        }

        return total.ToString();
    }
}
