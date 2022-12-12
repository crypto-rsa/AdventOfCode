using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022
{
    public class Day12 : IAdventDay
    {
        public string Name => "12. 12. 2022";

        private static string GetInput() => File.ReadAllText("2022/Resources/day12.txt");

        private readonly record struct Point(int Row, int Col)
        {
            public static Point operator +(Point first, Point second) => new(first.Row + second.Row, first.Col + second.Col);
        }

        private static Dictionary<Point, int> GetDistances(string[] map, Point start, bool climbUp)
        {
            var distance = new Dictionary<Point, int>
            {
                [start] = 0,
            };

            var queue = new Queue<Point>();
            queue.Enqueue(start);

            while (queue.Any())
            {
                var pos = queue.Dequeue();
                var steps = distance[pos];

                foreach (var neighbour in GetNeighbours(pos))
                {
                    if (distance.TryGetValue(neighbour, out int neighbourDistance) && neighbourDistance <= steps + 1)
                        continue;

                    distance[neighbour] = steps + 1;
                    queue.Enqueue(neighbour);
                }
            }

            return distance;

            IEnumerable<Point> GetNeighbours(Point point)
            {
                foreach (var offset in new Point[] { new(0, -1), new(-1, 0), new(0, +1), new(+1, 0) })
                {
                    var newPos = point + offset;

                    if (newPos.Row < 0 || newPos.Row >= map.Length || newPos.Col < 0 || newPos.Col >= map[newPos.Row].Length)
                        continue;

                    var (from, to) = climbUp ? (point, newPos) : (newPos, point);

                    if (GetElevation(from) + 1 < GetElevation(to))
                        continue;

                    yield return newPos;
                }
            }

            int GetElevation(Point point) => map[point.Row][point.Col] switch
            {
                'S' => 0,
                'E' => 25,
                var c => c - 'a',
            };
        }

        private static IEnumerable<Point> GetPoints(string[] map, char label) => map
            .Select((s, i) => new Point(i, s.IndexOf(label, StringComparison.Ordinal)))
            .Where(p => p.Col >= 0);

        public string Solve()
        {
            var map = GetInput().SplitToLines().ToArray();
            var start = GetPoints(map, 'S').Single();
            var end = GetPoints(map, 'E').Single();

            return GetDistances(map, start, climbUp: true).TryGetValue(end, out int stepsToEnd) ? stepsToEnd.ToString() : "No solution found";
        }

        public string SolveAdvanced()
        {
            var map = GetInput().SplitToLines().ToArray();
            var start = GetPoints(map, 'S').Single();
            var end = GetPoints(map, 'E').Single();
            var distances = GetDistances(map, end, climbUp: false);

            return GetPoints(map, 'a').Append(start).Select(p => distances[p]).Min().ToString();
        }
    }
}
