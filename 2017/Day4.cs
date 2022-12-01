using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day4 : IAdventDay
    {
        public string Name => "4. 12. 2017";

        private static IEnumerable<string[]> GetInput() => System.IO.File.ReadAllLines(@"2017/Resources/day4.txt").Select(s => s.Split(' '));

        public string Solve()
        {
            return GetInput().Count(words => words.Distinct().Count() == words.Length).ToString();
        }

        public string SolveAdvanced()
        {
            return GetInput().Count(words => words.Select(GetCanonicalWord).Distinct().Count() == words.Length).ToString();

            string GetCanonicalWord(string word) => new string(word.OrderBy(c => c).ToArray());
        }
    }
}
