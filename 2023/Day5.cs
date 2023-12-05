using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day5 : IAdventDay
{
    public string Name => "5. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day5.txt");

    private record Interval(long Start, long Length)
    {
        public long End => Start + Length;

        public bool Contains(long index) => index >= Start && index < End;

        public static Interval FromStartEnd(long start, long end) => new( start, end - start );
    }

    private class Mapping(long destinationStart, long sourceStart, long length)
    {
        public Interval Source { get; } = new( sourceStart, length );

        private Interval Destination { get; } = new( destinationStart, length );

        public long? GetDestination(long source) => Source.Contains(source)
            ? Destination.Start + (source - Source.Start)
            : null;

        public static Mapping FromIdentity(long start, long end)
            => new( start, start, end - start );
    }

    private class Map
    {
        private readonly Dictionary<(string Source, string Destination), List<Mapping>> _maps = new();

        public long GetDestination(long sourceIndex, string sourceName, string destinationName)
        {
            string currentMapName = sourceName;
            long currentIndex = sourceIndex;

            while (currentMapName != destinationName)
            {
                var nextMap = _maps.Single(p => p.Key.Source == currentMapName);
                currentIndex = nextMap.Value.Select(m => m.GetDestination(currentIndex)).FirstOrDefault(i => i.HasValue) ?? currentIndex;
                currentMapName = nextMap.Key.Destination;
            }

            return currentIndex;
        }

        public void AddMap(string[] lines)
        {
            var match = System.Text.RegularExpressions.Regex.Match(lines[0], @"(\w+)-to-(\w+) map:");
            var mappings = lines
                .Skip(1)
                .Select(line => line.Split(' '))
                .Select(parts => new Mapping(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2])))
                .ToList();

            mappings.Sort((m1, m2) => Comparer<long>.Default.Compare(m1.Source.Start, m2.Source.Start));

            long index = 0;

            if (mappings.First().Source.Start is var firstMappingStart and > 0)
            {
                mappings.Insert(0, Mapping.FromIdentity(0, firstMappingStart));
                index = firstMappingStart;
            }

            for (int i = 1; i < mappings.Count; i++)
            {
                if (mappings[i].Source.Start > mappings[i - 1].Source.End)
                {
                    mappings.Insert(i, Mapping.FromIdentity(mappings[i - 1].Source.End, mappings[i].Source.Start));
                    i++;
                }

                index = mappings[i].Source.End;
            }

            mappings.Add(Mapping.FromIdentity(index, long.MaxValue));

            _maps.Add((match.Groups[1].Value, match.Groups[2].Value), mappings.OrderBy(m => m.Source.Start).ToList());
        }

        public List<Interval> GetIntervals(Interval sourceInterval, List<Mapping> sourceMappings)
        {
            var newIntervals = new List<Interval>();

            long start = sourceInterval.Start;

            while (start < sourceInterval.End)
            {
                var sourceMapping = sourceMappings.First(m => m.Source.Contains(start));
                long end = Math.Min(sourceMapping.Source.End, sourceInterval.End) - 1;

                newIntervals.Add(Interval.FromStartEnd(sourceMapping.GetDestination(start)!.Value, sourceMapping.GetDestination(end)!.Value));

                start = end + 1;
            }

            return newIntervals;
        }

        public List<Mapping> GetMap(string sourceName, out string destinationName)
        {
            var item = _maps.First(p => p.Key.Source == sourceName);
            destinationName = item.Key.Destination;

            return item.Value;
        }
    }

    public string Solve()
    {
        var blocks = GetInput().ParseByBlankLines().ToList();
        var map = new Map();

        foreach (var block in blocks.Skip(1))
        {
            map.AddMap(block.ToArray());
        }

        return blocks[0]
            .Single()
            .Split("seeds: ", StringSplitOptions.RemoveEmptyEntries)[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => map.GetDestination(long.Parse(s), "seed", "location"))
            .Min()
            .ToString();
    }

    public string SolveAdvanced()
    {
        var blocks = GetInput().ParseByBlankLines().ToList();
        var map = new Map();

        foreach (var block in blocks.Skip(1))
        {
            map.AddMap(block.ToArray());
        }

        var seeds = blocks[0]
            .Single()
            .Split("seeds: ", StringSplitOptions.RemoveEmptyEntries)[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();

        var seedMappings = new List<Mapping>();

        for (int i = 0; i < seeds.Length; i += 2)
        {
            seedMappings.Add(new Mapping(seeds[i], seeds[i], seeds[i + 1]));
        }

        string mapName = "seed";
        var sourceIntervals = seedMappings.Select(m => m.Source).OrderBy(i => i.Start).ToList();

        while (mapName != "location")
        {
            var sourceMappings = map.GetMap(mapName, out string destinationName);
            var destinationIntervals = new List<Interval>();

            foreach (var interval in sourceIntervals)
            {
                destinationIntervals.AddRange(map.GetIntervals(interval, sourceMappings));
            }

            sourceIntervals = destinationIntervals;
            mapName = destinationName;
        }

        return sourceIntervals.Select(i => i.Start).Min().ToString();
    }
}
