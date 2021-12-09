using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day9 : IAdventDay
    {
        public string Name => "9. 12. 2021";

        private static string[] GetInput() => File.ReadAllLines("2021/Resources/day9.txt");

        private class HeightMap
        {
            private readonly int[][] _array = GetInput().Select(s => s.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

            private IEnumerable<(int Row, int Col)> GetLowPoints()
            {
                for (int r = 0; r < _array.Length; r++)
                {
                    for (int c = 0; c < _array[r].Length; c++)
                    {
                        var candidate = (r, c);

                        if (GetNeighbours(candidate).All(i => IsLower(candidate, i)))
                        {
                            yield return candidate;
                        }
                    }
                }
            }

            private bool IsLower((int Row, int Col) point1, (int Row, int Col) point2) => _array[point1.Row][point1.Col] < _array[point2.Row][point2.Col];

            private IEnumerable<(int Row, int Col)> GetNeighbours((int, int) point)
            {
                (int row, int col) = point;

                if (row > 0)
                    yield return (row - 1, col);

                if (col < _array[row].Length - 1)
                    yield return (row, col + 1);

                if (row < _array.Length - 1)
                    yield return (row + 1, col);

                if (col > 0)
                    yield return (row, col - 1);
            }

            public int GetTotalRisk() => GetLowPoints().Sum(i => _array[i.Row][i.Col] + 1);

            public int GetBasinSize()
            {
                var basinIndex = new Dictionary<(int Row, int Col), int>();
                var lowPoints = GetLowPoints().ToList();

                int curBasin = 0;

                foreach (var lowPoint in lowPoints)
                {
                    if (basinIndex.ContainsKey(lowPoint))
                        continue;

                    curBasin++;

                    var queue = new Queue<(int Row, int Col)>();
                    queue.Enqueue(lowPoint);

                    while (queue.Count > 0)
                    {
                        var point = queue.Dequeue();

                        basinIndex[point] = curBasin;

                        foreach (var neighbour in GetNeighbours(point).Where(i => _array[i.Row][i.Col] < 9 && IsLower(point, i)))
                        {
                            queue.Enqueue(neighbour);
                        }
                    }
                }

                return basinIndex.GroupBy(i => i.Value).OrderByDescending(g => g.Count()).Take(3).Aggregate(1, (product, basin) => product * basin.Count());
            }
        }

        public string Solve() => new HeightMap().GetTotalRisk().ToString();

        public string SolveAdvanced() => new HeightMap().GetBasinSize().ToString();
    }
}
