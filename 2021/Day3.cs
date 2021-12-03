using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day3 : IAdventDay
    {
        public string Name => "3. 12. 2021";

        private static string[] GetInput() => File.ReadAllLines("2021/Resources/day3.txt");

        private static (string MostCommon, string LeastCommon) GetBitsByOccurrence(IEnumerable<string> numbers)
        {
            var bitsByOccurence = numbers
                .SelectMany(s => s.Select((c, i) => (Index: i, Value: c.ToString())))
                .GroupBy(i => i.Index)
                .OrderBy(g => g.Key)
                .Select(Process)
                .ToList();

            return (string.Concat(bitsByOccurence.Select(i => i.MostCommon)), string.Concat(bitsByOccurence.Select(i => i.LeastCommon)));

            (string MostCommon, string LeastCommon) Process(IGrouping<int, (int Index, string Value)> group)
            {
                var sorted = group.ToLookup(i => i.Value);

                var zeros = sorted["0"].Count();
                var ones = sorted["1"].Count();

                return zeros > ones ? ("0", "1") : ("1", "0");
            }
        }

        public string Solve()
        {
            (string mostCommon, string leastCommon) = GetBitsByOccurrence(GetInput());

            int gamma = Convert.ToInt32(mostCommon, fromBase: 2);
            int epsilon = Convert.ToInt32(leastCommon, fromBase: 2);

            return (gamma * epsilon).ToString();
        }

        public string SolveAdvanced()
        {
            int oxideGenerator = Calculate(tuple => tuple.MostCommon);
            int carbonDioxideScrubber = Calculate(tuple => tuple.LeastCommon);

            return (oxideGenerator * carbonDioxideScrubber).ToString();

            int Calculate(Func<(string MostCommon, string LeastCommon), string> selector)
            {
                var remaining = GetInput().ToHashSet();

                for(int index = 0; remaining.Count > 1; index++)
                {
                    var pattern = selector(GetBitsByOccurrence(remaining));

                    remaining = remaining.Where(s => s[index] == pattern[index]).ToHashSet();
                }

                return Convert.ToInt32(remaining.Single(), fromBase: 2);
            }
        }
    }
}
