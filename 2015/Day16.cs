using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2015
{
    public class Day16 : IAdventDay
    {
        private class Aunt
        {
            public Aunt(string input)
            {
                var match = Regex.Match(input, @"Sue (\d+): (\w+): (\d+), (\w+): (\d+), (\w+): (\d+)");
                var pairs = Enumerable.Range(1, 3).Select(i => new KeyValuePair<string, int>(match.Groups[2 * i].Value,
                    int.Parse(match.Groups[2 * i + 1].Value)));

                Number = int.Parse(match.Groups[1].Value);
                Items = new Dictionary<string, int>(pairs);
            }

            public int Number { get; }

            public Dictionary<string, int> Items { get; }

            public bool Matches(Dictionary<string, int> test)
                => Items.All(p => test[p.Key] == p.Value);

            public bool MatchesAdvanced(Dictionary<string, int> test)
            {
                return Items.All(MatchesItem);

                bool MatchesItem(KeyValuePair<string, int> pair) => pair.Key switch
                {
                    "cats" => pair.Value > test[pair.Key],
                    "trees" => pair.Value > test[pair.Key],
                    "pomeranians" => pair.Value < test[pair.Key],
                    "goldfish" => pair.Value < test[pair.Key],
                    _ => pair.Value == test[pair.Key],
                };
            }
        }

        public string Name => "16. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day16.txt");

        private static readonly Dictionary<string, int> _test = new Dictionary<string, int>()
        {
            ["children"] = 3,
            ["cats"] = 7,
            ["samoyeds"] = 2,
            ["pomeranians"] = 3,
            ["akitas"] = 0,
            ["vizslas"] = 0,
            ["goldfish"] = 5,
            ["trees"] = 3,
            ["cars"] = 2,
            ["perfumes"] = 1,
        };

        public string Solve()
        {
            var aunts = GetInput().Select(s => new Aunt(s)).ToList();

            return aunts.Single(a => a.Matches(_test)).Number.ToString();
        }

        public string SolveAdvanced()
        {
            var aunts = GetInput().Select(s => new Aunt(s)).ToList();

            return aunts.Single(a => a.MatchesAdvanced(_test)).Number.ToString();
        }
    }
}
