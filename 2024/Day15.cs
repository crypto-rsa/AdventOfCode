using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day15 : IAdventDay
{
    public string Name => "15. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day15.txt");

    private static Grid<char> CreateGrid(out string instructions)
    {
        var parts = GetInput().ParseByBlankLines().ToArray();
        var grid = new Grid<char>(parts[0].Select(s => s.ToArray()).ToArray());
        instructions = string.Join(string.Empty, parts[1]);

        return grid;
    }


    public string Solve()
    {
        var grid = CreateGrid(out string instructions);

        var position = grid.Find('@');

        foreach (char instruction in instructions)
        {
            var offset = Grid<char>.GetOffset(instruction);

            var nextStopPosition = position + offset;

            int trainLength = 0;

            while (grid[nextStopPosition] == 'O')
            {
                trainLength++;
                nextStopPosition += offset;

                if (!grid.IsValid(nextStopPosition))
                    break;
            }

            if (!grid.IsValid(nextStopPosition) || grid[nextStopPosition] == '#')
                continue;

            grid[position] = '.';
            grid[position + offset] = '@';

            position += offset;

            var tempPosition = position + offset;

            for (int i = 0; i < trainLength; i++)
            {
                grid[tempPosition] = 'O';
                tempPosition += offset;
            }
        }

        return grid
            .Where(p => grid[p] == 'O')
            .Sum(p => 100 * p.Row + p.Column)
            .ToString();
    }

    public string SolveAdvanced()
    {
        var smallGrid = CreateGrid(out string instructions);
        var array = new char[smallGrid.Height][];

        for (int i = 0; i < smallGrid.Height; i++)
        {
            array[i] = new char[smallGrid.Width * 2];

            for (int j = 0; j < smallGrid.Width; j++)
            {
                (array[i][2 * j], array[i][2 * j + 1]) = smallGrid[new GridPosition(i, j)] switch
                {
                    '#' => ('#', '#'),
                    'O' => ('[', ']'),
                    '.' => ('.', '.'),
                    '@' => ('@', '.'),
                };
            }
        }

        var grid = new Grid<char>(array);

        var position = grid.Find('@');

        foreach (char instruction in instructions)
        {
            var offset = Grid<char>.GetOffset(instruction);
            var nextPosition = position + offset;

            if (!grid.IsValid(nextPosition) || grid[nextPosition] == '#')
                continue;

            var boxPositions = new HashSet<GridPosition>();

            var queue = new Queue<GridPosition>();

            queue.Enqueue(nextPosition);

            while (queue.Count > 0)
            {
                var tempPosition = queue.Dequeue();

                if (!grid.IsValid(tempPosition))
                    continue;

                if (grid[tempPosition] is '[' or ']')
                {
                    boxPositions.Add(tempPosition);

                    if (instruction is '^' or 'v')
                    {
                        var sibling = grid[tempPosition] is '[' ? tempPosition.Right : tempPosition.Left;

                        if (!boxPositions.Contains(sibling))
                        {
                            queue.Enqueue(sibling);
                        }
                    }

                    if (!boxPositions.Contains(tempPosition + offset))
                    {
                        queue.Enqueue(tempPosition + offset);
                    }
                }
            }

            var front = boxPositions.Select(p => p + offset).Where(p => !boxPositions.Contains(p)).ToArray();

            if (front.Any(p => !grid.IsValid(p) || grid[p] != '.'))
                continue;

            if (boxPositions.Count > 0)
            {
                var newGrid = new Grid<char>(grid);

                foreach (var boxPosition in boxPositions)
                {
                    newGrid[boxPosition] = '.';
                }

                foreach (var boxPosition in boxPositions)
                {
                    newGrid[boxPosition + offset] = grid[boxPosition];
                }

                grid = newGrid;
            }

            grid[position] = '.';
            grid[nextPosition] = '@';

            position += offset;
        }

        return grid
            .Where(p => grid[p] == '[')
            .Sum(p => 100 * p.Row + p.Column)
            .ToString();
    }
}
