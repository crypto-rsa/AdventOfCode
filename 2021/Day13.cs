using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2021
{
    public class Day13 : IAdventDay
    {
        public string Name => "13. 12. 2021";

        private static string GetInput() => File.ReadAllText("2021/Resources/day13.txt");

        private class Paper
        {
            private readonly HashSet<(int Row, int Col)> _dots;
            private readonly List<(string, int)> _folds;

            public Paper()
            {
                var parts = GetInput().Split($"{Environment.NewLine}{Environment.NewLine}");

                _dots = parts[0].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(SplitCoordinates).ToHashSet();
                _folds = parts[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(SplitFolds).ToList();

                (int, int) SplitCoordinates(string line)
                {
                    var coordinateItems = line.Split(',');

                    return (int.Parse(coordinateItems[1]), int.Parse(coordinateItems[0]));
                }

                (string, int) SplitFolds(string line)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(line, @"fold along (.)=(\d+)");

                    return (match.Groups[1].Value, int.Parse(match.Groups[2].Value));
                }
            }

            public int FoldOnceAndCalculateDots()
            {
                Fold(0);

                return _dots.Count;
            }

            public string FoldAll()
            {
                for (int i = 0; i < _folds.Count; i++)
                {
                    Fold(i);
                }

                int maxRow = _dots.Max(i => i.Row);
                int maxCol = _dots.Max(i => i.Col);

                var builder = new System.Text.StringBuilder();
                builder.AppendLine();

                for (int row = 0; row <= maxRow; row++)
                {
                    for (int col = 0; col <= maxCol; col++)
                    {
                        builder.Append(_dots.Contains((row, col)) ? '#' : ' ');
                    }

                    builder.AppendLine();
                }

                return builder.ToString();
            }

            private void Fold(int index)
            {
                (string direction, int coordinate) = _folds[index];

                if (direction == "x")
                {
                    foreach (var dot in _dots.Where(i => i.Col > coordinate).ToList())
                    {
                        _dots.Remove(dot);
                        _dots.Add((dot.Row, 2 * coordinate - dot.Col));
                    }
                }
                else
                {
                    foreach (var dot in _dots.Where(i => i.Row > coordinate).ToList())
                    {
                        _dots.Remove(dot);
                        _dots.Add(((2 * coordinate - dot.Row), dot.Col));
                    }
                }
            }
        }

        public string Solve() => new Paper().FoldOnceAndCalculateDots().ToString();

        public string SolveAdvanced() => new Paper().FoldAll();
    }
}
