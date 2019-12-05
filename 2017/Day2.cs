using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day2 : IAdventDay
    {
        public string Name => "2. 12. 2017";

        private static IEnumerable<int[]> GetInput() => System.IO.File.ReadAllLines(@"2017/Resources/day2.txt").Select(s => s.Split('\t').Select(w => int.Parse(w)).ToArray());

        public string Solve()
        {
            return GetInput().Sum( a => a.Max() - a.Min()).ToString();
        }

        public string SolveAdvanced()
        {
            return GetInput().SelectMany(a => a.SelectMany((n1, i1) => a.Select((n2, i2) => i1 != i2 && n1 % n2 == 0 ? n1 / n2 : 0))).Sum().ToString();
        }
    }
}
