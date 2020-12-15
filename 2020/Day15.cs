using System.Collections.Generic;

namespace Advent_of_Code.Year2020
{
    public class Day15 : IAdventDay
    {
        public string Name => "15. 12. 2020";

        private readonly static int[] _startingNumbers = new int[] { 16, 12, 1, 0, 15, 7, 11 };

        public string Solve() => Iterate(2020).ToString();

        public string SolveAdvanced() => Iterate(30000000).ToString();

        private static int Iterate(int iterations)
        {
            var occurrences = new Dictionary<int, (int Count, int Prev, int PrevPrev)>();
            int lastNumber = -1;

            for (int i = 1; i <= iterations; i++)
            {
                int currentNumber = i <= _startingNumbers.Length
                    ? _startingNumbers[i - 1] : GetNext(lastNumber);

                lastNumber = currentNumber;

                Store(currentNumber, i);
            }

            return lastNumber;

            void Store(int number, int turn)
            {
                occurrences.TryGetValue(number, out var item);
                occurrences[number] = item.Count == 0 ? (1, turn, 0) : (2, turn, item.Prev);
            }

            int GetNext(int number)
            {
                if (!occurrences.TryGetValue(number, out var item) || item.Count < 2)
                    return 0;

                return item.Prev - item.PrevPrev;
            }
        }
    }
}
