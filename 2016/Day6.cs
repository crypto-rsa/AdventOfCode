using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day6 : IAdventDay
    {
        public string Name => "6. 12. 2016";

        private static string[] GetInput() => File.ReadAllLines("2016/Resources/day6.txt");

        public string Solve()
        {
            var lines = GetInput();
            var length = lines[0].Length;

            return string.Join(string.Empty, Enumerable.Range(0, length).Select(GetMostFrequent));

            char GetMostFrequent(int index) => lines.Select(s => s[index]).GroupBy(c => c).OrderByDescending(g => g.Count()).First().Key;
        }

        public string SolveAdvanced()
        {
            var lines = GetInput();
            var length = lines[0].Length;

            return string.Join(string.Empty, Enumerable.Range(0, length).Select(GetMostFrequent));

            char GetMostFrequent(int index) => lines.Select(s => s[index]).GroupBy(c => c).OrderBy(g => g.Count()).First().Key;
        }
    }
}
