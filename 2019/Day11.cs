using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_of_Code.Year2019
{
    public class Day11 : IAdventDay
    {
        public string Name => "11. 12. 2019";

        public string Solve() => Run(GetInput(), advanced: false);

        public string SolveAdvanced() => Run(GetInput(), advanced: true);

        private static List<long> GetInput() => File.ReadAllText("2019/Resources/day11.txt").Split(',').Select(long.Parse).ToList();

        private string Run(List<long> program, bool advanced)
        {
            var whiteCells = new HashSet<(int X, int Y)>();
            var paintedCells = new HashSet<(int X, int Y)>();
            var position = (X: 0, Y: 0);
            var direction = (X: 0, Y: +1);
            bool painting = true;

            int ip = 0;
            int relativeBase = 0;

            if (advanced)
            {
                whiteCells.Add(position);
            }

            while (true)
            {
                switch (Get(ip) % 100)
                {
                    case 1:
                        Set(GetWriteIndex(2), GetParameterValue(0) + GetParameterValue(1));
                        ip += 4;
                        break;

                    case 2:
                        Set(GetWriteIndex(2), GetParameterValue(0) * GetParameterValue(1));
                        ip += 4;
                        break;

                    case 3:
                        Set(GetWriteIndex(0), whiteCells.Contains(position) ? 1 : 0);
                        ip += 2;
                        break;

                    case 4:
                        long output = GetParameterValue(0);
                        if (painting)
                        {
                            if (output == 0)
                            {
                                whiteCells.Remove(position);
                            }
                            else
                            {
                                whiteCells.Add(position);
                            }

                            paintedCells.Add(position);
                        }
                        else
                        {
                            direction = output == 0 ? TurnLeft() : TurnRight();
                            position = (position.X + direction.X, position.Y + direction.Y);
                        }
                        painting = !painting;
                        ip += 2;
                        break;

                    case 5:
                        if (GetParameterValue(0) != 0)
                        {
                            ip = (int)GetParameterValue(1);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;

                    case 6:
                        if (GetParameterValue(0) == 0)
                        {
                            ip = (int)GetParameterValue(1);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;

                    case 7:
                        Set(GetWriteIndex(2), GetParameterValue(0) < GetParameterValue(1) ? 1 : 0);
                        ip += 4;
                        break;

                    case 8:
                        Set(GetWriteIndex(2), GetParameterValue(0) == GetParameterValue(1) ? 1 : 0);
                        ip += 4;
                        break;

                    case 9:
                        relativeBase += (int)GetParameterValue(0);
                        ip += 2;
                        break;

                    case 99:
                        if (advanced)
                        {
                            Print();
                            return "See above";
                        }

                        return paintedCells.Count.ToString();

                    default:
                        return null;
                }
            }

            long GetParameterValue(int index)
            {
                long opCode = Get(ip) / 100;

                for (long i = 0; i < index; i++)
                {
                    opCode /= 10;
                }

                return (opCode % 10) switch
                {
                    0 => Get((int)Get(ip + index + 1)),
                    1 => Get(ip + index + 1),
                    2 => Get(relativeBase + (int)Get(ip + index + 1)),
                    _ => throw new InvalidOperationException(),
                };
            }

            int GetWriteIndex(int index)
            {
                long opCode = Get(ip) / 100;

                for (long i = 0; i < index; i++)
                {
                    opCode /= 10;
                }

                return (opCode % 10) switch
                {
                    0 => (int)Get(ip + index + 1),
                    2 => relativeBase + (int)Get(ip + index + 1),
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

            (int X, int Y) TurnLeft() => direction switch
            {
                (0, +1) => (-1, 0),
                (-1, 0) => (0, -1),
                (0, -1) => (+1, 0),
                (+1, 0) => (0, +1),
                _ => throw new InvalidOperationException(),
            };

            (int X, int Y) TurnRight() => direction switch
            {
                (0, +1) => (+1, 0),
                (+1, 0) => (0, -1),
                (0, -1) => (-1, 0),
                (-1, 0) => (0, +1),
                _ => throw new InvalidOperationException(),
            };

            void Print()
            {
                var min = whiteCells.Aggregate((X: int.MaxValue, Y: int.MaxValue), (cur, next) => (Math.Min(cur.X, next.X), Math.Min(cur.Y, next.Y)));
                var max = whiteCells.Aggregate((X: int.MinValue, Y: int.MinValue), (cur, next) => (Math.Max(cur.X, next.X), Math.Max(cur.Y, next.Y)));

                for (int y = max.Y; y >= min.Y; y--)
                {
                    for (int x = min.X; x <= max.X; x++)
                    {
                        Console.Write(whiteCells.Contains((x, y)) ? '#' : ' ');
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
