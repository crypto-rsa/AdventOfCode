namespace Advent_of_Code.Year2015
{
    public class Day25 : IAdventDay
    {
        public string Name => "25. 12. 2015";

        private static string GetInput() => System.IO.File.ReadAllText("2015/Resources/day25.txt");

        public string Solve()
        {
            var match = System.Text.RegularExpressions.Regex.Match(GetInput(), @".*at row (\d+), column (\d+)");

            var (targetRow, targetCol) = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            var  (row, col) = (1, 1);

            long code = 20151125;

            while(row != targetRow || col != targetCol)
            {
                code = (code * 252533) % 33554393;

                if (row == 1)
                {
                    row = col + 1;
                    col = 1;
                }
                else
                {
                    row--;
                    col++;
                }
            }

            return code.ToString();
        }

        public string SolveAdvanced()
        {
            return string.Empty;
        }
    }
}
