using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day10 : IAdventDay
{
    public string Name => "10. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day10.txt");

    private static readonly Map _map = new();

    private class Map
    {
        public Map()
        {
            var map = GetInput().ParseAsGrid(c => c);

            var startPosition = map
                .Select((a, r) => (Row: r, Column: Array.FindIndex(a, c => c == 'S')))
                .Single(i => i.Row != -1 && i.Column != -1);

            var visited = new HashSet<(int, int)>();

            var distances = new Dictionary<(int Row, int Column), int>
            {
                [startPosition] = 0,
            };

            var heap = new Heap<(int Row, int Column)>();
            heap.Push(startPosition, 0);

            while (heap.Count > 0)
            {
                var position = heap.Pop();

                visited.Add(position);

                int steps = distances[position];

                foreach (var direction in GetDirections())
                {
                    var newPosition = (Row: position.Row + direction.Row, Column: position.Column + direction.Column);

                    if (visited.Contains(newPosition))
                        continue;

                    if (newPosition.Row < 0 || newPosition.Row >= map.Length)
                        continue;

                    if (newPosition.Column < 0 || newPosition.Column >= map[newPosition.Row].Length)
                        continue;

                    if (!Connects(direction.Symbol, map[position.Row][position.Column], map[newPosition.Row][newPosition.Column]))
                        continue;

                    if (!distances.TryGetValue(newPosition, out int curDistance) || steps + 1 < curDistance)
                    {
                        distances[newPosition] = steps + 1;
                        heap.Push(newPosition, steps + 1);
                    }
                }

                if (position == startPosition)
                {
                    map[position.Row][position.Column] = "|-LJ7F".First(IsStartChar);
                }
            }

            LongestPath = distances.Values.Max();

            var schema = new System.Text.StringBuilder();

            for (int r = 0; r < map.Length; r++)
            {
                bool isOut = true;
                char edgeStart = '0';

                for (int c = 0; c < map[r].Length; c++)
                {
                    if (!distances.ContainsKey((r, c)) && !isOut)
                    {
                        TilesInside++;
                    }

                    if (distances.ContainsKey((r, c)))
                    {
                        switch (edgeStart, map[r][c])
                        {
                            case (_, '-' or '.'):
                                break;

                            case (_, 'L' or 'F'):
                                edgeStart = map[r][c];

                                break;

                            case ('L', 'J') or ('F', '7'):
                                edgeStart = '0';

                                break;

                            case (_, '|') or ('L' or 'F', _):
                                isOut = !isOut;

                                break;
                        }
                    }
                }

                schema.AppendLine();
            }

            File.WriteAllText(@"C:\Users\Pavel\Downloads\map.txt", schema.ToString());

            IEnumerable<(char Symbol, int Row, int Column)> GetDirections()
            {
                yield return ('E', 0, +1);
                yield return ('S', +1, 0);
                yield return ('W', 0, -1);
                yield return ('N', -1, 0);
            }

            bool Connects(char direction, char pipeFrom, char pipeTo) => (direction, pipeFrom, pipeTo)
                is ('S', 'S' or '|' or '7' or 'F', '|' or 'L' or 'J')
                or ('N', 'S' or '|' or 'L' or 'J', '|' or '7' or 'F')
                or ('E', 'S' or '-' or 'L' or 'F', '-' or 'J' or '7')
                or ('W', 'S' or '-' or 'J' or '7', '-' or 'L' or 'F');

            bool IsStartChar(char c)
            {
                var neighbours = distances.Where(p => p.Value == 1).ToArray();
                var offsets = neighbours.Select(p => (p.Key.Row - startPosition.Row, p.Key.Column - startPosition.Column)).ToArray();
                var directions = offsets.Select(p => GetDirections().Single(i => (i.Row, i.Column) == (p.Item1, p.Item2)).Symbol).ToArray();

                return Enumerable.Range(0, 2).All(i => Connects(directions[i], c, map[neighbours[i].Key.Row][neighbours[i].Key.Column]));
            }
        }

        public int LongestPath { get; }

        public int TilesInside { get; }
    }

    public string Solve() => _map.LongestPath.ToString();

    public string SolveAdvanced() => _map.TilesInside.ToString();
}
