using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2015
{
    public class Day14 : IAdventDay
    {
        private const int TotalTime = 2503;

        private class Reindeer
        {
            public Reindeer(string input)
            {
                var match = Regex.Match(input, @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.");

                Name = match.Groups[1].Value;
                Speed = int.Parse(match.Groups[2].Value);
                FlyTime = int.Parse(match.Groups[3].Value);
                RestTime = int.Parse(match.Groups[4].Value);
            }

            public string Name { get; }

            public int Speed { get; }

            public int FlyTime { get; }

            public int RestTime { get; }

            public int GetDistanceAfter(int time)
            {
                var div = Math.DivRem(time, FlyTime + RestTime, out var rem);

                return Speed * (div * FlyTime + Math.Min(rem, FlyTime));
            }
        }

        public string Name => "14. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day14.txt");

        public string Solve() => GetInput().Select(s => new Reindeer(s)).Max(r => r.GetDistanceAfter(TotalTime)).ToString();

        public string SolveAdvanced()
        {
            var reindeers = GetInput().Select(s => new Reindeer(s)).ToList();
            var points = reindeers.ToDictionary(r => r, _ => 0);

            for (int i = 1; i <= TotalTime; i++)
            {
                foreach( var reindeer in reindeers.GroupBy(r => r.GetDistanceAfter(i)).OrderByDescending(g => g.Key).First() )
                {
                    points[reindeer]++;
                }
            }

            return points.Values.Max().ToString();
        }
    }
}
