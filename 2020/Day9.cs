using System;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day9 : IAdventDay
    {
        public string Name => "9. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day9.txt");

        private const int Offset = 25;

        public string Solve()
        {
            var numbers = GetInput().Select(s => long.Parse(s)).ToArray();
            var index = Enumerable.Range(Offset, numbers.Length - Offset).First(i => !IsSum(i, numbers));

            return numbers[index].ToString();
        }

        public string SolveAdvanced()
        {
            var numbers = GetInput().Select(s => long.Parse(s)).ToArray();
            var index = Enumerable.Range(Offset, numbers.Length - Offset).First(i => !IsSum(i, numbers));

            for (int start = 0; start < numbers.Length; start++)
            {
                long sum = numbers[start];

                for (int end = start + 1; end < numbers.Length; end++)
                {
                    sum += numbers[end];

                    if (sum == numbers[index])
                        return GetSum(start, end).ToString();

                    if (sum > numbers[index])
                        break;
                }
            }

            return string.Empty;

            (long Min, long Max) GetExtremes(int start, int end)
                => numbers[start..(end + 1)].Aggregate((Min: long.MaxValue, Max: long.MinValue), (cur, next) => (Math.Min(cur.Min, next), Math.Max(cur.Max, next)));

            long GetSum(int start, int end)
            {
                var (min, max) = GetExtremes(start, end);

                return min + max;
            }
        }

        private static bool IsSum(int index, long[] numbers)
        {
            for (int i = index - Offset; i < index; i++)
            {
                for (int j = i + 1; j < index; j++)
                {
                    if (numbers[index] == numbers[i] + numbers[j])
                        return true;
                }
            }

            return false;
        }
    }
}
