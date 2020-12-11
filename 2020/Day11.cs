using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2020
{
    public class Day11 : IAdventDay
    {
        public string Name => "11. 12. 2020";

        private static string[] GetInput() => System.IO.File.ReadAllLines("2020/Resources/day11.txt");

        private const char Empty = 'L';
        private const char Occupied = '#';
        private const char Floor = '.';

        public string Solve() => Iterate(4, GetOccupiedNeighbours).ToString();

        public string SolveAdvanced() => Iterate(5, GetOccupiedNeighboursInDirection).ToString();

        private static int Iterate(int occupiedMinimum, Func<char[][], int, int, int> getOccupiedNeighbours)
        {
            var lines = GetInput();

            var arrays = new char[][][]
            {
                lines.Select(s => s.ToCharArray()).ToArray(),
                lines.Select(s => new char[s.Length]).ToArray(),
            };

            int current = 0;

            while (true)
            {
                var input = arrays[current];
                var output = arrays[1 - current];

                current = 1 - current;

                if (!Iterate(input, output, occupiedMinimum, getOccupiedNeighbours))
                    break;
            }

            return arrays[current].Sum(a => a.Count(c => c == Occupied));
        }

        private static bool Iterate(char[][] input, char[][] output, int occupiedMinimum, Func<char[][], int, int, int> getOccupiedNeighbours)
        {
            bool anyChange = false;

            for (int row = 0; row < input.Length; row++)
            {
                for (int col = 0; col < input[row].Length; col++)
                {
                    output[row][col] = (input[row][col], getOccupiedNeighbours(input, row, col)) switch
                    {
                        (Empty, 0) => Occupied,
                        (Occupied, int o) when o >= occupiedMinimum => Empty,
                        _ => input[row][col],
                    };

                    anyChange |= input[row][col] != output[row][col];
                }
            }

            return anyChange;
        }

        private static bool IsValid(char[][] input, int row, int col, (int RowOffset, int ColOffset) offset)
            => row + offset.RowOffset >= 0 && row + offset.RowOffset < input.Length 
            && col + offset.ColOffset >= 0 && col + offset.ColOffset < input[row].Length;

        private static IEnumerable<(int RowOffset, int ColOffset)> GetNeighbours()
        {
            yield return (-1, -1);
            yield return (0, -1);
            yield return (+1, -1);
            yield return (-1, 0);
            yield return (+1, 0);
            yield return (-1, +1);
            yield return (0, +1);
            yield return (+1, +1);
        }

        private static int GetOccupiedNeighbours(char[][] input, int row, int col )
            => GetNeighbours().Where(i => IsValid(input, row, col, i)).Count(i => input[row + i.RowOffset][col + i.ColOffset] == Occupied);

        private static int GetOccupiedNeighboursInDirection(char[][] input, int row, int col)
        {
            return GetNeighbours().Select(GetInDirection).Count(c => c == Occupied);

            char GetInDirection((int Row, int Col) offset)
            {
                var pos = (Row: row, Col: col);

                while(true)
                {
                    pos = (pos.Row + offset.Row, pos.Col + offset.Col);

                    if (!IsValid(input, pos.Row, pos.Col, (0, 0)))
                        return Floor;

                    if (input[pos.Row][pos.Col] != Floor)
                        return input[pos.Row][pos.Col];
                }
            }
        }
    }
}
