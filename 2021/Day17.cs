using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021;

public class Day17 : IAdventDay
{
    public string Name => "17. 12. 2021";

    private static string GetInput() => File.ReadAllText("2021/Resources/day17.txt");

    private class Target
    {
        private int MinX { get; }

        private int MaxX { get; }

        private int MinY { get; }

        private int MaxY { get; }

        public Target()
        {
            var match = System.Text.RegularExpressions.Regex.Match(GetInput(), @"target area: x=(\d+)..(\d+), y=(-?\d+)..(-?\d+)");

            MinX = Parse(1);
            MaxX = Parse(2);
            MinY = Parse(3);
            MaxY = Parse(4);

            int Parse(int index) => int.Parse(match.Groups[index].Value);
        }

        public (int MaxHeight, int SolutionCount) Solve()
        {
            // The Y position after 's' steps is: y * (s + 1) - s * (s - 1) / 2.
            // Since it has to be with [MinY, MaxY], this gives a quadratic equation for 's'.

            double d = Math.Pow(-2 * MaxX - 1, 2) + 8 * (MaxX - MinX);

            if (d < 0)
                return (0, 0);

            int s1 = (int)Math.Ceiling((2 * MaxX + 1 - Math.Sqrt(d)) / 2);
            int s2 = (int)Math.Floor((2 * MaxX + 1 + Math.Sqrt(d)) / 2);

            // The X velocity drops to zero after 'x' steps and stays there; 'x' is the initial X velocity.
            // The terminal X position in that case is given by x * (x + 1) / 2.
            // Again, this has to be within [MinX, MaxX], which gives a quadratic equation for 'x'.

            var limitX = GetLimitX().ToHashSet();
            bool hasLimitX = limitX.Any();

            var solutions = new HashSet<(int, int)>();
            int maxY = int.MinValue;

            for (int s = Math.Max(1, s1); s <= s2; s++)
            {
                double z = s * (s - 1) / 2.0;

                int x1 = (int)Math.Ceiling((MinX + z) / s);
                int x2 = (int)Math.Floor((MaxX + z) / s);

                if (!hasLimitX && s > x2)
                    continue;

                int y1 = (int)Math.Ceiling((MinY + z) / s);
                int y2 = (int)Math.Floor((MaxY + z) / s);

                if (y2 >= y1 && y2 > maxY)
                {
                    maxY = y2;
                }

                var testX = s > x2 ? limitX : Enumerable.Range(x1, x2 - x1 + 1);

                foreach (int xx in testX)
                {
                    for (int yy = y1; yy <= y2; yy++)
                    {
                        solutions.Add((xx, yy));
                    }
                }
            }

            int maxHeight = maxY * (maxY + 1) / 2;

            return (maxHeight, solutions.Count);

            IEnumerable<int> GetLimitX()
            {
                double lowerDiscriminant = 1 + 8.0 * MinX;
                double upperDiscriminant = 1 + 8.0 * MaxX;

                int lowerMin = (int)Math.Floor((-1 - Math.Sqrt(lowerDiscriminant)) / 2);
                int lowerMax = (int)Math.Ceiling((-1 + Math.Sqrt(lowerDiscriminant)) / 2);

                int upperMin = (int)Math.Ceiling((-1 - Math.Sqrt(upperDiscriminant)) / 2);
                int upperMax = (int)Math.Floor((-1 + Math.Sqrt(upperDiscriminant)) / 2);

                if (upperMin > upperMax)
                    yield break;

                if (MaxX < 0 && lowerMin < 0)
                {
                    for (int x = upperMin; x <= lowerMin; x++)
                        yield return x;
                }

                if (MinX > 0 && lowerMax > 0 && upperMax > 0)
                {
                    for (int x = lowerMax; x <= upperMax; x++)
                        yield return x;
                }
            }
        }
    }

    public string Solve() => new Target().Solve().MaxHeight.ToString();

    public string SolveAdvanced() => new Target().Solve().SolutionCount.ToString();
}
