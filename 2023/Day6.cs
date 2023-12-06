using System;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day6 : IAdventDay
{
    public string Name => "6. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day6.txt");

    static long GetWinningOptionsCount(long time, long bestDistance)
    {
        // the race distance is given by c * (t - c) where 'c' is the charge time and 't' is the total race time
        // therefore, in order to win the race, the following condition must hold: c * (t - c) > b, where 'b' is the best distance so far
        // thus:
        // ct - c^2 - b > 0
        // D = t^2 - 4b
        // c = (-t +- sqrt(D)) / (-2) = (t +- sqrt(D)) / 2
        double d = time * time - 4 * bestDistance;
        long c1 = (long)Math.Ceiling((time - Math.Sqrt(d)) / 2);
        long c2 = (long)Math.Floor((time + Math.Sqrt(d)) / 2);

        return (c2 - c1) + 1;
    }

    public string Solve()
    {
        var numbers = GetInput().SplitToLines().Select(GetNumbers).ToArray();
        var races = numbers[0].Zip(numbers[1], (t, d) => (Time: t, Distance: d)).ToArray();

        return races
            .Aggregate(1L, (current, race) => current * GetWinningOptionsCount(race.Time, race.Distance))
            .ToString();

        static long[] GetNumbers(string input) => input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToArray();
    }

    public string SolveAdvanced()
    {
        var numbers = GetInput().SplitToLines().Select(s => long.Parse(s.Replace(" ", string.Empty).Split(':')[1])).ToArray();

        return GetWinningOptionsCount(numbers[0], numbers[1]).ToString();
    }
}
