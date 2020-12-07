using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day17 : IAdventDay
    {
        public string Name => "17. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day17.txt");

        private const int TotalVolume = 150;

        public string Solve() => GetWays().Count().ToString();

        public string SolveAdvanced() => GetWays().GroupBy(i => i.Count).OrderBy(g => g.Key).First().Count().ToString();

        public IEnumerable<(int Sum, int LeastUsed, int Count)> GetWays()
        {
            var capacities = GetInput().Select(int.Parse).OrderByDescending(d => d).ToArray();

            var ways = new Stack<(int Sum, int LeastUsed, int Count)>();
            ways.Push((0, -1, 0));

            while (ways.Any())
            {
                var item = ways.Pop();

                if (item.Sum == TotalVolume)
                    yield return item;

                for(int i = item.LeastUsed + 1; i < capacities.Length; i++)
                {
                    if( item.Sum + capacities[i] <= TotalVolume)
                    {
                        ways.Push((item.Sum + capacities[i], i, item.Count + 1));
                    }
                }
            }
        }
    }
}
