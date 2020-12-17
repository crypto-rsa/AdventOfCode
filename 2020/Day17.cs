using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day17 : IAdventDay
    {
        private record Point(int X, int Y, int Z, int W)
        {
            public Point(Point current, Point next, Func<int, int, int> selector)
                : this(selector(current.X, next.X), selector(current.Y, next.Y), selector(current.Z, next.Z), selector(current.W, next.W))
            {
            }

            public Point(int value)
                : this(value, value, value, value)
            {
            }

            public IEnumerable<Point> GetNeighbours(bool useFourthDimension)
            {
                for (int x = -1; x <= +1; x++)
                {
                    for (int y = -1; y <= +1; y++)
                    {
                        for (int z = -1; z <= +1; z++)
                        {
                            for (int w = -1; w <= +1; w++)
                            {
                                if (!useFourthDimension && w != 0)
                                    continue;

                                if (x == 0 && y == 0 && z == 0 && (!useFourthDimension || w == 0))
                                    continue;

                                yield return new Point(X + x, Y + y, Z + z, W + w);
                            }
                        }
                    }
                }
            }
        }

        private const char Active = '#';

        public string Name => "17. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day17.txt");

        public string Solve() => Iterate(6, useFourthDimension: false).Values.Count(c => c == Active).ToString();

        public string SolveAdvanced() => Iterate(6, useFourthDimension: true).Values.Count(c => c == Active).ToString();

        private static Dictionary<Point, char> Iterate(int iterations, bool useFourthDimension)
        {
            var input = GetInput();
            var alive = new Dictionary<Point, char>();

            for (int x = 0; x < input.Length; x++)
            {
                for (int y = 0; y < input[x].Length; y++)
                {
                    if (input[x][y] == Active)
                    {
                        alive.Add(new Point(x, y, 0, 0), input[x][y]);
                    }
                }
            }

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                var (min, max) = GetExtent();
                var nextAlive = new Dictionary<Point, char>();

                for (int x = min.X - 1; x <= max.X + 1; x++)
                {
                    for (int y = min.Y - 1; y <= max.Y + 1; y++)
                    {
                        for (int z = min.Z - 1; z <= max.Z + 1; z++)
                        {
                            for (int w = min.W - 1; w <= max.W + 1; w++)
                            {
                                if (!useFourthDimension && w != 0)
                                    continue;

                                var current = new Point(x, y, z, w);
                                bool newActive = (GetAliveNeighbours(current), IsActive(current)) switch
                                {
                                    (2 or 3, true) => true,
                                    (3, false) => true,
                                    _ => false,
                                };

                                if (newActive)
                                {
                                    nextAlive.Add(current, Active);
                                }
                            }
                        }
                    }
                }

                alive = nextAlive;
            }

            return alive;

            bool IsActive(Point point) => alive.TryGetValue(point, out char c) && c == Active;

            (Point Min, Point Max) GetExtent()
                => alive.Keys.Aggregate((Min: new Point(int.MaxValue), Max: new Point(int.MinValue)),
                    (acc, next) => (new Point(acc.Min, next, Math.Min), new Point(acc.Max, next, Math.Max)));

            int GetAliveNeighbours(Point point)
                => point.GetNeighbours(useFourthDimension).Count(p => alive.TryGetValue(p, out char c) && c == Active);
        }
    }
}
