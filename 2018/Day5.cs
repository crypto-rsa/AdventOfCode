using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day5 : IAdventDay
    {
        public string Name => "5. 12. 2018";

        private static string GetInput() => System.IO.File.ReadAllText(@"2018/Resources/day5.txt").Trim();

        private static Dictionary<char, char> GetReactions()
        {
            var letters = Enumerable.Range(0, 26).Select(i => (char)('a' + i)).ToList();

            var reactions = new Dictionary<char, char>();
            foreach( var letter in letters )
            {
                reactions[letter] = char.ToUpper(letter);
                reactions[char.ToUpper(letter)] = letter;
            }

            return reactions;
        }

        public string Solve()
        {
            return GetProductLength(GetInput()).ToString();
        }

        private static int GetProductLength(string input)
        {
            var reactions = GetReactions();
            var stack = new Stack<char>();

            for (int i = 0; i < input.Length; i++)
            {
                if (stack.Count > 0 && stack.Peek() == reactions[input[i]])
                {
                    stack.Pop();
                }
                else
                {
                    stack.Push(input[i]);
                }
            }

            return stack.Count;
        }

        public string SolveAdvanced()
        {
            var rawInput = GetInput();

            return GetReactions().Keys.Where(c => char.IsLower(c)).Select(c => GetProductLength(GetShortened(c))).Min().ToString();

            string GetShortened(char c) => rawInput.Replace(c.ToString(), string.Empty).Replace(char.ToUpper(c).ToString(), string.Empty);
        }
    }
}