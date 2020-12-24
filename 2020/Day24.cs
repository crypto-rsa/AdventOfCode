using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Year2020
{
    public class Day24 : IAdventDay
    {
        private record Position(int Row, int Col)
        {
            public IEnumerable<Position> GetNeighbours()
            {
                yield return new Position(Row, Col + 2);
                yield return new Position(Row, Col - 2);
                yield return new Position(Row + 1, Col + 1);
                yield return new Position(Row + 1, Col - 1);
                yield return new Position(Row - 1, Col + 1);
                yield return new Position(Row - 1, Col - 1);
            }

            public Position GetNeighbour(string direction) => direction switch
            {
                "e" => new Position(Row, Col + 2),
                "w" => new Position(Row, Col - 2),
                "se" => new Position(Row + 1, Col + 1),
                "sw" => new Position(Row + 1, Col - 1),
                "ne" => new Position(Row - 1, Col + 1),
                "nw" => new Position(Row - 1, Col - 1),
                _ => throw new System.InvalidOperationException(),
            };

            public Position GetExtreme(Position other, Func<int, int, int> selector)
                => new Position(selector(Row, other.Row), selector(Col, other.Col));
        }

        public string Name => "24. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day24.txt");

        public string Solve() => InitializeFloor().Count.ToString();

        public string SolveAdvanced()
        {
            var blackTiles = InitializeFloor();

            for(int day = 1; day <= 100; day++)
            {
                var next = new HashSet<Position>(blackTiles);
                var extent = GetExtent(next);

                for(int row = extent.Min.Row - 1; row <= extent.Max.Row + 1; row++)
                {
                    for (int col = extent.Min.Col - 2; col <= extent.Max.Col + 2; col++)
                    {
                        var position = new Position(row, col);

                        if (blackTiles.Contains(position) && GetBlackNeighbours(position) is 0 or > 2
                            || !blackTiles.Contains(position) && GetBlackNeighbours(position) is 2)
                        {
                            Toggle(next, position);
                        }

                    }
                }

                blackTiles = next;
            }

            return blackTiles.Count.ToString();

            int GetBlackNeighbours(Position position) => position.GetNeighbours().Count(blackTiles.Contains);
        }

        private HashSet<Position> InitializeFloor()
        {
            var lines = GetInput().Select(ParseLine).ToList();
            var blackTiles = new HashSet<Position>();

            foreach (var line in lines)
            {
                var position = line.Aggregate(new Position(0, 0), (acc, next) => acc.GetNeighbour(next));

                Toggle(blackTiles, position);
            }

            return blackTiles;
        }

        private List<string> ParseLine(string input)
        {
            var directions = new List<string>();
            int pos = 0;

            while (pos < input.Length)
            {
                var match = Regex.Match(input[pos..], "^(e|se|sw|w|nw|ne)");

                directions.Add(match.Groups[1].Value);
                pos += match.Groups[1].Value.Length;
            }

            return directions;
        }

        private static void Toggle(HashSet<Position> set, Position position)
        {
            if (set.Contains(position))
            {
                set.Remove(position);
            }
            else
            {
                set.Add(position);
            }
        }

        private static (Position Min, Position Max) GetExtent(IEnumerable<Position> positions)
            => positions.Aggregate((Min: new Position(int.MaxValue, int.MaxValue), Max: new Position(int.MinValue, int.MinValue)),
                (acc, next) => (acc.Min.GetExtreme(next, Math.Min), acc.Max.GetExtreme(next, Math.Max)));
    }
}
