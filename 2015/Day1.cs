using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day1 : IAdventDay
    {
        public string Name => "1. 12. 2015";

        public string Solve()
        {
            var input = GetInput();

            return (input.Count(c => c == '(') - input.Count(c => c == ')')).ToString();
        }

        private static string GetInput() => System.IO.File.ReadAllText("2015/Resources/day1.txt");

        public string SolveAdvanced()
        {
            var input = GetInput();

            for (int i = 0, level = 0; i < input.Length; i++ )
            {
                level += input[i] == '(' ? +1 : -1;

                if (level == -1)
                    return (i + 1).ToString();
            }

            return string.Empty;
        }
    }
}
