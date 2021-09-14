using System.IO;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2016
{
    public class Day9 : IAdventDay
    {
        public string Name => "9. 12. 2016";

        private static string GetInput() => File.ReadAllText("2016/Resources/day9.txt").Trim();

        private static long Decompress(string input, bool recursive)
        {
            var regex = new Regex(@"\((\d+)x(\d+)\)");
            int pos = 0;
            long totalLength = 0;

            while (pos < input.Length)
            {
                var match = regex.Match(input, pos);
                int end = match.Success ? match.Index : input.Length;

                if (end > pos)
                {
                    totalLength += end - pos;
                    pos = end;
                }

                if (!match.Success)
                    continue;

                int length = int.Parse(match.Groups[1].Value);
                int repetitions = int.Parse(match.Groups[2].Value);

                long subLength = recursive ? Decompress(input[(match.Index + match.Length)..(match.Index + match.Length + length)], true) : length;
                totalLength += repetitions * subLength;
                pos += match.Length + length;
            }

            return totalLength;
        }

        public string Solve() => Decompress(GetInput(), recursive: false).ToString();

        public string SolveAdvanced() => Decompress(GetInput(), recursive: true).ToString();
    }
}
