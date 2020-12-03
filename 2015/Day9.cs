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
            int bestDistance = worstDistance;

            foreach (var permutation in Tools.Combinatorics.GetPermutations(cityCount))
            {
                int distance = 0;
                int prevCity = 0;

                for (int j = 0; j < cityCount; j++)
                {
                    var city = -1;

                    if (j == 0)
                    {
                        prevCity = permutation[j];
                    }
                    else
                    {
                        city = permutation[j];

                        distance += distances[(cityNames[prevCity], cityNames[city])];
                        prevCity = city;
                    }
                }

                bestDistance = distanceDelegate(bestDistance, distance);
            }

            return bestDistance;
        }
    }
}
