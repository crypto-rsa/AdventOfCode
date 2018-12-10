using System;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day10 : IAdventDay
    {
        private struct Vector
        {
            public Vector(string line)
            {
                var items = line.Split(',');
                X = int.Parse(items[0].Trim());
                Y = int.Parse(items[1].Trim());
            }

            public Vector(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }

            public int Y { get; }

            public static Vector operator +(Vector first, Vector second)
                => new Vector(first.X + second.X, first.Y + second.Y);

            public static Vector operator -(Vector first, Vector second)
                => new Vector(first.X - second.X, first.Y - second.Y);
        }
        private class Particle
        {
            public Particle(string line)
            {
                var items = line.Split('<', '>');
                Position = new Vector(items[1]);
                Velocity = new Vector(items[3]);
            }

            public Vector Position { get; private set; }

            public Vector Velocity { get; }

            public void StepForward()
            {
                Position += Velocity;
            }

            public void StepBackward()
            {
                Position -= Velocity;
            }
        }

        public string Name => "10. 12. 2018";

        private static Particle[] GetInput() => System.IO.File.ReadAllLines(@"2018/Resources/day10.txt").Select(s => new Particle(s)).ToArray();

        // private static 

        public string Solve()
        {
            PrintMessage();

            return "<check the console output>";
        }

        private int PrintMessage()
        {
            var particles = GetInput();
            var oldBounds = GetBounds();
            int iterations = 0;

            while (true)
            {
                foreach (var particle in particles)
                {
                    particle.StepForward();
                }

                var newBounds = GetBounds();
                if (GetArea(newBounds) >= GetArea(oldBounds))
                {
                    foreach (var particle in particles)
                    {
                        particle.StepBackward();
                    }

                    break;
                }

                oldBounds = newBounds;
                iterations++;
            };

            Print();

            return iterations;

            (Vector Min, Vector Max) GetBounds()
            {
                int minX = int.MaxValue;
                int maxX = int.MinValue;
                int minY = int.MaxValue;
                int maxY = int.MinValue;

                foreach (var particle in particles)
                {
                    minX = Math.Min(minX, particle.Position.X);
                    maxX = Math.Max(maxX, particle.Position.X);
                    minY = Math.Min(minY, particle.Position.Y);
                    maxY = Math.Max(maxY, particle.Position.Y);
                }

                return (new Vector(minX, minY), new Vector(maxX, maxY));
            }

            long GetArea((Vector Min, Vector Max) bounds)
                => Math.Abs(bounds.Max.X - bounds.Min.X) * (long)Math.Abs(bounds.Max.Y - bounds.Min.Y);

            void Print()
            {
                var points = particles.Select(p => new Vector(p.Position.X - oldBounds.Min.X, p.Position.Y - oldBounds.Min.Y)).ToHashSet();

                int width = Math.Abs(oldBounds.Max.X - oldBounds.Min.X);
                int height = Math.Abs(oldBounds.Max.Y - oldBounds.Min.Y);

                for (int row = 0; row <= height; row++)
                {
                    for (int col = 0; col <= width; col++)
                    {
                        Console.Write(points.Contains(new Vector(col, row)) ? '#' : ' ');
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }

        public string SolveAdvanced()
        {
            return PrintMessage().ToString();
        }
    }
}