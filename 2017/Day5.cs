using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day5 : IAdventDay
    {
        public string Name => "5. 12. 2017";

        private static int[] GetInput() => System.IO.File.ReadAllLines(@"2017/Resources/day5.txt").Select(s => int.Parse(s)).ToArray();

        public string Solve()
        {
            var indices = GetInput();
            int curIndex = 0;
            int steps = 0;

            while( curIndex < indices.Length )
            {
                curIndex += ++indices[curIndex] - 1;
                steps++;
            }

            return steps.ToString();
        }

        public string SolveAdvanced()
        {
            var indices = GetInput();
            int curIndex = 0;
            int steps = 0;

            while( curIndex < indices.Length )
            {
                int offset = indices[curIndex];
                indices[curIndex] += offset >= 3 ? -1 : +1;
                curIndex += offset;
                steps++;
            }

            return steps.ToString();
        }
    }
}
