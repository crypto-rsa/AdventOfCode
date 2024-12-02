using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2024;

public class Day2 : IAdventDay
{
    public string Name => "2. 12. 2024";

    private static string[] GetInput() => File.ReadAllLines("2024/Resources/day2.txt");

    private static bool IsSafeReport(int[] levels)
    {
        if (levels.Length < 2)
            return false;

        bool isIncreasing = levels[1] > levels[0];

        for (int i = 1; i < levels.Length; i++)
        {
            int difference = isIncreasing
                ? levels[i] - levels[i - 1]
                : levels[i - 1] - levels[i];

            if (difference is not (>= 1 and <= 3))
                return false;
        }

        return true;
    }

    public string Solve()
    {
        return GetInput().Count(IsSafe).ToString();

        bool IsSafe(string report)
        {
            var levels = report.Split(' ').Select(int.Parse).ToArray();

            return IsSafeReport(levels);
        }
    }

    public string SolveAdvanced()
    {
        return GetInput().Count(IsSafe).ToString();

        bool IsSafe(string report)
        {
            var levels = report.Split(' ').Select(int.Parse).ToArray();

            return IsSafeReport(levels) || Enumerable.Range(0, levels.Length).Any(i => IsSafeReport(GetCopyWithoutElement(levels, i)));
        }

        int[] GetCopyWithoutElement(int[] array, int index)
        {
            var copy = new int[array.Length - 1];

            System.Array.Copy(array, 0, copy, 0, index);
            System.Array.Copy(array, index + 1, copy, index, array.Length - index - 1);

            return copy;
        }
    }
}
