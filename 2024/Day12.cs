using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace Advent_of_Code.Year2024;

public class Day12 : IAdventDay
{
    public string Name => "12. 12. 2024";

    private static string GetInput() => File.ReadAllText("2024/Resources/day12.txt");

    private record Component(char Name, HashSet<GridPosition> Plots)
    {
        #region Fields

        private readonly Grid<char> _grid;

        #endregion

        #region Constructors

        public Component(Grid<char> grid, GridPosition position)
            : this(grid[position], [])
        {
            _grid = grid;
        }

        #endregion

        #region Methods

        public int GetValue(Func<int> multiplierGetter) => Plots.Count * multiplierGetter();

        public int GetPerimeter()
        {
            return Plots.Count * 4 - Plots.Sum(GetPerimeterEdgesToRemove);

            int GetPerimeterEdgesToRemove(GridPosition plot)
                => _grid.GetNeighbours(plot).Count(neighbour => Plots.Contains(neighbour));
        }

        public int GetNumberOfSides()
        {
            var edges = new List<(int Sequential, int Level)>[] { [], [], [], [] };

            foreach (var plot in Plots)
            {
                if (!Plots.Contains(plot.Above))
                {
                    edges[0].Add((plot.Column, plot.Row));
                }

                if (!Plots.Contains(plot.Below))
                {
                    edges[1].Add((plot.Column, plot.Row));
                }

                if (!Plots.Contains(plot.Left))
                {
                    edges[2].Add((plot.Row, plot.Column));
                }

                if (!Plots.Contains(plot.Right))
                {
                    edges[3].Add((plot.Row, plot.Column));
                }
            }

            return edges.Sum(GetContinuousParts);

            int GetContinuousParts(List<(int Sequential, int Level)> list) => list
                .GroupBy(t => t.Level)
                .Sum(g => g.Select(t => t.Sequential).Order().Aggregate((Count: 0, Last: int.MinValue), Aggregate).Count);

            (int Count, int Last) Aggregate((int Count, int Last) acc, int next) => next == acc.Last + 1 ? (acc.Count, next) : (acc.Count + 1, next);
        }

        #endregion
    }

    private static List<Component> GetComponents()
    {
        var grid = Grid<char>.ParseAsCharacters(GetInput());
        var assigned = new HashSet<GridPosition>();
        var components = new List<Component>();

        foreach (var position in grid)
        {
            if (assigned.Contains(position))
                continue;

            var component = new Component(grid, position);
            var stack = new Stack<GridPosition>();

            stack.Push(position);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                assigned.Add(current);
                component.Plots.Add(current);

                foreach (var neighbour in grid.GetNeighbours(current))
                {
                    if (!assigned.Contains(neighbour) && grid[neighbour] == component.Name)
                    {
                        stack.Push(neighbour);
                    }
                }
            }

            components.Add(component);
        }

        return components;
    }

    public string Solve() => GetComponents().Sum(c => c.GetValue(c.GetPerimeter)).ToString();

    public string SolveAdvanced() => GetComponents().Sum(c => c.GetValue(c.GetNumberOfSides)).ToString();
}
