using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2023;

public class Day11 : IAdventDay
{
    public string Name => "11. 12. 2023";

    private static string GetInput() => File.ReadAllText("2023/Resources/day11.txt");

    private static long GetTotalDistance(int multiplier)
    {
        var map = GetInput().ParseAsGrid(c => c);
        var isRowEmpty = Enumerable.Repeat(true, map.Length).ToArray();
        var isColumnEmpty = Enumerable.Repeat(true, map[0].Length).ToArray();
        var galaxies = new List<(int Row, int Column)>();

        for (int r = 0; r < map.Length; r++)
        {
            for (int c = 0; c < map[r].Length; c++)
            {
                if (map[r][c] == '#')
                {
                    galaxies.Add((r, c));
                    isRowEmpty[r] = false;
                    isColumnEmpty[c] = false;
                }
            }
        }

        long totalDistance = 0;

        for (int i = 0; i < galaxies.Count; i++)
        {
            var galaxy1 = galaxies[i];

            for (int j = i + 1; j < galaxies.Count; j++)
            {
                var galaxy2 = galaxies[j];
                int fromRow = Math.Min(galaxy1.Row, galaxy2.Row);
                int toRow = Math.Max(galaxy1.Row, galaxy2.Row);
                int fromColumn = Math.Min(galaxy1.Column, galaxy2.Column);
                int toColumn = Math.Max(galaxy1.Column, galaxy2.Column);

                int expandingRows = isRowEmpty.Skip(fromRow).Take(toRow - fromRow).Count(b => b);
                int expandingColumns = isColumnEmpty.Skip(fromColumn).Take(toColumn - fromColumn).Count(b => b);

                totalDistance += toRow - fromRow + toColumn - fromColumn + (expandingRows + expandingColumns) * (multiplier - 1);
            }
        }

        return totalDistance;
    }

    public string Solve() => GetTotalDistance(2).ToString();

    public string SolveAdvanced() => GetTotalDistance(1_000_000).ToString();
}
