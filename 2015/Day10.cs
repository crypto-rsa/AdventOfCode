using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day10 : IAdventDay
    {
        public string Name => "10. 12. 2015";

        private const string Input = "1113122113";

        public string Solve() => Iterate(Input, 40).Length.ToString();

        public string SolveAdvanced() => Iterate(Input, 50).Length.ToString();

        private static string Iterate(string input)
        {
            var segmented = input.Aggregate(new List<(char, int)>(), Aggregate);

            return string.Concat(segmented.Select(ToSubstring));

            List<(char, int)> Aggregate(List<(char Char, int Count)> prev, char next)
            {
                if( prev.Count == 0 || prev.Last().Char != next )
                {
                    prev.Add((next, 1));
                }
                else
                {
                    prev[^1] = (next, prev.Last().Count + 1);
                }

                return prev;
            }

            string ToSubstring((char Char, int Count) item) => $"{item.Count}{item.Char}";
        }

        private static string Iterate(string input, int iterations)
        {
            var s = input;

            for (int i = 0; i < iterations; i++)
            {
                s = Iterate(s);
            }

            return s;
        }
    }
}
