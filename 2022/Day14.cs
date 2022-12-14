using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day14 : IAdventDay
{
    public string Name => "14. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day14.txt");

    private readonly record struct Point(int X, int Y)
    {
        public static Point FromString(string input)
        {
            var parts = input.Split(',');

            return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public IEnumerable<Point> GetPointsTo(Point destination)
        {
            var difference = new Point(destination.X - X, destination.Y - Y);

            if (difference.X * difference.Y != 0)
                throw new InvalidOperationException("Diagonal lines are not allowed!");

            for (int i = 0; i <= Math.Max(Math.Abs(difference.X), Math.Abs(difference.Y)); i++)
            {
                yield return new Point(X + Math.Sign(difference.X) * i, Y + Math.Sign(difference.Y) * i);
            }
        }
    }

    private static HashSet<Point> GetMap()
    {
        var map = new HashSet<Point>();

        foreach (string line in GetInput().SplitToLines())
        {
            var endPoints = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries).Select(Point.FromString).ToArray();

            var startPoint = endPoints[0];

            foreach (var endPoint in endPoints.Skip(1))
            {
                foreach (var p in startPoint.GetPointsTo(endPoint))
                {
                    map.Add(p);
                }

                startPoint = endPoint;
            }
        }

        return map;
    }

    private static Point AddUnit(ISet<Point> map, bool hasHardBottom, int bottom)
    {
        var position = new Point(500, 0);

        while (true)
        {
            if (hasHardBottom ? position.Y == bottom - 1 : position.Y > bottom)
                break;

            var down = position with { Y = position.Y + 1 };
            var downLeft = down with { X = down.X - 1 };
            var downRight = down with { X = down.X + 1 };

            if (!map.Contains(down))
            {
                position = down;
            }
            else if (!map.Contains(downLeft))
            {
                position = downLeft;
            }
            else if (!map.Contains(downRight))
            {
                position = downRight;
            }
            else
            {
                break;
            }
        }

        if (hasHardBottom || position.Y <= bottom)
        {
            map.Add(position);
        }

        return position;
    }

    public string Solve()
    {
        var map = GetMap();
        int bottom = map.Max(p => p.Y);

        int total = 0;

        for (;; total++)
        {
            var next = AddUnit(map, hasHardBottom: false, bottom);

            if (next.Y > bottom)
                break;
        }

        return total.ToString();
    }

    public string SolveAdvanced()
    {
        var map = GetMap();
        int bottom = map.Max(p => p.Y) + 2;
        var top = new Point(500, 0);

        int total = 0;

        while (true)
        {
            var next = AddUnit(map, hasHardBottom: true, bottom);

            total++;

            if (next.Equals(top))
                break;
        }

        return total.ToString();
    }
}
