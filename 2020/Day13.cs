using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day13 : IAdventDay
    {
        public string Name => "13. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day13.txt");

        public string Solve()
        {
            var input = GetInput();
            var timestamp = long.Parse(input[0]);
            var intervals = input[1].Split(',').Where(s => s != "x").Select(int.Parse).ToList();
            var earliest = intervals.Select(i => (Interval: i, Waiting: i - timestamp % i)).OrderBy(i => i.Waiting).First();

            return (earliest.Interval * earliest.Waiting).ToString();
        }

        public string SolveAdvanced()
        {
            var intervals = GetInput()[1].Split(',').Select((s, i) => (Text: s, Index: i))
                .Where(i => i.Text != "x").Select(i => (Value: long.Parse(i.Text), i.Index)).ToList();

            long m = intervals.Aggregate(1L, (acc, next) => acc * next.Value);
            var chineseRemainder = intervals.Select(GetRemainder).ToArray();

            long x = chineseRemainder.Aggregate(0L, (acc, next) => (acc + next.N * next.Y * next.R) % m);

            return x.ToString();

            (long M, long N, long Y, long R) GetRemainder((long Value, int Index) item)
            {
                long mod = item.Value;
                long r = mod - item.Index % mod;
                long n = m / mod;
                long product = 0;

                for(int y = 1; ; y++)
                {
                    product = (product + n) % mod;
                    if (product == 1)
                        return (mod, n, y, r);
                }
            }
        }
    }
}
