using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2025;

public class Day2 : IAdventDay
{
    public string Name => "2. 12. 2025";

    private static string GetInput() => File.ReadAllText("2025/Resources/day2.txt");

    private static int GetLength(long number)
    {
        int length = 0;

        while (number > 0)
        {
            length++;
            number /= 10;
        }

        return length;
    }

    public string Solve()
    {
        long total = 0;

        foreach (string item in GetInput().Split(','))
        {
            var parts = item.Split('-');
            long lowerBound = long.Parse(parts[0]);
            long upperBound = long.Parse(parts[1]);
            int startPrefixLength = (parts[0].Length + 1) / 2;

            long startPart = parts[0].Length % 2 == 0
                ? long.Parse(parts[0][..startPrefixLength])
                : (long)Math.Pow(10, startPrefixLength - 1);

            while (true)
            {
                int length = GetLength(startPart);

                long candidate = startPart;

                for (int i = 0; i < length; i++)
                {
                    candidate *= 10;
                }

                candidate += startPart;

                if (candidate < lowerBound)
                {
                    startPart++;

                    continue;
                }

                if (candidate > upperBound)
                    break;

                total += candidate;
                startPart++;
            }
        }

        return total.ToString();
    }

    public string SolveAdvanced()
    {
        var invalidIDs = new HashSet<long>();

        foreach (string item in GetInput().Split(','))
        {
            var parts = item.Split('-');
            long lowerBound = long.Parse(parts[0]);
            long upperBound = long.Parse(parts[1]);

            int lowerBoundLength = GetLength(lowerBound);
            int upperBoundLength = GetLength(upperBound);

            for (int totalLength = lowerBoundLength; totalLength <= upperBoundLength; totalLength++)
            {
                long startNumber = totalLength == lowerBoundLength
                    ? lowerBound
                    : (long)Math.Pow(10, totalLength - 1);

                for (int prefixLength = 1; prefixLength < totalLength; prefixLength++)
                {
                    long count = Math.DivRem(totalLength, prefixLength, out long remainder);

                    if (remainder != 0)
                        continue;

                    long startPrefix = startNumber / (long)Math.Pow(10, totalLength - prefixLength);

                    while (true)
                    {
                        if (GetLength(startPrefix) > prefixLength)
                            break;

                        long candidate = startPrefix;

                        for (int i = 0; i < count - 1; i++)
                        {
                            candidate *= (long)Math.Pow(10, prefixLength);
                            candidate += startPrefix;
                        }

                        if (candidate < lowerBound)
                        {
                            startPrefix++;

                            continue;
                        }

                        if (candidate > upperBound)
                            break;

                        invalidIDs.Add(candidate);
                        startPrefix++;
                    }
                }
            }
        }

        return invalidIDs.Sum().ToString();
    }
}
