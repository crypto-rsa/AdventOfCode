using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021;

public class Day22 : IAdventDay
{
    public string Name => "22. 12. 2021";

    private static string[] GetInput() => File.ReadAllLines("2021/Resources/day22.txt");

    private record Action(string Toggle, Cuboid Cuboid)
    {
        public static Action Create(string input)
        {
            var parts = input.Split(' ');

            return new Action(parts[0], Cuboid.Create(parts[1]));
        }
    }

    private record Interval(int Min, int Max)
    {
        public Interval GetIntersection(Interval other) => new(Math.Max(Min, other.Min), Math.Min(Max, other.Max));

        public bool IsNonEmpty => Min < Max;

        public bool IsInit => Min >= -50 && Max <= 51;

        public long Length => IsNonEmpty ? Max - Min : 0;
    }

    private record Cuboid(Interval X, Interval Y, Interval Z)
    {
        public static Cuboid Create(string input)
        {
            var match = System.Text.RegularExpressions.Regex.Match(input, @"x=(-?\d+)..(-?\d+),y=(-?\d+)..(-?\d+),z=(-?\d+)..(-?\d+)");

            return new Cuboid(new Interval(Parse(1), Parse(2) + 1), new Interval(Parse(3), Parse(4) + 1), new Interval(Parse(5), Parse(6) + 1));

            int Parse(int index) => int.Parse(match.Groups[index].Value);
        }

        private Cuboid GetIntersection(Cuboid other)
        {
            var x = X.GetIntersection(other.X);
            var y = Y.GetIntersection(other.Y);
            var z = Z.GetIntersection(other.Z);

            return new Cuboid(x, y, z);
        }

        public IEnumerable<Cuboid> Subtract(Cuboid other)
        {
            return SplitByInternal().Where(i => i.IsNonEmpty);

            IEnumerable<Cuboid> SplitByInternal()
            {
                var intersection = GetIntersection(other);

                if (intersection.IsEmpty)
                {
                    yield return this;

                    yield break;
                }

                for (int xPart = 0; xPart < 3; xPart++)
                {
                    for (int yPart = 0; yPart < 3; yPart++)
                    {
                        for (int zPart = 0; zPart < 3; zPart++)
                        {
                            if ((xPart, yPart, zPart) == (1, 1, 1))
                                continue;

                            yield return new Cuboid(GetPart(xPart, 0), GetPart(yPart, 1), GetPart(zPart, 2));
                        }
                    }
                }

                Interval GetPart(int part, int index) => part switch
                {
                    0 => new Interval(GetInterval(index).Min, intersection.GetInterval(index).Min),
                    1 => new Interval(intersection.GetInterval(index).Min, intersection.GetInterval(index).Max),
                    2 => new Interval(intersection.GetInterval(index).Max, GetInterval(index).Max),
                    _ => throw new ArgumentOutOfRangeException(nameof( index )),
                };
            }
        }

        private Interval GetInterval(int index) => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof( index )),
        };

        private bool IsEmpty => !IsNonEmpty;

        private bool IsNonEmpty => X.IsNonEmpty && Y.IsNonEmpty && Z.IsNonEmpty;

        public bool IsInit => X.IsInit && Y.IsInit && Z.IsInit;

        public long Volume => X.Length * Y.Length * Z.Length;
    }

    private static long GetCubesToggledOn(bool initializationOnly)
    {
        var actions = GetInput().Select(Action.Create).Where(a => !initializationOnly || a.Cuboid.IsInit).ToList();
        var onCuboids = new List<Cuboid>();

        foreach ((string toggle, var cuboid) in actions)
        {
            var newCuboids = new List<Cuboid>();

            if (toggle == "on")
            {
                newCuboids.Add(cuboid);
            }

            newCuboids.AddRange(onCuboids.SelectMany(c => c.Subtract(cuboid)));

            onCuboids = newCuboids;
        }

        return onCuboids.Sum(c => c.Volume);
    }

    public string Solve() => GetCubesToggledOn(initializationOnly: true).ToString();

    public string SolveAdvanced() => GetCubesToggledOn(initializationOnly: false).ToString();
}
