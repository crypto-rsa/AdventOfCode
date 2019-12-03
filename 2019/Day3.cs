using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day3 : IAdventDay
    {
        private struct Point
        {
            public Point(int x, int y, int steps = 0)
            {
                X = x;
                Y = y;
                Steps = steps;
            }

            public int X { get; }

            public int Y { get; }

            public int Steps { get; }

            public int DistanceFromOrigin => Math.Abs(X) + Math.Abs(Y);
        }

        private struct Segment
        {
            private readonly Point _start;

            private readonly Point _end;

            public Segment(Point start, Point end)
            {
                _start = start;
                _end = end;
            }

            public char Direction => this[0].X == this[1].X ? 'V' : 'H';

            public Point this[int index] => index == 0 ? _start : _end;

            public int Bottom => Math.Min(_start.Y, _end.Y);

            public int Top => Math.Max(_start.Y, _end.Y);

            public int Left => Math.Min(_start.X, _end.X);

            public int Right => Math.Max(_start.X, _end.X);
        }

        public string Name => "3. 12. 2019";

        public string Solve()
        {
            var segments = GetInput().Select(GetSegments).ToArray();
            var closest = FindIntersections(segments[0], segments[1]).Aggregate((Point?)null, PickClosestToOrigin);

            return closest.HasValue ? closest.Value.DistanceFromOrigin.ToString() : string.Empty;
        }

        private List<Segment> GetSegments((char Direction, int Distance)[] directions)
        {
            var curPoint = new Point(0, 0);
            var segments = new List<Segment>();
            int totalSteps = 0;

            foreach (var (direction, distance) in directions)
            {
                totalSteps += distance;

                var nextPoint = direction switch
                {
                    'R' => new Point(curPoint.X + distance, curPoint.Y, totalSteps),
                    'U' => new Point(curPoint.X, curPoint.Y + distance, totalSteps),
                    'L' => new Point(curPoint.X - distance, curPoint.Y, totalSteps),
                    'D' => new Point(curPoint.X, curPoint.Y - distance, totalSteps),
                    _ => throw new InvalidOperationException(),
                };

                segments.Add(new Segment(curPoint, nextPoint));
                curPoint = nextPoint;
            }

            return segments;
        }

        private IEnumerable<Point> FindIntersections(List<Segment> segments1, List<Segment> segments2)
        {
            for (int i = 0; i < segments1.Count; i++)
            {
                for (int j = 0; j < segments2.Count; j++)
                {
                    var point = GetIntersection(segments1[i], segments2[j]);

                    if (point.HasValue && (point.Value.X != 0 || point.Value.Y != 0))
                    {
                        yield return point.Value;
                    }
                }
            }

            static Point? GetIntersection(Segment s1, Segment s2)
            {
                if (s1[0].Equals(s2[0]))
                    return new Point(s1[0].X, s1[0].X, s1[0].Steps + s2[0].Steps);

                if (s1[0].Equals(s2[1]))
                    return new Point(s1[0].X, s1[0].X, s1[0].Steps + s2[1].Steps);

                if (s1[1].Equals(s2[0]))
                    return new Point(s1[1].X, s1[1].Y, s1[1].Steps + s2[0].Steps);

                if (s1[1].Equals(s2[1]))
                    return new Point(s1[1].X, s1[1].Y, s1[1].Steps + s2[1].Steps);

                if (s1.Direction == s2.Direction)
                    return null;

                var vertical = s1.Direction == 'V' ? s1 : s2;
                var horizontal = s1.Direction == 'H' ? s1 : s2;

                if (horizontal.Bottom >= vertical.Bottom && horizontal.Bottom <= vertical.Top && horizontal.Left <= vertical.Left && horizontal.Right >= vertical.Right)
                    return new Point(vertical.Left, horizontal.Bottom, s1[0].Steps + s2[0].Steps + Math.Abs(vertical.Left - horizontal[0].X) + Math.Abs(horizontal.Bottom - vertical[0].Y));

                return null;
            }
        }

        private Point? PickClosestToOrigin(Point? prev, Point next) => prev == null || next.DistanceFromOrigin < prev.Value.DistanceFromOrigin ? next : prev;

        private Point? PickByLeastSteps(Point? prev, Point next) => prev == null || next.Steps < prev.Value.Steps ? next : prev;

        private static (char Direction, int Distance)[][] GetInput()
        {
            return File.ReadAllLines("2019/Resources/day3.txt").Select(s => s.Split(',').Select(Parse).ToArray()).ToArray();

            static (char, int) Parse(string s) => (s[0], int.Parse(s.Substring(1)));
        }

        public string SolveAdvanced()
        {
            var segments = GetInput().Select(GetSegments).ToArray();
            var closest = FindIntersections(segments[0], segments[1]).Aggregate((Point?)null, PickByLeastSteps);

            return closest.HasValue ? closest.Value.Steps.ToString() : string.Empty;
        }
    }
}