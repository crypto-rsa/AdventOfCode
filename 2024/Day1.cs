using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2024;

public class Day1 : IAdventDay
{
    public string Name => "1. 12. 2024";

    private static string[] GetInput() => File.ReadAllLines("2024/Resources/day1.txt");

    private record Lists(List<int> Left, List<int> Right)
    {
        public static Lists Aggregate(Lists current, string next)
        {
            var parts = next.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            current.Left.Add(int.Parse(parts[0]));
            current.Right.Add(int.Parse(parts[1]));

            return current;
        }
    }

    public string Solve()
    {
        var lists = GetInput().Aggregate(new Lists([], []), Lists.Aggregate);

        lists.Left.Sort();
        lists.Right.Sort();

        return lists.Left.Zip(lists.Right, (l, r) => Math.Abs(l - r)).Sum().ToString();
    }

    public string SolveAdvanced()
    {
        var lists = GetInput().Aggregate(new Lists([], []), Lists.Aggregate);
        var counts = lists.Right.ToLookup(i => i);

        return lists.Left
            .Sum(i => i * counts[i].Count())
            .ToString();
    }
}
