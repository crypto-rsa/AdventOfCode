using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day12 : IAdventDay
    {
        public string Name => "12. 12. 2019";

        public string Solve() => Run(iterations: 1000).ToString();

        public string SolveAdvanced() => GetCycleLength().ToString();

        private struct Vector
        {
            public Vector(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Vector(int[] components)
                : this(components[0], components[1], components[2])
            {
            }

            public int X { get; }

            public int Y { get; }

            public int Z { get; }

            public int[] Components => new int[] { X, Y, Z };

            public int this[int component] => component switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new ArgumentOutOfRangeException(nameof(component)),
            };

            public int SumOfAbsoluteValues => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

            public static Vector operator +(Vector first, Vector second) => new Vector(first.X + second.X, first.Y + second.Y, first.Z + second.Z);

            public static Vector operator *(int scalar, Vector vector) => new Vector(scalar * vector.X, scalar * vector.Y, scalar * vector.Z);

            public static Vector Parse(string input) => new Vector(input.Trim('<', '>').Replace(" ", string.Empty).Split(',').Select(s => int.Parse(s[2..])).ToArray());

            public override string ToString() => $"<x={X}, y={Y}, z={Z}>";
        }

        private class Moon
        {
            public Moon(string position)
            {
                Position = Vector.Parse(position);
                Velocity = new Vector();
            }

            public Moon(Moon other)
            {
                Position = other.Position;
                Velocity = other.Velocity;
            }

            public Vector Position { get; private set; }

            public Vector Velocity { get; private set; }

            public int Energy => Position.SumOfAbsoluteValues * Velocity.SumOfAbsoluteValues;

            public void UpdatePosition() => Position += Velocity;

            public void UpdateVelocity(Moon other)
            {
                Velocity += new Vector(Position.Components.Zip(other.Position.Components, GetNewVelocity).ToArray());

                static int GetNewVelocity(int i, int j)
                {
                    return (i - j) switch
                    {
                        int d when d < 0 => +1,
                        int d when d > 0 => -1,
                        _ => 0,
                    };
                }
            }
        }

        private static List<Moon> GetInput() => File.ReadAllLines("2019/Resources/day12.txt").Select(s => new Moon(s)).ToList();

        private int Run(int iterations)
        {
            var moons = GetInput();

            for (int iteration = 1; iteration <= iterations; iteration++)
            {
                DoIteration(moons);
            }

            return moons.Sum(m => m.Energy);
        }

        private long GetCycleLength()
        {
            var moons = GetInput();
            var startState = moons.ConvertAll(m => new Moon(m));
            var cycleLength = new long[3];

            for( int iteration = 1; ; iteration++ )
            {
                DoIteration(moons);

                for (int i = 0; i < 3; i++)
                {
                    if (cycleLength[i] > 0)
                        continue;

                    bool equal = moons.Zip(startState, (m1, m2) => m1.Position[i] == m2.Position[i] && m1.Velocity[i] == m2.Velocity[i]).All(b => b);
                    if (equal)
                    {
                        cycleLength[i] = iteration;
                    }
                }

                if (cycleLength.All(i => i > 0))
                    break;
            }

            return Tools.Combinatorics.LCM(cycleLength).ToNumber();
        }

        private static void DoIteration(List<Moon> moons)
        {
            for (int i = 0; i < moons.Count; i++)
            {
                for (int j = 0; j < moons.Count; j++)
                {
                    moons[i].UpdateVelocity(moons[j]);
                }
            }

            for (int i = 0; i < moons.Count; i++)
            {
                moons[i].UpdatePosition();
            }
        }
    }
}
