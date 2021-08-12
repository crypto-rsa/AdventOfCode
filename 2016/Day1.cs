using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day1 : IAdventDay
    {
        public string Name => "1. 12. 2016";

        private static List<(char Direction, int Distance)> GetInput() => System.IO.File.ReadAllText("2016/Resources/day1.txt")
            .Split(", ")
            .Select(s => (s[0], int.Parse(s[1..])))
            .ToList();

        public string Solve()
        {
            (int x, int y) = Iterate().Last();

            return (Math.Abs(x) + Math.Abs(y)).ToString();
        }

        private static IEnumerable<(int X, int Y)> Iterate()
        {
            var position = (Direction: 'N', X: 0, Y: 0);

            foreach ((char direction, int distance) in GetInput())
            {
                char newDirection = (position.Direction, direction) switch
                {
                    ('N', 'L') => 'W',
                    ('N', 'R') => 'E',
                    ('W', 'L') => 'S',
                    ('W', 'R') => 'N',
                    ('S', 'L') => 'E',
                    ('S', 'R') => 'W',
                    ('E', 'L') => 'N',
                    ('E', 'R') => 'S',
                    _ => throw new InvalidOperationException(),
                };

                (int xOffset, int yOffset) = newDirection switch
                {
                    'N' => (0, +distance),
                    'E' => (+distance, 0),
                    'S' => (0, -distance),
                    'W' => (-distance, 0),
                    _ => throw new InvalidOperationException(),
                };

                foreach( var i in GetSteps(xOffset) )
                {
                    foreach( var j in GetSteps(yOffset))
                    {
                        if ((i, j) == (0, 0))
                            continue;

                        yield return (position.X + i, position.Y + j);
                    }
                }

                position = (newDirection, position.X + xOffset, position.Y + yOffset);
            }

            IEnumerable<int> GetSteps(int offset) => Enumerable.Range(0, Math.Abs(offset) + 1).Select(i => offset >= 0 ? i : -i);
        }

        public string SolveAdvanced()
        {
            var visited = new HashSet<(int, int)>()
            {
                (0, 0),
            };

            foreach ((int x, int y) in Iterate())
            {
                if (visited.Contains((x, y)))
                    return (Math.Abs(x) + Math.Abs(y)).ToString();

                visited.Add((x, y));
            }

            return "No location was visited twice";
        }
    }
}