using System;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day5 : IAdventDay
    {
        public string Name => "5. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day5.txt");

        public string Solve() => GetInput().Max(ToNumber).ToString();

        public string SolveAdvanced()
        {
            var allNumbers = GetInput().Select(ToNumber).ToHashSet();
            var number = Enumerable.Range(0, 1024).Single(i => !allNumbers.Contains(i) && allNumbers.Contains(i - 1) && allNumbers.Contains(i + 1));

            return number.ToString();
        }

        private static int ToNumber(string input)
        {
            var binary = input.Replace('F', '0').Replace('B', '1').Replace('L', '0').Replace('R', '1');

            return Convert.ToInt32(binary[0..7], 2) * 8 + Convert.ToInt32(binary[7..10], 2);
        }
    }
}
