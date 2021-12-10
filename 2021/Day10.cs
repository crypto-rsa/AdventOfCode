using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day10 : IAdventDay
    {
        public string Name => "10. 12. 2021";

        private static IEnumerable<string> GetInput() => File.ReadAllLines("2021/Resources/day10.txt");

        public string Solve()
        {
            return GetInput().Sum(GetScore).ToString();

            int GetScore(string line)
            {
                var stack = new Stack<char>();

                foreach (char c in line)
                {
                    int charScore = GetCorruptionScore(c);

                    if (charScore == 0)
                    {
                        stack.Push(c);
                    }
                    else
                    {
                        var opening = stack.Pop();

                        if (!Matches(opening, c))
                            return charScore;
                    }
                }

                return 0;
            }
        }

        private static int GetCorruptionScore(char c) => c switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => 0,
        };

        private static int GetCompletionScore(char c) => c switch
        {
            '(' => 1,
            '[' => 2,
            '{' => 3,
            '<' => 4,
            _ => 0,
        };

        private static bool Matches(char opening, char closing) => (opening, closing) is ('(', ')') or ('[', ']') or ('{', '}') or ('<', '>');

        public string SolveAdvanced()
        {
            var lineScore = GetInput().Select(GetScore).Where(i => i >= 0).OrderBy(i => i).ToArray();

            return lineScore[lineScore.Length / 2].ToString();

            long GetScore(string line)
            {
                var stack = new Stack<char>();

                foreach (char c in line)
                {
                    int charScore = GetCorruptionScore(c);

                    if (charScore == 0)
                    {
                        stack.Push(c);
                    }
                    else
                    {
                        var opening = stack.Pop();

                        if (!Matches(opening, c))
                            return -1;
                    }
                }

                long score = 0;

                while (stack.Count > 0)
                {
                    score = score * 5 + GetCompletionScore(stack.Pop());
                }

                return score;
            }
        }
    }
}
