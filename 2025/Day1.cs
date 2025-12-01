using System;
using System.Collections.Generic;
using System.IO;

namespace Advent_of_Code.Year2025;

public class Day1 : IAdventDay
{
    public string Name => "1. 12. 2025";

    private static string[] GetInput() => File.ReadAllLines("2025/Resources/day1.txt");

    private const int DialSize = 100;

    private IEnumerable<int> GetOffsets()
    {
        foreach (var line in GetInput())
        {
            yield return line[0] switch
            {
                'L' => -int.Parse(line[1..]),
                'R' => +int.Parse(line[1..]),
                _ => 0
            };
        }
    }

    public string Solve()
    {
        int result = 0;
        int current = 50;

        foreach (int offset in GetOffsets())
        {
            current = (current + DialSize + offset) % DialSize;

            if (current is 0)
            {
                result++;
            }
        }

        return result.ToString();
    }

    public string SolveAdvanced()
    {
        int result = 0;
        int current = 50;

        foreach (int offset in GetOffsets())
        {
            int fullRotations = Math.DivRem(Math.Abs(offset), DialSize, out var remainder);
            int nextStop = current + (offset >= 0 ? remainder : -remainder);

            result += fullRotations;

            if ((current, nextStop) is (_, >= DialSize) or (> 0, <= 0))
            {
                result++;
            }

            current = (nextStop + DialSize) % DialSize;
        }

        return result.ToString();
    }
}
