using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2025;

public class Day4 : IAdventDay
{
    public string Name => "4. 12. 2025";

    private static string GetInput() => File.ReadAllText("2025/Resources/day4.txt");

    public string Solve()
    {
        var grid = Grid<char>.ParseAsCharacters(GetInput());

        return grid
            .Count(p => grid[p] is '@' && grid.GetNeighbours(p, includeDiagonals: true).Count(n => grid[n] is '@') < 4)
            .ToString();
    }

    public string SolveAdvanced()
    {
        int removed = 0;

        var grid = Grid<char>.ParseAsCharacters(GetInput());
        var removablePositions = grid.Where(IsRemovable).ToList();

        while (true)
        {
            removed += removablePositions.Count;

            foreach (var p in removablePositions)
            {
                grid[p] = '.';
            }

            removablePositions = removablePositions.SelectMany(p => grid.GetNeighbours(p, includeDiagonals: true))
                .Where(IsRemovable)
                .Distinct()
                .ToList();

            if (removablePositions.Count == 0)
                break;
        }

        return removed.ToString();

        bool IsRemovable(GridPosition p) => grid[p] is '@' && grid.GetNeighbours(p, includeDiagonals: true).Count(n => grid[n] is '@') < 4;
    }
}
