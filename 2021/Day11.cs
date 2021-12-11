using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day11 : IAdventDay
    {
        public string Name => "11. 12. 2021";

        private static string[] GetInput() => File.ReadAllLines("2021/Resources/day11.txt");

        private const int Size = 10;

        private static int[][] GetArray()
        {
            var input = GetInput();

            return input.Select(s => s.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
        }

        private static int Iterate(int[][] array, int? iterations)
        {
            var neighbours = GetNeighbours();
            int flashes = 0;

            for (int i = 1; !iterations.HasValue || i <= iterations; i++)
            {
                var flashPositions = new HashSet<(int Row, int Col)>();

                for (int row = 0; row < Size; row++)
                {
                    for (int col = 0; col < Size; col++)
                    {
                        array[row][col]++;

                        if (array[row][col] > 9)
                        {
                            flashPositions.Add((row, col));
                        }
                    }
                }

                var queue = new Queue<(int Row, int Col)>(flashPositions);

                while (queue.Any())
                {
                    var item = queue.Dequeue();

                    foreach (var neighbour in neighbours[item])
                    {
                        array[neighbour.Row][neighbour.Col]++;

                        if (array[neighbour.Row][neighbour.Col] > 9)
                        {
                            if (flashPositions.Add(neighbour))
                            {
                                queue.Enqueue(neighbour);
                            }
                        }
                    }
                }

                for (int row = 0; row < Size; row++)
                {
                    for (int col = 0; col < Size; col++)
                    {
                        if (array[row][col] > 9)
                        {
                            array[row][col] = 0;
                        }
                    }
                }

                if (flashPositions.Count == Size * Size)
                    return i;

                flashes += flashPositions.Count;
            }

            return flashes;
        }

        private static Dictionary<(int Row, int Col), List<(int Row, int Col)>> GetNeighbours()
        {
            var dictionary = new Dictionary<(int, int), List<(int, int)>>();

            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    var list = new List<(int, int)>();

                    for (int rowOffset = -1; rowOffset <= +1; rowOffset++)
                    {
                        for (int colOffset = -1; colOffset <= +1; colOffset++)
                        {
                            if( (rowOffset, colOffset) == (0, 0))
                                continue;

                            if (row + rowOffset < 0 || row + rowOffset >= Size || col + colOffset < 0 || col + colOffset >= Size)
                                continue;

                            list.Add((row + rowOffset, col + colOffset));
                        }
                    }

                    dictionary.Add((row, col), list);
                }
            }

            return dictionary;
        }

        public string Solve() => Iterate(GetArray(), 100).ToString();

        public string SolveAdvanced() => Iterate(GetArray(), null).ToString();
    }
}
