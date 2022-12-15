using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day15 : IAdventDay
{
    public string Name => "15. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day15.txt");

    private readonly record struct Interval(int Min, int Max);

    private readonly record struct Point(int X, int Y)
    {
        public static Point FromString(string input)
        {
            var match = Regex.Match(input, @"x=(-?\d+), y=(-?\d+)");

            return new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
        }
    }

    private readonly record struct Sensor(Point Position, Point Beacon)
    {
        private int DistanceToBeacon => Math.Abs(Position.X - Beacon.X) + Math.Abs(Position.Y - Beacon.Y);

        public static Sensor FromString(string input)
        {
            var match = Regex.Match(input, @"Sensor at (.*): closest beacon is at (.*)");

            return new Sensor(Point.FromString(match.Groups[1].Value), Point.FromString(match.Groups[2].Value));
        }

        public Interval? GetExcludedColumnsAtRow(int row, bool excludeBeacon)
        {
            int distanceToRow = Math.Abs(Position.Y - row);
            int distanceToEdge = DistanceToBeacon - distanceToRow;

            if (distanceToEdge < 0)
                return null;

            if (excludeBeacon && Beacon.Y == row && Beacon.X == Position.X)
                return null;

            if (excludeBeacon && Beacon.Y == row && Beacon.X < Position.X)
                return new Interval(Position.X - distanceToEdge + 1, Position.X + distanceToEdge);

            if (excludeBeacon && Beacon.Y == row && Beacon.X > Position.X)
                return new Interval(Position.X - distanceToEdge, Position.X + distanceToEdge - 1);

            return new Interval(Position.X - distanceToEdge, Position.X + distanceToEdge);
        }

        public IEnumerable<(int Row, Interval Interval)> GetExcludedIntervals(bool excludeBeacon)
        {
            int distanceToBeacon = DistanceToBeacon;

            for (int row = Position.Y - distanceToBeacon; row <= Position.Y + distanceToBeacon; row++)
            {
                var interval = GetExcludedColumnsAtRow(row, excludeBeacon);

                if (interval != null)
                    yield return (row, interval.Value);
            }
        }
    }

    public string Solve()
    {
        const int row = 2_000_000;
        var sensors = GetInput().SplitToLines().Select(Sensor.FromString).ToArray();

        var intervals = sensors
            .Select(s => s.GetExcludedColumnsAtRow(row, excludeBeacon: true))
            .Where(i => i != null)
            .OrderBy(i => i.Value.Min)
            .Cast<Interval>()
            .ToArray();

        int total = 0;
        Interval? current = null;

        foreach (var interval in intervals)
        {
            if (current != null && interval.Min > current.Value.Max)
            {
                current = null;
            }

            if (current == null)
            {
                total += interval.Max - interval.Min + 1;
                current = interval;

                continue;
            }

            if (interval.Max <= current.Value.Max)
                continue;

            total += interval.Max - current.Value.Max;

            current = new Interval(current.Value.Min, interval.Max);
        }

        return total.ToString();
    }

    public string SolveAdvanced()
    {
        const long max = 4_000_000;
        const long xMultiplier = 4_000_000;

        var sensors = GetInput().SplitToLines().Select(Sensor.FromString).ToArray();

        var intervalsByRow = sensors
            .SelectMany(s => s.GetExcludedIntervals(excludeBeacon: false))
            .ToLookup(i => i.Row, i => i.Interval);

        for (int row = 0; row < max; row++)
        {
            Interval? current = null;

            foreach (var interval in intervalsByRow[row].OrderBy(i => i.Min))
            {
                if (current == null)
                {
                    if (interval.Min == 1)
                        return row.ToString();

                    current = interval;

                    continue;
                }

                if (current.Value.Min <= 0 && current.Value.Max >= max)
                    break;

                if (interval.Max <= current.Value.Max)
                    continue;

                if (interval.Min == current.Value.Max + 2)
                    return ((current.Value.Max + 1) * xMultiplier + row).ToString();

                current = new Interval(current.Value.Min, interval.Max);
            }

            if (current!.Value.Max == max - 1)
                return (max * xMultiplier + row).ToString();
        }

        return "No solution found!";
    }
}
