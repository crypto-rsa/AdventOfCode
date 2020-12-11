using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2015
{
    public class Day19 : IAdventDay
    {
        private class Chemistry
        {
            public Chemistry(string[] lines)
            {
                Base = lines.Last();
                Reactions = lines.Take(lines.Length - 2).Select(GetReaction).ToList();

                static (string, string) GetReaction(string s)
                {
                    var items = s.Split(" => ", StringSplitOptions.RemoveEmptyEntries);

                    return (items[0], items[1]);
                }
            }

            public string Base { get; }

            public List<(string Input, string Output)> Reactions { get; }
        }

        public string Name => "19. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day19.txt");

        public string Solve()
        {
            var chemistry = new Chemistry(GetInput());
            var molecules = GetVariants(chemistry.Base, chemistry.Reactions, false).ToHashSet();

            return molecules.Count.ToString();
        }

        public string SolveAdvanced()
        {
            var chemistry = new Chemistry(GetInput());
            var stack = new Stack<(string Base, int Steps)>();
            var visited = new HashSet<string>();

            stack.Push((chemistry.Base, 0));

            while (stack.Count > 0)
            {
                var (baseString, steps) = stack.Pop();

                if (baseString == "e")
                    return steps.ToString();

                foreach (var variant in GetVariants(baseString, chemistry.Reactions, true).Where(s => !visited.Contains(s)))
                {
                    if (visited.Contains(variant))
                        continue;

                    if (variant.Length > chemistry.Base.Length)
                        continue;

                    stack.Push((variant, steps + 1));
                    visited.Add(variant);
                }
            }

            return "Not found!";
        }

        private static IEnumerable<string> GetVariants(string baseString, IEnumerable<(string Input, string Output)> reactions, bool reverse)
        {
            return reactions.SelectMany(GetMatches).OrderBy(i => i.Match.Value.Length - i.Replacement.Length).Select(GetVariant);

            string GetVariant((Match Match, string Replacement) item)
                => baseString[0..item.Match.Index] + item.Replacement + baseString[(item.Match.Index + item.Match.Length)..^0];

            IEnumerable<(Match Match, string Replacement)> GetMatches((string Input, string Output) reaction)
            {
                var (searchString, replacement) = reverse ? (reaction.Output, reaction.Input) : (reaction.Input, reaction.Output);

                return Regex.Matches(baseString, searchString).Select(m => (m, replacement));
            }
        }
    }
}
