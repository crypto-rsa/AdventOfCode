using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day19 : IAdventDay
    {
        private class Rules
        {
            private readonly Dictionary<int, string> _regexes = new();

            public Rules(string[] input, bool useAdvancedRules)
            {
                var parsed = new Dictionary<int, List<List<int>>>();

                foreach(var line in input)
                {
                    if(line.Contains('"'))
                    {
                        var parts = line.Split(": ", StringSplitOptions.RemoveEmptyEntries);

                        _regexes[int.Parse(parts[0])] = parts[1].Replace("\"", string.Empty);
                    }
                    else
                    {
                        var (number, alternatives) = Parse(line);

                        parsed[number] = alternatives;
                    }
                }

                while (_regexes.Count < input.Length)
                {
                    var rule = parsed.Keys.First(i => !_regexes.ContainsKey(i) && parsed[i].SelectMany(j => j).All(j => _regexes.ContainsKey(j)));
                    var alternatives = parsed[rule].ConvertAll(l => string.Concat(l.Select(i => _regexes[i])));

                    _regexes[rule] = alternatives.Count == 1 ? alternatives[0] : $"({string.Join("|", alternatives)})";

                    if (useAdvancedRules)
                    {
                        if (rule == 8)
                        {
                            _regexes[rule] = $"({_regexes[42]})+";
                        }
                        else if(rule == 11)
                        {
                            _regexes[rule] = $"(?'Open'{_regexes[42]})+(?'-Open'{_regexes[31]})+(?(Open)(?!))";
                        }
                    }
                }

                _regexes[0] = $"^{_regexes[0]}$";

                static (int Number, List<List<int>> Alternatives) Parse(string line)
                {
                    var parts = line.Split(": ", StringSplitOptions.RemoveEmptyEntries);

                    var alternatives = parts[1].Split(" | ", StringSplitOptions.RemoveEmptyEntries);

                    return (int.Parse(parts[0]), alternatives.Select(s => s.Split(' ').Select(int.Parse).ToList()).ToList());
                }
            }

            public int MatchesRule0(string input)
                => input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Count(s => System.Text.RegularExpressions.Regex.IsMatch(s, _regexes[0]));
        }

        public string Name => "19. 12. 2020";

        private static string GetInput() => System.IO.File.ReadAllText("2020/Resources/day19.txt");

        public string Solve() => GetMatchingCount(useAdvancedRules: false).ToString();

        public string SolveAdvanced() => GetMatchingCount(useAdvancedRules: true).ToString();

        private int GetMatchingCount(bool useAdvancedRules)
        {
            var parts = GetInput().Split("\r\n\r\n");
            var rules = new Rules(parts[0].Split("\r\n", StringSplitOptions.RemoveEmptyEntries), useAdvancedRules);

            return rules.MatchesRule0(parts[1]);
        }
    }
}
