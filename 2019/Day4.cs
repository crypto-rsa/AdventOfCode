using System.Collections.Generic;

namespace Advent_of_Code.Year2019
{
    public class Day4 : IAdventDay
    {
        public string Name => "4. 12. 2019";

        public string Solve() => GetCount(needsExactDouble: false).ToString();

        public string SolveAdvanced() => GetCount(needsExactDouble: true).ToString();

        private static (int Min, int Max) GetInput() => (128392, 643281);

        private int GetCount( bool needsExactDouble )
        {
            var (min, max) = GetInput();

            var array = GetArray(min);
            var maxArray = GetArray(max);
            int total = 0;

            do
            {
                total += Matches() ? 1 : 0;

                Increment();
            }
            while (IsLessOrEqual());

            return total;

            static int[] GetArray(int number)
            {
                var digits = new List<int>();
                int cur = number;

                while (cur > 0)
                {
                    digits.Add(cur % 10);
                    cur /= 10;
                }

                return digits.ToArray();
            }

            bool IsLessOrEqual()
            {
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    if (array[i] < maxArray[i])
                        return true;

                    if (array[i] > maxArray[i])
                        return false;
                }

                return true;
            }

            void Increment()
            {
                int digitToIncrement = 0;
                for (int i = 0; i < array.Length && array[i] == 9; i++)
                {
                    array[i] = 0;
                    digitToIncrement++;
                }

                if (digitToIncrement < array.Length)
                {
                    array[digitToIncrement]++;
                }
            }

            bool Matches()
            {
                bool hasDouble = false;
                bool hasExactDouble = false;

                for (int i = 1; i < array.Length; i++)
                {
                    if (array[i - 1] < array[i])
                        return false;

                    bool isDouble = array[i - 1] == array[i];
                    hasDouble |= isDouble;
                    hasExactDouble |= isDouble && (i <= 1 || array[i - 2] != array[i - 1]) && (i >= array.Length - 1 || array[i + 1] != array[i]);
                }

                return hasDouble && (hasExactDouble || !needsExactDouble);
            }
        }
    }
}