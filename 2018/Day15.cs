using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code.Year2018
{
    public class Day15 : IAdventDay
    {
        private struct Vector
        {
            public Vector(int row, int col)
            {
                Row = row;
                Col = col;
            }

            public static Vector operator +(Vector first, Vector second)
                => new Vector(first.Row + second.Row, first.Col + second.Col);

            public int Row { get; }

            public int Col { get; }

            public static Vector Invalid { get; } = new Vector(-1, -1);
        }

        private class Game
        {
            private char[,] _maze;

            private int _rows;

            private int _columns;

            private List<Unit> _units = new List<Unit>();

            public Game(int elfAttackScore)
            {
                ElfAttackScore = elfAttackScore;
                Initialize();
            }

            public IEnumerable<Unit> SortedUnits => _units.OrderBy(u => u.Position.Row).ThenBy(u => u.Position.Col);

            public int CompletedRounds { get; set; }

            public int ElfAttackScore { get; }

            public bool AnyElfDied { get; private set; }

            private void Initialize()
            {
                var lines = System.IO.File.ReadAllLines(@"2018/Resources/day15.txt");
                _rows = lines.Length;
                _columns = lines[0].Length;

                _maze = new char[_rows, _columns];

                for (int row = 0; row < _rows; row++)
                {
                    var line = lines[row];

                    for (int col = 0; col < _columns; col++)
                    {
                        _maze[row, col] = line[col] == '#' ? '#' : '.';

                        if (line[col] == 'G' || line[col] == 'E')
                        {
                            var unit = new Unit(line[col], row, col);
                            _units.Add(unit);
                        }
                    }
                }
            }

            private IEnumerable<Vector> GetRange(Vector position)
            {
                var offsets = new Vector[]
                {
                    new Vector(-1, 0),
                    new Vector(0, -1),
                    new Vector(+1, 0),
                    new Vector(0, +1),
                };

                return offsets.Select(v => v + position).Where(IsFree);

                bool IsFree(Vector vector)
                {
                    if (vector.Row < 0 || vector.Row >= _rows)
                        return false;

                    if (vector.Col < 0 || vector.Col >= _columns)
                        return false;

                    if (_maze[vector.Row, vector.Col] != '.')
                        return false;

                    return !_units.Any(u => u.Position.Equals(vector));
                }
            }

            private static int Distance(Vector first, Vector second)
                => Math.Abs(first.Row - second.Row) + Math.Abs(first.Col - second.Col);

            private Vector GetNextMove(Unit unit)
            {
                var range = GetRange(unit.Position).ToList();
                if (!range.Any())
                    return Vector.Invalid;

                if (GetAdjacentCompetitor(unit) != null)
                    return Vector.Invalid;

                var distances = new Dictionary<Vector, int>();
                var open = new HashSet<Vector>(_units.Where(u => u.Type != unit.Type).SelectMany(u => GetRange(u.Position)));

                foreach (var v in open)
                {
                    distances[v] = 0;
                }

                while (open.Any())
                {
                    var nearest = open.OrderBy(GetDistance).First();
                    open.Remove(nearest);

                    int d = GetDistance(nearest);
                    foreach (var v2 in GetRange(nearest))
                    {
                        if (d + 1 < GetDistance(v2))
                        {
                            open.Add(v2);
                            distances[v2] = d + 1;
                        }
                    }
                }

                if (range.All(v => GetDistance(v) == int.MaxValue))
                    return Vector.Invalid;

                return range.OrderBy(v => GetDistance(v)).ThenBy(v => v.Row * _columns + v.Col).First();

                int GetDistance(Vector v)
                    => distances.TryGetValue(v, out int d) ? d : int.MaxValue;
            }

            private Unit GetAdjacentCompetitor(Unit unit)
                => _units.Where(u => u.Type != unit.Type)
                    .Where(u => Distance(u.Position, unit.Position) == 1)
                    .OrderBy(u => u.HitPoints)
                    .ThenBy(u => u.Position.Row * _columns + u.Position.Col)
                    .FirstOrDefault();

            private void Attack(Unit unit)
            {
                var competitor = GetAdjacentCompetitor(unit);
                if (competitor == null)
                    return;

                competitor.HitPoints -= (unit.Type == 'E' ? ElfAttackScore : 3);
                if (competitor.HitPoints <= 0)
                {
                    _units.Remove(competitor);
                    if (competitor.Type == 'E')
                    {
                        AnyElfDied = true;
                    }
                }
            }

            private void DoTurn(Unit unit)
            {
                var nextMove = GetNextMove(unit);
                if (!nextMove.Equals(Vector.Invalid))
                {
                    unit.Position = nextMove;
                }

                Attack(unit);
            }

            public bool DoRound()
            {
                foreach (var unit in SortedUnits.ToList())
                {
                    if (unit.HitPoints <= 0)
                        continue;

                    if (!_units.Any(u => u.Type != unit.Type))
                        return false;

                    DoTurn(unit);
                }

                CompletedRounds++;

                return true;
            }

            public void Print()
            {
                var unitPositions = _units.ToDictionary(u => u.Position, u => u.Type);

                for (int row = 0; row < _rows; row++)
                {
                    for (int col = 0; col < _columns; col++)
                    {
                        if (!unitPositions.TryGetValue(new Vector(row, col), out char c))
                        {
                            c = _maze[row, col];
                        }

                        Console.Write(c);
                    }

                    Console.Write("\t");
                    var units = _units.Where(u => u.Position.Row == row).OrderBy(u => u.Position.Col);
                    Console.WriteLine(string.Join(", ", units.Select(u => $"{u.Type}({u.HitPoints})")));
                }

                Console.WriteLine();
            }

            public void Run(bool report)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                while (true)
                {
                    if (!DoRound())
                        break;

                    if (report)
                    {
                        Console.WriteLine($"After round {CompletedRounds}");
                        Print();
                        Console.WriteLine($"Completed in {watch.ElapsedMilliseconds} ms");
                        watch.Restart();
                    }
                }
            }

            public int SumOfHitPoints() => _units.Sum(u => u.HitPoints);
        }

        private class Unit
        {
            public Unit(char type, int row, int col)
            {
                Type = type;
                Position = new Vector(row, col);
            }

            public char Type { get; }

            public Vector Position { get; set; }

            public int HitPoints { get; set; } = 200;
        }

        public string Name => "15. 12. 2018";

        public string Solve()
        {
            var game = new Game(3);
            game.Run(report: false);

            return (game.CompletedRounds * game.SumOfHitPoints()).ToString();
        }

        public string SolveAdvanced()
        {
            int lowerBound = 1;
            int upperBound = int.MaxValue;

            while (true)
            {
                int power = upperBound == int.MaxValue ? 2 * lowerBound : (upperBound + lowerBound) / 2;
                var game = new Game(power);

                game.Run(report: false);

                if (game.AnyElfDied)
                {
                    lowerBound = power;
                }
                else
                {
                    upperBound = power;
                }

                if (upperBound - lowerBound <= 1)
                {
                    game = new Game(upperBound);
                    game.Run(report: false);

                    return (game.CompletedRounds * game.SumOfHitPoints()).ToString();
                }
            }
        }
    }
}