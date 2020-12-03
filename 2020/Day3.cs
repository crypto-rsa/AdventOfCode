namespace Advent_of_Code.Year2020
{
    public class Day3 : IAdventDay
    {
        public string Name => "3. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day3.txt");

        public string Solve() => GetTreeCount(3, 1).ToString();

        public string SolveAdvanced()
        {
            var result = GetTreeCount(1, 1) * GetTreeCount(3, 1) * GetTreeCount(5, 1) * GetTreeCount(7, 1) * GetTreeCount(1, 2);

            return result.ToString();
        }

        private static long GetTreeCount(int stepRight, int stepDown)
        {
            var input = GetInput();
            long treeCount = 0;
            int y = 0;

            for (int row = stepDown; row < input.Length; row += stepDown)
            {
                y = (y + stepRight) % input[row].Length;

                if (input[row][y] == '#')
                {
                    treeCount++;
                }
            }

            return treeCount;
        }
    }
}
