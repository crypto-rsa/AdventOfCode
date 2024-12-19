using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day18 : IAdventDay
{
    public string Name => "18. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day18.txt");

    (Grid<char>, List<GridPosition>) ParseInput()
    {
        var bytePositions = GetInput().SplitToLines().Select(Split).ToList();
        var grid = Grid<char>.Create(71, 71, '.');

        return (grid, bytePositions);

        GridPosition Split(string s)
        {
            var parts = s.Split(',');

            return new GridPosition(int.Parse(parts[1]), int.Parse(parts[0]));
        }
    }

    public string Solve()
    {
        var (grid, bytePositions) = ParseInput();

        foreach (var position in bytePositions.Take(1024))
        {
            grid[position] = '#';
        }

        return grid.FindPathLength(grid.TopLeft, grid.BottomRight, '.').ToString();
    }

    public string SolveAdvanced()
    {
        var (grid, bytePositions) = ParseInput();

        var stack = new Stack<(int LowerBound, int UpperBound)>();

        stack.Push((0, bytePositions.Count - 1));

        while (stack.Count > 0)
        {
            (int lowerBound, int upperBound) = stack.Pop();

            if (lowerBound == upperBound)
                return FormatByte(lowerBound);

            int middle = (lowerBound + upperBound) / 2;
            int pathLength = FillAndFindPathLength(middle);

            stack.Push(pathLength != -1 ? (middle + 1, upperBound) : (lowerBound, middle));
        }

        return "No byte blocks the path";

        int FillAndFindPathLength(int bytes)
        {
            var tempGrid = Grid<char>.Create(grid.Height, grid.Width, '.');

            for (int i = 0; i <= bytes; i++)
            {
                tempGrid[bytePositions[i]] = '#';
            }

            return tempGrid.FindPathLength(tempGrid.TopLeft, tempGrid.BottomRight, '.');
        }

        string FormatByte(int index) => $"{bytePositions[index].Column},{bytePositions[index].Row}";
    }
}
