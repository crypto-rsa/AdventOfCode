using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day5 : IAdventDay
    {
        public string Name => "5. 12. 2021";

        private static IEnumerable<Line> GetInput() => File.ReadAllLines("2021/Resources/day5.txt").Select(s => new Line(s));

        private record Line(int X1 = 0, int Y1 = 0, int X2 = 0, int Y2 = 0)
        {
            public Line(string input)
                : this()
            {
                var match = System.Text.RegularExpressions.Regex.Match(input, @"(\d+),(\d+) -> (\d+),(\d+)");

                X1 = Parse(1);
                Y1 = Parse(2);
                X2 = Parse(3);
                Y2 = Parse(4);

                int Parse(int groupIndex) => int.Parse(match.Groups[groupIndex].Value);
            }

            public bool IsHorizontal => Y1 == Y2;

            public bool IsVertical => X1 == X2;

            public IEnumerable<(int X, int Y)> GetPositions()
            {
                var xOffset = Math.Sign(X2 - X1);
                var yOffset = Math.Sign(Y2 - Y1);

                for ((int x, int y) = (X1, Y1);; (x, y) = (x + xOffset, y + yOffset))
                {
                    yield return (x, y);

                    if ((x, y) == (X2, Y2))
                        break;
                }
            }
        }

        private static int GetOverlappingCount(Func<Line, bool> linePredicate)
        {
            var incidence = new Dictionary<(int, int), int>();

            foreach (var position in GetInput().Where(linePredicate).SelectMany(l => l.GetPositions()))
            {
                if (!incidence.ContainsKey(position))
                {
                    incidence[position] = 0;
                }

                incidence[position]++;
            }

            return incidence.Count(p => p.Value > 1);
        }

        public string Solve() => GetOverlappingCount(l => l.IsHorizontal || l.IsVertical).ToString();

        public string SolveAdvanced() => GetOverlappingCount(l => true).ToString();
    }
}
