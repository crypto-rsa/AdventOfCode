using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2024;

public class Day11 : IAdventDay
{
    public string Name => "11. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day11.txt");

    private static long GetSequenceLength(int iterations)
    {
        var numberCounts = GetInput().Split(' ').Select(long.Parse).ToDictionary(n => n, _ => 1L);

        for (int iteration = 1; iteration <= iterations; iteration++)
        {
            var items = numberCounts.Where(p => p.Value > 0).ToArray();

            foreach ((long number, long count) in items)
            {
                numberCounts[number] -= count;

                if (number == 0)
                {
                    AddOrIncrement(1, count);
                }
                else if (HasEvenDigits(number, out long left, out long right))
                {
                    AddOrIncrement(left, count);
                    AddOrIncrement(right, count);
                }
                else
                {
                    AddOrIncrement(number * 2024, count);
                }
            }
        }

        return numberCounts.Select(p => p.Value).Sum();

        void AddOrIncrement(long n, long count)
        {
            if (!numberCounts.TryAdd(n, count))
            {
                numberCounts[n] += count;
            }
        }

        bool HasEvenDigits(long number, out long left, out long right)
        {
            long n = number;
            long digits = 0;
            long divisor = 1;

            while (n > 0)
            {
                digits++;
                n /= 10;

                if (digits % 2 == 0)
                {
                    divisor *= 10;
                }
            }

            if (digits % 2 != 0)
            {
                left = right = 0;

                return false;
            }

            left = number / divisor;
            right = number % divisor;

            return true;
        }
    }

    public string Solve() => GetSequenceLength(iterations: 25).ToString();

    public string SolveAdvanced() => GetSequenceLength(iterations: 75).ToString();
}
