using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day8 : IAdventDay
    {
        public string Name => "8. 12. 2021";

        private static string[] GetInput() => File.ReadAllLines("2021/Resources/day8.txt");

        private class Displays
        {
            private class Assignment : Dictionary<char, char>
            {
                private Assignment()
                {
                }

                private Assignment(Assignment other)
                    : base(other)
                {
                }

                public static Assignment Create(string[] patterns) => Extend(new Assignment(), patterns);

                private static Assignment Extend(Assignment current, string[] patterns)
                {
                    if (current.IsComplete)
                        return current;

                    char nextChar = (char)('a' + current.Count);

                    return AllSegments.Except(current.Values).Select(GetWithReplacement).FirstOrDefault(a => a?.IsComplete == true);

                    Assignment GetWithReplacement(char replacement)
                    {
                        var next = new Assignment(current)
                        {
                            [nextChar] = replacement,
                        };

                        return patterns.All(p => Matches(next, p)) ? Extend(next, patterns) : new Assignment();
                    }

                    bool Matches(Assignment assignment, string pattern)
                    {
                        return SegmentsByLength[pattern.Length].Any(s => CanTranslateTo(pattern, s, assignment));
                    }

                    bool CanTranslateTo(string pattern, string target, Assignment assignment)
                    {
                        return pattern.Where(assignment.ContainsKey).Select(c => assignment[c]).All(target.Contains);
                    }
                }

                private bool IsComplete => Count == SegmentCount;

                public int Translate(string pattern)
                {
                    var translated = SegmentsByLength[pattern.Length].Single(s => pattern.Select(c => this[c]).All(s.Contains));

                    return Segments.Single(i => i.Value == translated).Key;
                }
            }

            private const int SegmentCount = 7;

            private const string AllSegments = "abcdefg";

            private static Dictionary<int, string> Segments { get; } = new()
            {
                [0] = "abcefg",
                [1] = "cf",
                [2] = "acdeg",
                [3] = "acdfg",
                [4] = "bcdf",
                [5] = "abdfg",
                [6] = "abdefg",
                [7] = "acf",
                [8] = "abcdefg",
                [9] = "abcdfg",
            };

            private static Dictionary<int, List<string>> SegmentsByLength { get; } = Segments
                .Values
                .GroupBy(i => i.Length)
                .ToDictionary(g => g.Key, g => g.ToList());

            private readonly List<(string[] Patterns, string[] Output)> _entries;

            public Displays()
            {
                _entries = GetInput().Select(Parse).ToList();

                (string[], string[]) Parse(string line)
                {
                    var parts = line.Split(" | ");

                    var patterns = parts[0].Split(' ');
                    var output = parts[1].Split(' ');

                    return (patterns, output);
                }
            }

            public int GetUniqueDigitsCount()
            {
                var uniqueLengths = new[] { 1, 4, 7, 8 }.Select(i => Segments[i].Length).ToHashSet();

                return _entries.Sum(i => i.Output.Count(s => uniqueLengths.Contains(s.Length)));
            }

            public int GetTotalOutput()
            {
                int total = 0;

                foreach (var entry in _entries)
                {
                    var assignment = Assignment.Create(entry.Patterns);

                    int output = 0;

                    foreach (string pattern in entry.Output)
                    {
                        output = 10 * output + assignment.Translate(pattern);
                    }

                    total += output;
                }

                return total;
            }
        }

        public string Solve() => new Displays().GetUniqueDigitsCount().ToString();

        public string SolveAdvanced() => new Displays().GetTotalOutput().ToString();
    }
}
