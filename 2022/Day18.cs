using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day18 : IAdventDay
{
    public string Name => "18. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day18.txt");

    private readonly record struct Cube(int X, int Y, int Z)
    {
        public static Cube FromString(string s)
        {
            var parts = s.Split(',');

            return new Cube(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }

        public IEnumerable<Cube> GetNeighbours()
        {
            yield return new Cube(X - 1, Y + 0, Z + 0);
            yield return new Cube(X + 1, Y + 0, Z + 0);
            yield return new Cube(X + 0, Y - 1, Z + 0);
            yield return new Cube(X + 0, Y + 1, Z + 0);
            yield return new Cube(X + 0, Y + 0, Z - 1);
            yield return new Cube(X + 0, Y + 0, Z + 1);
        }

        public bool IsWithin(Cube min, Cube max) => X >= min.X && X <= max.X && Y >= min.Y && Y <= max.Y && Z >= min.Z && Z <= max.Z;

        public static Cube Min { get; } = new(int.MinValue, int.MinValue, int.MinValue);

        public static Cube Max { get; } = new(int.MaxValue, int.MaxValue, int.MaxValue);
    }

    private static ISet<Cube> GetCubes() => GetInput()
        .SplitToLines()
        .Select(Cube.FromString)
        .ToHashSet();

    public string Solve()
    {
        var cubes = GetCubes();

        return cubes
            .SelectMany(c => c.GetNeighbours())
            .Count(c => !cubes.Contains(c))
            .ToString();
    }

    public string SolveAdvanced()
    {
        var cubes = GetCubes();
        var (min, max) = GetExtremes();

        var waterMin = new Cube(min.X - 1, min.Y - 1, min.Z - 1);
        var waterMax = new Cube(max.X + 1, max.Y + 1, max.Z + 1);

        var water = new HashSet<Cube>();
        var candidates = new HashSet<Cube> { waterMin };

        while (candidates.Count > 0)
        {
            var candidate = candidates.First();

            water.Add(candidate);

            foreach (var neighbour in candidate.GetNeighbours())
            {
                if (!neighbour.IsWithin(waterMin, waterMax))
                    continue;

                if (cubes.Contains(neighbour) || water.Contains(neighbour))
                    continue;

                candidates.Add(neighbour);
            }

            candidates.Remove(candidate);
        }

        return cubes
            .SelectMany(c => c.GetNeighbours())
            .Count(water.Contains)
            .ToString();

        (Cube Min, Cube Max) GetExtremes() => cubes.Aggregate((Min: Cube.Max, Max: Cube.Min), (cur, next) => (Extend(cur.Min, next, Math.Min), Extend(cur.Max, next, Math.Max)));

        Cube Extend(Cube current, Cube next, Func<int, int, int> selector) => new(selector(current.X, next.X), selector(current.Y, next.Y), selector(current.Z, next.Z));
    }
}
