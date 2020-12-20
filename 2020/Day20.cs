using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day20 : IAdventDay
    {
        private const int TileSize = 10;

        private const int ClearTileSize = TileSize - 2;

        private const int Symmetries = 8;

        private class Tile
        {
            private readonly char[][] _fields;

            public Tile(string input)
            {
                var lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                ID = long.Parse(lines[0][5..9]);
                _fields = lines.Skip(1).Select(s => s.ToCharArray()).ToArray();

                InitializeEdges();
            }

            public Tile(Tile source, int symmetry)
            {
                ID = source.ID;
                _fields = new char[TileSize][];

                for (int row = 0; row < TileSize; row++)
                {
                    _fields[row] = new char[TileSize];

                    for (int col = 0; col < TileSize; col++)
                    {
                        _fields[row][col] = GetField(source._fields, row, col, symmetry);
                    }
                }

                InitializeEdges();
            }

            public static char GetField(char[][] array, int row, int col, int symmetry) => symmetry switch
            {
                0 => array[row][col],
                1 => array[^(col + 1)][row],
                2 => array[^(row + 1)][^(col + 1)],
                3 => array[col][^(row + 1)],
                4 => array[^(row + 1)][col],
                5 => array[row][^(col + 1)],
                6 => array[^(col + 1)][^(row + 1)],
                7 => array[col][row],
                _ => throw new InvalidOperationException(),
            };

            public long ID { get; }

            public List<string> HorizontalEdges { get; } = new List<string>();

            public List<string> VerticalEdges { get; } = new List<string>();

            public char this[(int Row, int Col) position] => _fields[position.Row][position.Col];

            private void InitializeEdges()
            {
                HorizontalEdges.Add(new string(_fields[0]));
                HorizontalEdges.Add(new string(_fields[TileSize - 1]));

                VerticalEdges.Add(new string(Enumerable.Range(0, TileSize).Select(i => _fields[i][0]).ToArray()));
                VerticalEdges.Add(new string(Enumerable.Range(0, TileSize).Select(i => _fields[i][TileSize - 1]).ToArray()));
            }

            public IEnumerable<Tile> GetSymmetricTiles() => Enumerable.Range(0, Symmetries).Select(i => new Tile(this, i));
        }

        private record Assignment(int Size, Dictionary<(int Row, int Col), Tile> Values)
        {
            public Assignment(int size)
                : this(size, new Dictionary<(int Row, int Col), Tile>())
            {
            }

            public static Assignment Extend(Assignment initial, (int, int) position, Tile tile)
            {
                var dictionary = new Dictionary<(int Row, int Col), Tile>(initial.Values)
                {
                    [position] = tile,
                };

                return new Assignment(initial.Size, dictionary);
            }

            public bool IsComplete => Values.Count == Size * Size;

            public long ProductOfCorners => Values[(0, 0)].ID * Values[(0, Size - 1)].ID * Values[(Size - 1, 0)].ID * Values[(Size - 1, Size - 1)].ID;
        }

        private class Monster
        {
            private const string Pattern = "                  # \r\n#    ##    ##    ###\r\n #  #  #  #  #  #   ";

            private const int Rows = 3;

            private const int Cols = 20;

            private readonly char[][] _fields;

            private Monster()
            {
                _fields = Pattern.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToCharArray()).ToArray();
            }

            private Monster(Monster source, int symmetry)
            {
                bool isTransposed = symmetry is 1 or 3 or 6 or 7;
                var (rows, cols) = isTransposed ? (Cols, Rows) : (Rows, Cols);

                _fields = new char[rows][];

                for (int row = 0; row < rows; row++)
                {
                    _fields[row] = new char[cols];

                    for (int col = 0; col < cols; col++)
                    {
                        _fields[row][col] = Tile.GetField(source._fields, row, col, symmetry);
                    }
                }
            }

            public static IEnumerable<Monster> GetMonsters()
            {
                var monster = new Monster();

                return Enumerable.Range(0, Symmetries).Select(s => new Monster(monster, s));
            }

            public void ApplyTo(char[][] image)
            {
                int count = 0;

                for(int top = 0; top < image.Length - _fields.Length; top++)
                {
                    for(int left = 0; left < image[0].Length - _fields[0].Length; left++)
                    {
                        bool matches = true;

                        for(int row = 0; row < _fields.Length; row++)
                        {
                            for(int col = 0; col < _fields[row].Length; col++)
                            {
                                if(_fields[row][col] == '#' && image[top + row][left + col] != '#')
                                {
                                    matches = false;
                                    break;
                                }
                            }

                            if (!matches)
                                break;
                        }

                        if (matches)
                        {
                            for (int row = 0; row < _fields.Length; row++)
                            {
                                for (int col = 0; col < _fields[row].Length; col++)
                                {
                                    if (_fields[row][col] == '#' )
                                    {
                                        image[top + row][left + col] = 'O';
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public string Name => "20. 12. 2020";

        private static string GetInput() => System.IO.File.ReadAllText("2020/Resources/day20.txt");

        public string Solve()
        {
            var allTiles = GetInput().Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Tile(s)).SelectMany(t => t.GetSymmetricTiles()).ToList();

            int size = (int)Math.Sqrt(allTiles.Count / Symmetries);
            var solution = DoAssignment(new Assignment(size), allTiles);

            return solution.IsComplete ? solution.ProductOfCorners.ToString() : "Not found!";
        }

        public string SolveAdvanced()
        {
            var allTiles = GetInput().Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Tile(s)).SelectMany(t => t.GetSymmetricTiles()).ToList();

            int size = (int)Math.Sqrt(allTiles.Count / Symmetries);
            var solution = DoAssignment(new Assignment(size), allTiles);

            int reducedSize = size * ClearTileSize;
            var finalImage = CreateFinalImage(solution, reducedSize);

            foreach( var monster in Monster.GetMonsters() )
            {
                monster.ApplyTo(finalImage);
            }

            return finalImage.Sum(a => a.Count(c => c == '#')).ToString();
        }

        private char[][] CreateFinalImage(Assignment assignment, int size)
        {
            var array = Enumerable.Range(0, size).Select(_ => new char[size]).ToArray();

            foreach (var pair in assignment.Values)
            {
                int top = pair.Key.Row * ClearTileSize;
                int left = pair.Key.Col * ClearTileSize;

                for (int row = 0; row < ClearTileSize; row++)
                {
                    for (int col = 0; col < ClearTileSize; col++)
                    {
                        array[top + row][left + col] = pair.Value[(row + 1, col + 1)];
                    }
                }
            }

            return array;
        }

        private static Assignment DoAssignment(Assignment initial, List<Tile> allTiles)
        {
            if (initial.IsComplete)
                return initial;

            int size = initial.Size;
            var usedIds = initial.Values.Values.Select(t => t.ID).ToHashSet();

            var position = GetFirstUnusedPosition();

            foreach (var tile in allTiles.Where(t => CanExtend(position, t)))
            {
                var tiles = allTiles.Where(t => t.ID != tile.ID).ToList();
                var assignment = DoAssignment(Assignment.Extend(initial, position, tile), tiles);

                if (assignment.IsComplete)
                    return assignment;
            }

            return initial;

            (int Row, int Col) GetFirstUnusedPosition()
            {
                for (int row = 0; row < size; row++)
                {
                    for (int col = 0; col < size; col++)
                    {
                        if (!initial.Values.ContainsKey((row, col)))
                            return (row, col);
                    }
                }

                throw new InvalidOperationException();
            }

            bool CanExtend((int Row, int Col) position, Tile tile)
            {
                if (usedIds.Contains(tile.ID))
                    return false;

                if (initial.Values.TryGetValue((position.Row - 1, position.Col), out var top) && top.HorizontalEdges[1] != tile.HorizontalEdges[0])
                    return false;

                if (initial.Values.TryGetValue((position.Row + 1, position.Col), out var bottom) && bottom.HorizontalEdges[0] != tile.HorizontalEdges[1])
                    return false;

                if (initial.Values.TryGetValue((position.Row, position.Col - 1), out var left) && left.VerticalEdges[1] != tile.VerticalEdges[0])
                    return false;

                if (initial.Values.TryGetValue((position.Row, position.Col + 1), out var right) && right.VerticalEdges[0] != tile.VerticalEdges[1])
                    return false;

                return true;
            }
        }
    }
}
