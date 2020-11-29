using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day9 : IAdventDay
    {
        public string Name => "9. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day9.txt");

        public string Solve() => SolveInternal(int.MaxValue, Math.Min).ToString();

        public string SolveAdvanced() => SolveInternal(0, Math.Max).ToString();

        private int SolveInternal(int worstDistance, Func<int, int, int> distanceDelegate)
        {
            var distances = new Dictionary<(string, string), int>();

            foreach (var item in GetInput())
            {
                var match = System.Text.RegularExpressions.Regex.Match(item, @"(\w+) to (\w+) = (\d+)");
                var value = int.Parse(match.Groups[3].Value);

                distances.Add((match.Groups[1].Value, match.Groups[2].Value), value);
                distances.Add((match.Groups[2].Value, match.Groups[1].Value), value);
            }

            var cityNames = distances.Keys.Select(i => i.Item1).Distinct().OrderBy(s => s).Select((s, i) => (i, s)).ToDictionary(i => i.Item1, i => i.Item2);

            int cityCount = (int)Math.Ceiling(Math.Sqrt(distances.Count));
            var factorial = Enumerable.Range(0, cityCount + 1).Select(i => Tools.Combinatorics.Factorial(i)).ToArray();

            int bestDistance = worstDistance;

            for (int permutation = 0; permutation < factorial[cityCount]; permutation++)
            {
                int distance = 0;
                int n = permutation;
                var cities = Enumerable.Range(0, cityCount).ToList();

                int prevCity = -1;

                for (int j = 0; j < cityCount; j++)
                {
                    int d = (int)(n / factorial[cityCount - 1 - j]);
                    var city = -1;

                    if (j == 0)
                    {
                        prevCity = cities[d];
                    }
                    else
                    {
                        city = cities[d];

                        distance += distances[(cityNames[prevCity], cityNames[city])];
                        prevCity = city;
                    }

                    n -= (int)(d * factorial[cityCount - 1 - j]);
                    cities.RemoveAt(d);
                }

                bestDistance = distanceDelegate(bestDistance, distance);
            }

            return bestDistance;
        }
    }
}
