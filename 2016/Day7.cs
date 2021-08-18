using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2016
{
    public class Day7 : IAdventDay
    {
        public string Name => "7. 12. 2016";

        private static IEnumerable<Code> GetInput() => File.ReadAllLines("2016/Resources/day7.txt").Select(s => new Code(s));

        private readonly struct Code
        {
            public Code(string input)
            {
                var match = Regex.Match(input, @"^(([a-z]+)(\[([a-z]+)\])?)+?$");

                HasTls = match.Groups[2].Captures.Any(c => IsAbba(c.Value)) && !match.Groups[3].Captures.Any(c => IsAbba(c.Value));

                var aba = match.Groups[2].Captures.SelectMany(c => GetAba(c.Value)).ToHashSet();
                HasSsl = match.Groups[3].Captures.Any(c => aba.Any(a => HasBab(c.Value, a)));

                static IEnumerable<Match> GetMatches(string text, string pattern)
                {
                    var regex = new Regex(pattern);
                    int start = 0;

                    while (true)
                    {
                        var m = regex.Match(text, start);

                        if (!m.Success)
                            break;

                        yield return m;

                        start = m.Index + 1;
                    }
                }

                static bool IsAbba(string s)
                {
                    var abbaMatches = GetMatches(s, @"([a-z])([a-z])\2\1");

                    return abbaMatches.Any(m => m.Groups[1].Value != m.Groups[2].Value);
                }

                static IEnumerable<string> GetAba(string s)
                {
                    var abaMatches = GetMatches(s, @"([a-z])([a-z])\1");

                    return abaMatches.Where(m => m.Groups[1].Value != m.Groups[2].Value).Select(m => m.Value);
                }

                static bool HasBab(string s, string aba) => s.Contains($"{aba[1]}{aba[0]}{aba[1]}");
            }

            public bool HasTls { get; }

            public bool HasSsl { get; }
        }

        public string Solve() => GetInput().Count(c => c.HasTls).ToString();

        public string SolveAdvanced() => GetInput().Count(c => c.HasSsl).ToString();
    }
}
