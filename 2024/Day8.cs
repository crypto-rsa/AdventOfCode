using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day8 : IAdventDay
{
    public string Name => "8. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day8.txt");

    private static int GetAntinodeCount(bool fullLine)
    {
        var map = GetInput().ParseAsGrid(c => c);
        var positions = map.SelectMany((a, row) => a.Select((c, col) => (Row: row, Col: col, Char: c))).Where(i => i.Char != '.').ToList();
        var antinodes = new HashSet<(int Row, int Col)>();

        foreach (var item in positions.GroupBy(i => i.Char))
        {
            foreach (var antenna1 in item)
            {
                foreach (var antenna2 in item)
                {
                    var rowDiff = antenna2.Row - antenna1.Row;
                    var colDiff = antenna2.Col - antenna1.Col;

                    if ((rowDiff, colDiff) is (0, 0))
                        continue;

                    if (fullLine)
                    {
                        for (int i = 1;; i++)
                        {
                            if (!AddOffsetPosition(antenna1.Row, antenna1.Col, rowDiff, colDiff, i))
                                break;
                        }

                        for (int i = -1;; i--)
                        {
                            if (!AddOffsetPosition(antenna2.Row, antenna2.Col, rowDiff, colDiff, i))
                                break;
                        }
                    }
                    else
                    {
                        AddOffsetPosition(antenna2.Row, antenna2.Col, rowDiff, colDiff, +1);
                        AddOffsetPosition(antenna1.Row, antenna1.Col, rowDiff, colDiff, -1);
                    }
                }
            }
        }

        return antinodes.Count;

        bool IsValid(int row, int col) => row >= 0 && row < map.Length && col >= 0 && col < map[0].Length;

        bool AddOffsetPosition(int row, int col, int rowOffset, int colOffset, int multiplier)
        {
            var newRow = row + rowOffset * multiplier;
            var newCol = col + colOffset * multiplier;

            if (!IsValid(newRow, newCol))
                return false;

            antinodes.Add((newRow, newCol));

            return true;
        }
    }

    public string Solve() => GetAntinodeCount(fullLine: false).ToString();

    public string SolveAdvanced() => GetAntinodeCount(fullLine: true).ToString();
}
