using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day5 : IAdventDay
{
    public string Name => "5. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day5.txt");

    private record Data
    {
        public Data(string input)
        {
            var parts = input.ParseByBlankLines().ToArray();

            Rules = parts[0].Select(ParseRule).ToList();
            Updates = parts[1].ParseIntegerSequences(",").Select(i => i.ToList()).ToList();

            (int, int) ParseRule(string rule)
            {
                var ruleParts = rule.Split("|");

                return (int.Parse(ruleParts[0]), int.Parse(ruleParts[1]));
            }
        }

        public IReadOnlyList<(int Before, int After)> Rules { get; }

        public IReadOnlyList<List<int>> Updates { get; }

        public bool Matches(List<int> update)
        {
            foreach (var rule in Rules)
            {
                int index1 = update.FindIndex(i => i == rule.Before);
                int index2 = update.FindIndex(i => i == rule.After);

                if (index1 == -1 || index2 == -1)
                    continue;

                if (index1 > index2)
                    return false;
            }

            return true;
        }

        public int GetMiddleInFixedOrdering(List<int> update)
        {
            if (Matches(update))
                return 0;

            var directPredecessors = update.ToDictionary(i => i, GetDirectPredecessors);
            var allPredecessors = new Dictionary<int, HashSet<int>>(directPredecessors);

            foreach (var item in allPredecessors)
            {
                var candidates = new Stack<int>(item.Value);

                while (candidates.Any())
                {
                    int candidate = candidates.Pop();

                    foreach (var n in directPredecessors[candidate])
                    {
                        item.Value.Add(n);
                        candidates.Push(n);
                    }
                }
            }

            return allPredecessors.Single(i => i.Value.Count == update.Count / 2).Key;

            HashSet<int> GetDirectPredecessors(int number) => Rules
                .Where(r => r.After == number && update.Contains(r.Before))
                .Select(r => r.Before)
                .ToHashSet();
        }
    }

    public string Solve()
    {
        var data = new Data(GetInput());
        int total = 0;

        foreach (var update in data.Updates.Where(data.Matches))
        {
            total += update[update.Count / 2];
        }

        return total.ToString();
    }

    public string SolveAdvanced()
    {
        var data = new Data(GetInput());

        return data.Updates.Sum(data.GetMiddleInFixedOrdering).ToString();
    }
}
