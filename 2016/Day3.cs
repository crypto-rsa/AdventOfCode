using System;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day3 : IAdventDay
    {
        public string Name => "3. 12. 2016";

        private static string[][] GetInput() => File.ReadAllLines("2016/Resources/day3.txt").Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToArray();

        private readonly struct Triple
        {
            public Triple(string[] a)
                : this(a[0], a[1], a[2])
            {
            }

            public Triple(string a, string b, string c)
            {
                A = int.Parse(a);
                B = int.Parse(b);
                C = int.Parse(c);
            }

            private int A { get; }

            private int B { get; }

            private int C { get; }

            public bool IsTriangle => A < B + C && B < A + C && C < A + B;
        }

        public string Solve() => GetInput().Select(a => new Triple(a)).Count(t => t.IsTriangle).ToString();

        public string SolveAdvanced()
        {
            var input = GetInput();

            return Enumerable.Range(0, input.Length)
                .Select(i => new Triple(GetItem(3 * i), GetItem(3 * i + 1), GetItem(3 * i + 2)))
                .Count(t => t.IsTriangle)
                .ToString();

            string GetItem(int index)
            {
                var col = index / input.Length;
                var row = index % input.Length;

                return input[row][col];
            }
        }
    }
}
