using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2017
{
    public class Day21 : IAdventDay
    {
        public string Name => "21. 12. 2017";

        private static readonly int[] _initialArray = ".#...####".Select(c => c == '#' ? 1 : 0).ToArray();

        private static readonly Dictionary<int, List<List<int>>> _permutations = new Dictionary<int, List<List<int>>>()
        {
            [2] = new List<List<int>>()
            {
                new List<int> {0, 1, 2, 3},
                new List<int> {2, 0, 3, 1},
                new List<int> {3, 2, 1, 0},
                new List<int> {1, 3, 0, 2},
                new List<int> {2, 3, 0, 1},
                new List<int> {1, 0, 3, 2},
                new List<int> {0, 2, 1, 3},
                new List<int> {3, 1, 2, 0},
            },
            [3] = new List<List<int>>()
            {
                new List<int>{0, 1, 2, 3, 4, 5, 6, 7, 8},
                new List<int>{6, 3, 0, 7, 4, 1, 8, 5, 2},
                new List<int>{8, 7, 6, 5, 4, 3, 2, 1, 0},
                new List<int>{2, 5, 8, 1, 4, 7, 0, 3, 6},
                new List<int>{6, 7, 8, 3, 4, 5, 0, 1, 2},
                new List<int>{2, 1, 0, 5, 4, 3, 8, 7, 6},
                new List<int>{0, 3, 6, 1, 4, 7, 2, 5, 8},
                new List<int>{8, 5, 2, 7, 4, 1, 6, 3, 0},
            }
        };

        private readonly Dictionary<(int Size, int ID), (int[] Array, int Rule, int[] Original)> _patterns = GetInput();

        public string Solve() => Iterate(5).ToString();

        public string SolveAdvanced() => Iterate(18).ToString();

        private int Iterate(int iterations)
        {
            var array = _initialArray;

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                array = Enhance(array);
            }

            return array.Sum();
        }

        private static Dictionary<(int, int), (int[], int, int[])> GetInput()
        {
            var dictionary = new Dictionary<(int, int), (int[], int, int[])>();
            var input = System.IO.File.ReadAllLines("2017/Resources/day21.txt").ToList();

            int index = 1;
            foreach (var line in input)
            {
                var items = line.Split("=>");
                var patterns = items.Select(s => s.Trim().Replace("/", string.Empty).Select(c => c == '#' ? 1 : 0).ToArray()).ToArray();
                var size = items[0].Count(c => c == '/') + 1;

                foreach (var permutation in _permutations[size])
                {
                    var id = GetIdFromArray(permutation.Select(i => patterns[0][i]).ToArray());

                    dictionary[(size, id)] = (patterns[1], index, patterns[0]);
                }

                index++;
            }

            return dictionary;
        }

        private static int GetIdFromArray(int[] array) => array.Aggregate(0, (agg, i) => agg * 2 + i);

        private IEnumerable<int[]> Split(int[] array, int division)
        {
            int arraySize = (int)Math.Sqrt(array.Length);
            int blockSize = arraySize / division;

            for (int blockRow = 0; blockRow < division; blockRow++)
            {
                for (int blockCol = 0; blockCol < division; blockCol++)
                {
                    var blockArray = new int[blockSize * blockSize];

                    for (int row = 0; row < blockSize; row++)
                    {
                        for (int col = 0; col < blockSize; col++)
                        {
                            blockArray[row * blockSize + col] = array[(blockRow * blockSize + row) * arraySize + blockCol * blockSize + col];
                        }
                    }

                    yield return Enhance(blockArray);
                }
            }
        }

        private int[] Merge(List<int[]> blocks)
        {
            int arraySize = (int)Math.Sqrt(blocks.Sum(a => a.Length));
            var array = new int[arraySize * arraySize];
            int blocksInRow = (int)Math.Sqrt(blocks.Count);

            for (int blockRow = 0; blockRow < blocksInRow; blockRow++)
            {
                for (int blockCol = 0; blockCol < blocksInRow; blockCol++)
                {
                    var block = blocks[blockRow * blocksInRow + blockCol];
                    int blockSize = (int)Math.Sqrt(block.Length);

                    for (int row = 0; row < blockSize; row++)
                    {
                        for (int col = 0; col < blockSize; col++)
                        {
                            array[(blockRow * blockSize + row) * arraySize + blockCol * blockSize + col] = block[row * blockSize + col];
                        }
                    }
                }
            }

            return array;
        }

        private int[] Enhance(int[] array)
        {
            int arraySize = (int)Math.Sqrt(array.Length);

            if (arraySize == 2 || arraySize == 3)
            {
                var item = _patterns[(arraySize, GetIdFromArray(array))];

                return item.Array;
            }

            int division = arraySize / (arraySize % 2 == 0 ? 2 : 3);

            return Merge(Split(array, division).ToList());
        }
    }
}
