using System.Diagnostics;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day1 : IAdventDay
    {
        public string Name => "1. 12. 2017";

        public string Solve()
        {
            int[] digits = GetInput();

            return digits.Where((d, i) => digits[(i + 1) % digits.Length] == d).Sum().ToString();
        }

        private static int[] GetInput() => System.IO.File.ReadAllText(@"2017/Resources/day1.txt").Trim().Select(c => int.Parse(c.ToString())).ToArray();

        public string SolveAdvanced()
        {
            var digits = GetInput();
            int offset = digits.Length / 2;

            return digits.Where((d, i) => digits[(i + offset) % digits.Length] == d).Sum().ToString();
        }
    }
}
