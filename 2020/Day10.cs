using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day10 : IAdventDay
    {
        public string Name => "10. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day10.txt");

        public string Solve()
        {
            var jolts = GetJolts();
            var lookup = Enumerable.Range(0, jolts.Count).ToLookup(GetDifference);

            return (lookup[1].Count() * lookup[3].Count()).ToString();

            int GetDifference(int index) => index > 0 ? jolts[index] - jolts[index - 1] : int.MaxValue;
        }

        public string SolveAdvanced()
        {
            var jolts = GetJolts();
            var ways = new long[jolts.Count];
            ways[0] = 1;

            for (int i = 1; i < ways.Length; i++)
            {
                for(int j = i - 1; j >= 0 && jolts[i] - jolts[j] <= 3; j--)
                {
                    ways[i] += ways[j];
                }
            }

            return ways[jolts.Count - 1].ToString();
        }

        private static List<int> GetJolts()
        {
            var jolts = GetInput().Select(int.Parse).OrderBy(d => d).ToList();

            jolts.Insert(0, 0);
            jolts.Add(jolts.Max() + 3);

            return jolts;
        }
    }
}
