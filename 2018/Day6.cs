using System;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day6 : IAdventDay
    {
        private struct Location
        {
            public Location(int x, int y, int index)
            {
                X = x;
                Y = y;
                Index = index;
            }

            public Location(string s, int index)
            {
                var items = s.Split(", ");
                X = int.Parse(items[0]);
                Y = int.Parse(items[1]);
                Index = index;
            }

            public int Index { get; }

            public int X { get; }

            public int Y { get; }
        }
        public string Name => "6. 12. 2018";

        private static Location[] GetInput() => System.IO.File.ReadAllLines(@"2018/Resources/day6.txt").Select((s, i) => new Location(s, i)).ToArray();

        private int GetDistance(Location location, int x, int y) => Math.Abs(location.X - x) + Math.Abs(location.Y - y);

        public string Solve()
        {
            var coordinates = GetInput();
            var max = new Location(coordinates.Max(i => i.X), coordinates.Max(i => i.Y), -1);
            var min = new Location(coordinates.Min(i => i.X), coordinates.Min(i => i.Y), -1);

            var closest = new int[coordinates.Length];
            var isInfinite = coordinates.ToDictionary(l => l.Index, _ => false);
            for (int j = 0; j <= max.Y; j++)
            {
                for (int i = 0; i <= max.X; i++)
                {
                    var distances = coordinates.GroupBy(l => GetDistance(l, i, j)).OrderBy(g => g.Key).First();
                    if (distances.Count() == 1)
                    {
                        closest[distances.First().Index]++;

                        if (j == 0 || j == max.Y || i == 0 || i == max.X)
                        {
                            isInfinite[distances.First().Index] = true;
                        }
                    }
                }
            }

            return coordinates.Where(l => !isInfinite[l.Index]).Select(l => closest[l.Index]).Max().ToString();
        }

        public string SolveAdvanced()
        {
            var coordinates = GetInput();
            var max = new Location(coordinates.Max(i => i.X), coordinates.Max(i => i.Y), -1);

            int closeLocations = 0;
            for (int j = 0; j <= max.Y; j++)
            {
                for (int i = 0; i <= max.X; i++)
                {
                    var distances = coordinates.Sum(l => GetDistance(l, i, j));
                    if (distances < 10_000)
                    {
                        closeLocations++;
                    }
                }
            }

            return closeLocations.ToString();
        }
    }
}