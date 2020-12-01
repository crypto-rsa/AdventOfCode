using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day1 : IAdventDay
    {
        public string Name => "1. 12. 2020";

        private static int[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day1.txt").Select(int.Parse).ToArray();

        public string Solve()
        {
            var input = GetInput();

            for (int i = 0; i < input.Length; i++)
            {
                for(int j = i + 1; j < input.Length; j++)
                {
                    if (input[i] + input[j] == 2020)
                        return (input[i] * input[j]).ToString();
                }
            }

            throw new System.InvalidOperationException();
        }

        public string SolveAdvanced()
        {
            var input = GetInput();

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = i + 1; j < input.Length; j++)
                {
                    for(int k = j + 1; k < input.Length; k++ )
                    {
                        if (input[i] + input[j] + input[k] == 2020)
                            return (input[i] * input[j] * input[k]).ToString();
                    }
                }
            }

            throw new System.InvalidOperationException();
        }
    }
}
