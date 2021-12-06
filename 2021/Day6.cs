using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day6 : IAdventDay
    {
        public string Name => "6. 12. 2021";

        private static IEnumerable<int> GetInput() => File.ReadAllText("2021/Resources/day6.txt").Split(',').Select(int.Parse);

        private static long GetFishCount(int days)
        {
            var counts = GetInput().GroupBy(i => i).ToDictionary(g => g.Key, g => (long)g.Count());

            for (int day = 1; day <= days; day++)
            {
                long spawning = SafeGetCount(0);

                for (int i = 1; i <= 8; i++)
                {
                    counts[i - 1] = SafeGetCount(i);
                }

                counts[6] += spawning;
                counts[8] = spawning;
            }

            return counts.Values.Sum();

            long SafeGetCount(int key)
            {
                counts.TryGetValue(key, out long value);

                return value;
            }
        }

        public string Solve() => GetFishCount(80).ToString();

        public string SolveAdvanced() => GetFishCount(256).ToString();
    }
}
