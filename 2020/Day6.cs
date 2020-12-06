using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day6 : IAdventDay
    {
        public string Name => "6. 12. 2020";

        private static string GetInput() => System.IO.File.ReadAllText("2020/Resources/day6.txt");

        public string Solve()
        {
            var groups = GetInput().Split("\r\n\r\n", StringSplitOptions.None);

            return groups.Sum(GetAnswerCount).ToString();

            static int GetAnswerCount(string group) => group.Where(c => c is >= 'a' and <= 'z').Distinct().Count();
        }

        public string SolveAdvanced()
        {
            var groups = GetInput().Split("\r\n\r\n", StringSplitOptions.None);

            return groups.Sum(GetAnswerCount).ToString();

            static int GetAnswerCount(string group)
            {
                var sets = group.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(s => new HashSet<char>(s)).ToArray();
                var allAnswers = Enumerable.Range(0, 26).Select(i => (char)('a' + i)).ToHashSet();

                return sets.Aggregate(allAnswers, (acc, next) => acc.Intersect(next).ToHashSet()).Count;
            }
        }
    }
}
