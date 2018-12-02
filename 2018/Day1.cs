using System.Collections.Generic;
using System.IO;
using System.Linq;
using Advent_of_Code;

namespace Advent_of_Code_2018
{
    public class Day1 : IAdventDay
    {
        public string Name => "1. 12. 2018";

        public string Solve()
        {
            return GetInput().Sum().ToString();
        }

        private static IEnumerable<int> GetInput() => File.ReadAllLines(@"2018/Resources/day1.txt").Select(s => int.Parse(s));

        public string SolveAdvanced()
        {
            var sums = new HashSet<int>();
            var input = GetInput().ToArray();

            int sum = 0;
            int index = 0;

            do
            {
                sum += input[index];
                index = (index + 1) % input.Length;
            } while (sums.Add(sum));

            return sum.ToString();
        }
    }
}