using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2022;

public class Day8 : IAdventDay
{
    public string Name => "8. 12. 2022";

    private static string GetInput() => File.ReadAllText("2022/Resources/day8.txt");

    private static IEnumerable<(int, int)> GetVisibleFromEdge(int[][] grid, int row, int col, int rowStep, int colStep)
    {
        int highest = -1;

        while (row >= 0 && row < grid.Length && col >= 0 && col < grid[row].Length)
        {
            int height = grid[row][col];

            if (height > highest)
            {
                yield return (row, col);
            }

            highest = System.Math.Max(highest, height);

            row += rowStep;
            col += colStep;
        }
    }

    private static int GetVisibleCountFromTreeHouse(int[][] grid, int row, int col, int rowStep, int colStep)
    {
        int treeHouseHeight = grid[row][col];

        row += rowStep;
        col += colStep;

        int count = 0;

        while (row >= 0 && row < grid.Length && col >= 0 && col < grid[row].Length)
        {
            count++;

            if (grid[row][col] >= treeHouseHeight)
                break;

            row += rowStep;
            col += colStep;
        }

        return count;
    }

    public string Solve()
    {
        var grid = GetInput().ParseAsGrid(c => int.Parse(c.ToString()));

        var visible = new HashSet<(int Row, int Col)>();

        for (int row = 0; row < grid.Length; row++)
        {
            visible.AddRange(GetVisibleFromEdge(grid, row, 0, 0, +1));
            visible.AddRange(GetVisibleFromEdge(grid, row, grid[row].Length - 1, 0, -1));
        }

        for (int col = 0; col < grid[0].Length; col++)
        {
            visible.AddRange(GetVisibleFromEdge(grid, 0, col, +1, 0));
            visible.AddRange(GetVisibleFromEdge(grid, grid.Length - 1, col, -1, 0));
        }

        return visible.Count.ToString();
    }

    public string SolveAdvanced()
    {
        var grid = GetInput().ParseAsGrid(c => int.Parse(c.ToString()));

        return grid
            .SelectMany((a, r) => a.Select((_, c) => GetScenicScore(r, c)))
            .Max()
            .ToString();

        int GetScenicScore(int row, int col)
        {
            int left = GetVisibleCountFromTreeHouse(grid, row, col, 0, -1);
            int up = GetVisibleCountFromTreeHouse(grid, row, col, -1, 0);
            int right = GetVisibleCountFromTreeHouse(grid, row, col, 0, +1);
            int down = GetVisibleCountFromTreeHouse(grid, row, col, +1, 0);

            return left * up * right * down;
        }
    }
}
