using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021;

public class Day20 : IAdventDay
{
    public string Name => "20. 12. 2021";

    private static string GetInput() => File.ReadAllText("2021/Resources/day20.txt");

    private class Image
    {
        private readonly bool[] _pattern;

        private HashSet<(int Row, int Col)> _lightPixels = new();

        private int _minRow;

        private int _maxRow;

        private int _minCol;

        private int _maxCol;

        private bool _isOutsidePixelLight;

        private static readonly (int RowOffset, int ColOffset, int Power)[] _offsets = new[]
        {
            (-1, -1, 256),
            (-1, 0, 128),
            (-1, +1, 64),
            (0, -1, 32),
            (0, 0, 16),
            (0, +1, 8),
            (+1, -1, 4),
            (+1, 0, 2),
            (+1, +1, 1),
        };

        public Image()
        {
            var parts = GetInput().Split($"{Environment.NewLine}{Environment.NewLine}");
            var lines = parts[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            _pattern = string.Concat(parts[0].Split(Environment.NewLine)).Select(c => c == '#').ToArray();
            _isOutsidePixelLight = _pattern[0];

            _minRow = 0;
            _minCol = 0;
            _maxRow = lines.Length - 1;

            for (int row = 0; row < lines.Length; row++)
            {
                for (int col = 0; col < lines[row].Length; col++)
                {
                    if (lines[row][col] == '#')
                    {
                        _lightPixels.Add((row, col));
                    }

                    _maxCol = Math.Max(_maxCol, lines[row].Length - 1);
                }
            }
        }

        private void Iterate()
        {
            var newLightPixels = new HashSet<(int, int)>();

            _isOutsidePixelLight = _isOutsidePixelLight ? _pattern.Last() : _pattern.First();

            for (int row = _minRow - 3; row <= _maxRow + 3; row++)
            {
                for (int col = _minCol - 3; col <= _maxCol + 3; col++)
                {
                    if (_pattern[GetIndex(row, col)])
                    {
                        newLightPixels.Add((row, col));
                    }
                }
            }

            _lightPixels = newLightPixels;
            _minRow -= 3;
            _maxRow += 3;
            _minCol -= 3;
            _maxCol += 3;

            int GetIndex(int row, int col)
            {
                int index = 0;

                foreach (var (rowOffset, colOffset, power) in _offsets)
                {
                    int newRow = row + rowOffset;
                    int newCol = col + colOffset;

                    bool isLight = (newRow < _minRow || newRow > _maxRow || newCol < _minCol || newCol > _maxCol)
                        ? _isOutsidePixelLight
                        : _lightPixels.Contains((newRow, newCol));

                    if(isLight)
                    {
                        index += power;
                    }
                }

                return index;
            }
        }

        public int GetLightPixelCount(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Iterate();
            }

            return _lightPixels.Count;
        }
    }

    public string Solve() => new Image().GetLightPixelCount(2).ToString();

    public string SolveAdvanced() => new Image().GetLightPixelCount(50).ToString();
}
