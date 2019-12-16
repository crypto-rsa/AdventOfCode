using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day15 : IAdventDay
    {
        public string Name => "15. 12. 2019";

        public string Solve() => FindDistanceToTarget().ToString();

        public string SolveAdvanced() => GetDistancesFromTarget().Values.Max().ToString();

        private static List<long> GetInput() => File.ReadAllText("2019/Resources/day15.txt").Split(',').Select(long.Parse).ToList();

        private readonly Dictionary<(int X, int Y), char> _map = new Dictionary<(int X, int Y), char>();

        private (int X, int Y) _mapPosition;

        private (int X, int Y) _target;

        private readonly Stack<int> _moves = new Stack<int>();

        private int FindDistanceToTarget()
        {
            Run(GetInput());

            return GetDistancesFromTarget()[(0, 0)];
        }

        private void Run(List<long> program)
        {
            int position = 0;
            int relativeBase = 0;
            int? lastMove = 0;
            bool backtrack = false;

            _mapPosition = (X: 0, Y: 0);
            _map[_mapPosition] = '.';

            while (true)
            {
                switch (Get(position) % 100)
                {
                    case 1:
                        Set(GetWriteIndex(2), GetParameterValue(0) + GetParameterValue(1));
                        position += 4;
                        break;

                    case 2:
                        Set(GetWriteIndex(2), GetParameterValue(0) * GetParameterValue(1));
                        position += 4;
                        break;

                    case 3:
                        backtrack = false;
                        lastMove = GetNextMove();

                        if (!lastMove.HasValue && _moves.Count > 0)
                        {
                            lastMove = Backtrack();
                            backtrack = true;
                        }

                        if (!lastMove.HasValue)
                        {
                            Print();
                            Console.ReadKey();
                            return;
                        }

                        Set(GetWriteIndex(0), lastMove.Value);
                        position += 2;
                        break;

                    case 4:
                        var output = GetParameterValue(0);
                        position += 2;

                        if (output != 0 && !backtrack)
                        {
                            _moves.Push(lastMove.Value);
                        }

                        switch (output)
                        {
                            case 0:
                                _map[GetNextPosition(lastMove.Value, _mapPosition)] = '#';
                                break;

                            case 1:
                                _mapPosition = GetNextPosition(lastMove.Value, _mapPosition);
                                _map[_mapPosition] = '.';
                                break;

                            case 2:
                                _mapPosition = GetNextPosition(lastMove.Value, _mapPosition);
                                _map[_mapPosition] = 'o';
                                _target = _mapPosition;
                                break;
                        }
                        break;

                    case 5:
                        if (GetParameterValue(0) != 0)
                        {
                            position = (int)GetParameterValue(1);
                        }
                        else
                        {
                            position += 3;
                        }
                        break;

                    case 6:
                        if (GetParameterValue(0) == 0)
                        {
                            position = (int)GetParameterValue(1);
                        }
                        else
                        {
                            position += 3;
                        }
                        break;

                    case 7:
                        Set(GetWriteIndex(2), GetParameterValue(0) < GetParameterValue(1) ? 1 : 0);
                        position += 4;
                        break;

                    case 8:
                        Set(GetWriteIndex(2), GetParameterValue(0) == GetParameterValue(1) ? 1 : 0);
                        position += 4;
                        break;

                    case 9:
                        relativeBase += (int)GetParameterValue(0);
                        position += 2;
                        break;

                    default:
                        return;
                }
            }

            int? GetNextMove()
            {
                return GetMoves().FirstOrDefault(i => !_map.ContainsKey(GetNextPosition(i.Value, _mapPosition)));

                static IEnumerable<int?> GetMoves()
                {
                    yield return 1;
                    yield return 4;
                    yield return 2;
                    yield return 3;
                }
            }

            long GetParameterValue(int index)
            {
                long opCode = Get(position) / 100;

                for (long i = 0; i < index; i++)
                {
                    opCode /= 10;
                }

                return (opCode % 10) switch
                {
                    0 => Get((int)Get(position + index + 1)),
                    1 => Get(position + index + 1),
                    2 => Get(relativeBase + (int)Get(position + index + 1)),
                    _ => throw new InvalidOperationException(),
                };
            }

            int GetWriteIndex(int index)
            {
                long opCode = Get(position) / 100;

                for (long i = 0; i < index; i++)
                {
                    opCode /= 10;
                }

                return (opCode % 10) switch
                {
                    0 => (int)Get(position + index + 1),
                    2 => relativeBase + (int)Get(position + index + 1),
                    _ => throw new InvalidOperationException(),
                };
            }

            long Get(int index) => index <= program.Count ? program[index] : 0;

            void Set(int index, long value)
            {
                if (index >= program.Count)
                {
                    program.AddRange(Enumerable.Repeat(0L, index - program.Count + 1));
                }

                program[index] = value;
            }
        }

        private (int X, int Y) GetNextPosition(int index, (int X, int Y) curPos) => index switch
        {
            1 => (curPos.X + 0, curPos.Y + 1),
            2 => (curPos.X + 0, curPos.Y - 1),
            3 => (curPos.X - 1, curPos.Y + 0),
            4 => (curPos.X + 1, curPos.Y + 0),
            _ => throw new InvalidOperationException(),
        };

        private int Backtrack() => _moves.Pop() switch
        {
            1 => 2,
            2 => 1,
            3 => 4,
            4 => 3,
            _ => throw new InvalidOperationException(),
        };

        private Dictionary<(int X, int Y), int> GetDistancesFromTarget()
        {
            var open = new Dictionary<(int X, int Y), int>
            {
                [_target] = 0,
            };

            var closed = new Dictionary<(int X, int Y), int>();

            while (true)
            {
                var key = GetNearest();

                if (key == null)
                    break;

                var curDistance = open[key.Value];

                for (int i = 1; i <= 4; i++)
                {
                    var (x, y) = GetNextPosition(i, (key.Value.X, key.Value.Y));

                    if (_map[(x, y)] == '#')
                        continue;

                    if (!closed.TryGetValue((x, y), out int value) || value > curDistance + 1)
                    {
                        closed.Remove((x, y));
                        open[(x, y)] = curDistance + 1;
                    }
                }

                closed[key.Value] = curDistance;
                open.Remove(key.Value);
            }

            return closed;

            int GetDistanceEstimate((int X, int Y) pos) => open[pos] + Math.Abs(pos.X - _target.X) + Math.Abs(pos.Y - _target.Y);

            (int X, int Y)? GetNearest()
            {
                if (open.Count == 0)
                    return null;

                (int X, int Y) minKey = (0, 0);
                int minDistance = int.MaxValue;

                foreach (var item in open)
                {
                    int estimate = GetDistanceEstimate((item.Key.X, item.Key.Y));
                    if (estimate < minDistance)
                    {
                        minDistance = estimate;
                        minKey = item.Key;
                    }
                }

                return minKey;
            }
        }

        private void Print()
        {
            Console.Clear();

            var (minX, minY) = _map.Keys.Aggregate((X: int.MaxValue, Y: int.MaxValue), (cur, next) => (Math.Min(cur.X, next.X), Math.Min(cur.Y, next.Y)));
            var (maxX, maxY) = _map.Keys.Aggregate((X: int.MinValue, Y: int.MinValue), (cur, next) => (Math.Max(cur.X, next.X), Math.Max(cur.Y, next.Y)));

            for (int y = maxY; y >= minY; y--)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Console.Write(GetState((x, y)));
                }

                Console.WriteLine();
            }

            char GetState((int X, int Y) position)
            {
                if (position == _mapPosition)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    return '*';
                }

                if (position == (0, 0))
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ResetColor();

                if (!_map.TryGetValue(position, out char value))
                    return ' ';

                return value;
            }
        }
    }
}
