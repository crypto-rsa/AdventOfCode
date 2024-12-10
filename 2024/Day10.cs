using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day10 : IAdventDay
{
    public string Name => "10. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day10.txt");

    public string Solve()
    {
        var grid = Grid<int>.ParseAsIntegers(GetInput());
        int result = 0;

        foreach (var startPosition in grid.FindAll(0))
        {
            var reachable = new HashSet<GridPosition>();
            var stack = new Stack<(GridPosition, int LastNumber)>();

            stack.Push((startPosition, 0));

            while (stack.Count > 0)
            {
                (var position, int lastNumber) = stack.Pop();

                if (lastNumber == 9)
                {
                    reachable.Add(position);

                    continue;
                }

                foreach (var neighbour in grid.GetNeighbours(position).Where(p => grid[p] == lastNumber + 1))
                {
                    stack.Push((neighbour, lastNumber + 1));
                }
            }

            result += reachable.Count;
        }

        return result.ToString();
    }

    public string SolveAdvanced()
    {
        var grid = Grid<int>.ParseAsIntegers(GetInput());
        int result = 0;

        foreach (var startPosition in grid.FindAll(0))
        {
            int rating = 0;
            var stack = new Stack<(GridPosition, int LastNumber)>();

            stack.Push((startPosition, 0));

            while (stack.Count > 0)
            {
                (var position, int lastNumber) = stack.Pop();

                if (lastNumber == 9)
                {
                    rating++;

                    continue;
                }

                foreach (var neighbour in grid.GetNeighbours(position).Where(p => grid[p] == lastNumber + 1))
                {
                    stack.Push((neighbour, lastNumber + 1));
                }
            }

            result += rating;
        }

        return result.ToString();
    }
}
