using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day14 : IAdventDay
{
    public string Name => "14. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day14.txt");

    private record Coordinates(int X, int Y);

    private record Robot
    {
        public Robot(string line)
        {
            var parts = line.Split('=', ' ', ',');

            Position = new Coordinates(X: int.Parse(parts[1]), Y: int.Parse(parts[2]));
            Velocity = new Coordinates(X: int.Parse(parts[4]), Y: int.Parse(parts[5]));
        }

        private Coordinates Velocity { get; }

        public Coordinates Position { get; private set; }

        public void DoMove(int width, int height)
        {
            int x = (Position.X + Velocity.X + width) % width;
            int y = (Position.Y + Velocity.Y + height) % height;

            Position = new Coordinates(x, y);
        }

        public void DoMoves(int count, int width, int height)
        {
            int x = ((Position.X + Velocity.X * count) % width + width) % width;
            int y = ((Position.Y + Velocity.Y * count) % height + height) % height;

            Position = new Coordinates(x, y);
        }

        public (int X, int Y) GetQuadrant(int width, int height)
        {
            int x = Math.Sign(Position.X - width / 2);
            int y = Math.Sign(Position.Y - height / 2);

            return (x, y);
        }
    }

    private static string GetSafetyFactor(int width, int height, int moves)
    {
        var robots = GetInput().SplitToLines().Select(s => new Robot(s)).ToList();

        foreach (var robot in robots)
        {
            robot.DoMoves(moves, width, height);
        }

        return robots
            .GroupBy(r => r.GetQuadrant(width, height))
            .Where(g => g.Key is not (0, _) and not (_, 0))
            .Aggregate(1, (acc, g) => acc * g.Count())
            .ToString();
    }

    public string Solve() => GetSafetyFactor(101, 103, 100);

    public string SolveAdvanced()
    {
        var robots = GetInput().SplitToLines().Select(s => new Robot(s)).ToList();

        const int width = 101;
        const int height = 103;

        for (int i = 1;; i++)
        {
            foreach (var robot in robots)
            {
                robot.DoMove(width, height);
            }

            var positions = robots.Select(r => r.Position).ToHashSet();
            int largestComponentSize = 0;

            while (positions.Count > 0)
            {
                var component = new HashSet<Coordinates> { positions.First() };
                positions.Remove(component.First());

                var queue = new Queue<Coordinates>(component);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    foreach (var neighbour in GetNeighbours(current))
                    {
                        if (positions.Remove(neighbour))
                        {
                            component.Add(neighbour);
                            queue.Enqueue(neighbour);
                        }
                    }
                }

                largestComponentSize = Math.Max(largestComponentSize, component.Count);
            }

            if (largestComponentSize > robots.Count / 5)
                return i.ToString();
        }

        IEnumerable<Coordinates> GetNeighbours(Coordinates c)
        {
            yield return c with { X = c.X - 1 };
            yield return c with { X = c.X + 1 };
            yield return c with { Y = c.Y - 1 };
            yield return c with { Y = c.Y + 1 };
        }
    }
}
