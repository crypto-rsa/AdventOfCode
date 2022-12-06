using System.IO;

namespace Advent_of_Code.Year2022;

public class Day6 : IAdventDay
{
    public string Name => "6. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day6.txt");

    private static int GetLengthToMarkerEnd(int patternLength)
    {
        var input = GetInput();

        int start = 0;
        int end = 0;

        while (end - start + 1 < patternLength)
        {
            end++;

            if (end >= input.Length)
                return -1;

            for (int p = end - 1; p >= start; p--)
            {
                if (input[p] == input[end])
                {
                    start = p + 1;
                }
            }
        }

        return end + 1;
    }

    public string Solve() => GetLengthToMarkerEnd(patternLength: 4).ToString();

    public string SolveAdvanced() => GetLengthToMarkerEnd(patternLength: 14).ToString();
}
