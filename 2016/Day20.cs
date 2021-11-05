using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day20 : IAdventDay
    {
        public string Name => "20. 12. 2016";

        private static List<Interval> GetInput() => File.ReadAllLines("2016/Resources/day20.txt").Select(Split).OrderBy(i => i.Min).ToList();

        private record Interval(long Min, long Max)
        {
            public long Count => Max - Min + 1;
        }

        private static Interval Split(string s)
        {
            var parts = s.Split('-');

            return new Interval(long.Parse(parts[0]), long.Parse(parts[1]));
        }

        private static List<Interval> GetIntervals()
        {
            var inputIntervals = GetInput();
            var outputIntervals = new List<Interval>();
            (long min, long max) = (0, 0);

            if (inputIntervals.Count == 0)
                return outputIntervals;

            for (int processed = 0; processed <= inputIntervals.Count; processed++)
            {
                var currentInterval = processed < inputIntervals.Count ? inputIntervals[processed] : null;

                if (processed == 0)
                {
                    (min, max) = (currentInterval!.Min, currentInterval.Max);

                    continue;
                }

                if (currentInterval == null || currentInterval.Min > max + 1)
                {
                    outputIntervals.Add(new Interval(min, max));
                }

                if (currentInterval != null && currentInterval.Min <= max + 1)
                {
                    max = Math.Max(max, currentInterval.Max);
                }
                else if (currentInterval != null)
                {
                    (min, max) = (currentInterval.Min, currentInterval.Max);
                }
            }

            return outputIntervals;
        }

        public string Solve()
        {
            var intervals = GetIntervals();
            long smallest = 0;

            foreach ((long min, long max) in intervals)
            {
                if (smallest >= min && smallest <= max)
                {
                    smallest = max + 1;
                }
            }

            return smallest.ToString();
        }

        public string SolveAdvanced() => ((long)uint.MaxValue + 1 - GetIntervals().Sum(i => i.Count)).ToString();
    }
}
