using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2019;

public class Day20 : IAdventDay
{
    public string Name => "20. 12. 2019";

    private static string[] GetInput() => File.ReadAllLines("2019/Resources/day20.txt");

    private class Board
    {
        private readonly string[] _map;

        private readonly (int Row, int Col) _entry;

        private readonly (int Row, int Col) _exit;

        private readonly Dictionary<(int Row, int Col), (int Row, int Col)> _portals = new();

        private readonly Dictionary<(int Row, int Col), int> _distanceEstimate = new();

        private readonly HashSet<(int Row, int Col)> _innerPortals = new();

        public Board()
        {
            _map = GetInput();

            var portals = new List<(string Name, int Row, int Col)>();

            for (int row = 0; row < _map.Length; row++)
            {
                for (int col = 0; col < _map[row].Length; col++)
                {
                    if (IsChar(row, col))
                    {
                        if (IsChar(row, col + 1))
                        {
                            string name = $"{_map[row][col]}{_map[row][col + 1]}";

                            foreach (int i in new[] { +2, -1 })
                            {
                                if (!IsFree(row, col + i))
                                    continue;

                                portals.Add((name, row, col + i));

                                if (col != 0 && col + 1 != _map[row].Length - 1)
                                {
                                    _innerPortals.Add((row, col + i));
                                }
                            }
                        }

                        if (IsChar(row + 1, col))
                        {
                            string name = $"{_map[row][col]}{_map[row + 1][col]}";

                            foreach (int i in new[] { +2, -1 })
                            {
                                if (!IsFree(row + i, col))
                                    continue;

                                portals.Add((name, row + i, col));

                                if (row != 0 && row + 1 != _map.Length - 1)
                                {
                                    _innerPortals.Add((row + i, col));
                                }
                            }
                        }
                    }
                }
            }

            _entry = GetCoords(portals.Single(i => i.Name == "AA"));
            _exit = GetCoords(portals.Single(i => i.Name == "ZZ"));

            foreach (var group in portals.GroupBy(i => i.Name).Where(g => g.Count() == 2))
            {
                var items = group.ToArray();

                _portals[GetCoords(items[0])] = GetCoords(items[1]);
                _portals[GetCoords(items[1])] = GetCoords(items[0]);
            }

            CalculateDistanceEstimate();

            bool IsChar(int row, int col) => IsValid(row, col) && _map[row][col] is >= 'A' and <= 'Z';

            (int, int) GetCoords((string, int Row, int Col) item) => (item.Row, item.Col);
        }

        private void CalculateDistanceEstimate()
        {
            var queue = new Queue<(int Row, int Col)>();
            queue.Enqueue(_exit);

            _distanceEstimate[_exit] = 0;

            while (queue.Count > 0)
            {
                var position = queue.Dequeue();

                int stepsToNext = _distanceEstimate[position] + 1;

                foreach (var nextPosition in GetNeighbourPositions(position.Row, position.Col, 0, (r, c) => IsValid(r, c) && _map[r][c] is '.' or '#', false))
                {
                    var nextKey = (nextPosition.Row, nextPosition.Col);

                    if (_distanceEstimate.TryGetValue(nextKey, out int curDistance) && stepsToNext >= curDistance)
                        continue;

                    _distanceEstimate[nextKey] = stepsToNext;
                    queue.Enqueue(nextKey);
                }
            }
        }

        private bool IsFree(int row, int col) => IsValid(row, col) && _map[row][col] == '.';

        private bool IsValid(int row, int col) => row >= 0 && row < _map.Length && col >= 0 && col < _map[row].Length;

        public int FindShortestPath(bool recursive)
        {
            var entry = (_entry.Row, _entry.Col, 0);
            var exit = (_exit.Row, _exit.Col, 0);

            var distances = new Dictionary<(int Row, int Col, int Level), int>
            {
                [entry] = 0,
            };

            var heap = new Heap<(int Row, int Col, int Level)>();
            heap.Push(entry, 0);

            while (heap.Count > 0)
            {
                var position = heap.Pop();

                if (position == exit)
                    return distances[position];

                int steps = distances[position];

                foreach (var nextPosition in GetNeighbourPositions(position.Row, position.Col, position.Level, IsFree, recursive))
                {
                    if (!distances.TryGetValue(nextPosition, out int curDistance) || steps + 1 < curDistance)
                    {
                        distances[nextPosition] = steps + 1;
                        heap.Push(nextPosition, steps + _distanceEstimate[(nextPosition.Row, nextPosition.Col)]);
                    }
                }
            }

            return -1;
        }

        private IEnumerable<(int Row, int Col, int Level)> GetNeighbourPositions(int row, int col, int level, Func<int, int, bool> isReachable, bool recursive)
        {
            var neighbours = new (int Row, int Col)[]
            {
                (row, col + 1),
                (row + 1, col),
                (row, col - 1),
                (row - 1, col),
            };

            foreach (var position in neighbours.Where(i => isReachable(i.Row, i.Col)))
            {
                yield return (position.Row, position.Col, level);
            }

            if (!_portals.TryGetValue((row, col), out var portalEnd))
                yield break;

            if (recursive)
            {
                if (_innerPortals.Contains((row, col)))
                    yield return (portalEnd.Row, portalEnd.Col, level + 1);
                else if (level > 0)
                    yield return (portalEnd.Row, portalEnd.Col, level - 1);
            }
            else
            {
                yield return (portalEnd.Row, portalEnd.Col, level);
            }
        }
    }

    public string Solve() => new Board().FindShortestPath(recursive: false).ToString();

    public string SolveAdvanced() => new Board().FindShortestPath(recursive: true).ToString();
}
