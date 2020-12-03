using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day13 : IAdventDay
    {
        public string Name => "13. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day13.txt");

        public string Solve() => SolveInternal(includeMyself: false).ToString();

        public string SolveAdvanced() => SolveInternal(includeMyself: true).ToString();

        private int SolveInternal(bool includeMyself)
        {
            var happiness = new Dictionary<(string, string), int>();

            foreach (var item in GetInput())
            {
                var match = System.Text.RegularExpressions.Regex.Match(item, @"(\w+) would (gain|lose) (\d+) happiness units by sitting next to (\w+).");
                var value = (match.Groups[2].Value == "gain" ? +1 : -1) * int.Parse(match.Groups[3].Value);

                happiness.Add((match.Groups[1].Value, match.Groups[4].Value), value);
            }

            var names = happiness.Keys.Select(i => i.Item1).Distinct().OrderBy(s => s).Select((s, i) => (i, s)).ToDictionary(i => i.Item1, i => i.Item2);

            if (includeMyself)
            {
                const string myself = "Myself";

                foreach (var name in names.Values)
                {
                    happiness[(myself, name)] = 0;
                    happiness[(name, myself)] = 0;
                }

                names[names.Count] = myself;
            }

            return Tools.Combinatorics.GetPermutations(names.Count - 1).Max(p => Enumerable.Range(0, names.Count).Sum(i => GetHappiness(i, p)));

            int GetHappiness(int index, int[] permutation)
            {
                // always start from the first person (the list is circular)
                var fullPermutation = permutation.Select(i => i + 1).Prepend(0).ToArray();

                var people = Enumerable.Range(-1, 3).Select(i => names[fullPermutation[(index + fullPermutation.Length + i) % fullPermutation.Length]]).ToArray();

                return happiness[(people[1], people[0])] + happiness[(people[1], people[2])];
            }
        }
    }
}
