using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2016
{
    public class Day15 : IAdventDay
    {
        public string Name => "15. 12. 2016";

        private static Disc[] GetInput() => File.ReadAllLines("2016/Resources/day15.txt").Select(Disc.Create).ToArray();

        private record Disc(int Index, int PositionCount, int StartPosition)
        {
            public static Disc Create(string input)
            {
                var match = System.Text.RegularExpressions.Regex.Match(input, @"Disc #(\d+) has (\d+) positions; at time=0, it is at position (\d+).");

                return new Disc(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            }
        }

        private static long SolveChineseRemainder(IEnumerable<Disc> input)
        {
            var items = input.Select(d => (Modulus: d.PositionCount, Remainder: MakeNonNegative(d.PositionCount - (d.StartPosition + d.Index), d.PositionCount))).ToArray();

            var current = Solve(items[0], items[1]);

            for (int i = 2; i < items.Length; i++)
            {
                current = Solve(current, items[i]);
            }

            return current.Remainder;
        }

        private static (long Modulus, long Remainder) Solve((long Modulus, long Remainder) item1, (long Modulus, long Remainder) item2)
        {
            (long b1, long b2) = SolveBezout();

            long modulus = item1.Modulus * item2.Modulus;
            long remainder = MakeNonNegative(item1.Remainder * b2 * item2.Modulus + item2.Remainder * b1 * item1.Modulus, modulus);

            return (modulus, remainder);

            (long, long) SolveBezout()
            {
                (long r0, long r1) = (item1.Modulus, item2.Modulus);
                (long s0, long s1) = (1, 0);
                (long t0, long t1) = (0, 1);

                while (r1 != 0)
                {
                    long quotient = r0 / r1;
                    (r0, r1) = (r1, r0 - quotient * r1);
                    (s0, s1) = (s1, s0 - quotient * s1);
                    (t0, t1) = (t1, t0 - quotient * t1);
                }

                return (s0, t0);
            }
        }

        private static long MakeNonNegative(long number, long modulus) => ((number % modulus) + modulus) % modulus;

        public string Solve() => SolveChineseRemainder(GetInput()).ToString();

        public string SolveAdvanced()
        {
            var input = GetInput();

            return SolveChineseRemainder(input.Append(new Disc(input.Length + 1, 11, 0))).ToString();
        }
    }
}
