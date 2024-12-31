using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day20 : IAdventDay
{
    public string Name => "20. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day20.txt");

    private int GetPathCount(Grid<char> grid, int minShortcut, int maxCheatLength)
    {
        var start = grid.Find('S');
        var end = grid.Find('E');

        var heap = new Heap<GridPosition>();
        var steps = new Dictionary<GridPosition, int> { [start] = 0 };
        var previous = new Dictionary<GridPosition, GridPosition>();

        heap.Push(start, start.DistanceTo(end));

        while (heap.Count > 0)
        {
            var position = heap.Pop();
            int currentSteps = steps[position];

            if (position == end)
                break;

            foreach (var neighbour in grid.GetNeighbours(position))
            {
                if (grid[neighbour] == '#')
                    continue;

                if (!steps.TryGetValue(neighbour, out var fewestSteps) || currentSteps + 1 < fewestSteps)
                {
                    steps[neighbour] = currentSteps + 1;
                    previous[neighbour] = position;
                    heap.Push(neighbour, neighbour.DistanceTo(end) + currentSteps + 1);
                }
            }
        }

        var endPositions = previous.Keys.Append(start).ToHashSet();
        int pathCount = 0;

        foreach (var shortcutStart in endPositions)
        {
            foreach (var shortcutEnd in endPositions)
            {
                if (shortcutStart.DistanceTo(shortcutEnd) > maxCheatLength)
                    continue;

                int savedSteps = steps[shortcutEnd] - (steps[shortcutStart] + shortcutStart.DistanceTo(shortcutEnd));

                if (savedSteps >= minShortcut)
                {
                    pathCount++;
                }
            }
        }

        return pathCount;
    }

    public string Solve()
    {
        var grid = Grid<char>.ParseAsCharacters(GetInput());

        return GetPathCount(grid, minShortcut: 100, maxCheatLength: 2).ToString();
    }

    public string SolveAdvanced()
    {
        var grid = Grid<char>.ParseAsCharacters(GetInput());

        return GetPathCount(grid, minShortcut: 100, maxCheatLength: 20).ToString();
    }
}
