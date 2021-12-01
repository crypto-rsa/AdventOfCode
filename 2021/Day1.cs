using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day1 : IAdventDay
    {
        public string Name => "1. 12. 2021";

        private static int[] GetInput() => File.ReadAllLines("2021/Resources/day1.txt").Select(int.Parse).ToArray();

        public string Solve()
        {
            var input = GetInput();

            return Enumerable.Range(1, input.Length - 1).Count(i => input[i] > input[i - 1]).ToString();
        }

        public string SolveAdvanced()
        {
            var input = GetInput();

            if (input.Length < 3)
                return 0.ToString();

            var tripletSums = new int[input.Length - 2];

            tripletSums[0] = input.Take(3).Sum();

            for (int i = 3; i < input.Length; i++)
            {
                tripletSums[i - 2] = tripletSums[i - 3] - input[i - 3] + input[i];
            }

            return Enumerable.Range(1, tripletSums.Length - 1).Count(i => tripletSums[i] > tripletSums[i - 1]).ToString();
        }
    }
}
