using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day7 : IAdventDay
    {
        public string Name => "7. 12. 2021";

        private static IEnumerable<int> GetInput() => File.ReadAllText("2021/Resources/day7.txt").Split(',').Select(int.Parse);

        public string Solve()
        {
            // We need to find 'x' for which Σ(a_i - x) is minimized. The derivative is Σsgn(a_i - x), which is minimized when x is the median of all a_i.
            var numbers = GetInput().OrderBy(i => i).ToArray();

            int median1 = numbers[numbers.Length / 2];
            int median2 = numbers[numbers.Length / 2 - 1];

            int sum1 = numbers.Sum(i => Math.Abs(median1 - i));
            int sum2 = numbers.Sum(i => Math.Abs(median2 - i));

            return Math.Min(sum1, sum2).ToString();
        }

        public string SolveAdvanced()
        {
            // We need to find 'x' for which Σ(T(a_i - x)) is minimized, where T(n) is the n-th triangular number (= n*(n - 1)/2).
            // The derivative is Σ(a_i - x - 1/2), which is minimized when x is the average of a_i plus 1/2.
            var numbers = GetInput().ToArray();

            int x1 = (int)numbers.Average();
            int x2 = x1 + 1;

            int sum1 = numbers.Sum(i => T(Math.Abs(i - x1)));
            int sum2 = numbers.Sum(i => T(Math.Abs(i - x2)));

            return Math.Min(sum1, sum2).ToString();

            int T(int n) => n * (n + 1) / 2;
        }
    }
}
