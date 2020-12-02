using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2020
{
    public class Day2 : IAdventDay
    {
        private class PasswordWithPolicy
        {
            public PasswordWithPolicy(string input)
            {
                var match = Regex.Match(input, @"(\d+)-(\d+) (.): (\w+)");

                int min = int.Parse(match.Groups[1].Value);
                int max = int.Parse(match.Groups[2].Value);
                char letter = match.Groups[3].Value[0];
                string password = match.Groups[4].Value;

                int count = password.Count<char>(c => c == letter);

                IsValidOld = count >= min && count <= max;
                IsValidNew = (password[min - 1] == letter) ^ (password[max - 1] == letter);
            }

            public bool IsValidOld { get; }

            public bool IsValidNew { get; }
        }

        public string Name => "2. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day2.txt");

        public string Solve() => GetInput().Select(s => new PasswordWithPolicy(s)).Count(p => p.IsValidOld).ToString();

        public string SolveAdvanced() => GetInput().Select(s => new PasswordWithPolicy(s)).Count(p => p.IsValidNew).ToString();
    }
}
