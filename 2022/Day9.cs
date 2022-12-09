using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day9 : IAdventDay
{
    public string Name => "9. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day9.txt");

    private readonly record struct Point(int X, int Y)
    {
        public Point Move(int xOffset, int yOffset) => new(X + xOffset, Y + yOffset);

        public Point Abs() => new(Math.Abs(X), Math.Abs(Y));

        public static Point operator -(Point first, Point second) => new(first.X - second.X, first.Y - second.Y);
    }

    private static IEnumerable<Point> Simulate(int knotCount)
    {
        var knots = Enumerable.Range(0, knotCount).Select(_ => new Point()).ToArray();

        yield return knots[^1];

        foreach (string instruction in GetInput().SplitToLines())
        {
            var parts = instruction.Split(' ');
            int offset = int.Parse(parts[1]);

            (int x, int y) = parts[0] switch
            {
                "R" => (+1, 0),
                "U" => (0, +1),
                "L" => (-1, 0),
                "D" => (0, -1),
                _ => throw new InvalidOperationException(),
            };

            for (int i = 0; i < offset; i++)
            {
                knots[0] = knots[0].Move(x, y);

                for (int knot = 1; knot < knotCount; knot++)
                {
                    var offsetToPrev = knots[knot - 1] - knots[knot];
                    var absOffset = offsetToPrev.Abs();

                    if (absOffset.X > 1 || absOffset.Y > 1)
                    {
                        knots[knot] = knots[knot].Move(Math.Sign(offsetToPrev.X) * Math.Min(absOffset.X, 1), Math.Sign(offsetToPrev.Y) * Math.Min(absOffset.Y, 1));
                    }

                    yield return knots[^1];
                }
            }
        }
    }

    public string Solve() => Simulate(2).Distinct().Count().ToString();

    public string SolveAdvanced() => Simulate(10).Distinct().Count().ToString();
}
