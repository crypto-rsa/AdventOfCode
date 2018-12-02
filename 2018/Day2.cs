using System.Collections.Generic;
using System.IO;
using System.Linq;
using Advent_of_Code;

namespace Advent_of_Code_2018
{
    public class Day2 : IAdventDay
    {
        public string Name => "2. 12. 2018";

        public string Solve()
        {
            var groupCounts = GetInput().Select(s => s.GroupBy(c => c).ToLookup(g => g.Count()));

            return (groupCounts.Count(l => l[2].Any()) * groupCounts.Count(l => l[3].Any())).ToString();
        }

        private static IEnumerable<string> GetInput() => File.ReadAllLines(@"2018/Resources/day2.txt");

        public string SolveAdvanced()
        {
            var inputArray = GetInput().ToArray();

            for (int i = 0; i < inputArray.Length; i++)
            {
                string first = inputArray[i];

                for (int j = i + 1; j < inputArray.Length; j++)
                {
                    string second = inputArray[j];

                    var differentChars = first.Zip(second, (c1, c2) => c1 == c2 ? 0 : 1).Sum();
                    if (differentChars == 1)
                        return string.Join(string.Empty, first.Zip(second, (c1, c2) => c1 == c2 ? c1.ToString() : string.Empty));
                }
            }

            return string.Empty;
        }
    }
}