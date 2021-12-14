using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day14 : IAdventDay
    {
        public string Name => "14. 12. 2021";

        private static string GetInput() => File.ReadAllText("2021/Resources/day14.txt");

        private class Polymer
        {
            private readonly string _template;

            private readonly Dictionary<string, string> _rules;

            public Polymer()
            {
                var parts = GetInput().Split($"{Environment.NewLine}{Environment.NewLine}");

                _template = parts[0];
                _rules = parts[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToDictionary(s => s[..2], s => $"{s[0]}{s[^1]}{s[1]}");
            }

            private static IEnumerable<string> GetPairs(string input) => Enumerable.Range(0, input.Length - 1).Select(i => input[i..(i + 2)]);

            private string GetReplacement(string input) => _rules.TryGetValue(input, out var output) ? output : input;

            public long GetMostAndLeastCommonElementDifference(int iterations)
            {
                var pairs = GetPairs(_template).GroupBy(s => s).ToDictionary(g => g.Key, g => (long)g.Count());
                string lastPair = _template[^2..];

                for (int i = 0; i < iterations; i++)
                {
                    var newPairs = new Dictionary<string, long>();

                    foreach (var item in pairs)
                    {
                        var replacement = GetReplacement(item.Key);

                        Add(newPairs, item.Key, item.Value, replacement);
                    }

                    pairs = newPairs;
                }

                var elementsByCount = pairs
                    .Select(i => (Element: i.Key[..1], Count: i.Value))
                    .Append((Element: lastPair[1..], Count: 1))
                    .GroupBy(i => i.Element)
                    .ToDictionary(g => g.Key, g => g.Sum(i => i.Count));

                var elementCounts = elementsByCount.Values.OrderByDescending(i => i).ToList();

                return (elementCounts.First() - elementCounts.Last());

                void Add(Dictionary<string, long> dictionary, string key, long oldCount, string replacement)
                {
                    var replacementPairs = GetPairs(replacement).ToList();

                    foreach (string s in replacementPairs)
                    {
                        dictionary.TryGetValue(s, out long curCount);

                        dictionary[s] = oldCount + curCount;
                    }

                    if (key == lastPair)
                    {
                        lastPair = replacementPairs.Last();
                    }
                }
            }
        }

        public string Solve() => new Polymer().GetMostAndLeastCommonElementDifference(10).ToString();

        public string SolveAdvanced() => new Polymer().GetMostAndLeastCommonElementDifference(40).ToString();
    }
}
