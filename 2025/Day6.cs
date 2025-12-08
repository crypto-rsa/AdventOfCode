using System;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2025;

public class Day6 : IAdventDay
{
    public string Name => "6. 12. 2025";

    private static string[] GetInput() => File.ReadAllLines("2025/Resources/day6.txt");

    public string Solve()
    {
        long total = 0;
        var parts = GetInput().Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToArray();
        int problemCount = parts.Select(a => a.Length).Distinct().Single();

        for (int i = 0; i < problemCount; i++)
        {
            if (parts.Last()[i] == "+")
            {
                total += parts.Take(parts.Length - 1).Sum(p => long.Parse(p[i]));
            }
            else if (parts.Last()[i] == "*")
            {
                total += parts.Take(parts.Length - 1).Select(p => long.Parse(p[i])).Aggregate(1L, (a, b) => a * b);
            }
        }

        return total.ToString();
    }

    public string SolveAdvanced()
    {
        long total = 0;
        var parts = GetInput();
        var operators = parts.Last();

        int prevPosition = operators.Length - 1;

        for (int position = operators.Length - 1; position >= 0; position--)
        {
            if (operators[position] is ' ')
                continue;

            long value = operators[position] is '+' ? 0 : 1;

            for (int i = prevPosition; i >= position; i--)
            {
                long number = 0;

                for (int j = 0; j < parts.Length - 1; j++)
                {
                    if (parts[j][i] is ' ')
                        continue;

                    number = 10 * number + long.Parse(parts[j][i].ToString());
                }

                if (operators[position] is '+')
                    value += number;
                else
                    value *= number;
            }

            total += value;

            prevPosition = position - 2;
        }

        return total.ToString();
    }
}
