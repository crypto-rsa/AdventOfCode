using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day18 : IAdventDay
    {
        public string Name => "18. 12. 2016";

        private static string GetInput() => File.ReadAllText("2016/Resources/day18.txt").Trim('\r', '\n');

        private static int GetSafeTileCount(int rowCount)
        {
            var currentRow = GetInput();
            int safeTiles = 0;

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                safeTiles += currentRow.Count(c => c == '.');
                currentRow = new string(Enumerable.Range(0, currentRow.Length).Select(i => IsTrap(GetPattern(i)) ? '^' : '.').ToArray());
            }

            return safeTiles;

            bool IsTrap(string pattern) => pattern is "^^." or ".^^" or "^.." or "..^";

            string GetPattern(int index)
            {
                if (index == 0)
                    return "." + currentRow[..2];

                if (index == currentRow.Length - 1)
                    return currentRow[^2..] + ".";

                return currentRow[(index - 1)..(index + 2)];
            }
        }

        public string Solve() => GetSafeTileCount(40).ToString();

        public string SolveAdvanced() => GetSafeTileCount(400000).ToString();
    }
}
