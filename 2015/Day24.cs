using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2015
{
    public class Day24 : IAdventDay
    {
        public string Name => "24. 12. 2015";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2015/Resources/day24.txt");

        public string Solve()
        {
            var numbers = GetInput().Select(int.Parse).OrderBy(i => i).ToList();
            int target = numbers.Sum() / 3;

            return FindSetWithMinimumProduct(numbers, target).ToString();
        }

        public string SolveAdvanced()
        {
            var numbers = GetInput().Select(int.Parse).OrderBy(i => i).ToList();
            int target = numbers.Sum() / 4;

            return FindSetWithMinimumProduct(numbers, target).ToString();
        }

        private long FindSetWithMinimumProduct(List<int> numbers, int target)
        {
            int count = numbers.Count;
            var partialSum = new int[count][];

            for (int i = 0; i < count; i++)
            {
                partialSum[i] = new int[count - i + 1];

                for(int j = 1; j < partialSum[i].Length; j++)
                {
                    partialSum[i][j] = j == 1 ? numbers[i] : partialSum[i][j - 1] + numbers[i + j - 1];
                }
            }

            for (int elementCount = 1; elementCount < count; elementCount++)
            {
                if (partialSum[count - elementCount][elementCount] < target)
                    continue;

                var included = Enumerable.Range(0, count).Select(i => i < elementCount).ToArray();
                int currentSum = partialSum[0][elementCount];
                long minimumProduct = long.MaxValue;

                while(true)
                {
                    if (currentSum == target)
                    {
                        long product = Enumerable.Range(0, count).Aggregate(1L, (acc, next) => acc * (included[next] ? numbers[next] : 1));

                        if (product < minimumProduct)
                        {
                            minimumProduct = product;
                        }
                    }

                    do
                    {
                        int moveIndex = -1;
                        int moveSum = 0;
                        int moveCount = 0;
                        for (int i = count - 1; i >= 0; i--)
                        {
                            if (included[i])
                            {
                                moveCount++;
                                moveSum += numbers[i];
                            }

                            if (i < count - 1 && included[i] && !included[i + 1])
                            {
                                moveIndex = i;
                                break;
                            }
                        }

                        if (moveIndex == -1)
                            break;

                        currentSum = currentSum - moveSum + partialSum[moveIndex + 1][moveCount];

                        for (int i = moveIndex; i < count; i++)
                        {
                            included[i] = i > moveIndex && i < moveIndex + moveCount + 1;
                        }
                    }
                    while (currentSum > target);

                    if (currentSum > target)
                        break;
                }

                if(minimumProduct < long.MaxValue)
                    return minimumProduct;
            }

            return long.MaxValue;
        }
    }
}
