using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day10 : IAdventDay
    {
        public string Name => "10. 12. 2019";

        public string Solve() => GetStation().Count.ToString();

        public string SolveAdvanced() => GetAsteroidId(order: 200).ToString();

        private static (HashSet<(int X, int Y)> Positions, int Width, int Height) GetInput()
        {
            var input = System.IO.File.ReadAllLines("2019/Resources/day10.txt");
            int width = input.Select(s => s.Length).Distinct().Single();
            int height = input.Length;

            var asteroids = new HashSet<(int, int)>();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (input[j][i] == '#')
                    {
                        asteroids.Add((i, j));
                    }
                }
            }

            return (asteroids, width, height);
        }

        private ((int X, int Y) Position, int Count) GetStation()
        {
            var (positions, width, height) = GetInput();
            var vectors = GetVectors(width, height).Distinct().ToList();

            return positions.Select(p => (Position: p, Count: vectors.Count(v => SeesAsteroid(p, v)))).OrderByDescending(i => i.Count).First();

            bool SeesAsteroid((int X, int Y) position, (int X, int Y) vector)
            {
                return Enumerable.Range(1, Math.Max(width, height))
                    .Select(d => GetPosition(position, vector, d))
                    .TakeWhile(position1 => IsInMap(position1, width, height))
                    .Any(positions.Contains);
            }
        }

        private static bool IsInMap((int X, int Y) position, int width, int height) => position.X >= 0 && position.X < width && position.Y >= 0 && position.Y < height;

        private static (int X, int Y) GetPosition((int X, int Y) start, (int X, int Y) vector, int scalar) => (start.X + scalar * vector.X, start.Y + scalar * vector.Y);

        private static IEnumerable<(int X, int Y)> GetVectors(int width, int height)
        {
            for (int i = 0; i <= +width; i++)
            {
                for (int j = 0; j <= +height; j++)
                {
                    if (GCD(i, j) == 1)
                    {
                        yield return (-i, -j);
                        yield return (-i, +j);
                        yield return (+i, -j);
                        yield return (+i, +j);
                    }
                }
            }

            static int GCD(int a, int b)
            {
                int max = Math.Max(a, b);
                int min = Math.Min(a, b);

                return min == 0 ? max : GCD(min, max % min);
            }
        }

        private int GetAsteroidId(int order)
        {
            var (positions, width, height) = GetInput();
            var vectors = GetVectors(width, height).Distinct().OrderBy(GetAngle).ToList();

            var station = GetStation().Position;
            var (x, y) = GetSortedPositions().ElementAt(order - 1);

            return x * 100 + y;

            static double GetAngle((int X, int Y) vector)
            {
                return vector switch
                {
                    (0, int y) when y < 0 => 0,
                    (int x, int y) when x < 0 && y <= 0 => Math.PI * 5 / 2.0 - Math.Atan2(-y, x),
                    (int x, int y) => Math.PI / 2.0 - Math.Atan2(-y, x),
                };
            }

            IEnumerable<(int X, int Y)> GetSortedPositions()
            {
                return vectors.SelectMany(GetPositionsInDirection).OrderBy(item => item.Layer).Select(item => item.Position);

                IEnumerable<((int X, int Y) Position, int Layer)> GetPositionsInDirection((int X, int Y) vector)
                {
                    int layer = 0;
                    for (int scalar = 1; scalar < Math.Max(width, height); scalar++)
                    {
                        var target = GetPosition(station, vector, scalar);

                        if (!IsInMap(target, width, height))
                            yield break;

                        if (positions.Contains(target))
                            yield return (target, ++layer);
                    }
                }
            }
        }
    }
}
