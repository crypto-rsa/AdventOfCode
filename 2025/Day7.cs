using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2025;

public class Day7 : IAdventDay
{
    public string Name => "7. 12. 2025";

    private static string GetInput() => File.ReadAllText("2025/Resources/day7.txt");

    public string Solve()
    {
        var grid = Grid<char>.ParseAsCharacters(GetInput());
        var startPosition = grid.Find('S');

        int splits = 0;
        var tachyonColumns = new HashSet<int> { startPosition.Column };

        for (int row = startPosition.Row + 1; row < grid.Height; row++)
        {
            foreach (int column in tachyonColumns.ToList())
            {
                if (grid[new GridPosition(row, column)] == '^')
                {
                    splits++;
                    tachyonColumns.Remove(column);

                    var left = new GridPosition(row, column - 1);

                    if (grid.IsValid(left))
                    {
                        tachyonColumns.Add(left.Column);
                    }

                    var right = new GridPosition(row, column + 1);

                    if (grid.IsValid(right))
                    {
                        tachyonColumns.Add(right.Column);
                    }
                }
            }
        }

        return splits.ToString();
    }

    public string SolveAdvanced()
    {
        var grid = Grid<char>.ParseAsCharacters(GetInput());
        var startPosition = grid.Find('S');

        var ways = new Dictionary<GridPosition, long>
        {
            [startPosition] = 1,
        };

        for (int row = startPosition.Row + 1; row < grid.Height; row++)
        {
            for (int column = 0; column < grid.Width; column++)
            {
                var above = new GridPosition(row - 1, column);
                var position = new GridPosition(row, column);
                var left = new GridPosition(row, column - 1);
                var right = new GridPosition(row, column + 1);

                ways.TryAdd(position, 0);
                ways.TryAdd(left, 0);
                ways.TryAdd(right, 0);

                if (grid[position] == '.')
                {
                    ways[position] += ways.GetValueOrDefault(above, 0L);
                }
                else if (grid[position] == '^')
                {
                    ways[left] += ways[above];
                    ways[right] += ways[above];
                }
            }
        }

        return Enumerable.Range(0, grid.Width).Sum(c => ways.GetValueOrDefault(new GridPosition(grid.Height - 1, c), 0L)).ToString();
    }
}
