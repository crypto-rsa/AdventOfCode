using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2021
{
    public class Day15 : IAdventDay
    {
        public string Name => "15. 12. 2021";

        private static string[] GetInput() => File.ReadAllLines("2021/Resources/day15.txt");

        private static int GetLowestPathRisk(int multiplier)
        {
            var riskLevel = GetInput().Select(s => s.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            int baseWidth = riskLevel.Select(a => a.Length).Distinct().Single();
            int width = multiplier * baseWidth;

            var distances = new Dictionary<(int, int), int>
            {
                [(0, 0)] = 0,
            };

            var neighbourOffsets = new[] { (-1, 0), (0, +1), (+1, 0), (0, -1) };
            var heap = new Heap<(int Row, int Col)>();

            heap.Push((0, 0), 2 * width);

            while (heap.Count > 0)
            {
                var node = heap.Pop();

                if (node.Row == width - 1 && node.Col == width - 1)
                    return distances[node];

                int distance = distances[node];

                foreach ((int rowOffset, int colOffset) in neighbourOffsets)
                {
                    int targetRow = node.Row + rowOffset;
                    int targetCol = node.Col + colOffset;

                    if (targetRow < 0 || targetRow >= width || targetCol < 0 || targetCol >= width)
                        continue;

                    var nextNode = (targetRow, targetCol);
                    int nextNodeDistance = distance + GetRiskLevel(targetRow, targetCol);

                    if (!distances.TryGetValue(nextNode, out int curDistance) || nextNodeDistance < curDistance)
                    {
                        distances[nextNode] = nextNodeDistance;
                        heap.Push(nextNode, nextNodeDistance + width - targetRow + width - targetCol);
                    }
                }
            }

            return -1;

            int GetRiskLevel(int row, int col)
            {
                int baseLevel = riskLevel[row % baseWidth][col % baseWidth];
                int offset = row / baseWidth + col / baseWidth;

                if (baseLevel + offset < 10)
                    return baseLevel + offset;

                return (baseLevel + offset + 1) % 10;
            }
        }

        public string Solve() => GetLowestPathRisk(1).ToString();

        public string SolveAdvanced() => GetLowestPathRisk(5).ToString();
    }
}
